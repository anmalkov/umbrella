﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PhilipsHue.Sdk.Models;

public class PhilipsHueZone : PhilipsHueBase
{
    [JsonPropertyName("children")]
    public List<PhilipsHueResourceIdentifier> Children { get; set; } = new();

    [JsonPropertyName("services")]
    public List<PhilipsHueResourceIdentifier> Services { get; set; } = new();
}
