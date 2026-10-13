using RPGMDataKit.Data.Structures.Internal.Converters;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMDataKit.Storages.Dump.Yaml.MV;

/// <summary>
/// A dictionary keyed by index (the events of a map) is written as the list it is in the data file, with an empty
/// item for every missing index, and is read back from that list.
/// </summary>
public class IndexDictionaryConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        // Dictionary<int, T> itself or a class made from it (the dictionary read from a data file)
        for (var t = type; t != null; t = t.BaseType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return t.GetGenericArguments()[0] == typeof(int);
        }

        return false;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value == null)
        {
            serializer(null);
            return;
        }

        var dictionary = (System.Collections.IDictionary)value;
        var length = 0;
        foreach (int key in dictionary.Keys)
            length = Math.Max(length, key + 1);

        length = Math.Max(length, (int)(value.GetType().GetProperty(nameof(SlotDictionary<object>.Slots))?.GetValue(value) ?? 0));

        emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Any));
        for (var i = 0; i < length; ++i)
            serializer(dictionary.Contains(i) ? dictionary[i] : null);

        emitter.Emit(new SequenceEnd());
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer deserializer)
    {
        var dictionaryType = type;
        while (!dictionaryType.IsGenericType || dictionaryType.GetGenericTypeDefinition() != typeof(Dictionary<,>))
            dictionaryType = dictionaryType.BaseType!;

        var valueType = dictionaryType.GetGenericArguments()[1];
        var dictionary = (System.Collections.IDictionary)Activator.CreateInstance(typeof(SlotDictionary<>).MakeGenericType(valueType))!;

        parser.Consume<SequenceStart>();
        var index = 0;
        while (parser.Current is not SequenceEnd)
        {
            if (parser.Current is Scalar { Value: null or "" or "null" or "~" })
                parser.Consume<Scalar>();
            else
                dictionary[index] = deserializer(valueType);

            ++index;
        }

        parser.Consume<SequenceEnd>();
        dictionary.GetType().GetProperty(nameof(SlotDictionary<object>.Slots))!.SetValue(dictionary, index);

        return dictionary;
    }
}
