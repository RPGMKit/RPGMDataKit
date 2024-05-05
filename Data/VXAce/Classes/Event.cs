using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Event")]
public class Event : RubySerializationEntity, IEvent
{
    [RubyObject("RPG::Event::Page")]
    public class Page : RubySerializationEntity, IPage
    {
        [RubyObject("RPG::Event::Page::Condition")]
        public class PageCondition : RubySerializationEntity
        {
            [RubyProperty("@switch1_valid")]
            public bool Switch1Valid;

            [RubyProperty("@switch2_valid")]
            public bool Switch2Valid;

            [RubyProperty("@variable_valid")]
            public bool VariableValid;

            [RubyProperty("@self_switch_valid")]
            public bool SelfSwitchValid;

            [RubyProperty("@item_valid")]
            public bool ItemValid;

            [RubyProperty("@actor_valid")]
            public bool ActorValid;

            [RubyProperty("@switch1_id")]
            public int Switch1Id;

            [RubyProperty("@switch2_id")]
            public int Switch2Id;

            [RubyProperty("@variable_id")]
            public int VariableId;

            [RubyProperty("@variable_value")]
            public int VariableValue;

            [RubyProperty("@self_switch_ch", CandidateFlags.Character)]
            public string SelfSwitchCh;

            [RubyProperty("@item_id")]
            public int ItemId;

            [RubyProperty("@actor_id")]
            public int ActorId;
            
            [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
            public object _1;
        }

        [RubyObject("RPG::Event::Page::Graphic")]
        public class PageGraphic : RubySerializationEntity
        {
            [RubyProperty("@tile_id")]
            public int TileId;
            
            [RubyProperty("@character_name")]
            public string CharacterName;
            
            [RubyProperty("@character_index")]
            public int CharacterIndex;
            
            [RubyProperty("@direction")]
            public int Direction;
            
            [RubyProperty("@pattern")]
            public int Pattern;
            
            [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
            public object _1;
        }

        [RubyProperty("@condition")]
        public PageCondition Condition;

        [RubyProperty("@graphic")]
        public PageGraphic Graphic;

        [RubyProperty("@move_type")]
        public int MoveType;

        [RubyProperty("@move_speed")]
        public int MoveSpeed;

        [RubyProperty("@move_frequency")]
        public int MoveFrequency;

        [RubyProperty("@move_route")]
        public MoveRoute MoveRoute;

        [RubyProperty("@walk_anime")]
        public bool WalkAnime;

        [RubyProperty("@step_anime")]
        public bool StepAnime;

        [RubyProperty("@direction_fix")]
        public bool DirectionFix;

        [RubyProperty("@through")]
        public bool Through;

        [RubyProperty("@priority_type")]
        public int PriorityType;

        [RubyProperty("@trigger")]
        public int Trigger;

        [RubyProperty("@list")]
        public List<EventCommand> List;
        
        [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
        public object _1;
        
        public IEnumerable<NamedCommand> NamedCommands => List.Select(VXAceCommandFactory.Create);
        public IEnumerable<IEventCommand> Commands => List;
        public string Dump(Storage storage, DumpOptions options)
        {
            throw new NotImplementedException();
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
    public List<Page> _Pages { get; set; }

    public IEnumerable<IPage> Pages => _Pages;

    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1 { get; set; }

    public string Dump(Storage storage, DumpOptions options)
    {
        throw new NotImplementedException();
    }
}
