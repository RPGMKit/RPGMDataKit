using RPGMDataKit.Data.Structures.Commands.ParameterProviders;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::EventCommand")]
public class Command : RubySerializationEntity, ICommand
{
    [RubyProperty("@code")]
    public int Code { get; set; }

    [RubyProperty("@indent")]
    public int? Indent { get; set; }

    public IParameterProvider ParameterProvider => new SimpleParameterProvider(this);

    [RubyProperty("@parameters")]
    public List<object?>? Parameters { get; set; }
    
    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1;
}