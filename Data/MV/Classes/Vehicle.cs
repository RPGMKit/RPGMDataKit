using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Vehicle : JsonEntity
{
    [JsonProperty("bgm")]
    public SoundData BGM { get; set; }

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
