using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes
{
    public class Event : JsonEntity, IEvent, INewEntity
    {
        /// <summary>The values that the editor of MV gives to a new event: one empty page; the place and the number are for the map to set.</summary>
        public void InitializeNew()
        {
            Name = "";
            Note = "";
            var page = new EventPage();
            page.InitializeNew();
            Pages = [page];
        }

        public class EventPage : MvCommandContainer, IEventPage, INewEntity
        {
            /// <summary>The values that the editor of MV gives to a new page: no condition, no picture, the command that ends the page.</summary>
            public void InitializeNew()
            {
                var condition = new EventPageCondition();
                condition.InitializeNew();
                Condition = condition;
                var graphic = new PageGraphic();
                graphic.InitializeNew();
                Graphic = graphic;
                MoveType = EventMoveType.Fixed;
                MoveSpeed = 3;
                MoveFrequency = 3;
                var route = new MoveRoute();
                route.InitializeNew();
                MoveRoute = route;
                WalkAnime = true;
                StepAnime = false;
                DirectionFix = false;
                Through = false;
                PriorityType = EventPriority.BelowCharacters;
                Trigger = EventTriggerType.ActionButton;
                NamedCommands = [CommandContainerHelper.CreateNamed(new Command { Code = 0, Indent = 0, Parameters = [] }, StorageVersion.MV)];
            }

            public class EventPageCondition : JsonEntity, IEventPageCondition, INewEntity
            {
                /// <summary>The values that the editor of MV gives to a new condition: nothing is required.</summary>
                public void InitializeNew()
                {
                    Switch1Valid = false;
                    Switch2Valid = false;
                    VariableValid = false;
                    SelfSwitchValid = false;
                    ItemValid = false;
                    ActorValid = false;
                    Switch1Id = 1;
                    Switch2Id = 1;
                    VariableId = 1;
                    VariableValue = 0;
                    SelfSwitchCh = "A";
                    ItemId = 1;
                    ActorId = 1;
                }

                [JsonProperty("switch1Valid")]
                public bool Switch1Valid { get; set; }

                [JsonProperty("switch2Valid")]
                public bool Switch2Valid { get; set; }

                [JsonProperty("variableValid")]
                public bool VariableValid { get; set; }

                [JsonProperty("selfSwitchValid")]
                public bool SelfSwitchValid { get; set; }

                [JsonProperty("itemValid")]
                public bool ItemValid { get; set; }

                [JsonProperty("actorValid")]
                public bool ActorValid { get; set; }

                [JsonProperty("switch1Id")]
                public int Switch1Id { get; set; }

                [JsonProperty("switch2Id")]
                public int Switch2Id { get; set; }

                [JsonProperty("variableId")]
                public int VariableId { get; set; }

                [JsonProperty("variableValue")]
                public int VariableValue { get; set; }

                [JsonProperty("selfSwitchCh")]
                public string SelfSwitchCh { get; set; }

                [JsonProperty("itemId")]
                public int ItemId { get; set; }

                [JsonProperty("actorId")]
                public int ActorId { get; set; }
            }

            public class PageGraphic : JsonEntity, IPageGraphic, INewEntity
            {
                /// <summary>The values that the editor of MV gives to a new picture of a page: nothing, facing down.</summary>
                public void InitializeNew()
                {
                    TileId = 0;
                    CharacterName = "";
                    CharacterIndex = 0;
                    Direction = (int)Enums.Direction.Down;
                    Pattern = 0;
                }

                [JsonProperty("tileId")]
                public int TileId { get; set; }

                [JsonProperty("characterName")]
                public string CharacterName { get; set; }

                [JsonProperty("characterIndex")]
                public int CharacterIndex { get; set; }

                [JsonProperty("direction")]
                public int Direction { get; set; }

                [JsonProperty("pattern")]
                public int Pattern { get; set; }
            }

            [JsonProperty("conditions")]
            public IEventPageCondition Condition { get; set; }

            [JsonProperty("image")]
            public IPageGraphic Graphic { get; set; }

            [JsonProperty("moveType")]
            public int MoveType { get; set; }

            [JsonProperty("moveSpeed")]
            public int MoveSpeed { get; set; }

            [JsonProperty("moveFrequency")]
            public int MoveFrequency { get; set; }

            [JsonProperty("moveRoute")]
            public IMoveRoute MoveRoute { get; set; }

            [JsonProperty("walkAnime")]
            public bool WalkAnime { get; set; }

            [JsonProperty("stepAnime")]
            public bool StepAnime { get; set; }

            [JsonProperty("directionFix")]
            public bool DirectionFix { get; set; }

            [JsonProperty("through")]
            public bool Through { get; set; }

            [JsonProperty("priorityType")]
            public int PriorityType { get; set; }

            [JsonProperty("trigger")]
            public int Trigger { get; set; }

            public async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
            {
                if (options.Format == DumpFormat.Text)
                {
                    await MapEventHelper.DumpPageInfo(this, writer, context, options);
                    await writer.WriteLineAsync("Commands:");
                    foreach (var line in CommandContainerHelper.CommandsAsText(this, context, options))
                        await writer.WriteLineAsync(line);
                    return;
                }
            }
        }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }

        [JsonProperty(PropertyName = "pages")]
        public List<IEventPage?> Pages { get; set; } = new();

        IList<IEventPage?> IEvent.Pages => Pages;

        [JsonProperty(PropertyName = "x")]
        public int X { get; set; }

        [JsonProperty(PropertyName = "y")]
        public int Y { get; set; }
    }
}