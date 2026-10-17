using RubyMarshalCS.Entities;
using RubyMarshalCS.Serialization.Attributes;
using RPGMDataKit.Data.Structures.Interfaces;
using YamlDotNet.Serialization.Callbacks;

namespace RPGMDataKit.Data.Structures.Internal;

public abstract class RubySerializationEntity : IHasOptionalProperties
{
    public bool HasProperty(string propertyName) => OptionalProperties.Has(GetType(), propertyName);

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
    
    [RubyOnSerializing]
    protected void OnRubySerializing(RubyObject obj)
    {
        OnSerializing();
    }
    
    [RubyOnDeserialized]
    protected void OnRubyDeserialized(RubyObject obj)
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