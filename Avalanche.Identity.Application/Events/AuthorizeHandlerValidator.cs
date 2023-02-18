using OpenIddict.Abstractions;
using OpenIddict.Server;

namespace Avalanche.Identity.Application.Events;

public class
    AuthorizeHandlerValidator : IOpenIddictServerHandler<OpenIddictServerEvents.ValidateAuthorizationRequestContext>
{
    private readonly IOpenIddictApplicationManager _applications;

    public AuthorizeHandlerValidator(IOpenIddictApplicationManager applications)
    {
        _applications = applications;
    }

    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ValidateAuthorizationRequestContext>()
            .UseScopedHandler<AuthorizeHandlerValidator>()
            .SetType(OpenIddictServerHandlerType.Custom)
            .Build();

    public async ValueTask HandleAsync(OpenIddictServerEvents.ValidateAuthorizationRequestContext context)
    {
        var request = context.Request;

        if (request.ClientId is null)
        {
            context.Reject(OpenIddictConstants.Errors.InvalidClient);

            return;
        }

        var client = await _applications.FindByClientIdAsync(request.ClientId);

        if (client is null)
        {
            context.Reject(OpenIddictConstants.Errors.InvalidClient);
        }
    }
}