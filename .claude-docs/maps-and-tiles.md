# Maps, tiles and tilesets

How a map is stored in VX Ace and in MV, what a tile id means, what a tileset is, and what RPGMDataKit gives to work with all of it.
The facts are checked against: `rpg_core.js` / `rpg_objects.js` / `rpg_sprites.js` of MV 1.6.1, the standard scripts of VX Ace (dumped from a clean
project with `VXAceStorage.DumpScripts`), the library code, and the data of one full VX Ace game (216 maps, 9 tilesets) and its MV port.
What is a guess or was not checked is in [Not checked](#not-checked).

## Where the data is

| | VX Ace | MV |
|---|---|---|
| a map | `Data/Map001.rvdata2` (Ruby Marshal, `RPG::Map`) | `data/Map001.json` |
| the list of maps | `Data/MapInfos.rvdata2`: a hash id -> info (the id is the key, **not** in the entry) | `data/MapInfos.json`: an array, the index is the id (the entry has `id`) |
| tilesets | `Data/Tilesets.rvdata2` (`RPG::Tileset`) | `data/Tilesets.json` |
| tile sheets | `Graphics/Tilesets/*.png`, tile 32x32 | `img/tilesets/*.png`, tile 48x48 |

Library: `Storage.Maps` (`MapsHolder`: `Get(id)` / `GetAsync(id)` return `IMap?`, null when the file is missing; loaded maps are cached, `Unload()` drops them),
`Storage.MapInfos` (`DataLookupHolder<IMapInfo>`), `Storage.Tilesets` (`DataLookupHolder<ITileSet>`). Classes: `Data/VXAce/Classes/Map.cs`,
`Data/MV/Classes/Map.cs`, `Tileset.cs` next to them; the interfaces are in `Data/Structures/Interfaces` (`IMap`, `IMapInfo`, `ITileSet`).

## The tiles of a map: `IMap.Data`

`IMap.Data` is a [`Table`](../Data/Structures/Table.cs) of `Width x Height x layers`. Cells are stored **x first, then y, then z**:
`index = x + y * Width + z * Width * Height`. This is the same order the engines use (`$dataMap.data[(z * height + y) * width + x]` in MV,
`@map.data[x, y, z]` in VX Ace), so the table is the same for both makers and a cell is `Data[x, y, z]`.

The number of layers is **not** the same:

| z | VX Ace (4 layers) | MV (6 layers) |
|---|---|---|
| 0 | tile (ground; A1-A5 in the data that was looked at) | tile, lower layer 1 |
| 1 | tile (only A1/A2 in that data) | tile, lower layer 2 |
| 2 | tile (only B-E in that data) | tile, upper layer 1 |
| 3 | **shadows and regions in one number**: low bits = shadow quarters, `value >> 8` = region id | tile, upper layer 2 |
| 4 | - | shadows (bits 0-3, see below) |
| 5 | - | region id (the whole number) |

- Any tile id can sit on any tile layer; the data that was looked at only shows what the editor produced (layer 0: A1-A5 and empty, layer 1: A1/A2,
  layer 2: B-E). What the editors forbid was not checked.
- In MV the engine draws every tile layer the same way: a tile whose flag has the star bit (`0x10`, "above characters") goes to the layer that is
  drawn over characters, any other goes under them. Layers 0-3 differ only in the order of drawing (and in the order of the passage check).
- **Shadows**: four bits, one per quarter of the tile: bit 0 top left, bit 1 top right, bit 2 bottom left, bit 3 bottom right (`ShadowFlag`). The engine of MV reads only
  `value & 0x0F` (`Tilemap._drawShadow`).
- **Region ids**: MV `regionId = data[x, y, 5]`; VX Ace `regionId = data[x, y, 3] >> 8` (`Game_Map#region_id`). In that data the
  regions are 0, 1 and 2. The editors limit them (VX Ace 0-63, MV 0-255; not checked here). The `rvdata2` table keeps signed
  16-bit cells, so `region << 8` must stay below 32768.
- Tile ids are always `0` for "nothing"; the tile at `0` of sheet B is not a tile, it is the empty cell.
- After loading an MV map the library reshapes `Data` to `Width x Height x (cells / (Width * Height))` (`Map.OnDeserialized`), so `SizeZ` is 6 for a map
  that the editor of MV wrote and 4 for a VX Ace map. Read `SizeZ` instead of assuming the layers.
- A map that is made by `New<IMap>()` is 17x13 with the first tileset: `Table(17, 13, 4)` for VX Ace, 17x13x6 zeros for MV.

### Tile events

An event whose current page shows a tile (`Graphic.TileId > 0`) **and has the priority "below characters"** (`PriorityType == 0`) is a *tile event*
(`Game_CharacterBase#tile?` / `isTile`). It is drawn as a tile and takes part in the passage checks as one more tile on top of the layers
(`Game_Map#all_tiles` = tile events + layered tiles). `Graphic.TileId` is the same id as in the layers. More about events in [events.md](events.md).

## Tile ids (the same in VX Ace and MV)

A tile id is a number below 8192. The ranges (`Tilemap.TILE_ID_*` in MV, the same numbers in VX Ace):

| ids | sheet | count | kind |
|---|---|---|---|
| 0 | - | 1 | empty |
| 0-255 | B | 256 | normal tiles (ids 1-255 are real, 0 is empty) |
| 256-511 | C | 256 | normal |
| 512-767 | D | 256 | normal |
| 768-1023 | E | 256 | normal |
| 1024-1535 | - | 512 | not used (the flags still have cells for them) |
| 1536-1663 | A5 | 128 | normal tiles, used for the ground (the range goes to 2047, the rest is not used) |
| 2048-2815 | A1 | 768 | autotiles: 16 kinds x 48 shapes, animated water and waterfalls |
| 2816-4351 | A2 | 1536 | autotiles: 32 kinds x 48, ground |
| 4352-5887 | A3 | 1536 | autotiles: 32 kinds x 48, roofs and house walls |
| 5888-8191 | A4 | 2304 | autotiles: 48 kinds x 48, wall tops and wall sides |

- **An autotile** is a tile id of `2048` and above: `kind = (id - 2048) / 48`, `shape = (id - 2048) % 48`; `id = 2048 + kind * 48 + shape`.
  The shape says which of the neighbours are of the same kind (the editor picks it when the tile is painted). Kinds are numbered from A1:
  A1 0-15, A2 16-47, A3 48-79, A4 80-127.
- **A normal tile** (B-E, A5) sits at `sx = (floor(id / 128) % 2 * 8 + id % 8)`, `sy = floor(id % 256 / 8) % 16` (in tiles) of its sheet: ids 0-127 are
  the left half (8 columns), 128-255 the right half. The sheet B-E is 16x16 tiles, the sheet A5 is 8x16.
- `Spriteset_Battle` takes the battleback from the kind of the autotile under the player (`autotile_type`), see "Tileset mode".

### The kinds, as the engine of MV separates them

| kinds | what | how |
|---|---|---|
| A1 0, 1 | the sea, the deep sea | animated, 3 frames (the cycle 0, 1, 2, 1) |
| A1 2, 3 | not animated (the engine reads them from a fixed column of the sheet) | `Tilemap.isWaterTile` is false for them (no code of the core uses that function) |
| A1 4-15, even | water | animated like 0 and 1 |
| A1 4-15, odd | waterfalls | animated downwards (3 frames), 4 shapes (`WATERFALL_AUTOTILE_TABLE`) |
| A2 | ground (grass, soil, ...) | floor type; a tile with the counter flag (`0x80`) is a **table**: the engine draws an edge under it |
| A3, kind % 16 < 8 | roofs | wall type (16 shapes are used) |
| A3, kind % 16 >= 8 | the walls of houses | wall type |
| A4, kind % 16 < 8 | wall tops | floor type |
| A4, kind % 16 >= 8 | wall sides | wall type |

"Floor type" autotiles use 48 shapes (`FLOOR_AUTOTILE_TABLE`), "wall type" 16 (`WALL_AUTOTILE_TABLE`), waterfalls 4; a shape number beyond the table of its type draws nothing (`_drawAutotile` skips it).
The tiles of A3 and A4 are called "shadowing" in the engine: the shadow of a tile on the layer below is not drawn over them.

## Tilesets

A tileset (`ITileSet`: `Id`, `Name`, `Mode`, `TilesetNames`, `Flags`, `Note`) says which sheets a map uses and what every tile can do.

- **`TilesetNames`** has 9 entries, in this order: `A1, A2, A3, A4, A5, B, C, D, E` (the file names without extension, `""` for no sheet).
  In the engine of MV the sheet i becomes `Tilemap.bitmaps[i]`; the tiles of a missing sheet are not drawn.
- **`Flags`** is a one-dimensional table of 8192 numbers, one per tile id (see below).
- **`Mode`** (`TilesetMode`): `0` = field (world) type, `1` = area type. The engine uses it for one thing, `overworld?` / `isOverworld()`: when a map
  does not specify its own battle backgrounds, a map of mode 0 takes them from the terrain under the player (the autotile kinds of layers 1 and 0
  give "Wasteland", "DirtField", "Desert", "Lava", "Snowfield", "Clouds", "PoisonSwamp", "Forest", "Cliff"; on a vehicle "Ship"; otherwise
  "Grassland"), a map of mode 1 gets none. VX Ace has the same rule (`Spriteset_Battle#battleback1_name`), only some names of the backgrounds differ
  ("Forest1" in VX Ace, "Forest" in MV). The editors show the modes as "World/Field Type" and "Area Type".
- A new tileset: `ITileSet` is made by `New<ITileSet>()`: mode 1, nine empty names, and the flags the editor gives (VX Ace: `DefaultTilesetFlags`,
  8192 numbers; MV: one number, `0x0010`).

### The flags of a tile (`TileFlags`)

| bits | meaning |
|---|---|
| `0x0001` `0x0002` `0x0004` `0x0008` | down, left, right, up: **1 = blocked**. A character cannot leave the tile in that direction and cannot enter it from that side |
| `0x0010` | the star: no effect on passage, drawn above characters |
| `0x0020` | ladder |
| `0x0040` | bush |
| `0x0080` | counter (and, for A2, the "table" edge) |
| `0x0100` | damage floor |
| `0x0200` `0x0400` `0x0800` | `NoBoat` / `NoShip` / `NoAirshipLand`: **bit set = NOT allowed** (see [gotchas](gotchas.md)) |
| `0xF000` | terrain tag: `flags >> 12` (the editors offer 0-7) |

How the engine uses them (`Game_Map#check_passage`, the same in both makers): all tiles of the square are taken from the top (tile events, then the layers from the
highest), tiles with the star flag are skipped, and the first tile whose bit is clear means "passable", whose bit is set means "impassable". The terrain tag is
the first non-zero tag from the top; ladder, bush, counter and damage floor are true when any layer has the bit.

### Sheets

| sheet | tiles | VX Ace (32 px) | MV (48 px) |
|---|---|---|---|
| A1 | 16 x 12 | 512 x 384 | 768 x 576 |
| A2 | 16 x 12 | 512 x 384 | 768 x 576 |
| A3 | 16 x 8 | 512 x 256 | 768 x 384 |
| A4 | 16 x 15 | 512 x 480 | 768 x 720 |
| A5 | 8 x 16 | 256 x 512 | 384 x 768 |
| B, C, D, E | 16 x 16 | 512 x 512 | 768 x 768 |

These are the sizes of the stock projects and of the game that was looked at. A port that scales the graphics uses other sizes (at the double scale
the sheets are 1024 wide with a 64 px tile).

## VX Ace and MV in one place

| | VX Ace | MV |
|---|---|---|
| tile size | 32 | 48 |
| layers in `Data` | 4 (shadow and region together in z = 3) | 6 (shadow z = 4, region z = 5) |
| layers checked for passage | 3 (z = 2, 1, 0) | 4 (z = 3, 2, 1, 0) |
| the container | Ruby `Table`, cells signed 16-bit | a flat JSON array |
| tile ids, autotile kinds, flag bits | the same | the same |
| `MapInfos` | hash, id is the key | array, id is a field |
| a new map's parallax picture | hidden | shown (`ParallaxShow = true`) |

## What the library gives, and what it does not

- The raw table and the maps' other fields (`TilesetId`, `ParallaxName`, battlebacks, `Note`, encounters, BGM/BGS in the classes). `Table`: `this[x, y, z]`, `TryGet`, `Reshape`, `FromCells`; an index outside the dimensions throws.
- `TileFlags` (the bits above; the vehicle bits are `NoBoat`, `NoShip`, `NoAirshipLand`: a set bit means NOT allowed, see [gotchas](gotchas.md)) and `TilesetMode`.
- **`TileIds`** (`Data/Helpers/TileIds.cs`): the meaning of a tile id, the same functions as `Tilemap` of MV (checked against the engine for every id 0-8191): the ranges (`B`, `C`, `D`, `E`, `A5`, `A1`-`A4`, `Max`),
  `IsVisible`, `IsAutotile`, `Kind`, `Shape`, `MakeAutotile(kind, shape)`, `SheetOf(id)` (`TileSheet`, whose number is the place of the sheet in `TilesetNames`), `NormalTileCell(id)`
  (the column and row on the sheet), and the classes `IsA1`-`IsA5`, `IsWater`, `IsWaterfall`, `IsGround`, `IsShadowing`, `IsRoof`, `IsWallTop`, `IsWallSide`, `IsWall`, `IsFloorTypeAutotile`, `IsWallTypeAutotile`.
- **`MapTileExtensions`** (on `IMap`): `TileLayerCount()` (3 in VX Ace, 4 in MV), `GetTile` / `SetTile(x, y, layer, id)`, `LayeredTiles(x, y)` (top layer first, as the engine looks), `GetShadow` / `SetShadow` (0-15, or the quarters as `ShadowFlag`: `GetShadowFlags`, `SetShadow(x, y, ShadowFlag.TopLeft | ShadowFlag.BottomRight)`) and
  `GetRegion` / `SetRegion`. They hide where the shadow and the region are (one cell in VX Ace, two layers in MV). The maker is told by the class of the map. A VX Ace region is 0-127 (the cell is a signed 16-bit number).
- **`TilesetExtensions`** (on `ITileSet`): `SheetName` / `SetSheetName(TileSheet)`, `SheetNameOf(tileId)`, `GetFlags` / `SetFlags(tileId)` (a short table, as MV's editor writes for a new tileset, reads 0 past its end and
  grows when written), `HasFlags`, `GetTerrainTag` / `SetTerrainTag` (0-7), `BlocksDirection`.
- **Not there**: choosing the shape of an autotile from the neighbours (what the editors do while painting), a passage check over tile events and layers (`check_passage`), creating a map in a `Storage`
  (its id and its `MapInfos` entry are made by hand), resizing a map.

## Facts observed in the data of one full VX Ace game

- 216 maps, all with `Data.SizeZ = 4`; 9 tilesets, the first (mode 0) is the world map, the other eight have mode 1; every `Flags` has 8192 cells.
- Cells of layer 0: A1, A2, A3, A4, A5 and empty; layer 1: A1, A2 and empty; layer 2: B, C, D, E and empty.
- Layer 3: the low byte takes 0-15 (shadow quarters, 5 is the most common), the high byte (regions) takes 0, 1 and 2.

## Not checked

- How the real VX Ace draws the layers (its `Tilemap` is native code): the ids and flags are the same as in MV, the layer order is assumed to be the same.
- Which tiles the editors let a layer take (the numbers above only say what the editors did in one game).
- The range of the regions in the editors (VX Ace 0-63, MV 0-255, from memory) and of the terrain tags (0-7), and how the editors label the tileset modes.
- The meaning of A1 kinds 2 and 3 in the editor's terms (only what the engine does with them is known).
