using RPGMDataKit.Data.Structures;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace RPGMDataKit.Storages.Dump.Yaml.MV;

/// <summary>MV keeps tables as flat arrays, so the YAML dump is a plain list of cells.</summary>
public class FlatTableConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => type == typeof(Table);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var cells = (List<int>?)rootDeserializer(typeof(List<int>));

        return Table.FromCells(cells ?? new List<int>());
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        serializer(((Table?)value)?.Elements, typeof(List<int>));
    }
}
