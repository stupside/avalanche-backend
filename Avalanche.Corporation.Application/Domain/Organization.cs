namespace Avalanche.Corporation.Application.Domain;

public sealed class Organization
{
    public Guid Id { get; init; }
    
    public required Guid UserId { get; init; }
    
    public required string Name { get; init; }
    
    public const int NameMaxLenght = 40;
    public const int NameMinLenght = 5;
    
    public IEnumerable<Member>? Members { get; init; }
    public IEnumerable<Invitation>? Invitations { get; init; }
}