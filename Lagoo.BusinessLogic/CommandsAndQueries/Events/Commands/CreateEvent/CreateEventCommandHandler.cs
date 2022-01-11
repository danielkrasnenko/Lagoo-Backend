using AutoMapper;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.Domain.Entities;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDto>
{
    private readonly IAppDbContext _context;

    private readonly IMapper _mapper;

    public CreateEventCommandHandler(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EventDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var newEvent = _mapper.Map<Event>(request);

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync(CancellationToken.None);

        return _mapper.Map<EventDto>(newEvent);
    }
}