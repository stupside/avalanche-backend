using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avalanche.Sales.Application.Domain.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasMany(m => m.Orders).WithOne().HasForeignKey(m => m.PlanId);

        builder.Property(m => m.Name).IsRequired();

        builder.Property(m => m.Price).IsRequired();

        builder.Property(m => m.AvailabilitySpan).IsRequired();

        builder.HasIndex(m => new
        {
            OrganizationId = m.CatalogId,
            m.Name
        }).IsUnique();
    }
}