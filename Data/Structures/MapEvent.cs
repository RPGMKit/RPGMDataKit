using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures
{
    public class MapEvent : JsonEntity
    {
        [JsonProperty(PropertyName = "id")]
        public int Id;
        [JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonProperty(PropertyName = "note")]
        public string Note;
        [JsonProperty(PropertyName = "pages")]
        public LookupList<EventPage> Pages;
        [JsonProperty(PropertyName = "x")]
        public int X;
        [JsonProperty(PropertyName = "y")]
        public int Y;
    }
}