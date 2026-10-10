using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Event")]
public class Event : RubySerializationEntity, IEvent
{
    [RubyObject("RPG::Event::Page")]
    public class EventPage : RubySerializationEntity, IEventPage
    {
        [RubyObject("RPG::Event::Page::Condition")]
        public class EventPageCondition : RubySerializationEntity, IEventPageCondition
        {
            [RubyProperty("@switch1_valid")]
            public bool Switch1Valid { get; set; }

            [RubyProperty("@switch2_valid")]
            public bool Switch2Valid { get; set; }

            [RubyProperty("@variable_valid")]
            public bool VariableValid { get; set; }

            [RubyProperty("@self_switch_valid")]
            public bool SelfSwitchValid { get; set; }

            [RubyProperty("@item_valid")]
            public bool ItemValid { get; set; }

            [RubyProperty("@actor_valid")]
            public bool ActorValid { get; set; }

            [RubyProperty("@switch1_id")]
            public int Switch1Id { get; set; }

            [RubyProperty("@switch2_id")]
            public int Switch2Id { get; set; }

            [RubyProperty("@variable_id")]
            public int VariableId { get; set; }

            [RubyProperty("@variable_value")]
            public int VariableValue { get; set; }

            [RubyProperty("@self_switch_ch", CandidateFlags.Character)]
            public string SelfSwitchCh { get; set; }

            [RubyProperty("@item_id")]
            public int ItemId { get; set; }

            [RubyProperty("@actor_id")]
            public int ActorId { get; set; }
            
            [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
            public object _1 { get; set; }
        }

        [RubyObject("RPG::Event::Page::Graphic")]
        public class PageGraphic : RubySerializationEntity, IPageGraphic
        {
            [RubyProperty("@tile_id")]
            public int TileId { get; set; }
            
            [RubyProperty("@character_name")]
            public string CharacterName { get; set; }
            
            [RubyProperty("@character_index")]
            public int CharacterIndex { get; set; }
            
            [RubyProperty("@direction")]
            public int Direction { get; set; }
            
            [RubyProperty("@pattern")]
            public int Pattern { get; set; }
            
            [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
            public object _1 { get; set; }
        }

        [RubyProperty("@condition")]
        public IEventPageCondition Condition { get; set; }

        [RubyProperty("@graphic")]
        public IPageGraphic Graphic { get; set; }

        [RubyProperty("@move_type")]
        public int MoveType { get; set; }

        [RubyProperty("@move_speed")]
        public int MoveSpeed { get; set; }

        [RubyProperty("@move_frequency")]
        public int MoveFrequency { get; set; }

        [RubyProperty("@move_route")]
        public MoveRoute _MoveRoute { get; set; }

        public IMoveRoute MoveRoute => _MoveRoute;

        [RubyProperty("@walk_anime")]
        public bool WalkAnime { get; set; }

        [RubyProperty("@step_anime")]
        public bool StepAnime { get; set; }

        [RubyProperty("@direction_fix")]
        public bool DirectionFix { get; set; }

        [RubyProperty("@through")]
        public bool Through { get; set; }

        [RubyProperty("@priority_type")]
        public int PriorityType { get; set; }

        [RubyProperty("@trigger")]
        public int Trigger { get; set; }

        [RubyProperty("@list")]
        public List<Command> List { get; set; }
        
        [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
        public object _1;

        public List<NamedCommand> NamedCommands { get; set; }
        
        public IEnumerable<ICommand> Commands => List;

        public override void OnDeserialized()
        {
            base.OnDeserialized();
            NamedCommands = CommandContainerHelper.Unpack(List, StorageVersion.VXAce);
            MoveRoute.NamedCommands = CommandContainerHelper.Unpack(_MoveRoute.List, StorageVersion.VXAce);
        }

        public override void OnSerializing()
        {
            base.OnSerializing();
            List = CommandContainerHelper.Pack<Command>(NamedCommands, StorageVersion.VXAce);
            _MoveRoute.List = CommandContainerHelper.Pack<MoveCommand>(_MoveRoute.NamedCommands, StorageVersion.VXAce);
        }

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

            throw new NotImplementedException($"{options.Format} not supported");
        }
    }

    [RubyProperty("@id")]
    public int Id { get; set; }

    [RubyProperty("@name")]
    public string Name { get; set; }

    [RubyProperty("@x")]
    public int X { get; set; }

    [RubyProperty("@y")]
    public int Y { get; set; }

    [RubyProperty("@pages")]
    public List<IEventPage?> Pages { get; set; } = new();

    IList<IEventPage?> IEvent.Pages => Pages;

    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1 { get; set; }
}
