namespace RPGMDataKit.Data.Structures.Interfaces;

/// <summary>What a skill and an item have in common (<c>RPG::UsableItem</c> of VX Ace; in MV the same fields, written in both).</summary>
public interface IUsableItem : IBaseItem, IHasIcon, IHasDescription
{
    /// <summary>Whom it is used on (<see cref="Enums.ItemScope"/>).</summary>
    public int Scope { get; set; }

    /// <summary>When it can be used (<see cref="Enums.ItemOccasion"/>).</summary>
    public int Occasion { get; set; }

    public int Speed { get; set; }

    public int AnimationId { get; set; }

    public int SuccessRate { get; set; }

    public int Repeats { get; set; }

    public int TpGain { get; set; }

    /// <summary>How the hit is calculated (<see cref="Enums.HitType"/>).</summary>
    public int HitType { get; set; }

    public IItemDamage Damage { get; }

    public IReadOnlyList<IEffect> Effects { get; }
}
