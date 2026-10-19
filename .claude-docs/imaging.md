# Pictures of maps (RPGMDataKit.Imaging)

The project `Imaging/RPGMDataKit.Imaging.csproj` puts the tiles of a map together into a picture, the way the engine draws them. It is a separate project because it needs an image library
(SixLabors.ImageSharp, the same version as the converter: 3.1.12), which the data library does not. The data library excludes the folder (`Compile Remove="Imaging\\**"` in `RPGMDataKit.csproj`).

```csharp
using RPGMDataKit.Imaging;

using var picture = storage.RenderMap(3);                                   // all of the map 3
picture.SaveAsPng("map3.png");

using var part = storage.RenderMap(3, new MapImageOptions { Region = new MapRegion(10, 4, 20, 15) });   // squares x 10..29, y 4..18
```

## What is in the picture

The tiles only: the lower layer (under the characters: the ground, the shadows, the objects without the star flag), then the upper layer (the tiles with the star flag, which are over the characters). The events,
characters, weather and the parallax picture are not drawn (the events and the characters go between the two layers when they are added: `MapImageLayers.Lower` and `.Upper` draw them apart).

The size of the picture is the size of the part in squares times the size of a tile, taken from the sheets (32 px in VX Ace, 48 in MV, and whatever a scaled project has).

### `MapImageOptions`

| Option | Meaning |
|---|---|
| `Region` (`MapRegion(X, Y, Width, Height)`) | the part of the map in squares; it must lie in the map; the picture is as big as the part |
| `Layers` (`MapImageLayers`) | `Lower`, `Upper` or `Both` (default) |
| `Shadows` | draw the shadows (default true) |
| `AnimationFrame` | the frame of the water and waterfalls of A1 (default 0): the water goes 0, 1, 2, 1, a waterfall 0, 1, 2 |
| `Grid` (`MapGridOptions`) | lines between the squares and, if asked, a text in each square; none by default (see below) |
| `Background` | the colour under the tiles; transparent by default |
| `TileSize` | the size of a tile when it cannot be taken from the sheets (a tileset with no sheet); a multiple of 4 |

### The grid (`MapGridOptions`)

```csharp
new MapImageOptions { Grid = new MapGridOptions { Labels = GridLabels.Coordinates } }
new MapImageOptions { Grid = new MapGridOptions { Labels = GridLabels.TileId, TileLayer = 1, LabelPosition = GridLabelPosition.Center } }
new MapImageOptions { Grid = new MapGridOptions { Label = (x, y) => events.Contains((x, y)) ? "E" : null } }
```

The grid is drawn over all the layers, after them. It is **two lines side by side** so that it is seen on a light map and on a dark one: a light line (`Color`, white with half of the alpha) of `LineWidth` pixels
(1 to a quarter of a tile) at the left and the top of every square, and a dark one of the same width (`ShadowColor`, black with half of the alpha) next to it, inside of the square. The light line also closes the picture at the
right and the bottom, so the size of the picture does not change. Where a light line and a dark one cross, the light one is on top. A transparent `ShadowColor` gives a line of one colour. Both are laid over the tiles by their alpha.

What a square says: `Labels` is `Coordinates` (`x,y` in the map, also when `Region` cuts a part), `Region` (the region id; 0 is not written) or `TileId` (the id on `TileLayer`; 0 and a layer the map does not have are not written).
`Label` (x, y) -> text or null is a function of your own that replaces `Labels`; its x and y are the map's too. A null or empty text writes nothing. The text is white with a dark outline of one pixel all round (`LabelColor`, `LabelOutline`; the outline is the text drawn eight times in the outline colour, one pixel off in each direction) at the top left of the
square or in its middle (`LabelPosition`), so that it reads on light tiles and on dark ones; its size is 0.28 of a tile, made smaller when the text is wider than the square.

The texts are drawn with `SixLabors.ImageSharp.Drawing` 2.1.7 (the newest one that works with ImageSharp 3.1.x; its 2.2.0 wants ImageSharp 3.2.0 and 3.x wants ImageSharp 4). The font is `FontFamily` or, when it is not given,
the first of Arial, Segoe UI, DejaVu Sans, Liberation Sans, or any font of the system; a machine with no font at all throws `InvalidOperationException` (the lines need no font).
The lines are made by whole numbers like the tiles and are tested pixel by pixel; the texts are anti-aliased by the library and are tested only for where they are written, not for how.

## A sheet with the numbers of its tiles (`TilesetSheetImage`)

To find the id of a tile to paint with (`PaintTile`) or the kind of an autotile, `storage.RenderTilesetSheet(tilesetId, TileSheet.B)` gives the picture of one sheet of a tileset of the project with the numbers on it
(`TilesetSheetImage.Render(pixels, sheet)` for a sheet you have in memory):
- **B, C, D, E, A5**: the id of every tile, in its top left corner. The ids of a sheet go down the rows of its left half (8 tiles wide: 0-127 on B), then its right half (128-255); A5 has 8 columns (1536-1663).
  `NormalTileId(sheet, column, row)` is the same number without a picture (the other way round is `TileIds.NormalTileCell`).
- **A1-A4**: a frame round each block (the picture of one kind of autotile) with its **kind** and the **id of its first shape** (`TileIds.MakeAutotile(kind, 0)`); `Blocks(sheet)` lists the blocks (kind, column, row, width, height in tiles):
  where the engine takes them from (`Tilemap._drawAutotile`): A2 blocks 2 x 3 in 8 columns and 4 rows (the kind is `16 + row * 8 + column`), A3 2 x 2 (from 48), A4 alternating 2 x 3 tops and 2 x 2 sides (from 80),
  A1 the sea (kind 0, the deep sea 1: three frames side by side, the first is the block), the two still waters, then water and waterfalls in pairs.
