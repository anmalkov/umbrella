using OpenWeatherMap.Sdk.Exceptions;
using OpenWeatherMap.Sdk.Models;
using System.Text.Json;

namespace OpenWeatherMap.Sdk;

public class OpenWeatherMapClient : IOpenWeatherMapClient
{
    private readonly HttpClient _httpClient;

    private string _apiKey;

    public OpenWeatherMapClient(HttpClient httpClient, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _apiKey = apiKey;
    }


    public async Task<IEnumerable<GeoCoordinates>> GetGeoCoordinatesAsync(string cityName, string? stateCode = null, string? countryCode = null, int? limit = null)
    {
        var url = GetUrlForGeoCoordinatesRequest(cityName, stateCode, countryCode, limit);
        var json = await GetJsonAsync(url);
        return ParseJsonToGeoCoordinates(json);
    }

    public async Task<CurrentWeather> GetCurrentWeatherAsync(double latitude, double longitude, WeatherUnits? units = null, string? languageCode = null)
    {
        var url = GetUrlForCurrentWeatherRequest(latitude, longitude, units, languageCode);
        var json = await GetJsonAsync(url);
        return ParseJsonToCurrentWeather(json);
    }

    public async Task<WeatherHourlyForecast> GetFiveDaysThreeHoursForecastAsync(double latitude, double longitude, WeatherUnits? units = null, string? languageCode = null, int? limit = null)
    {
        var url = GetUrlForFiveDaysThreeHoursForecastRequest(latitude, longitude, units, languageCode, limit);
        var json = await GetJsonAsync(url);
        return ParseJsonToWeatherHourlyForecast(json);
    }

    public WeatherDailyForecast GetFiveDaysDailyForecast(WeatherHourlyForecast hourlyForecast)
    {
        var weatherDailyForecast = new WeatherDailyForecast(
            hourlyForecast.CityId,
            hourlyForecast.CityName,
            hourlyForecast.Latitude,
            hourlyForecast.Longitude,
            hourlyForecast.CountryCode,
            hourlyForecast.Population,
            hourlyForecast.TimeZone,
            GetDailyForecast(hourlyForecast)
        );
        return weatherDailyForecast;
    }

