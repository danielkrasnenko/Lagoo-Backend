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
        var query = _context.Events.AsQueryable();

        query = ApplyFiltration(query, request.Type, request.IsPrivate);
        
        var count = await query.CountAsync(cancellationToken);
        
        query = PaginationHelper.Paginate(query, request.Page, request.PageSize);

        query = ApplySorting(query, request.SortBy, request.SortingOrder);

        var events = await query
            .AsNoTracking()
            .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
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
            return eventsQuery;
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
            SortingOrder.Ascending => eventsQuery.OrderBy(columnSelector),
            SortingOrder.Descending => eventsQuery.OrderByDescending(columnSelector),
            _ => throw new BadRequestException(EventResources.InvalidSortingOrder)
        };
    }
}