using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::State")]
public class State : BaseItem
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

    [RubyProperty("@release_by_damage", CandidateFlags.In)]
    public bool ReleaseByDamage;
    
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
}
