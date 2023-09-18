namespace Avalanche.Sales.Application.Domain;

public sealed class Catalog
{
    public Guid Id { get; init; }

    public required Guid OrganizationId { get; init; }

    public IEnumerable<Plan>? Plans { get; init; }
}