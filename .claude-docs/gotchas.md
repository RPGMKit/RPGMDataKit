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
  too; MV does not mind (it reads `& 0x0F`), but the numbers look odd (256, 512...). The converter used to do that and masks the shadows now; use `GetShadow` / `GetRegion`.
- **The number of layers depends on the maker** (VX Ace 4, MV 6; `IMap.Data` says so). Use `TileLayerCount()` or read `Data.SizeZ`.
- **`rvdata2` tables hold signed 16-bit cells.** A value outside -32768..32767 makes saving throw. VX Ace region ids are small (`region << 8`
  fits), but never write an MV region of 128 or more into a VX Ace map.
- **`MapInfos`: the id is the key in VX Ace and a field in MV** (`IMapInfo.Id`). For VX Ace it is not saved with the entry.
- **`SetTile` does not reshape autotiles; `PaintTile` does.** A tile put with `SetTile` keeps the shape you gave it, and the tiles next to it keep theirs. The editors do not always reshape either, so a saved map has some shapes
  that `UpdateAutotiles()` would change (about 0.3 % of ground in an MV map, 3-4 % in a VX Ace one, 10 % of A4 wall sides).
- **A bush on layer 0 is not what the editor makes.** An overlay (the right half of A2: kinds 20-23, 28-31, 36-39, 44-47, and the deep sea) belongs on layer 1 over a ground on layer 0; put on layer 0 it replaces the ground and its transparent
  edges show nothing. `PaintTile(x, y, layer, id)` paints where you say, `PaintTileAuto(x, y, id)` chooses the layer as the editors do.
- **`SetTile` of a tile of A (A1-A5) clears the shadows of six squares and works them out again from the walls**, as the editor does: the square, above, below, to the right, upper right and lower right of it. A shadow drawn by hand
  on any of them is lost. Writing to `IMap.Data` directly does not do it; `UpdateWallShadows()` is the gentle one (it changes only an empty shadow and the wall shadow, 5, and leaves shadows of other shapes alone).
- **Autotile shape 47 is never made** (the editors use 46 for a tile with nothing round it), and the edge of the map is not the same for roofs and walls in the two makers (VX Ace joins the left and right, MV does not).
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
- **`MapInfo.Expanded` is the state of the editor's tree** (is the branch open); nothing in the game reads it. `CreateMap` and `SetMapParent` do not change it, `ExpandMap` / `CollapseMap` do.
- **`Maps.Set(id, map)` does not add a `MapInfos` entry** (the tree of maps would not show the map): use `Storage.CreateMap`, which makes both. `IMap.Events` has no `NextId`: take the id yourself.

## Resources and pictures (details in [storage.md](storage.md#resources-pictures-sounds-movies), [imaging.md](imaging.md))

- **A resource folder is not the same name in both makers** and a resource is found by `Storage.FindResourceFile`, not by `File.Exists(folder + name + ".png")`: the extensions differ (VX Ace reads jpg and bmp, MV png only) and the case of a name is
  not the case of the file on a file system that tells them apart.
- **`SetResourceFolder` is for one storage**; the converter's scaled sheets (`img/tilesets_game`) are found only after `storage.SetResourceFolder(ResourceType.Tileset, "img/tilesets_game")`.
- **`SetResourceFolder` does not add a kind.** A kind the project lacks (`SideViewActor` in VX Ace, or a name of your own) must be added with `RegisterResource` first; `SetResourceFolder` on it throws. `ResetResourceFolder` goes back to the folder the
  kind started with, and `UnregisterResource` works on registered kinds only.
- **A kind is found by name, the case ignored** (`"tileset"` = `ResourceType.Tileset`); an unknown name is an `ArgumentException`, but a built-in name the maker lacks is a `DoesNotExistInVersionException`.
- **`Imaging` is a separate project** (an image library lives there only); the data library excludes the folder (`Compile Remove="Imaging\\**"`), so a new file there is not compiled into the core. Do not move a type used by the core into it.
- **ImageSharp**: the project uses 3.1.12 like the converter, and NuGet warns about advisories (NU1902, NU1903). Version 4 builds without warnings in Debug but a Release build wants a Six Labors licence key.
- **The texts of the grid need a font of the system** (`SixLabors.ImageSharp.Drawing`); a container with no fonts (`fc-list` empty) must give `MapGridOptions.FontFamily`. The lines need none. `ImageSharp.Drawing` 2.1.7 is pinned
  because 2.2.0 wants ImageSharp 3.2.0 and 3.x wants ImageSharp 4 (a licence key).
- **A picture of a map has no events**; the engine draws the lower and the upper layers around the characters, which is why `Layers` can draw them apart.

## Events (details in [events.md](events.md))

- **The active page of a map event is the last one whose conditions hold**, not the first. A troop page with no condition set never starts.
- **A tile event is an event with a tile *and* priority "below characters"**; the same tile with another priority is just an event that is drawn as a tile.

## Database (details in [database.md](database.md))

- **Features (VX Ace) and traits (MV) are the same record under two names**; `IHasFeatures.Features` is read-only, edit the list of the maker's class (`Features` / `Traits`).
- **Equipment type ids**: VX Ace counts from 0 (weapon), MV from 1. A weapon with `ETypeId` 0 is fine in VX Ace and means "nothing" in MV.
- **Damage formulas are code**: JavaScript in MV, Ruby in VX Ace; they are not converted by the library.
- **The actor's profile** is `Profile` in MV and `Description` in VX Ace; `IconIndex` and `Description` exist in VX Ace's base class for every entity but the editor uses them for some only.
- **Class parameters**: a list of 8 lists in MV, a `Table` of 8 x 100 in VX Ace; index 0 of the level axis is a placeholder.
- **Party ability ids are 0-5** (six abilities), not 0-4 as the comment of `FeatureCodes.PartyAbility` used to say.
