using RubyMarshalCS.Entities;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public abstract class RubySerializationEntity
{
    [RubyExtensionData]
    public Dictionary<string, object?> _unknownFields = new();
    
    public static Dictionary<Type, Dictionary<string, string>> _globalUnknownFields = new();
    
    [RubyOnDeserialize]
    protected void OnDeserialization(RubyObject context)
    {
        if (_unknownFields != null)
        {
            foreach (var (key, _) in _unknownFields)
            {
                if (!_globalUnknownFields.ContainsKey(GetType()))
                    _globalUnknownFields[GetType()] = new();

                _globalUnknownFields[GetType()][key] = key;
            }
        }
        
        OnDeserialize();
    }

    [RubyOnPreSerialize]
    protected void OnPreSerialization(RubyObject context)
    {
        OnPreSerialize();
    }

    public virtual void OnDeserialize()
    {
    }

    public virtual void OnPreSerialize()
    {
    }
}