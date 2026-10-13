using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class MoveRoute : MvMoveCommandContainer, IMoveRoute, INewEntity
{
    public MoveRoute()
    {
        NamedCommands = new List<NamedCommand>();
    }

    /// <summary>The values that the editor of MV gives to a new route: it repeats, does not wait, and has only the command that ends it.</summary>
    public void InitializeNew()
    {
        Repeat = true;
        Skippable = false;
        Wait = false;
        NamedCommands = [CommandContainerHelper.CreateNamed(new MoveCommand { Code = 0, Parameters = [] }, StorageVersion.MV)];
    }

    [JsonProperty(PropertyName = "repeat")]
    public bool Repeat { get; set; }

    [JsonProperty(PropertyName = "skippable")]
    public bool Skippable { get; set; }

    [JsonProperty(PropertyName = "wait")]
    public bool Wait { get; set; }


    public async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
    {
        throw new NotImplementedException();
    }
}