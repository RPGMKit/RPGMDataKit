using Newtonsoft.Json;

namespace RPGMDataKit.Data.Structures.Internal.Converters;

/// <summary>
/// Reads a JSON array as a dictionary keyed by index (null slots are skipped) and writes it back as an array.
/// </summary>
public class ListToDictionaryJsonConverter<TInterface, TConcrete> : JsonConverter<Dictionary<int, TInterface?>>
    where TConcrete : class, TInterface
{
    public override Dictionary<int, TInterface?>? ReadJson(JsonReader reader, Type objectType,
        Dictionary<int, TInterface?>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        if (reader.TokenType != JsonToken.StartArray)
            throw new JsonSerializationException($"Expected an array, got {reader.TokenType}");

        var dict = new SlotDictionary<TInterface?>();
        var length = 0;
        for (; reader.Read() && reader.TokenType != JsonToken.EndArray; ++length)
        {
            if (reader.TokenType != JsonToken.Null)
                dict[length] = serializer.Deserialize<TConcrete>(reader);
        }

        dict.Slots = length;
        return dict;
    }

    public override void WriteJson(JsonWriter writer, Dictionary<int, TInterface?>? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        // the array keeps the length it had in the file, so the trailing nulls are not lost
        var length = Math.Max(value.Count == 0 ? 0 : value.Keys.Max() + 1, (value as SlotDictionary<TInterface?>)?.Slots ?? 0);

        writer.WriteStartArray();
        for (var i = 0; i < length; ++i)
        {
            if (value.TryGetValue(i, out var item) && item != null)
                serializer.Serialize(writer, item, typeof(TConcrete));
            else
                writer.WriteNull();
        }

        writer.WriteEndArray();
    }
}
