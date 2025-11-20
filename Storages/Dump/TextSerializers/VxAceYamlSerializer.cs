using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RPGMakerLibCoreV2.Storages.Dump.Yaml;
using RPGMakerLibCoreV2.Storages.Dump.Yaml.VXAce;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.SpecialTypes;
using RubyMarshalCS.SpecialTypes.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump.TextSerializers;

public class VxAceYamlSerializer : AbstractYamlSerializer
{
    private void PrepareBuilder<T>(BuilderSkeleton<T> builder) where T : BuilderSkeleton<T>
    {
        builder
            .WithTypeInspector(inner => new RubyObjectTypeInspector(inner))
            .WithTypeConverter(new EncodingConverter())
            .WithTypeConverter(new DynamicPropertyConverter())
            .WithTypeConverter(new BinaryStringConverter())
            .WithTypeConverter(new ScriptConverter())
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
            .WithAttemptingUnquotedStringTypeDeserialization();

        PrepareBuilder(builder);

        return builder.Build();
    }
}
