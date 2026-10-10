using RPGMakerLibCoreV2.Storages.Dump.Yaml;
using RPGMakerLibCoreV2.Storages.Dump.Yaml.MV;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump.TextSerializers;

public class MvYamlSerializer : AbstractYamlSerializer
{
    private readonly TypeMap _types;

    private ISerializer? _serializer;

    public MvYamlSerializer(TypeMap? types = null)
    {
        _types = types ?? TypeMap.Mv;
    }

    private IDeserializer? _deserializer;
    
    protected void PrepareBuilder<T>(BuilderSkeleton<T> builder) where T : BuilderSkeleton<T>
    {
        builder
            .WithTypeInspector(inner => new MvObjectTypeInspector(inner))
            .WithTypeConverter(new JTokenConverter())
            .WithTypeConverter(new JsonEntityConverter())
            ;

        foreach (var (type, tag) in ConverterHelper.TaggedTypes)
            builder.WithTagMapping($"!{tag}", type);
    }

    protected override ISerializer CreateSerializer()
    {
        if (_serializer != null)
            return _serializer;

        var builder = new SerializerBuilder()
            .WithQuotingNecessaryStrings()
            .EnsureRoundtrip()
            .DisableAliases()
            .WithEmissionPhaseObjectGraphVisitor(args => new ForceIntegerEnumVisitor(args.InnerVisitor))
            .WithEventEmitter(ev => new MultilineScalarFlowStyleEmitter(ev))
            .WithDefaultScalarStyle(ScalarStyle.DoubleQuoted);

        PrepareBuilder(builder);

        _serializer = builder.Build();

        return _serializer;
    }

    protected override IDeserializer CreateDeserializer()
    {
        if (_deserializer != null)
            return _deserializer;

        var builder = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNodeDeserializer(new ForceIntDeserializer())
            .WithNodeTypeResolver(new TypeMapNodeTypeResolver(_types))
            .WithAttemptingUnquotedStringTypeDeserialization();

        PrepareBuilder(builder);

        _deserializer = builder.Build();

        return _deserializer;
    }
}
