using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMap.Sdk.Models;

public record GeoCoordinates (
    string CityName,
    double Latitude,
    double Longitude,
    string CountryCode,
    string? StateCode,
    IDictionary<string, string> LocalNames
);
