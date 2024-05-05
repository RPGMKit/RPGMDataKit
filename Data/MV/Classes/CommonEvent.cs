using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class CommonEvent : JsonEntity, ICommonEvent<EventCommand>
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    
        [JsonProperty(PropertyName = "list")]
        public CommandContainer<EventCommand> List { get; set; }

        public override string Dump(Storage storage, DumpOptions options)
        {
            if (options.Format == DumpFormat.Text)
                return string.Join("\r\n", List.NamedCommands.Select(_ => _.AsString(storage, options)));

            return base.Dump(storage, options);
        }
    }
}
