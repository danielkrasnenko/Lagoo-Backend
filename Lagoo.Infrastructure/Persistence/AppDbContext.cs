using System.Linq.Expressions;
using System.Reflection;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lagoo.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>, IAppDbContext
{
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(cancellationToken);
    }

    public Task LoadRelatedEntityAsync<TEntity, TRelatedEntity>(TEntity entity, Expression<Func<TEntity, TRelatedEntity?>> expression) where TEntity : class where TRelatedEntity : class
    {
        return Entry(entity).Reference(expression).LoadAsync();
    }

    public Task LoadRelatedCollectionAsync<TEntity, TRelatedEntity>(TEntity entity, Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> expression,
        Expression<Func<TRelatedEntity, bool>>? condition = null) where TEntity : class where TRelatedEntity : class
    {
        var collection = Entry(entity).Collection(expression);

        return condition is null
            ? collection.LoadAsync()
            : collection.Query().Where(condition).LoadAsync();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
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