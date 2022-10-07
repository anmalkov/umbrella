using Umbrella.Core.Models;

namespace Umbrella.Core.Events;

public sealed class EntityStateChangedEvent : IEvent 
{
    public string Name => EventNames.EntityStateChanged;
    public string EntityId { get; init; }
    public IEntityState State { get; init; }

    public EntityStateChangedEvent(string entityId, IEntityState state)
    {
        EntityId = entityId;
        State = state;
    }
}
