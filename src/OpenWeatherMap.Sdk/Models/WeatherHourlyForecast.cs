namespace OpenWeatherMap.Sdk.Models;

public enum WeatherPartOfDay
{
    Day,
    Night
}

public record HourlyForecast(
    DateTime ForecastDateTime,
    int WeatherId,
    string WeatherMain,
    string WeatherDescription,
    string WeatherIcon,
    double Temperature,
    double TemperatureFeelsLike,
    double TemperatureMin,
    double TemperatureMax,
    double Pressure,
    double? PressureSeaLevel,
    double? PressureGroundLevel,
    byte Humidity,
    double Visibility,
    double WindSpeed,
    int WindDegree,
    double? WindGust,
    byte Cloudiness,
    byte PrecipitationProbability,
    double? RainLastThreeHours,
    double? SnowLastThreeHours,
    WeatherPartOfDay PartOfDay
);

public record WeatherHourlyForecast (
    int CityId,
    string CityName,
    double Latitude,
    double Longitude,
    string CountryCode,
    DateTime Sunrise,
    DateTime Sunset,
    int Population,
    int TimeZone,  // shift in seconds from UTC
    IEnumerable<HourlyForecast> HourlyForecast
);
