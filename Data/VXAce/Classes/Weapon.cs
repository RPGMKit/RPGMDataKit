using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Weapon")]
public class Weapon : EquipItem, IWeapon
{
    [RubyProperty("@wtype_id")]
    public int WTypeId;

    [RubyProperty("@animation_id")]
    public int AnimationId;

    /// <summary>The values of <c>RPG::Weapon.new</c> of VX Ace: the features of the first type of the weapon and of the hit rate.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        WTypeId = 0;
        AnimationId = 0;
        Features =
        [
            new Feature { Code = FeatureCodes.AttackElement, DataId = 1, Value = 0 },
            new Feature { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.HitRate, Value = 0 },
        ];
    }
}
