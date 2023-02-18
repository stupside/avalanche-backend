using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avalanche.Merchant.Application.Domain.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name).IsRequired();

        builder.Property(m => m.Price).IsRequired();

        builder.Property(m => m.AvailabilitySpan).IsRequired();

        builder.HasIndex(m => new
        {
            m.StoreId,
            m.Name
        }).IsUnique();
    }
}