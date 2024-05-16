using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.SpecialTypes;
using RubyMarshalCS.SpecialTypes.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump.VXAce;

public class DynamicPropertyConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return typeof(IDynamicProperty).IsAssignableFrom(type);
    }

    private static readonly List<Type> allowedTypes = new()
    {
        typeof(SByte),
        typeof(Byte),
        typeof(Int16),
        typeof(UInt16),
        typeof(Int32),
        typeof(UInt32),
        typeof(Int64),
        typeof(UInt64),
        typeof(Single),
        typeof(Double),
        typeof(Decimal),
        typeof(String),
    };

    public object? ReadYaml(IParser parser, Type type)
    {
        string? valueTypeName = null;
        object? value = null;
        List<string> propertyAllowedTypes = new();

        parser.Consume<MappingStart>();

        for (var i = 0; i < 3; ++i)
        {
            var field = parser.Consume<Scalar>();
            switch (field.Value)
            {
                case "Type":
                    valueTypeName = parser.Consume<Scalar>().Value;
                    break;
                case "AllowedTypes":
                    parser.Consume<SequenceStart>();
                    while (!parser.TryConsume<SequenceEnd>(out _))
                        propertyAllowedTypes.Add(parser.Consume<Scalar>().Value);
                    // parser.Consume<SequenceEnd>();
                    break;
                case "Value":
                    value = parser.Consume<Scalar>().Value;
                    break;
            }
        }

        parser.Consume<MappingEnd>();

        if (!propertyAllowedTypes.Any(_ => _.ToString() == valueTypeName))
            throw new Exception($"Value type {valueTypeName} not allowed");

        foreach (var typeName in propertyAllowedTypes)
            if (!propertyAllowedTypes.Any(_ => _.ToString() == typeName))
                throw new Exception($"Value type {typeName} not allowed");

        Type genericType;
        switch (propertyAllowedTypes.Count)
        {
            case 2:
                genericType = typeof(DynamicProperty<,>);
                break;
            case 3:
                genericType = typeof(DynamicProperty<,,>);
                break;
            case 4:
                genericType = typeof(DynamicProperty<,,,>);
                break;
            default:
                throw new Exception("Unsupported number of generic arguments");
        }

        var propType = genericType.MakeGenericType(propertyAllowedTypes.Select(GetTypeByName).ToArray());

        var prop = (IDynamicProperty)Activator.CreateInstance(propType)!;
        prop.Set(SerializationHelper.AssignmentConversion(GetTypeByName(valueTypeName!), value, false));

        return prop;
    }

    private static Type GetTypeByName(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type == null)
            throw new Exception($"Type {typeName} not found");

        return type;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        var prop = (IDynamicProperty)value!;

        var tmp = type.GetGenericArguments();

        var allowed = allowedTypes;

        var ms = new MappingStart(null, "!DynamicProperty", false, MappingStyle.Block);

        emitter.Emit(ms);

        emitter.Emit(new Scalar("Type"));
        emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, prop.GetValueType()?.ToString(),
            ScalarStyle.DoubleQuoted, true, true));

        emitter.Emit(new Scalar("AllowedTypes"));
        emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, true, SequenceStyle.Any));
        foreach (var t in tmp)
            emitter.Emit(
                new Scalar(AnchorName.Empty, TagName.Empty, t.ToString(), ScalarStyle.DoubleQuoted, true, true));

        emitter.Emit(new SequenceEnd());

        emitter.Emit(new Scalar("Value"));
        var val = prop.Get();
        emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, val?.ToString(),
            val is string ? ScalarStyle.DoubleQuoted : ScalarStyle.Any, true, true));

        emitter.Emit(new MappingEnd());
    }
}
