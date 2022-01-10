using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;

namespace Lagoo.BusinessLogic.UnitTests;

public static class TestHelpers
{
    public static DbSet<TEntity> MockDbSet<TEntity>(params TEntity[] entities) where TEntity : class
    {
        return entities.AsQueryable().BuildMockDbSet();
    }

    public static DbSet<TEntity> MockDbSet<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        return entities.AsQueryable().BuildMockDbSet();
    }
}