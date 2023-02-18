using Avalanche.Application;
using Avalanche.Drm.Application.Flow;
using Avalanche.Drm.Application.Flow.Channels;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Avalanche.Drm.Application;

public static class WebApplicationBuilderExtension
{
    public static void UseAvalancheDrm(this WebApplicationBuilder builder)
    {
        builder.Services.UseAvalanche(m => { m.UseFeatures(typeof(WebApplicationBuilderExtension).Assembly); });
        
        builder.Services.AddTransient<TestConsumer>();
        
        builder.Services.AddSingleton<TestChannelManager>();

        builder.Services.AddCap(m =>
        {
            m.UseEntityFramework<AvalancheDrmContext>();

            m.UseRabbitMQ(r =>
            {
                var rabbitmq = builder.Configuration.GetSection(nameof(RabbitMQOptions)).Get<RabbitMQOptions>() ??
                               throw new NullReferenceException();

                r.HostName = rabbitmq.HostName;

                r.Port = rabbitmq.Port;

                r.VirtualHost = rabbitmq.VirtualHost;

                r.UserName = rabbitmq.UserName;
                r.Password = rabbitmq.Password;
            });
        });
    }
}