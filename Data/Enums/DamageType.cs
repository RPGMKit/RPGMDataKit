namespace RPGMDataKit.Data.Enums;

/// <summary>What the damage formula of an item or a skill does (<c>Type</c> of the damage). The same numbers in VX Ace and MV.</summary>
public static class DamageType
{
    public const int None = 0;
    public const int HpDamage = 1;
    public const int MpDamage = 2;
    public const int HpRecover = 3;
    public const int MpRecover = 4;
    public const int HpDrain = 5;
    public const int MpDrain = 6;
}
