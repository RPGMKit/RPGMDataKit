using RubyMarshalCS.Entities;
using RubyMarshalCS.Serialization.Attributes;
using YamlDotNet.Serialization.Callbacks;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public abstract class RubySerializationEntity
{
    [RubyExtensionData]
    public Dictionary<string, object?> _unknownFields = new();
    
    public static Dictionary<Type, Dictionary<string, string>> _globalUnknownFields = new();

    [OnSerializing]
    public void OnYamlSerializing()
    {
        OnSerializing();
    }

    [OnDeserialized]
    public void OnYamlDeserialized()
    {
        OnDeserialized();
    }
    
    [RubyOnPreSerialize]
    protected void OnRubySerializing(RubyObject context)
    {
        OnSerializing();
    }
    
    [RubyOnDeserialize]
    protected void OnRubyDeserialized(RubyObject context)
    {
        OnDeserialized();
    }

    public virtual void OnDeserialized()
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
    }

    public virtual void OnSerializing()
    {
    }
}