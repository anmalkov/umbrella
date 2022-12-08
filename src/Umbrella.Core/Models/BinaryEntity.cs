using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public enum BinaryEntityType
{
    Opening,
    Motion
}

public sealed class BinaryEntity : EntityBase
{
    public BinaryEntityType Type { get; set; } = default;
    
    public BinaryEntity(string id, BinaryEntityType type) : base(id, EntityType.Binary)
    {
        Type = type;
    }
}
