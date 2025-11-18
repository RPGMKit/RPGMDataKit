using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Map : JsonEntity, IMap
{
    [JsonProperty(PropertyName = "autoplayBgm")]
    public bool AutoPlayBgm { get; set; }
    
    [JsonProperty(PropertyName = "autoplayBgs")]
    public bool AutoPlayBgs { get; set; }
    
    [JsonProperty(PropertyName = "battleback1Name")]
    public string Battleback1Name { get; set; }
    
    [JsonProperty(PropertyName = "battleback2Name")]
    public string Battleback2Name { get; set; }
    
    [JsonProperty(PropertyName = "bgm")]
    public SoundData Bgm { get; set; }
    
    [JsonProperty(PropertyName = "bgs")]
    public SoundData Bgs { get; set; }
    
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
    public List<object> EncounterList { get; set; }
    
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

    // ground 1 => ground 2? => bottom object => top object => shadow flags => region id
    [JsonProperty(PropertyName = "data")]
    public LookupList<int> _Data { get; set; }

    [JsonProperty(PropertyName = "events")]
    public LookupList<Event> _Events { get; set; }

    public IDataLookup<int> Data => _Data;

    public IByIdGetter<IEvent> Events => _Events;
}
