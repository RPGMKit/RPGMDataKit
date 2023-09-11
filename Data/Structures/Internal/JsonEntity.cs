using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RPGMakerLibCoreV2.Data.Structures.Internal
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class JsonEntity : IDumpable
    {
        [JsonExtensionData]
        private IDictionary<string, JToken> _unknownFields;

        public object Storage { get; private set; }

        public virtual void Initialize(object storage)
        {
            Storage = storage;
        }

        public virtual string Dump(Storage storage, DumpOptions options)
        {
            switch (options.Format)
            {
                case DumpFormat.Text:
                    throw new Exception($"'Text' serialization not implemented for '{GetType().Name}'");
                case DumpFormat.PrettyJson:
                    return JsonConvert.SerializeObject(this, Formatting.Indented);
            }

            return "";
        }
    }
}
