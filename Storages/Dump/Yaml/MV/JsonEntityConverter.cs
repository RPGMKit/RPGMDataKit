using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump.Yaml.MV;

public class JsonEntityConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return typeof(JsonEntity).IsAssignableFrom(type);
    }

    private IDictionary<string, JToken>? GetExtensionMember(object value, Type type, bool create = false)
    {
        var extensionMember =
            ConverterHelper.GetMembersWithAttribute<JsonExtensionDataAttribute>(type).FirstOrDefault();
        if (extensionMember == null)
            return null;

        var ret = extensionMember.GetValue(value) as IDictionary<string, JToken>;
        if (ret != null || !create)
            return ret;

        ret = new Dictionary<string, JToken>();
        extensionMember.SetValue(value, ret);

        return ret;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value == null)
        {
            serializer(value);
            return;
        }
        
        ((JsonEntity)value).OnYamlSerializing();

        var tag = ConverterHelper.TaggedTypes.FirstOrDefault(t => t.Key == type).Value;
        
        emitter.Emit(new MappingStart(null, tag != null ? $"!{tag}" : null, false, MappingStyle.Any));
        var members = ConverterHelper.GetMembersWithAttribute<JsonPropertyAttribute>(type);
        var extensionDictionary = GetExtensionMember(value, type);
        foreach (var member in members.OrderBy(p =>
                     p.AttributeAs<JsonPropertyAttribute>()?.PropertyName ?? p.MemberInfo.Name))
        {
            emitter.Emit(
                new Scalar(member.AttributeAs<JsonPropertyAttribute>()?.PropertyName ?? member.MemberInfo.Name));
            serializer(member.GetValue(value));
        }

        if (extensionDictionary != null)
        {
            foreach (var unknownField in extensionDictionary)
            {
                emitter.Emit(new Scalar(unknownField.Key));
                serializer(unknownField.Value);
            }
        }

        emitter.Emit(new MappingEnd());
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer deserializer)
    {
        var obj = Activator.CreateInstance(type)!;

        var curr = parser.Current;
        
        if (curr is Scalar sc && string.IsNullOrEmpty(sc.Value))
        {
            parser.Consume<Scalar>();
            return null;
        }

        parser.Consume<MappingStart>();
        
        var members = ConverterHelper.GetMembersWithAttribute<JsonPropertyAttribute>(type);
        var extensionDictionary = GetExtensionMember(obj, type, true);

        while (parser.Current is Scalar)
        {
            var scalar = parser.Consume<Scalar>();
            var member = members.FirstOrDefault(m =>
            {
                var name = m.AttributeAs<JsonPropertyAttribute>()?.PropertyName ?? m.MemberInfo.Name;
                return name == scalar.Value;
            });
            
            if (member == null)
            {
                if (extensionDictionary != null)
                {
                    extensionDictionary[scalar.Value] = (JToken)deserializer(typeof(JToken));
                    continue;
                }

                deserializer(typeof(object));
                continue;
            }
            
            member.SetValue(obj, deserializer(member.MemberType));
        }

        parser.Consume<MappingEnd>();
        
        ((JsonEntity)obj).OnYamlDeserialized();

        return obj;
    }
}
