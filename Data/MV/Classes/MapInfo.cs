using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class MapInfo : JsonEntity, IMapInfo
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
    public float ScrollXFloat { get; set; }

    [JsonProperty(PropertyName = "scrollY")]
    public float ScrollYFloat { get; set; }
    
    public int ScrollX
    {
        get => (int)ScrollXFloat;
        set => ScrollXFloat = value;
    }

    public int ScrollY
    {
        get => (int)ScrollYFloat;
        set => ScrollYFloat = value;
    }
}