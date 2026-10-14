namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IItem : IUsableItem
{
    public int ITypeId { get; set; }

    public int Price { get; set; }

    public bool Consumable { get; set; }
}
