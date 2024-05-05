using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::Actor")]
class Actor : BaseItem
{
    [RubyProperty("@face_index")]
    public int FaceIndex;

    [RubyProperty("@character_index")]
    public int CharacterIndex;

    [RubyProperty("@initial_level")]
    public int InitialLevel;

    [RubyProperty("@face_name")]
    public string FaceName;

    [RubyProperty("@class_id")]
    public int ClassId;

    [RubyProperty("@character_name")]
    public string CharacterName;

    [RubyProperty("@equips")]
    public List<int> Equips;

    [RubyProperty("@nickname")]
    public string NickName;

    [RubyProperty("@max_level")]
    public int MaxLevel;
}