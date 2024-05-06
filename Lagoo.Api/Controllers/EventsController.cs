using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.DeleteEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEventPartially;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lagoo.Api.Controllers;

/// <summary>
///   The events controller
/// </summary>
[Route("api/events")]
public class EventsController : ApiController
{
    public EventsController(ISender sender) : base(sender) { }
    
    /// <summary>
    ///   Get events with optionally specified filtration, sorting and pagination
    /// </summary>
    /// <param name="query">Optional parameters for filtration, sorting and pagination</param>
    /// <returns>Events and their count, or throws in case of invalid parameters</returns>
    [HttpGet]
    public Task<GetEventsResponseDto> GetEvents([FromQuery] GetEventsQuery query) => Sender.Send(query);

    /// <summary>
    ///   Get an event by specified ID
    /// </summary>
    /// <param name="query">ID of an event</param>
    /// <returns>Requested event or not found response</returns>
    [HttpGet("{eventId:long}")]
    public Task<ReadEventDto> GetEvent([FromRoute] GetEventQuery query) => Sender.Send(query);

    /// <summary>
    ///   Create an event
    /// </summary>
    /// <param name="command">Properties for creating an event</param>
    /// <returns>New event DTO with some default data from database or throws in case of validation failures</returns>
    [HttpPost]
    public Task<ReadEventDto> CreateEvent([FromBody] CreateEventCommand command) => Sender.Send(command);

    /// <summary>
    ///   Update an event with the given ID
    /// </summary>
    /// <param name="eventId">ID of needful event</param>
    /// <param name="command">Updated event properties</param>
    /// <returns>Updated event DTO or throws in case of validation failures or wrong event ID</returns>
    [HttpPut("{eventId:long}")]
    public Task<ReadEventDto> UpdateEvent([FromRoute] long eventId, [FromBody] UpdateEventCommand command)
    {
        command.EventId = eventId;
        return Sender.Send(command);
    }

    /// <summary>
    ///   Update an event with the given ID partially
    /// </summary>
    /// <param name="eventId">ID of needful event</param>
    /// <param name="command">Some updated event properties</param>
    /// <returns>Updated event DTO or throws in case of validation failures or wrong event ID</returns>
    [HttpPatch("{eventId:long}")]
    public Task<ReadEventDto> UpdateEventPartially([FromRoute] long eventId, [FromBody] UpdateEventPartiallyCommand command)
    {
        command.EventId = eventId;
        return Sender.Send(command);
    }

    /// <summary>
    ///   Delete an event with the given ID
    /// </summary>
    /// <param name="command">ID of needful event</param>
    [HttpDelete("{eventId:long}")]
    public Task DeleteEvent([FromRoute] DeleteEventCommand command) => Sender.Send(command);
}