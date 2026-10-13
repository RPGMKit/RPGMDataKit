using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Vehicle : JsonEntity
{
    [JsonProperty("bgm")]
    public BGM BGM { get; set; }

    [JsonProperty("characterIndex")]
    public int CharacterIndex { get; set; }

    [JsonProperty("characterName")]
    public string CharacterName { get; set; }

    [JsonProperty("startMapId")]
    public int StartMapId { get; set; }

    [JsonProperty("startX")]
    public int StartX { get; set; }

    [JsonProperty("startY")]
    public int StartY { get; set; }
}
