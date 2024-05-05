using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures;

public class System : JsonEntity
{
    [JsonProperty(PropertyName = "encryptionKey")]
    public string EncryptionKey;

    [JsonProperty(PropertyName = "switches")]
    public LookupList<string> Switches;

    [JsonProperty(PropertyName = "variables")]
    public LookupList<string> Variables;
}
