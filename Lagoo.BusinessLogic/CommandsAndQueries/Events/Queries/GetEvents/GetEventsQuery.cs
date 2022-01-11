using Lagoo.BusinessLogic.Common.Enums;
using Lagoo.Domain.Enums;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvents;

/// <summary>
///   A Query for getting events with optional filtering, sorting and pagination parameters
/// </summary>
public class GetEventsQuery : IRequest<GetEventsResponseDto>
{
    public int? Page { get; set; }

    public int? PageSize { get; set; }
    
    public EventType? Type { get; set; }

    public bool? IsPrivate { get; set; }

    public GetEventsSortBy? SortBy { get; set; }

    public SortingOrder? SortingOrder { get; set; }
}