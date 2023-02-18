namespace Avalanche.Drm.Application.Domain;

public sealed class Challenge
{
    public Guid Id { get; init; }

    public required Guid StoreId { get; init; }
    
    public Guid? UserId { get; set; }
    public Guid? TicketId { get; set; } 
}