using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public class Dashboard : IStorableItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Icon { get; set; }

    public List<Widget> Widgets { get; set; } = new();

    public Dashboard(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
