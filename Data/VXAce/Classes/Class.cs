using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Class")]
public class Class : BaseItem, IClass
{
    [RubyObject("RPG::Class::Learning")]
    public class Learning : RubySerializationEntity, ILearning, INewEntity
    {
        [RubyProperty("@level")]
        public int Level { get; set; }

        [RubyProperty("@skill_id")]
        public int SkillId { get; set; }

        [RubyProperty("@note")]
        public string Note { get; set; }

        /// <summary>The values of <c>RPG::Class::Learning.new</c> of VX Ace.</summary>
        public void InitializeNew()
        {
            Level = 1;
            SkillId = 1;
            Note = "";
        }
    }

    [RubyProperty("@exp_params")]
    public List<int> ExpParams { get; set; }

    [RubyProperty("@params")]
    public Table Params { get; set; }

    [RubyProperty("@learnings")]
    public List<Learning> Learnings { get; set; }

    /// <summary>The values of <c>RPG::Class.new</c> of VX Ace: the curves of the parameters of the editor and its basic features.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        ExpParams = [30, 20, 30, 30];
        Params = new Table(8, 100);
        for (var level = 1; level < 100; level++)
        {
            Params[0, level] = 400 + level * 50;
            Params[1, level] = 80 + level * 10;
            for (var parameter = 2; parameter <= 5; parameter++)
                Params[parameter, level] = 15 + level * 5 / 4;

            for (var parameter = 6; parameter <= 7; parameter++)
                Params[parameter, level] = 30 + level * 5 / 2;
        }

        Learnings = new();
        Features =
        [
            new Feature { Code = FeatureCodes.SpecialParameter, DataId = 0, Value = 1 },
            new Feature { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.HitRate, Value = 0.95 },
            new Feature { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.Evasion, Value = 0.05 },
            new Feature { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.Critical, Value = 0.04 },
            new Feature { Code = FeatureCodes.SkillTypeAdd, DataId = 1, Value = 0 },
            new Feature { Code = FeatureCodes.EquipWeaponType, DataId = 1, Value = 0 },
            new Feature { Code = FeatureCodes.EquipArmorType, DataId = 1, Value = 0 },
        ];
    }
}