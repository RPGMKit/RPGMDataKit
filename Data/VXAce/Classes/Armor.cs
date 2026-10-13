using RPGMDataKit.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Armor")]
public class Armor : EquipItem, IArmor
{
    [RubyProperty("@atype_id")]
    public int ATypeId;
}
