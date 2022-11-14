using OpenWeatherMap.Sdk;
using OpenWeatherMap.Sdk.Models;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Threading;
using Umbrella.Core.Events;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;
using Umbrella.Core.Services;

namespace Umbrella.Extensions.OpenWeatherMap;

internal record EntityCoordinates(string EntityId, double Latitude, double Longitude);

public class OpenWeatherMapExtension : IExtension
{
    private const string ApiKeyParameterName = "apiKey";
    private const string CitiesParameterName = "cities";
    private const string UnitsParameterName = "units";
    private const char CitiesDivider = ';';
    private const char CountryCodeDivider = ',';
    private const string EntitiesCoordinatesParameterName = "citiesCoordinates";
    private const int UpdateWeatherIntervalInMinutes = 10;

    public string Id => "owm";
    public string? DisplayName => "Open Weather Map";
    public string? Image => "";

    public string? HtmlForRegistration => @$"
  <div class=""mb-3"">
    <label for=""{ApiKeyParameterName}"" class=""form-label"">API key</label>
    <input type=""text"" class=""form-control"" name=""{ApiKeyParameterName}"" aria-describedby=""apiKeyHelp"">
    <div id=""apiKeyHelp"" class=""form-text"">Open <a href=""https://home.openweathermap.org/api_keys"" target=""_blank"" rel=""noopener noreferrer"">My API keys page</a> to get your key</div>
  </div>
  <div class=""mb-3"">
    <label for=""{CitiesParameterName}"" class=""form-label"">Cities</label>
    <input type=""text"" class=""form-control"" name=""{CitiesParameterName}"" aria-describedby=""citiesHelp"">
    <div id=""citiesHelp"" class=""form-text"">Enter a city name and/or a country code divided by '{CountryCodeDivider}'. If you want to enter multiple cities use '{CitiesDivider}' as a divider.<br />Format: [cityName1]{CountryCodeDivider}[CountryCode1]{CitiesDivider}[cityName2]{CountryCodeDivider}[CountryCode2]{CitiesDivider}...{CitiesDivider}[cityNameN]{CountryCodeDivider}[CountryCodeN]</div>
  </div>
  <div class=""mb-3"">
    <label for=""{UnitsParameterName}"" class=""form-label"">Units</label>
    <select class=""form-select"" name=""{UnitsParameterName}"" aria-describedby=""unitsHelp"">
        <option value=""metric"">Metric</option>
        <option value=""imperial"">Imperial</option>
    </select>
    <div id=""unitsHelp"" class=""form-text"">Select <b>Metric</b> units for temperature in Celsius or <b>Imperial</b> units for temperature in Fahrenheit</div>
  </div>";

    private readonly HttpClient _httpClient;
    private readonly IRegistrationService _registrationService;
    private readonly IEventsService _eventsService;

    private IOpenWeatherMapClient? _openWeatherMapClient;
    private IEnumerable<EntityCoordinates> _entitiesCoordinates;
    private bool _unitsIsMetric = true;
    private CancellationTokenSource? _cancellationTokenSource = null;


    public OpenWeatherMapExtension(IRegistrationService coreService, IEventsService eventsService) : this(coreService, eventsService, null)
    {
    }

    public OpenWeatherMapExtension(IRegistrationService coreService, IEventsService eventsService, HttpClient? httpClient)
    {
        if (httpClient is null)
        {
            var handler = new HttpClientHandler()
            {
                //ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(handler);
        }
        else
        {
            _httpClient = httpClient;
        }
        _registrationService = coreService;
        _eventsService = eventsService;
    }

    public OpenWeatherMapExtension(IRegistrationService coreService, IEventsService eventsService, HttpClient? httpClient, IOpenWeatherMapClient openWeatherMapClient)
        : this(coreService, eventsService, httpClient)
    {
        _openWeatherMapClient = openWeatherMapClient;
    }
    

    public async Task RegisterAsync(Dictionary<string, string?>? parameters)
    {
        var apiKey = GetParameter(parameters, ApiKeyParameterName, true);
        var cities = GetParameter(parameters, CitiesParameterName, true);
        var units = GetParameter(parameters, UnitsParameterName, true);
        if (units != "metric" && units != "imperial")
        {
            throw new ArgumentException($"Parameter '{UnitsParameterName}' should be either 'metric' or 'imperial'");
        }

        _openWeatherMapClient = new OpenWeatherMapClient(_httpClient, apiKey!);
        var citiesCoordinates = await GetCitiesCoordinatesAsync(cities!);
        if (!citiesCoordinates.Any())
        {
            throw new ArgumentException($"No cities were found for the provided cities list: {cities}");
        }

        parameters!.Remove(CitiesParameterName);
        parameters.Add(EntitiesCoordinatesParameterName, JsonSerializer.Serialize(citiesCoordinates));
    }

    public Task UnregisterAsync(Dictionary<string, string?>? parameters)
    {
        return Task.CompletedTask;
    }

    public async Task StartAsync(Dictionary<string, string?>? parameters)
    {
        if (parameters is not null && parameters.ContainsKey(UnitsParameterName))
        {
            var units = parameters[UnitsParameterName];
            _unitsIsMetric = string.IsNullOrWhiteSpace(units) || units.ToLower() != "imperial";
        }
        if (parameters is not null && parameters.ContainsKey(ApiKeyParameterName))
        {
            var apiKey = parameters[ApiKeyParameterName];
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                _openWeatherMapClient = new OpenWeatherMapClient(_httpClient, apiKey);
            }
        }
        if (parameters is not null && parameters.ContainsKey(EntitiesCoordinatesParameterName))
        {
            _entitiesCoordinates = JsonSerializer.Deserialize<List<EntityCoordinates>>(parameters?[EntitiesCoordinatesParameterName] ?? "") ?? new List<EntityCoordinates>();
        }

        await ReportCurrentWeatherForEntitiesAsync();

        UpdateWeatherRegularly();
    }

