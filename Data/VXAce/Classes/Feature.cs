using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::BaseItem::Feature")]
public class Feature : RubySerializationEntity, IFeature, INewEntity
{
    /// <summary>What the editor of VX Ace puts into a feature that is added and not changed: the attack element 1 with the value 1 (a float).</summary>
    public void InitializeNew()
    {
        Code = FeatureCodes.AttackElement;
        DataId = 1;
        Value = 1.0;
    }

    [RubyProperty("@code")]
    public int Code { get; set; }

    [RubyProperty("@data_id")]
    public int DataId { get; set; }

    [RubyProperty("@value")]
    public object Value { get; set; }
}
