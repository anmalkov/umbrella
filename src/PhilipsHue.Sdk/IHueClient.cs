using PhilipsHue.Sdk.Models;

namespace PhilipsHue.Sdk;

public interface IHueClient
{
    Task<PhilipsHueRegistrationInfo?> RegisterAsync(string applicationName, string deviceName);
    Task<IEnumerable<PhilipsHueLight>> GetLightsAsync();
}