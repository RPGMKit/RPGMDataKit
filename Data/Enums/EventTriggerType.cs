namespace RPGMDataKit.Data.Enums;

/// <summary>How an event starts (<c>Trigger</c> of a page). The same numbers in VX Ace and MV.</summary>
public class EventTriggerType
{
    public const int ActionButton = 0;
    public const int PlayerTouch = 1;
    public const int EventTouch = 2;
    public const int Autorun = 3;
    public const int Parallel = 4;
}