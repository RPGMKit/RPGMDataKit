using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Tileset")]
public class Tileset : RubySerializationEntity, ITileSet
{
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
