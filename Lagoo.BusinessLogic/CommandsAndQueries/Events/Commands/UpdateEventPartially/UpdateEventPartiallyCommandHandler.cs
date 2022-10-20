using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEventPartially;

public class UpdateEventPartiallyCommandHandler : IRequestHandler<UpdateEventPartiallyCommand, ReadEventDto>
{
    private readonly IEventRepository _eventRepository;

    public UpdateEventPartiallyCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<ReadEventDto> Handle(UpdateEventPartiallyCommand request, CancellationToken cancellationToken)
    {
        var partiallyUpdatedEvent = await _eventRepository.UpdateAsync(request, cancellationToken);

        if (partiallyUpdatedEvent is null)
        {
            throw new NotFoundException(EventResources.EventWasNotFound);
        }
        
        return partiallyUpdatedEvent;
    }
}