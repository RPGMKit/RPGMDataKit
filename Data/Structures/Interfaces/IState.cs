namespace RPGMDataKit.Data.Structures.Interfaces;

/// <remarks>No <see cref="IHasDescription"/>: states created in the editor of MV have no description (VX Ace keeps it in every <c>BaseItem</c>).</remarks>
public interface IState : IBaseItem, IHasIcon, IHasFeatures
{
    /// <summary><see cref="Enums.StateRestriction"/>.</summary>
    public int Restriction { get; set; }

    public int Priority { get; set; }

    public bool RemoveAtBattleEnd { get; set; }

    public bool RemoveByRestriction { get; set; }

    /// <summary><see cref="Enums.AutoRemovalTiming"/>.</summary>
    public int AutoRemovalTiming { get; set; }

    public int MinTurns { get; set; }

    public int MaxTurns { get; set; }

    public bool RemoveByDamage { get; set; }

    public int ChanceByDamage { get; set; }

    public bool RemoveByWalking { get; set; }

    public int StepsToRemove { get; set; }

    public string Message1 { get; set; }

    public string Message2 { get; set; }

    public string Message3 { get; set; }

    public string Message4 { get; set; }
}
