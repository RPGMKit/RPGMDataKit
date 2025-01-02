using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class Event : JsonEntity, IEvent
    {
        public class EventPage : JsonEntity, IEventPage
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
            public EventPageCondition _Condition { get; set; }

            public IEventPageCondition Condition => _Condition;

            [JsonProperty("image")]
            public PageGraphic _Graphic { get; set; }

            public IPageGraphic Graphic => _Graphic;

            [JsonProperty("moveType")]
            public int MoveType { get; set; }

            [JsonProperty("moveSpeed")]
            public int MoveSpeed { get; set; }

            [JsonProperty("moveFrequency")]
            public int MoveFrequency { get; set; }

            [JsonProperty("moveRoute")]
            public MoveRoute _MoveRoute { get; set; }
            
            public IMoveRoute MoveRoute => _MoveRoute;

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

            [JsonProperty("list")]
            public LookupList<Command> List { get; set; }

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

            public List<NamedCommand> NamedCommands { get; set; }
            public IEnumerable<ICommandWithIndent> Commands => List;

            public override void OnDeserialize() =>
                NamedCommands = CommandContainerHelper.Unpack(Commands, StorageVersion.MV);

            public override void OnPreSerialize() =>
                List = CommandContainerHelper.Pack<Command>(NamedCommands, StorageVersion.MV);
        }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }
        [JsonProperty(PropertyName = "pages")]
        public LookupList<EventPage> _Pages { get; set; }

        public IByIdGetter<IEventPage> Pages => _Pages;

        [JsonProperty(PropertyName = "x")]
        public int X { get; set; }
        [JsonProperty(PropertyName = "y")]
        public int Y { get; set; }
    }
}