using Umbrella.Core.Models;

namespace Umbrella.Core.Services;

public interface ICoreService
{
    Task StartAsync(Action<string, IEntityState>? entityStateUpdated = null);
    Task StopAsync();
}
