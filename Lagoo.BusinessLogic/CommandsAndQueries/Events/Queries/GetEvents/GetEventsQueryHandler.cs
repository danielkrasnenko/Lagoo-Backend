using Lagoo.BusinessLogic.Core.Repositories;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvents;

/// <summary>
///   A handler for <see cref="GetEventsQuery"/>
/// </summary>
public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, GetEventsResponseDto>
{
    private readonly IEventRepository _eventRepository;

    public GetEventsQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public Task<GetEventsResponseDto> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        return _eventRepository.GetAllAsync(request, cancellationToken);
    }
}