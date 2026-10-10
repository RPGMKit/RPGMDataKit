using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Class")]
public class Class : BaseItem, IClass
{
    [RubyObject("RPG::Class::Learning")]
    public class Learning : RubySerializationEntity, ILearning
    {
        [RubyProperty("@level")]
        public int Level { get; set; }

        [RubyProperty("@skill_id")]
        public int SkillId { get; set; }

        [RubyProperty("@note")]
        public string Note { get; set; }
    }

    [RubyProperty("@exp_params")]
    public List<int> ExpParams { get; set; }

    [RubyProperty("@params")]
    public Table Params { get; set; }

    [RubyProperty("@learnings")]
    public List<Learning> Learnings { get; set; }
}