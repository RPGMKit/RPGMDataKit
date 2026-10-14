using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Trait : JsonEntity, IFeature, INewEntity
{
    /// <summary>What the editor of MV puts into a trait that is added and not changed: the rate of damage from the element 1 is 100%.</summary>
    public void InitializeNew()
    {
        Code = FeatureCodes.ElementRate;
        DataId = 1;
        Value = 1;
    }

    [JsonProperty("code")]
    public int Code { get; set; }
    
    [JsonProperty("dataId")]
    public int DataId { get; set; }
    
    [JsonProperty("value")]
    public object Value { get; set; }
}
