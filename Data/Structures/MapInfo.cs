using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures;

public class MapInfo : JsonEntity
{
    [JsonProperty(PropertyName = "id")]
    public int Id;

    [JsonProperty(PropertyName = "expanded")]
    public bool Expanded;

    [JsonProperty(PropertyName = "name")]
    public string Name;

    [JsonProperty(PropertyName = "order")]
    public int Order;
    
    [JsonProperty(PropertyName = "parentId")]
    public int Parent;

    [JsonProperty(PropertyName = "scrollX")]
    public float ScrollX;

    [JsonProperty(PropertyName = "scrollY")]
    public float ScrollY;
}