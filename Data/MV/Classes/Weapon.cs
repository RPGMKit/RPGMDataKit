using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Weapon : JsonEntity, IWeapon
{
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
