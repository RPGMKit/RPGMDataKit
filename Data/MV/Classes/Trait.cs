using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Trait : JsonEntity, IFeature
{
    [JsonProperty("code")]
    public int Code { get; set; }
    
    [JsonProperty("dataId")]
    public int DataId { get; set; }
    
    [JsonProperty("value")]
    public object Value { get; set; }
}
