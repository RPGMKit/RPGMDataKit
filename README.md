# RPGMDataKit

A .NET library for reading, editing and writing the data files of **RPG Maker VX Ace** (`.rvdata2`) and **RPG Maker MV** (`.json`) projects through one set of interfaces.

Code written against `IActor`, `ISkill`, `IMap` and the other interfaces works with either version. The library keeps what it does not know about, so a file that is loaded and saved comes back without losing anything.

## Features

- **One API for two engines.** `VXAceStorage` and `MVStorage` expose the same typed tables: actors, classes, skills, items, weapons, armor, enemies, states, animations, tilesets, troops, common events, maps, map infos and `System`.
- **Typed event commands.** The commands of events, common events and troops are classes (`ChangeGoldCommand`, `ShowTextBaseCommand`, `TransferPlayerCommand` and so on) with named properties instead of raw parameter lists. Unknown commands are kept as they are.
- **Nothing is lost on save.** Members that the classes do not describe go to the unknown fields of the entity and are written back. Round trips of the data of empty VX Ace and MV projects are covered by tests.
- **Factories for new entities.** `storage.New<IActor>()` creates an entity with the values the editor gives to a new one.
- **Custom entity types.** A `TypeMap` decides which class implements each interface; copy a built-in map and register your own classes.
- **Text dumps.** `storage.CreateYamlStorage()` writes a project as YAML files, which makes data easy to read, review and keep in version control; the same classes load it back.
- **Diff of two projects.** `Differ` compares two storages (variables, switches, common events, map event pages) and returns patches.

## Requirements

