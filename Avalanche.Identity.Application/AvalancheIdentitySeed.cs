using Avalanche.Identity.Application.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace Avalanche.Identity.Application;

public class AvalancheIdentitySeed : IHostedService
{
    private readonly AvalancheOidcConfiguration _configuration;

    private readonly IServiceProvider _serviceProvider;

    public AvalancheIdentitySeed(
        IOptions<AvalancheOidcConfiguration> options,
        IServiceProvider serviceProvider)
    {
        _configuration = options.Value;

        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<AvalancheIdentityContext>();

        await context.Database.EnsureCreatedAsync(cancellationToken);

        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        await CreateApplications(applicationManager, cancellationToken);

        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        await CreateScopes(scopeManager, cancellationToken);
    }

    private async Task CreateScopes(IOpenIddictScopeManager scopeManager, CancellationToken cancellationToken)
    {
        foreach (var descriptor in _configuration.CustomScopes)
        {
            if (descriptor.Name is null) throw new ArgumentException();

            if (await scopeManager.FindByNameAsync(descriptor.Name, cancellationToken) is not null)
                continue;

            await scopeManager.CreateAsync(descriptor, cancellationToken);
        }
    }

    private async Task CreateApplications(IOpenIddictApplicationManager applicationManager,
        CancellationToken cancellationToken)
    {
        foreach (var descriptor in _configuration.Applications)
        {
            if (descriptor.ClientId is null) throw new ArgumentException();

            if (await applicationManager.FindByClientIdAsync(descriptor.ClientId, cancellationToken) is not null)
                continue;

            await applicationManager.CreateAsync(descriptor, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}