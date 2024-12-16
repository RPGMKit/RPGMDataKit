using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;
using RubyMarshalCS.SpecialTypes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::BaseItem::Feature")]
public class Feature : RubySerializationEntity
{
    [RubyProperty("@code")]
    public int Code;

    [RubyProperty("@data_id")]
    public int DataId;

    [RubyProperty("@value", CandidateFlags.Dynamic)]
    public DynamicProperty<int, double> Value;
}
