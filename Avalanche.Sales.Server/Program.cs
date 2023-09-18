using System.Reflection;
using Avalanche.Application;
using Avalanche.Sales.Application;
using Microsoft.EntityFrameworkCore;

var assembly = Assembly.GetExecutingAssembly();

AvalancheApplication.New(args, builder =>
{
    builder.Services.AddDbContext<SalesContext>(m =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Npgsql");

        m.UseNpgsql(connectionString, r => { r.MigrationsAssembly(assembly.FullName); });
    });
    
    builder.UseSales();

    var application = builder.Build();

    application.UseAvalancheRequestLogging();

    application.UseRouting();
    
    {
        using var scope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        using var context = scope.ServiceProvider.GetService<SalesContext>();

        context?.Database.Migrate();
    }

    application.Run();
});