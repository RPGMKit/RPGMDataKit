using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Helpers.Dump;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Troop")]
public class Troop : RubySerializationEntity, ITroop, INewEntity
{
    /// <summary>The values of <c>RPG::Troop.new</c> of VX Ace: no enemies, one empty page of the battle events.</summary>
    public void InitializeNew()
    {
        Name = "";
        Members = new();
        var page = new EventPage();
        page.InitializeNew();
        Pages = [page];
    }

    [RubyObject("RPG::Troop::Member")]
    public class Member : RubySerializationEntity, IMember, INewEntity
    {
        /// <summary>What the editor of VX Ace gives to an enemy that is added to a troop: the first enemy at the middle of the field (272, 288), visible.</summary>
        public void InitializeNew()
        {
            EnemyId = 1;
            X = 272;
            Y = 288;
            Hidden = false;
        }

        [RubyProperty("@enemy_id")]
        public int EnemyId { get; set; }

        [RubyProperty("@x")]
        public int X { get; set; }

        [RubyProperty("@y")]
        public int Y { get; set; }

        [RubyProperty("@hidden")]
        public bool Hidden { get; set; }
    }

    [RubyObject("RPG::Troop::Page")]
    public class EventPage : VxCommandContainer, ITroopPage, INewEntity
    {
        /// <summary>The values of <c>RPG::Troop::Page.new</c> of VX Ace: no condition, only the command that ends the page.</summary>
        public void InitializeNew()
        {
            var condition = new PageCondition();
            condition.InitializeNew();
            Condition = condition;
            Span = TroopPageSpan.Battle;
            NamedCommands = [CommandContainerHelper.CreateNamed(new Command { Code = 0, Indent = 0, Parameters = [] }, StorageVersion.VXAce)];
        }

        [RubyObject("RPG::Troop::Page::Condition")]
        public class PageCondition : RubySerializationEntity, ITroopPageCondition, INewEntity
        {
            /// <summary>The values of <c>RPG::Troop::Page::Condition.new</c> of VX Ace: nothing is required.</summary>
            public void InitializeNew()
            {
                TurnEnding = false;
                TurnValid = false;
                EnemyValid = false;
                ActorValid = false;
                SwitchValid = false;
                TurnA = 0;
                TurnB = 0;
                EnemyIndex = 0;
                EnemyHp = 50;
                ActorId = 1;
                ActorHp = 50;
                SwitchId = 1;
            }

            [RubyProperty("@turn_ending")]
            public bool TurnEnding { get; set; }

            [RubyProperty("@turn_valid")]
            public bool TurnValid { get; set; }

            [RubyProperty("@enemy_valid")]
            public bool EnemyValid { get; set; }

            [RubyProperty("@actor_valid")]
            public bool ActorValid { get; set; }

            [RubyProperty("@switch_valid")]
            public bool SwitchValid { get; set; }

            [RubyProperty("@turn_a")]
            public int TurnA { get; set; }

            [RubyProperty("@turn_b")]
            public int TurnB { get; set; }

            [RubyProperty("@enemy_index")]
            public int EnemyIndex { get; set; }

            [RubyProperty("@enemy_hp")]
            public int EnemyHp { get; set; }

            [RubyProperty("@actor_id")]
            public int ActorId { get; set; }

            [RubyProperty("@actor_hp")]
            public int ActorHp { get; set; }

            [RubyProperty("@switch_id")]
            public int SwitchId { get; set; }
        }

        [RubyProperty("@condition")]
        public ITroopPageCondition Condition { get; set; }

        [RubyProperty("@span")]
        public int Span { get; set; }

        public async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
        {
            var head = new StringWriter();
            await TroopHelper.DumpPageInfo(this, head, context, options);
            await DumpWriters.For(options.Format).Write(writer, head.ToString(), this, context, options);
        }
    }

    [RubyProperty("@id")]
    public int Id { get; set; }

    [RubyProperty("@name")]
    public string Name { get; set; }

    [RubyProperty("@members")]
    public List<IMember?> Members { get; set; } = new();

    IList<IMember?> ITroop.Members => Members;

    [RubyProperty("@pages")]
    public List<ITroopPage?> Pages { get; set; } = new();

    IList<ITroopPage?> ITroop.Pages => Pages;
}
