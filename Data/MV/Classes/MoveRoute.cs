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

    [JsonProperty(PropertyName = "list")]
    public List<MoveCommand>? List { get; set; }

    public List<NamedCommand> NamedCommands { get; set; }
        
    public IEnumerable<IMoveCommand>? Commands => List;

    public override void OnDeserialized()
    {
        base.OnDeserialized();
        NamedCommands = CommandContainerHelper.Unpack(List, StorageVersion.MV);
    }

    public override void OnSerializing()
    {
        base.OnSerializing();
        List = CommandContainerHelper.Pack<MoveCommand>(NamedCommands, StorageVersion.MV);
    }
        
    public async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
    {
        throw new NotImplementedException();
    }
}