- .NET 10
- [RubyMarshalCS](https://github.com/Dramacydal/RubyMarshalCS), a git submodule in `external/RubyMarshalCS` (reads and writes Ruby Marshal, the format of `.rvdata2`)
- NuGet: Newtonsoft.Json, YamlDotNet

## Getting started

Clone with the submodule and build:

```bash
git clone --recurse-submodules https://github.com/RPGMKit/RPGMDataKit.git
cd RPGMDataKit
dotnet build
```

Add a reference to `RPGMDataKit.csproj` from your project:

```xml
<ProjectReference Include="path\to\RPGMDataKit\RPGMDataKit.csproj" />
```

## Usage

### Open a project

```csharp
using RPGMDataKit.Storages;

// VX Ace: the folder of the project, the data folder is "Data" (.rvdata2)
var vx = new VXAceStorage(@"C:\Games\MyGame", "Data");

// MV: the folder with the "data" folder, usually "www"
var mv = new MVStorage(@"C:\Games\MyMvGame\www");
```

Tables are loaded when you first read them. `Preload()` loads everything, `Unload()` drops the loaded data.

### Read and edit data

```csharp
var actor = vx.Actors.Get(1)!;
actor.Name = "Hero";

foreach (var (id, skill) in vx.Skills.Get())
    Console.WriteLine($"{id}: {skill?.Name}");

vx.SaveAll();   // or vx.Skills.Save() for one table
```

`DataLookupHolder<T>` tables have `Get(id)`, `Add`, `Set`, `Remove`, `NextId()` and `Clear()`. `System` is a `DataHolder<ISystem>` with `Get()`, and maps are read through `Maps.Get(id)`.

### Create entities

```csharp
var skill = vx.New<ISkill>();
skill.Name = "Fireball";
var id = vx.Skills.Add(skill);
```

### Tables of your own (MV)

Data files that are not part of the editor, for example the data of a plugin, can be added to a storage. They are saved, preloaded and unloaded with the other tables:

```csharp
public class Quest : JsonEntity, IHasId
{
    [JsonProperty("id")]    public int Id { get; set; }
    [JsonProperty("title")] public string Title { get; set; } = "";
}

var quests = mv.AddList<Quest>("Quests");                  // data/Quests.json: [null, {...}, {...}]
var stats  = mv.AddDictionary<Quest>("Stats");             // data/Stats.json: {"3": {...}}
var config = mv.AddEntity<Settings>("Settings");           // data/Settings.json: {...}

var id = quests.Add(new Quest { Title = "First" });        // the index of the new entity
mv.SaveAll();
```

- Any holder, standard or added, can be found by name (the name of its data file without the extension, not case sensitive): `mv.GetHolder<DataLookupHolder<Quest>>("quests")`, `mv.GetHolder<DataLookupHolder<IActor>>("Actors")`, or `TryGetHolder`; `mv.HolderNames` lists the names.
- `optional: true` lets the file be absent: the table starts empty (a new entity for `AddEntity`) and the file is written on save.
- The id of an entity is its **place**: the index in a list or the key in a dictionary, as the engines use it (`$dataItems[id]`). An entity in a list must have an id (`IHasId`).
- When a table is loaded and the id written in an entity differs from its place, `mv.IdMismatched` is raised and the entity is left as it is. Subscribe before the table is first read, or call `Preload()` afterwards. The standard tables raise it too.
- The class must derive from `JsonEntity`, which keeps the members the class does not describe. Another class is refused when added.
- VX Ace does not support this yet.

### Event commands

Pages of events, common events and troops are `ICommandContainer`s, and `NamedCommands` gives typed commands:

```csharp
using RPGMDataKit.Data.Structures.Commands;

var map = vx.Maps.Get(1)!;
foreach (var ev in map.Events.Values)
    foreach (var page in ev!.Pages)
        foreach (var command in page!.NamedCommands)
            if (command is ChangeGoldCommand gold)
                gold.Value *= 2;
```

`MapEventHelper.WalkMapEventPages` walks every page of every map event with its context (map, event, page).

### Your own entity classes

```csharp
var types = TypeMap.VXAce.Clone().Add<IActor, MyActor>();
var vx = new VXAceStorage(@"C:\Games\MyGame", types, "Data");
```

The built-in maps `TypeMap.VXAce` and `TypeMap.Mv` are read-only; clone one to change it. `MVStorage` takes a map the same way.

### YAML dump

```csharp
var yaml = vx.CreateYamlStorage();            // the dump of this storage, VX Ace or MV
await yaml.Dump(@"C:\dump");                  // the folder must exist
yaml.Load(@"C:\dump");                        // fills the storage from the dump
```

`yaml.Reporter` gives progress and lets you cancel. The dump contains every holder of the storage, the standard ones and the ones you added (a folder for a list or a dictionary, a file for a single entity). Loading leaves a holder as it is when the dump does not have it. The dump works with the entity types of the storage it was made by.

### Compare two projects

```csharp
var patches = await Differ.CalculatePatches(left, right, new DumpOptions());
```

`DumpOptions` selects the text format of the commands: indents, command codes, extended format and the line numbers.

## Project layout

| Path | What is there |
|---|---|
| `Storages/` | `VXAceStorage`, `MVStorage`, `TypeMap`, serializers, YAML dump |
| `Data/Structures/Interfaces/` | the entity interfaces shared by both versions |
| `Data/VXAce/Classes/`, `Data/MV/Classes/` | the implementations for each version |
| `Data/Structures/Commands/` | typed event commands and movement commands |
| `Data/Helpers/` | walkers and formatters for maps, common events and troops |
| `Diff/`, `Differ.cs` | text diff of two projects |
| `tests/RPGMDataKit.Tests/` | xUnit tests with the data of empty projects of both editors |
| `external/RubyMarshalCS/` | the Ruby Marshal library (submodule) |

## Tests

```bash
dotnet test tests/RPGMDataKit.Tests
```

The tests check the default values of the factories, the class hierarchies, saving and loading of the data of empty VX Ace and MV projects, and that no members are lost on the way.

## Notes

- Files of the editors are binary (`.rvdata2`) or JSON (MV). Keep a copy of a project before writing to it.
- This library does not include anything from the editors: no standard scripts, graphics or audio. The project files in `tests/` are the data files of empty projects, without the standard scripts.

## License

[MIT](LICENSE). Third-party code and dependencies: [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
