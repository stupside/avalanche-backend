namespace Avalanche.Identity.Application.Domain;

public class UserCredential
{
    public Guid Id { get; init; }

    public required string Hash { get; set; }
}