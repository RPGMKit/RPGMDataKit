namespace RPGMDataKit.Data.Enums;

/// <summary>Whether the map loops (<c>ScrollType</c> of a map). The same numbers in VX Ace and MV (checked in <c>loop_horizontal?</c> and <c>isLoopHorizontal</c>).</summary>
public static class MapScrollType
{
    public const int NoLoop = 0;
    public const int LoopVertically = 1;
    public const int LoopHorizontally = 2;
    public const int LoopBoth = 3;
}
