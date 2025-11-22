using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class Tileset : JsonEntity, ITileSet
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "flags")]
        public LookupList<int> _Flags { get; set; }

        public IDataLookup<int> Flags => _Flags;

        [JsonProperty(PropertyName = "mode")]
        public int Mode { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }

        [JsonProperty(PropertyName = "tilesetNames")]
        public LookupList<string> _TilesetNames { get; set; }

        public IDataLookup<string> TilesetNames => _TilesetNames;
    }
}