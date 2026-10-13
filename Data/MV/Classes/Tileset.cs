using Newtonsoft.Json;
using RPGMDataKit.Data.Structures;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures.Internal.Converters;

namespace RPGMDataKit.Data.MV.Classes
{
    public class Tileset : JsonEntity, ITileSet
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "flags")]
        [JsonConverter(typeof(FlatTableJsonConverter))]
        public Table Flags { get; set; } = new();

        [JsonProperty(PropertyName = "mode")]
        public int Mode { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }

        [JsonProperty(PropertyName = "tilesetNames")]
        public List<string> TilesetNames { get; set; } = new();

        IList<string> ITileSet.TilesetNames => TilesetNames;
    }
}