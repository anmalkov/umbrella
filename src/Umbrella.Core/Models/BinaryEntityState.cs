using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Models;

public sealed class BinaryEntityState : IEntityState
{
    public bool? Connected { get; set; } = default;
    public bool? IsOn { get; set; }

    public IEntityState Clone()
    {
        return new BinaryEntityState
        {
            Connected = Connected,
            IsOn = IsOn
        };
    }

    public void UpdateProperties(IEntityState state)
    {
        if (state.Connected.HasValue)
        {
            Connected = state.Connected.Value;
        }
        if (state is not BinaryEntityState binaryState)
        {
            return;
        }
        
        if (binaryState.IsOn.HasValue)
        {
            IsOn = binaryState.IsOn;
        }
    }
}
