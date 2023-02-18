namespace Avalanche.Vault.Application.Domain;

public sealed class Ticket
{
    public Guid Id { get; set; }

    public required Guid StoreId { get; init; }

    public required Guid UserId { get; init; }
    
    public required IEnumerable<Validity> Validities { get; init; }
}