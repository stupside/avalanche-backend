using System.Reflection;
using Avalanche.Application;
using Avalanche.Vault.Application.Consumers;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Avalanche.Vault.Application;

public static class WebApplicationBuilderExtension
{
    public static void UseAvalanchePassport(this WebApplicationBuilder builder)
    {
        builder.Services.UseAvalanche(m => { m.UseFeatures(Assembly.GetExecutingAssembly()); });
        
        builder.Services.AddTransient<ChallengerConsumer>();
        builder.Services.AddTransient<MerchantConsumer>();

        builder.Services.AddCap(m =>
        {
            m.UseEntityFramework<AvalancheVaultContext>();

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