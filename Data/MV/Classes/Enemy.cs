using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Enemy : JsonEntity, IEnemy
{
    public class DropItem : JsonEntity, IDropItem
    {
        [JsonProperty("kind")]
        public int Kind { get; set; }

        [JsonProperty("dataId")]
        public int DataId { get; set; }

        [JsonProperty("denominator")]
        public int Denominator { get; set; }
    }
    
    public class Action : JsonEntity, IAction
    {
        [JsonProperty("skillId")]
        public int SkillId { get; set; }

        [JsonProperty("conditionType")]
        public int ConditionType { get; set; }

        [JsonProperty("conditionParam1")]
        public object ConditionParam1 { get; set; }

        [JsonProperty("conditionParam2")]
        public object ConditionParam2 { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }
    }
    
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("note")]
    public string Note { get; set; }
    
    [JsonProperty("battlerName")]
    public string BattlerName { get; set; }

    [JsonProperty("battlerHue")]
    public int BattlerHue { get; set; }

    [JsonProperty("params")]
    public List<int> Params { get; set; }

    [JsonProperty("exp")]
    public int Exp { get; set; }

    [JsonProperty("gold")]
    public int Gold { get; set; }

    [JsonProperty("dropItems")]
    public List<DropItem> DropItems { get; set; }

    [JsonProperty("actions")]
    public List<Action> Actions { get; set; }
    
    [JsonProperty("traits")]
    public List<Trait> Traits { get; set; }
}
