using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public record WeatherDailyForecast (
    DateTime Date,
    string Condition,
    double TemperatureDay,
    double TemperatureNight,
    double? Pressure,
    double? Precipitation,
    byte? PrecipitationProbability,
    double? Humidity,
    double? WindSpeed,
    double? WindDegree
);

public record WeatherHourlyForecast (
    DateTime DateTime,
    string Condition,
    double Temperature,
    double? TemperatureFeelsLike,
    double? Pressure,
    double? Precipitation,
    byte? PrecipitationProbability,
    double? Humidity,
    double? WindSpeed,
    double? WindDegree
);

public sealed class WeatherEntityState : IEntityState
{
    public bool? Connected { get; set; } = default;
    public string UpdatedAt { get; set; }
    public string Condition { get; set; }
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
    public IEnumerable<WeatherDailyForecast>? DailyForecast { get; set; }
    public IEnumerable<WeatherHourlyForecast>? HourlyForecast { get; set; }


    public IEntityState Clone()
    {
        return new WeatherEntityState
        {
            Connected = Connected,    
            UpdatedAt = UpdatedAt,
            Condition = Condition,
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
            DailyForecast = DailyForecast?.Select(f => new WeatherDailyForecast(
                    f.Date,
                    f.Condition,
                    f.TemperatureDay,
                    f.TemperatureNight,
                    f.Pressure,
                    f.Precipitation,
                    f.PrecipitationProbability,
                    f.Humidity,
                    f.WindSpeed,
                    f.WindDegree
                )).ToList(),
            HourlyForecast = HourlyForecast?.Select(f => new WeatherHourlyForecast(
                    f.DateTime,
                    f.Condition,
                    f.Temperature,
                    f.TemperatureFeelsLike,
                    f.Pressure,
                    f.Precipitation,
                    f.PrecipitationProbability,
                    f.Humidity,
                    f.WindSpeed,
                    f.WindDegree
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
    }
}
