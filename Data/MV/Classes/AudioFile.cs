using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes
{
    public abstract class AudioFile : JsonEntity, IAudioFile
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "volume")]
        public int Volume { get; set; }

        [JsonProperty(PropertyName = "pitch")]
        public int Pitch { get; set; }

        [JsonProperty(PropertyName = "pan")]
        public int Pan { get; set; }
    }
}
