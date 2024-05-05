using Lagoo.Domain.ConfigurationConstants;
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
            .HasMaxLength(AppUserConfigurationConstants.FirstNameMaxLength);

        builder
            .Property(u => u.LastName)
            .HasMaxLength(AppUserConfigurationConstants.LastNameMaxLength);

        builder
            .Property(u => u.Address)
            .HasMaxLength(AppUserConfigurationConstants.AddressMaxLength);

        builder
            .Property(u => u.RegisteredAt)
            .HasDefaultValueSql("now()");
    }
}