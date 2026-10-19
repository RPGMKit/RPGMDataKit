# Gotchas

Things in the library or the formats that look wrong or surprising. Add a line when you hit one.

## Maps and tiles (details in [maps-and-tiles.md](maps-and-tiles.md))

- **The vehicle bits (`0x200` boat, `0x400` ship, `0x800` airship landing) are "blocked" bits: a set bit means NOT allowed.** Like the four
  directions (`0x1`-`0x8`), and both engines test them the same way (`check_passage(x, y, bit)` says "passable" when the bit is **clear**).
  The members were named `BoatPassable`, `ShipPassable`, `AirhsipLandOk` (the engine's questions, the opposite of what the bit does); they are now
  `NoBoat`, `NoShip`, `NoAirshipLand`, as `PassNo*` for the directions. Checked on the flags of a stock tileset: the sea is `0x080F` (boat and ship
  may enter, an airship may not land), the deep sea `0x0A0F` (only the ship), the ground `0x0600` (an airship lands, the boat and the ship cannot),
  a wall `0x0E0F` (nothing).
- **`TileFlags.PassNo*` (`0x1` down, `0x2` left, `0x4` right, `0x8` up)**: 1 = blocked.
- **One number holds two things in VX Ace.** Layer 3 of a VX Ace map is the shadow (low bits, `& 0x0F` is what MV reads) *and* the region
  (`value >> 8`). MV keeps them in layers 4 and 5. Code that copies "the last layer" between makers copies the region into the shadow layer
  too; MV does not mind (it reads `& 0x0F`), but the numbers look odd (256, 512...).
- **The number of layers depends on the maker** (VX Ace 4, MV 6). The comment on `IMap.Data` describes the MV layout. Read `Data.SizeZ`.
- **`rvdata2` tables hold signed 16-bit cells.** A value outside -32768..32767 makes saving throw. VX Ace region ids are small (`region << 8`
  fits), but never write an MV region of 128 or more into a VX Ace map.
- **`MapInfos`: the id is the key in VX Ace and a field in MV** (`IMapInfo.Id`). For VX Ace it is not saved with the entry.
- **Tileset `Mode` 0 is the field (world) type**, 1 the area type; it matters only for the battle backgrounds of a map that has none of its own.
- **Ids 1024-1535 and the tail of the A5 range (1664-2047) are unused**, but the `Flags` table of a tileset still has a cell for every id up to 8191.

## Commands (details in [commands.md](commands.md))

- **A default is applied before `OnBinding`.** In `OnBinding` the value of a defaulted property is already there, in the neutral form. A property that converts by
  maker (`ShowChoicesCommand.Cancel`) must get its default in `OnBinding`, not from an attribute, or it is converted twice.
- **"Set in code" means "non-null".** `IsParameterSet(i)` is `Parameters[i] != null`. A property of a reference type left at `null` is taken as not set; `0`, `false` and `""` are set.
- **A loaded command is never touched** by defaults, `OnBinding` or completion; only a command made in code is. So a file can hold what the library would not write (the 2-parameter Show
  Battle Animation of an old MV editor reads as `EntireTroop = false`, a clone of it has 3 parameters).
- **A `Clone()` has no editor defaults**; it has what the original has. Put into a list of the other maker, the parameters that maker does not have are dropped (`HasProperty`).
- **Getters are called by the machinery** (completion, copy, `Materialize`): a getter must not change data. `IncludeEquipment` of Change Weapons / Armors answers `false` unless the operator is Take, while its setter writes the value as it is.
- **`When Cancel` (403)** is written with `[6, null]` for MV and with no parameters for VX Ace. The engines ignore the parameters (another tool's output had an empty list for MV and played the same).
- **A command that the other maker does not have throws on binding** (`ChangeDefeatME`, `ChangeVehicleBGM` in VX Ace), as a blend mode `Screen` does in a VX Ace list. The command is made fine while it is detached.

## Storage and files (details in [storage.md](storage.md))

- **The data folder of VX Ace is `Data`, the constructors default to `"data"`.** On a case-sensitive file system `new VXAceStorage(path)` looks in the wrong folder; pass `"Data"`.
- **Only loaded holders are saved.** A table that was never read is not rewritten; after `Unload()` a `Save()` writes nothing.
- **The id of an entity is its place** (index or key), not the `id` written in it. A mismatch raises `Storage.IdMismatched`; the place wins.
- **`Preload()` reads every map.** Use `PreloadAsync()` on a big project.
- **MV files are written as one line**; the editor writes one entity per line. MV reads both, but a text diff against an editor-saved file shows every line changed.
- **A new map or event needs its place made by hand**: `Maps.Set(id, map)` does not add a `MapInfos` entry, and `IMap.Events` has no `NextId`.

## Events (details in [events.md](events.md))

- **The active page of a map event is the last one whose conditions hold**, not the first. A troop page with no condition set never starts.
- **A tile event is an event with a tile *and* priority "below characters"**; the same tile with another priority is just an event that is drawn as a tile.
