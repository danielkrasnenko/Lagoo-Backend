using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;

namespace Lagoo.BusinessLogic.Hubs.Events;

public interface IEventsClient
{
    Task TakeActionOnUpdate(ReadEventDto updatedEventDto);

    Task TakeActionOnDelete(long id);
}