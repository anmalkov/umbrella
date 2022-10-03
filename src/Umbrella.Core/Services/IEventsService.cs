using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Events;

namespace Umbrella.Core.Services;

public interface IEventsService
{
    void Subscribe(string eventName, Action<IEvent?> eventHandler);
    void Unsubscribe(string eventName, Action<IEvent?> eventHandler);

    void PublishAsync(string eventName, IEvent? data);
}
