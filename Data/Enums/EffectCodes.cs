namespace RPGMDataKit.Data.Enums;

/// <summary>The codes of the effects of an item or a skill (<c>IEffect.Code</c>). The same numbers in VX Ace and MV.</summary>
public static class EffectCodes
{
    public const int RecoverHp = 11;
    public const int RecoverMp = 12;
    public const int GainTp = 13;
    public const int AddState = 21;
    public const int RemoveState = 22;
    public const int AddBuff = 31;
    public const int AddDebuff = 32;
    public const int RemoveBuff = 33;
    public const int RemoveDebuff = 34;
    public const int Special = 41;
    public const int Grow = 42;
    public const int LearnSkill = 43;
    public const int CommonEvent = 44;
}
