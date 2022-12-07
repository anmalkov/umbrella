using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public sealed class TemperatureEntityState : IEntityState
{
    public bool? Connected { get; set; } = default;
    public byte? BatteryLevel { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }

    public IEntityState Clone()
    {
        return new TemperatureEntityState
        {
            Connected = Connected,
            BatteryLevel = BatteryLevel,
            Temperature = Temperature,
            Humidity = Humidity
        };
    }

    public void UpdateProperties(IEntityState state)
    {
        if (state.Connected.HasValue)
        {
            Connected = state.Connected.Value;
        }
        if (state is not TemperatureEntityState temperatureState)
        {
            return;
        }
        
        if (temperatureState.BatteryLevel.HasValue)
        {
            BatteryLevel = temperatureState.BatteryLevel;
        }
        if (temperatureState.Temperature.HasValue)
        {
            Temperature = temperatureState.Temperature;
        }
        if (temperatureState.Humidity.HasValue)
        {
            Humidity = temperatureState.Humidity;
        }
    }
}
