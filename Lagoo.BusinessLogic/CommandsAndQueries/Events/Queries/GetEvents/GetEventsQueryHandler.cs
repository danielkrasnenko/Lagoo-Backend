using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lagoo.BusinessLogic.Common.Enums;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.BusinessLogic.Common.Helpers;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvents;

/// <summary>
///   A handler for <see cref="GetEventsQuery"/>
/// </summary>
public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, GetEventsResponseDto>
{
    private readonly IAppDbContext _context;

    private readonly IMapper _mapper;

    public GetEventsQueryHandler(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetEventsResponseDto> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        var query = ApplyFiltration(_context.Events, request.Type, request.IsPrivate);
        
        var count = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, request.SortBy, request.SortingOrder);

        query = PaginationHelper.Paginate(query, request.PageSize, request.Page,
            request.LastFetchedEventId.HasValue ? e => e.Id > request.LastFetchedEventId.Value : null);

        var events = await query
            .AsNoTracking()
            .ProjectTo<CollectionEventDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new GetEventsResponseDto
        {
            Count = count,
            Events = events
        };
    }

    // Filtration according to specified parameters
    private IQueryable<Event> ApplyFiltration(IQueryable<Event> eventsQuery, EventType? type, bool? isPrivate)
    {
        if (type.HasValue)
        {
            eventsQuery = eventsQuery.Where(e => e.Type == type);
        }

        if (isPrivate.HasValue)
        {
            eventsQuery = eventsQuery.Where(e => e.IsPrivate == isPrivate);
        }

        return eventsQuery;
    }

    // Apply sorting if both needed parameters are specified
    private IQueryable<Event> ApplySorting(IQueryable<Event> eventsQuery, GetEventsSortBy? sortBy,
        SortingOrder? sortingOrder)
    {
        if (!sortBy.HasValue || !sortingOrder.HasValue)
        {
            return eventsQuery.OrderBy(e => e.Id);
        }
        
        Expression<Func<Event, object>> columnSelector = sortBy switch
        {
            GetEventsSortBy.Name => e => e.Name,
            GetEventsSortBy.Duration => e => e.Duration,
            GetEventsSortBy.BeginsAt => e => e.BeginsAt,
            GetEventsSortBy.CreatedAt => e => e.CreatedAt,
            _ => throw new BadRequestException(EventResources.InvalidSortingProperty)
        };
        
        return sortingOrder switch
        {
            SortingOrder.Ascending => eventsQuery.OrderBy(columnSelector).ThenBy(e => e.Id),
            SortingOrder.Descending => eventsQuery.OrderByDescending(columnSelector).ThenByDescending(e => e.Id),
            _ => throw new BadRequestException(EventResources.InvalidSortingOrder)
        };
    }
}