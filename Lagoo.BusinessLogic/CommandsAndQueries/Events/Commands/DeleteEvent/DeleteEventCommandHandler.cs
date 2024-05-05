using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.DeleteEvent;

public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand>
{
    private readonly IEventRepository _eventRepository;

    public DeleteEventCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _eventRepository.DeleteAsync(request.EventId, cancellationToken);

        if (!deleted)
        {
            throw new NotFoundException(EventResources.EventWasNotFound);
        }
    }
}