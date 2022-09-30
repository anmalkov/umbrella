using System.Text.Json.Serialization;

namespace PhilipsHue.Sdk.Models;

public class PhilipsHueMetadata
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("archetype")]
    public string? Archetype { get; set; }

    [JsonPropertyName("image")]
    public PhilipsHueResourceIdentifier? Image { get; set; }
}
