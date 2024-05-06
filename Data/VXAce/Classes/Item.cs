using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Item")]
public class Item : UsableItem, IItem
{
    [RubyProperty("@itype_id")]
    public int ITypeId;
    [RubyProperty("@price")]
    public int Price;
    [RubyProperty("@consumable")]
    public bool Consumable;
}
