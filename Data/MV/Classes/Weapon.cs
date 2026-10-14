using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.MV.Classes;

public class Weapon : EquipItem, IWeapon
{
    [JsonProperty("wtypeId")]
    public int WTypeId { get; set; }

    [JsonProperty("animationId")]
    public int AnimationId { get; set; }

    /// <summary>The values that the editor of MV gives to a new weapon: the equipment type 1, the traits of the attack element and of the hit rate.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        WTypeId = 0;
        AnimationId = 0;
        ETypeId = 1;
        Traits =
        [
            new Trait { Code = FeatureCodes.AttackElement, DataId = 1, Value = 0 },
            new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.HitRate, Value = 0 },
        ];
    }
}
