using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public enum BinaryEntityType
{
    Opening = 1,
    Motion = 2
}

public sealed class BinaryEntity : EntityBase
{
    public BinaryEntityType BinaryType { get; set; } = default;
    
    public BinaryEntity(string id, BinaryEntityType binaryType) : base(id, EntityType.Binary)
    {
        BinaryType = binaryType;
    }
}
