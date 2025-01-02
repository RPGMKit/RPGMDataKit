using System.Diagnostics;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RPGMakerLibCoreV2.Data.Structures.Internal
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class JsonEntity : IDumpable
    {
        [JsonExtensionData]
        private IDictionary<string, JToken> _unknownFields;

        public static Dictionary<Type, Dictionary<string, string>> _globalUnknownFields = new();
        
        // public object Storage { get; private set; }
        //
        // public virtual void Initialize(object storage)
        // {
        //     Storage = storage;
        // }

        public virtual void Dump(TextWriter writer, EntityContext context, DumpOptions options)
        {
            switch (options.Format)
            {
                case DumpFormat.Text:
                    throw new Exception($"'Text' serialization not implemented for '{GetType().Name}'");
                case DumpFormat.PrettyJson:
                {
                    writer.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
                    return;
                }
            }
        }

        [OnDeserialized]
        void OnDeserialization(StreamingContext context)
        {
            if (_unknownFields != null)
            {
                foreach (var (key, _) in _unknownFields)
                {
                    if (!_globalUnknownFields.ContainsKey(GetType()))
                        _globalUnknownFields[GetType()] = new();

                    _globalUnknownFields[GetType()][key] = key;
                }
            }
            
            OnDeserialize();
        }

        [OnSerializing]
        void OnPreSerialization(StreamingContext context)
        {
            OnPreSerialize();
        }

        public virtual void OnDeserialize()
        {
        }

        public virtual void OnPreSerialize()
        {
        }
    }
}
