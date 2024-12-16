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

    [RubyProperty("@list")]
    public List<MoveCommand> List { get; set; } = new();
    
    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1;

    public List<NamedCommand> NamedCommands { get; set; }
    public IEnumerable<IMoveCommand> Commands => List;
    
    protected override void OnDeserialize() => NamedCommands = CommandContainerHelper.Unpack(Commands, StorageVersion.VXAce);

    protected override void OnPreSerialize() => List = CommandContainerHelper.Pack<MoveCommand>(NamedCommands, StorageVersion.VXAce);
    
    public void Dump(TextWriter writer, EntityContext context, DumpOptions options)
    {
        throw new NotImplementedException();
    }
}