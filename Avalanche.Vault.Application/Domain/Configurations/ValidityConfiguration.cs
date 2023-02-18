using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avalanche.Vault.Application.Domain.Configurations;

public class ValidityConfiguration : IEntityTypeConfiguration<Validity>
{
    public void Configure(EntityTypeBuilder<Validity> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.From).IsRequired();
        builder.Property(m => m.To).IsRequired();
    }
}