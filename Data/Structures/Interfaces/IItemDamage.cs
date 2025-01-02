namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IItemDamage
{
    public int Type { get; set; }

    public int ElementId { get; set; }

    public string Formula { get; set; }

    public int Variance { get; set; }

    public bool Critical { get; set; }
}
