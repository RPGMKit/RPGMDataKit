using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Armor")]
public class Armor : EquipItem, IArmor
{
    [RubyProperty("@atype_id")]
    public int ATypeId { get; set; }

    /// <summary>The values of <c>RPG::Armor.new</c> of VX Ace: the second type of equipment (a shield), the feature of the evasion.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        ATypeId = 0;
        ETypeId = 1;
        Features = [new Feature { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.Evasion, Value = 0 }];
    }
}
