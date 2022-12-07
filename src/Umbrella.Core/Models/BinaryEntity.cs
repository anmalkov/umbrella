using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public sealed class BinaryEntity : EntityBase
{
    public BinaryEntity(string id) : base(id, EntityType.Binary) { }
}
