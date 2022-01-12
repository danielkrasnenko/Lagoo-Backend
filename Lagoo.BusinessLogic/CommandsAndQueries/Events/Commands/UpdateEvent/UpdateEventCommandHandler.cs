using AutoMapper;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEvent;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, EventDto>
{
    private readonly IAppDbContext _context;

    private readonly IMapper _mapper;
    
    public UpdateEventCommandHandler(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EventDto> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await _context.Events.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (@event is null)
        {
            throw new NotFoundException(EventResources.EventWasNotFound);
        }
        
        _mapper.Map(request, @event);
        @event.LastModifiedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(CancellationToken.None);

        return _mapper.Map<EventDto>(@event);
    }
}