using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Item")]
public class Item : UsableItem, IItem
{
    [RubyProperty("@itype_id")]
    public int ITypeId { get; set; }

    [RubyProperty("@price")]
    public int Price { get; set; }

    [RubyProperty("@consumable")]
    public bool Consumable { get; set; }

    /// <summary>The values of <c>RPG::Item.new</c> of VX Ace: usable anywhere on one friend, a normal item that is used up.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        Scope = ItemScope.OneAlly;
        ITypeId = 1;
        Price = 0;
        Consumable = true;
    }
}
