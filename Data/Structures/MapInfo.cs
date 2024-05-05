using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures;

public class MapInfo : JsonEntity
{
    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }

    [JsonProperty(PropertyName = "expanded")]
    public bool Expanded { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "order")]
    public int Order { get; set; }
    
    [JsonProperty(PropertyName = "parentId")]
    public int Parent { get; set; }

    [JsonProperty(PropertyName = "scrollX")]
    public float ScrollX { get; set; }

    [JsonProperty(PropertyName = "scrollY")]
    public float ScrollY { get; set; }
}