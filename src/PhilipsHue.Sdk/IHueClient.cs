using PhilipsHue.Sdk.Models;

namespace PhilipsHue.Sdk;

public interface IHueClient : IDisposable
{
    event Action<IEnumerable<PhilipsHueEventResponse>> OnEventMessage;

    Task<PhilipsHueRegistrationInfo?> RegisterAsync(string applicationName, string deviceName);
    Task<IEnumerable<PhilipsHueLight>> GetLightsAsync();
    Task<PhilipsHueLight> GetLightAsync(Guid id);
    Task<IEnumerable<PhilipsHueRoom>> GetRoomsAsync();
    Task<PhilipsHueRoom> GetRoomAsync(Guid id);
    Task<IEnumerable<PhilipsHueZone>> GetZonesAsync();
    Task<PhilipsHueZone> GetZoneAsync(Guid id);
    Task<IEnumerable<PhilipsHueZigbeeConnectivity>> GetZigbeeConnectivityAsync();

    Task<PhilipsHuePutResponse> UpdateLightAsync(Guid id, PhilipsHueUpdateLight data);

    Task StartListeningForEventsAsync(CancellationToken? cancellationToken = null);
    void StopListeningForEvents();
}