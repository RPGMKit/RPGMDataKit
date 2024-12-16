using RubyMarshalCS.Entities;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public abstract class RubySerializationEntity
{
    [RubyExtensionData]
    public Dictionary<string, object?> _unknownFields = new();
    
    [RubyOnDeserialize]
    void OnDeserialization(RubyObject context)
    {
        OnDeserialize();
    }

    [RubyOnPreSerialize]
    void OnPreSerialization(RubyObject context)
    {
        OnPreSerialize();
    }

    protected virtual void OnDeserialize()
    {
    }
        
    protected virtual void OnPreSerialize()
    {
    }
}