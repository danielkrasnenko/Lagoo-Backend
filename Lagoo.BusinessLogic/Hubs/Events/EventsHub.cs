using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace Lagoo.BusinessLogic.Hubs.Events;

public class EventsHub : Hub<IEventsClient>
{
    public const string Route = "/events-hub";
    
    public Task NotifyOthersAboutUpdateAsync(ReadEventDto updatedEventDto)
    {
        return Clients.Others.TakeActionOnUpdate(updatedEventDto);
    }

    public Task NotifyOthersAboutDeletionAsync(long id)
    {
        return Clients.Others.TakeActionOnDelete(id);
    }
}