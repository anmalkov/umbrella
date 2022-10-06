using PhilipsHue.Sdk.Models;

namespace PhilipsHue.Sdk;

public interface IHueClient
{
    event Action<IEnumerable<PhilipsHueEventResponse>> OnEventMessage;

    Task<PhilipsHueRegistrationInfo?> RegisterAsync(string applicationName, string deviceName);
    Task<IEnumerable<PhilipsHueLight>> GetLightsAsync();

    Task<PhilipsHuePutResponse> UpdateLightAsync(Guid id, PhilipsHueUpdateLight data);

    Task StartListeningForEventsAsync(CancellationToken? cancellationToken = null);
    void StopListeningForEvents();
}