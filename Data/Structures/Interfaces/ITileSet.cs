using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ITileSet
{
    public int Id { get; set; }
    public LookupList<int> Flags { get; }
    public int Mode { get; set; } // enum TilesetMode
    public string Name { get; set; }
    public string Note { get; set; }
    public LookupList<string> TilesetNames { get; set; }
}