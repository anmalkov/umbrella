namespace Umbrella.Core.Services;

public interface ICoreService
{
    Task StartAsync();
    Task StopAsync();
}
