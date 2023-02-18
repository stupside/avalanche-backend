using System.Reflection;
using Avalanche.Application;
using Avalanche.Application.Grpc;
using Avalanche.Drm.Application;
using Avalanche.Drm.Server.Services;
using Avalanche.Identity.Client;
using Microsoft.EntityFrameworkCore;

var assembly = Assembly.GetExecutingAssembly();

AvalancheApplication.New(args, builder =>
{
    builder.SetupAvalancheGrpc();

    builder.Services.AddDbContext<AvalancheDrmContext>(m =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Npgsql");

        m.UseNpgsql(connectionString, r => { r.MigrationsAssembly(assembly.FullName); });
    });
    
    builder.UseAvalancheIdentityClient();

    builder.UseAvalancheDrm();

    var application = builder.Build();

    application.UseAvalancheRequestLogging();

    application.UseRouting();
    
    application.UseAvalancheIdentityClient();

    application.UseAvalancheGrpc();

    application.MapGrpcService<AuthService>();
    
    {
        using var scope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        using var context = scope.ServiceProvider.GetService<AvalancheDrmContext>();

        context?.Database.Migrate();
    }

    application.Run();
});