using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures
{
    public class Route : JsonEntity
    {
        [JsonProperty(PropertyName = "repeat")]
        public bool Repeat;

        [JsonProperty(PropertyName = "skippable")]
        public bool Skippable;

        [JsonProperty(PropertyName = "wait")]
        public bool Wait;
    }
}
