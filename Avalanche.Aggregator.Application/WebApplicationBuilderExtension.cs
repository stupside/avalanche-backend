using Avalanche.Aggregator.Application.Auth;
using Avalanche.Aggregator.Application.Auth.Handlers;
using Avalanche.Application;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Avalanche.Aggregator.Application;

public static class WebApplicationBuilderExtension
{
    public static void UseDrm(this WebApplicationBuilder builder)
    {
        builder.Services.UseAvalanche(m => { m.UseFeatures(typeof(WebApplicationBuilderExtension).Assembly); });

        builder.Services.AddSingleton<ChallengeManager>();

        builder.Services.AddScoped<IChallenge, ChallengeRate>();
        
        builder.Services.AddCap(m =>
        {
            m.UseEntityFramework<AvalancheAggregatorContext>();

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