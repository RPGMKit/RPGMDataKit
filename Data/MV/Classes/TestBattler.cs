using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class TestBattler : JsonEntity
{
    [JsonProperty("actorId")]
    public int ActorId { get; set; }

    [JsonProperty("equips")]
    public List<int> Equips { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }
}
