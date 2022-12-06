using Xiaomi.Sdk.Models;

namespace Xiaomi.Sdk;

internal interface IXiaomiClient
{
    event Action<XiaomiEventResponse> OnEventMessage;

    Task<IEnumerable<XiaomiDevice>> GetAllDevicesAsync();
    Task<XiaomiDevice?> GetDeviceAsync(string id);

    Task<IEnumerable<XiaomiCloudDevice>> GetDevicesFromCloudAsync(string username, string password, string serverCountryCode);

    void StartListeningForEvents(CancellationToken? cancellationToken = null);
    void StopListeningForEvents();
}
