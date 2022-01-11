namespace Lagoo.BusinessLogic.Common.Helpers;

public static class PaginationHelper
{
    public static IQueryable<TItem> Paginate<TItem>(IQueryable<TItem> itemsQuery, int? page, int? pageSize)
    {
        if (page.HasValue && pageSize.HasValue)
        {
            return itemsQuery.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return itemsQuery;
    }
    
    public static IEnumerable<TItem> Paginate<TItem>(IEnumerable<TItem> itemsQuery, int? page, int? pageSize)
    {
        if (page.HasValue && pageSize.HasValue)
        {
            return itemsQuery.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return itemsQuery;
    }
}