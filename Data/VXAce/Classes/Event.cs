using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Event")]
public class Event : RubySerializationEntity, IEvent, INewEntity
{
    [RubyObject("RPG::Event::Page")]
    public class EventPage : VxCommandContainer, IEventPage, INewEntity
    {
        /// <summary>The values of <c>RPG::Event::Page.new</c> of VX Ace: no condition, no picture, the command that ends the page.</summary>
        public void InitializeNew()
        {
            var condition = new EventPageCondition();
            condition.InitializeNew();
            Condition = condition;
            var graphic = new PageGraphic();
            graphic.InitializeNew();
            Graphic = graphic;
            MoveType = 0;
            MoveSpeed = 3;
            MoveFrequency = 3;
            var route = new MoveRoute();
            route.InitializeNew();
            MoveRoute = route;
            WalkAnime = true;
            StepAnime = false;
            DirectionFix = false;
            Through = false;
            PriorityType = 0;
            Trigger = 0;
            NamedCommands = [CommandContainerHelper.CreateNamed(new Command { Code = 0, Indent = 0, Parameters = [] }, StorageVersion.VXAce)];
        }

        [RubyObject("RPG::Event::Page::Condition")]
        public class EventPageCondition : RubySerializationEntity, IEventPageCondition, INewEntity
        {
            /// <summary>The values of <c>RPG::Event::Page::Condition.new</c> of VX Ace: nothing is required.</summary>
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
        public class PageGraphic : RubySerializationEntity, IPageGraphic, INewEntity
        {
            /// <summary>The values of <c>RPG::Event::Page::Graphic.new</c> of VX Ace: no picture, facing down.</summary>
            public void InitializeNew()
            {
                TileId = 0;
                CharacterName = "";
                CharacterIndex = 0;
                Direction = 2;
                Pattern = 0;
            }

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
        public IMoveRoute MoveRoute { get; set; }

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

        [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
        public object _1;

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

    /// <summary>The values of <c>RPG::Event.new</c> of VX Ace: one empty page; the place and the number are for the map to set.</summary>
    public void InitializeNew()
    {
        Name = "";
        var page = new EventPage();
        page.InitializeNew();
        Pages = [page];
    }

    [RubyProperty("@pages")]
    public List<IEventPage?> Pages { get; set; } = new();

    IList<IEventPage?> IEvent.Pages => Pages;

    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1 { get; set; }
}
