namespace Avalanche.Licence.Application.Domain;

public sealed class Validity
{
    public Guid Id { get; init; }
    
    public required Guid OrganizationId { get; init; }
    public required Guid UserId { get; init; }

    public required DateTimeOffset From { get; init; }
    public required DateTimeOffset To { get; init; }
}