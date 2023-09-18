using System.Reflection;
using Avalanche.Application;
using Avalanche.Corporation.Application;
using Microsoft.EntityFrameworkCore;

var assembly = Assembly.GetExecutingAssembly();

AvalancheApplication.New(args, builder =>
{
    builder.Services.AddDbContext<CorporationContext>(m =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Npgsql");

        m.UseNpgsql(connectionString, r => { r.MigrationsAssembly(assembly.FullName); });
    });
    
    builder.UseBusiness();

    var application = builder.Build();

    application.UseAvalancheRequestLogging();

    application.UseRouting();
    
    {
        using var scope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        using var context = scope.ServiceProvider.GetService<CorporationContext>();

        context?.Database.Migrate();
    }

    application.Run();
});