using Avalanche.Vault.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Vault.Application;

public class AvalancheVaultContext : DbContext
{
    public AvalancheVaultContext(DbContextOptions<AvalancheVaultContext> options) : base(options)
    {
    }

    public required DbSet<Ticket> Tickets { get; init; }
    public required DbSet<Validity> Validities { get; init; }
}