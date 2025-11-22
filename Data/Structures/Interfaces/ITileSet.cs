using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ITileSet
{
    public int Id { get; set; }
    public IDataLookup<int> Flags { get; }
    public int Mode { get; set; } // enum TilesetMode
    public string Name { get; set; }
    public string Note { get; set; }
    public IDataLookup<string> TilesetNames { get; }
}