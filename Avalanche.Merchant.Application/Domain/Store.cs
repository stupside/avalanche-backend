namespace Avalanche.Merchant.Application.Domain;

public sealed class Store
{
    public Guid Id { get; init; }

    public required Guid UserId { get; init; }

    public const int NameMaxLenght = 40;
    public const int NameMinLenght = 5;

    public const int DescriptionMaxLenght = 150;
    public const int DescriptionMinLenght = 5;

    public required string Name { get; set; }
    public required string Description { get; set; }

    public string? Logo { get; set; }

    public required string Email { get; set; }

    public IEnumerable<Plan>? Plans { get; init; }
}