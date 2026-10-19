using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures.Internal.Converters;

namespace RPGMDataKit.Data.MV.Classes;

public class Map : JsonEntity, IMap, INewEntity
{
    /// <summary>The values that the editor of MV gives to a new map: an empty map of 17 x 13 tiles with the first tileset.</summary>
    public void InitializeNew()
    {
        AutoPlayBgm = false;
        AutoPlayBgs = false;
        Battleback1Name = "";
        Battleback2Name = "";
        Bgm = new BGM();
        Bgm.InitializeNew();
        Bgs = new BGS();
        Bgs.InitializeNew();
        TilesetId = 1;
        DisplayName = "";
        Width = 17;
        Height = 13;
        DisableDashing = false;
        EncounterList = new();
        EncounterStep = 30;
        Note = "";
        ParallaxLoopX = false;
        ParallaxLoopY = false;
        ParallaxName = "";
        ParallaxShow = true; // unlike VX Ace, the editor of MV shows the parallax picture of a new map
        ParallaxSX = 0;
        ParallaxSY = 0;
        ScrollType = MapScrollType.NoLoop;
        SpecifyBattleback = false;

        // 6 layers: two of the ground, two of the upper tiles, the shadows, the regions
        var data = Table.FromCells(new List<int>(new int[Width * Height * 6]));
        data.Reshape(Width, Height, 6);
        Data = data;
        Events = new();
    }

    /// <summary>A troop that appears on the map in a random encounter.</summary>
    public class Encounter : JsonEntity, INewEntity
    {
        /// <summary>What the editor of MV gives to a new encounter: the first troop, the weight 5, on the whole map.</summary>
        public void InitializeNew()
        {
            TroopId = 1;
            Weight = 5;
            RegionSet = new();
        }

        [JsonProperty("regionSet")]
        public List<int> RegionSet { get; set; } = new();
        [JsonProperty("troopId")]
        public int TroopId { get; set; }
        [JsonProperty("weight")]
        public int Weight { get; set; }
    }

    [JsonProperty(PropertyName = "autoplayBgm")]
    public bool AutoPlayBgm { get; set; }

    [JsonProperty(PropertyName = "autoplayBgs")]
    public bool AutoPlayBgs { get; set; }

    [JsonProperty(PropertyName = "battleback1Name")]
    public string Battleback1Name { get; set; }

    [JsonProperty(PropertyName = "battleback2Name")]
    public string Battleback2Name { get; set; }

    [JsonProperty(PropertyName = "bgm")]
    public BGM Bgm { get; set; }

    [JsonProperty(PropertyName = "bgs")]
    public BGS Bgs { get; set; }

    [JsonProperty(PropertyName = "tilesetId")]
    public int TilesetId { get; set; }

    [JsonProperty(PropertyName = "displayName")]
    public string DisplayName { get; set; }

    [JsonProperty(PropertyName = "height")]
    public int Height { get; set; }

    [JsonProperty(PropertyName = "width")]
    public int Width { get; set; }

    [JsonProperty(PropertyName = "disableDashing")]
    public bool DisableDashing { get; set; }

    [JsonProperty(PropertyName = "encounterList")]
    public List<Encounter> EncounterList { get; set; } = new();

    [JsonProperty(PropertyName = "encounterStep")]
    public int EncounterStep { get; set; }

    [JsonProperty(PropertyName = "note")]
    public string Note { get; set; }

    [JsonProperty(PropertyName = "parallaxLoopX")]
    public bool ParallaxLoopX { get; set; }

    [JsonProperty(PropertyName = "parallaxLoopY")]
    public bool ParallaxLoopY { get; set; }

    [JsonProperty(PropertyName = "parallaxName")]
    public string ParallaxName { get; set; }

    [JsonProperty(PropertyName = "parallaxShow")]
    public bool ParallaxShow { get; set; }

    [JsonProperty(PropertyName = "parallaxSx")]
    public int ParallaxSX { get; set; }

    [JsonProperty(PropertyName = "parallaxSy")]
    public int ParallaxSY { get; set; }

    [JsonProperty(PropertyName = "scrollType")]
    public int ScrollType { get; set; }

    [JsonProperty(PropertyName = "specifyBattleback")]
    public bool SpecifyBattleback { get; set; }

    // 6 layers: lower layer 1, lower layer 2, upper layer 1, upper layer 2, the shadows (the low four bits), the regions
    [JsonProperty(PropertyName = "data")]
    [JsonConverter(typeof(FlatTableJsonConverter))]
    public Table Data { get; set; } = new();

    [JsonProperty(PropertyName = "events")]
    [JsonConverter(typeof(ListToDictionaryJsonConverter<IEvent, Event>))]
    public Dictionary<int, IEvent?> Events { get; set; } = new();

    IDictionary<int, IEvent?> IMap.Events => Events;

    public override void OnDeserialized()
    {
        base.OnDeserialized();

        var cells = Width * Height;
        Data.Reshape(Width, Height, cells > 0 ? Data.Count / cells : 0);
    }
}
