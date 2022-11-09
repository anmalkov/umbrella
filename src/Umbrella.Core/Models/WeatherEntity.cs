using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public sealed class WeatherEntity : EntityBase
{
    public string DesiredCityName { get; set; }
    public string? DesiredCountryCode { get; set; }
    public string FoundCityName { get; set; }
    public string? FoundCountryCode { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }

    public WeatherEntity(string id, string desiredCityName, string foundCityName) : base(id, EntityType.Weather)
    {
        DesiredCityName = desiredCityName;
        FoundCityName = foundCityName;
    }
}
