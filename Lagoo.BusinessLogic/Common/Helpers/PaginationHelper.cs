using System.Linq.Expressions;

namespace Lagoo.BusinessLogic.Common.Helpers;

public static class PaginationHelper
{
    public static IQueryable<TItem> Paginate<TItem>(IQueryable<TItem> itemsQuery, int? pageSize, int? page = null, Expression<Func<TItem, bool>>? conditionForSkipping = null)
    {
        if (!pageSize.HasValue)
        {
            return itemsQuery;
        }
        
        if (page.HasValue)
        {
            return itemsQuery.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return conditionForSkipping is not null
            ? itemsQuery.Where(conditionForSkipping).Take(pageSize.Value)
            : itemsQuery;
    }
    
    public static IEnumerable<TItem> Paginate<TItem>(IEnumerable<TItem> itemsQuery, int? pageSize, int? page = null, Func<TItem, bool>? conditionForSkipping = null)
    {
        if (!pageSize.HasValue)
        {
            return itemsQuery;
        }
        
        if (page.HasValue)
        {
            return itemsQuery.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return conditionForSkipping is not null
            ? itemsQuery.Where(conditionForSkipping).Take(pageSize.Value)
            : itemsQuery;
    }
}