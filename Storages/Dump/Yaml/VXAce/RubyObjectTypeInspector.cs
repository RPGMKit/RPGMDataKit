using System.Reflection;
using RubyMarshalCS.Serialization.Attributes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace RPGMakerLibCoreV2.Storages.Dump.Yaml.VXAce;

class RubyObjectTypeInspector : TypeInspectorSkeleton
{
    private readonly ITypeInspector _innerInspector;

    public RubyObjectTypeInspector(ITypeInspector innerInspector)
    {
        _innerInspector = innerInspector;
    }

    public override string GetEnumName(Type enumType, string name)
    {
        throw new NotImplementedException();
    }

    public override string GetEnumValue(object enumValue)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
    {
        var props = _innerInspector.GetProperties(type, container);

        if (type.GetCustomAttribute<RubyObjectAttribute>() != null)
            props = props.Where(_ => _.GetCustomAttribute<RubyPropertyAttribute>() != null);

        // return props;
        return props.OrderBy(_ => _.Name);
    }
}
