using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Avalanche.Application;

public class AvalancheApplicationBuilder
{
    private readonly IServiceCollection _services;

    public AvalancheApplicationBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public void UseFeatures(Assembly assembly)
    {
        _services.AddValidatorsFromAssemblies(new[]
        {
            typeof(AvalancheApplicationBuilder).Assembly,
            assembly
        });

        _services.AddMediatR(m =>
        {
            m.RegisterServicesFromAssemblies(typeof(AvalancheApplicationBuilder).Assembly, assembly);
        });
    }
}