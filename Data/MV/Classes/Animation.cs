using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Animation : JsonEntity, IAnimation
{
    public class Timing : JsonEntity
    {
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