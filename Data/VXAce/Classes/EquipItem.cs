using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::EquipItem")]
public class EquipItem : BaseItem
{
    [RubyProperty("@price")]
    public int Price;

    [RubyProperty("@etype_id")]
    public int ETypeId;

    [RubyProperty("@params")]
    public List<int> Params;
}
