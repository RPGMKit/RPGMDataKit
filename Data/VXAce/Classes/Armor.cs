using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Armor")]
public class Armor : EquipItem, IArmor
{
    [RubyProperty("@atype_id")]
    public int ATypeId;
}
