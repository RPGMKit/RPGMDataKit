using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Skill : UsableItem, ISkill
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("note")]
    public string Note { get; set; }
    
    [JsonProperty("description")]
    public string Description { get; set; }
    
    [JsonProperty("animationId")]
    public int AnimationId { get; set; }

    [JsonProperty("damage")]
    public ItemDamage Damage { get; set; }
    
    [JsonProperty("effects")]
    public List<Effect> Effects { get; set; }
    
    [JsonProperty("occasion")]
    public int Occasion { get; set; }

    [JsonProperty("repeats")]
    public int Repeats { get; set; }
    
    [JsonProperty("hitType")]
    public int HitType { get; set; }
    
    [JsonProperty("iconIndex")]
    public int IconIndex { get; set; }
    
    [JsonProperty("stypeId")]
    public int STypeId { get; set; }

    [JsonProperty("mpCost")]
    public int MpCost { get; set; }

    [JsonProperty("tpCost")]
    public int TpCost { get; set; }
    
    [JsonProperty("tpGain")]
    public int TpGain { get; set; }
    
    [JsonProperty("scope")]
    public int Scope { get; set; }

    [JsonProperty("speed")]
    public int Speed { get; set; }
    
    [JsonProperty("successRate")]
    public int SuccessRate { get; set; }

    [JsonProperty("message1")]
    public string Message1 { get; set; }

    [JsonProperty("message2")]
    public string Message2 { get; set; }

    [JsonProperty("requiredWtypeId1")]
    public int RequiredWTypeId1 { get; set; }

    [JsonProperty("requiredWtypeId2")]
    public int RequiredWTypeId2 { get; set; }
}
