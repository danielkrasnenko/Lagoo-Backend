using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.DeleteEvent;

public class DeleteEventCommand : IRequest
{
    public long EventId { get; set; }
}