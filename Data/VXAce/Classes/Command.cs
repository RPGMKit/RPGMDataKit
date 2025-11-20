using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::EventCommand")]
public class Command : RubySerializationEntity, ICommand
{
    [RubyProperty("@code")]
    public int Code { get; set; }

    [RubyProperty("@indent")]
    public int? Indent { get; set; }

    public IParameterProvider ParameterProvider => new SimpleParameterProvider(this);

    [RubyProperty("@parameters")]
    public LookupList<object?>? Parameters { get; set; }
    
    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1;
}