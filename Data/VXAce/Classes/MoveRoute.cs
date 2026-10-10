using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::MoveRoute")]
public class MoveRoute : RubySerializationEntity, IMoveRoute
{
    [RubyProperty("@repeat")]
    public bool Repeat { get; set; }

    [RubyProperty("@skippable")]
    public bool Skippable { get; set; }

    [RubyProperty("@wait")]
    public bool Wait { get; set; }

    private readonly CommandList<MoveCommand> _commands = new(StorageVersion.VXAce) { Raw = new() };

    [RubyProperty("@list")]
    public List<MoveCommand>? List
    {
        get => _commands.Raw;
        set => _commands.Raw = value;
    }

    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1;

    public List<NamedCommand> NamedCommands
    {
        get => _commands.Named;
        set => _commands.Named = value;
    }

    public IEnumerable<IMoveCommand> Commands => List;

    public async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
    {
        throw new NotImplementedException();
    }
}