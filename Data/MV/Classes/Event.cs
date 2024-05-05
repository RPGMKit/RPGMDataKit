using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class Event : JsonEntity, IEvent
    {
        public class Page : JsonEntity, IPage
        {
            [JsonProperty(PropertyName = "list")]
            public LookupList<EventCommand> List { get; set; }

            public override string Dump(Storage storage, DumpOptions options)
            {
                if (options.Format == DumpFormat.Text)
                    return string.Join("\r\n", NamedCommands.Select(_ => _.AsString(storage, options)));

                return base.Dump(storage, options);
            }

            public IEnumerable<NamedCommand> NamedCommands => List.Select(MVCommandFactory.Create);
            public IEnumerable<IEventCommand> Commands => List;
        }
        
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }
        [JsonProperty(PropertyName = "pages")]
        public LookupList<Page> _Pages { get; set; }

        public IEnumerable<IPage> Pages => _Pages;

        [JsonProperty(PropertyName = "x")]
        public int X { get; set; }
        [JsonProperty(PropertyName = "y")]
        public int Y { get; set; }
    }
}