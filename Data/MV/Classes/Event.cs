using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class Event : JsonEntity, IEvent
    {
        public class EventPage : MvCommandContainer, IEventPage
        {
            public class EventPageCondition : JsonEntity, IEventPageCondition
            {
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

            public class PageGraphic : JsonEntity, IPageGraphic
            {
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