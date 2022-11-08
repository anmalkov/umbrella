namespace OpenWeatherMap.Sdk.Models;

public record CurrentWeather(
    DateTime UpdatedAt,
    int CityId,
    string CityName,
    double Latitude,
    double Longitude,
    string CountryCode,
    DateTime Sunrise,
    DateTime Sunset,
    int TimeZone,  // shift in seconds from UTC
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
    double? RainLastOneHour,
    double? RainLastThreeHours,
    double? SnowLastOneHour,
    double? SnowLastThreeHours
);
