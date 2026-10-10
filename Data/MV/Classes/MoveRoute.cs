using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class MoveRoute : JsonEntity, IMoveRoute
{
    [JsonProperty(PropertyName = "repeat")]
    public bool Repeat { get; set; }

    [JsonProperty(PropertyName = "skippable")]
    public bool Skippable { get; set; }

    [JsonProperty(PropertyName = "wait")]
    public bool Wait { get; set; }

    private readonly CommandList<MoveCommand> _commands = new(StorageVersion.MV);

    [JsonProperty(PropertyName = "list")]
    public List<MoveCommand>? List
    {
        get => _commands.Raw;
        set => _commands.Raw = value;
    }

    public List<NamedCommand> NamedCommands
    {
        get => _commands.Named;
        set => _commands.Named = value;
    }

    public IEnumerable<IMoveCommand>? Commands => List;

    public async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
    {
        throw new NotImplementedException();
    }
}