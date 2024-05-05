using System.Linq.Expressions;
using System.Reflection;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lagoo.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Event> Events { get; set; } = null!;

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(cancellationToken);
    }

    public Task LoadRelatedEntityAsync<TEntity, TRelatedEntity>(TEntity entity, Expression<Func<TEntity, TRelatedEntity?>> expression,
        CancellationToken cancellationToken) where TEntity : class where TRelatedEntity : class
    {
        return Entry(entity).Reference(expression).LoadAsync(cancellationToken);
    }

    public Task LoadRelatedCollectionAsync<TEntity, TRelatedEntity>(TEntity entity, Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> expression,
        CancellationToken cancellationToken, Expression<Func<TRelatedEntity, bool>>? condition = null) where TEntity : class where TRelatedEntity : class
    {
        var collection = Entry(entity).Collection(expression);

        return condition is null
            ? collection.LoadAsync(cancellationToken)
            : collection.Query().Where(condition).LoadAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AppUser>().ToTable("asp_net_users");
        builder.Entity<IdentityRole<Guid>>().ToTable("asp_net_roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("asp_net_user_roles").HasKey(ur => new { ur.RoleId, ur.UserId });
        builder.Entity<IdentityUserToken<Guid>>().ToTable("asp_net_user_tokens").HasKey(ut => new { ut.UserId, ut.LoginProvider });
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("asp_net_user_logins").HasKey(l => new { l.LoginProvider, l.ProviderKey });
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("asp_net_user_claims");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("asp_net_role_claims");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.HasPostgresEnum<EventType>();
    }

    private void OnBeforeSaving()
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            var stringProperties = entry.CurrentValues.Properties.Where(p => p.ClrType == typeof(string));
            
            foreach (var property in stringProperties)
            {
                if (entry.CurrentValues[property.Name] is not null)
                {
                    entry.CurrentValues[property.Name] = entry.CurrentValues[property.Name]?.ToString()?.Trim();
                }
            }
        }
    }
}