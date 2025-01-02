using RPGMakerLibCoreV2.Data;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RubyMarshalCS;

namespace RPGMakerLibCoreV2.Storages;

public class VXAceStorage : Storage
{
    public DataLookupHolder<Script> Scripts;
    
    public VXAceStorage(string projectPath) : base(projectPath)
    {
        Version = StorageVersion.VXAce;
    }

    protected override string MapPath(int index) => DataPath($"Map{index:D3}.rvdata2");

    protected override void Init()
    {
        Actors = new(this, DataPath("Actors.rvdata2"), path => LoadData<LookupList<Actor>>(path).MorphToBase<IActor>());
        Animations = new(this, DataPath("Animations.rvdata2"),
            path => LoadData<LookupList<Animation>>(path).MorphToBase<IAnimation>());
        Armors = new(this, DataPath("Armors.rvdata2"), path => LoadData<LookupList<Armor>>(path).MorphToBase<IArmor>());
        Classes = new(this, DataPath("Classes.rvdata2"), path => LoadData<LookupList<Class>>(path).MorphToBase<IClass>());
        CommonEvents = new(this, DataPath("CommonEvents.rvdata2"),
            path => LoadData<LookupList<CommonEvent>>(path).MorphToBase<ICommonEvent>());
        Enemies = new(this, DataPath("Enemies.rvdata2"), path => LoadData<LookupList<Enemy>>(path).MorphToBase<IEnemy>());
        Items = new(this, DataPath("Items.rvdata2"), path => LoadData<LookupList<Item>>(path).MorphToBase<IItem>());
        Maps = new(this, MapPath, LoadData<Map>);
        MapInfos = new(this, DataPath("MapInfos.rvdata2"),
            path => LoadData<LookupMap<MapInfo>>(path).MorphToBase<IMapInfo>());
        Scripts = new(this, DataPath("Scripts.rvdata2"), LoadData<LookupList<Script>>);
        Skills = new(this, DataPath("Skills.rvdata2"), path => LoadData<LookupList<Skill>>(path).MorphToBase<ISkill>());
        States = new(this, DataPath("States.rvdata2"), path => LoadData<LookupList<State>>(path).MorphToBase<IState>());
        System = new(this, DataPath("System.rvdata2"), LoadData<Data.VXAce.Classes.System>);
        Tilesets = new(this, DataPath("Tilesets.rvdata2"),
            path => LoadData<LookupList<TileSet>>(path).MorphToBase<ITileSet>());
        Troops = new(this, DataPath("Troops.rvdata2"), path => LoadData<LookupList<Troop>>(path).MorphToBase<ITroop>());
        Weapons = new(this, DataPath("Weapons.rvdata2"), path => LoadData<LookupList<Weapon>>(path).MorphToBase<IWeapon>());
    }

    public override string DataPath(string dataObject)
    {
        return Path.Combine("data", dataObject);
    }

    public override T LoadData<T>(string relativePath)
    {
        var path = Path.Combine(ProjectPath, relativePath);
        if (!File.Exists(path))
            throw new FileNotFoundException(path);
        
        return RubyMarshal.Load<T>(DataPath(path));
    }

    public override void SaveData<T>(string relativePath, T data)
    {
        throw new NotImplementedException();
    }

    public override void SaveAll()
    {
        throw new NotImplementedException();
    }
}