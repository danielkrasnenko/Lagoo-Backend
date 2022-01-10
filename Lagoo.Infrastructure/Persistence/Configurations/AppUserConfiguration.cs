using Lagoo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lagoo.Infrastructure.Persistence.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public const int FirstNameMaxLength = 256;

    public const int LastNameMaxLength = 256;

    public const int AddressMaxLength = 256;
    
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder
            .Property(u => u.FirstName)
            .HasMaxLength(FirstNameMaxLength);

        builder
            .Property(u => u.LastName)
            .HasMaxLength(LastNameMaxLength);

        builder
            .Property(u => u.Address)
            .HasMaxLength(AddressMaxLength);

        builder
            .Property(u => u.RegisteredAt)
            .HasDefaultValueSql("getutcdate()");
    }
}