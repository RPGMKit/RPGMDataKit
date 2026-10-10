using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::MoveRoute")]
public class MoveRoute : VxMoveCommandContainer, IMoveRoute
{
    public MoveRoute()
    {
        List = new();
    }

    [RubyProperty("@repeat")]
    public bool Repeat { get; set; }

    [RubyProperty("@skippable")]
    public bool Skippable { get; set; }

    [RubyProperty("@wait")]
    public bool Wait { get; set; }

    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1;

    public async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
    {
        throw new NotImplementedException();
    }
}