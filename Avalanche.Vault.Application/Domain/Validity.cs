namespace Avalanche.Vault.Application.Domain;

public sealed class Validity
{
    public Guid Id { get; init; }

    public required DateTimeOffset From { get; init; }
    public required DateTimeOffset To { get; init; }

    public required Guid TicketId { get; set; }
}