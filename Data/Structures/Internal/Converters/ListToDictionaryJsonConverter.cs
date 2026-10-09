using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RPGMakerLibCoreV2.Data.Structures.Internal.Converters;

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

        var array = JArray.Load(reader);
        var dict = new Dictionary<int, TInterface?>();
        for (var i = 0; i < array.Count; ++i)
        {
            if (array[i].Type != JTokenType.Null)
                dict[i] = array[i].ToObject<TConcrete>(serializer);
        }

        return dict;
    }

    public override void WriteJson(JsonWriter writer, Dictionary<int, TInterface?>? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var length = value.Count == 0 ? 0 : value.Keys.Max() + 1;

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
