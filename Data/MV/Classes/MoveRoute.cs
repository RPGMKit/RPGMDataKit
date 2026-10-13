using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

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