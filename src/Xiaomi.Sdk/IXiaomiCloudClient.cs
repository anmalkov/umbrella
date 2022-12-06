using Xiaomi.Sdk.Models;

namespace Xiaomi.Sdk;

internal interface IXiaomiCloudClient
{
    Task<IEnumerable<XiaomiCloudDevice>> GetDevicesAsync(string username, string password, string serverCountryCode);
}
