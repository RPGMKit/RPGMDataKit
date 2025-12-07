using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class Tileset : JsonEntity, ITileSet
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "flags")]
        public List<short> _Flags { get; set; }

        public IList<short> Flags => _Flags;

        [JsonProperty(PropertyName = "mode")]
        public int Mode { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }

        [JsonProperty(PropertyName = "tilesetNames")]
        public List<string> _TilesetNames { get; set; }

        public IList<string> TilesetNames => _TilesetNames;
    }
}