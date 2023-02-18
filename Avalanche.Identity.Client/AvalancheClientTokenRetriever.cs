using Microsoft.Extensions.Configuration;
using OpenIddict.Client;

namespace Avalanche.Identity.Client;

public class AvalancheClientTokenRetriever
{
    private readonly OpenIddictClientService _client;
    private readonly AvalancheClientConfiguration _configuration;

    public AvalancheClientTokenRetriever(OpenIddictClientService client, IConfiguration configuration)
    {
        _client = client;

        var section =
            configuration.GetRequiredSection(AvalancheClientConfiguration.AvalancheApplicationOidcKey);

        _configuration = section.Get<AvalancheClientConfiguration>() ?? throw new InvalidOperationException();
    }

    public async ValueTask<string?> GetAccessTokenForCurrentService(IEnumerable<string> scopes)
    {
        var issuer = _configuration.ClientRegistration.Issuer;

        if (issuer is null) return null;

        var (response, _) =
            await _client.AuthenticateWithClientCredentialsAsync(issuer, scopes.ToArray());

        return response.AccessToken;
    }
}