using Microsoft.AspNetCore.SignalR;
using Umbrella.Core.Models;

namespace Umbrella.Ui.Hubs;

public class EntityStateHub : Hub
{
    public async Task SendStateUpdate(string id, IEntityState state)
    {
        await Clients.All.SendAsync("ReceiveStateUpdate", id, state);
    }
}
