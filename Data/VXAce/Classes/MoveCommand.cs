using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::MoveCommand")]
public class MoveCommand : RubySerializationEntity
{
    [RubyProperty("@code")]
    public int Code;

    [RubyProperty("@parameters")]
    public List<object> Parameters;
    
    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1;
}
