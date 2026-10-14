using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.MV.Classes;

/// <summary>What a weapon and an armor of MV have in common (the fields are the same as in <c>RPG::EquipItem</c> of VX Ace).</summary>
public abstract class EquipItem : BaseItem, IEquipItem
{
    [JsonProperty("iconIndex")]
    public int IconIndex { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("price")]
    public int Price { get; set; }

    [JsonProperty("etypeId")]
    public int ETypeId { get; set; }

    [JsonProperty("params")]
    public List<int> Params { get; set; }

    [JsonProperty("traits")]
    public List<Trait> Traits { get; set; }

    IReadOnlyList<IFeature> IHasFeatures.Features => Traits;

    public override void InitializeNew()
    {
        base.InitializeNew();
        IconIndex = 0;
        Description = "";
        Price = 0;
        ETypeId = 1;
        Params = [0, 0, 0, 0, 0, 0, 0, 0];
        Traits = new();
    }
}
