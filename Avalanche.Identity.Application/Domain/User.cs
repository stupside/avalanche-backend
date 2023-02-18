namespace Avalanche.Identity.Application.Domain;

public class User
{
    public Guid Id { get; init; }

    public required string Username { get; init; }

    public Guid UserCredentialId { get; init; }
    public UserCredential? UserCredential { get; init; }
}