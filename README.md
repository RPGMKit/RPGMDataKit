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

page.Append(b => b
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
- `MoveRoute(r => r.Down(2).Wait(30).TurnLeft().Speed(MoveChangeSpeedCommand.SpeedType.Fasterx2), eventId: -1, repeat: false, skippable: false, wait: true)`: Set Movement Route with the steps in the order they are walked (`Down`, `Left`, `Up`, `Forward(times)`, `Jump`, `Wait`, the turns, `SwitchOn`, `Speed`, `Frequency`, `Image`, `Opacity`, `Blend`, `PlaySe`, `Script` and the others, `Add` for any step). The empty command that ends every route is added, the line after the command for each step (the command 505, which the editors write and show the route from) is added too, and the steps get the values the editor of the maker of the list gives a new one. The event is -1 for the player (the editors' default), 0 for the event that runs the commands.
- `Shop(s => s.Item(1).Weapon(4, price: 70).Armor(6), purchaseOnly: false)`: the goods in the order they are sold, the first one is the shop command and the others are the lines after it; without a price a thing is sold at the price of the database.
- `Append` puts the commands at the end of the list (before the empty command that ends it), `InsertAt(index, ...)` in place of the command with the index, which goes down, as the editors do (the end of a block is its empty command, so give its index to put the commands at the end of the block; the length of the list is the end of it), `Replace` instead of all of them, `ReplaceAt(index, ...)` instead of the command with the index, `RemoveAt(index)`. Each of them but `RemoveAt` takes the function of the builder or commands that are made by hand (`page.Append(new WaitCommand())`, `page.ReplaceAt(3, new CommentCommand { Comment = "x" })`; the lines of a command of several lines are then to be given too). A command that is replaced or removed goes with all it holds: a text with its lines, an `If` with its branches up to `Branch End`, a movement route with its lines. The index of a line of a command of several lines (text, scrolling text, comment, script, shop, movement route) and of what divides or ends a block (`When`, `When Cancel`, `Branch End`, `Else`, `End Loop`, `If Win`...) is refused by all of them, and the empty command that ends a block or the list is refused by `ReplaceAt` and `RemoveAt`: a `CommandPlaceException` says where to go instead. They set the empty commands, the lines of the movement routes and the indents of the whole list afterwards, so the list is as the editor makes it.
- The commands of the builder have the parameters that the editors write for the maker of the list: for example Cancel of `Show Choices` is `-1`/`-2` in MV and `0`/`5` in VX Ace, and `When Cancel` has `[6, null]` in MV and nothing in VX Ace.

#### Indents, empty commands and structure

The engines find the end of a branch by the indents (a branch is the commands that are deeper than its `If`) and the editors expect an empty command (code 0, the `@>` line) as the last command of every block: the list itself, each branch of `If`, the body of `Loop`, each `When` of `Show Choices`, each branch of `Battle Processing`. Both can be checked and set:

```csharp
IndentHelper.SetupIndents(page);                   // the indents from the blocks
EmptyCommandHelper.SetupEmptyCommands(page);       // adds the missing empty commands, removes the ones that end no block
IndentHelper.ValidateIndents(page);                // throws if an indent is not the one of its place
EmptyCommandHelper.ValidateEmptyCommands(page);    // throws if an empty command is missing or extra
IndentHelper.ValidateStructure(page);              // throws if the blocks are not closed or are mixed up
RouteLinesHelper.SetupRouteLines(page);            // writes the lines (505) after each Set Movement Route again from its route
RouteLinesHelper.ValidateRouteLines(page);         // throws if the lines are not the steps of the route
```

The route of Set Movement Route is what counts (the engines run it from the command); the editors also write a line (the command 505) for each step of it and show the route from them, so after the steps of `SetMovementRouteCommand.Route` are changed the lines are written again by `SetupRouteLines`. A line that is changed by itself is written again from the route too.

The checks throw `CommandIndentException`, `EmptyCommandException`, `CommandStructureException` and `RouteLinesException`. All of them derive from `CommandListException` and say which command is the first wrong one (`CommandIndex`, `CommandCode`). Loading does not check or change the commands; the text dump (`DumpHelper.Dump`) checks the indents.

#### Commands that differ between VX Ace and MV

The makers write some parameters differently. For these commands a property says what the parameter means in both, and the raw parameter is the one of the maker of the list the command is in (a command that is made in code and is not in a list yet has no maker, and gets the raw value when it is put into one):

| Command | Property | The raw parameter |
|---|---|---|
| Show Choices | `Cancel` (`Disallow`, `Branch`, `Choice1` … `Choice6`; VX Ace has up to 4 choices) | VX Ace: 0, 5, n; MV: -1, -2, n-1 (`CancelChoiceIndex`) |
| Change Equipment | `SlotIndex` (from 0) | VX Ace counts the slots from 0, MV from 1 (`TypeId`) |
| Get Location Info | `Type` (`InfoType.Region` for the region) | the region is 5 in VX Ace and 6 in MV |
| If, a button | `Button`, and `MvName` (null for the buttons X, Y and Z, which MV has no name for) | a number in VX Ace, a name in MV |

Some parameters exist in one maker only: the position, the background and the default choice of Show Choices, the type of Select Item and "Save Level" of Change Class (`SaveLevel`) are MV only, and the parameter "purchase only" belongs to the first line of a shop. In the other maker such a property throws `DoesNotExistInVersionException` (such a property is marked with `[DoesNotExistInVersion]`, and `HasProperty(nameof(...))` of `IHasOptionalProperties` tells it without calling the property: on a command, whose class is the same for both makers, the attribute names the makers, `[DoesNotExistInVersion(StorageVersion.VXAce)]`, and the answer is for the maker of the list that the command is in; the sound, the actor and the system have it too, where the class of VX Ace marks the property, such as the pan of a sound, with no makers); a command with it that is put into a list of VX Ace loses the parameter, so `Clone()` of a command of one maker into a list of the other keeps what the command means:

```csharp
var choices = vxPage.NamedCommands.OfType<ShowChoicesCommand>().First();
mvPage.Append(b => b.Add(choices.Clone()));   // Cancel, the choices... as MV writes them; the window gets what the editor of MV gives
```

#### What a new command has

A command that is made in code and put into a list has the values that the editor of that maker gives a new command, not zeros and nulls: `new ChangeItemsCommand()` is for the item 1 and the count 1, `new WaitCommand()` waits for 60 frames, a Show Choices has the choices Yes and No and its Cancel is the second of them, a sound has the volume of the editor (80 for an SE and a BGS in VX Ace), a Tint Screen has a tone, a text is `""`. What is set in code stays, a zero too. Such a value is marked on the property: `[EditorDefault(60, StorageVersion.VXAce)]`; an attribute makes the value itself, so there are the ones for a tone (`[EditorDefaultTone(-34, 0, 68, 0, StorageVersion.VXAce)]`), a color and a list of texts (`[EditorDefaultList("Yes", "No")]`), and your own is a class of `EditorDefaultBaseAttribute` (the value was read from the files that the editors of both makers wrote; without the makers it is the same for both, and where they differ there is an attribute for each: the flash of Flash Screen has the alpha 255 in VX Ace and 170 in MV, the target of Force Action is Random in VX Ace and Last Target in MV). A copy made with `Clone()` has what the original has, not these values. What the editor takes from the project (the map and the music of a new Transfer Player or Change Battle BGM, the graphic of an actor) is not known to the library: set it in code.

### Create a map

```csharp
using RPGMDataKit.Data.Helpers;

var cave = vx.CreateMap("Cave", width: 30, height: 20, tilesetId: 2, parentId: 1);
cave.Map.SetTile(0, 0, 0, TileIds.MakeAutotile(16, 0));
vx.SaveAll();                                        // writes the new Map003 and the list of maps
// cave.Id is the number of the map, cave.Info its entry in the tree
```

`CreateMap` makes an empty map with the values that the editor gives to a new one, takes the next number and order after the other maps, and puts the map and its entry into the storage; `parentId` puts it under another map
of the tree (0 is the root). The tileset and the parent must exist.

A new event on a map:

```csharp
var door = cave.Map.CreateEvent(3, 4, "Door", b => b.Text("It is locked.").IfSwitch(5, then: t => t.Text("Open")));
var empty = cave.Map.CreateEvent(5, 6);              // EV002, one page that holds only the empty command
```

`CreateEvent` takes the number after the highest one on the map, sets `Id`, and names the event `EV001`, `EV002`... by it unless a name is given. The first page has the values of a new page of the maker; the optional
builder (see "Building commands with blocks") makes its commands. A square outside of the map, or one that has an event already, is refused. `map.ClearEvents()` removes all the events of the map (and numbering starts from 1 again). Further pages: `ev.Pages.Add(storage.New<IEventPage>())`.

The tree of maps has its own methods (`RPGMDataKit.Data.Helpers`):

```csharp
vx.GetChildMaps(1);                    // the numbers of the maps right under the map 1, in the order of the tree (0 is the root)
vx.SetMapParent(3, 5);                 // the map 3 with its children goes under the map 5, after the maps that are there (0 = to the root)
vx.GetMapsAtLevel(5);                  // the maps on the level of the map 5 (under the same parent), in order, the map 5 among them
vx.GetMapPositionAtLevel(5);           // (Index, Count): where the map 5 is on its level and how many places the level has (0 to Count - 1)
vx.ReorderMap(5, 0);                   // the map 5 becomes the first among the maps of its level (the place it has afterwards)
vx.ReorderMap(5, ^1);                  // ... the last one (^1 counts from the end, so the number of places need not be known)
vx.ExpandMap(1);                       // opens the map 1 in the editor's tree; recursive: true opens everything under it too
vx.CollapseMap(1, recursive: true);
```

`SetMapParent` refuses a move that would make a loop (a map under itself or under one of its own children) and does not open or close anything; opening a branch (`Expanded`) is a field of the editor and has no effect in the game.

### Tiles of a map

`IMap.Data` is a table of the maker's layout (4 layers in VX Ace, 6 in MV). The helpers in `RPGMDataKit.Data.Helpers` hide the difference:

```csharp
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;

var map = vx.Maps.Get(1)!;
var tileset = vx.Tilesets.Get(map.TilesetId)!;

int layers = map.TileLayerCount();                       // 3 in VX Ace, 4 in MV
map.SetTile(3, 2, 0, TileIds.MakeAutotile(16, 0));       // a tile of A2 on the lowest layer
map.SetRegion(3, 2, 5);                                  // the region, wherever the maker keeps it
map.SetShadow(3, 2, ShadowFlag.TopLeft | ShadowFlag.BottomRight);   // the dark quarters (or a number 0-15)

foreach (var id in map.LayeredTiles(3, 2))               // from the top layer down
{
    var sheet = TileIds.SheetOf(id);                     // TileSheet.A2, ... or null
    var flags = tileset.GetFlags(id);                    // the flags of the tile
    var name = tileset.SheetNameOf(id);                  // the file of the sheet it is cut from
}
```

An autotile has a shape (which of its neighbours are of the same kind) that the editors choose while a tile is painted. `PaintTile` does the same, so a map can be drawn by code:

```csharp
var grass = TileIds.MakeAutotile(16, 0);                 // the kind is what counts, the shape is chosen
for (var y = 2; y < 7; y++)
    for (var x = 3; x < 10; x++)
        map.PaintTile(x, y, 0, grass);                   // the tile, and the tiles round it are reshaped

map.PaintTile(5, 4, 0, 0);                               // erase: the tiles round it are reshaped too
map.PaintTileAuto(3, 2, TileIds.MakeAutotile(20, 0));    // the layer is chosen as the editor does: a bush (an overlay) goes over the ground on layer 1
map.PaintAutotileAuto(3, 2, 20);                          // the same by the kind of the autotile (A1-A4: 0-127, see TilesetSheetImage.Blocks); the shape is chosen
map.PaintTileAuto(3, 2, 88);                              // a tile of B-E goes to the top layer; a second one moves it down (MV)
map.SetTile(4, 2, 0, TileIds.MakeAutotile(80, 0));       // a wall (A3, A4): the shadow beside it is put as the editor does (the left half of the square to its right)
                                                         // (like the editor: putting a tile of A reworks the shadows of the square and the five squares round it, those drawn by hand there are lost)
int shadows = map.UpdateWallShadows();                   // all the shadows beside walls at once (after writing to Data); shadows drawn by hand of other shapes are not touched
int shape = map.AutotileShapeAt(3, 2, 0);                // the shape that a tile should have
int changed = map.UpdateAutotiles();                     // reshape every autotile of the map (after SetTile, or a map made by other code)
```

`TileIds` tells what a tile id is (the sheet, an autotile and its kind and shape, water, a wall, a roof...); the tileset helpers read and write the flags, the terrain tag and the names of the sheets.
What is behind them is in `.claude-docs/maps-and-tiles.md`.

### Pictures, sounds and movies of a project

`Storage` knows the folder of each kind of resource of the maker, so the code does not write `Graphics/Tilesets` or `img/tilesets` itself:

```csharp
using RPGMDataKit.Data.Enums;

var folder = vx.GetResourceFolder(ResourceType.Tileset);                       // <project>/Graphics/Tilesets  (MV: <project>/img/tilesets)
var path   = vx.GetResourcePath(ResourceType.Face, "Actor1", ".png");          // <project>/Graphics/Faces/Actor1.png
var file   = vx.FindResourceFile(ResourceType.Bgm, "Town");                    // the file that exists (any extension the maker reads, any case), or null

mv.SetResourceFolder(ResourceType.Tileset, "img/tilesets_game");               // another folder for one kind, for this storage
```

The built-in kinds are `ResourceType` (`Animation`, `Character`, `Face`, `Picture`, `Tileset`, `Bgm`, `Se`, `Movie`, ...); the sprites of the sideview battle exist in MV only.
A kind is also known by its name (`ResourceTypes.Names` maps each member to it; the case does not matter), and you can add kinds of your own:

```csharp
var same = vx.GetResourceFolder("tileset");                                    // the same as with ResourceType.Tileset

mv.RegisterResource("Portrait", "img/portraits");                              // a kind of your own (pictures of the maker unless extensions are given)
mv.RegisterResource("Voice", "audio/voice", [".ogg", ".m4a"]);
var voice = mv.FindResourceFile("VOICE", "Hero01");

vx.RegisterResource(ResourceType.SideViewActor, "Graphics/SV Actors");        // a built-in kind the maker lacks
foreach (var name in mv.ResourceNames) { /* Animation, Battleback1, ..., Portrait, Voice */ }
```

### A picture of a map

The project `Imaging` (`RPGMDataKit.Imaging`, which needs ImageSharp) draws the tiles of a map into a picture exactly as the engine does: the lower and the upper layers, the shadows, the autotiles, the edges of tables.
The sheets are taken from the project through the resource methods above.

```csharp
using RPGMDataKit.Imaging;

using var whole = vx.RenderMap(3);                                                              // the whole map 3
whole.SaveAsPng("map3.png");
using var part = vx.RenderMap(3, new MapImageOptions { Region = new MapRegion(10, 4, 20, 15) }); // only the squares x 10-29, y 4-18

// a grid with the coordinates of the squares (the tile ids: GridLabels.TileId, the regions: GridLabels.Region, or Label = (x, y) => "text")
using var grid = vx.RenderMap(3, new MapImageOptions { Grid = new MapGridOptions { Labels = GridLabels.Coordinates } });
```

A sheet of a tileset with the numbers of its tiles, to find the id of a tile (B, C, D, E, A5: the id of every tile) or the kind of an autotile (A1-A4: the kind and the id of the first shape of every block):

```csharp
using var sheet = mv.RenderTilesetSheet(2, TileSheet.B);      // the sheet B of the tileset 2 of the project, every tile with its id
sheet.SaveAsPng("b_ids.png");
var blocks = TilesetSheetImage.Blocks(TileSheet.A2);          // where the block of every kind is on the sheet, in tiles
int id = TilesetSheetImage.NormalTileId(TileSheet.B, 5, 11);  // the id of the tile in the column 5, row 11 of a sheet: 93
```

The texts of the grid need a font of the system (or `MapGridOptions.FontFamily`). The events and characters are not drawn. Details and how the picture was checked against the engine: `.claude-docs/imaging.md`.

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

`DumpOptions` selects the format of the commands: indents, command codes, extended format and the line numbers.

### Dump the commands as text or html

```csharp
await DumpHelper.Dump(@"C:\dump", storage, new DumpOptions { Format = DumpFormat.Html, ExpandCommonEvents = true });
```

`DumpHelper.Dump` writes a file for each common event, each page of a map event and each page of a troop (`.txt` for `DumpFormat.Text`, `.html` for `DumpFormat.Html`). An entity can also write
itself: `await page.Dump(writer, new EntityContext(storage), options)`; in html that is a whole document, not a part of one.

The html file is self-contained (the style and the script are inside it):

- a block (`If`, `Else`, a choice, a loop, a battle) folds; the lines are numbered by the order of the commands, with the number at the left edge and the arrow after it; "Open all" and "Close all" are on the top bar;
- the images of a command show in a tooltip, cut by the index in the sheet (the face of `Show Text`, the sprite and the face of `Change Actor Images`, the image step of a movement route, `Show Picture`, a vehicle,
  a parallax). A pill after the text of the command opens an image in a new tab;
- a command that plays a sound (BGM, BGS, ME, SE, the music of a battle, of a vehicle, the victory and defeat ME, the SE step of a route) has a pill that plays and stops it, with the volume and the pitch of the command;
- the top bar has the path of the project, which the pictures and sounds are taken from. It is the path the dump was made for and can be changed on the page ("Reset" brings it back); the change is kept by the browser;
- `Call Common Event` and `Exit Event Processing` have their own colors.

`DumpOptions.ExpandCommonEvents` (html only; the text format throws `NotSupportedException`) puts the called common events into the page, under the call, closed. Their lines are numbered from 000 as in their own dump.
The page then has the checkbox "Expand common events" (the browser remembers it); when it is off a call is a plain line. A call of an event that is open already (the one that is dumped, or one that calls it on the way)
is not expanded; it is marked, and a click goes to the line where the chain began.

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
