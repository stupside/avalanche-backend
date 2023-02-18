using Avalanche.Application;
using Avalanche.Identity.Client;
using Microsoft.AspNetCore.Server.Kestrel.Https;

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

    builder.UseAvalancheIdentityClient();

    builder.Services.AddControllers();

    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    builder.Services.AddLettuceEncrypt();

    var application = builder.Build();

    application.UseRouting();

    application.UseAvalancheIdentityClient();

    application.MapReverseProxy(proxy =>
    {
        proxy.UseForwardedHeaders();

        proxy.UseSessionAffinity();

        proxy.UseLoadBalancing();
        proxy.UsePassiveHealthChecks();
    });

    application.Run();
});