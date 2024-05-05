using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class System : JsonEntity, ISystem
{
    [JsonProperty(PropertyName = "encryptionKey")]
    public string EncryptionKey { get; set; }

    [JsonProperty(PropertyName = "switches")]
    public LookupList<string> Switches { get; set; }

    [JsonProperty(PropertyName = "variables")]
    public LookupList<string> Variables { get; set; }
}
