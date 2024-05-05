using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures
{
    public class Event : JsonEntity
    {
        public class Page : JsonEntity
        {
            [JsonProperty(PropertyName = "list")]
            public CommandContainer List;

            public override string Dump(Storage storage, DumpOptions options)
            {
                if (options.Format == DumpFormat.Text)
                    return string.Join("\r\n", List.Commands.Select(_ => _.AsString(storage, options)));

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