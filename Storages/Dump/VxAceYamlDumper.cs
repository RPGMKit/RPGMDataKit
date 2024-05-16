using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Dump.VXAce;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.SpecialTypes;
using RubyMarshalCS.SpecialTypes.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump;

public class VxAceYamlDumper : AbstractYamlDumper
{
    protected override void _Dump(string path, Storage storage)
    {
        var vxStorage = (VXAceStorage)storage;

        DumpEntity(path, nameof(vxStorage.Actors), vxStorage.Actors);
        DumpEntity(path, nameof(vxStorage.Animations), vxStorage.Animations);
        DumpEntity(path, nameof(vxStorage.Armors), vxStorage.Armors);
        DumpEntity(path, nameof(vxStorage.Classes), vxStorage.Classes);
        DumpEntity(path, nameof(vxStorage.CommonEvents), vxStorage.CommonEvents);
        DumpEntity(path, nameof(vxStorage.Enemies), vxStorage.Enemies);
        DumpEntity(path, nameof(vxStorage.Items), vxStorage.Items);
        DumpEntity(path, nameof(vxStorage.Scripts), vxStorage.Scripts);
        DumpEntity(path, nameof(vxStorage.Skills), vxStorage.Skills);
        DumpEntity(path, nameof(vxStorage.States), vxStorage.States);
        DumpEntity(path, nameof(vxStorage.System), vxStorage.System);
        DumpEntity(path, nameof(vxStorage.MapInfos), vxStorage.MapInfos);
        DumpEntity(path, nameof(vxStorage.Tilesets), vxStorage.Tilesets);
        DumpEntity(path, nameof(vxStorage.Troops), vxStorage.Troops);
        DumpEntity(path, nameof(vxStorage.Weapons), vxStorage.Weapons);
        
        DumpMaps(path, vxStorage);
    }

    protected override void _Load(string path, Storage storage)
    {
        var deser = GetDeserializer();

        var res = deser.Deserialize<Enemy>(File.ReadAllText(@"D:\Games\Warlock\Warlock and boobs v0.441\Dump\Enemies\001.yaml"));
        // var res = deser.Deserialize<Script>(File.ReadAllText(@"D:\Games\Warlock\Warlock and boobs v0.441\Dump\Scripts\002.yaml"));

        return;
    }

    private void PrepareBuilder<T>(BuilderSkeleton<T> builder) where T : BuilderSkeleton<T>
    {
        builder
            .WithTypeInspector(inner => new RubyObjectTypeInspector(inner))
            .WithTypeConverter(new EncodingConverter())
            .WithTypeConverter(new ScriptConverter())
            .WithTypeConverter(new DynamicPropertyConverter());

        var rubyObjectTypes = SerializationHelper.GetRegisteredRubyObjects("");
        foreach (var (typeName,type) in rubyObjectTypes)
            builder.WithTagMapping($"!{typeName}", type);
        
        builder.WithTagMapping("!BinaryString", typeof(BinaryString));
        builder.WithTagMapping("!Script", typeof(Script));
        builder.WithTagMapping("!DynamicProperty", typeof(IDynamicProperty));
    }

    public override ISerializer GetSerializer()
    {
        var builder = new SerializerBuilder()
            .WithQuotingNecessaryStrings()
            .EnsureRoundtrip()
            .DisableAliases()
            .WithDefaultScalarStyle(ScalarStyle.DoubleQuoted);

        PrepareBuilder(builder);

        return builder.Build();
    }

    public override IDeserializer GetDeserializer()
    {
        var builder = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithAttemptingUnquotedStringTypeDeserialization();

        PrepareBuilder(builder);

        return builder.Build();
    }
}