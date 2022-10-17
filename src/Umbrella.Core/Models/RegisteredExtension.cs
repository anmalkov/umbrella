using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public class RegisteredExtension : IStorableItem
{
    public string Id { get; init; }
    public Dictionary<string, string?>? Parameters { get; set; }

    [JsonConstructor]
    public RegisteredExtension(string id): this(id, null) { }

    public RegisteredExtension(string id, Dictionary<string, string?>? parameters)
    {
        Id = id;
        Parameters = parameters;
    }
}
