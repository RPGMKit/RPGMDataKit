using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::System::Terms")]
class Terms : RubySerializationEntity
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
