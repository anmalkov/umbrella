using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public sealed class LightEntity : EntityBase
{
    public bool TurnedOn { get; set; }
    public byte Brightness { get; set; }
    public int ColorTemperature { get; set; }
    public int MinColorTemperature { get; set; }
    public int MaxColorTemperature { get; set; }
    public Tuple<byte, byte, byte>? RgbColor { get; set; }

    public LightEntity(string id) : base(id, EntityType.Light) { }
}
