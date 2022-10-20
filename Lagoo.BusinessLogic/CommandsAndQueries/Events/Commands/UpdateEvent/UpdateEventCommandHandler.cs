using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEvent;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, ReadEventDto>
{
    private readonly IEventRepository _eventRepository;

    public UpdateEventCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<ReadEventDto> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var updatedEvent = await _eventRepository.UpdateAsync(request, cancellationToken);

        if (updatedEvent is null)
        {
            throw new NotFoundException(EventResources.EventWasNotFound);
        }
        
        return updatedEvent;
    }
}