using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Storages.Abstraction;

namespace RPGMDataKit.Data.Helpers;

/// <summary>A map made by <see cref="MapCreationExtensions.CreateMap"/>: its number, the map and its entry in the list of maps.</summary>
public sealed record NewMap(int Id, IMap Map, IMapInfo Info);

/// <summary>Making a new map in a project, as the editors do: the map file and the entry in the list of maps (<c>MapInfos</c>).</summary>
public static class MapCreationExtensions
{
    /// <summary>
    /// Makes an empty map with the values that the editor gives to a new one and puts it into the storage: <c>Maps</c> gets the map and <c>MapInfos</c> its entry. They are written
    /// by <c>SaveAll</c> (or <c>Maps.Save(id)</c> and <c>MapInfos.Save()</c>). The number is the next free one after the highest number of the maps (as the editors number them), and so is the order: the entry goes after the other maps. The size, the tileset and the other values of the map are the ones of a new map of the maker.
    /// </summary>
    /// <param name="name">The name in the tree of maps (the editors call a new one <c>MAP001</c>, <c>MAP002</c>...). The name shown in the game is <c>IMap.DisplayName</c>, which stays empty.</param>
    /// <param name="width">The width in squares, at least 1.</param>
    /// <param name="height">The height in squares, at least 1.</param>
    /// <param name="tilesetId">The tileset of the map; it must exist.</param>
    /// <param name="parentId">The map this one is under in the tree, 0 for the root; it must exist.</param>
    public static NewMap CreateMap(this Storage storage, string name, int width = 17, int height = 13, int tilesetId = 1, int parentId = 0)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (width < 1)
            throw new ArgumentOutOfRangeException(nameof(width), width, "A map is at least 1 square wide");
        if (height < 1)
            throw new ArgumentOutOfRangeException(nameof(height), height, "A map is at least 1 square high");
        if (storage.Tilesets.Get(tilesetId) == null)
            throw new ArgumentException($"There is no tileset {tilesetId}", nameof(tilesetId));
        if (parentId != 0 && storage.MapInfos.Get(parentId) == null)
            throw new ArgumentException($"There is no map {parentId} to put the new one under", nameof(parentId));

        var id = storage.MapInfos.NextId();
        while (storage.Maps.Get(id) != null)
            id++;

        var map = storage.New<IMap>();
        map.TilesetId = tilesetId;
        map.Resize(width, height);

        var info = storage.New<IMapInfo>();
        info.Name = name;
        info.Parent = parentId;
        info.Order = storage.MapInfos.Get().Values.Where(i => i != null).Select(i => i!.Order).DefaultIfEmpty(0).Max() + 1;

        // the editor opens a new map with its view in the middle (a position in pixels of the editor's own tiles); it is a field of the editor and has no effect in the game
        var tile = storage.Version == StorageVersion.MV ? 48 : 32;
        info.ScrollX = width * tile / 2;
        info.ScrollY = height * tile / 2;

        storage.MapInfos.Set(id, info);
        storage.Maps.Set(id, map);

        return new NewMap(id, map, info);
    }
}
