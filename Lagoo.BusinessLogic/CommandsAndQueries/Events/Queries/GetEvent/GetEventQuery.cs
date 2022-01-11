using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvent;

public class GetEventQuery : IRequest<GetEventResponseDto>
{
    public long Id { get; set; }
}