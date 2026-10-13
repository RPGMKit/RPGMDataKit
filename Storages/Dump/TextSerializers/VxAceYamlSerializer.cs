using RPGMDataKit.Data.VXAce.Classes;
using RPGMDataKit.Storages.Dump.Yaml;
using RPGMDataKit.Storages.Dump.Yaml.VXAce;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.SpecialTypes;
using RubyMarshalCS.SpecialTypes.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace RPGMDataKit.Storages.Dump.TextSerializers;

public class VxAceYamlSerializer : AbstractYamlSerializer
{
    private readonly TypeMap _types;

    public VxAceYamlSerializer(TypeMap? types = null)
    {
        _types = types ?? TypeMap.VXAce;
    }

    private void PrepareBuilder<T>(BuilderSkeleton<T> builder) where T : BuilderSkeleton<T>
    {
        builder
            .WithTypeInspector(inner => new RubyObjectTypeInspector(inner))
            .WithTypeConverter(new EncodingConverter())
            .WithTypeConverter(new DynamicPropertyConverter())
            .WithTypeConverter(new BinaryStringConverter())
            .WithTypeConverter(new RubyEntityConverter());

        var rubyObjectTypes = SerializationHelper.GetRegisteredRubyObjects("");
        foreach (var (typeName, type) in rubyObjectTypes)
            builder.WithTagMapping($"!{typeName}", type);

        builder.WithTagMapping("!BinaryString", typeof(BinaryString));
        builder.WithTagMapping("!Script", typeof(Script));
        builder.WithTagMapping("!DynamicProperty", typeof(IDynamicProperty));
    }

    protected override ISerializer CreateSerializer()
    {
        var builder = new SerializerBuilder()
            .WithQuotingNecessaryStrings()
            .EnsureRoundtrip()
            .DisableAliases()
            .WithEmissionPhaseObjectGraphVisitor(args => new ForceIntegerEnumVisitor(args.InnerVisitor))
            .WithEventEmitter(ev => new MultilineScalarFlowStyleEmitter(ev))
            .WithDefaultScalarStyle(ScalarStyle.DoubleQuoted);

        PrepareBuilder(builder);

        return builder.Build();
    }

    protected override IDeserializer CreateDeserializer()
    {
        var builder = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNodeDeserializer(new ForceIntDeserializer())
            .WithNodeTypeResolver(new TypeMapNodeTypeResolver(_types))
            .WithAttemptingUnquotedStringTypeDeserialization();

        PrepareBuilder(builder);

        return builder.Build();
    }
}
