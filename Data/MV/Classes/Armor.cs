using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.MV.Classes;

public class Armor : EquipItem, IArmor
{
    [JsonProperty("atypeId")]
    public int ATypeId { get; set; }

    /// <summary>The values that the editor of MV gives to a new armor: the equipment type 2 (a shield), the trait of the evasion.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        ATypeId = 0;
        ETypeId = 2;
        Traits = [new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.Evasion, Value = 0 }];
    }
}
