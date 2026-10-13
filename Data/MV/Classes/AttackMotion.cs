using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class AttackMotion : JsonEntity
{
    [JsonProperty("type")]
    public int Type { get; set; }
    
    [JsonProperty("weaponImageId")]
    public int WeaponImageId { get; set; }
}