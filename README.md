# RPGMDataKit

A .NET library for reading, editing and writing the data files of **RPG Maker VX Ace** (`.rvdata2`) and **RPG Maker MV** (`.json`) projects through one set of interfaces.

Code written against `IActor`, `ISkill`, `IMap` and the other interfaces works with either version. The library keeps what it does not know about, so a file that is loaded and saved comes back without losing anything.

## Features

- **One API for two engines.** `VXAceStorage` and `MVStorage` expose the same typed tables: actors, classes, skills, items, weapons, armor, enemies, states, animations, tilesets, troops, common events, maps, map infos and `System`.
- **Typed event commands.** The commands of events, common events and troops are classes (`ChangeGoldCommand`, `ShowTextBaseCommand`, `TransferPlayerCommand` and so on) with named properties instead of raw parameter lists. Unknown commands are kept as they are.
- **Blocks of commands without the bookkeeping.** `CommandBuilder` makes `If`, `Loop`, `Show Choices` and `Battle Processing` with their branches from nested functions, and sets the indents and the empty commands the way the editors do; validators say which command of a list is wrong.
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

#### Building commands with blocks

A list of commands is flat: an `If` with an `Else` is five commands in the right order, each with its indent, and every branch ends with an empty command. `CommandBuilder` takes the structure and makes the commands:

```csharp
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;

page.AppendCommands(b => b
    .Text("Hello", "again")                               // a text, one line for each string
    .IfSwitch(5,
        then: t => t.Text("The switch is on"),
        @else: e => e.Add(new WaitCommand { FrameCount = 30 }))
    .Choices(c => c
        .When("Yes", y => y.Text("Fine"))                 // a choice and what it runs
        .When("No")                                       // an empty branch
        .Cancel())                                        // Cancel is a branch (When Cancel)
    .Loop(l => l.IfSwitch(2, then: t => t.BreakLoop()))
    .Battle(3, win: w => w.Text("Won"), escape: _ => { }));
```

