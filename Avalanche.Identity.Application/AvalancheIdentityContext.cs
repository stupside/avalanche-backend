using Avalanche.Identity.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Identity.Application;

public class AvalancheIdentityContext : DbContext
{
    public AvalancheIdentityContext(DbContextOptions<AvalancheIdentityContext> options) : base(options)
    {
    }

    public required DbSet<User> Users { get; init; }

    public required DbSet<UserCredential> UserCredentials { get; init; }
}