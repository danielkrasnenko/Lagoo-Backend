using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace Lagoo.BusinessLogic.UnitTests;

public static class TestHelpers
{
    public static IStringLocalizer<TResources> MockLocalizer<TResources>()
    {
        var localizer = Substitute.For<IStringLocalizer<TResources>>();

        localizer[string.Empty].ReturnsForAnyArgs(info => new LocalizedString(info.Arg<string>(), info.Arg<string>()));

        localizer[string.Empty, string.Empty]
            .ReturnsForAnyArgs(info => new LocalizedString(info.Arg<string>(), info.Arg<string>()));

        return localizer;
    }

    public static DbSet<TEntity> MockDbSet<TEntity>(params TEntity[] entities) where TEntity : class
    {
        return entities.AsQueryable().BuildMockDbSet();
    }

    public static DbSet<TEntity> MockDbSet<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        return entities.AsQueryable().BuildMockDbSet();
    }
}