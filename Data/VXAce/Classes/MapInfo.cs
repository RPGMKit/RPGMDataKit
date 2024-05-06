using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::MapInfo")]
class MapInfo : RubySerializationEntity, IMapInfo
{
    [RubyProperty("@expanded")]
    public bool Expanded { get; set; }

    [RubyProperty("@name")]
    public string Name { get; set; }

    [RubyProperty("@order")]
    public int Order { get; set; }

    [RubyProperty("@parent_id")]
    public int Parent { get; set; }

    [RubyProperty("@scroll_x")]
    public int ScrollX { get; set; }

    [RubyProperty("@scroll_y")]
    public int ScrollY { get; set; }
    
    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1 { get; set; }
}