    private async Task UpdateWeatherRegularly()
    {
        var timer = new PeriodicTimer(TimeSpan.FromMinutes(UpdateWeatherIntervalInMinutes));

        _cancellationTokenSource ??= new CancellationTokenSource();

        while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
        {
            await ReportCurrentWeatherForEntitiesAsync();
        }
    }

    public Task StopAsync()
    {
        _cancellationTokenSource?.Cancel();
        return Task.CompletedTask;
    }

    private async Task ReportCurrentWeatherForEntitiesAsync()
    {
        if (_openWeatherMapClient is null || !_entitiesCoordinates.Any())
        {
            return;
        }

        var units = _unitsIsMetric ? WeatherUnits.Metric : WeatherUnits.Imperial;
        foreach (var entityCoordinate in _entitiesCoordinates)
        {
            var currentWeather = await _openWeatherMapClient.GetCurrentWeatherAsync(entityCoordinate.Latitude, entityCoordinate.Longitude, units);
            var hourlyForecast = await _openWeatherMapClient.GetFiveDaysThreeHoursForecastAsync(entityCoordinate.Latitude, entityCoordinate.Longitude, units);
            var dailyForecast = _openWeatherMapClient.GetFiveDaysDailyForecast(hourlyForecast);
            var todayDailyForecast = dailyForecast.DailyForecast.First();

            var state = new WeatherEntityState
            {
                UpdatedAt = currentWeather.UpdatedAt,
                ConditionCode = GetWeatherCondition(currentWeather.WeatherId),
                Condition = currentWeather.WeatherMain,
                ConditionDescription = currentWeather.WeatherDescription,
                Temperature = currentWeather.Temperature,
                TemperatureFeelsLike = currentWeather.TemperatureFeelsLike,
                TemperatureUnit = units == WeatherUnits.Metric ? "°C" : "°F",
                Pressure = currentWeather.Pressure,
                PressureUnit = "hPa",
                Precipitation = GetPrecipitation(todayDailyForecast.Rain, todayDailyForecast.Snow),
                PrecipitationUnit = units == WeatherUnits.Metric ? "mm" : "in",
                PrecipitationProbability = todayDailyForecast.PrecipitationProbability,
                Humidity = currentWeather.Humidity,
                WindSpeed = currentWeather.WindSpeed,
                WindSpeedUnit = units == WeatherUnits.Metric ? "m/s" : "m/h",
                WindDegree = currentWeather.WindDegree,
                Sunrise = currentWeather.Sunrise,
                Sunset = currentWeather.Sunset,
                DailyForecast = dailyForecast.DailyForecast.Select(f => new Core.Models.WeatherDailyForecast(
                    f.ForecastDateTime,
                    GetWeatherCondition(f.WeatherId),
                    f.WeatherMain,
                    f.WeatherDescription,
                    f.TemperatureDay,
                    f.TemperatureNight,
                    f.Pressure,
                    GetPrecipitation(f.Rain, f.Snow),
                    f.PrecipitationProbability,
                    f.Humidity,
                    f.WindSpeed,
                    f.WindDegree,
                    f.Sunrise,
                    f.Sunset
                )),
                HourlyForecast = hourlyForecast.HourlyForecast.Select(f => new Core.Models.WeatherHourlyForecast(
                    f.ForecastDateTime,
                    GetWeatherCondition(f.WeatherId),
                    f.WeatherMain,
                    f.WeatherDescription,
                    f.Temperature,
                    f.TemperatureFeelsLike,
                    f.Pressure,
                    GetPrecipitation(f.RainLastThreeHours, f.SnowLastThreeHours),
                    f.PrecipitationProbability,
                    f.Humidity,
                    f.WindSpeed,
                    f.WindDegree,
                    f.PartOfDay == WeatherPartOfDay.Night
                )),
            };
            _eventsService.Publish(new EntityStateChangedEvent(entityCoordinate.EntityId, state));
        }
    }

