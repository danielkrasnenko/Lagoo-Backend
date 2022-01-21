using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;

namespace Lagoo.Api.Hubs.Events;

public interface IEventsClient
{
    Task TakeActionOnUpdate(EventDto updatedEventDto);

    Task TakeActionOnDelete(long id);
}