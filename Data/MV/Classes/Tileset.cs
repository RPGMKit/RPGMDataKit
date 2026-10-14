using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures.Internal.Converters;

namespace RPGMDataKit.Data.MV.Classes
{
    public class Tileset : JsonEntity, ITileSet, INewEntity
    {
        /// <summary>What the editor of MV gives to a new tileset: the mode of the area map, nine empty names, one flag.</summary>
        public void InitializeNew()
        {
            Mode = (int)TilesetMode.AreaType;
            Name = "";
            TilesetNames = [.. Enumerable.Repeat("", 9)];
            // the editor of MV writes only the first flag of a new tileset; the others are added when the tiles are set
            Flags = Table.FromCells([0x0010]);
            Note = "";
        }

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