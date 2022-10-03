namespace Umbrella.Core.Events;

public sealed class LightChangeStateEvent : IEvent
{
    public string EventName => EventNames.LightChangeState;
    public bool TurnedOn { get; private set; }
    public byte Brightness { get; private set; }
    public int ColorTemperature { get; private set; }
}
