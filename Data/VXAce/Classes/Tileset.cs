using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Tileset")]
public class Tileset : RubySerializationEntity, ITileSet, INewEntity
{
    /// <summary>The values of <c>RPG::Tileset.new</c> of VX Ace: the mode of the world map, nine empty names, the flags that the editor gives to the tiles.</summary>
    public void InitializeNew()
    {
        Mode = (int)TilesetMode.AreaType;
        Name = "";
        TilesetNames = [.. Enumerable.Repeat("", 9)];
        Flags = DefaultTilesetFlags.Create();
        Note = "";
    }

    [RubyProperty("@id")]
    public int Id { get; set; }

    [RubyProperty("@mode")]
    public int Mode { get; set; }

    [RubyProperty("@name")]
    public string Name { get; set; }

    [RubyProperty("@tileset_names")]
    public List<string> TilesetNames { get; set; } = new();

    IList<string> ITileSet.TilesetNames => TilesetNames;

    [RubyProperty("@flags")]
    public Table Flags { get; set; } = new();

    [RubyProperty("@note")]
    public string Note { get; set; }
}
