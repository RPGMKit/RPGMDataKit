using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

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
