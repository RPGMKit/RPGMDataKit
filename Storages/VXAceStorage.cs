using RPGMakerLibCoreV2.Data;
using RPGMakerLibCoreV2.Data.Enums;
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
        Actors = new(this, DataPath("Actors.rvdata2"), LoadData<LookupList<Actor>>);
        Animations = new(this, DataPath("Animations.rvdata2"), LoadData<LookupList<Animation>>);
        Armors = new(this, DataPath("Armors.rvdata2"), LoadData<LookupList<Armor>>);
        Classes = new(this, DataPath("Classes.rvdata2"), LoadData<LookupList<Class>>);
        CommonEvents = new(this, DataPath("CommonEvents.rvdata2"), LoadData<LookupList<CommonEvent>>);
        Enemies = new(this, DataPath("Enemies.rvdata2"), LoadData<LookupList<Enemy>>);
        Items = new(this, DataPath("Items.rvdata2"), LoadData<LookupList<Item>>);
        Maps = new(this, MapPath, LoadData<Map>);
        MapInfos = new(this, DataPath("MapInfos.rvdata2"), LoadData<LookupMap<MapInfo>>);
        Scripts = new(this, DataPath("Scripts.rvdata2"), LoadData<LookupList<Script>>);
        Skills = new(this, DataPath("Skills.rvdata2"), LoadData<LookupList<Skill>>);
        States = new(this, DataPath("States.rvdata2"), LoadData<LookupList<State>>);
        System = new(this, DataPath("System.rvdata2"), LoadData<Data.VXAce.Classes.System>);
        Tilesets = new(this, DataPath("Tilesets.rvdata2"), LoadData<LookupList<TileSet>>);
        Troops = new(this, DataPath("Troops.rvdata2"), LoadData<LookupList<Troop>>);
        Weapons = new(this, DataPath("Weapons.rvdata2"), LoadData<LookupList<Weapon>>);
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