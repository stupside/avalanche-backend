using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avalanche.Identity.Application.Domain.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasIndex(m => m.Username).IsUnique();

        builder.HasOne(m => m.UserCredential)
            .WithOne()
            .HasForeignKey<User>(m => m.UserCredentialId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}