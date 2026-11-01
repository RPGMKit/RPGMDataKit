namespace RPGMDataKit.Data.Enums;

/// <summary>How an event starts (<c>Trigger</c> of a page). The same numbers in VX Ace and MV.</summary>
public enum EventTriggerType
{
    ActionButton = 0,
    PlayerTouch = 1,
    EventTouch = 2,
    Autorun = 3,
    Parallel = 4,
}
