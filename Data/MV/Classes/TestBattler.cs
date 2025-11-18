using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class TestBattler : JsonEntity
{
    [JsonProperty("actorId")]
    public int ActorId { get; set; }

    [JsonProperty("equips")]
    public List<int> Equips { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }
}
