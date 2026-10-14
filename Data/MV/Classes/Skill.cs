using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.MV.Classes;

public class Skill : UsableItem, ISkill
{
    [JsonProperty("stypeId")]
    public int STypeId { get; set; }

    [JsonProperty("mpCost")]
    public int MpCost { get; set; }

    [JsonProperty("tpCost")]
    public int TpCost { get; set; }

    [JsonProperty("message1")]
    public string Message1 { get; set; }

    [JsonProperty("message2")]
    public string Message2 { get; set; }

    [JsonProperty("requiredWtypeId1")]
    public int RequiredWTypeId1 { get; set; }

    [JsonProperty("requiredWtypeId2")]
    public int RequiredWTypeId2 { get; set; }

    /// <summary>The values that the editor of MV gives to a new skill: on one enemy, in battle, of the first type of skills.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        Scope = ItemScope.OneEnemy;
        Occasion = ItemOccasion.Always; // as in VX Ace (not checked in the editor of MV yet)
        STypeId = 1;
        MpCost = 0;
        TpCost = 0;
        Message1 = "";
        Message2 = "";
        RequiredWTypeId1 = 0;
        RequiredWTypeId2 = 0;
    }
}
