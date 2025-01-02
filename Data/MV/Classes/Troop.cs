using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Troop : JsonEntity, ITroop
{
    public class Member : JsonEntity, IMember
    {
        [JsonProperty("enemyId")]
        public int EnemyId { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }
    }

    public class EventPage : JsonEntity, ITroopPage
    {
        public class PageCondition : JsonEntity, ITroopPageCondition
        {
            [JsonProperty("turnEnding")]
            public bool TurnEnding { get; set; }

            [JsonProperty("turnValid")]
            public bool TurnValid { get; set; }

            [JsonProperty("enemyValid")]
            public bool EnemyValid { get; set; }

            [JsonProperty("actorValid")]
            public bool ActorValid { get; set; }

            [JsonProperty("switchValid")]
            public bool SwitchValid { get; set; }

            [JsonProperty("turnA")]
            public int TurnA { get; set; }

            [JsonProperty("turnB")]
            public int TurnB { get; set; }

            [JsonProperty("enemyIndex")]
            public int EnemyIndex { get; set; }

            [JsonProperty("enemyHp")]
            public int EnemyHp { get; set; }

            [JsonProperty("actorId")]
            public int ActorId { get; set; }

            [JsonProperty("actorHp")]
            public int ActorHp { get; set; }

            [JsonProperty("switchId")]
            public int SwitchId { get; set; }
        }

        [JsonProperty("span")]
        public int Span { get; set; }

        [JsonProperty("conditions")]
        public PageCondition _Condition { get; set; }
       
        public ITroopPageCondition Condition => _Condition;

        [JsonProperty(PropertyName = "list")]
        public LookupList<Command> List { get; set; }

        public List<NamedCommand> NamedCommands { get; set; }
        
        public IEnumerable<ICommandWithIndent> Commands => List;

        public override void OnDeserialize() => NamedCommands = CommandContainerHelper.Unpack(Commands, StorageVersion.MV);

        public override void OnPreSerialize() => List = CommandContainerHelper.Pack<Command>(NamedCommands, StorageVersion.MV);

        public override void Dump(TextWriter writer, EntityContext context, DumpOptions options)
        {
            if (options.Format == DumpFormat.Text)
            {
                foreach (var line in CommandContainerHelper.CommandsAsText(this, context, options))
                    writer.WriteLine(line);
                return;
            }

            base.Dump(writer, context, options);
        }
    }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("members")]
    public LookupList<Member> _Members { get; set; }

    public IByIdGetter<IMember> Members => _Members;

    [JsonProperty("pages")]
    public LookupList<EventPage> _Pages { get; set; }

    public IByIdGetter<ITroopPage> Pages => _Pages;
}
