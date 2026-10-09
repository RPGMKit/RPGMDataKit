using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Troop")]
public class Troop : RubySerializationEntity, ITroop
{
    [RubyObject("RPG::Troop::Member")]
    public class Member : RubySerializationEntity, IMember
    {
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
    public class EventPage : RubySerializationEntity, ITroopPage
    {
        [RubyObject("RPG::Troop::Page::Condition")]
        public class PageCondition : RubySerializationEntity, ITroopPageCondition
        {
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
        public PageCondition _Condition { get; set; }

        public ITroopPageCondition Condition => _Condition;

        [RubyProperty("@span")]
        public int Span { get; set; }

        [RubyProperty("@list")]
        public List<Command> List;

        public List<NamedCommand> NamedCommands { get; set; }

        public IEnumerable<ICommand> Commands => List;

        public override void OnDeserialized()
        {
            base.OnDeserialized();
            NamedCommands = CommandContainerHelper.Unpack(List, StorageVersion.VXAce);
        }

        public override void OnSerializing()
        {
            base.OnSerializing();
            List = CommandContainerHelper.Pack<Command>(NamedCommands, StorageVersion.VXAce);
        }

        public async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
        {
            if (options.Format == DumpFormat.Text)
            {
                await TroopHelper.DumpPageInfo(this, writer, context, options);
                await writer.WriteLineAsync("Commands:");
                foreach (var line in CommandContainerHelper.CommandsAsText(this, context, options))
                    await writer.WriteLineAsync(line);
                return;
            }

            throw new NotImplementedException($"{options.Format} not supported");
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
