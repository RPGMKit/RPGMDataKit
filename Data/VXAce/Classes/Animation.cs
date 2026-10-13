using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures;
using RPGMDataKit.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Animation")]
public class Animation : RubySerializationEntity, IAnimation, INewEntity
{
    /// <summary>The values of <c>RPG::Animation.new</c> of VX Ace: no pictures, one empty frame, no timings, shown on the middle of the target.</summary>
    public void InitializeNew()
    {
        Name = "";
        Animation1Name = "";
        Animation1Hue = 0;
        Animation2Name = "";
        Animation2Hue = 0;
        Position = 1;
        FrameMax = 1;
        var frame = new Frame();
        frame.InitializeNew();
        Frames = [frame];
        Timings = new();
    }

    [RubyObject("RPG::Animation::Frame")]
    public class Frame : RubySerializationEntity, INewEntity
    {
        /// <summary>The values of <c>RPG::Animation::Frame.new</c> of VX Ace: no cells, a table of 8 numbers for each (none yet).</summary>
        public void InitializeNew()
        {
            CellMax = 0;
            CellData = new Table(0, 8);
        }

        [RubyProperty("@cell_max")]
        public int CellMax;
        
        [RubyProperty("@cell_data")]
        public Table CellData;
    }

    [RubyObject("RPG::Animation::Timing")]
    public class Timing : RubySerializationEntity, INewEntity
    {
        /// <summary>The values of <c>RPG::Animation::Timing.new</c> of VX Ace: at the first frame, no sound, the white flash of 5 frames of nothing.</summary>
        public void InitializeNew()
        {
            Frame = 0;
            SE = new SE();
            SE.InitializeNew();
            SE.Volume = 80;
            Condition = 0;
            FlashScope = 0;
            FlashColor = new Color { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
            FlashDuration = 5;
        }

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
