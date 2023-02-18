using OpenIddict.Abstractions;

namespace Avalanche.Identity.Application.Configurations;

[Serializable]
public sealed class AvalancheOidcConfiguration
{
    public const string AvalancheOidcConfigurationKey = "Identity";

    [Serializable]
    public sealed class EndpointsConfiguration
    {
        public required string Introspection { get; init; }
        public required string Authorize { get; init; }
        public required string Token { get; init; }
        public required string Userinfo { get; init; }
    }

    [Serializable]
    public sealed class TokenLifetimeConfiguration
    {
        public required TimeSpan AccessToken { get; init; }
        public required TimeSpan RefreshToken { get; init; }
    }

    public Uri? Issuer { get; init; }

    public required IEnumerable<string> ValidIssuers { get; init; }

    public required IEnumerable<string>? Scopes { get; init; }

    public required IEnumerable<OpenIddictScopeDescriptor> CustomScopes { get; init; }

    public required IEnumerable<string>? Claims { get; init; }

    public required TokenLifetimeConfiguration TokenLifetime { get; init; }
    public required EndpointsConfiguration Endpoints { get; init; }

    public required IEnumerable<OpenIddictApplicationDescriptor> Applications { get; init; }
}