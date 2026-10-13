using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMDataKit.Storages.Dump.Yaml;

/// <summary>Deserializes an untagged node declared as an entity interface into the concrete type from <see cref="TypeMap"/>.</summary>
public class TypeMapNodeTypeResolver : INodeTypeResolver
{
    private readonly TypeMap _types;

    public TypeMapNodeTypeResolver(TypeMap types)
    {
        _types = types;
    }

    public bool Resolve(NodeEvent? nodeEvent, ref Type currentType)
    {
        if (!currentType.IsInterface || nodeEvent is { Tag.IsEmpty: false })
            return false;

        var concrete = _types.Resolve(currentType);
        if (concrete == currentType)
            return false;

        currentType = concrete;
        return true;
    }
}
