# Tests, fixtures, and checking a change on real data

## Running

```bash
dotnet test tests/RPGMDataKit.Tests
```

xUnit, net10.0. The project copies `tests/RPGMDataKit.Tests/Projects/**` to the output folder. Tests that write files use a temporary folder and delete it.

## The fixtures

`tests/RPGMDataKit.Tests/Projects/Mv/data` and `.../VXAce/Data` are the **data files of an empty project of each editor** (no graphics, no audio, and for VX Ace
no standard scripts: they are not ours to keep). The VX Ace tests put an empty `Scripts.rvdata2` in a copy of the folder. The projects hold a few database
entries, two maps and a few (almost empty) common events, so the tests read what an editor really wrote.

## The helper `Version` (`Versions.cs`)

`Version.Mv` and `Version.VXAce` carry the type map and the serializer of a maker; `Version.All` is both. Most tests are `[Theory]` over `Version.All`, so every behaviour is
checked for both makers (`[MemberData(nameof(Versions))]`). `Version.New<T>()` makes a new entity as the library does; `FactoryInterfaces()` lists the entity interfaces whose class knows
the values of a new entity (`INewEntity`).

## What each file covers

| File | Covers |
|---|---|
| `ProjectFilesTests` | an empty project of each editor is loaded and saved: nothing is lost, the only members the classes do not describe are the ones the editors leave behind (the `LeftOvers` list) |
| `RoundTripTests` | a new entity is saved, loaded and saved again: the file is the same, no member is lost and none sits in the unknown fields |
| `FactoryDefaultsTests` | the values the editors give to a new entity (`New<T>()`), compared with the editors on real projects |
| `HierarchyTests` | what the database entities share in both makers |
| `HasPropertyTests` | `HasProperty` for properties that one maker lacks (the pan of a sound, the sideview battler, the equipment types) |
| `CustomHolderTests` | tables of your own (`AddList`, `AddDictionary`, `AddEntity`), ids and `IdMismatched`, the unknown members |
| `AsyncTests` | the non-blocking methods give the same files as the blocking ones |
| `EditorDefaultTests` | the defaults of new commands per maker, by values and by reflection over every attribute (see [editor-defaults.md](editor-defaults.md)) |
| `CommandMeaningTests` | commands that the makers write differently: the property says the meaning, the raw parameter is the maker's |
| `ShowChoicesCancelTests` | Show Choices: the cancel, the MV-only parameters, the defaults |
| `HtmlDumpIndexTests` | the index of an html dump: what it lists and links, the index written by `DumpHelper.Dump`, no index for a text dump |
| `HtmlDumpTests` | the html dump: folding, numbers of lines (`data-line`), escaping, images and sounds of commands, the common events expanded and the recursion, the text dump left as it was |
| `ChangeClassCommandTests`, `ChangeEquipmentDescriptionTests`, `MoveCommandsTests` | single commands: parameters per maker, how they read in the dump |
| `CommandBuilderTests` | `CommandBuilder`: what it makes, as lists of `code@indent`; the blocks |
| `CommandPlaceTests`, `CommandReplaceRemoveTests` | `InsertAt`, `ReplaceAt`, `RemoveAt`: where commands go, the lines of a command with several lines, a block goes with all it holds |
| `IndentTests`, `EmptyCommandTests`, `CommandStructureTests`, `RouteLinesTests` | `IndentHelper`, `EmptyCommandHelper`, block structure checks and their typed exceptions, the lines (505) after Set Movement Route |
| `TileTests` | `TileIds` (the ranges, kinds and shapes of autotiles, the classes of tiles), the layers, shadows (`ShadowFlag`) and regions of a map of either maker, the sheets and flags of a tileset |
| `AutotileTests` | the shape of an autotile from its neighbours: the functions of `AutotileShapes` (all 256 combinations give the 47 shapes), what is connected on a map (same kind, the edge of the map per maker, water and waterfall, wall side and wall top), `PaintTile` and `UpdateAutotiles` |
| `TilesetSheetImageTests` | the sheet with the numbers of its tiles: the ids of B-E and A5, the blocks of A1-A4 where the engine cuts them (and that they do not overlap), the picture over the background, the grid and the numbers |
| `MapShadowTests` | the shadow beside walls: the walls and shadows of a map painted in the editor of MV, a roof or a wall, any layer, painting and removing a wall (`SetTile`, `PaintTile`, `PaintTileAuto`), a tile of A reworks the shadows of six squares (and clears those drawn by hand there), the explicit `UpdateWallShadows` leaves other shapes alone, the region that shares the cell in VX Ace |
| `MapPaintTests` | `PaintTileAuto`: the layer chosen for the ground, an overlay and B-E in MV and VX Ace; the cases painted in the MV editor (see [maps-and-tiles.md](maps-and-tiles.md)); `TileIds.IsOverlay` |
| `MapCreationTests` | `Storage.CreateMap`: the number, the order, the size, the values of a new map, saving and loading it |
| `MapEventTests` | `IMap.CreateEvent`: the id, the name, the first page with and without a builder, refused squares, saving and loading |
| `MapTreeTests` | the tree of maps: `GetChildMaps`, `SetMapParent` (and the loop it refuses), `ReorderMap`, `GetMapsAtLevel`, `GetMapPositionAtLevel`, `ExpandMap` / `CollapseMap` |
| `ResourceTests` | the folders of every `ResourceType` per maker, the table of names, access by name in any case, registering / unregistering kinds, the paths, the extensions, finding a file in any case, replacing the folder of a kind |
| `MapRendererTests` (+ `TileScenario`) | the picture of a map (`Imaging`): equal to the hash of the picture the engine draws (see [imaging.md](imaging.md)), a part of the map, the layers, shadows, frames of the water, sheets in the project, the grid (lines pixel by pixel; the texts: where they are written; those tests return early on a machine with no system font) |

