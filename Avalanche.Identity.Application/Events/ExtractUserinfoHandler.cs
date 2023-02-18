using OpenIddict.Abstractions;
using OpenIddict.Server;

namespace Avalanche.Identity.Application.Events;

public class ExtractUserinfoHandler : IOpenIddictServerHandler<OpenIddictServerEvents.ExtractUserinfoRequestContext>
{
    private readonly IOpenIddictScopeManager _scopes;

    public ExtractUserinfoHandler(IOpenIddictScopeManager scopes)
    {
        _scopes = scopes;
    }

    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ExtractUserinfoRequestContext>()
            .UseScopedHandler<ExtractUserinfoHandler>()
            .SetType(OpenIddictServerHandlerType.Custom)
            .Build();

    public ValueTask HandleAsync(OpenIddictServerEvents.ExtractUserinfoRequestContext context)
    {
        return ValueTask.CompletedTask;
    }
}