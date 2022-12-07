using Xiaomi.Sdk.Models;

namespace Xiaomi.Sdk;

internal interface IXiaomiCloudClient
{
    Task<IEnumerable<XiaomiCloudDevice>> GetAllDevicesAsync(string username, string password, string serverCountryCode);
}
