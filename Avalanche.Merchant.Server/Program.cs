using System.Reflection;
using Avalanche.Application;
using Avalanche.Application.Grpc;
using Avalanche.Identity.Client;
using Avalanche.Merchant.Application;
using Avalanche.Merchant.Server.Services;
using Microsoft.EntityFrameworkCore;

var assembly = Assembly.GetExecutingAssembly();

AvalancheApplication.New(args, builder =>
{
    builder.SetupAvalancheGrpc();

    builder.Services.AddDbContext<AvalancheMerchantContext>(m =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Npgsql");

        m.UseNpgsql(connectionString, r => { r.MigrationsAssembly(assembly.FullName); });
    });

    builder.UseAvalancheIdentityClient();

    builder.UseAvalancheMerchant();

    var application = builder.Build();

    application.UseAvalancheRequestLogging();

    application.UseRouting();

    application.UseAvalancheIdentityClient();

    application.UseAvalancheGrpc();

    application.MapGrpcService<PlanService>();
    application.MapGrpcService<StoreService>();

    {
        using var scope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        using var context = scope.ServiceProvider.GetService<AvalancheMerchantContext>();

        context?.Database.Migrate();
    }

    application.Run();
});