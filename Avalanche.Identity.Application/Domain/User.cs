namespace Avalanche.Identity.Application.Domain;

public class User
{
    public Guid Id { get; init; }

    public required string Username { get; init; }

    public required string Hash { get; set; }
}