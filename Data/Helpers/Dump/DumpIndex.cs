namespace RPGMDataKit.Data.Helpers.Dump;

/// <summary>What a dump wrote, for the index of the dump: the common events, the pages of the events of the maps and the pages of the troops, with the files of them.</summary>
public sealed class DumpIndex(string root)
{
    /// <summary><c>Pictures</c> is what the page shows for the page (its character), as the images of an html page are written; <c>null</c> when it has none.</summary>
    public sealed record Page(int Id, string Path, string? Pictures = null);

    public sealed record CommonEventEntry(string Name, string Path);

    public sealed class EventEntry(string name, int x, int y)
    {
        public string Name { get; } = name;
        public int X { get; } = x;
        public int Y { get; } = y;
        public List<Page> Pages { get; } = [];
    }

    public sealed class TroopEntry(string name)
    {
        public string Name { get; } = name;
        public List<Page> Pages { get; } = [];
    }

    public SortedDictionary<int, CommonEventEntry> CommonEvents { get; } = new();

    /// <summary>The events of each map that has pages in the dump.</summary>
    public SortedDictionary<int, SortedDictionary<int, EventEntry>> Maps { get; } = new();

    public SortedDictionary<int, TroopEntry> Troops { get; } = new();

    public void AddCommonEvent(int id, string name, string filePath) =>
        CommonEvents[id] = new CommonEventEntry(name, Relative(filePath));

    public void AddMapPage(int mapId, int eventId, string eventName, int x, int y, int pageId, string filePath, string? pictures = null)
    {
        if (!Maps.TryGetValue(mapId, out var events))
            Maps[mapId] = events = new();

        if (!events.TryGetValue(eventId, out var entry))
            events[eventId] = entry = new EventEntry(eventName, x, y);

        entry.Pages.Add(new Page(pageId, Relative(filePath), pictures));
    }

    public void AddTroopPage(int troopId, string troopName, int pageId, string filePath)
    {
        if (!Troops.TryGetValue(troopId, out var troop))
            Troops[troopId] = troop = new TroopEntry(troopName);

        troop.Pages.Add(new Page(pageId, Relative(filePath)));
    }

    /// <summary>The path of a file of the dump, from the folder of the index, with the slashes of a link.</summary>
    private string Relative(string filePath) => Path.GetRelativePath(root, filePath).Replace('\\', '/');
}
