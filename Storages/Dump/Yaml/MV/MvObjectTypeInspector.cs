using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace RPGMDataKit.Storages.Dump.Yaml.MV;

class MvObjectTypeInspector : TypeInspectorSkeleton
{
    private readonly ITypeInspector _innerInspector;

    public MvObjectTypeInspector(ITypeInspector innerInspector)
    {
        _innerInspector = innerInspector;
    }

    // an enum of an entity is written as the number of the file (the data is the same as it was when the property was an int), and read from it
    public override string GetEnumName(Type enumType, string name) => Enum.Format(enumType, Enum.Parse(enumType, name), "D");

    public override string GetEnumValue(object enumValue) => Enum.Format(enumValue.GetType(), enumValue, "D");

    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
    {
        var props = _innerInspector.GetProperties(type, container);

        props = props.Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null || p.GetCustomAttribute<JsonExtensionDataAttribute>() != null);

        return props.OrderBy(p => p.Name);
    }
}


