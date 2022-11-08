namespace OpenWeatherMap.Sdk.Models;

public record DailyForecast(
    DateTime ForecastDateTime,
    DateTime Sunrise,
    DateTime Sunset,
    int WeatherId,
    string WeatherMain,
    string WeatherDescription,
    string WeatherIcon,
    double TemperatureDay,
    double TemperatureDayFeelsLike,
    double TemperatureMin,
    double TemperatureMax,
    double TemperatureNight,
    double TemperatureNightFeelsLike,
    double TemperatureEvening,
    double TemperatureEveningFeelsLike,
    double TemperatureMorning,
    double TemperatureMorningFeelsLike,
    double Pressure,
    byte Humidity,
    double Visibility,
    double WindSpeed,
    int WindDegree,
    double WindGust,
    byte Cloudiness,
    byte PrecipitationProbability,
    double? Rain,
    double? Snow
);

public record WeatherDailyForecast (
    int CityId,
    string CityName,
    double Latitude,
    double Longitude,
    string CountryCode,
    int Population,
    int TimeZone,  // shift in seconds from UTC
    IEnumerable<DailyForecast> DailyForecast
);
