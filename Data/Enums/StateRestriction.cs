namespace RPGMDataKit.Data.Enums;

/// <summary>What a state does to the actions of a battler (<c>Restriction</c> of a state). The same numbers in VX Ace and MV.</summary>
public static class StateRestriction
{
    public const int None = 0;
    public const int AttackEnemies = 1;
    public const int AttackAnyone = 2;
    public const int AttackAllies = 3;
    public const int CannotMove = 4;
}
