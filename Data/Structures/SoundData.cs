using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures
{
    public class SoundData : JsonEntity
    {
        [JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonProperty(PropertyName = "volume")]
        public int Volume;
        [JsonProperty(PropertyName = "pitch")]
        public int Pitch;
        [JsonProperty(PropertyName = "pan")]
        public int Pan;
    }
}
