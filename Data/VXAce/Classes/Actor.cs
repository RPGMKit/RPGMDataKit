using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

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
}