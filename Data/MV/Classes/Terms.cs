using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Terms : JsonEntity
{
    [JsonProperty("basic")]
    public List<string> Basic { get; set; }

    [JsonProperty("commands")]
    public List<string?> Commands { get; set; }

    [JsonProperty("messages")]
    public Dictionary<string, string> Messages { get; set; }

    [JsonProperty("params")]
    public List<string> Params;
}
