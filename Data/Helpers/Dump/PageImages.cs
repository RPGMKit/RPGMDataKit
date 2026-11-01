using System.Text.Json;
using System.Text.Json.Serialization;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Storages.Abstraction;

namespace RPGMDataKit.Data.Helpers.Dump;

/// <summary>An image that an html page shows: how its sheet is cut (<c>K</c>), the path without an extension, the index in the sheet, the direction and the pattern of a character.</summary>
internal sealed record ImageRef(string K, string P, int I, int? D = null, int? N = null);

internal static class PageImages
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static string Serialize(IEnumerable<ImageRef> images) => JsonSerializer.Serialize(images, JsonOptions);

    /// <summary>
    /// The graphic of a page as the images of the page need it: its tile (the sheet of the tileset of the map, as the engine picks it: the names of the tileset from the 6th, a sheet for each 256 tiles),
    /// or its character; <c>null</c> when the page shows nothing or its tile has no sheet.
    /// </summary>
    public static string? GraphicOf(IPageGraphic graphic, Storage storage, int tilesetId)
    {
        var mv = storage.Version == StorageVersion.MV;

        if (graphic.TileId > 0)
        {
            var sheet = 5 + graphic.TileId / 256;
            var names = storage.Tilesets.Get(tilesetId)?.TilesetNames;
            if (names == null || sheet >= names.Count || string.IsNullOrWhiteSpace(names[sheet]))
                return null;

            return Serialize([new ImageRef("tile", (mv ? "img/tilesets/" : "Graphics/Tilesets/") + names[sheet], graphic.TileId)]);
        }

        if (string.IsNullOrWhiteSpace(graphic.CharacterName))
            return null;

        var folder = mv ? "img/characters/" : "Graphics/Characters/";
        return Serialize([new ImageRef("char", folder + graphic.CharacterName, graphic.CharacterIndex, graphic.Direction, graphic.Pattern)]);
    }
}
