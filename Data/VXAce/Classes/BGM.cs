using RPGMDataKit.Data.Structures.Interfaces;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::BGM")]
public class BGM : AudioFile, IBGM
{
}
