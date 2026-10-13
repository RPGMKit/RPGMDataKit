using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Armor : JsonEntity, IArmor, INewEntity
{
    /// <summary>The values that the editor of MV gives to a new armor: the equipment type 2 (a shield), the trait of the evasion.</summary>
    public void InitializeNew()
    {
        Name = "";
        Note = "";
        ATypeId = 0;
        Descriptiong = "";
        ETypeId = 2;
        IconIndex = 0;
        Params = [0, 0, 0, 0, 0, 0, 0, 0];
        Price = 0;
        Traits = [new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.Evasion, Value = 0 }];
    }

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
