using System.Reflection;
using Avalanche.Application;
using Avalanche.Application.Grpc;
using Avalanche.Identity.Client;
using Avalanche.Vault.Application;
using Avalanche.Vault.Server.Services;
using Microsoft.EntityFrameworkCore;

var assembly = Assembly.GetExecutingAssembly();

AvalancheApplication.New(args, builder =>
{
    builder.SetupAvalancheGrpc();

    builder.Services.AddDbContext<AvalancheVaultContext>(m =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Npgsql");

        m.UseNpgsql(connectionString, r => { r.MigrationsAssembly(assembly.FullName); });
    });

    builder.UseAvalanchePassport();

    builder.UseAvalancheIdentityClient();

    var application = builder.Build();

    application.UseAvalancheRequestLogging();

    application.UseRouting();

    application.UseAvalancheIdentityClient();

    application.UseAvalancheGrpc();

    application.MapGrpcService<TicketService>();

    {
        using var scope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        using var context = scope.ServiceProvider.GetService<AvalancheVaultContext>();

        context?.Database.Migrate();
    }

    application.Run();
});