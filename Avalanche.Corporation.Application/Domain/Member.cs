namespace Avalanche.Corporation.Application.Domain;

public sealed class Member
{
    public Guid Id { get; init; }
    
    public required Guid UserId { get; init; }
    
    public required Guid OrganizationId { get; init; }
}