Tests of lists write them as strings of codes (`"101@0 401@0 102@0 ..."` = code and indent), which is the readable form of a flat list of commands.

## What is not tested here

- The audio and scripts, and the graphics beyond the tiles of a map (`Imaging` draws those from made-up sheets and, once, from real ones; see [imaging.md](imaging.md)).
- Real games: there are none in the repository. Use the checks below.
- The UI of the editors: the defaults are read from the files the editors wrote ([editor-defaults.md](editor-defaults.md)).

## Checking a change on real data

A change to the command classes, the serializers or the entities can change what is written to files of real projects. The unit tests do not see that, so before a commit that touches them:

1. **The unit tests** of this repository, and the tests of the converter that uses the library (`RPGMResourceConverter`, its own `dotnet test`).
2. **A before/after of the converter's data**: take the library as it was before the change (`git archive <commit> | tar -x -C <folder>`, with the submodule copied), build a small program that calls
   `DataConverter.Run(sourceProject, "Data", targetFolder, write: true)` against it and against the current library, run both on the same VX Ace projects, and compare the `data` folders.
   Compare as JSON, not as bytes, and ignore `"indent": null` in the steps of a movement route (it is the only legal difference seen between two writers). Expect them to be identical.
3. **A clone check**: load a project, and for every command of every list make `Clone()`, put the clone into a list of the same maker, and compare the raw parameters with the original's.
   A difference means the machinery changed a copy (a default, a dropped parameter). Known and accepted: a command that has fewer parameters than the library writes (the old 2-parameter Show Battle Animation).
4. **A look at the dump**: `storage.CreateYamlStorage().Dump(folder)` before and after, and a diff of the folders.

The programs for 2 and 3 are throw-away: a console project that references `RPGMDataKit.csproj` and walks `CommonEvents` and `MapEventHelper.WalkMapEventPages`.
Build them with `--artifacts-path` outside the repository so that the build output does not end up in it.

## Conventions

- A new behaviour that differs by maker gets a `[Theory]` over `Version.All` or two `[Fact]`s with the values for each.
- Do not put a game's files or names into the repository: tests read the empty projects in `Projects/`, and the docs say "a game that was looked at".
- A test that reads a number from an editor says in its summary which editor and what was done to get it.
