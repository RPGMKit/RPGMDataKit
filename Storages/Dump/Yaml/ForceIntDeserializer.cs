using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMDataKit.Storages.Dump.Yaml;

public class ForceIntDeserializer : INodeDeserializer
{
    public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value,
        ObjectDeserializer rootDeserializer)
    {
        if (expectedType == typeof(object) && reader.Accept<Scalar>(out var scalar))
        {
            if (scalar.Style == ScalarStyle.Plain)
            {
                if (int.TryParse(scalar.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
                {
                    value = intValue;
                    reader.MoveNext();
                    return true;
                }

                if (double.TryParse(scalar.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
                {
                    value = doubleValue;
                    reader.MoveNext();
                    return true;
                }
            }
        }

        value = null;
        return false;
    }
}
