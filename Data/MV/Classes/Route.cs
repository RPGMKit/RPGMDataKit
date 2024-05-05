using Newtonsoft.Json;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class Route
    {
        [JsonProperty(PropertyName = "repeat")]
        public bool Repeat;

        [JsonProperty(PropertyName = "skippable")]
        public bool Skippable;

        [JsonProperty(PropertyName = "wait")]
        public bool Wait;
    }
}
