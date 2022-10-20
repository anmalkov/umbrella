using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Events;
using Umbrella.Core.Models;

namespace Umbrella.Core.Services;

public class StateHistory
{
    public IEntityState? OldState { get; set; }
    public IEntityState State { get; set; }

    public StateHistory(IEntityState state, IEntityState? oldState = default)
    {
        State = state;
        OldState = oldState;   
    }
}

public class EntitiesStateService : IEntitiesStateService
{
    private readonly IEventsService _eventsService;
    private readonly ConcurrentDictionary<string, StateHistory> _state = new();

    public Action<string, IEntityState>? EntityStateUpdated { get; set; } = null;

    public EntitiesStateService(IEventsService eventsService)
    {
        _eventsService = eventsService;
    }

    public IEntityState? GetState(string id)
    {
        return _state.ContainsKey(id) ? _state[id].State : default;
    }

    public IDictionary<string, IEntityState>? GetStates()
    {
        if (!_state.Any())
        {
            return default;
        }
        return _state.ToDictionary(s => s.Key, s => s.Value.State);
    }

    public void StartMonitoring()
    {
        _eventsService.Subscribe(EventNames.EntityStateChanged, OnChangeState);
    }

    public void StopMonitoring()
    {
        _eventsService.Unsubscribe(EventNames.EntityStateChanged, OnChangeState);
    }


    private void OnChangeState(IEvent? payload)
    {
        if (payload is null || payload is not EntityStateChangedEvent stateChangedEvent)
        {
            return;
        }

        var entityId = stateChangedEvent.EntityId;
        if (!_state.ContainsKey(entityId))
        {
            _state.TryAdd(entityId, new StateHistory(stateChangedEvent.State));
            return;
        }

        var stateHistory = _state[entityId];
        stateHistory.OldState = stateHistory.State.Clone();
        stateHistory.State.UpdateProperties(stateChangedEvent.State);

        if (EntityStateUpdated is not null)
        {
            EntityStateUpdated.Invoke(entityId, stateHistory.State);
        }
    }
}
