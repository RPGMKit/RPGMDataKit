using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures
{
    public class EventPage : JsonEntity
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
}

