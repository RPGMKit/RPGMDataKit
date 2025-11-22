using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;
using RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Map")]
public class Map : RubySerializationEntity, IMap
{
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
    public List<object> EncounterList { get; set; }

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
    public int ParallaxXY { get; set; }

    [RubyProperty("@parallax_show")]
    public bool ParallaxShow { get; set; }

    [RubyProperty("@note")]
    public string Note { get; set; }

    [RubyProperty("@data")]
    public Table _Data { get; set; }

    public IDataLookup<int> Data => new ListWrapper<int, short>(_Data.Elements, i => (short)i, s => (int)s);

    [RubyProperty("@events")]
    public LookupMap<Event?> _Events { get; set; }

    public IByIdGetter<IEvent?> Events => _Events;

    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1 { get; set; }
}
