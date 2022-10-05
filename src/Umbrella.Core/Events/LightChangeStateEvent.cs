namespace Umbrella.Core.Events;

public sealed class LightChangeStateEvent : IEvent
{
    public string Name => EventNames.LightChangeState;
    public string EntityId { get; init; }
    public bool? TurnedOn { get; set; }
    public byte? Brightness { get; set; }
    public int? ColorTemperature { get; set; }

    public LightChangeStateEvent(string entityId, bool? turnedOn = null)
    {
        EntityId = entityId;
        TurnedOn = turnedOn;
    }
}
