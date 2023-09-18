using OpenIddict.Server;

namespace Avalanche.Identity.Application.Events;

public class ExtractUserinfoHandler : IOpenIddictServerHandler<OpenIddictServerEvents.ExtractUserinfoRequestContext>
{
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