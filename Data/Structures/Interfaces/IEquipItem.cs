namespace RPGMDataKit.Data.Structures.Interfaces;

/// <summary>What a weapon and an armor have in common (<c>RPG::EquipItem</c> of VX Ace; in MV the same fields, written in both).</summary>
public interface IEquipItem : IBaseItem, IHasIcon, IHasDescription, IHasFeatures
{
    public int Price { get; set; }

    /// <summary>The type of the equipment. VX Ace counts them from 0 (the weapon), MV from 1.</summary>
    public int ETypeId { get; set; }

    /// <summary>The 8 parameters that the equipment adds (the maximum HP, the maximum MP, ATK, DEF, MAT, MDF, AGI, LUK).</summary>
    public List<int> Params { get; set; }
}
