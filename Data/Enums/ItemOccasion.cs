namespace RPGMDataKit.Data.Enums;

/// <summary>When an item or a skill can be used (<c>Occasion</c> of a usable item). The same numbers in VX Ace and MV.</summary>
public static class ItemOccasion
{
    public const int Always = 0;
    public const int BattleOnly = 1;
    public const int MenuOnly = 2;
    public const int Never = 3;
}
