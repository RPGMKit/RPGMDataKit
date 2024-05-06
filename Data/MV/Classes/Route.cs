using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class Route : IRoute
    {
        [JsonProperty(PropertyName = "repeat")]
        public bool Repeat { get; set; }

        [JsonProperty(PropertyName = "skippable")]
        public bool Skippable { get; set; }

        [JsonProperty(PropertyName = "wait")]
        public bool Wait { get; set; }
    }
}
