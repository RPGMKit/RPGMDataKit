using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::EquipItem")]
public class EquipItem : BaseItem
{
    [RubyProperty("@price")]
    public int Price;

    [RubyProperty("@etype_id")]
    public int ETypeId;

    [RubyProperty("@params")]
    public List<int> Params;

    /// <summary>The values of <c>RPG::EquipItem.new</c> of VX Ace: no price, the parameters are 0.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        Price = 0;
        ETypeId = 0;
        Params = [0, 0, 0, 0, 0, 0, 0, 0];
    }
}
