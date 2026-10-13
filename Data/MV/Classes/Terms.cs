using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

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
