using Newtonsoft.Json.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump.MV;

public class JArrayConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(JArray);
    }

    private static Scalar EmptyScalar = new Scalar(null, null, "", ScalarStyle.ForcePlain, true, false);

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value == null)
        {
            emitter.Emit(EmptyScalar);
            return;
        }

        if (value is not JArray jArray)
        {
            throw new ArgumentException("Expected JArray, but got " + value.GetType().FullName);
        }

        emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Any));

        foreach (var item in jArray)
            SerializeJToken(emitter, item, serializer);

        emitter.Emit(new SequenceEnd());
    }

    private void SerializeJToken(IEmitter emitter, JToken? token, ObjectSerializer serializer)
    {
        if (token == null)
        {
            emitter.Emit(EmptyScalar);
            return;
        }

        switch (token.Type)
        {
            case JTokenType.Null:
                emitter.Emit(EmptyScalar);
                break;
            case JTokenType.String:
                emitter.Emit(new Scalar(token.ToString()));
                break;
            case JTokenType.Integer:
            case JTokenType.Float:
            case JTokenType.Boolean:
            case JTokenType.Date:
                emitter.Emit(new Scalar(token.ToString())); // Convert numbers and booleans to string
                break;
            case JTokenType.Object:
                SerializeJObject(emitter, (JObject)token, serializer);
                break;
            case JTokenType.Array:
                WriteYaml(emitter, (JArray)token, typeof(JArray), serializer); // Recursive call
                break;
            default:
                // Handle other JTokenType values as needed.  For example:
                // JTokenType.Raw, JTokenType.Bytes, JTokenType.Guid, etc.
                // You might want to throw an exception if you encounter a type you don't support.
                emitter.Emit(new Scalar(token.ToString())); // Fallback: output as string.  Adjust as necessary
                break;
        }
    }

    private void SerializeJObject(IEmitter emitter, JObject jObject, ObjectSerializer serializer)
    {
        emitter.Emit(new MappingStart(null, null, false, MappingStyle.Any));

        foreach (var property in jObject.Properties())
        {
            emitter.Emit(new Scalar(property.Name));
            SerializeJToken(emitter, property.Value, serializer);
        }

        emitter.Emit(new MappingEnd());
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer serializer)
    {
        // Implement deserialization if needed.  For serialization-only, throw NotImplementedException:
        throw new NotImplementedException();
    }
}
