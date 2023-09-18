using Avalanche.Licence.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Licence.Application;

public class AvalancheLicenceContext : DbContext
{
    public AvalancheLicenceContext(DbContextOptions<AvalancheLicenceContext> options) : base(options)
    {
    }

    public required DbSet<Validity> Validities { get; init; }
}