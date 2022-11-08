using OpenWeatherMap.Sdk.Models;
using System.Collections.Generic;
using System.Transactions;

namespace OpenWeatherMap.Sdk;

public interface IOpenWeatherMapClient
{
    Task<IEnumerable<GeoCoordinates>> GetGeoCoordinatesAsync(string cityName, string? stateCode = null, string? countryCode = null, int? limit = null);
    Task<CurrentWeather> GetCurrentWeatherAsync(double latitude, double longitude, WeatherUnits? units = null, string? languageCode = null);
    Task<WeatherHourlyForecast> GetFiveDaysThreeHoursForecastAsync(double latitude, double longitude, WeatherUnits? units = null, string? languageCode = null, int? limit = null);
    WeatherDailyForecast GetFiveDaysDailyForecast(WeatherHourlyForecast hourlyForecast);
}