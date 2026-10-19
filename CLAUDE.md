# CLAUDE.md - RPGMDataKit

Entry point for agents. A C# library (net10.0) that reads and writes the data of RPG Maker VX Ace (`.rvdata2`) and MV (`.json`) projects
behind one set of interfaces. User documentation: [README.md](README.md). Deeper notes: [.claude-docs/index.md](.claude-docs/index.md).

## Documentation index
- [.claude-docs/index.md](.claude-docs/index.md) - what to read for which task
- [.claude-docs/commands.md](.claude-docs/commands.md) - how an event command works (views over raw commands, the life of a command made in code), adding a command
- [.claude-docs/command-formats.md](.claude-docs/command-formats.md) - the parameters of every command in VX Ace and MV, and the differences
- [.claude-docs/editor-defaults.md](.claude-docs/editor-defaults.md) - what a new command is given, per maker, and how it was found
- [.claude-docs/storage.md](.claude-docs/storage.md) - storages, holders, ids, type map, entities, serialization
- [.claude-docs/database.md](.claude-docs/database.md) - database records, fields per maker, features, effects, damage
- [.claude-docs/events.md](.claude-docs/events.md) - map events, pages, conditions, common events, troops
- [.claude-docs/testing.md](.claude-docs/testing.md) - tests, fixtures, checking a change on real projects
- [.claude-docs/maps-and-tiles.md](.claude-docs/maps-and-tiles.md) - map layers, tile ids, tilesets, flags, regions; VX Ace vs MV
- [.claude-docs/imaging.md](.claude-docs/imaging.md) - the picture of a map (`Imaging` project), how it is drawn and checked against the engine
- [.claude-docs/gotchas.md](.claude-docs/gotchas.md) - traps

## Commands
- `dotnet build`
- `dotnet test tests/RPGMDataKit.Tests` (it covers the `Imaging` project too, which is separate: `Imaging/RPGMDataKit.Imaging.csproj`, an image library lives there only)
- The Ruby Marshal reader is the submodule `external/RubyMarshalCS`: `git submodule update --init`.

## Boundaries
### MUST
- Keep the maker-specific classes (`Data/VXAce/Classes`, `Data/MV/Classes`) behind the interfaces in `Data/Structures/Interfaces`.
- A new thing that depends on the editor's behaviour gets its value from the editor, and the source is written next to it (see "What a new command has" in the README).
### MUST NOT
- Do not write a fact into the docs that was not checked; put it under "Not checked".
