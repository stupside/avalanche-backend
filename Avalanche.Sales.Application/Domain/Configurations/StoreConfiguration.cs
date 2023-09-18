using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avalanche.Sales.Application.Domain.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Catalog>
{
    public void Configure(EntityTypeBuilder<Catalog> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.OrganizationId).IsRequired();

        builder.HasMany(m => m.Plans)
            .WithOne()
            .HasForeignKey(m => m.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}