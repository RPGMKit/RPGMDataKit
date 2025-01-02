using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::BaseItem")]
public class BaseItem : RubySerializationEntity
{
    [RubyProperty("@id")]
    public int Id { get; set; }

    [RubyProperty("@name")]
    public string Name { get; set; }

    [RubyProperty("@icon_index")]
    public int IconIndex { get; set; }

    [RubyProperty("@description")]
    public string Description { get; set; }

    [RubyProperty("@features")]
    public List<Feature> Features { get; set; }

    [RubyProperty("@note")]
    public string Note { get; set; }
}
