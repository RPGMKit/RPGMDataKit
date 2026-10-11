using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Enemy")]
public class Enemy : BaseItem, IEnemy
{
    [RubyObject("RPG::Enemy::DropItem")]
    public class DropItem : RubySerializationEntity, IDropItem
    {
        [RubyProperty("@kind")]
        public int Kind { get; set; }

        [RubyProperty("@data_id")]
        public int DataId { get; set; }

        [RubyProperty("@denominator")]
        public int Denominator { get; set; }
    }

    [RubyObject("RPG::Enemy::Action")]
    public class Action : RubySerializationEntity, IAction
    {
        [RubyProperty("@skill_id")]
        public int SkillId { get; set; }

        [RubyProperty("@condition_type")]
        public int ConditionType { get; set; }

        [RubyProperty("@condition_param1")]
        public object ConditionParam1 { get; set; }

        [RubyProperty("@condition_param2")]
        public object ConditionParam2 { get; set; }

        [RubyProperty("@rating")]
        public int Rating { get; set; }
    }

    [RubyProperty("@battler_name")]
    public string BattlerName { get; set; }

    [RubyProperty("@battler_hue")]
    public int BattlerHue { get; set; }

    [RubyProperty("@params")]
    public List<int> Params { get; set; }

    [RubyProperty("@exp")]
    public int Exp { get; set; }

    [RubyProperty("@gold")]
    public int Gold { get; set; }

    [RubyProperty("@drop_items")]
    public List<DropItem> DropItems { get; set; }

    [RubyProperty("@actions")]
    public List<Action> Actions { get; set; }
}
