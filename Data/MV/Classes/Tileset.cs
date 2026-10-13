using Newtonsoft.Json;
using RPGMDataKit.Data.Structures;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures.Internal.Converters;

namespace RPGMDataKit.Data.MV.Classes
{
    public class Tileset : JsonEntity, ITileSet, INewEntity
    {
        /// <summary>The values that the editor of MV gives to a new tileset: the mode of the world map, nine empty names, the flags of the empty tiles.</summary>
        public void InitializeNew()
        {
            Mode = 1;
            Name = "";
            TilesetNames = [.. Enumerable.Repeat("", 9)];
            Flags = new Table(8192);
            Flags[0] = 0x0010;
            for (var tile = 2048; tile <= 2815; tile++)
                Flags[tile] = 0x000F;

            for (var tile = 4352; tile <= 8191; tile++)
                Flags[tile] = 0x000F;

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