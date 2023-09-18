using Avalanche.Sales.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Sales.Application;

public class SalesContext : DbContext
{
    public SalesContext(DbContextOptions<SalesContext> options) : base(options)
    {
    }

    public required DbSet<Catalog> Catalogs { get; init; }
    public required DbSet<Order> Orders { get; init; }
    public required DbSet<Plan> Plans { get; init; }
    public required DbSet<Transaction> Transactions { get; init; }
}