using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class System : JsonEntity, ISystem
{
    [JsonProperty(PropertyName = "airship")]
    public Vehicle Airship { get; set; }
    
    [JsonProperty(PropertyName = "armorTypes")]
    public List<string> ArmorTypes { get; set; }
    
    [JsonProperty(PropertyName = "attackMotions")]
    public List<AttackMotion> AttackMotions { get; set; }
    
    [JsonProperty(PropertyName = "battleBgm")]
    public SoundData BattleBgm { get; set; }
    
    [JsonProperty(PropertyName = "battleback1Name")]
    public string Battleback1Name { get; set; }
    
    [JsonProperty(PropertyName = "battleback2Name")]
    public string Battleback2Name { get; set; }
    
    [JsonProperty(PropertyName = "battlerHue")]
    public int BattlerHue { get; set; }
    
    [JsonProperty(PropertyName = "battlerName")]
    public string BattlerName { get; set; }
    
    [JsonProperty(PropertyName = "boat")]
    public Vehicle Boat { get; set; }
    
    [JsonProperty(PropertyName = "currencyUnit")]
    public string CurrencyUnit { get; set; }
    
    [JsonProperty(PropertyName = "defeatMe")]
    public SoundData DefeatMe { get; set; }
    
    [JsonProperty(PropertyName = "editMapId")]
    public int EditMapId { get; set; }
    
    [JsonProperty(PropertyName = "elements")]
    public List<string> Elements { get; set; }
    
    [JsonProperty(PropertyName = "encryptionKey")]
    public string EncryptionKey { get; set; }
    
    [JsonProperty(PropertyName = "gameTitle")]
    public string GameTitle { get; set; }
    
    [JsonProperty(PropertyName = "gameoverMe")]
    public SoundData GameOverMe { get; set; }
    
    [JsonProperty(PropertyName = "hasEncryptedAudio")]
    public bool HasEncryptedAudio { get; set; }
    
    [JsonProperty(PropertyName = "hasEncryptedImages")]
    public bool HasEncryptedImages { get; set; }
    
    [JsonProperty(PropertyName = "locale")]
    public string Locale { get; set; }
    
    [JsonProperty(PropertyName = "magicSkills")]
    public List<object> MagicSkills { get; set; }

    [JsonProperty(PropertyName = "menuCommands")]
    public List<bool> MenuCommands { get; set; }
    
    [JsonProperty(PropertyName = "optDisplayTp")]
    public bool OptDisplayTp { get; set; }
    
    [JsonProperty(PropertyName = "optDrawTitle")]
    public bool OptDrawTitle { get; set; }
    
    [JsonProperty(PropertyName = "optExtraExp")]
    public bool OptExtraExp { get; set; }
    
    [JsonProperty(PropertyName = "optFloorDeath")]
    public bool OptFloorDeath { get; set; }
    
    [JsonProperty(PropertyName = "optFollowers")]
    public bool OptFollowers { get; set; }
    
    [JsonProperty(PropertyName = "optSideView")]
    public bool OptSideView { get; set; }
    
    [JsonProperty(PropertyName = "optSlipDeath")]
    public bool OptSlipDeath { get; set; }
    
    [JsonProperty(PropertyName = "optTransparent")]
    public bool OptTransparent { get; set; }
    
    [JsonProperty(PropertyName = "partyMembers")]
    public List<int> PartyMembers { get; set; }
    
    [JsonProperty(PropertyName = "ship")]
    public Vehicle Ship { get; set; }
    
    [JsonProperty(PropertyName = "skillTypes")]
    public List<string> SkillTypes { get; set; }
    
    [JsonProperty(PropertyName = "sounds")]
    public List<SoundData> Sounds { get; set; }
    
    [JsonProperty(PropertyName = "startMapId")]
    public int StartMapId { get; set; }
    
    [JsonProperty(PropertyName = "startX")]
    public int StartX { get; set; }
    
    [JsonProperty(PropertyName = "startY")]
    public int StartY { get; set; }

    [JsonProperty(PropertyName = "switches")]
    public List<string> Switches { get; set; }
    
    [JsonProperty("terms")]
    public Terms Terms { get; set; }
    
    [JsonProperty("testBattlers")]
    public List<TestBattler> TestBattlers { get; set; }
    
    [JsonProperty("testTroopId")]
    public int TestTroopId { get; set; }
    
    [JsonProperty("title1Name")]
    public string Title1Name { get; set; }
    
    [JsonProperty("title2Name")]
    public string Title2Name { get; set; }
    
    [JsonProperty("titleBgm")]
    public SoundData TitleBgm { get; set; }

    [JsonProperty(PropertyName = "equipTypes")]
    public List<string> EquipmentTypes { get; set; }

    [JsonProperty(PropertyName = "variables")]
    public List<string> Variables { get; set; }

    [JsonProperty(PropertyName = "versionId")]
    public int VersionId { get; set; }

    [JsonProperty(PropertyName = "victoryMe")]
    public SoundData VictoryMe { get; set; }

    [JsonProperty(PropertyName = "weaponTypes")]
    public List<string> WeaponTypes { get; set; }

    [JsonProperty(PropertyName = "windowTone")]
    public List<int> WindowTone { get; set; }
}
