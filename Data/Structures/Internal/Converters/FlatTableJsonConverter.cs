using Newtonsoft.Json;

namespace RPGMakerLibCoreV2.Data.Structures.Internal.Converters;

/// <summary>
/// Reads a flat JSON array as a one-dimensional <see cref="Table"/> and writes the cells back as a flat array.
/// The owner reshapes the table once its own dimensions are known.
/// </summary>
public class FlatTableJsonConverter : JsonConverter<Table>
{
    public override Table? ReadJson(JsonReader reader, Type objectType, Table? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        return Table.FromCells(serializer.Deserialize<List<int>>(reader) ?? new List<int>());
    }

    public override void WriteJson(JsonWriter writer, Table? value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
            serializer.Serialize(writer, value.Elements);
    }
}
