namespace Avalanche.Corporation.Application.Domain;

public sealed class Invitation
{
    public Guid Id { get; init; }
    
    public required string Email { get; init; }
    
    public enum InvitationStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    public required InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    
    public required Guid OrganizationId { get; init; }
    
    public Guid? MemberId { get; init; }
}