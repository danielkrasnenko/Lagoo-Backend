using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEventPartially;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
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
    public Task<EventDto> GetEvent([FromRoute] GetEventQuery query) => Mediator.Send(query);

    /// <summary>
    ///   Create an event
    /// </summary>
    /// <param name="command">Properties for creating an event</param>
    /// <returns>New event DTO with some default data from database or throws in case of validation failures</returns>
    [HttpPost]
    public Task<EventDto> CreateEvent([FromBody] CreateEventCommand command) => Mediator.Send(command);

    /// <summary>
    ///   Update an event with the given ID
    /// </summary>
    /// <param name="id">ID of needful event</param>
    /// <param name="command">Updated event properties</param>
    /// <returns>Updated event DTO or throws in case of validation failures or wrong event ID</returns>
    [HttpPut("{id:long}")]
    public Task<EventDto> UpdateEvent([FromRoute] long id, [FromBody] UpdateEventCommand command)
    {
        command.Id = id;
        return Mediator.Send(command);
    }

    /// <summary>
    ///   Update an event with the given ID partially
    /// </summary>
    /// <param name="id">ID of needful event</param>
    /// <param name="command">Some updated event properties</param>
    /// <returns>Updated event DTO or throws in case of validation failures or wrong event ID</returns>
    [HttpPatch("{id:long}")]
    public Task<EventDto> UpdateEventPartially([FromRoute] long id, [FromBody] UpdateEventPartiallyCommand command)
    {
        command.Id = id;
        return Mediator.Send(command);
    }
}