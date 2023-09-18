using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avalanche.Corporation.Application.Domain.Configurations;

public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasMany(m => m.Members).WithOne().HasForeignKey(m => m.OrganizationId);
        builder.HasMany(m => m.Invitations).WithOne().HasForeignKey(m => m.OrganizationId);

        builder.HasIndex(m => m.Name).IsUnique();

        builder.Property(m => m.Name).IsRequired();

        builder.Property(m => m.UserId).IsRequired();
    }
}