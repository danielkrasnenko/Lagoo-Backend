using Lagoo.BusinessLogic.Common.Mappings;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvents;

public class GetEventsResponseDto
{
    public int Count { get; set; }

    public ICollection<EventDto> Events { get; set; } = new List<EventDto>();
}

public class EventDto : IMapFrom<Event>
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public EventType Type { get; set; }

    public string Address { get; set; } = string.Empty;

    public string? Comment { get; set; }

    public bool IsPrivate { get; set; }

    public TimeSpan Duration { get; set; }

    public DateTime BeginsAt { get; set; }

    public DateTime CreatedAt { get; set; }
}