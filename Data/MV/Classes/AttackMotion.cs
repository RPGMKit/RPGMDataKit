using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class AttackMotion : JsonEntity
{
    [JsonProperty("type")]
    public int Type { get; set; }
    
    [JsonProperty("weaponImageId")]
    public int WeaponImageId { get; set; }
}