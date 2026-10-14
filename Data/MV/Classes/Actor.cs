using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Actor : BaseItem, IActor
{
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
    
    [JsonProperty("nickname")]
    public string NickName { get; set; }

    [JsonProperty("profile")]
    public string Profile { get; set; }

    IReadOnlyList<IFeature> IHasFeatures.Features => Traits;

    /// <summary>The values that the editor of MV gives to a new actor.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        BattlerName = "";
        CharacterIndex = 0;
        CharacterName = "";
        ClassId = 1;
        Equips = [0, 0, 0, 0, 0];
        FaceIndex = 0;
        FaceName = "";
        Traits = new();
        InitialLevel = 1;
        MaxLevel = 99;
        NickName = "";
        Profile = "";
    }
}
