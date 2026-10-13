using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes
{
    public abstract class AudioFile : JsonEntity, IAudioFile, INewEntity
    {
        /// <summary>The values that the editor of MV gives to a new sound: no file, the volume of 90%, the pitch of 100%, the pan 0.</summary>
        public void InitializeNew()
        {
            Name = "";
            Volume = 90;
            Pitch = 100;
            Pan = 0;
        }

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
