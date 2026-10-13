namespace RPGMDataKit.Data.Enums;

/// <summary>When a state runs out by itself (<c>AutoRemovalTiming</c> of a state). The same numbers in VX Ace and MV.</summary>
public static class AutoRemovalTiming
{
    public const int None = 0;
    public const int ActionEnd = 1;
    public const int TurnEnd = 2;
}
