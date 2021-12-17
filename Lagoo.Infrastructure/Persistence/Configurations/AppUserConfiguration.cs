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
            .HasColumnType("nvarchar")
            .HasMaxLength(255);

        builder
            .Property(u => u.LastName)
            .HasColumnType("nvarchar")
            .HasMaxLength(255);

        builder
            .Property(u => u.Address)
            .HasColumnType("nvarchar")
            .HasMaxLength(255);

        builder
            .Property(u => u.RegistrationUtcDate)
            .HasDefaultValueSql("getutcdate()");
    }
}