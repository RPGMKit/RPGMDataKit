namespace RPGMDataKit.Data.Enums;

/// <summary>How an event moves by itself (<c>MoveType</c> of a page). The same numbers in VX Ace and MV.</summary>
public static class EventMoveType
{
    public const int Fixed = 0;
    public const int Random = 1;
    public const int Approach = 2;
    public const int Custom = 3;
}
