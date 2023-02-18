using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;

namespace Avalanche.Identity.Client;

public static class WebApplicationBuilderExtension
{
    public static void UseAvalancheIdentityClient(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration
            .GetSection(AvalancheClientConfiguration.AvalancheApplicationOidcKey)
            .Get<AvalancheClientConfiguration>();

        if (configuration is null) throw new NullReferenceException();

        builder.Services.AddAuthentication(m =>
        {
            m.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            m.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            m.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });

        builder.Services.AddAuthorization();

        builder.Services.AddOpenIddict()
            .AddClient(options =>
            {
                options.AllowClientCredentialsFlow();

                options.DisableTokenStorage();

                options.UseSystemNetHttp()
                    .SetProductInformation(Assembly.GetCallingAssembly());

                options.AddRegistration(configuration.ClientRegistration);
            })
            .AddValidation(options =>
            {
                options.SetIssuer(configuration.ClientIntrospection.Issuer);

                options.AddAudiences(configuration.ClientIntrospection.Audiences.ToArray());

                options.UseIntrospection()
                    .SetClientId(configuration.ClientIntrospection.ClientId)
                    .SetClientSecret(configuration.ClientIntrospection.ClientSecret);

                options.UseSystemNetHttp()
                    .SetProductInformation(Assembly.GetCallingAssembly());

                options.UseAspNetCore();
            });
    }
}