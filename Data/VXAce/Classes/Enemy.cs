using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;
using RubyMarshalCS.SpecialTypes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Enemy")]
public class Enemy : BaseItem, IEnemy
{
    [RubyObject("RPG::Enemy::DropItem")]
    public class DropItem : RubySerializationEntity
    {
        [RubyProperty("@kind")]
        public int Kind;

        [RubyProperty("@data_id")]
        public int DataId;

        [RubyProperty("@denominator")]
        public int Denominator;
    }

    [RubyObject("RPG::Enemy::Action")]
    public class Action : RubySerializationEntity
    {
        [RubyProperty("@skill_id")]
        public int SkillId;

        [RubyProperty("@condition_type")]
        public int ConditionType;

        [RubyProperty("@condition_param1")]
        public DynamicProperty<int, double> ConditionParam1;

        [RubyProperty("@condition_param2")]
        public DynamicProperty<int, double> ConditionParam2;

        [RubyProperty("@rating")]
        public int Rating;
    }

    [RubyProperty("@battler_name")]
    public string BattlerName;

    [RubyProperty("@battler_hue")]
    public int BattlerHue;

    [RubyProperty("@params")]
    public List<int> Params;

    [RubyProperty("@exp")]
    public int Exp;

    [RubyProperty("@gold")]
    public int Gold;

    [RubyProperty("@drop_items")]
    public List<DropItem> DropItems;

    [RubyProperty("@actions")]
    public List<Action> Actions;

    [RubyProperty("@hit", CandidateFlags.In)]
    public int Hit;
    
    [RubyProperty("@eva", CandidateFlags.In)]
    public int Eva;
}
