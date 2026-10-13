using RPGMDataKit.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Item")]
public class Item : UsableItem, IItem
{
    [RubyProperty("@itype_id")]
    public int ITypeId;
    [RubyProperty("@price")]
    public int Price;
    [RubyProperty("@consumable")]
    public bool Consumable;

    /// <summary>The values of <c>RPG::Item.new</c> of VX Ace: usable anywhere on one friend, a normal item that is used up.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        Scope = 7;
        ITypeId = 1;
        Price = 0;
        Consumable = true;
    }
}
