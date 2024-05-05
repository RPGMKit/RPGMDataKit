using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Animation : JsonEntity
{
    [JsonProperty(PropertyName = "id")]
    public int Id;
    [JsonProperty(PropertyName = "name")]
    public string Name;
}