using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public sealed class WeatherEntity : EntityBase
{
    public string City { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }

    public WeatherEntity(string id, string city) : base(id, EntityType.Weather)
    {
        City = city;
    }
}