    private IEnumerable<DailyForecast> GetDailyForecast(WeatherHourlyForecast hourlyForecast)
    {
        var dailyForecasts = new List<DailyForecast>();
        var days = hourlyForecast.HourlyForecast.Select(f => f.ForecastDateTime.Date).Distinct().ToArray();
        foreach (var day in days)
        {
            var hourlyForecastWholeDay = hourlyForecast.HourlyForecast.Where(f => f.ForecastDateTime.Date == day).ToArray();
            var hourlyForecastEvening = hourlyForecastWholeDay.Where(f => f.ForecastDateTime.TimeOfDay >= TimeSpan.FromHours(19) && f.ForecastDateTime.TimeOfDay <= TimeSpan.FromHours(21)).ToArray();
            var hourlyForecastMorning = hourlyForecastWholeDay.Where(f => f.ForecastDateTime.TimeOfDay >= TimeSpan.FromHours(7) && f.ForecastDateTime.TimeOfDay <= TimeSpan.FromHours(9)).ToArray();
            var hourlyForecastDay = hourlyForecastWholeDay.Where(f => f.PartOfDay == WeatherPartOfDay.Day).ToArray();
            var hourlyForecastNight = hourlyForecastWholeDay.Where(f => f.PartOfDay == WeatherPartOfDay.Night).ToArray();
            var maxWindSpeedForecast = hourlyForecastWholeDay.Where(f => f.WindSpeed == hourlyForecastWholeDay.Select(f => f.WindSpeed).Max()).First();
            var minWeatherIdForecast = hourlyForecastWholeDay.Where(f => f.WeatherId == hourlyForecastWholeDay.Select(f => f.WeatherId).Min()).First();
            var rainForecast = hourlyForecastWholeDay.Where(f => f.RainLastThreeHours.HasValue).ToArray();
            var snowForecast = hourlyForecastWholeDay.Where(f => f.SnowLastThreeHours.HasValue).ToArray();

            var daylyForecast = new DailyForecast(
                day,
                hourlyForecast.Sunrise,
                hourlyForecast.Sunset,
                minWeatherIdForecast.WeatherId,
                minWeatherIdForecast.WeatherMain,
                minWeatherIdForecast.WeatherDescription,
                minWeatherIdForecast.WeatherIcon,
                hourlyForecastDay.Length > 0 ? hourlyForecastDay.Select(f => f.Temperature).Max() : hourlyForecastWholeDay.Select(f => f.Temperature).Max(),
                hourlyForecastDay.Length > 0 ? hourlyForecastDay.Select(f => f.TemperatureFeelsLike).Max() : hourlyForecastWholeDay.Select(f => f.TemperatureFeelsLike).Max(),
                hourlyForecastWholeDay.Select(f => f.TemperatureMin).Min(),
                hourlyForecastWholeDay.Select(f => f.TemperatureMax).Max(),
                hourlyForecastNight.Length > 0 ? hourlyForecastNight.Select(f => f.Temperature).Min() : hourlyForecastWholeDay.Select(f => f.Temperature).Min(),
                hourlyForecastNight.Length > 0 ? hourlyForecastNight.Select(f => f.TemperatureFeelsLike).Min() : hourlyForecastWholeDay.Select(f => f.TemperatureFeelsLike).Min(),
                hourlyForecastEvening.Length > 0 ? hourlyForecastEvening.Select(f => f.Temperature).Average() : hourlyForecastWholeDay.Select(f => f.Temperature).Average(),
                hourlyForecastEvening.Length > 0 ? hourlyForecastEvening.Select(f => f.TemperatureFeelsLike).Average() : hourlyForecastWholeDay.Select(f => f.TemperatureFeelsLike).Average(),
                hourlyForecastMorning.Length > 0 ? hourlyForecastMorning.Select(f => f.Temperature).Average() : hourlyForecastWholeDay.Select(f => f.Temperature).Average(),
                hourlyForecastMorning.Length > 0 ? hourlyForecastMorning.Select(f => f.TemperatureFeelsLike).Average() : hourlyForecastWholeDay.Select(f => f.TemperatureFeelsLike).Average(),
                hourlyForecastWholeDay.Select(f => f.Pressure).Max(),
                hourlyForecastWholeDay.Select(f => f.Humidity).Max(),
                hourlyForecastWholeDay.Select(f => f.Visibility).Min(),
                maxWindSpeedForecast.WindSpeed,
                maxWindSpeedForecast.WindDegree,
                hourlyForecastWholeDay.Where(f => f.WindGust.HasValue).Select(f => f.WindGust!.Value).Max(),
                (byte)Math.Round(hourlyForecastWholeDay.Select(f => (int)f.Cloudiness).Average()),
                hourlyForecastWholeDay.Select(f => f.PrecipitationProbability).Max(),
                rainForecast.Length > 0 ? rainForecast.Select(f => f.RainLastThreeHours!.Value).Max() : null,
                snowForecast.Length > 0 ? snowForecast.Select(f => f.SnowLastThreeHours!.Value).Max() : null
            );
            dailyForecasts.Add(daylyForecast);
        }
        return dailyForecasts;
    }

    private static string GetUnitsPart(WeatherUnits? units)
    {
        return units.HasValue
            ? units switch
            {
                WeatherUnits.Imperial => "&units=imperial",
                WeatherUnits.Metric => "&units=metric",
                _ => ""
            }
            : "";
    }

    private static string GetLanguagePart(string? languageCode)
    {
        return string.IsNullOrWhiteSpace(languageCode) ? "" : $"&lang={languageCode}";
    }


