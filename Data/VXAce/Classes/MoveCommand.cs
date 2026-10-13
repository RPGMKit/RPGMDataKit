using RPGMDataKit.Data.Structures.Commands.ParameterProviders;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::MoveCommand")]
public class MoveCommand : RubySerializationEntity, IMoveCommand
{
    [RubyProperty("@code")]
    public int Code { get; set; }

    [RubyProperty("@parameters")]
    public List<object?>? Parameters { get; set; }
    
    public int? Indent { get; set; }
    
    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1;
    
    public IParameterProvider ParameterProvider => new SimpleParameterProvider(this);
}
