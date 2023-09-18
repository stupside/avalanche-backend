namespace Avalanche.Sales.Application.Domain;

public sealed class Order
{
    public Guid Id { get; set; }

    public required Guid PlanId { get; init; }
    public required Guid UserId { get; init; }

    public required uint AmountDue { get; init; }
    
    public required DateTimeOffset Availability { get; init; }
    public required TimeSpan AvailabilitySpan { get; init; }

    public IEnumerable<Transaction>? Transactions { get; init; }
}