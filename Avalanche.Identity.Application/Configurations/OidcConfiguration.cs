using OpenIddict.Abstractions;

namespace Avalanche.Identity.Application.Configurations;

[Serializable]
public sealed class OidcConfiguration
{
    public const string AvalancheOidcConfigurationKey = "Identity";

    public Uri? Issuer { get; init; }

    public required IEnumerable<string> ValidIssuers { get; init; }

    public required IEnumerable<string>? Scopes { get; init; }

    public required IEnumerable<OpenIddictScopeDescriptor> CustomScopes { get; init; }

    public required IEnumerable<string>? Claims { get; init; }

    public required TokenLifetimeConfiguration TokenLifetime { get; init; }
    public required EndpointsConfiguration Endpoints { get; init; }

    public required IEnumerable<OpenIddictApplicationDescriptor> Applications { get; init; }
}