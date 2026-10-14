using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Animation : JsonEntity, IAnimation, INewEntity
{
    /// <summary>The values that the editor of MV gives to a new animation: no pictures, one empty frame, no timings, shown on the middle of the target.</summary>
    public void InitializeNew()
    {
        Name = "";
        Animation1Name = "";
        Animation1Hue = 0;
        Animation2Name = "";
        Animation2Hue = 0;
        Position = AnimationPosition.Center;
        Frames = [new List<object>()];
        Timings = new();
    }

    public class Timing : JsonEntity, INewEntity
    {
        /// <summary>What the editor of MV gives to a new timing: at the first frame, no sound (null), the white flash.</summary>
        public void InitializeNew()
        {
            Frame = 0;
            SE = null!;
            Condition = 0;
            FlashScope = Enums.FlashScope.None;
            FlashColor = Color.FromArray([255, 255, 255, 255]);
            FlashDuration = 5;
        }

        [JsonProperty("frame")]
        public int Frame;

        [JsonProperty("se")]
        public SE SE;
        
        [JsonProperty("conditions")]
        public int Condition;
        
        [JsonProperty("flashScope")]
        public int FlashScope;

        [JsonProperty("flashColor")]
        public Color FlashColor;
        
        [JsonProperty("flashDuration")]
        public int FlashDuration;
    }
    
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("animation1Name")]
    public string Animation1Name { get; set; }
    
    [JsonProperty("animation1Hue")]
    public int Animation1Hue { get; set; }
    
    [JsonProperty("animation2Name")]
    public string Animation2Name { get; set; }
    
    [JsonProperty("animation2Hue")]
    public int Animation2Hue { get; set; }
    
    [JsonProperty("position")]
    public int Position { get; set; }

    [JsonProperty("frames")]
    public List<object> Frames { get; set; }
    
    [JsonProperty("timings")]
    public List<Timing> Timings { get; set; }
}