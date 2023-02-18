using Avalanche.Drm.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Drm.Application;

public class AvalancheDrmContext : DbContext
{
    public AvalancheDrmContext(DbContextOptions<AvalancheDrmContext> options) : base(options)
    {
    }

    public required DbSet<Challenge> Challenges { get; init; }
}