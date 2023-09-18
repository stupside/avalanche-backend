using Avalanche.Identity.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Identity.Application;

public class IdentityContext : DbContext
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }

    public required DbSet<User> Users { get; init; }
}