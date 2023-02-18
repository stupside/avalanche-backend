namespace Avalanche.Merchant.Application.Domain;

public sealed class Plan
{
    public Guid Id { get; set; }

    public const int NameMaxLenght = 30;
    public const int NameMinLenght = 5;

    public required string Name { get; set; }

    public required uint Price { get; set; }
    public required TimeSpan AvailabilitySpan { get; set; }

    public required Guid StoreId { get; set; }
}