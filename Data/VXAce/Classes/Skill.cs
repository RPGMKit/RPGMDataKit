using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Skill")]
public class Skill : UsableItem, ISkill
{
    [RubyProperty("@stype_id")]
    public int STypeId;

    [RubyProperty("@mp_cost")]
    public int MpCost;

    [RubyProperty("@tp_cost")]
    public int TpCost;

    [RubyProperty("@message1")]
    public string Message1;

    [RubyProperty("@message2")]
    public string Message2;

    [RubyProperty("@required_wtype_id1")]
    public int RequiredWTypeId1;

    [RubyProperty("@required_wtype_id2")]
    public int RequiredWTypeId2;

    /// <summary>The values of <c>RPG::Skill.new</c> of VX Ace: on one enemy, always usable, of the first type of skills.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        Scope = ItemScope.OneEnemy;
        Occasion = ItemOccasion.Always;
        STypeId = 1;
        MpCost = 0;
        TpCost = 0;
        Message1 = "";
        Message2 = "";
        RequiredWTypeId1 = 0;
        RequiredWTypeId2 = 0;
    }
}
