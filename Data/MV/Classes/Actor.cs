using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Actor : JsonEntity, IActor
{
    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }
    
    [JsonProperty("battlerName")]
    public string BattlerName { get; set; }
    
    [JsonProperty("characterIndex")]
    public int CharacterIndex { get; set; }
    
    [JsonProperty("characterName")]
    public string CharacterName { get; set; }
    
    [JsonProperty("classId")]
    public int ClassId { get; set; }
    
    [JsonProperty("equips")]
    public List<int> Equips { get; set; }
    
    [JsonProperty("faceIndex")]
    public int FaceIndex { get; set; }
    
    [JsonProperty("faceName")]
    public string FaceName { get; set; }
    
    [JsonProperty("traits")]
    public List<Trait> Traits { get; set; }
    
    [JsonProperty("initialLevel")]
    public int InitialLevel { get; set; }
    
    [JsonProperty("maxLevel")]
    public int MaxLevel { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("nickname")]
    public string NickName { get; set; }
    
    [JsonProperty("note")]
    public string Note { get; set; }
    
    [JsonProperty("profile")]
    public string Profile { get; set; }
}
