namespace Umbrella.Core.Events;

public sealed class LightStateChangedEvent : IEvent
{
    public string Name => EventNames.LightStateChanged;
    public string EntityId { get; init; }
    public bool? TurnedOn { get; private set; }
    public byte? Brightness { get; private set; }
    public int? ColorTemperature { get; private set; }

    public LightStateChangedEvent(string entityId, bool? turnedOn = null)
    {
        EntityId = entityId;
        TurnedOn = turnedOn;
    }
}
