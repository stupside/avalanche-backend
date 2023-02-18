using Avalanche.Application;
using Avalanche.Identity.Application.Configurations;
using Avalanche.Identity.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Avalanche.Identity.Application;

public static class WebApplicationBuilderExtension
{
    public static void UseAvalancheIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<AvalancheIdentitySeed>();

        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.UseAvalanche(m => { m.UseFeatures(typeof(WebApplicationBuilderExtension).Assembly); });

        builder.Services.Configure<AvalancheOidcConfiguration>(
            builder.Configuration.GetSection(AvalancheOidcConfiguration.AvalancheOidcConfigurationKey));
    }
}