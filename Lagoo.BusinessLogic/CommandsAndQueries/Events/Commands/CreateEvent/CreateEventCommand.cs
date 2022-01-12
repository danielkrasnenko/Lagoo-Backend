using AutoMapper;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.Common.Mappings;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;

public class CreateEventCommand : IRequest<EventDto>, IMapFrom<Event>
{
    public string Name { get; set; } = string.Empty;

    public EventType Type { get; set; }

    public string Address { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public bool IsPrivate { get; set; }

    public TimeSpan Duration { get; set; }

    public DateTime BeginsAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateEventCommand, Event>()
            .ForMember(e => e.Id, opt => opt.Ignore())
            .ForMember(e => e.CreatedAt, opt => opt.Ignore())
            .ForMember(e => e.LastModifiedAt, opt => opt.Ignore());
    }
}