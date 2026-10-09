using RPGMakerLibCoreV2.Data;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Serialization;

namespace RPGMakerLibCoreV2.Storages;

public class VXAceStorage : Storage
{
    public DataLookupHolder<Script> Scripts;
    
    public VXAceStorage(string projectPath, string dataDirectory = "data") : base(StorageVersion.VXAce,
        new RubyMarshalDataSerializer(), TypeMap.VXAce, projectPath, dataDirectory)
    {
    }

    protected override void Init()
    {
        LoadListHolder(out Actors, FileName("Actors"));
        LoadListHolder(out Animations, FileName("Animations"));
        LoadListHolder(out Armors, FileName("Armors"));
        LoadListHolder(out Classes, FileName("Classes"));
        LoadListHolder(out CommonEvents, FileName("CommonEvents"));
        LoadListHolder(out Enemies, FileName("Enemies"));
        LoadListHolder(out Items, FileName("Items"));
        LoadMapsHolder(out Maps);
        LoadDictionaryHolder(out MapInfos, FileName("MapInfos"));
        LoadListHolder(out Scripts, FileName("Scripts"));
        LoadListHolder(out Skills, FileName("Skills"));
        LoadListHolder(out States, FileName("States"));
        LoadHolder(out System, FileName("System"));
        LoadListHolder(out Tilesets, FileName("Tilesets"));
        LoadListHolder(out Troops, FileName("Troops"));
        LoadListHolder(out Weapons, FileName("Weapons"));
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