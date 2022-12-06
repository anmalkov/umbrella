using System.Text.Json.Serialization;

namespace Xiaomi.Sdk.Models;

public record XiaomiCloudDevice(
    [property:JsonPropertyName("did")]
    string Id,
    [property:JsonPropertyName("name")]
    string Name,
    [property:JsonPropertyName("model")]
    string Model,
    [property:JsonPropertyName("isOnline")]
    bool IsOnline,
    [property:JsonPropertyName("token")]
    string? Token,
    [property:JsonPropertyName("localip")]
    string? LocalIp,
    [property:JsonPropertyName("mac")]
    string? Mac,
    string? FirmwareVersion
);
