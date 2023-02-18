using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Avalanche.Application;

public static class AvalancheApplication
{
    public static void New(string[] args, Action<WebApplicationBuilder> action)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables(prefix: "Avalanche_");

        builder.Host.UseSerilog(Log.Logger);

        try
        {
            action(builder);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static void UseAvalanche(this IServiceCollection serviceCollection,
        Action<AvalancheApplicationBuilder> action)
    {
        action(new AvalancheApplicationBuilder(serviceCollection));
    }

    public static void UseAvalancheRequestLogging(this WebApplication application)
    {
        application.UseSerilogRequestLogging();
    }
}