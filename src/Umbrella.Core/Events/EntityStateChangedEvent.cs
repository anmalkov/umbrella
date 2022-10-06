using Umbrella.Core.Models;

namespace Umbrella.Core.Events;

public sealed class EntityStateChangedEvent<T> : IEvent where T : IEntityState
{
    public string Name => EventNames.EntityStateChanged;
    public string EntityId { get; init; }
    public T State { get; init; }

    public EntityStateChangedEvent(string entityId, T state)
    {
        EntityId = entityId;
        State = state;
    }
}
