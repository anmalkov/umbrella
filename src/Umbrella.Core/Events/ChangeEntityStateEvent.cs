using Umbrella.Core.Models;

namespace Umbrella.Core.Events;

public sealed class ChangeEntityStateEvent<T> : IEvent where T : IEntityState
{
    public string Name => EventNames.ChangeEntityState;
    public string EntityId { get; init; }
    public T State { get; set; }

    public ChangeEntityStateEvent(string entityId, T state)
    {
        EntityId = entityId;
        State = state;
    }
}
