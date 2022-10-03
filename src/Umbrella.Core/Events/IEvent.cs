using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Events;

public interface IEvent
{
    string EventName { get; }
}