    private static WeatherCondition GetWeatherCondition(int weatherId)
    {
        return weatherId switch
        {
            // clouds
            800 => WeatherCondition.Clear,
            801 => WeatherCondition.FewClouds,
            802 => WeatherCondition.BrokenClouds,
            803 => WeatherCondition.MostlyCloudy,
            804 => WeatherCondition.Cloudy,
            _ when weatherId >= 800 && weatherId < 900 => WeatherCondition.Cloudy,

            // rain
            500 => WeatherCondition.ScatteredShowers,
            501 => WeatherCondition.LightRain,
            502 => WeatherCondition.ModerateRain,
            _ when weatherId >= 503 && weatherId <= 531 => WeatherCondition.HeavyRain,
            _ when weatherId >= 300 && weatherId < 600 => WeatherCondition.LightRain,

            // tstorms
            210 => WeatherCondition.ScatteredTstorms,
            _ when weatherId >= 200 && weatherId < 300 => WeatherCondition.Tstorms,

            // snow
            _ when weatherId >= 600 && weatherId <= 601 => WeatherCondition.LightSnow,
            602 => WeatherCondition.HeavySnow,
            _ when weatherId >= 611 && weatherId <= 616 => WeatherCondition.RainSnow,
            622 => WeatherCondition.Blizzard,
            _ when weatherId >= 600 && weatherId < 700 => WeatherCondition.LightSnow,
            
            // fog
            _ when weatherId >= 700 && weatherId < 800 => WeatherCondition.Fog,
            
            _ => WeatherCondition.Clear
        };
    }

    private static double? GetPrecipitation(double? rain, double? snow)
    {
        if (rain is not null && snow is not null)
        {
            return rain > snow ? rain : snow;
        }
        
        return rain is not null ? rain : snow is not null ? snow : null;
    }

    private async Task<IEnumerable<EntityCoordinates>> GetCitiesCoordinatesAsync(string cities)
    {
        if (_openWeatherMapClient is null)
        {
            return new List<EntityCoordinates>();
        }

        var citiesWithCountries = cities.Split(CitiesDivider).Select(p => p.Split(CountryCodeDivider)).Select(p => new Tuple<string, string?>(p[0], p.Length > 1 ? p[1] : null)).ToArray();

        var entitiesCoordinates = new List<EntityCoordinates>();
        foreach (var cityWithCountry in citiesWithCountries)
        {
            var coordinates = (await _openWeatherMapClient.GetGeoCoordinatesAsync(cityWithCountry.Item1, countryCode: cityWithCountry.Item2)).FirstOrDefault();
            if (coordinates is null)
            {
                continue;
            }
            var entity = MapCityToEntity(cityWithCountry.Item1, cityWithCountry.Item2, coordinates);
            await _registrationService.RegisterEntityAsync(entity, Id);
            entitiesCoordinates.Add(new EntityCoordinates(
                entity.Id,
                coordinates.Latitude,
                coordinates.Longitude
            ));
        }
        return entitiesCoordinates;
    }

    private string GenerateEntityId(string name)
    {
        name = name.ToLower().Replace(' ', '_');
        return $"weather.{name}";
    }

    private WeatherEntity MapCityToEntity(string cityName, string? countryCode, GeoCoordinates coordinates)
    {
        return new WeatherEntity(GenerateEntityId(cityName), cityName, coordinates.CityName)
        {
            Enabled = true,
            Name = cityName,
            DesiredCountryCode = countryCode,
            FoundCountryCode = coordinates.CountryCode,
            Latitude = coordinates.Latitude,
            Longitude = coordinates.Longitude,
        };
    }
    
    private static string? GetParameter(Dictionary<string, string?>? parameters, string parameterName, bool parameterRequired)
    {
        if (parameters is null || !parameters.ContainsKey(parameterName))
        {
            if (!parameterRequired)
            {
                return null;
            }
            throw new ArgumentException($"Missing required parameter '{parameterName}'");
        }

        var value = parameters[parameterName];
        if (parameterRequired && string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Parameter '{parameterName}' must have a value");
        }

        return value;
    }

}