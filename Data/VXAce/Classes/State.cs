using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::State")]
public class State : BaseItem, IState
{
    [RubyProperty("@restriction")]
    public int Restriction { get; set; }

    [RubyProperty("@priority")]
    public int Priority { get; set; }

    [RubyProperty("@remove_at_battle_end")]
    public bool RemoveAtBattleEnd { get; set; }

    [RubyProperty("@remove_by_restriction")]
    public bool RemoveByRestriction { get; set; }

    [RubyProperty("@auto_removal_timing")]
    public int AutoRemovalTiming { get; set; }

    [RubyProperty("@min_turns")]
    public int MinTurns { get; set; }

    [RubyProperty("@max_turns")]
    public int MaxTurns { get; set; }

    [RubyProperty("@remove_by_damage")]
    public bool RemoveByDamage { get; set; }

    [RubyProperty("@chance_by_damage")]
    public int ChanceByDamage { get; set; }

    [RubyProperty("@remove_by_walking")]
    public bool RemoveByWalking { get; set; }

    [RubyProperty("@steps_to_remove")]
    public int StepsToRemove { get; set; }

    [RubyProperty("@message1")]
    public string Message1 { get; set; }

    [RubyProperty("@message2")]
    public string Message2 { get; set; }

    [RubyProperty("@message3")]
    public string Message3 { get; set; }

    [RubyProperty("@message4")]
    public string Message4 { get; set; }

    /// <summary>The values of <c>RPG::State.new</c> of VX Ace: lasts one turn, priority 50, 100 steps to remove by walking.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        Restriction = StateRestriction.None;
        Priority = 50;
        RemoveAtBattleEnd = false;
        RemoveByRestriction = false;
        AutoRemovalTiming = Enums.AutoRemovalTiming.None;
        MinTurns = 1;
        MaxTurns = 1;
        RemoveByDamage = false;
        ChanceByDamage = 100;
        RemoveByWalking = false;
        StepsToRemove = 100;
        Message1 = "";
        Message2 = "";
        Message3 = "";
        Message4 = "";
    }
}
