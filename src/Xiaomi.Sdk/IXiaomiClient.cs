using Xiaomi.Sdk.Models;

namespace Xiaomi.Sdk;

public interface IXiaomiClient
{
    event Action<XiaomiEventResponse> OnEventMessage;

    Task<IEnumerable<XiaomiDevice>> GetAllDevicesAsync();
    Task<XiaomiDevice?> GetDeviceAsync(string id);

    Task<IEnumerable<XiaomiCloudDevice>> GetAllDevicesFromCloudAsync(string username, string password, string serverCountryCode);

    void StartListeningForEvents(CancellationToken? cancellationToken = null);
    void StopListeningForEvents();
}
