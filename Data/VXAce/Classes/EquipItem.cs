using RPGMDataKit.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::EquipItem")]
public class EquipItem : BaseItem, IEquipItem
{
    [RubyProperty("@price")]
    public int Price { get; set; }

    [RubyProperty("@etype_id")]
    public int ETypeId { get; set; }

    [RubyProperty("@params")]
    public List<int> Params { get; set; }

    /// <summary>The values of <c>RPG::EquipItem.new</c> of VX Ace: no price, the parameters are 0.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        Price = 0;
        ETypeId = 0;
        Params = [0, 0, 0, 0, 0, 0, 0, 0];
    }
}
