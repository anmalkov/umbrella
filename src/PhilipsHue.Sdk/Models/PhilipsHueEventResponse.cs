using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PhilipsHue.Sdk.Models;

public class PhilipsHueEventResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = default!;

    [JsonPropertyName("creationtime")]
    public DateTimeOffset CreationTime { get; set; }

    [JsonPropertyName("data")]
    public List<PhilipsHueEventData> Data { get; set; } = new();

    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;
}

public class PhilipsHueEventData
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = default!;

    [JsonPropertyName("id_v1")]
    public string? IdV1 { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;
    
    [JsonPropertyName("owner")]
    public PhilipsHueResourceIdentifier? Owner { get; set; }

    [JsonPropertyName("on")]
    public On? On { get; set; }

    [JsonPropertyName("color")]
    public Color? Color { get; set; }

    [JsonPropertyName("color_temperature")]
    public ColorTemperature? ColorTemperature { get; set; }

    [JsonPropertyName("dimming")]
    public Dimming? Dimming { get; set; }
}



