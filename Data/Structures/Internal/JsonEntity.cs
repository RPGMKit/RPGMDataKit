using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace RPGMakerLibCoreV2.Data.Structures.Internal
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class JsonEntity
    {
        [JsonExtensionData]
        private IDictionary<string, JToken> _unknownFields;

        // public object Storage { get; private set; }
        //
        // public virtual void Initialize(object storage)
        // {
        //     Storage = storage;
        // }

        [YamlDotNet.Serialization.Callbacks.OnDeserialized]
        public void OnYamlDeserialized()
        {
            OnDeserialized();
        }

        [YamlDotNet.Serialization.Callbacks.OnSerializing]
        public void OnYamlSerializing()
        {
            var wtf = new JsonSerializerSettings();
            wtf.Error += Error;
            OnSerializing();
            OnDump();
        }

        private void Error(object? sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
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
        }

        public virtual void OnSerializing()
        {
            return;
        }

        /// <summary>Called before the entity is written to a YAML dump (and not before it is saved to a data file).</summary>
        protected virtual void OnDump()
        {
        }
    }
}
