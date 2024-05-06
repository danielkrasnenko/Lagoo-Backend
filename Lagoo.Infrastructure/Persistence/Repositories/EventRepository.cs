using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEventPartially;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvents;
using Lagoo.BusinessLogic.Common.Enums;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.Helpers;
using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Lagoo.Infrastructure.Persistence.Repositories;

public class EventRepository : RepositoryBase, IEventRepository
{
    private readonly IMapper _mapper;
    
    public EventRepository(AppDbContext context, IMapper mapper) : base(context)
    {
        _mapper = mapper;
    }

    public async Task<GetEventsResponseDto> GetAllAsync(GetEventsQuery request, CancellationToken cancellationToken)
    {
        var query = ApplyFiltration(Context.Events, request.Type, request.IsPrivate);
        
        var count = await query.CountAsync(cancellationToken: cancellationToken);

        query = ApplySorting(query, request.SortBy, request.SortingOrder);

        query = PaginationHelper.Paginate(query, request.PageSize, request.Page,
            request.LastFetchedEventId.HasValue ? e => e.Id > request.LastFetchedEventId.Value : null);

        var events = await query
            .AsNoTracking()
            .ProjectTo<CollectionEventDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken: cancellationToken);

        return new GetEventsResponseDto
        {
            Count = count,
            Events = events
        };
    }

    public async Task<ReadEventDto?> GetAsync(long eventId, CancellationToken cancellationToken)
    {
        var @event = await Context.Events.ProjectTo<ReadEventDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        return @event ?? null;
    }

    public async Task<ReadEventDto> CreateAsync(CreateEventCommand createEventDto, CancellationToken cancellationToken)
    {
        var newEvent = _mapper.Map<Event>(createEventDto);

        Context.Events.Add(newEvent);
        await Context.SaveChangesAsync(CancellationToken.None);

        return _mapper.Map<ReadEventDto>(newEvent);
    }

    public async Task<ReadEventDto?> UpdateAsync(UpdateEventCommand updateEventDto, CancellationToken cancellationToken)
    {
        var @event = await Context.Events.FirstOrDefaultAsync(e => e.Id == updateEventDto.EventId, cancellationToken);

        if (@event is null)
        {
            return null;
        }
        
        _mapper.Map(updateEventDto, @event);
        @event.LastModifiedAt = DateTime.UtcNow;
        
        await Context.SaveChangesAsync(CancellationToken.None);

        return _mapper.Map<ReadEventDto>(@event);
    }

    public async Task<ReadEventDto?> UpdateAsync(UpdateEventPartiallyCommand updateEventPartiallyDto, CancellationToken cancellationToken)
    {
        var @event = await Context.Events.FirstOrDefaultAsync(e => e.Id == updateEventPartiallyDto.EventId, cancellationToken);

        if (@event == null)
        {
            return null;
        }
        
        PatchEvent(@event, updateEventPartiallyDto);

        await Context.SaveChangesAsync(CancellationToken.None);

        return _mapper.Map<ReadEventDto>(@event);
    }

    public async Task<bool> DeleteAsync(long eventId, CancellationToken cancellationToken)
    {
        var @event = await Context.Events.FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        if (@event is null)
        {
            return false;
        }

        Context.Events.Remove(@event);
        await Context.SaveChangesAsync(CancellationToken.None);

        return true;
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
    
    private void PatchEvent(Event @event, UpdateEventPartiallyCommand command)
    {
        @event.Name = command.Name ?? @event.Name;
        @event.Type = command.Type ?? @event.Type;
        @event.Address = command.Address ?? @event.Address;
        @event.Comment = command.Comment ?? @event.Comment;
        @event.IsPrivate = command.IsPrivate ?? @event.IsPrivate;
        @event.Duration = command.Duration ?? @event.Duration;
        @event.BeginsAt = command.BeginsAt ?? @event.BeginsAt;
        @event.LastModifiedAt = DateTime.UtcNow;
    }
}