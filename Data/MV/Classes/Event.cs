using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class Event : JsonEntity
    {
        public class Page : JsonEntity, IPage<EventCommand>
        {
            [JsonProperty(PropertyName = "list")]
            public CommandContainer<EventCommand> List { get; set; }

            public override string Dump(Storage storage, DumpOptions options)
            {
                if (options.Format == DumpFormat.Text)
                    return string.Join("\r\n", List.NamedCommands.Select(_ => _.AsString(storage, options)));

                return base.Dump(storage, options);
            }
        }
        
        [JsonProperty(PropertyName = "id")]
        public int Id;
        [JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonProperty(PropertyName = "note")]
        public string Note;
        [JsonProperty(PropertyName = "pages")]
        public LookupList<Page> Pages;
        [JsonProperty(PropertyName = "x")]
        public int X;
        [JsonProperty(PropertyName = "y")]
        public int Y;
    }
}