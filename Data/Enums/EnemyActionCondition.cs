namespace RPGMDataKit.Data.Enums;

/// <summary>When an enemy uses an action (<c>ConditionType</c> of an action; the parameters depend on it). The same numbers in VX Ace and MV.</summary>
public static class EnemyActionCondition
{
    /// <summary>No parameters.</summary>
    public const int Always = 0;

    /// <summary>Param1 + Param2 * X turns.</summary>
    public const int Turn = 1;

    /// <summary>HP from Param1 to Param2 (fractions of 1).</summary>
    public const int Hp = 2;

    /// <summary>MP from Param1 to Param2 (fractions of 1).</summary>
    public const int Mp = 3;

    /// <summary>Param1 is the state.</summary>
    public const int State = 4;

    /// <summary>The level of the party is Param1 or more.</summary>
    public const int PartyLevel = 5;

    /// <summary>Param1 is the switch, it is ON.</summary>
    public const int Switch = 6;
}
