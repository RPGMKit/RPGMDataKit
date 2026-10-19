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
| `Background` | the colour under the tiles; transparent by default |
| `TileSize` | the size of a tile when it cannot be taken from the sheets (a tileset with no sheet); a multiple of 4 |

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
- ImageSharp 3.1.12 has advisories on NuGet (moderate and high; the converter uses the same version); loading a picture that somebody else made is the risk, and the pictures of a project are the user's own.
  ImageSharp 4 asks for a licence key in a Release build, and ImageSharp 3.2.0 has the same advisories.