- `Add(command)` puts any command; `Text`, `ScrollingText`, `Comment` and `Script` make the first command and the lines that follow it. `Text` and `ScrollingText` cut each string at the line breaks (`\n`) and drop the `\r`, so `Text("a\r\nb")` has two lines.
- `If(condition, then, @else)` takes a condition command. The conditions have shortcuts: `IfSwitch`, `IfVariable`, `IfVariableWithVariable`, `IfSelfSwitch`, `IfScript`, `IfGold`, `IfItem`, `IfWeapon`, `IfArmor`, `IfTimer`, `IfEventFacing`, `IfVehicle`, the kinds of the condition about an actor (`IfActorInParty`, `IfActorName`, `IfActorClass`, `IfActorSkill`, `IfActorWeapon`, `IfActorArmor`, `IfActorState`) and about an enemy (`IfEnemyAppeared`, `IfEnemyState`). The Else branch is there when the function is given, even an empty one. The button condition is `new IfButtonCommand { Button = ... }`.
- `Choices` takes the choices together with their branches, so there is no branch without a choice. Without `Cancel()` the cancel is not allowed. The command with the position and the background can be given as the second argument.
- `Battle(troopId, win, escape, lose)`: the branches that are given decide what the battle can do (`escape` sets "Can Escape", `lose` sets "Continue Even When Loser"). The Win branch is there whenever there is another one.
- `MoveRoute(r => r.Down(2).Wait(30).TurnLeft().Speed(MoveChangeSpeedCommand.SpeedType.Fasterx2), eventId: -1, repeat: false, skippable: false, wait: true)`: Set Movement Route with the steps in the order they are walked (`Down`, `Left`, `Up`, `Forward(times)`, `Jump`, `Wait`, the turns, `SwitchOn`, `Speed`, `Frequency`, `Image`, `Opacity`, `Blend`, `PlaySe`, `Script` and the others, `Add` for any step). The empty command that ends every route is added, and the steps get the values the editor of the maker of the list gives a new one. The event is -1 for the player (the editors' default), 0 for the event that runs the commands.
- `Shop(s => s.Item(1).Weapon(4, price: 70).Armor(6), purchaseOnly: false)`: the goods in the order they are sold, the first one is the shop command and the others are the lines after it; without a price a thing is sold at the price of the database.
- `AppendCommands` puts the commands at the end of the list (before the empty command that ends it), `InsertCommands(index, ...)` before the command with the index (at the end of a block, into that block), `ReplaceCommands` instead of all of them. They set the empty commands and the indents of the whole list afterwards, so the list is as the editor makes it.
- The commands of the builder have the parameters that the editors write for the maker of the list: for example Cancel of `Show Choices` is `-1`/`-2` in MV and `0`/`5` in VX Ace, and `When Cancel` has `[6, null]` in MV and nothing in VX Ace.

#### Indents, empty commands and structure

The engines find the end of a branch by the indents (a branch is the commands that are deeper than its `If`) and the editors expect an empty command (code 0, the `@>` line) as the last command of every block: the list itself, each branch of `If`, the body of `Loop`, each `When` of `Show Choices`, each branch of `Battle Processing`. Both can be checked and set:

```csharp
IndentHelper.SetupIndents(page);                   // the indents from the blocks
EmptyCommandHelper.SetupEmptyCommands(page);       // adds the missing empty commands, removes the ones that end no block
IndentHelper.ValidateIndents(page);                // throws if an indent is not the one of its place
EmptyCommandHelper.ValidateEmptyCommands(page);    // throws if an empty command is missing or extra
IndentHelper.ValidateStructure(page);              // throws if the blocks are not closed or are mixed up
```

The checks throw `CommandIndentException`, `EmptyCommandException` and `CommandStructureException`. All of them derive from `CommandListException` and say which command is the first wrong one (`CommandIndex`, `CommandCode`). Loading does not check or change the commands; the text dump (`DumpHelper.Dump`) checks the indents.

#### Commands that differ between VX Ace and MV

The makers write some parameters differently. For these commands a property says what the parameter means in both, and the raw parameter is the one of the maker of the list the command is in (a command that is made in code and is not in a list yet has no maker, and gets the raw value when it is put into one):

| Command | Property | The raw parameter |
|---|---|---|
| Show Choices | `Cancel` (`Disallow`, `Choice`, `Branch`) and `CancelChoice` (from 0) | VX Ace: 0, n+1, 5; MV: -1, n, -2 (`CancelChoiceIndex`) |
| Change Equipment | `SlotIndex` (from 0) | VX Ace counts the slots from 0, MV from 1 (`TypeId`) |
| Get Location Info | `Type` (`InfoType.RegionI` for the region) | the region is 5 in VX Ace and 6 in MV |
| If, a button | `Button`, and `MvName` (null for the buttons X, Y and Z, which MV has no name for) | a number in VX Ace, a name in MV |

Some parameters exist in one maker only: the position, the background and the default choice of Show Choices, the type of Select Item and "Save Level" of Change Class (`SaveLevel`) are MV only, and the parameter "purchase only" belongs to the first line of a shop. In the other maker such a property throws `DoesNotExistInVersionException` (such a property is marked with `[DoesNotExistInVersion]`, and `HasProperty(nameof(...))` of `IHasOptionalProperties` tells it without calling the property: on a command, whose class is the same for both makers, the attribute names the makers, `[DoesNotExistInVersion(StorageVersion.VXAce)]`, and the answer is for the maker of the list that the command is in; the sound, the actor and the system have it too, where the class of VX Ace marks the property, such as the pan of a sound, with no makers); a command with it that is put into a list of VX Ace loses the parameter, so `Clone()` of a command of one maker into a list of the other keeps what the command means:

```csharp
var choices = vxPage.NamedCommands.OfType<ShowChoicesCommand>().First();
mvPage.AppendCommands(b => b.Add(choices.Clone()));   // Cancel, the choices... as MV writes them; the window gets what the editor of MV gives
```

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

### Without blocking the thread

The files can be read and written without blocking the thread, and several at the same time:

```csharp
await mv.PreloadAsync();                 // the tables at the same time, then the maps
await mv.SaveAllAsync();                 // the files at the same time
await mv.PreloadAsync(maxDegreeOfParallelism: 4);

await yaml.LoadAsync(@"C:\dump");        // the dump is read file by file
await Differ.DumpPatchesAsync(path, patches);
```

`maxDegreeOfParallelism` is how many files are handled at once; 0 is the number of processors. The result is the same as with the methods that block. On the data of a game of 217 maps, reading is about three times faster for MV and about two times for VX Ace; writing is about three times faster for MV. VX Ace files are written at the same speed, because RubyMarshalCS keeps its caches in static dictionaries and the Ruby objects are converted one at a time (the files themselves are read and written without blocking).

`IdMismatched` is raised from a thread of the pool while the tables are being loaded at the same time; the handlers are called one at a time.

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
| `Data/Helpers/` | walkers and formatters for maps, common events and troops; `CommandBuilder` and the checks and setters of indents, empty commands and block structure |
| `Diff/`, `Differ.cs` | text diff of two projects |
| `tests/RPGMDataKit.Tests/` | xUnit tests with the data of empty projects of both editors |
| `external/RubyMarshalCS/` | the Ruby Marshal library (submodule) |

## Tests

```bash
dotnet test tests/RPGMDataKit.Tests
```

The tests check the default values of the factories, the class hierarchies, saving and loading of the data of empty VX Ace and MV projects, that no members are lost on the way, and the blocks of commands: what the builder makes, indents, empty commands and block structure, for both versions.

## Notes

- Files of the editors are binary (`.rvdata2`) or JSON (MV). Keep a copy of a project before writing to it.
- This library does not include anything from the editors: no standard scripts, graphics or audio. The project files in `tests/` are the data files of empty projects, without the standard scripts.

## License

[MIT](LICENSE). Third-party code and dependencies: [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
