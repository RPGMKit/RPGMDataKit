using RPGMDataKit.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::State")]
public class State : BaseItem, IState
{
    [RubyProperty("@restriction")]
    public int Restriction;

    [RubyProperty("@priority")]
    public int Priority;

    [RubyProperty("@remove_at_battle_end")]
    public bool RemoveAtBattleEnd;

    [RubyProperty("@remove_by_restriction")]
    public bool RemoveByRestriction;

    [RubyProperty("@auto_removal_timing")]
    public int AutoRemovalTiming;

    [RubyProperty("@min_turns")]
    public int MinTurns;

    [RubyProperty("@max_turns")]
    public int MaxTurns;

    [RubyProperty("@remove_by_damage")]
    public bool RemoveByDamage;
    
    [RubyProperty("@chance_by_damage")]
    public int ChanceByDamage;

    [RubyProperty("@remove_by_walking")]
    public bool RemoveByWalking;

    [RubyProperty("@steps_to_remove")]
    public int StepsToRemove;

    [RubyProperty("@message1")]
    public string Message1;

    [RubyProperty("@message2")]
    public string Message2;

    [RubyProperty("@message3")]
    public string Message3;

    [RubyProperty("@message4")]
    public string Message4;

    /// <summary>The values of <c>RPG::State.new</c> of VX Ace: lasts one turn, priority 50, 100 steps to remove by walking.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        Restriction = 0;
        Priority = 50;
        RemoveAtBattleEnd = false;
        RemoveByRestriction = false;
        AutoRemovalTiming = 0;
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
