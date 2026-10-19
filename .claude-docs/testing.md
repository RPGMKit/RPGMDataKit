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
| `ChangeClassCommandTests`, `ChangeEquipmentDescriptionTests`, `MoveCommandsTests` | single commands: parameters per maker, how they read in the dump |
| `CommandBuilderTests` | `CommandBuilder`: what it makes, as lists of `code@indent`; the blocks |
| `CommandPlaceTests`, `CommandReplaceRemoveTests` | `InsertAt`, `ReplaceAt`, `RemoveAt`: where commands go, the lines of a command with several lines, a block goes with all it holds |
| `IndentTests`, `EmptyCommandTests`, `CommandStructureTests`, `RouteLinesTests` | `IndentHelper`, `EmptyCommandHelper`, block structure checks and their typed exceptions, the lines (505) after Set Movement Route |

Tests of lists write them as strings of codes (`"101@0 401@0 102@0 ..."` = code and indent), which is the readable form of a flat list of commands.

## What is not tested here

- The graphics, audio, scripts and anything else that is not data (the library does not read them).
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
