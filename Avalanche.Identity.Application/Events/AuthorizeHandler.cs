using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server;

namespace Avalanche.Identity.Application.Events;

public class AuthorizeHandler : IOpenIddictServerHandler<OpenIddictServerEvents.HandleAuthorizationRequestContext>
{
    private readonly IOpenIddictScopeManager _scopes;

    public AuthorizeHandler(IOpenIddictScopeManager scopes)
    {
        _scopes = scopes;
    }

    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.HandleAuthorizationRequestContext>()
            .UseScopedHandler<AuthorizeHandler>()
            .SetType(OpenIddictServerHandlerType.Custom)
            .Build();

    public async ValueTask HandleAsync(OpenIddictServerEvents.HandleAuthorizationRequestContext context)
    {
        var request = context.Transaction.GetHttpRequest();

        if (request is null)
        {
            context.Reject(OpenIddictConstants.Errors.InvalidRequest);

            return;
        }

        var authenticate =
            await request.HttpContext.AuthenticateAsync(request.Scheme);

        if (authenticate.Succeeded is false)
        {
            var authority = request.PathBase;
            var route = request.Path;

            var query = QueryString.Create(
                request.HasFormContentType ? request.Form.ToList() : request.Query.ToList()
            );

            var endpoint = authority + route;

            var redirection = endpoint.Add(query);

            if (true) // TODO: await _applicationManager.ValidateRedirectUriAsync(, endpoint)
            {
                var properties = new AuthenticationProperties { RedirectUri = redirection };

                await request.HttpContext.ChallengeAsync(request.Scheme, properties);

                context.HandleRequest();

                return;
            }
        }

        var subject = authenticate.Principal.GetClaim(OpenIddictConstants.Claims.Subject);

        if (subject is null)
        {
            context.Reject(OpenIddictConstants.Errors.InvalidRequest);

            return;
        }

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role
        );

        identity.SetScopes(context.Request.GetScopes());

        identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, subject));

        var enumerator = _scopes.ListResourcesAsync(identity.GetScopes()).GetAsyncEnumerator();

        var resources = new List<string>();

        while (await enumerator.MoveNextAsync())
            resources.Add(enumerator.Current);

        identity.SetResources(resources);

        identity.SetDestinations(static claim => claim.Type switch
        {
            OpenIddictConstants.Claims.Name when (claim.Subject ?? throw new InvalidOperationException())
                .HasScope(OpenIddictConstants.Permissions.Scopes.Profile) => new[]
                {
                    OpenIddictConstants.Destinations.AccessToken,
                    OpenIddictConstants.Destinations.IdentityToken
                },
            _ => new[]
            {
                OpenIddictConstants.Destinations.AccessToken
            }
        });

        var principal = new ClaimsPrincipal(identity);

        context.SignIn(principal);
    }
}