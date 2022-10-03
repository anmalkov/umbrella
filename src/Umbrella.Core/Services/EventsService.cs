using System.Collections.Concurrent;
using Umbrella.Core.Events;

namespace Umbrella.Core.Services;

public sealed class EventsService : IEventsService
{
    private readonly ConcurrentDictionary<string, List<Action<IEvent?>>> _subscriptions = new();
    
    public EventsService()
    {
        
    }

    public void Publish(string eventName, IEvent? data)
    {
        if (!_subscriptions.ContainsKey(eventName) || _subscriptions[eventName] is null)
        {
            return;
        }
        
        var actions = _subscriptions[eventName];
        foreach ( var action in actions)
        {
            action.Invoke(data);
        }
    }

    public void Subscribe(string eventName, Action<IEvent?> eventHandler)
    {
        if (!_subscriptions.ContainsKey(eventName))
        {
            _subscriptions.TryAdd(eventName, new List<Action<IEvent?>> { eventHandler });
            return;
        }

        if (_subscriptions[eventName] is null)
        {
            _subscriptions[eventName] = new List<Action<IEvent?>>();
        }

        var actions = _subscriptions[eventName];
        if (actions.Any(a => a == eventHandler))
        {
            return;
        }
        
        _subscriptions[eventName].Add(eventHandler);
    }

    public void Unsubscribe(string eventName, Action<IEvent?> eventHandler)
    {
        if (!_subscriptions.ContainsKey(eventName) || _subscriptions[eventName] is null)
        {
            return;
        }

        var actions = _subscriptions[eventName];
        if (!actions.Any(a => a == eventHandler))
        {
            return;
        }

        _subscriptions[eventName].Remove(eventHandler);
    }
}