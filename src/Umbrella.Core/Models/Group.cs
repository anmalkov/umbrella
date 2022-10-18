using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public class Group : IStorableItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Icon { get; set; }

    public List<string> Entities { get; set; } = new(); 

    public Group(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
