using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Armor")]
public class Armor : EquipItem
{
    [RubyProperty("@atype_id")]
    public int ATypeId;
}
