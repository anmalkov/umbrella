using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public interface IEntity
{
    string Id { get; }
    string? Name { get; }
    string? Icon { get; }
    string? Owner { get; set; }
    bool Enabled { get; }
    EntityType Type { get; }
}
