namespace Avalanche.Merchant.Events;

[Serializable]
public record OrderCompleted
{
    public required Guid StoreId { get; init; }
    public required Guid UserId { get; init; }

    public required DateTimeOffset Availability { get; init; }
    public required TimeSpan AvailabilitySpan { get; init; }
}