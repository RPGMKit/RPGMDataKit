using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Weapon")]
public class Weapon : EquipItem
{
    [RubyProperty("@wtype_id")]
    public int WTypeId;

    [RubyProperty("@animation_id")]
    public int AnimationId;
}
