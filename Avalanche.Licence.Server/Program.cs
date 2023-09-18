using System.Reflection;
using Avalanche.Application;
using Avalanche.Application.Grpc;
using Avalanche.Licence.Application;
using Avalanche.Licence.Server.Rpc;
using Microsoft.EntityFrameworkCore;

var assembly = Assembly.GetExecutingAssembly();

AvalancheApplication.New(args, builder =>
{
    builder.SetupAvalancheGrpc();

    builder.Services.AddDbContext<AvalancheLicenceContext>(m =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Npgsql");

        m.UseNpgsql(connectionString, r => { r.MigrationsAssembly(assembly.FullName); });
    });

    builder.UseCalendar();
    
    var application = builder.Build();

    application.UseAvalancheRequestLogging();

    application.UseRouting();
    
    application.UseAvalancheGrpc();

    application.MapGrpcService<ValidityRpc>();

    {
        using var scope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        using var context = scope.ServiceProvider.GetService<AvalancheLicenceContext>();

        context?.Database.Migrate();
    }

    application.Run();
});