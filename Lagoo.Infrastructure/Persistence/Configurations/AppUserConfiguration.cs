using Lagoo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lagoo.Infrastructure.Persistence.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder
            .Property(u => u.FirstName)
            .HasMaxLength(256);

        builder
            .Property(u => u.LastName)
            .HasMaxLength(256);

        builder
            .Property(u => u.Address)
            .HasMaxLength(256);

        builder
            .Property(u => u.RegisteredAt)
            .HasDefaultValueSql("getutcdate()");
    }
}