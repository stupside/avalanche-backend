using System.Security.Claims;
using Avalanche.Identity.Application.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server;

namespace Avalanche.Identity.Application.Events;

public class TokenHandler : IOpenIddictServerHandler<OpenIddictServerEvents.HandleTokenRequestContext>
{
    private readonly IUserService _users;

    private readonly IOpenIddictScopeManager _scopes;
    private readonly IOpenIddictApplicationManager _applications;


    public TokenHandler(IUserService users, IOpenIddictScopeManager scopes, IOpenIddictApplicationManager applications)
    {
        _users = users;
        _scopes = scopes;
        _applications = applications;
    }

    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.HandleTokenRequestContext>()
            .UseScopedHandler<TokenHandler>()
            .SetType(OpenIddictServerHandlerType.Custom)
            .Build();

    public async ValueTask HandleAsync(OpenIddictServerEvents.HandleTokenRequestContext context)
    {
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        if (context.Request.IsPasswordGrantType())
        {
            if (context.Request.Username is null || context.Request.Password is null)
            {
                context.Reject(OpenIddictConstants.Errors.InvalidRequest);

                return;
            }

            var user = await _users.FindByUsername(context.Request.Username);

            if (user is null)
            {
                context.Reject(OpenIddictConstants.Errors.AccessDenied);

                return;
            }

            if (await _users.VerifyAsync(user, context.Request.Password))
            {
                identity.SetScopes(context.Request.GetScopes());

                var subject = user.Id.ToString();

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
            else
            {
                context.Reject(OpenIddictConstants.Errors.AccessDenied);
            }
        }
        else if (context.Request.IsClientCredentialsGrantType())
        {
            var client = context.Request.ClientId;

            if (client is null)
                throw new ArgumentException();

            var application = await _applications.FindByClientIdAsync(client);

            if (application is null)
                throw new ArgumentException();

            identity.SetClaim(OpenIddictConstants.Claims.Subject, await _applications.GetClientIdAsync(application));
            identity.SetClaim(OpenIddictConstants.Claims.Name, await _applications.GetDisplayNameAsync(application));

            identity.SetScopes(context.Request.GetScopes());

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
        else if (context.Request.IsAuthorizationCodeFlow() || context.Request.IsRefreshTokenGrantType())
        {
            var request = context.Transaction.GetHttpRequest();

            var authenticate =
                await request!.HttpContext.AuthenticateAsync(request.Scheme);

            if (authenticate.Principal is null)
            {
                context.Reject(OpenIddictConstants.Errors.AccessDenied);

                return;
            }

            context.SignIn(authenticate.Principal);
        }
    }
}