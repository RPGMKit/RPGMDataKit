using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::MoveRoute")]
public class MoveRoute : VxMoveCommandContainer, IMoveRoute, INewEntity
{
    public MoveRoute()
    {
        NamedCommands = new List<NamedCommand>();
    }

    /// <summary>The values of <c>RPG::MoveRoute.new</c> of VX Ace: it repeats, does not wait, and has only the command that ends it.</summary>
    public void InitializeNew()
    {
        Repeat = true;
        Skippable = false;
        Wait = false;
        NamedCommands = [CommandContainerHelper.CreateNamed(new MoveCommand { Code = 0, Parameters = [] }, StorageVersion.VXAce)];
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