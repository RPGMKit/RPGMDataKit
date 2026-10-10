using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

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
    public Table _Flags { get; set; }

    public IList<short> Flags => _Flags.Elements;

    [RubyProperty("@note")]
    public string Note { get; set; }
}
