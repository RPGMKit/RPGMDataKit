using RPGMDataKit.Data;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.VXAce.Classes;
using RPGMDataKit.Storages.Abstraction;
using RPGMDataKit.Storages.Serialization;

namespace RPGMDataKit.Storages;

public class VXAceStorage : Storage
{
    public DataLookupHolder<Script> Scripts;
    
    public VXAceStorage(string projectPath, string dataDirectory = "data") : this(projectPath, TypeMap.VXAce,
        dataDirectory)
    {
    }

    /// <param name="types">Entity implementations, e.g. <c>TypeMap.VXAce.Clone().Add&lt;IActor, MyActor&gt;()</c>.</param>
    public VXAceStorage(string projectPath, TypeMap types, string dataDirectory = "data") : base(
        StorageVersion.VXAce, new RubyMarshalDataSerializer(), types, projectPath, dataDirectory)
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
        // an entry of the file has no id (it is the key): the key becomes the id of the entity
        LoadDictionaryHolder(out MapInfos, FileName("MapInfos"), idIsStored: false);
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