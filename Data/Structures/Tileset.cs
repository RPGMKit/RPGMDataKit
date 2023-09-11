using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures
{
    public class Tileset : JsonEntity
    {
        [JsonProperty(PropertyName = "id")]
        public int Id;
        [JsonProperty(PropertyName = "flags")]
        public LookupList<int> Flags;
        [JsonProperty(PropertyName = "mode")]
        public TilesetMode Mode;
        [JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonProperty(PropertyName = "note")]
        public string Note;
        [JsonProperty(PropertyName = "tilesetNames")]
        public LookupList<string> TilesetNames;
    }
}