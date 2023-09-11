using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures
{
    public class CommonEvent : JsonEntity
    {
        [JsonProperty(PropertyName = "id")]
        public int Id;
        [JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonProperty(PropertyName = "list")]
        public CommandContainer List;

        public override string Dump(Storage storage, DumpOptions options)
        {
            if (options.Format == DumpFormat.Text)
                return string.Join("\r\n", List.Commands.Select(_ => _.AsString(storage, options)));

            return base.Dump(storage, options);
        }
    }
}
