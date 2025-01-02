using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;
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

        public IEnumerable<ICommandWithIndent> Commands => List;

        public override void OnDeserialize() => NamedCommands = CommandContainerHelper.Unpack(Commands, StorageVersion.VXAce);

        public override void OnPreSerialize() => List = CommandContainerHelper.Pack<Command>(NamedCommands, StorageVersion.VXAce);

        public void Dump(TextWriter writer, EntityContext context, DumpOptions options)
        {
            if (options.Format == DumpFormat.Text)
            {
                foreach (var line in CommandContainerHelper.CommandsAsText(this, context, options))
                    writer.WriteLine(line);
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
    public LookupList<Member> _Members { get; set; }

    public IByIdGetter<IMember> Members => _Members;

    [RubyProperty("@pages")]
    public LookupList<EventPage> _Pages { get; set; }

    public IByIdGetter<ITroopPage> Pages => _Pages;
}
