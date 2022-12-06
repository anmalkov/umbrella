using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Xiaomi.Sdk.Models;

internal record XiaomiResponse(
    [property:JsonPropertyName("cmd")]
    string Command,
    [property:JsonPropertyName("sid")]
    string Sid,
    [property:JsonPropertyName("model")]
    string? Model,
    [property:JsonPropertyName("data")]
    string? Data
);
