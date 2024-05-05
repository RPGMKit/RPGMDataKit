using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Troop")]
public class Troop : RubySerializationEntity
{
    [RubyObject("RPG::Troop::Member")]
    public class Member : RubySerializationEntity
    {
        [RubyProperty("@enemy_id")]
        public int EnemyId;
        [RubyProperty("@x")]
        public int X;
        [RubyProperty("@y")]
        public int Y;
        [RubyProperty("@hidden")]
        public bool Hidden;
    }

    [RubyObject("RPG::Troop::Page")]
    public class Page : RubySerializationEntity
    {
        [RubyObject("RPG::Troop::Page::Condition")]
        public class PageCondition : RubySerializationEntity
        {
            [RubyProperty("@turn_ending")]
            public bool TurnEnding;

            [RubyProperty("@turn_valid")]
            public bool TurnValid;

            [RubyProperty("@enemy_valid")]
            public bool EnemyValid;

            [RubyProperty("@actor_valid")]
            public bool ActorValid;

            [RubyProperty("@switch_valid")]
            public bool SwitchValid;

            [RubyProperty("@turn_a")]
            public int TurnA;

            [RubyProperty("@turn_b")]
            public int TurnB;

            [RubyProperty("@enemy_index")]
            public int EnemyIndex;

            [RubyProperty("@enemy_hp")]
            public int EnemyHp;

            [RubyProperty("@actor_id")]
            public int ActorId;

            [RubyProperty("@actor_hp")]
            public int ActorHp;

            [RubyProperty("@switch_id")]
            public int SwitchId;
        }

        [RubyProperty("@condition")]
        public PageCondition Condition;
        [RubyProperty("@span")]
        public int Span;
        [RubyProperty("@list")]
        public List<EventCommand> List;
    }

    [RubyProperty("@id")]
    public int Id;

    [RubyProperty("@name")]
    public string Name;

    [RubyProperty("@members")]
    public List<object> Members;

    [RubyProperty("@pages")]
    public List<Page> Pages;
}
