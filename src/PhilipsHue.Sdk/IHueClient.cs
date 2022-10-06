using PhilipsHue.Sdk.Models;

namespace PhilipsHue.Sdk;

public interface IHueClient : IDisposable
{
    event Action<IEnumerable<PhilipsHueEventResponse>> OnEventMessage;

    Task<PhilipsHueRegistrationInfo?> RegisterAsync(string applicationName, string deviceName);
    Task<IEnumerable<PhilipsHueLight>> GetLightsAsync();
    Task<PhilipsHueLight> GetLightAsync(Guid id);

    Task<PhilipsHuePutResponse> UpdateLightAsync(Guid id, PhilipsHueUpdateLight data);

    Task StartListeningForEventsAsync(CancellationToken? cancellationToken = null);
    void StopListeningForEvents();
}