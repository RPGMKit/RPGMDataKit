using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class CommonEvent : JsonEntity, ICommonEvent
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        public IEnumerable<NamedCommand> NamedCommands => List.Select(CommandFactory.Create);
        public IEnumerable<IEventCommand> Commands => List;

        [JsonProperty(PropertyName = "list")]
        public LookupList<EventCommand> List { get; set; }

        public override string Dump(Storage storage, DumpOptions options)
        {
            if (options.Format == DumpFormat.Text)
                return string.Join("\r\n", NamedCommands.Select(_ => _.AsString(storage, options)));

            return base.Dump(storage, options);
        }
    }
}