    private static string GetUrlForGeoCoordinatesRequest(string cityName, string? stateCode, string? countryCode, int? limit)
    {
        var stateCodePart = string.IsNullOrWhiteSpace(stateCode) ? "" : "," + stateCode;
        var countryCodePart = string.IsNullOrWhiteSpace(countryCode) ? "" : "," + countryCode;
        var limitPart = limit.HasValue ? $"&limit={limit}" : "";
        var url = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}{stateCodePart}{countryCodePart}{limitPart}";
        return url;
    }

    private static string GetUrlForCurrentWeatherRequest(double latitude, double longitude, WeatherUnits? units, string? languageCode)
    {
        var unitsPart = GetUnitsPart(units);
        string languagePart = GetLanguagePart(languageCode);
        return $"http://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}{unitsPart}{languagePart}";
    }

    private static string GetUrlForFiveDaysThreeHoursForecastRequest(double latitude, double longitude, WeatherUnits? units, string? languageCode, int? limit)
    {
        var unitsPart = GetUnitsPart(units);
        var languagePart = GetLanguagePart(languageCode);
        var limitPart = limit.HasValue ? $"&cnt={limit}" : "";
        return $"http://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}{unitsPart}{languagePart}{limitPart}";
    }

    private static IEnumerable<GeoCoordinates> ParseJsonToGeoCoordinates(string json)
    {
        var geoCoordinates = new List<GeoCoordinates>();
        using var document = JsonDocument.Parse(json);
        foreach (var element in document.RootElement.EnumerateArray())
        {
            var stateIsPresent = element.TryGetProperty("state", out var stateElement);
            var coordinates = new GeoCoordinates(
                 element.GetProperty("name").GetString()!,
                 element.GetProperty("lat").GetDouble(),
                 element.GetProperty("lon").GetDouble(),
                 element.GetProperty("country").GetString()!,
                 stateIsPresent ? stateElement.GetString() : null,
                 element.GetProperty("local_names").EnumerateObject().ToDictionary(e => e.Name, e => e.Value.GetString()!)
            );
            geoCoordinates.Add(coordinates);
        }
        return geoCoordinates;
    }

    private static CurrentWeather ParseJsonToCurrentWeather(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var coord = root.GetProperty("coord");
        var sys = root.GetProperty("sys");
        var main = root.GetProperty("main");
        var wind = root.GetProperty("wind");
        JsonElement? rain = root.TryGetProperty("rain", out _) ? root.GetProperty("rain") : null;
        JsonElement? snow = root.TryGetProperty("snow", out _) ? root.GetProperty("snow") : null;
        var weather = root.GetProperty("weather").EnumerateArray().First();
        var currentWeather = new CurrentWeather(
            DateTimeOffset.FromUnixTimeSeconds(root.GetProperty("dt").GetInt32()).DateTime,
            root.GetProperty("id").GetInt32(),
            root.GetProperty("name").GetString()!,
            coord.GetProperty("lat").GetDouble(),
            coord.GetProperty("lon").GetDouble(),
            sys.GetProperty("country").GetString()!,
            DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunrise").GetInt32()).DateTime,
            DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunset").GetInt32()).DateTime,
            root.GetProperty("timezone").GetInt32(),
            weather.GetProperty("id").GetInt32(),
            weather.GetProperty("main").GetString()!,
            weather.GetProperty("description").GetString()!,
            weather.GetProperty("icon").GetString()!,
            main.GetProperty("temp").GetDouble(),
            main.GetProperty("feels_like").GetDouble(),
            main.GetProperty("temp_min").GetDouble(),
            main.GetProperty("temp_max").GetDouble(),
            main.GetProperty("pressure").GetDouble(),
            main.TryGetProperty("sea_level", out var seaLevel) ? seaLevel.GetDouble() : null,
            main.TryGetProperty("grnd_level", out var groundLevel) ? groundLevel.GetDouble() : null,
            main.GetProperty("humidity").GetByte(),
            root.GetProperty("visibility").GetDouble(),
            wind.GetProperty("speed").GetDouble(),
            wind.GetProperty("deg").GetInt32(),
            wind.TryGetProperty("gust", out var gust) ? gust.GetDouble() : null,
            root.GetProperty("clouds").GetProperty("all").GetByte(),
            rain is not null ? rain.Value.GetProperty("1h").GetDouble() : null,
            rain is not null ? rain.Value.GetProperty("3h").GetDouble() : null,
            snow is not null ? snow.Value.GetProperty("1h").GetDouble() : null,
            snow is not null ? snow.Value.GetProperty("3h").GetDouble() : null
        );
        return currentWeather;
    }

    private WeatherHourlyForecast ParseJsonToWeatherHourlyForecast(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var hourlyForecast = new List<HourlyForecast>();
        foreach (var element in root.GetProperty("list").EnumerateArray())
        {
            var main = element.GetProperty("main");
            var wind = element.GetProperty("wind");
            var weather = element.GetProperty("weather").EnumerateArray().First();
            JsonElement? rain = element.TryGetProperty("rain", out _) ? element.GetProperty("rain") : null;
            JsonElement? snow = element.TryGetProperty("snow", out _) ? element.GetProperty("snow") : null;
            var sys = element.GetProperty("sys");
            var forecast = new HourlyForecast(
                DateTimeOffset.FromUnixTimeSeconds(element.GetProperty("dt").GetInt32()).DateTime,
                weather.GetProperty("id").GetInt32(),
                weather.GetProperty("main").GetString()!,
                weather.GetProperty("description").GetString()!,
                weather.GetProperty("icon").GetString()!,
                main.GetProperty("temp").GetDouble(),
                main.GetProperty("feels_like").GetDouble(),
                main.GetProperty("temp_min").GetDouble(),
                main.GetProperty("temp_max").GetDouble(),
                main.GetProperty("pressure").GetDouble(),
                main.TryGetProperty("sea_level", out var seaLevel) ? seaLevel.GetDouble() : null,
                main.TryGetProperty("grnd_level", out var groundLevel) ? groundLevel.GetDouble() : null,
                main.GetProperty("humidity").GetByte(),
                element.GetProperty("visibility").GetDouble(),
                wind.GetProperty("speed").GetDouble(),
                wind.GetProperty("deg").GetInt32(),
                wind.TryGetProperty("gust", out var gust) ? gust.GetDouble() : null,
                element.GetProperty("clouds").GetProperty("all").GetByte(),
                (byte)(element.GetProperty("pop").GetDouble() * 100),
                rain is not null ? rain.Value.GetProperty("3h").GetDouble() : null,
                snow is not null ? snow.Value.GetProperty("3h").GetDouble() : null,
                sys.GetProperty("pod").GetString() == "d" ? WeatherPartOfDay.Day : WeatherPartOfDay.Night
            );
            hourlyForecast.Add(forecast);
        }

        var city = root.GetProperty("city");
        var coord = city.GetProperty("coord");
        var weatherForecast = new WeatherHourlyForecast(
            city.GetProperty("id").GetInt32(),
            city.GetProperty("name").GetString()!,
            coord.GetProperty("lat").GetDouble(),
            coord.GetProperty("lon").GetDouble(),
            city.GetProperty("country").GetString()!,
            DateTimeOffset.FromUnixTimeSeconds(city.GetProperty("sunrise").GetInt32()).DateTime,
            DateTimeOffset.FromUnixTimeSeconds(city.GetProperty("sunset").GetInt32()).DateTime,
            city.GetProperty("population").GetInt32(),
            city.GetProperty("timezone").GetInt32(),
            hourlyForecast
        );
        return weatherForecast;
    }

    private async Task<string> GetJsonAsync(string url)
    {
        url = $"{url}&appid={_apiKey}";
        
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new OpenWeatherMapException($"Response from the OpenWeatherMap API has unexpected HTTP status code: {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync();
    }
}