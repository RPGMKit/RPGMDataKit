using System.Reflection;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump.Yaml.VXAce;

public class RubyEntityConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return typeof(RubySerializationEntity).IsAssignableFrom(type);
    }

    private IDictionary<string, object?>? GetExtensionMember(object value, Type type, bool create = false)
    {
        var extensionMember =
            ConverterHelper.GetMembersWithAttribute<RubyExtensionDataAttribute>(type).FirstOrDefault();
        if (extensionMember == null)
            return null;

        var ret = extensionMember.GetValue(value) as IDictionary<string, object?>;
        if (ret != null || !create)
            return ret;

        ret = new Dictionary<string, object?>();
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
        
        ((RubySerializationEntity)value).OnYamlSerializing();

        var tag = type.GetCustomAttribute<RubyObjectAttribute>()!.Name;
        
        emitter.Emit(new MappingStart(null, $"!{tag}", false, MappingStyle.Any));
        var members = ConverterHelper.GetMembersWithAttribute<RubyPropertyAttribute>(type);
        var extensionDictionary = GetExtensionMember(value, type);
        foreach (var member in members.OrderBy(p =>
                     p.AttributeAs<RubyPropertyAttribute>()!.PropertyName ?? p.MemberInfo.Name))
        {
            emitter.Emit(
                new Scalar(member.AttributeAs<RubyPropertyAttribute>()!.PropertyName ?? member.MemberInfo.Name));
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
        
        var members = ConverterHelper.GetMembersWithAttribute<RubyPropertyAttribute>(type);
        var extensionDictionary = GetExtensionMember(obj, type, true);

        while (parser.Current is Scalar)
        {
            var scalar = parser.Consume<Scalar>();
            var member = members.FirstOrDefault(m =>
            {
                var name = m.AttributeAs<RubyPropertyAttribute>()!.PropertyName ?? m.MemberInfo.Name;
                return name == scalar.Value;
            });
            
            if (member == null)
            {
                if (extensionDictionary != null)
                {
                    extensionDictionary[scalar.Value] = deserializer(typeof(object));
                    continue;
                }

                deserializer(typeof(object));
                continue;
            }
            
            member.SetValue(obj, deserializer(member.MemberType));
        }

        parser.Consume<MappingEnd>();
        
        ((RubySerializationEntity)obj).OnYamlDeserialized();

        return obj;
    }
}
