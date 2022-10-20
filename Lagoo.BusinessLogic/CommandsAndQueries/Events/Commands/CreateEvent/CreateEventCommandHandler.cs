using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.Core.Repositories;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, ReadEventDto>
{
    private readonly IEventRepository _eventRepository;

    public CreateEventCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public Task<ReadEventDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        return _eventRepository.CreateAsync(request, cancellationToken);
    }
}