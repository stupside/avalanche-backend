using System.Reflection;
using Avalanche.Application;
using Avalanche.Gateway.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using OpenIddict.Validation.AspNetCore;

AvalancheApplication.New(args, builder =>
{
    builder.WebHost.ConfigureKestrel(k =>
    {
        k.ConfigureHttpsDefaults(h =>
        {
            h.ClientCertificateMode = ClientCertificateMode.RequireCertificate;

            h.UseLettuceEncrypt(k.ApplicationServices);
        });
    });
    
    builder.Services.AddLettuceEncrypt();
    
    builder.Services.AddAuthentication(m =>
    {
        m.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        m.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        m.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    });
    
    builder.Services.AddAuthorization();
    
    var configuration = builder.Configuration
        .GetSection(ClientIdentityConfiguration.AvalancheApplicationOidcKey)
        .Get<ClientIdentityConfiguration>();
    
    if (configuration is null) throw new NullReferenceException();

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

    builder.Services.AddControllers();

    builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    
    var application = builder.Build();

    application.UseRouting();

    application.UseAuthentication();
    application.UseAuthorization();
    
    application.MapReverseProxy(proxy =>
    {
        proxy.UseForwardedHeaders();
        
        proxy.UseSessionAffinity();

        proxy.UseLoadBalancing();
        proxy.UsePassiveHealthChecks();
    });

    application.Run();
});