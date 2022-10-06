using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public sealed class LightEntity : EntityBase
{
    public int? MinColorTemperature { get; set; }
    public int? MaxColorTemperature { get; set; }

    public LightEntity(string id) : base(id, EntityType.Light) { }
}
