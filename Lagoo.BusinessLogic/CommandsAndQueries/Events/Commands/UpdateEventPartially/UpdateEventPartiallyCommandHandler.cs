using AutoMapper;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEventPartially;

public class UpdateEventPartiallyCommandHandler : IRequestHandler<UpdateEventPartiallyCommand, EventDto>
{
    private readonly IAppDbContext _context;

    private readonly IMapper _mapper;

    public UpdateEventPartiallyCommandHandler(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EventDto> Handle(UpdateEventPartiallyCommand request, CancellationToken cancellationToken)
    {
        var @event = await _context.Events.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (@event == null)
        {
            throw new NotFoundException(EventResources.EventWasNotFound);
        }
        
        PatchEvent(@event, request);

        await _context.SaveChangesAsync(CancellationToken.None);

        return _mapper.Map<EventDto>(@event);
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