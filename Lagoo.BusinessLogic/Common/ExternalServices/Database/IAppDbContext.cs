using System.Linq.Expressions;
using Lagoo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Lagoo.BusinessLogic.Common.ExternalServices.Database;

/// <summary>
///  An interface for declaring needed functionality to work with Persistence Store
/// </summary>
public interface IAppDbContext
{
    public DbSet<AppUser> Users { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task LoadRelatedEntityAsync<TEntity, TRelatedEntity>(TEntity entity,
        Expression<Func<TEntity, TRelatedEntity?>> expression) where TEntity : class where TRelatedEntity : class;

    Task LoadRelatedCollectionAsync<TEntity, TRelatedEntity>(TEntity entity,
        Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> expression,
        Expression<Func<TRelatedEntity, bool>>? condition = null) where TEntity : class where TRelatedEntity : class;

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
}