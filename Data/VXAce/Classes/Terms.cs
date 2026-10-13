using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::System::Terms")]
public class Terms : RubySerializationEntity
{
    [RubyProperty("@params")]
    public List<string> _params;

    [RubyProperty("@etypes")]
    public List<string> etypes;

    [RubyProperty("@commands")]
    public List<string> commands;

    [RubyProperty("@basic")]
    public List<string> basic;
}
