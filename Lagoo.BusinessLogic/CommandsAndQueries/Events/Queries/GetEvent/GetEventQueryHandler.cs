using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvent;

public class GetEventQueryHandler : IRequestHandler<GetEventQuery, ReadEventDto>
{
    private readonly IEventRepository _eventRepository;

    public GetEventQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<ReadEventDto> Handle(GetEventQuery request, CancellationToken cancellationToken)
    {
        var @event = await _eventRepository.GetAsync(request.EventId, cancellationToken);

        return @event ?? throw new NotFoundException(EventResources.EventWasNotFound);
    }
}