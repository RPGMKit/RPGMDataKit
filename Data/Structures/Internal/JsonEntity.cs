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

        public virtual async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
        {
            throw new Exception($"'{options.Format}' dump format not implemented for '{GetType().Name}'");
        }

        [YamlDotNet.Serialization.Callbacks.OnDeserialized]
        public void OnYamlDeserialized()
        {
            OnDeserialized();
        }

        [YamlDotNet.Serialization.Callbacks.OnSerializing]
        public void OnYamlSerializing()
        {
            OnSerializing();
        }
        
        [System.Runtime.Serialization.OnDeserialized]
        void OnGenericDeserialized(StreamingContext context)
        {
            OnDeserialized();
        }

        [System.Runtime.Serialization.OnSerializing]
        void OnGenericSerializing(StreamingContext context)
        {
            OnSerializing();
        }

        public virtual void OnDeserialized()
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
        }

        public virtual void OnSerializing()
        {
            return;
        }
    }
}
