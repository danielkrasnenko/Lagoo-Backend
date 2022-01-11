using Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lagoo.Api.Controllers;

[Route("api/events")]
public class EventsController : ApiController
{
    public EventsController(IMediator mediator) : base(mediator) { }
    
    /// <summary>
    ///   Get events with optionally specified filtration, sorting and pagination
    /// </summary>
    /// <param name="query">Optional parameters for filtration, sorting and pagination</param>
    /// <returns>Events and their count, or throws in case of invalid parameters</returns>
    [HttpGet]
    public Task<GetEventsResponseDto> GetEvents([FromQuery] GetEventsQuery query) => Mediator.Send(query);

    /// <summary>
    ///   Get an event by specified ID
    /// </summary>
    /// <param name="query">ID of an event</param>
    /// <returns>Requested event or not found response</returns>
    [HttpGet("{id:long}")]
    public Task<GetEventResponseDto> GetEvent([FromRoute] GetEventQuery query) => Mediator.Send(query);
}