using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::MoveCommand")]
public class MoveCommand : RubySerializationEntity, IMoveCommand
{
    [RubyProperty("@code")]
    public int Code { get; set; }

    [RubyProperty("@parameters")]
    public LookupList<object?>? Parameters { get; set; }
    
    public int? Indent { get; set; }
    
    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1;
    
    public IParameterProvider ParameterProvider => new SimpleParameterProvider(this);
}
