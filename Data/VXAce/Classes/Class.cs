using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Class")]
public class Class : BaseItem
{
    [RubyObject("RPG::Class::Learning")]
    public class Learning : RubySerializationEntity
    {
        [RubyProperty("@level")]
        public int Level;

        [RubyProperty("@skill_id")]
        public int SkillId;

        [RubyProperty("@note")]
        public string Note;
    }

    [RubyProperty("@exp_params")]
    public List<int> ExpParams;

    [RubyProperty("@params")]
    public object Params;

    [RubyProperty("@learnings")]
    public List<Learning> Learnings;
}