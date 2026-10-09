namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ITileSet : IHasId
{
    public IList<short> Flags { get; }
    public int Mode { get; set; } // enum TilesetMode
    public string Name { get; set; }
    public string Note { get; set; }
    public IList<string> TilesetNames { get; }
}