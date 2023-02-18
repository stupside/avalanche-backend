using System.Text.Json;
using Avalanche.Merchant.Plan;
using Avalanche.Merchant.Store;
using Avalanche.Seed;
using Bogus;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Client;

var services = new ServiceCollection();

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var seed = configuration.GetSection(Seed.SeedKey).Get<Seed>();

if (seed is null) throw new Exception();

services.AddOpenIddict()
    .AddClient(options =>
    {
        options.AllowPasswordFlow();

        options.DisableTokenStorage();

        options.UseSystemNetHttp()
            .SetProductInformation(typeof(Program).Assembly);

        options.AddRegistration(seed.Backend.Identity);
    });

static async Task CreateAccountAsync(IServiceProvider provider, string identity, string username, string password)
{
    using var client = provider.GetRequiredService<HttpClient>();

    var response = await client.PostAsync($"{identity}accounts?username={username}&password={password}", null);

    response.EnsureSuccessStatusCode();
}

static async Task<string?> GetTokenAsync(IServiceProvider provider, string identity, string email, string password,
    IEnumerable<string> scopes)
{
    var service = provider.GetRequiredService<OpenIddictClientService>();

    var (response, _) = await service.AuthenticateWithPasswordAsync(
        issuer: new Uri(identity, UriKind.Absolute),
        scopes: scopes.ToArray(),
        username: email,
        password: password);

    return response.AccessToken;
}

await using var provider = services.BuildServiceProvider();

var tokens = new List<Metadata>();

for (var i = 0; i <= seed.Users.GenerateUsers; i++)
{
    var faker = new Faker();

    var username = faker.Person.UserName;

    const string password = "password";

    await CreateAccountAsync(provider, seed.Backend.Identity.Issuer!.ToString(), username, password);

    var token = await GetTokenAsync(provider, seed.Backend.Identity.Issuer!.ToString(), username, password,
        seed.Backend.Identity.Scopes);

    if (token is not null)
    {
        var metadata = new Metadata { { "Authorization", $"Bearer {token}" } };

        tokens.Add(metadata);
    }

    Console.WriteLine("Access token for {0}:{1} {2}", username, password, token);
}

var channel = GrpcChannel.ForAddress(seed.Backend.Gateway);

var storeService = new StoreService.StoreServiceClient(channel);
var planService = new PlanService.PlanServiceClient(channel);

var plans = new List<string>();

foreach (var token in tokens)
{
    var faker = new Faker();

    for (var i = 0; i <= seed.Stores.GenerateStores; i++)
    {
        var name = faker.Company.CompanyName();
        var description = faker.Commerce.ProductDescription();
        var email = faker.Person.Email;

        var createStoreRequest = new CreateStoreRpc.Types.Command
        {
            Name = name,
            Description = description,
            Email = email
        };

        try
        {
            var createStoreResponse = storeService.Create(createStoreRequest, token);

            Console.WriteLine("Created new store");
            Console.WriteLine(JsonSerializer.Serialize(createStoreRequest));
            Console.WriteLine();

            for (var j = 0; j <= faker.Random.UInt(1, seed.Stores.GenerateMaxPlans); j++)
            {
                var createPlanRequest = new CreatePlanRpc.Types.Command
                {
                    StoreId = createStoreResponse.StoreId,

                    Name = faker.Commerce.ProductName(),
                    Validity = TimeSpan.FromDays(faker.Random.UInt(0, 25)).ToDuration(),
                    Price = faker.Random.UInt(100, 1000)
                };

                try
                {
                    var createPlanResponse = planService.Create(createPlanRequest, token);

                    Console.WriteLine("Created new plan");
                    Console.WriteLine(JsonSerializer.Serialize(createPlanRequest));
                    Console.WriteLine();

                    plans.Add(createPlanResponse.PlanId);
                }
                catch
                {
                    // ignored
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}

foreach (var token in tokens)
{
    var faker = new Faker();

    foreach (var plan in faker.Random.ListItems(plans, (int)faker.Random.UInt(1, seed.Stores.GenerateMaxTickets)))
    {
        var orderPlanRequest = new OrderPlanRpc.Types.Command
        {
            PlanId = plan,
            AvailableInDays = faker.Random.UInt(0, 10)
        };

        planService.Order(orderPlanRequest, token);

        Console.WriteLine("Ordered plan");

        Console.WriteLine(JsonSerializer.Serialize(orderPlanRequest));
        Console.WriteLine();
    }
}