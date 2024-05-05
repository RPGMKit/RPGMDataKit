using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::MoveRoute")]
public class MoveRoute : RubySerializationEntity
{
    [RubyProperty("@repeat")]
    public bool Repeat;

    [RubyProperty("@skippable")]
    public bool Skippable;

    [RubyProperty("@wait")]
    public bool Wait;

    [RubyProperty("@list")]
    public List<MoveCommand> List;
    
    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1;
}