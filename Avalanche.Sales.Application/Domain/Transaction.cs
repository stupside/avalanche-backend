namespace Avalanche.Sales.Application.Domain;

public sealed class Transaction
{
    public Guid Id { get; init; }
    
    public required uint AmountReceived { get; init; }
    
    public required Guid OrderId { get; init; }
}