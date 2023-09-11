using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data;

public class Map : JsonEntity
{
    [JsonProperty(PropertyName = "tilesetId")]
    public int TilesetId;
    
    [JsonProperty(PropertyName = "displayName")]
    public string DisplayName;

    [JsonProperty(PropertyName = "height")]
    public int Height;
    
    [JsonProperty(PropertyName = "width")]
    public int Width;
    
    // ground 1 => ground 2? => bottom object => top object => shadow flags => region id
    [JsonProperty(PropertyName = "data")]
    public LookupList<int> Data = new LookupList<int>();

    [JsonProperty(PropertyName = "events")]
    public LookupList<MapEvent> Events;
}
