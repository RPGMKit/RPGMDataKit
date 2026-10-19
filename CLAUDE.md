# CLAUDE.md - RPGMDataKit

Entry point for agents. A C# library (net10.0) that reads and writes the data of RPG Maker VX Ace (`.rvdata2`) and MV (`.json`) projects
behind one set of interfaces. User documentation: [README.md](README.md). Deeper notes: [.claude-docs/index.md](.claude-docs/index.md).

## Documentation index
- [.claude-docs/index.md](.claude-docs/index.md) - what to read for which task
- [.claude-docs/maps-and-tiles.md](.claude-docs/maps-and-tiles.md) - map layers, tile ids, tilesets, flags, regions; VX Ace vs MV
- [.claude-docs/gotchas.md](.claude-docs/gotchas.md) - traps

## Commands
- `dotnet build`
- `dotnet test tests/RPGMDataKit.Tests`
- The Ruby Marshal reader is the submodule `external/RubyMarshalCS`: `git submodule update --init`.

## Boundaries
### MUST
- Keep the maker-specific classes (`Data/VXAce/Classes`, `Data/MV/Classes`) behind the interfaces in `Data/Structures/Interfaces`.
- A new thing that depends on the editor's behaviour gets its value from the editor, and the source is written next to it (see "What a new command has" in the README).
### MUST NOT
- Do not write a fact into the docs that was not checked; put it under "Not checked".
