using Umbrella.Core.Models;

namespace Umbrella.Core.Events;

public sealed class ChangeEntityStateEvent : IEvent
{
    public string Name => EventNames.ChangeEntityState;
    public string EntityId { get; init; }
    public IEntityState State { get; set; }

    public ChangeEntityStateEvent(string entityId, IEntityState state)
    {
        EntityId = entityId;
        State = state;
    }
}
