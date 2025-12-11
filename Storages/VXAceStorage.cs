using RPGMakerLibCoreV2.Data;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RubyMarshalCS;

namespace RPGMakerLibCoreV2.Storages;

public class VXAceStorage : Storage
{
    public DataLookupHolder<Script> Scripts;
    
    public VXAceStorage(string projectPath, string dataDirectory = "data") : base(projectPath, dataDirectory)
    {
        Version = StorageVersion.VXAce;
    }

    protected override string MapPath(int index) => $"Map{index:D3}.rvdata2";

    protected override void Init()
    {
        Actors = LoadListHolder<Actor, IActor>("Actors.rvdata2");
        Animations = LoadListHolder<Animation, IAnimation>("Animations.rvdata2");
        Armors = LoadListHolder<Armor, IArmor>("Armors.rvdata2");
        Classes = LoadListHolder<Class, IClass>("Classes.rvdata2");
        CommonEvents = LoadListHolder<CommonEvent, ICommonEvent>("CommonEvents.rvdata2");
        Enemies = LoadListHolder<Enemy, IEnemy>("Enemies.rvdata2");
        Items = LoadListHolder<Item, IItem>("Items.rvdata2");
        Maps = new(this, MapPath, LoadData<Map>);
        MapInfos = LoadDictionaryHolder<MapInfo, IMapInfo>("MapInfos.rvdata2");
        Scripts = LoadListHolder<Script, Script>("Scripts.rvdata2");
        Skills = LoadListHolder<Skill, ISkill>("Skills.rvdata2");
        States = LoadListHolder<State, IState>("States.rvdata2");
        System = LoadHolder<Data.VXAce.Classes.System, ISystem>("System.rvdata2");
        Tilesets = LoadListHolder<Tileset, ITileSet>("Tilesets.rvdata2");
        Troops = LoadListHolder<Troop,ITroop>("Troops.rvdata2");
        Weapons = LoadListHolder<Weapon, IWeapon>("Weapons.rvdata2");
    }

    public override void Unload()
    {
        base.Unload();
        Scripts.Unload();
    }

    public override string DataPath(string dataObject)
    {
        return Path.Combine(DataDirectory, dataObject);
    }

    public override T LoadData<T>(string dataFile)
    {
        var path = Path.Combine(ProjectPath, DataDirectory, dataFile);
        if (!File.Exists(path))
            throw new FileNotFoundException(path);
        
        return RubyMarshal.Load<T>(path);
    }

    public override void SaveData<T>(string dataFile, T data)
    {
        var path = Path.Combine(ProjectPath, DataDirectory, dataFile);
        RubyMarshal.Dump(path, data);
    }

    public override void Preload()
    {
        base.Preload();
        Scripts.Get();
    }
    
    public override void SaveAll()
    {
        base.SaveAll();
        Scripts.Save();
    }

    public async Task DumpScripts(string path)
    {
        if (!Directory.Exists(path))
            throw new Exception("Directory not found: " + path);

        foreach (var (i, script) in Scripts.Get())
        {
            if (script == null)
                continue;

            if (string.IsNullOrWhiteSpace(script.Code))
                continue;

            var name = !string.IsNullOrWhiteSpace(script.Name) ? $"{i:D3}_{script.Name}.rb" : $"{i:D3}_script.rb";

            await File.WriteAllTextAsync(Path.Combine(path, name), script.Code);
        }
    }
}