using OpenIddict.Client;

namespace Avalanche.Gateway.Identity;

[Serializable]
public sealed class ClientIdentityConfiguration
{
    public const string AvalancheApplicationOidcKey = "Identity";
    
    public required OpenIddictClientRegistration ClientRegistration { get; init; }

    [Serializable]
    public sealed class OpenIddictClientIntrospection
    {
        public required string Issuer { get; init; }
        public required string ClientId { get; init; }
        public required string ClientSecret { get; init; }
        public required IEnumerable<string> Audiences { get; init; }
    }

    public required OpenIddictClientIntrospection ClientIntrospection { get; init; }
}