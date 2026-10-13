using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures;
using RPGMDataKit.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Animation")]
public class Animation : RubySerializationEntity, IAnimation
{
    [RubyObject("RPG::Animation::Frame")]
    public class Frame : RubySerializationEntity
    {
        [RubyProperty("@cell_max")]
        public int CellMax;
        
        [RubyProperty("@cell_data")]
        public Table CellData;
    }

    [RubyObject("RPG::Animation::Timing")]
    public class Timing : RubySerializationEntity
    {
        [RubyProperty("@frame")]
        public int Frame;

        [RubyProperty("@se")]
        public SE SE;
        
        [RubyProperty("@condition")]
        public int Condition;
        
        [RubyProperty("@flash_scope")]
        public int FlashScope;

        [RubyProperty("@flash_color")]
        public Color FlashColor;
        
        [RubyProperty("@flash_duration")]
        public int FlashDuration;
    }

    [RubyProperty("@id")]
    public int Id { get; set; }
    
    [RubyProperty("@name")]
    public string Name { get; set; }
    
    [RubyProperty("@animation1_name")]
    public string Animation1Name { get; set; }
    
    [RubyProperty("@animation1_hue")]
    public int Animation1Hue { get; set; }
    
    [RubyProperty("@animation2_name")]
    public string Animation2Name { get; set; }
    
    [RubyProperty("@animation2_hue")]
    public int Animation2Hue { get; set; }
    
    [RubyProperty("@position")]
    public int Position { get; set; }

    [RubyProperty("@frame_max")]
    public int FrameMax { get; set; }

    [RubyProperty("@frames")]
    public List<Frame> Frames { get; set; }
    
    [RubyProperty("@timings")]
    public List<Timing> Timings { get; set; }
}
