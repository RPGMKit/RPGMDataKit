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
}
