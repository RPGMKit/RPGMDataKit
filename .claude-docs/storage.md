# Storage, holders, entities and serialization

How a project is opened, how its files become objects and back, which class stands for what, and the rules that keep a file the way the editor wrote it.
User-facing examples are in the [README](../README.md) (Usage); this page is the machinery behind them.

## The pieces

| Piece | What it is | Where |
|---|---|---|
| `Storage` | one project: the paths, one holder per data file, `New<T>()`, `Preload`, `SaveAll`, the YAML dump, the search over commands | `Storages/Abstraction/Storage.cs` |
| `VXAceStorage`, `MVStorage` | the two kinds; each lists its files in `Init()` and picks its serializer | `Storages/` |
| `TypeMap` | which class stands for each entity interface (`IActor` -> `Data/VXAce/Classes/Actor` or `Data/MV/Classes/Actor`); makes new entities | `Storages/TypeMap.cs` |
| `IDataSerializer` | reads and writes one file: `RubyMarshalDataSerializer` (`.rvdata2`), `JsonDataSerializer` (`.json`) | `Storages/Serialization` |
| a **holder** | one data file held in memory: `DataHolder<T>` (one object), `DataLookupHolder<T>` (a table by id), `MapsHolder` (the map files) | `Data/` , `MapsHolder.cs` |
| an **entity** | a record of the data: an interface in `Data/Structures/Interfaces` and one class per maker (`Data/VXAce/Classes`, `Data/MV/Classes`) | |

User code works with the **interfaces** (`IActor`, `IMap`, `ITileSet`, ...), not with the maker's classes.

## Opening a project

```csharp
var vx = new VXAceStorage(@"C:\Games\Game", dataDirectory: "Data");
var mv = new MVStorage(@"C:\Games\GameMV");   // data directory "data"
```

- The data directory is relative to the project path. **VX Ace's folder is `Data`, MV's is `data`** (the constructor's default is `"data"` for both): on a
  case-sensitive file system `new VXAceStorage(path)` looks for the wrong folder, so pass `"Data"` for VX Ace.
- Nothing is read in the constructor. A holder reads its file **the first time it is used** (`Get()`), and keeps the data; the lists are not re-read.
- The files per maker (the same names, the extension is `.rvdata2` / `.json`): `Actors`, `Animations`, `Armors`, `Classes`, `CommonEvents`, `Enemies`, `Items`,
  `MapInfos`, `Skills`, `States`, `System`, `Tilesets`, `Troops`, `Weapons`, and the maps `Map001` ...; VX Ace also has `Scripts` (`VXAceStorage.Scripts`).
- `Preload()` / `PreloadAsync()` read everything at once (the tables in parallel, then every map listed in `MapInfos`: the map numbers are the keys of the map infos, so those come first).
  `SaveAll()` / `SaveAllAsync()` write; `Unload()` drops what was read.
- **Only loaded holders are saved.** `Save()` of a holder that was never read does nothing (the file is not rewritten); `Maps.Save()` writes the loaded maps only.
  `Unload()` followed by `Save()` writes nothing.

## Holders and ids

- `DataHolder<T>`: `Get()`, `GetAsync()`, `Set(value)`, `Load()`, `Save()`, `Unload()`. `System` is one.
- `DataLookupHolder<T>`: a `Dictionary<int, T?>`. `Get(id)`, `Add(value)` (the next free id, from 1: slot 0 is not used), `Set(id, value)` (the entity gets the id),
  `Remove(id)` (other ids do not shift; the slot is saved as null), `NextId()`, `Clear()` (empties without reading the file).
- **The id of an entity is its place**: the index in a JSON array (MV) or in the Ruby array, the key in the Ruby hash (VX Ace `MapInfos`). The engines look an entity up
  by its place and take `entity.id` back from it, so the two must be equal. When a list is loaded and an entity's own `Id` differs from its place, `Storage.IdMismatched`
  is raised (subscribe before the table is first read, or call `Preload()` afterwards); the entity is left as it is, the place wins.
- A list is **saved** as an array as long as its highest id + 1, with `null` in the gaps; a dictionary as it is.
- VX Ace `MapInfos` is a hash whose entries do not keep their id: when it is loaded the key is put into the entity's `Id` (`idIsStored: false`). MV's `MapInfos` is a list
  whose entries have `id`.
- `MapsHolder`: `Get(id)` returns `IMap?` (null when the file does not exist), caches, and is safe from several tasks; `Set(id, map)` puts a map in (the entry in
  `MapInfos` is made by `Storage.CreateMap`, which does both); `Save(id)` writes one loaded map.
- Holders of your own (MV): `AddList<T>`, `AddDictionary<T>`, `AddEntity<T>` (README); the class must derive from `JsonEntity`, otherwise the members it does not describe are lost on save.
  `HolderNames`, `GetHolder<T>(name)` find any holder by the name of its file.

## Entities and the type map

- `TypeMap.VXAce` and `TypeMap.Mv` are built from the classes in `RPGMDataKit.Data.VXAce.Classes` / `...MV.Classes`: an interface is mapped to the **only** class that declares it
  there. Two classes for one interface make the entry ambiguous (an error when it is resolved). The built-in maps are read-only; `Clone().Add<IActor, MyActor>()` makes your own.
- `storage.New<T>()` (= `TypeMap.New<T>()`) makes an entity with **the values the editor gives to a new one**: the class is built by a compiled parameterless constructor, then `INewEntity.InitializeNew()`
  is called. A loaded entity is never initialized: the constructor leaves fields as they are (null when absent from the file), so the file keeps its shape.
- `TypeMap.Factory<T>()` gives the constructor only (used for raw commands, sounds, ...). `JsonSerializer` of the map creates the entities when MV files are read.
- The classes of the makers are the only place where the format is written down:
  - **VX Ace**: `RubySerializationEntity` with `[RubyObject("RPG::Actor")]` on the class and `[RubyProperty("@name")]` on properties. `Table`, `Color`, `Tone` have user serializers
    (`Data/VXAce/Serializers`); `Table` cells are 16-bit signed (see [maps-and-tiles.md](maps-and-tiles.md)).
  - **MV**: `JsonEntity` with `[JsonProperty("name")]` (`MemberSerialization.OptIn`: only marked members are written). Lists that the engine keeps as arrays with a leading `null` (database lists)
    are handled by the holder, `Table`s that are flat JSON arrays by `FlatTableJsonConverter`, a list of events by `ListToDictionaryJsonConverter`.
- `HasProperty(name)` (`IHasOptionalProperties`) is false for a property that one maker's class does not have (`[DoesNotExistInVersion]` without makers on an entity property:
  the pan of a sound in VX Ace). Properties that exist in only one maker are on the shared interface and throw `DoesNotExistInVersionException` when used on the other.

