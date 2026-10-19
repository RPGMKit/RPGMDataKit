using RPGMDataKit.Storages.Abstraction;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RPGMDataKit.Imaging;

public static class MapRenderExtensions
{
    /// <summary>
    /// A picture of the tiles of a map of the project, from the sheets in the project (found through <see cref="Storage.FindResourceFile"/>: <c>Graphics/Tilesets</c> of VX Ace, <c>img/tilesets</c> of MV;
    /// the pictures must be files in the folder, not packed in an archive). The part of the map, the layers and the like are in the options. The events and characters are not drawn.
    /// </summary>
    /// <example><c>storage.RenderMap(3, new MapImageOptions { Region = new MapRegion(10, 4, 20, 15) }).SaveAsPng("part.png");</c></example>
    /// <exception cref="ArgumentException">There is no such map, or the tileset of the map does not exist.</exception>
    public static Image<Rgba32> RenderMap(this Storage storage, int mapId, MapImageOptions? options = null)
    {
        var map = storage.Maps.Get(mapId) ?? throw new ArgumentException($"There is no map {mapId}", nameof(mapId));
        var tileset = storage.Tilesets.Get(map.TilesetId) ?? throw new ArgumentException($"The map {mapId} has the tileset {map.TilesetId}, which does not exist", nameof(mapId));

        return MapRenderer.Render(map, tileset, new StorageTilesetSheets(storage, tileset), options);
    }
}
