using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public sealed class TemperatureEntity : EntityBase
{
    public TemperatureEntity(string id) : base(id, EntityType.Temperature) { }
}
