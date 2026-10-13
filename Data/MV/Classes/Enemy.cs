using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Enemy : JsonEntity, IEnemy, INewEntity
{
    public class DropItem : JsonEntity, IDropItem, INewEntity
    {
        /// <summary>The values that the editor of MV gives to a new drop: nothing falls out.</summary>
        public void InitializeNew()
        {
            Kind = 0;
            DataId = 1;
            Denominator = 1;
        }

        [JsonProperty("kind")]
        public int Kind { get; set; }

        [JsonProperty("dataId")]
        public int DataId { get; set; }

        [JsonProperty("denominator")]
        public int Denominator { get; set; }
    }
    
    public class Action : JsonEntity, IAction, INewEntity
    {
        /// <summary>The values that the editor of MV gives to a new action: the first skill (attack), always, rating 5.</summary>
        public void InitializeNew()
        {
            SkillId = 1;
            ConditionType = 0;
            ConditionParam1 = 0;
            ConditionParam2 = 0;
            Rating = 5;
        }

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

    /// <summary>The values that the editor of MV gives to a new enemy: three empty drops, one action (attack), the hit rate and the evasion.</summary>
    public void InitializeNew()
    {
        Name = "";
        Note = "";
        BattlerName = "";
        BattlerHue = 0;
        Params = [100, 0, 10, 10, 10, 10, 10, 10];
        Exp = 0;
        Gold = 0;
        DropItems = [NewDrop(), NewDrop(), NewDrop()];
        Actions = [NewAction()];
        Traits =
        [
            new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.HitRate, Value = 0.95 },
            new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.Evasion, Value = 0.05 },
            new Trait { Code = FeatureCodes.AttackElement, DataId = 1, Value = 0 },
        ];

        static DropItem NewDrop()
        {
            var drop = new DropItem();
            drop.InitializeNew();

            return drop;
        }

        static Action NewAction()
        {
            var action = new Action();
            action.InitializeNew();

            return action;
        }
    }
}
