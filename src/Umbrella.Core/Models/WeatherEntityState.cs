using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public enum WeatherCondition
{
    Clear = 1,
    FewClouds = 2,
    BrokenClouds = 3,
    MostlyCloudy = 4,
    Cloudy = 5,
    ScatteredShowers = 10,
    ScatteredTstorms = 11,
    LightRain = 12,
    ModerateRain = 13,
    HeavyRain = 14,
    Tstorms = 15,
    LightSnow = 20,
    HeavySnow = 21,
    RainSnow = 22,
    Blizzard = 23,
    Fog = 30
};

public record WeatherDailyForecast (
    DateTime Date,
    WeatherCondition ConditionCode,
    string Condition,
    string ConditionDescription,
    double TemperatureDay,
    double TemperatureNight,
    double? Pressure,
    double? Precipitation,
    byte? PrecipitationProbability,
    double? Humidity,
    double? WindSpeed,
    double? WindDegree,
    DateTime? Sunrise,
    DateTime? Sunset
);

public record WeatherHourlyForecast (
    DateTime DateTime,
    WeatherCondition ConditionCode,
    string Condition,
    string ConditionDescription,
    double Temperature,
    double? TemperatureFeelsLike,
    double? Pressure,
    double? Precipitation,
    byte? PrecipitationProbability,
    double? Humidity,
    double? WindSpeed,
    double? WindDegree,
    bool isNight
);

public sealed class WeatherEntityState : IEntityState
{
    public bool? Connected { get; set; } = default;
    public DateTime UpdatedAt { get; set; }
    public WeatherCondition ConditionCode { get; set; }
    public string Condition { get; set; }
    public string ConditionDescription { get; set; }
    public double Temperature { get; set; }
    public double? TemperatureFeelsLike { get; set; }
    public string TemperatureUnit { get; set; }
    public double? Pressure { get; set; }
    public string? PressureUnit { get; set; }
    public double? Precipitation { get; set; }
    public string? PrecipitationUnit { get; set; }
    public byte? PrecipitationProbability { get; set; }
    public double? Humidity { get; set; }
    public double? WindSpeed { get; set; }
    public string? WindSpeedUnit { get; set; }
    public double? WindDegree { get; set; }
    public DateTime? Sunrise { get; set; }
    public DateTime? Sunset { get; set; }
    public IEnumerable<WeatherDailyForecast>? DailyForecast { get; set; }
    public IEnumerable<WeatherHourlyForecast>? HourlyForecast { get; set; }

    public IEntityState Clone()
    {
        return new WeatherEntityState
        {
            Connected = Connected,    
            UpdatedAt = UpdatedAt,
            ConditionCode = ConditionCode,
            Condition = Condition,
            ConditionDescription = ConditionDescription,
            Temperature = Temperature,
            TemperatureFeelsLike = TemperatureFeelsLike,
            TemperatureUnit = TemperatureUnit,
            Pressure = Pressure,
            PressureUnit = PressureUnit,
            Precipitation = Precipitation,
            PrecipitationUnit = PrecipitationUnit,
            PrecipitationProbability = PrecipitationProbability,
            Humidity = Humidity,
            WindSpeed = WindSpeed,
            WindSpeedUnit = WindSpeedUnit,
            WindDegree = WindDegree,
            Sunrise = Sunrise,
            Sunset = Sunset,
            DailyForecast = DailyForecast?.Select(f => new WeatherDailyForecast(
                    f.Date,
                    f.ConditionCode,
                    f.Condition,
                    f.ConditionDescription,
                    f.TemperatureDay,
                    f.TemperatureNight,
                    f.Pressure,
                    f.Precipitation,
                    f.PrecipitationProbability,
                    f.Humidity,
                    f.WindSpeed,
                    f.WindDegree,
                    f.Sunrise,
                    f.Sunset
                )).ToList(),
            HourlyForecast = HourlyForecast?.Select(f => new WeatherHourlyForecast(
                    f.DateTime,
                    f.ConditionCode,
                    f.Condition,
                    f.ConditionDescription,
                    f.Temperature,
                    f.TemperatureFeelsLike,
                    f.Pressure,
                    f.Precipitation,
                    f.PrecipitationProbability,
                    f.Humidity,
                    f.WindSpeed,
                    f.WindDegree,
                    f.isNight
                )).ToList(),
        };
    }
    
    public void UpdateProperties(IEntityState state)
    {
        if (state.Connected.HasValue)
        {
            Connected = state.Connected.Value;
        }
        if (state is not WeatherEntityState weatherState)
        {
            return;
        }
        if (UpdatedAt == weatherState.UpdatedAt)
        {
            return;
        }
        Connected = weatherState.Connected;
        UpdatedAt = weatherState.UpdatedAt;
        ConditionCode = weatherState.ConditionCode;
        Condition = weatherState.Condition;
        ConditionDescription = weatherState.ConditionDescription;
        Temperature = weatherState.Temperature;
        TemperatureFeelsLike = weatherState.TemperatureFeelsLike;
        TemperatureUnit = weatherState.TemperatureUnit;
        Pressure = weatherState.Pressure;
        PressureUnit = weatherState.PressureUnit;
        Precipitation = weatherState.Precipitation;
        PrecipitationUnit = weatherState.PrecipitationUnit;
        PrecipitationProbability = weatherState.PrecipitationProbability;
        Humidity = weatherState.Humidity;
        WindSpeed = weatherState.WindSpeed;
        WindSpeedUnit = weatherState.WindSpeedUnit;
        WindDegree = weatherState.WindDegree;
        Sunrise = weatherState.Sunrise;
        Sunset = weatherState.Sunset;
        DailyForecast = weatherState.DailyForecast?.Select(f => new WeatherDailyForecast(
                f.Date,
                f.ConditionCode,
                f.Condition,
                f.ConditionDescription,
                f.TemperatureDay,
                f.TemperatureNight,
                f.Pressure,
                f.Precipitation,
                f.PrecipitationProbability,
                f.Humidity,
                f.WindSpeed,
                f.WindDegree,
                f.Sunrise,
                f.Sunset
            )).ToList();
        HourlyForecast = weatherState.HourlyForecast?.Select(f => new WeatherHourlyForecast(
                f.DateTime,
                f.ConditionCode,
                f.Condition,
                f.ConditionDescription,
                f.Temperature,
                f.TemperatureFeelsLike,
                f.Pressure,
                f.Precipitation,
                f.PrecipitationProbability,
                f.Humidity,
                f.WindSpeed,
                f.WindDegree,
                f.isNight
            )).ToList();
    }
}
