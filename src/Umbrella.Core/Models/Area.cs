using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umbrella.Core.Repositories;

namespace Umbrella.Core.Models;

public class Area : IStorableItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Icon { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }

    [JsonIgnore]
    public bool InsideHouse => !Longitude.HasValue && !Latitude.HasValue;

    public Area(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
