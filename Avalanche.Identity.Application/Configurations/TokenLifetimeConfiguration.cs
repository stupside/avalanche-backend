namespace Avalanche.Identity.Application.Configurations;

[Serializable]
public sealed class TokenLifetimeConfiguration
{
    public required TimeSpan AccessToken { get; init; }
    public required TimeSpan RefreshToken { get; init; }
}