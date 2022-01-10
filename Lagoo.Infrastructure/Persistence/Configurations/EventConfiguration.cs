using Lagoo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lagoo.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public const int NameMaxLength = 256;

    public const int AddressMaxLength = 512;

    public const int CommentMaxLength = 1026;

    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder
            .Property(e => e.Name)
            .HasMaxLength(NameMaxLength);
        
        builder
            .Property(e => e.Type)
            .HasConversion<byte>();

        builder
            .Property(e => e.Address)
            .HasMaxLength(AddressMaxLength);

        builder
            .Property(e => e.Comment)
            .HasMaxLength(CommentMaxLength);

        builder
            .Property(e => e.BeginsAt)
            .HasDefaultValueSql("getutcdate()");
    }
}