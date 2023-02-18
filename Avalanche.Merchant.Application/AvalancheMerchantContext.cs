using Avalanche.Merchant.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Merchant.Application;

public class AvalancheMerchantContext : DbContext
{
    public AvalancheMerchantContext(DbContextOptions<AvalancheMerchantContext> options) : base(options)
    {
    }

    public required DbSet<Plan> Plans { get; init; }
    public required DbSet<Store> Stores { get; init; }
}