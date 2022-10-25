using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public class Widget
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public byte Column { get; set; }
    public byte PositionInColumn { get; set; }
    public string Type { get; set; }
    public IEnumerable<string> TargetIds { get; set; }

    public Widget(int id, byte column, byte positionInColumn, string type, IEnumerable<string> targetIds)
    {
        Id = id;
        Column = column;
        PositionInColumn = positionInColumn;
        Type = type;
        TargetIds = targetIds;
    }
}
