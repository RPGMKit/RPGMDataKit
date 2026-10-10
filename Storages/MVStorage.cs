using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Serialization;

namespace RPGMakerLibCoreV2.Storages;

public class MVStorage : Storage
{
    public MVStorage(string projectPath, string dataDirectory = "data") : this(projectPath, TypeMap.Mv,
        dataDirectory)
    {
    }

    /// <param name="types">Entity implementations, e.g. <c>TypeMap.Mv.Clone().Add&lt;IActor, MyActor&gt;()</c>.</param>
    public MVStorage(string projectPath, TypeMap types, string dataDirectory = "data") : base(StorageVersion.MV,
        new JsonDataSerializer(types), types, projectPath, dataDirectory)
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
        LoadListHolder(out MapInfos, FileName("MapInfos"));
        LoadListHolder(out Skills, FileName("Skills"));
        LoadListHolder(out States, FileName("States"));
        LoadHolder(out System, FileName("System"));
        LoadListHolder(out Tilesets, FileName("Tilesets"));
        LoadListHolder(out Troops, FileName("Troops"));
        LoadListHolder(out Weapons, FileName("Weapons"));
    }
}   
