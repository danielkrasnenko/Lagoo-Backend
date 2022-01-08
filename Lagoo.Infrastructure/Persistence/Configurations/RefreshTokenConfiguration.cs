using Lagoo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lagoo.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder
            .HasIndex(rt => rt.Value);

        builder
            .HasIndex(rt => rt.DeviceId);
        
        builder
            .Property(rt => rt.Value)
            .HasColumnType("char")
            .HasMaxLength(40);
    }
}