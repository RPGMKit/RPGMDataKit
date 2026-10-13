using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Map")]
public class Map : RubySerializationEntity, IMap, INewEntity
{
    /// <summary>The values of <c>RPG::Map.new(17, 13)</c> of VX Ace: an empty map of 17 x 13 tiles with the first tileset.</summary>
    public void InitializeNew()
    {
        DisplayName = "";
        TilesetId = 1;
        Width = 17;
        Height = 13;
        ScrollType = MapScrollType.NoLoop;
        SpecifyBattleback = false;
        Battleback1Name = "";
        Battleback2Name = "";
        AutoplayBGM = false;
        BGM = new BGM();
        BGM.InitializeNew();
        AutoplayBGS = false;
        BGS = new BGS();
        BGS.InitializeNew();
        BGS.Volume = 80;
        DisableDashing = false;
        EncounterList = new();
        EncounterStep = 30;
        ParallaxName = "";
        ParallaxLoopX = false;
        ParallaxLoopY = false;
        ParallaxSX = 0;
        ParallaxSY = 0;
        ParallaxShow = false;
        Note = "";
        Data = new Table(Width, Height, 4);
        Events = new();
    }

    /// <summary>A troop that appears on the map in a random encounter.</summary>
    [RubyObject("RPG::Map::Encounter")]
    public class Encounter : RubySerializationEntity
    {
        [RubyProperty("@troop_id")]
        public int TroopId { get; set; }
        [RubyProperty("@weight")]
        public int Weight { get; set; }
        [RubyProperty("@region_set")]
        public List<int> RegionSet { get; set; } = new();
    }

    [RubyProperty("@display_name")]
    public string DisplayName { get; set; }

    [RubyProperty("@tileset_id")]
    public int TilesetId { get; set; }

    [RubyProperty("@width")]
    public int Width { get; set; }

    [RubyProperty("@height")]
    public int Height { get; set; }

    [RubyProperty("@scroll_type")]
    public int ScrollType { get; set; }

    [RubyProperty("@specify_battleback")]
    public bool SpecifyBattleback { get; set; }

    [RubyProperty("@battleback1_name")]
    public string Battleback1Name { get; set; }

    [RubyProperty("@battleback2_name")]
    public string Battleback2Name { get; set; }

    [RubyProperty("@autoplay_bgm")]
    public bool AutoplayBGM { get; set; }

    [RubyProperty("@bgm")]
    public BGM BGM { get; set; }

    [RubyProperty("@autoplay_bgs")]
    public bool AutoplayBGS { get; set; }

    [RubyProperty("@bgs")]
    public BGS BGS { get; set; }

    [RubyProperty("@disable_dashing")]
    public bool DisableDashing { get; set; }

    [RubyProperty("@encounter_list")]
    public List<Encounter> EncounterList { get; set; } = new();

    [RubyProperty("@encounter_step")]
    public int EncounterStep { get; set; }

    [RubyProperty("@parallax_name")]
    public string ParallaxName { get; set; }

    [RubyProperty("@parallax_loop_x")]
    public bool ParallaxLoopX { get; set; }

    [RubyProperty("@parallax_loop_y")]
    public bool ParallaxLoopY { get; set; }

    [RubyProperty("@parallax_sx")]
    public int ParallaxSX { get; set; }

    [RubyProperty("@parallax_sy")]
    public int ParallaxSY { get; set; }

    [RubyProperty("@parallax_show")]
    public bool ParallaxShow { get; set; }

    [RubyProperty("@note")]
    public string Note { get; set; }

    [RubyProperty("@data")]
    public Table Data { get; set; } = new();

    [RubyProperty("@events")]
    public Dictionary<int, IEvent?> Events { get; set; } = new();

    IDictionary<int, IEvent?> IMap.Events => Events;

    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1 { get; set; }
}
