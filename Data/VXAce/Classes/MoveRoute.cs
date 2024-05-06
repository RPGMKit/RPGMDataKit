using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::MoveRoute")]
public class MoveRoute : RubySerializationEntity, IRoute
{
    [RubyProperty("@repeat")]
    public bool Repeat { get; set; }

    [RubyProperty("@skippable")]
    public bool Skippable { get; set; }

    [RubyProperty("@wait")]
    public bool Wait { get; set; }

    [RubyProperty("@list")]
    public List<MoveCommand> List;
    
    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1;
}