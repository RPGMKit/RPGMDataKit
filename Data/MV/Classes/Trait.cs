using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Trait : JsonEntity, IFeature
{
    [JsonProperty("code")]
    public int Code { get; set; }
    
    [JsonProperty("dataId")]
    public int DataId { get; set; }
    
    [JsonProperty("value")]
    public object Value { get; set; }
}
