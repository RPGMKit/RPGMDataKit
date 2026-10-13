using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Trait : JsonEntity, IFeature, INewEntity
{
    /// <summary>The values of a new trait, as in VX Ace (not checked in the editor of MV yet): the attack element 1 with the value 1.</summary>
    public void InitializeNew()
    {
        Code = FeatureCodes.AttackElement;
        DataId = 1;
        Value = 1.0;
    }

    [JsonProperty("code")]
    public int Code { get; set; }
    
    [JsonProperty("dataId")]
    public int DataId { get; set; }
    
    [JsonProperty("value")]
    public object Value { get; set; }
}
