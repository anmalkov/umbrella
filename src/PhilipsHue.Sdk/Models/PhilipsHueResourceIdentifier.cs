using System.Text.Json.Serialization;

namespace PhilipsHue.Sdk.Models;

public record PhilipsHueResourceIdentifier
{
    [JsonPropertyName("rid")]
    public Guid Rid { get; set; }

    [JsonPropertyName("rtype")]
    public string Rtype { get; set; } = default!;
}
