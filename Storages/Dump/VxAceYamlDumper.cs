using RPGMakerLibCoreV2.Data.Helpers.Dump;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Dump.MV;
using RPGMakerLibCoreV2.Storages.Dump.VXAce;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.SpecialTypes;
using RubyMarshalCS.SpecialTypes.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using Actor = RPGMakerLibCoreV2.Data.VXAce.Classes.Actor;
using Animation = RPGMakerLibCoreV2.Data.VXAce.Classes.Animation;
using Armor = RPGMakerLibCoreV2.Data.VXAce.Classes.Armor;
using Class = RPGMakerLibCoreV2.Data.VXAce.Classes.Class;
using CommonEvent = RPGMakerLibCoreV2.Data.VXAce.Classes.CommonEvent;
using Enemy = RPGMakerLibCoreV2.Data.VXAce.Classes.Enemy;
using Item = RPGMakerLibCoreV2.Data.VXAce.Classes.Item;
using Map = RPGMakerLibCoreV2.Data.VXAce.Classes.Map;
using MapInfo = RPGMakerLibCoreV2.Data.VXAce.Classes.MapInfo;
using Skill = RPGMakerLibCoreV2.Data.VXAce.Classes.Skill;
using State = RPGMakerLibCoreV2.Data.VXAce.Classes.State;
using Troop = RPGMakerLibCoreV2.Data.VXAce.Classes.Troop;
using Weapon = RPGMakerLibCoreV2.Data.VXAce.Classes.Weapon;

namespace RPGMakerLibCoreV2.Storages.Dump;

public class VxAceYamlDumper : AbstractYamlDumper
{
    protected override void _Dump(string path, Storage storage)
    {
        var vxStorage = (VXAceStorage)storage;

        Reporter?.SetStage(DumpStage.Init);
        Reporter?.Report();
        
        Reporter?.SetStage(DumpStage.Progress);
        
        Reporter?.SetGlobalTotal(16);

        DumpHolder(path, nameof(vxStorage.Actors), vxStorage.Actors);
        DumpHolder(path, nameof(vxStorage.Animations), vxStorage.Animations);
        DumpHolder(path, nameof(vxStorage.Armors), vxStorage.Armors);
        DumpHolder(path, nameof(vxStorage.Classes), vxStorage.Classes);
        DumpHolder(path, nameof(vxStorage.CommonEvents), vxStorage.CommonEvents);
        DumpHolder(path, nameof(vxStorage.Enemies), vxStorage.Enemies);
        DumpHolder(path, nameof(vxStorage.Items), vxStorage.Items);
        DumpHolder(path, nameof(vxStorage.Scripts), vxStorage.Scripts);
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
        var vxStorage = (VXAceStorage)storage;
        
        Reporter?.SetStage(DumpStage.Init);
        Reporter?.Report();
        
        Reporter?.SetStage(DumpStage.Progress);
        
        Reporter?.SetGlobalTotal(16);

        LoadHolder<Actor, IActor>(path, nameof(storage.Actors), storage.Actors);
        LoadHolder<Animation, IAnimation>(path, nameof(storage.Animations), storage.Animations);
        LoadHolder<Armor, IArmor>(path, nameof(storage.Armors), storage.Armors);
        LoadHolder<Class, IClass>(path, nameof(storage.Classes), storage.Classes);
        LoadHolder<CommonEvent, ICommonEvent>(path, nameof(storage.CommonEvents), storage.CommonEvents);
        LoadHolder<Enemy, IEnemy>(path, nameof(storage.Enemies), storage.Enemies);
        LoadHolder<Item, IItem>(path, nameof(storage.Items), storage.Items);
        LoadHolder<Script, Script>(path, nameof(vxStorage.Scripts), vxStorage.Scripts);
        LoadHolder<Skill, ISkill>(path, nameof(storage.Skills), storage.Skills);
        LoadHolder<State, IState>(path, nameof(storage.States), storage.States);
        LoadHolder<Data.VXAce.Classes.System, ISystem>(path, nameof(storage.System), storage.System);
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
            .WithTypeInspector(inner => new RubyObjectTypeInspector(inner))
            .WithTypeConverter(new EncodingConverter())
            .WithTypeConverter(new DynamicPropertyConverter());

        var rubyObjectTypes = SerializationHelper.GetRegisteredRubyObjects("");
        foreach (var (typeName,type) in rubyObjectTypes)
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
            .WithDefaultScalarStyle(ScalarStyle.DoubleQuoted);

        PrepareBuilder(builder);

        return builder.Build();
    }

    protected override IDeserializer CreateDeserializer()
    {
        var builder = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
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