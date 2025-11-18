using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Commands;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace RPGMakerLibCoreV2.Storages.Dump.MV;

class MvObjectTypeInspector : TypeInspectorSkeleton
{
    private readonly ITypeInspector _innerInspector;

    public MvObjectTypeInspector(ITypeInspector innerInspector)
    {
        _innerInspector = innerInspector;
    }

    public override string GetEnumName(Type enumType, string name)
    {
        throw new NotImplementedException();
    }

    public override string GetEnumValue(object enumValue)
    {
        var t = enumValue.GetType();

        return Enum.Format(t, enumValue, "D");
    }

    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
    {
        var props = _innerInspector.GetProperties(type, container);

        props = props.Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null || p.GetCustomAttribute<JsonExtensionDataAttribute>() != null);

        return props.OrderBy(p => p.Name);
    }
}
