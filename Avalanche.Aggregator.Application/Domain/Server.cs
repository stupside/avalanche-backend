namespace Avalanche.Aggregator.Application.Domain;

public class Server
{
    public Guid Id { get; init; }
    
    public required Guid OrganizationId { get; init; }
    
    public IEnumerable<Record>? Records { get; init; }
}