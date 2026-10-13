using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::BaseItem::Feature")]
public class Feature : RubySerializationEntity, IFeature
{
    [RubyProperty("@code")]
    public int Code { get; set; }

    [RubyProperty("@data_id")]
    public int DataId { get; set; }

    [RubyProperty("@value")]
    public object Value { get; set; }
}
