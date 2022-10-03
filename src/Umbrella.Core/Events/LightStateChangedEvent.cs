﻿namespace Umbrella.Core.Events;

public sealed class LightStateChangedEvent : IEvent
{
    public string EventName => EventNames.LightStateChanged;
    public bool TurnedOn { get; private set; }
    public byte Brightness { get; private set; }
    public int ColorTemperature { get; private set; }
}
