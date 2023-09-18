using Avalanche.Corporation.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Corporation.Application;

public class CorporationContext : DbContext
{
    public CorporationContext(DbContextOptions<CorporationContext> options) : base(options)
    {
    }
    
    public required DbSet<Organization> Organizations { get; init; }
    public required DbSet<Member> Members { get; init; }
    public required DbSet<Invitation> Invitations { get; init; }
}