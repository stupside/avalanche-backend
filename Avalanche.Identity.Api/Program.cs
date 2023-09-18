using System.Reflection;
using Avalanche.Application;
using Avalanche.Application.Http;
using Avalanche.Identity.Application;
using Avalanche.Identity.Application.Configurations;
using Avalanche.Identity.Application.Events;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;

var assembly = Assembly.GetExecutingAssembly();

AvalancheApplication.New(args, builder =>
{
    var oidc = builder.Configuration.GetSection(OidcConfiguration.AvalancheOidcConfigurationKey)
        .Get<OidcConfiguration>();

    if (oidc is null)
        throw new Exception();

    builder.Services.AddRouting();

    builder.Services.AddOpenIddict()
        .AddCore(options => { options.UseEntityFrameworkCore(m => { m.UseDbContext<IdentityContext>(); }); })
        .AddServer(options =>
        {
            if (oidc.Issuer is not null)
                options.SetIssuer(oidc.Issuer);

            options.SetIntrospectionEndpointUris(oidc.Endpoints.Introspection);

            options.SetAuthorizationEndpointUris(oidc.Endpoints.Authorize);
            options.AddEventHandler(AuthorizeHandlerValidator.Descriptor);
            options.AddEventHandler(AuthorizeHandler.Descriptor);

            options.SetTokenEndpointUris(oidc.Endpoints.Token);
            options.AddEventHandler(TokenHandlerValidator.Descriptor);
            options.AddEventHandler(TokenHandler.Descriptor);

            options.SetUserinfoEndpointUris(oidc.Endpoints.Userinfo);
            options.AddEventHandler(ExtractUserinfoHandler.Descriptor);

            options.SetConfigurationEndpointUris("/.well-known/openid-configuration");

            options
                .AllowAuthorizationCodeFlow()
                .AllowRefreshTokenFlow()
                .AllowPasswordFlow()
                .AllowClientCredentialsFlow();

            if (builder.Environment.IsDevelopment() is false)
                options
                    .UseReferenceAccessTokens()
                    .UseReferenceRefreshTokens();

            if (oidc.Scopes is not null)
                options.RegisterScopes(oidc.Scopes.ToArray());

            if (oidc.Claims is not null)
                options.RegisterClaims(oidc.Claims.ToArray());

            options.SetAccessTokenLifetime(oidc.TokenLifetime.AccessToken);
            options.SetRefreshTokenLifetime(oidc.TokenLifetime.RefreshToken);

            options.AddEphemeralEncryptionKey();
            options.AddEphemeralSigningKey();

            if (builder.Environment.IsDevelopment())
                options.DisableAccessTokenEncryption();

            options.Configure(m => { m.TokenValidationParameters.ValidIssuers = oidc.ValidIssuers; });

            options.UseAspNetCore(m =>
            {
                m.DisableTransportSecurityRequirement();

                m.EnableAuthorizationRequestCaching();
            });
        })
        .AddValidation(options =>
        {
            options.UseSystemNetHttp();

            options.UseLocalServer();

            options.UseAspNetCore();
        });

    builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

    builder.Services.AddAuthorization();

    builder.UseAvalancheApi();

    builder.Services.AddDbContext<IdentityContext>(m =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Npgsql");

        m.UseNpgsql(connectionString, r => { r.MigrationsAssembly(assembly.FullName); });

        m.UseOpenIddict();
    });

    builder.UseAvalancheIdentity();

    var application = builder.Build();

    application.UseAvalancheRequestLogging();

    application.UseRouting();

    application.UseAvalancheApi();

    application.UseAuthentication();
    application.UseAuthorization();

    if (application.Environment.IsDevelopment() is false)
    {
        using var scope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        using var context = scope.ServiceProvider.GetService<IdentityContext>();

        context?.Database.Migrate();
    }

    application.Run();
});