- Options (`TilesetSheetImageOptions`): `Labels`, `Grid` (both on), `Background` (grey, so that the transparent parts are seen) and `FontFamily`. The numbers need a font like the grid of a map.
  The frame of a tile or block is two lines: a light one (`GridColor`, white, alpha 230) on its edge and a dark one (`GridShadowColor`, black, alpha 230) just inside, each `GridLineWidth` pixels wide (1 to a quarter of a tile), so it is seen on light and
  on dark pictures; the frames of two neighbours give a light band between two dark ones.

Checked on the sheets of a real tileset (all of A1-A4, B and A5 were looked at: the blocks and the ids sit where the pictures are); the tests keep the positions of the blocks and the ids.

## Where the sheets come from

`storage.RenderMap` finds the sheets through the storage: `StorageTilesetSheets` asks `storage.FindResourceFile(ResourceType.Tileset, name)` for every name of the tileset, so the folder of the maker
(`Graphics/Tilesets` of VX Ace, `img/tilesets` of MV), the extensions it reads and the case of the name are the storage's business. See [storage.md](storage.md#resources-pictures-sounds-movies).
To use another folder for the sheets (for example `img/tilesets_game`, the scaled sheets of a converted project) put it in the storage: `storage.SetResourceFolder(ResourceType.Tileset, "img/tilesets_game")`.

For sheets that are not files of a project (a test, a generated picture), give `MapRenderer.Render(map, tileset, sheets, options)` an `ITilesetSheets`: `InMemoryTilesetSheets` or your own. The pictures must be
files in the folder, not packed in an archive (`Game.rgss3a`, an encrypted MV project).

## How the picture is made

`MapRenderer` repeats the code of the engine of MV (`Tilemap._paintTiles`, `_drawTile`, `_drawNormalTile`, `_drawAutotile`, `_drawTableEdge`, `_drawShadow`), square by square. The steps for one square:

1. **Lower**: the tiles of the layers 0 and 1 that do not have the star flag; the shadow (the quarters of the square that are dark: black with the alpha of a half); the edge of a table (when the square above has a tile of A2 with
   the counter flag on the layer 1 and this square does not, and the tile of the layer 0 is not a wall or a roof); the tiles of the layers 2 and 3 that do not have the star flag.
2. **Upper**: the tiles of all layers that have the star flag.

A **normal tile** (B-E, A5) is the cell `TileIds.NormalTileCell(id)` of its sheet. An **autotile** is cut as four quarters of its picture: `AutotileTables` (the tables of the engine) say which quarters for the shape; where the picture of
the kind starts on the sheet depends on the sheet (A1 has special places for the sea, the deep sea, the static waters and the waterfalls and moves with the frame; A2, A3 and A4 have a block of 2 x 3, 2 x 2, 2 x 2.5 tiles for each kind).
The front of a table (the lower quarters of an A2 tile with the counter flag) takes the quarter of the top of the table below it and the upper half of its own. Tiles of a sheet that is missing are not drawn.
VX Ace has three layers of tiles, MV four: the fourth of VX Ace is empty. The pixels are laid over one another with whole numbers (`PixelCanvas.Over`).

## How it was checked

The picture is compared with the code of the engine, not with a screenshot:

- `tests/RPGMDataKit.Tests/Tools/engine-tile-harness.js` runs the real `Tilemap` functions of `rpg_core.js` (the part that paints a square) in `node`, on a dumped scenario, with a bitmap that lays pixels over one another the same way.
  It needs the `rpg_core.js` of an MV project (they are not in the repository) and prints the hash of the picture.
- `TileScenario` (tests) is a small made-up map and tileset with every kind of tile (the pixels of the sheets tell their place). With `RPGMDATAKIT_TILE_SCENARIO_DUMP=<folder>` set, `MapRendererTests.TheMapIsDrawnAsTheEngineDrawsIt` writes the
  scenario and the hash of its own picture there; `node tests/RPGMDataKit.Tests/Tools/engine-tile-harness.js <rpg_core.js> <folder>` prints the hash of the engine. They are equal; the hash is in the test, so the test needs no `node`.
- The same was done once on real data: five maps of an MV project (sheets of 48 px) and five maps of a VX Ace project (sheets of 32 px) came out identical to the engine, pixel for pixel.

If a rule of the engine changes (a new version of MV), run the harness again on the scenario and change the hash in the test only when the difference is understood.

## Not there / not checked

- The events and characters, the parallax picture, the weather, the screen tone, the battle backgrounds.
- The drawing of VX Ace itself is native code (RGSS3): it is assumed to draw the same tiles the same way (the ids, the flags and the sheets are the same); there is no VX Ace engine to compare with.
- The animation: a picture is one frame (`AnimationFrame`).
- The look of the grid texts on every font and platform: they were looked at on real maps of both makers with one font (Liberation Sans), on a normal map and on a white and a black background; the outline keeps the text readable on both, but a very small tile (a small font) makes it crowded.
- ImageSharp 3.1.12 has advisories on NuGet (moderate and high; the converter uses the same version); loading a picture that somebody else made is the risk, and the pictures of a project are the user's own.
  ImageSharp 4 asks for a licence key in a Release build, and ImageSharp 3.2.0 has the same advisories.
