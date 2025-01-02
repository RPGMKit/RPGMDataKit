using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Class : JsonEntity, IClass
{
    public class Learning : JsonEntity, ILearning
    {
        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("skillId")]
        public int SkillId { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
    }
    
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("note")]
    public string Note { get; set; }
    
    [JsonProperty("traits")]
    public List<Trait> Traits { get; set; }
    
    [JsonProperty("expParams")]
    public List<int> ExpParams { get; set; }

    [JsonProperty("params")]
    public List<object> Params { get; set; }

    [JsonProperty("learnings")]
    public List<Learning> Learnings { get; set; }
}
