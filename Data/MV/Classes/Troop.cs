using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

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
        public ITroopPageCondition Condition { get; set; }

        private readonly CommandList<Command> _commands = new(StorageVersion.MV);

        [JsonProperty(PropertyName = "list")]
        public List<Command> List
        {
            get => _commands.Raw!;
            set => _commands.Raw = value;
        }

        public List<NamedCommand> NamedCommands
        {
            get => _commands.Named;
            set => _commands.Named = value;
        }

        public IEnumerable<ICommand> Commands => List;

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
        }
    }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("members")]
    public List<IMember?> Members { get; set; } = new();

    IList<IMember?> ITroop.Members => Members;

    [JsonProperty("pages")]
    public List<ITroopPage?> Pages { get; set; } = new();

    IList<ITroopPage?> ITroop.Pages => Pages;
}
