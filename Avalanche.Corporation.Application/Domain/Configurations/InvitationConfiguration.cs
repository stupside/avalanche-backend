using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avalanche.Corporation.Application.Domain.Configurations;

public sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasIndex(m => new { m.OrganizationId, m.Email, m.Status }).IsUnique();

        builder.HasOne<Member>().WithOne().HasForeignKey<Invitation>(m => m.MemberId);

        builder.Property(m => m.OrganizationId).IsRequired();

        builder.Property(m => m.Email).IsRequired();
        builder.Property(m => m.Status).IsRequired();
    }
}