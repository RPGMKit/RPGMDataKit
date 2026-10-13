namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IArmor : IHasId
{
    public string Name { get; set; }
    public int IconIndex { get; set; }
}
