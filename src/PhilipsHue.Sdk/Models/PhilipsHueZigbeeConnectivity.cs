using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PhilipsHue.Sdk.Models;

public class PhilipsHueZigbeeConnectivity : PhilipsHueBase
{
    [JsonPropertyName("owner")]
    public PhilipsHueResourceIdentifier Owner { get; set; } = default!;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = default!;
}
