using System.Globalization;
using Newtonsoft.Json.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump.MV;

public class JTokenConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return typeof(JToken).IsAssignableFrom(type);
    }

    private static Scalar EmptyScalar = new Scalar(null, null, "", ScalarStyle.ForcePlain, true, false);

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value == null)
        {
            emitter.Emit(EmptyScalar);
            return;
        }
        
        var token = (JToken)value;

        switch (token.Type)
        {
            case JTokenType.Null:
                emitter.Emit(EmptyScalar);
                break;
            case JTokenType.String:
                emitter.Emit(new Scalar(null, null, token.ToString(), ScalarStyle.DoubleQuoted, true, false));
                break;
            case JTokenType.Float:
                emitter.Emit(new Scalar(token.ToObject<float>().ToString(CultureInfo.InvariantCulture)));
                break;
            case JTokenType.Integer:
            case JTokenType.Boolean:
            case JTokenType.Date:
                emitter.Emit(new Scalar(token.ToString())); // Convert numbers and booleans to string
                break;
            case JTokenType.Object:
                SerializeJObject(emitter, (JObject)token, serializer);
                break;
            case JTokenType.Array:
                SerializeJArray(emitter, (JArray)token, serializer); // Recursive call
                break;
            default:
                // Handle other JTokenType values as needed.  For example:
                // JTokenType.Raw, JTokenType.Bytes, JTokenType.Guid, etc.
                // You might want to throw an exception if you encounter a type you don't support.
                emitter.Emit(new Scalar(token.ToString())); // Fallback: output as string.  Adjust as necessary
                break;
        }
    }

    private void SerializeJArray(IEmitter emitter, JArray? value, ObjectSerializer serializer)
    {
        if (value == null)
        {
            emitter.Emit(EmptyScalar);
            return;
        }

        emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Any));

        foreach (var item in value)
            WriteYaml(emitter, item, typeof(JToken), serializer);

        emitter.Emit(new SequenceEnd());
    }
    
    private void SerializeJObject(IEmitter emitter, JObject jObject, ObjectSerializer serializer)
    {
        emitter.Emit(new MappingStart(null, null, false, MappingStyle.Any));

        foreach (var property in jObject.Properties())
        {
            emitter.Emit(new Scalar(property.Name));
            WriteYaml(emitter, property.Value, typeof(JToken), serializer);
        }

        emitter.Emit(new MappingEnd());
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer serializer)
    {
        // Implement deserialization if needed.  For serialization-only, throw NotImplementedException:
        throw new NotImplementedException();
    }
}
