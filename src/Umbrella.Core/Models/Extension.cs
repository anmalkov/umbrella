using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public class Extension
{
    public string Id { get; init; }
    public Dictionary<string, string?>? Parameters { get; set; }

    [JsonConstructor]
    public Extension(string id): this(id, null) { }

    public Extension(string id, Dictionary<string, string?>? parameters)
    {
        Id = id;
        Parameters = parameters;
    }
}
