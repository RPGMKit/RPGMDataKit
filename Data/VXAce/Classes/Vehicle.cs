using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::System::Vehicle")]
public class Vehicle : RubySerializationEntity
{
    [RubyProperty("@start_x")]
    public int StartX;

    [RubyProperty("@start_y")]
    public int StartY;

    [RubyProperty("@bgm")]
    public BGM BGM;

    [RubyProperty("@character_index")]
    public int character_index;

    [RubyProperty("@character_name")]
    public string character_name;

    [RubyProperty("@start_map_id")]
    public int start_map_id;
}
