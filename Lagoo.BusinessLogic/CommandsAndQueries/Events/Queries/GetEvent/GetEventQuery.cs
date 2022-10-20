using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvent;

public class GetEventQuery : IRequest<ReadEventDto>
{
    public long EventId { get; set; }
}