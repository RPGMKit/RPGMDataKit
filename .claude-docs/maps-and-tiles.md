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
- **`CreateMap`** (`MapCreationExtensions`, on `Storage`): a new map and its `MapInfos` entry, with the next number and order, the size and the tileset you give, a place in the tree (`parentId`), and the other values of a
  new map of the maker. The numbers were checked against the maps that the editors made in the test projects: the next number, the order after the others, 17 x 13, the first tileset, and the view of the editor
  (`ScrollX`, `ScrollY`) in the middle of the map (VX Ace: 272, 208 = 17 x 13 tiles of 32 px, halved; the same rule with 48 px is used for MV, it is a field of the editor and has no effect in the game).
- **The tree of maps** (`MapTreeExtensions`, on `Storage`): `GetChildMaps(parentId)` (the order of the tree: `Order`, then the number), `SetMapParent(mapId, newParentId)` (under another map or the root, after the maps there, with its
  children; refuses a loop; it does nothing when the map is already under that map), `GetMapsAtLevel(mapId)` (the maps of its level in order, itself among them) and `GetMapPositionAtLevel(mapId)` (`Index` and `Count`: the places are `0` to `Count - 1`), `ReorderMap(mapId, index)` (the place among the maps of its level,
  0 is the first, `^1` the last: the parameter is a `System.Index`, a number converts to it; the map and the maps it passes exchange the `Order` numbers they already have, so the other levels
  are untouched; a level whose numbers are not all different gets new ones above all), `ExpandMap` / `CollapseMap(mapId, recursive)` (the `Expanded` field of the editor's tree; it is separate from `CreateMap` and from `SetMapParent`).
  `Order` is one number for the whole tree: the maps of a level go by it, and a map put last gets the highest number of all.
- **Autotile shapes** (`AutotileShapes`, `MapAutotileExtensions`): the shape that the editors give an autotile from what is round it (the engines only read it from the id). `AutotileShapes.Floor(up, down, left, right, upLeft, upRight, downLeft, downRight)`
  (0-46), `.Wall(up, down, left, right)` (0-15), `.Waterfall(left, right)` (0-3) are the pure functions; on a map: `AutotileShapeAt(x, y, layer)`, `UpdateAutotile(x, y, layer)`, `UpdateAutotiles()` / `UpdateAutotiles(x, y, width, height)`
  (the number of tiles that changed) and `PaintTile(x, y, layer, tileId)` (puts the tile, takes the shape for an autotile from its neighbours, and reshapes the 8 squares round it on the same layer; use it in place of `SetTile` when drawing).
- **Painting on the layer the editors choose** (`MapPaintExtensions.PaintTileAuto(x, y, tileId)`): see "Which layer a tile goes to" below. `PaintAutotileAuto(x, y, kind)` is the same for an autotile of A1-A4 by its kind
  (the shape in an id is not used anyway); everything else (B-E, A5) goes through `PaintTileAuto` with its id. `PaintTile(x, y, layer, tileId)` paints on a layer you give.
- **Not there**: a passage check over tile events and layers (`check_passage`), resizing a map that has tiles and events.

### The shadow beside walls

Tiles of A3 and A4 (roofs, house walls, tops and sides of walls: all of them alike) cast a shadow on the ground beside them, and the editors put it into the shadow layer themselves when the tile is painted.
`MapShadowExtensions` does the same: `HasWall(x, y)` (a tile of A3 or A4 on any layer), `WallShadowAt(x, y)`, `UpdateWallShadow(x, y)`, `UpdateWallShadows()` / `UpdateWallShadows(x, y, width, height)`.
**`SetTile` keeps the shadows itself, as the editor does** (so `PaintTile` and `PaintTileAuto`, which use it, do too). What the editor of MV does when a tile of A (A1-A5) is put on a square, as the maintainer found: the shadows of
**six squares** (the square itself, the ones above and below it, to its right, to its upper right and to its lower right) are cleared completely and worked out again from the wall rule alone; a shadow drawn by hand on any of them is lost.
`SetTile` does the same for a tile of A, and also when a tile that is not of A takes a wall away (the walls round those squares are not what they were). A tile of B-E that takes no wall away changes no shadow.
Writing to `IMap.Data` directly skips all this.

**The explicit methods are gentler.** `UpdateWallShadow(x, y)` / `UpdateWallShadows()` (the whole map, or a part) change only an empty shadow and the wall shadow itself (the value 5); a shadow of another shape drawn by hand
(a quarter, the right half, the whole square) is left alone, also where the rule would put a wall shadow. Use them after writing to `Data`; they cannot tell a left half drawn by hand from a wall's.

**The rule** (written by the maintainer from the editor of MV, and checked: a map painted in it with 13 shadowed squares and walls in every arrangement got the same shadows from the rule in all 13 squares and none in the others):
- the shadow is always the **left half of the square** (the top left and the bottom left quarters: the value 5; never one quarter, never the right ones);
- a square that has a tile of A3 or A4 on any layer gets none;
- any other square gets it when the square to its **left** and the square to its **up-left** (the diagonal) both have a tile of A3 or A4 on some layer. So the top row of a wall casts nothing beside it: the shadow starts one row lower.

The maintainer says that the editor of VX Ace has the same rules (the shadow, and the six squares recalculated when a tile of A is put).

Against the maps of two finished games (all the squares with the shadow 5): VX Ace 89.5 % of the squares that the rule shadows really have the shadow, and the rule finds 83 % of the squares that have it; MV 41 % and 82 %.
The rest are shadows drawn by hand with the shadow pen (the value 5 and others) and, in MV, walls that have no shadow beside them in the saved map (the author's choice, or the editor of an earlier version).
The shadows of other shapes (a single quarter, the right half, 15, ...) are not made by this rule; `SetShadow` writes them.

### Which layer a tile goes to

The editors do not leave this to the painter: with the layer on Auto the layer is chosen from the tile (and from what is on the square). `IsOverlay(id)` (`TileIds`) tells the two kinds of A tiles apart: **the ground** (A1, the left half of A2
(columns 0-3 of the sheet, kinds 16-19, 24-27, 32-35, 40-43), A3, A4, A5) and **overlays** (the right half of A2: columns 4-7, kinds 20-23, 28-31, 36-39, 44-47, bushes, tall grass, cloth; and the deep sea, A1 kind 1).
How the kinds are counted: see "The kinds" above; the number of a block of the A2 sheet is `16 + row * 8 + column`.

| tile | MV (4 layers) | VX Ace (3 layers) |
|---|---|---|
| ground (A1-A5; a wall of A4, a roof of A3 and A5 as well) | layer 0, and **everything else on the square is removed** (the overlay and the B-E tiles) | layer 0, the same |
| overlay | layer 1, in place of the overlay that was there; the ground under it stays (and it is on layer 1 even when layer 0 is empty); **the B-E tiles are removed** | layer 1, the same |
| B-E | the top layer (3); the tile that was on it moves down to layer 2, the one that was on layer 2 is lost; the same tile as the one on top is not added again | layer 2, in place of the one that was there |

Where it comes from:
- **MV** was painted in the editor of MV 1.6 with the layer on Auto, in cases like these (the second tile was painted on the square of the first one, and so on): grass; grass twice; bush (an overlay); bush twice; grass then bush; bush then grass
  (the bush is gone); a bush and then another overlay; a B tile on an empty square, on grass, on grass with a bush; two and three B tiles one after another; a wall of A4 on grass; A4 on A4; a roof of A3 on grass; A5 on grass, and a B tile on it.
  The saved maps were read, and painting the same cases with `PaintTileAuto` gives the same layers and the same shapes, cell by cell (`MapPaintTests` keeps the cases).
- **VX Ace**: the maintainer, who works with both editors, says that the behaviour is the same as in MV, with one difference: there are not two layers for B-E but one, so two B tiles cannot be put on one square
  (the new one replaces the old). The layers agree with what a saved game of that editor shows: every A2 tile of the columns 0-3 is on layer 0 and every one of the columns 4-7 on layer 1 (100 % of 79 000 and 20 000 tiles),
  the deep sea on layer 1, B-E only on layer 2 (40 000 tiles). The cases above were painted in the editor of MV only; for VX Ace there is the maintainer's word and the saved game, not a map painted for the purpose.
- In **older MV maps** the same tiles are not always on these layers (a game of MV had B-E tiles on layer 1 and overlays on layer 2): the editor of an earlier version, or the layer set by hand. Read a layer with care.

A second round in the MV editor gave the removal: the sand (kind 24) painted over grass and a B tile, over grass + bush + B tile, over grass + two B tiles; B tiles alone and then grass; and an A4 wall, an A3 roof and an A5 tile over grass + bush + B tile:
each time only the new tile was left, on layer 0. The same B tile painted twice stayed one tile on the top layer. A bush painted over grass and two B tiles left the grass and the bush (the B tiles are gone), and another overlay painted over grass, bush and a B tile
replaced the bush and removed the B tile. So **a tile of A removes everything above its own layer**: the ground the whole square but itself, an overlay the B-E tiles (and the overlay that was there), B-E nothing but the tile under the one it moves.

Not tried: the eraser (the editor of MV has none: there is no empty tile to paint with).

### How the shape of an autotile is chosen

**The numbers** are read from the tables of the engine (`FLOOR_AUTOTILE_TABLE`: which quarter of the picture goes where). Each quarter of a floor-type tile is an inner part, an edge, an outer corner or a notch (an inner corner),
and the 47 different shapes follow from the 4 sides and the 4 corners; the order of the table is: 0-15 every side connected and a notch at any of the 4 corners (bits 1 top left, 2 top right, 4 bottom right, 8 bottom left), 16-19 only the left
not connected (notches at the right corners), 20-23 only the top, 24-27 the right, 28-31 the bottom, 32 left and right, 33 top and bottom, 34-41 two neighbouring sides (top and left 34-35, top and right 36-37, bottom and right 38-39, bottom
and left 40-41; one possible notch), 42-45 three sides (only the bottom connected 42, only the right 43, only the top 44, only the left 45), 46 none. A corner counts only when both sides of it are connected. **Shape 47** (the picture
of a tile with nothing round it, drawn from the first tile of the block) is in the table, but no map of the real projects below has it: the editors give 46. A wall type has one bit for each side that is not connected (1 left, 2 top, 4 right, 8 bottom), a waterfall
two (1 left, 2 right).

**What is connected** (found by comparing with the shapes saved in real maps, same layer only): a neighbour of the same kind; water (A1, not a waterfall) to a waterfall; the side of a wall (A3 or A4) to the top of any A4 wall at its left and right.
The edge of the map: connected for water, ground, wall tops and waterfalls; for roofs and wall sides it is connected to the left and right in **VX Ace** and not connected anywhere in **MV**; above and below it is never connected.
Everything else (empty, a normal tile, another kind) is not connected.

How well this rule reproduces what the editors saved (the share of the autotiles whose saved shape equals the rule's), on one game made with each editor (about 180 000 autotiles each; a second VX Ace game gave 94 % for ground and 94 % for wall tops):

| tiles | MV editor, one game | VX Ace editor, one game |
|---|---|---|
| water and A1 ground | 99.9 % | 96 % |
| waterfalls | 100 % | |
| A2 ground | 99.7 % | 97 % |
| tops of A4 walls | 99.3 % | 96 % |
| roofs, walls of A3 | 99.5 % | 89-90 % |
| sides of A4 walls | 90 % | 89 % |

The rest are tiles the editors left as they were when something next to them changed (a neighbour erased or replaced, a block pasted), so a saved map has shapes that the rule would reshape (`UpdateAutotiles()` does it).
For the sides of A4 walls the editors also differ from the rule for a wall side joined to other kinds of wall side and to the wall top below it (about a third of them are saved as connected): not reproduced.
Water kinds 0 and 1 (the sea and the deep sea) and the other A1 kinds were seen connected to their own kind only; the editors' rule for the sea next to the deep sea was not seen in the data.

## Facts observed in the data of one full VX Ace game

- 216 maps, all with `Data.SizeZ = 4`; 9 tilesets, the first (mode 0) is the world map, the other eight have mode 1; every `Flags` has 8192 cells.
- Cells of layer 0: A1, A2, A3, A4, A5 and empty; layer 1: A1, A2 and empty; layer 2: B, C, D, E and empty.
- Layer 3: the low byte takes 0-15 (shadow quarters, 5 is the most common), the high byte (regions) takes 0, 1 and 2.

## Not checked

- How the real VX Ace draws the layers (its `Tilemap` is native code): the ids and flags are the same as in MV, the layer order is assumed to be the same.
- The rules of painting in the editor of VX Ace (the layer of a tile, what a tile removes, the shadows round it): from the maintainer's word and a saved game, not from cases painted for the purpose as for MV.
- Which tiles the editors let a layer take (the numbers above only say what the editors did in one game).
- The range of the regions in the editors (VX Ace 0-63, MV 0-255, from memory) and of the terrain tags (0-7), and how the editors label the tileset modes.
- The meaning of A1 kinds 2 and 3 in the editor's terms (only what the engine does with them is known).
