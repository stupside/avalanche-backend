using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avalanche.Merchant.Application.Domain.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasIndex(m => m.Name).IsUnique();

        builder.Property(m => m.UserId).IsRequired();

        builder.Property(m => m.Name).IsRequired();
        builder.Property(m => m.Description).IsRequired();

        builder.Property(m => m.Email).IsRequired();

        builder.HasMany(m => m.Plans)
            .WithOne()
            .HasForeignKey(m => m.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}