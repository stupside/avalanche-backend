using System.Reflection;
using Avalanche.Aggregator.Application;
using Avalanche.Application;
using Avalanche.Application.Grpc;
using Avalanche.Aggregator.Server.Services;
using Microsoft.EntityFrameworkCore;

var assembly = Assembly.GetExecutingAssembly();

AvalancheApplication.New(args, builder =>
{
    builder.SetupAvalancheGrpc();

    builder.Services.AddDbContext<AvalancheAggregatorContext>(m =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Npgsql");

        m.UseNpgsql(connectionString, r => { r.MigrationsAssembly(assembly.FullName); });
    });
    
    builder.UseDrm();

    var application = builder.Build();

    application.UseAvalancheRequestLogging();

    application.UseRouting();
    
    application.UseAvalancheGrpc();

    application.MapGrpcService<AuthServerRpc>();
    
    {
        using var scope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        using var context = scope.ServiceProvider.GetService<AvalancheAggregatorContext>();

        context?.Database.Migrate();
    }

    application.Run();
});