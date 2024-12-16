using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::System::TestBattler")]
public class TestBattler : RubySerializationEntity
{
    [RubyProperty("@actor_id")]
    public int ActorId;

    [RubyProperty("@level")]
    public int Level;

    [RubyProperty("@equips")]
    public List<int> Equips;
}
