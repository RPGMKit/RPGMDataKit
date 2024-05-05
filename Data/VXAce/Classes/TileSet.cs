using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization;
using RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Tileset")]
public class TileSet : RubySerializationEntity, ITileSet
{
    [RubyProperty("@id")]
    public int Id { get; set; }

    [RubyProperty("@mode")]
    public int Mode { get; set; }

    [RubyProperty("@name")]
    public string Name { get; set; }

    [RubyProperty("@tileset_names")]
    public LookupList<string> TilesetNames { get; set; }

    [RubyProperty("@flags")]
    public Table _Flags { get; set; }
    
    public LookupList<int> Flags => _Flags.Elements.Cast<int>().ToLookupList();

    [RubyProperty("@note")]
    public string Note { get; set; }
}
