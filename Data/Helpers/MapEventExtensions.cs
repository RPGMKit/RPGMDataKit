using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Storages;

namespace RPGMDataKit.Data.Helpers;

/// <summary>Making new events on a map, as the editors do (a number, a place, a name and a first page), and removing them.</summary>
public static class MapEventExtensions
{
    /// <summary>Removes every event of the map, the empty slots too; the next <see cref="CreateEvent"/> numbers from 1 again. Returns how many events there were.</summary>
    public static int ClearEvents(this IMap map)
    {
        ArgumentNullException.ThrowIfNull(map);
        var count = map.Events.Values.Count(e => e != null);
        map.Events.Clear();

        return count;
    }

    /// <summary>
    /// Makes an event with one page (the values of a new page of the maker: see <c>.claude-docs/events.md</c>) and puts it on the map. The number is the highest number
    /// of the events of the map + 1 (1 on a map without events), and <c>Id</c> of the event is set to it. The commands of the first page are made by <paramref name="build"/>
    /// (see <see cref="CommandBuilder"/>); without it the page holds only the empty command.
    /// </summary>
    /// <param name="x">The square, from 0.</param>
    /// <param name="y">The square, from 0.</param>
    /// <param name="name">The name of the event; the editors call a new one <c>EV001</c>, <c>EV002</c>... by its number, which is what a <c>null</c> gives.</param>
    /// <param name="build">The commands of the first page; <c>null</c> for none.</param>
    /// <exception cref="ArgumentOutOfRangeException">The square is outside of the map.</exception>
    /// <exception cref="InvalidOperationException">Another event is on the square (the editors do not put two events on one square).</exception>
    public static IEvent CreateEvent(this IMap map, int x, int y, string? name = null, Action<CommandBuilder>? build = null)
    {
        ArgumentNullException.ThrowIfNull(map);
        if (x < 0 || x >= map.Width)
            throw new ArgumentOutOfRangeException(nameof(x), x, $"The map is {map.Width} squares wide");
        if (y < 0 || y >= map.Height)
            throw new ArgumentOutOfRangeException(nameof(y), y, $"The map is {map.Height} squares high");
        if (map.Events.Values.FirstOrDefault(e => e != null && e.X == x && e.Y == y) is { } there)
            throw new InvalidOperationException($"The event {there.Id} is already on the square {x}, {y}");

        var id = map.Events.Keys.DefaultIfEmpty(0).Max() + 1;

        var ev = (MapTileExtensions.IsVxAceLayout(map) ? TypeMap.VXAce : TypeMap.Mv).New<IEvent>();
        ev.Id = id;
        ev.Name = name ?? $"EV{id:D3}";
        ev.X = x;
        ev.Y = y;

        if (build != null)
        {
            var page = ev.Pages[0] ?? throw new InvalidOperationException("A new event has no first page");
            page.Append(build);
        }

        map.Events[id] = ev;

        return ev;
    }
}
