using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public sealed class LightEntityState : IEntityState
{
    public bool Available { get; set; }
    public bool? TurnedOn { get; set; }
    public byte? Brightness { get; set; }
    public int? ColorTemperature { get; set; }
    public Tuple<byte, byte, byte>? RgbColor { get; set; }

    public LightEntityState(bool available = true)
    {
        Available = available;
    }
}
