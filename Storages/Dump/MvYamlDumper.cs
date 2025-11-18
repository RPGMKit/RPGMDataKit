using RPGMakerLibCoreV2.Data.Helpers.Dump;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Dump.MV;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump;

public class MvYamlDumper : AbstractYamlDumper
{
    protected override void _Dump(string path, Storage storage)
    {
        var vxStorage = (MVStorage)storage;

        Reporter?.SetStage(DumpStage.Init);
        Reporter?.Report();
        
        Reporter?.SetStage(DumpStage.Progress);
        
        Reporter?.SetGlobalTotal(15);

        DumpHolder(path, nameof(vxStorage.Actors), vxStorage.Actors);
        DumpHolder(path, nameof(vxStorage.Animations), vxStorage.Animations);
        DumpHolder(path, nameof(vxStorage.Armors), vxStorage.Armors);
        DumpHolder(path, nameof(vxStorage.Classes), vxStorage.Classes);
        DumpHolder(path, nameof(vxStorage.CommonEvents), vxStorage.CommonEvents);
        DumpHolder(path, nameof(vxStorage.Enemies), vxStorage.Enemies);
        DumpHolder(path, nameof(vxStorage.Items), vxStorage.Items);
        DumpHolder(path, nameof(vxStorage.Skills), vxStorage.Skills);
        DumpHolder(path, nameof(vxStorage.States), vxStorage.States);
        DumpHolder(path, nameof(vxStorage.System), vxStorage.System);
        DumpHolder(path, nameof(vxStorage.MapInfos), vxStorage.MapInfos);
        DumpHolder(path, nameof(vxStorage.Tilesets), vxStorage.Tilesets);
        DumpHolder(path, nameof(vxStorage.Troops), vxStorage.Troops);
        DumpHolder(path, nameof(vxStorage.Weapons), vxStorage.Weapons);
        
        DumpMaps(path, vxStorage);
        
        Reporter?.SetStage(DumpStage.Finished);
    }

    protected override void _Load(string path, Storage storage)
    {
        Reporter?.SetStage(DumpStage.Init);
        Reporter?.Report();
        
        Reporter?.SetStage(DumpStage.Progress);
        
        Reporter?.SetGlobalTotal(15);

        LoadHolder<Actor, IActor>(path, nameof(storage.Actors), storage.Actors);
        LoadHolder<Animation, IAnimation>(path, nameof(storage.Animations), storage.Animations);
        LoadHolder<Armor, IArmor>(path, nameof(storage.Armors), storage.Armors);
        LoadHolder<Class, IClass>(path, nameof(storage.Classes), storage.Classes);
        LoadHolder<CommonEvent, ICommonEvent>(path, nameof(storage.CommonEvents), storage.CommonEvents);
        LoadHolder<Enemy, IEnemy>(path, nameof(storage.Enemies), storage.Enemies);
        LoadHolder<Item, IItem>(path, nameof(storage.Items), storage.Items);
        LoadHolder<Skill, ISkill>(path, nameof(storage.Skills), storage.Skills);
        LoadHolder<State, IState>(path, nameof(storage.States), storage.States);
        LoadHolder<Data.MV.Classes.System, ISystem>(path, nameof(storage.System), storage.System);
        LoadHolder<MapInfo, IMapInfo>(path, nameof(storage.MapInfos), storage.MapInfos);
        LoadHolder<Tileset, ITileSet>(path, nameof(storage.Tilesets), storage.Tilesets);
        LoadHolder<Troop, ITroop>(path, nameof(storage.Troops), storage.Troops);
        LoadHolder<Weapon, IWeapon>(path, nameof(storage.Weapons), storage.Weapons);
        
        LoadMaps<Map>(path, storage);
        
        Reporter?.SetStage(DumpStage.Finished);
    }

    private void PrepareBuilder<T>(BuilderSkeleton<T> builder) where T : BuilderSkeleton<T>
    {
        builder
            .WithTypeInspector(inner => new MvObjectTypeInspector(inner));
        
        builder.WithTagMapping("!" + nameof(SoundData), typeof(SoundData));
        builder.WithTagMapping("!" + nameof(MoveRoute), typeof(MoveRoute));
        builder.WithTagMapping("!" + nameof(MoveCommand), typeof(MoveCommand));
    }

    public ISerializer _GetSerializer()=> CreateSerializer();
    public IDeserializer _GetDeserializer()=> CreateDeserializer();
    
    protected override ISerializer CreateSerializer()
    {
        var builder = new SerializerBuilder()
            .WithQuotingNecessaryStrings()
            .EnsureRoundtrip()
            .DisableAliases()
            .WithEmissionPhaseObjectGraphVisitor(args => new ForceIntegerEnumVisitor(args.InnerVisitor))
            // .WithTypeConverter(new JArrayConverter())
            .WithTypeConverter(new JTokenConverter())
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
    
    protected override void OnLoad<T>(T entity)
    {
    }

    protected override void OnDump<T>(T entity)
    {
    }
}