using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Weapon : JsonEntity, IWeapon, INewEntity
{
    /// <summary>The values that the editor of MV gives to a new weapon: the equipment type 1, the traits of the attack element and of the hit rate.</summary>
    public void InitializeNew()
    {
        Name = "";
        IconIndex = 0;
        WTypeId = 0;
        AnimationId = 0;
        Price = 0;
        ETypeId = 1;
        Params = [0, 0, 0, 0, 0, 0, 0, 0];
        Description = "";
        Note = "";
        Traits =
        [
            new Trait { Code = FeatureCodes.AttackElement, DataId = 1, Value = 0 },
            new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.HitRate, Value = 0 },
        ];
    }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("iconIndex")]
    public int IconIndex { get; set; }
    
    [JsonProperty("wtypeId")]
    public int WTypeId;

    [JsonProperty("animationId")]
    public int AnimationId;
    
    [JsonProperty("price")]
    public int Price;

    [JsonProperty("etypeId")]
    public int ETypeId;

    [JsonProperty("params")]
    public List<int> Params;
    
    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("note")]
    public string Note { get; set; }
    
    [JsonProperty("traits")]
    public List<Trait> Traits { get; set; }
}
