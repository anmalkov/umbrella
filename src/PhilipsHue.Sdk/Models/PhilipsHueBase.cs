using System.Text.Json.Serialization;
using System.Text.Json;

namespace PhilipsHue.Sdk.Models;

public class PhilipsHueBase
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = default!;

    [JsonPropertyName("id_v1")]
    public string? IdV1 { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;
    
    [JsonPropertyName("metadata")]
    public PhilipsHueMetadata? Metadata { get; set; } = default!;

    [JsonPropertyName("creation_time")]
    public DateTimeOffset? CreationTime { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; set; } = new();
}
