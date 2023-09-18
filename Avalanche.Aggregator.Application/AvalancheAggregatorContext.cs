using Avalanche.Aggregator.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Aggregator.Application;

public class AvalancheAggregatorContext : DbContext
{
    public AvalancheAggregatorContext(DbContextOptions<AvalancheAggregatorContext> options) : base(options)
    {
    }

    public required DbSet<Server> Servers { get; init; }
    public required DbSet<Record> Records { get; init; }
}