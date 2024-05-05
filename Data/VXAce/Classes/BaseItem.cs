using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::BaseItem")]
public class BaseItem : RubySerializationEntity
{
    [RubyProperty("@id")]
    public int Id;

    [RubyProperty("@name")]
    public string Name;

    [RubyProperty("@icon_index")]
    public int IconIndex;

    [RubyProperty("@description")]
    public string Description;

    [RubyProperty("@features")]
    public List<Feature> Features;

    [RubyProperty("@note")]
    public string Note;
}
