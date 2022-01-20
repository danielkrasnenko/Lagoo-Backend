using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace Lagoo.Api.Hubs.Events;

public class EventsHub : Hub
{
    public const string Route = "/events-hub";
    
    public Task NotifyOthersAboutUpdateAsync(EventDto updatedEventDto)
    {
        return Clients.Others.SendAsync(EventsHubClientMethods.TakeActionOnUpdate, updatedEventDto);
    }

    public Task NotifyOthersAboutDeletionAsync(long id)
    {
        return Clients.Others.SendAsync(EventsHubClientMethods.TakeActionOnDelete, id);
    }
}