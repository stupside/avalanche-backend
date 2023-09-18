namespace Avalanche.Aggregator.Application.Domain;

public sealed class Record
{
    public Guid Id { get; init; }
    
    public Guid? UserId { get; set; }
    
    public bool? Succeed { get; set; }
    public string? Message { get; set; }
    
    public required Guid ServerId { get; init; }
}