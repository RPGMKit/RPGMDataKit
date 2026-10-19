using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Storages.Abstraction;

namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// The tree of maps (<c>Storage.MapInfos</c>): which map is under which (<see cref="IMapInfo.Parent"/>, 0 is the root), in what order the maps of one level go
/// (<see cref="IMapInfo.Order"/>) and whether a map is opened to show its children in the editor (<see cref="IMapInfo.Expanded"/>, a field of the editor with no effect in the game).
/// </summary>
public static class MapTreeExtensions
{
    /// <summary>
    /// The place of a map on its level (the maps under the same map): <see cref="Index"/> is where it is now (from 0), <see cref="Count"/> the number of the maps of the level, itself
    /// included. The places that <see cref="ReorderMap"/> takes are 0 to <c>Count - 1</c>.
    /// </summary>
    public readonly record struct MapPosition(int Index, int Count);

    /// <summary>The numbers of the maps of the level of the map (the maps under the same parent, or the root maps), in the order of the tree, the map itself among them.</summary>
    /// <exception cref="ArgumentException">There is no such map.</exception>
    public static IReadOnlyList<int> GetMapsAtLevel(this Storage storage, int mapId) =>
        storage.GetChildMaps(Info(storage, mapId, nameof(mapId)).Parent);

    /// <summary>Where the map is on its level and how many maps the level has: the places for <see cref="ReorderMap"/> are 0 to <c>Count - 1</c>.</summary>
    /// <exception cref="ArgumentException">There is no such map.</exception>
    public static MapPosition GetMapPositionAtLevel(this Storage storage, int mapId)
    {
        var level = storage.GetMapsAtLevel(mapId);

        return new MapPosition(level.ToList().IndexOf(mapId), level.Count);
    }

    /// <summary>The numbers of the maps right under the map (0 is the root), in the order of the tree: by <see cref="IMapInfo.Order"/>, then by number.</summary>
    /// <exception cref="ArgumentException">There is no such map.</exception>
    public static IReadOnlyList<int> GetChildMaps(this Storage storage, int parentId)
    {
        if (parentId != 0)
            Info(storage, parentId, nameof(parentId));

        return Listed(storage).Where(p => p.Info.Parent == parentId).OrderBy(p => p.Info.Order).ThenBy(p => p.Id).Select(p => p.Id).ToList();
    }

    /// <summary>
    /// Puts the map under another map (0 is the root), after the maps that are there. The children of the map go with it. The map stays where it is when it is under that map already.
    /// The state of opened or closed of the maps is not changed (see <see cref="ExpandMap"/>).
    /// </summary>
    /// <exception cref="ArgumentException">There is no such map, or the new parent is the map itself or one of the maps under it (the tree would have a loop).</exception>
    public static void SetMapParent(this Storage storage, int mapId, int newParentId)
    {
        var info = Info(storage, mapId, nameof(mapId));
        if (newParentId != 0)
            Info(storage, newParentId, nameof(newParentId));
        if (info.Parent == newParentId)
            return;
        if (newParentId == mapId || IsUnder(storage, newParentId, mapId))
            throw new ArgumentException($"The map {mapId} cannot be put under the map {newParentId}: it is that map or is above it in the tree", nameof(newParentId));

        info.Parent = newParentId;
        info.Order = Listed(storage).Select(p => p.Info.Order).DefaultIfEmpty(0).Max() + 1;
    }

    /// <summary>
    /// Puts the map at a place among the maps of its level (the maps under the same map): <paramref name="index"/> is the place it has afterwards, 0 is the first and <c>^1</c> the last
    /// (a number converts to an <see cref="Index"/>). <see cref="GetMapPositionAtLevel"/> tells how many places the level has. The maps keep their parent and the other levels of the tree are
    /// not touched: the map and the maps it passes exchange the numbers of <see cref="IMapInfo.Order"/> they already have.
    /// </summary>
    /// <exception cref="ArgumentException">There is no such map.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The place is not 0 to the number of the maps of the level less one (see <see cref="GetMapPositionAtLevel"/>).</exception>
    public static void ReorderMap(this Storage storage, int mapId, Index index)
    {
        var info = Info(storage, mapId, nameof(mapId));
        var level = storage.GetChildMaps(info.Parent).ToList();
        var place = index.GetOffset(level.Count);
        if ((uint)place >= (uint)level.Count)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"The map has {level.Count} places on its level (0-{level.Count - 1})");

        var from = level.IndexOf(mapId);
        if (from == place)
            return;

        level.RemoveAt(from);
        level.Insert(place, mapId);

        // the level goes by the numbers it has; a level whose numbers are not all different (not made by this library) gets new ones above all
        var orders = level.Select(id => storage.MapInfos.Get(id)!.Order).Order().ToList();
        if (orders.Distinct().Count() != orders.Count)
        {
            var next = Listed(storage).Select(p => p.Info.Order).Max() + 1;
            orders = Enumerable.Range(next, level.Count).ToList();
        }

        for (var i = 0; i < level.Count; i++)
            storage.MapInfos.Get(level[i])!.Order = orders[i];
    }

    /// <summary>Opens the map in the tree to show its children; with <paramref name="recursive"/> every map under it too.</summary>
    /// <exception cref="ArgumentException">There is no such map.</exception>
    public static void ExpandMap(this Storage storage, int mapId, bool recursive = false) => SetExpanded(storage, mapId, true, recursive);

    /// <summary>Closes the map in the tree to hide its children; with <paramref name="recursive"/> every map under it too.</summary>
    /// <exception cref="ArgumentException">There is no such map.</exception>
    public static void CollapseMap(this Storage storage, int mapId, bool recursive = false) => SetExpanded(storage, mapId, false, recursive);

    private static void SetExpanded(Storage storage, int mapId, bool expanded, bool recursive)
    {
        Info(storage, mapId, nameof(mapId)).Expanded = expanded;
        if (!recursive)
            return;

        var done = new HashSet<int> { mapId };
        var pending = new Queue<int>([mapId]);
        while (pending.Count > 0)
        {
            var parent = pending.Dequeue();
            foreach (var (id, info) in Listed(storage).Where(p => p.Info.Parent == parent))
            {
                if (!done.Add(id))
                    continue;

                info.Expanded = expanded;
                pending.Enqueue(id);
            }
        }
    }

    // true if the map is under the ancestor, at any depth
    private static bool IsUnder(Storage storage, int mapId, int ancestorId)
    {
        var seen = new HashSet<int>();
        for (var id = mapId; id != 0 && seen.Add(id); id = storage.MapInfos.Get(id)?.Parent ?? 0)
        {
            if (id == ancestorId)
                return true;
        }

        return false;
    }

    private static IMapInfo Info(Storage storage, int mapId, string parameter) =>
        storage.MapInfos.Get(mapId) ?? throw new ArgumentException($"There is no map {mapId}", parameter);

    // the maps of the list (MV has an empty slot 0)
    private static IEnumerable<(int Id, IMapInfo Info)> Listed(Storage storage) =>
        storage.MapInfos.Get().Where(p => p.Value != null).Select(p => (p.Key, p.Value!));
}
