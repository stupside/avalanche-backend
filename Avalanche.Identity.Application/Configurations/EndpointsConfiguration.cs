namespace Avalanche.Identity.Application.Configurations;

[Serializable]
public sealed class EndpointsConfiguration
{
    public required string Introspection { get; init; }
    public required string Authorize { get; init; }
    public required string Token { get; init; }
    public required string Userinfo { get; init; }
}