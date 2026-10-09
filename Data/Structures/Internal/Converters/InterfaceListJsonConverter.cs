using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RPGMakerLibCoreV2.Data.Structures.Internal.Converters;

/// <summary>Reads a JSON array into a list of interface-typed items, creating each item as <typeparamref name="TConcrete"/>.</summary>
public class InterfaceListJsonConverter<TInterface, TConcrete> : JsonConverter<List<TInterface?>>
    where TConcrete : class, TInterface
{
    public override List<TInterface?>? ReadJson(JsonReader reader, Type objectType, List<TInterface?>? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        return JArray.Load(reader)
            .Select(token => token.Type == JTokenType.Null ? default : token.ToObject<TConcrete>(serializer))
            .Cast<TInterface?>()
            .ToList();
    }

    public override void WriteJson(JsonWriter writer, List<TInterface?>? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartArray();
        foreach (var item in value)
            serializer.Serialize(writer, item, typeof(TConcrete));

        writer.WriteEndArray();
    }
}
