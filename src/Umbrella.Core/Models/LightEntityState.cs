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
}
