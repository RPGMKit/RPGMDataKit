using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Helpers.Dump;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Troop : JsonEntity, ITroop, INewEntity
{
    /// <summary>The values that the editor of MV gives to a new troop: no enemies, one empty page of the battle events.</summary>
    public void InitializeNew()
    {
        Name = "";
        Members = new();
        var page = new EventPage();
        page.InitializeNew();
        Pages = [page];
    }

    public class Member : JsonEntity, IMember, INewEntity
    {
        /// <summary>What the editor of MV gives to an enemy that is added to a troop: the first enemy at the middle of the field (408, 436), visible.</summary>
        public void InitializeNew()
        {
            EnemyId = 1;
            X = 408;
            Y = 436;
            Hidden = false;
        }

        [JsonProperty("enemyId")]
        public int EnemyId { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }
    }

    public class EventPage : MvCommandContainer, ITroopPage, INewEntity
    {
        /// <summary>The values that the editor of MV gives to a new page: no condition, only the command that ends the page.</summary>
        public void InitializeNew()
        {
            var condition = new PageCondition();
            condition.InitializeNew();
            Condition = condition;
            Span = TroopPageSpan.Battle;
            NamedCommands = [CommandContainerHelper.CreateNamed(new Command { Code = 0, Indent = 0, Parameters = [] }, StorageVersion.MV)];
        }

        public class PageCondition : JsonEntity, ITroopPageCondition, INewEntity
        {
            /// <summary>The values that the editor of MV gives to a new condition: nothing is required.</summary>
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

        public async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
        {
            var head = new StringWriter();
            await TroopHelper.DumpPageInfo(this, head, context, options);
            await DumpWriters.For(options.Format).Write(writer, head.ToString(), this, context, options);
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
