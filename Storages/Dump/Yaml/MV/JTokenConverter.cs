using Newtonsoft.Json.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMDataKit.Storages.Dump.Yaml.MV;

public class JTokenConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return typeof(JToken).IsAssignableFrom(type);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value == null)
        {
            serializer(null);
            return;
        }

        var token = (JToken)value;

        switch (token.Type)
        {
            case JTokenType.Null:
                serializer(null);
                break;
            case JTokenType.String:
                serializer(token.ToString());
                break;
            case JTokenType.Float:
                serializer(token.ToObject<float>());
                break;
            case JTokenType.Integer:
            case JTokenType.Boolean:
            case JTokenType.Date:
                emitter.Emit(new Scalar(token.ToString()));
                break;
            case JTokenType.Object:
                SerializeJObject(emitter, (JObject)token, serializer);
                break;
            case JTokenType.Array:
                SerializeJArray(emitter, (JArray)token, serializer);
                break;
            default:
                throw new Exception($"Unsupportedf JToken type '{token.Type}'");
        }
    }

    private void SerializeJArray(IEmitter emitter, JArray? value, ObjectSerializer serializer)
    {
        if (value == null)
        {
            serializer(null);
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
            serializer(property.Value);
        }

        emitter.Emit(new MappingEnd());
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer deserializer)
    {
        if (type == typeof(JToken))
        {
            var obj = deserializer(typeof(object));
            if (obj == null)
                return JValue.CreateNull();

            return JToken.FromObject(obj);
        }

        if (type == typeof(JObject))
        {
            if (parser.Current is not MappingStart)
                throw new Exception("Current is not mapping start");

            var dict = deserializer(typeof(Dictionary<string, object>));

            return JObject.FromObject(dict);
        }

        if (type == typeof(JArray))
        {
            if (parser.Current is not SequenceStart)
                throw new Exception("Current is not sequence start");

            var list = deserializer(typeof(List<object>));

            return JArray.FromObject(list);
        }

        if (type == typeof(JValue))
        {
            if (parser.Current is not Scalar)
                throw new Exception("Current is not scalar");

            var value = deserializer(typeof(object));
            if (value == null)
                return JValue.CreateNull();

            return JValue.FromObject(value);
        }

        throw new Exception($"Unknown JToken type '{type.Name}'");
    }
}
