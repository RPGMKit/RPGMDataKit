using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Map : JsonEntity
{
    [JsonProperty(PropertyName = "tilesetId")]
    public int TilesetId { get; set; }

    [JsonProperty(PropertyName = "displayName")]
    public string DisplayName { get; set; }

    [JsonProperty(PropertyName = "height")]
    public int Height { get; set; }

    [JsonProperty(PropertyName = "width")]
    public int Width { get; set; }

    // ground 1 => ground 2? => bottom object => top object => shadow flags => region id
    [JsonProperty(PropertyName = "data")]
    public LookupList<int> Data { get; set; } = new LookupList<int>();

    [JsonProperty(PropertyName = "events")]
    public LookupList<Event> Events { get; set; }
}
