using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class MoveRoute : MvMoveCommandContainer, IMoveRoute
{
    public MoveRoute()
    {
        NamedCommands = new List<NamedCommand>();
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