## Unknown members are kept

- **MV**: `JsonEntity` has `[JsonExtensionData]`: any member of the file that the class does not describe is kept and written back. (The editors and the plugins add members.)
- **VX Ace**: `RubySerializationEntity` has `[RubyExtensionData]` and the serializer's settings store unknown members and unknown user objects instead of failing
  (`MissingMember` / `UndefinedUserObject` -> `Store`, `UndefinedObject` -> `Ignore`). `RubyMarshalDataSerializer.Log` (null by default) receives a note for each.
- So a project read and written without changes comes back with the same members. Writing is **not byte-identical** in general (see the strings below), but it is the same data.

## Strings and encodings

- The editor of VX Ace writes a string as a Ruby string **without an encoding flag**; the library writes a .NET string as a Ruby string **with the UTF-8 flag**. Both read the same in the engine.
  This was looked at and left as it is.
- MV's JSON: the editor writes one entity per line (`[`, `null,`, `{...},` ..., and a map with its top-level members on separate lines); the library writes each file as **one line**
  (Newtonsoft's defaults, no indentation). MV reads both. The order of the members of an object follows the class, not the editor's file.

## Dump and comparison (short)

`storage.CreateYamlStorage().Dump(folder)` writes every holder of the storage as YAML (`Storages/Dump`), `Load(folder)` fills them back; commands are written as the lines the editor shows
(`GetParametersDescription`). `Differ` (`Differ.cs`, `Diff/`) compares two projects. Both are described in the README.

### The dump of commands (text, html)

`DumpHelper.Dump` and `IDumpable.Dump` write the commands of common events, map event pages and troop pages. The entity writes only its own header (`CommonEventHelper.DumpCommonEventInfo`,
`MapEventHelper.DumpPageInfo`, `TroopHelper.DumpPageInfo`) into a `StringWriter`, and hands it with the command list to `DumpWriters.For(options.Format).Write(...)` (`Data/Helpers/Dump`). A format is an `IDumpWriter`:
`TextDumpWriter` and `HtmlDumpWriter`; a format with no writer (`PatchFormat` is only a name) throws `NotSupportedException`. A new format is a class, a member of `DumpFormat` and a line in `DumpWriters`.
An option that a writer cannot honour is refused by the writer (`ExpandCommonEvents` in `TextDumpWriter`).

`HtmlDumpWriter` builds the blocks from the `Indent` of the commands, takes the text of a line from `NamedCommand.GetTextParts` (the pieces `AsString` joins), and finds the images and sounds of a command in `ImagesOf` /
`SoundsOf`, which switch on the type of the command: the commands do not know about html. The style and the script are the files `Data/Helpers/Dump/Html/dump.css` and `dump.js`, packed into the dll as the resources
`html-dump/<file>` (see `RPGMDataKit.csproj`). The header of the page is text, so html shows it as it is, in a `<pre>`.

Not checked (the html dump): the playing of the sounds and the choice of the frame of a sprite sheet (the middle frame of the top row, facing down) were not compared with the engine; the browser
cannot play `.mid` and `.wma`, which VX Ace projects have. The tooltip of the images was looked at in Chrome only.

## Resources (pictures, sounds, movies)

The files of a project that are not data are kept in a folder for each kind, named by the maker. `Storage` knows the names, so no code needs `Graphics/Tilesets` or `img/tilesets` written in it.
The kinds are `ResourceType` (`Data/Enums/ResourceType.cs`): `Animation`, `Battleback1`, `Battleback2`, `Battler`, `Character`, `Face`, `Parallax`, `Picture`, `SideViewActor` and `SideViewEnemy` (MV only),
`System`, `Tileset`, `Title1`, `Title2`, `Bgm`, `Bgs`, `Me`, `Se`, `Movie`.

A kind is **known by its name**, not case sensitive. `ResourceTypes.Names` (`Data/Enums/ResourceTypes.cs`) is the table enum member -> name (`Picture` -> `"Picture"`), `type.NameOf()` reads it and
`ResourceTypes.TryParse(name, out type)` goes back. Each storage keeps a list of the kinds it has: the ones the maker has (`ResourceFolderName`, an abstract method of `MVStorage` / `VXAceStorage`), then the ones
added by `RegisterResource`. Every method below has an overload for the enum (which only calls the one for the name), so the enum stays the way to name a built-in kind and a string is the way to name any.

| Method | What it gives |
|---|---|
| `HasResource(name)` / `HasResourceType(type)` | the project has the kind (the maker has it, or it was registered) |
| `ResourceNames` | the names of the kinds: the maker's in the order of `ResourceType`, then the registered ones in the order of registration |
| `GetResourceFolder(kind)` | the folder under `ProjectPath` (`Graphics/Tilesets`, `img/tilesets`, `Audio/BGM`, `audio/bgm`, `Movies`, `movies`...); not checked to exist |
| `GetResourcePath(kind, name, extension = "")` | the folder + the name as the data writes it (`Outside_A2`; it may have a folder in it: `Monsters/Slime`) + the extension; a name that is empty, rooted or leads out with `..` is refused |
| `GetResourceExtensions(kind)` | what the maker reads: pictures png, jpg, jpeg, bmp (VX Ace) / png (MV); sounds ogg, wav, mp3, wma, mid, midi (VX Ace) / ogg, m4a (MV); movies ogv, ogg (Ogg Theora) / webm, mp4; for a registered kind the ones given to it |
| `FindResourceFile(kind, name)` | the file that exists (the first extension that has one), the case of the name not mattering, or null |
| `SetResourceFolder(kind, folder)` / `ResetResourceFolder(kind)` | put another folder (under the project, either separator, or a whole path) in place of the current one for one kind, for this storage only; Reset gives back the folder the kind started with (the maker's, or the one it was registered with) |
| `RegisterResource(name, folder, extensions = null)` / `RegisterResource(type, folder, extensions = null)` | add a kind of your own, or a built-in one the maker lacks (`SideViewActor` in VX Ace). Without extensions the files are pictures of the maker. For this storage only |
| `UnregisterResource(name)` | take away a registered kind; a kind of the maker cannot be taken away |

What a bad name does: a name that is not a kind at all is an `ArgumentException` (it lists the kinds there are); a name of a built-in kind that the maker lacks is a `DoesNotExistInVersionException`.
`SetResourceFolder` on a kind that does not exist throws too: it does not add one, `RegisterResource` does. A registered name may not be empty, have a separator, or repeat (in any case) a kind the project has already.

The folders were checked against the stock projects of both editors (the same names are in `ResourceTests`). A project that is encrypted or packed (`Game.rgss3a`, `.rpgmvp`) has no files to find.

Where the extensions come from:
- MV: read from the engine scripts of an MV project (pictures `.png` in `ImageManager`; sounds `.ogg`, `.m4a` in `AudioManager.audioFileExt`; movies `.webm`, `.mp4` in `Game_Interpreter.videoFileExt`).
- VX Ace pictures (`.png .jpg .jpeg .bmp`): checked by the maintainer in the editor.
- VX Ace sounds: a test tone in each of ogg, wav, mp3, wma, mid, midi played in the editor's resource manager; `.m4a` and `.flac` did not, so they are not in the list.

- VX Ace movies: a 3-second test video in each of Theora/Vorbis as `.ogv`, the same as `.ogg`, VP8/Vorbis `.webm` and H.264/AAC `.mp4` was played in the game: `.ogv` and `.ogg` played, `.webm` and `.mp4` crashed the game. So the container and codec count (Ogg Theora), and `.ogg` is read as a movie too. Theora must be 4:2:0 (`-pix_fmt yuv420p` in ffmpeg): the default 4:4:4 played with broken colours.

Not checked: whether the game player reads every sound and picture extension the editor accepts (the sounds and pictures were tried in the editor), and how an MV movie that has the other extension behaves (the engine script only picks `.webm`, else `.mp4`).

## Traps

- A holder that was not read is not saved: if you edit through `LoadData<T>(file)` yourself, you save yourself.
- `Preload()` reads **all** maps; on a big project it takes a while, prefer `PreloadAsync()`.
- `DataLookupHolder.Get(id)` returns `null` both for an id that is absent and for a slot that is saved as null; `Get()` (the dictionary) tells them apart.
- The Ruby marshal library is the submodule `external/RubyMarshalCS`; the project does not build without it (`git submodule update --init`).
