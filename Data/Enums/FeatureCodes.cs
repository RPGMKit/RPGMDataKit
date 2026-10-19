namespace RPGMDataKit.Data.Enums;

/// <summary>
/// The codes of the features of VX Ace (the traits of MV): <c>IFeature.Code</c>. The same numbers in both makers;
/// what <c>DataId</c> and <c>Value</c> mean depends on the code (<see cref="ExtraParameterIds"/> for <see cref="ExtraParameter"/>).
/// </summary>
public static class FeatureCodes
{
    /// <summary>Damage from an element: DataId is the element, Value the rate (1 = 100%).</summary>
    public const int ElementRate = 11;

    /// <summary>Chance of a debuff: DataId is the parameter (0..7), Value the rate.</summary>
    public const int DebuffRate = 12;

    /// <summary>Chance of getting a state: DataId is the state, Value the rate.</summary>
    public const int StateRate = 13;

    /// <summary>Resistance to a state: DataId is the state.</summary>
    public const int StateResist = 14;

    /// <summary>A parameter (MHP, MMP, ATK, DEF, MAT, MDF, AGI, LUK): DataId is 0..7, Value the rate.</summary>
    public const int Parameter = 21;

    /// <summary>An additional parameter (hit, evasion, critical...): DataId is one of <see cref="ExtraParameterIds"/>, Value is added.</summary>
    public const int ExtraParameter = 22;

    /// <summary>A special parameter (target rate, guard effect...): DataId is 0..9, Value the rate.</summary>
    public const int SpecialParameter = 23;

    /// <summary>Element of an attack: DataId is the element.</summary>
    public const int AttackElement = 31;

    /// <summary>State of an attack: DataId is the state, Value the chance.</summary>
    public const int AttackState = 32;

    /// <summary>Speed of an attack: Value is added.</summary>
    public const int AttackSpeed = 33;

    /// <summary>Number of attacks: Value is added.</summary>
    public const int AttackTimes = 34;

    /// <summary>Adds a type of skills: DataId is the type.</summary>
    public const int SkillTypeAdd = 41;

    /// <summary>Seals a type of skills: DataId is the type.</summary>
    public const int SkillTypeSeal = 42;

    /// <summary>Adds a skill: DataId is the skill.</summary>
    public const int SkillAdd = 43;

    /// <summary>Seals a skill: DataId is the skill.</summary>
    public const int SkillSeal = 44;

    /// <summary>A type of weapons that can be equipped: DataId is the type.</summary>
    public const int EquipWeaponType = 51;

    /// <summary>A type of armors that can be equipped: DataId is the type.</summary>
    public const int EquipArmorType = 52;

    /// <summary>Fixes the equipment of a type: DataId is the type of equipment.</summary>
    public const int EquipFix = 53;

    /// <summary>Seals the equipment of a type: DataId is the type of equipment.</summary>
    public const int EquipSeal = 54;

    /// <summary>Type of the slot (dual wield): DataId is 1.</summary>
    public const int SlotType = 55;

    /// <summary>Extra action: Value is the chance.</summary>
    public const int ActionPlus = 61;

    /// <summary>Special flag: DataId is 0 auto battle, 1 guard, 2 substitute, 3 preserve TP.</summary>
    public const int SpecialFlag = 62;

    /// <summary>The way a battler collapses: DataId is 0 normal, 1 boss, 2 instant, 3 none (the largest of the features counts).</summary>
    public const int CollapseType = 63;

    /// <summary>Ability of the party: DataId is 0 encounter half, 1 no encounters, 2 cancel surprise, 3 raise preemptive, 4 double gold, 5 double drops.</summary>
    public const int PartyAbility = 64;
}

/// <summary>The ids (DataId) of the additional parameters of <see cref="FeatureCodes.ExtraParameter"/>.</summary>
public static class ExtraParameterIds
{
    public const int HitRate = 0;
    public const int Evasion = 1;
    public const int Critical = 2;
    public const int CriticalEvasion = 3;
    public const int MagicEvasion = 4;
    public const int MagicReflection = 5;
    public const int Counter = 6;
    public const int HpRegeneration = 7;
    public const int MpRegeneration = 8;
    public const int TpRegeneration = 9;
}
