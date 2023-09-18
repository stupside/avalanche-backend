using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avalanche.Sales.Application.Domain.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasMany(m => m.Transactions).WithOne().HasForeignKey(m => m.OrderId);

        builder.Property(m => m.UserId).IsRequired();

        builder.Property(m => m.Availability).IsRequired();
        builder.Property(m => m.AvailabilitySpan).IsRequired();
    }
}