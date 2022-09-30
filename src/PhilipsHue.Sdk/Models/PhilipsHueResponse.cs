using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PhilipsHue.Sdk.Models;

public class PhilipsHueError
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class PhilipsHueErrorResponse
{
    [JsonPropertyName("errors")]
    public List<PhilipsHueError> Errors { get; set; } = new();

    public bool HasErrors => Errors.Any();
}

public class PhilipsHueResponse<T> : PhilipsHueErrorResponse
{
    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = new();
}

public class PhilipsHuePostResponse : PhilipsHueResponse<PhilipsHueResourceIdentifier>
{

}

public class PhilipsHuePutResponse : PhilipsHueResponse<PhilipsHueResourceIdentifier>
{

}

public class PhilipsHueDeleteResponse : PhilipsHueResponse<PhilipsHueResourceIdentifier>
{

}


