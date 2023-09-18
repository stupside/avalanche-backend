using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avalanche.Aggregator.Application.Domain.Configurations;

public class ServerConfiguration : IEntityTypeConfiguration<Server>
{
    public void Configure(EntityTypeBuilder<Server> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasMany(m => m.Records).WithOne().HasForeignKey(m => m.ServerId);
    }
}