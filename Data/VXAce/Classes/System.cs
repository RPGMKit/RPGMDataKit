using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization.Attributes;
using RPGMDataKit.Data.Structures.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::System")]
public class System : RubySerializationEntity, ISystem
{
    [RubyProperty("@_")]
    public int Unk_ { get; set; }

    [RubyProperty("@elements")]
    public List<string> Elements { get; set; }

    [RubyProperty("@start_x")]
    public int StartX { get; set; }

    [RubyProperty("@start_y")]
    public int StartY { get; set; }

    [RubyProperty("@airship")]
    public Vehicle Airship { get; set; }

    [RubyProperty("@test_battlers")]
    public List<TestBattler> Battlers { get; set; }

    [RubyProperty("@edit_map_id")]
    public int EditMapId { get; set; }

    [RubyProperty("@battle_end_me")]
    public ME BattleEndME { get; set; }

    [RubyProperty("@party_members")]
    public List<int> PartyMembers { get; set; }

    [RubyProperty("@ship")]
    public Vehicle Ship { get; set; }

    [RubyProperty("@battler_hue")]
    public int BattlerHue { get; set; }

    [RubyProperty("@sounds")]
    public List<ISE?> Sounds { get; set; } = new();

    [RubyProperty("@variables")]
    public List<string> Variables { get; set; }

    [RubyProperty("@battle_bgm")]
    public BGM BattleBGM { get; set; }

    [RubyProperty("@version_id")]
    public int VersionId { get; set; }
    
    [RubyProperty("@start_map_id")]
    public int StartMapId { get; set; }
    
    [RubyProperty("@battler_name")]
    public string BattlerName { get; set; }
    
    [RubyProperty("@magic_number")]
    public int MagicNumber { get; set; }

    [RubyProperty("@switches")]
    public List<string> Switches { get; set; }

    [RubyProperty("@gameover_me")]
    public ME GameoverME { get; set; }

    [RubyProperty("@game_title")]
    public string GameTitle { get; set; }

    [RubyProperty("@title_bgm")]
    public BGM TitleBGM { get; set; }

    [RubyProperty("@terms")]
    public Terms Terms { get; set; }

    [RubyProperty("@test_troop_id")]
    public int TestTroopId { get; set; }

    [RubyProperty("@boat")]
    public Vehicle Boat { get; set; }

    [RubyProperty("@skill_types")]
    public List<string> SkillTypes { get; set; }
    
    [RubyProperty("@weapon_types")]
    public List<string> WeaponTypes { get; set; }
    
    [RubyProperty("@armor_types")]
    public List<string> ArmorTypes { get; set; }

    [RubyProperty("@currency_unit")]
    public string CurrencyUnit { get; set; }

    [RubyProperty("@window_tone")]
    public Tone _WindowTone { get; set; }

    [RubyProperty("@japanese")]
    public bool Japanese { get; set; }
    
    [RubyProperty("@opt_draw_title")]
    public bool OptDrawTitle { get; set; }
    
    [RubyProperty("@opt_use_midi")]
    public bool OptUseMidi { get; set; }
    
    [RubyProperty("@opt_transparent")]
    public bool OptTransparent { get; set; }
    
    [RubyProperty("@opt_followers")]
    public bool OptFollowers { get; set; }
    
    [RubyProperty("@opt_slip_death")]
    public bool OptSlipDeath { get; set; }
    
    [RubyProperty("@opt_extra_exp")]
    public bool OptExtraExp { get; set; }
    
    [RubyProperty("@opt_display_tp")]
    public bool OptDisplayTP { get; set; }
    
    [RubyProperty("@battleback1_name")]
    public string Battleback1Name { get; set; }
    
    [RubyProperty("@battleback2_name")]
    public string Battleback2Name { get; set; }
    
    [RubyProperty("@title1_name")]
    public string Title1Name { get; set; }
    
    [RubyProperty("@title2_name")]
    public string Title2Name { get; set; }
    
    [RubyProperty("@opt_floor_death")]
    public bool OptFloorDeath { get; set; }

    IList<ISE?> ISystem.Sounds => Sounds;
    
    [DoesNotExistInVersion]
    public List<string> EquipmentTypes => throw new DoesNotExistInVersionException($"{nameof(EquipmentTypes)}");

    public ITone WindowTone
    {
        get => _WindowTone;
        set => _WindowTone = new Tone() { Red = value.Red, Green = value.Green, Blue = value.Blue, Gray = value.Gray };
    }
}
