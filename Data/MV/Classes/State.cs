using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class State : JsonEntity, IState
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("note")]
    public string Note { get; set; }

    [JsonProperty("iconIndex")]
    public int IconIndex { get; set; }

    [JsonProperty("restriction")]
    public int Restriction { get; set; }

    [JsonProperty("priority")]
    public int Priority { get; set; }
    
    [JsonProperty("overlay")]
    public int Overlay { get; set; }

    [JsonProperty("removeAtBattleEnd")]
    public bool RemoveAtBattleEnd { get; set; }

    [JsonProperty("removeByRestriction")]
    public bool RemoveByRestriction { get; set; }

    [JsonProperty("autoRemovalTiming")]
    public int AutoRemovalTiming { get; set; }

    [JsonProperty("minTurns")]
    public int MinTurns { get; set; }

    [JsonProperty("maxTurns")]
    public int MaxTurns { get; set; }
    
    [JsonProperty("motion")]
    public int Motion { get; set; }

    [JsonProperty("removeByDamage")]
    public bool RemoveByDamage { get; set; }
    
    [JsonProperty("chanceByDamage")]
    public int ChanceByDamage { get; set; }

    [JsonProperty("removeByWalking")]
    public bool RemoveByWalking { get; set; }

    [JsonProperty("stepsToRemove")]
    public int StepsToRemove { get; set; }

    [JsonProperty("message1")]
    public string Message1 { get; set; }

    [JsonProperty("message2")]
    public string Message2 { get; set; }

    [JsonProperty("message3")]
    public string Message3 { get; set; }

    [JsonProperty("message4")]
    public string Message4 { get; set; }
    
    [JsonProperty("traits")]
    public List<Trait> Traits { get; set; }
}
