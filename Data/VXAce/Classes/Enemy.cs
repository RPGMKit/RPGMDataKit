using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Enemy")]
public class Enemy : BaseItem, IEnemy
{
    [RubyObject("RPG::Enemy::DropItem")]
    public class DropItem : RubySerializationEntity, IDropItem, INewEntity
    {
        /// <summary>The values of <c>RPG::Enemy::DropItem.new</c> of VX Ace: nothing falls out.</summary>
        public void InitializeNew()
        {
            Kind = DropItemKind.None;
            DataId = 1;
            Denominator = 1;
        }

        [RubyProperty("@kind")]
        public int Kind { get; set; }

        [RubyProperty("@data_id")]
        public int DataId { get; set; }

        [RubyProperty("@denominator")]
        public int Denominator { get; set; }
    }

    [RubyObject("RPG::Enemy::Action")]
    public class Action : RubySerializationEntity, IAction, INewEntity
    {
        /// <summary>The values of <c>RPG::Enemy::Action.new</c> of VX Ace: the first skill (attack), always, rating 5.</summary>
        public void InitializeNew()
        {
            SkillId = 1;
            ConditionType = EnemyActionCondition.Always;
            ConditionParam1 = 0;
            ConditionParam2 = 0;
            Rating = 5;
        }

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

    /// <summary>The values of <c>RPG::Enemy.new</c> of VX Ace: three empty drops, one action (attack), the hit rate and the evasion.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        BattlerName = "";
        BattlerHue = 0;
        Params = [100, 0, 10, 10, 10, 10, 10, 10];
        Exp = 0;
        Gold = 0;
        DropItems = [NewDrop(), NewDrop(), NewDrop()];
        Actions = [NewAction()];
        Features =
        [
            new Feature { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.HitRate, Value = 0.95 },
            new Feature { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.Evasion, Value = 0.05 },
            new Feature { Code = FeatureCodes.AttackElement, DataId = 1, Value = 0 },
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
