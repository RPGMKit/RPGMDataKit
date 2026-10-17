using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;
using RPGMDataKit.Data.Structures.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::Actor")]
public class Actor : BaseItem, IActor
{
    [RubyProperty("@face_index")]
    public int FaceIndex { get; set; }

    [RubyProperty("@character_index")]
    public int CharacterIndex { get; set; }

    [RubyProperty("@initial_level")]
    public int InitialLevel { get; set; }

    [RubyProperty("@face_name")]
    public string FaceName { get; set; }

    [RubyProperty("@class_id")]
    public int ClassId { get; set; }

    [RubyProperty("@character_name")]
    public string CharacterName { get; set; }

    [RubyProperty("@equips")]
    public List<int> Equips { get; set; }

    [RubyProperty("@nickname")]
    public string NickName { get; set; }

    [RubyProperty("@max_level")]
    public int MaxLevel { get; set; }

    /// <summary>The values of <c>RPG::Actor.new</c> of VX Ace.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        NickName = "";
        ClassId = 1;
        InitialLevel = 1;
        MaxLevel = 99;
        CharacterName = "";
        CharacterIndex = 0;
        FaceName = "";
        FaceIndex = 0;
        Equips = [0, 0, 0, 0, 0];
    }

    // there is no side view battle in VX Ace; no [RubyProperty], so the serializer does not touch it
    [DoesNotExistInVersion]
    public string BattlerName
    {
        get => throw new DoesNotExistInVersionException();
        set => throw new DoesNotExistInVersionException();
    }
}