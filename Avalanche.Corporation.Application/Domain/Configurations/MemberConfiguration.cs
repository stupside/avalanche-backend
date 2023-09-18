using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avalanche.Corporation.Application.Domain.Configurations;

public sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.HasIndex(m => new { m.OrganizationId, m.UserId }).IsUnique();
        
        builder.Property(m => m.OrganizationId).IsRequired();
        
        builder.Property(m => m.UserId).IsRequired();
    }
}