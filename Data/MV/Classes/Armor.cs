using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Armor : JsonEntity, IArmor
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("note")]
    public string Note { get; set; }

    [JsonProperty("atypeId")]
    public int ATypeId { get; set; }

    [JsonProperty("description")]
    public string Descriptiong { get; set; }

    [JsonProperty("etypeId")]
    public int ETypeId { get; set; }

    [JsonProperty("traits")]
    public List<Trait> Traits { get; set; }

    [JsonProperty("iconIndex")]
    public int IconIndex { get; set; }

    [JsonProperty("params")]
    public List<int> Params { get; set; }

    [JsonProperty("price")]
    public int Price { get; set; }
}
