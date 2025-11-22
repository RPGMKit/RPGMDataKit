using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;
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
    public LookupList<string> _TilesetNames { get; set; }

    public IDataLookup<string> TilesetNames => _TilesetNames;

    [RubyProperty("@flags")]
    public Table _Flags { get; set; }
    
    public IDataLookup<int> Flags => new ListWrapper<int, short>(_Flags.Elements, i => (short)i, s => (int)s);

    [RubyProperty("@note")]
    public string Note { get; set; }
}
