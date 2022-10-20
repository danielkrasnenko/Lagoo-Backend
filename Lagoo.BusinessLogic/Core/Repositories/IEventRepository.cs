using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEventPartially;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvents;

namespace Lagoo.BusinessLogic.Core.Repositories;

public interface IEventRepository : IRepository
{
    Task<GetEventsResponseDto> GetAllAsync(GetEventsQuery query, CancellationToken cancellationToken);

    Task<ReadEventDto?> GetAsync(long eventId, CancellationToken cancellationToken);

    Task<ReadEventDto> CreateAsync(CreateEventCommand createEventDto, CancellationToken cancellationToken);

    Task<ReadEventDto?> UpdateAsync(UpdateEventCommand updateEventDto, CancellationToken cancellationToken);
    
    Task<ReadEventDto?> UpdateAsync(UpdateEventPartiallyCommand updateEventPartiallyDto, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(long eventId, CancellationToken cancellationToken);
}