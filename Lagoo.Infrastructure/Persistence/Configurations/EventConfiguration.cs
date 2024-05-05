using Lagoo.Domain.ConfigurationConstants;
using Lagoo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lagoo.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder
            .Property(e => e.Name)
            .HasMaxLength(EventConfigurationConstansts.NameMaxLength);
        
        builder
            .Property(e => e.Type)
            .HasConversion<byte>();

        builder
            .Property(e => e.Address)
            .HasMaxLength(EventConfigurationConstansts.AddressMaxLength);

        builder
            .Property(e => e.Comment)
            .HasMaxLength(EventConfigurationConstansts.CommentMaxLength);

        builder
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");
    }
}