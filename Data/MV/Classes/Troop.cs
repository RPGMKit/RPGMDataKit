using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Troop : JsonEntity, ITroop
{
    public class Page : JsonEntity, IPage
    {
        [JsonProperty(PropertyName = "list")]
        public LookupList<EventCommand> List { get; set; }

        public IEnumerable<NamedCommand> NamedCommands => List.Select(CommandFactory.Create);
        public IEnumerable<IEventCommand> Commands => List;
    }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    public LookupList<Page> _Pages { get; set; }

    public IByIdGetter<IPage> Pages => _Pages;
}