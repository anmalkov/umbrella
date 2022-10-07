using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public sealed class LightEntityState : IEntityState
{
    public bool? Connected { get; set; } = default;
    public bool? TurnedOn { get; set; }
    public byte? Brightness { get; set; }
    public int? ColorTemperature { get; set; }
    public Tuple<byte, byte, byte>? RgbColor { get; set; }

    public IEntityState Clone()
    {
        return new LightEntityState
        {
            Connected = Connected,
            TurnedOn = TurnedOn,
            Brightness = Brightness,
            ColorTemperature = ColorTemperature,
            RgbColor = RgbColor
        };
    }

    public void UpdateProperties(IEntityState state)
    {
        if (state.Connected.HasValue)
        {
            Connected = state.Connected.Value;
        }
        if (state is not LightEntityState lightState)
        {
            return;
        }
        
        if (lightState.TurnedOn.HasValue)
        {
            TurnedOn = lightState.TurnedOn;
        }
        if (lightState.Brightness.HasValue)
        {
            Brightness = lightState.Brightness;
        }
        if (lightState.ColorTemperature.HasValue)
        {
            ColorTemperature = lightState.ColorTemperature;
        }
        if (lightState.RgbColor is not null)
        {
            RgbColor = lightState.RgbColor;
        }
    }
}
