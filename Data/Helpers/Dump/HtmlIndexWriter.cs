using System.Net;
using System.Text;
using RPGMDataKit.Storages.Abstraction;

namespace RPGMDataKit.Data.Helpers.Dump;

/// <summary>
/// The index of an html dump (<c>index.html</c> in the folder of the dump), three tabs: the common events, the maps, the troops. In the tab of the maps the left side is the maps (a tree in the order of
/// the editor, or a list by the numbers) and the right side the events of the map that is chosen, a flat list with the links to all the pages of each event; the troops are a flat list like it.
/// Every page is a link to its file. A filter keeps what has the words typed.
/// </summary>
public static class HtmlIndexWriter
{
    public const string FileName = "index.html";

    /// <summary>Writes the index into the folder and returns the path of the file.</summary>
    public static async Task<string> Write(string folder, Storage storage, DumpIndex index)
    {
        // every map of the project is listed, a map without events too: it can be the parent of the maps that have some
        var infos = storage.MapInfos.Get().Where(pair => pair.Value != null).ToDictionary(pair => pair.Key, pair => pair.Value!);
        var mapIds = infos.Keys.Union(index.Maps.Keys).OrderBy(id => id).ToList();

        var sb = new StringBuilder();
        sb.Append("<!DOCTYPE html>\n<html><head><meta charset=\"utf-8\"><title>Index</title>");
        sb.Append("<style>").Append(HtmlResources.Get("dump.css")).Append(HtmlResources.Get("index.css")).Append("</style></head><body>\n");
        sb.Append("<div class=\"bar\"><label>Project path: <input id=\"root\" size=\"36\" value=\"" + Encode(Path.GetFullPath(storage.ProjectPath)) + "\"></label> <button id=\"reset\">Reset</button> <input id=\"filter\" type=\"search\" placeholder=\"Filter: words of the names\">");
        sb.Append($" <span class=\"tabs\"><button data-tab=\"ce\">Common events ({index.CommonEvents.Count})</button>");
        sb.Append($"<button data-tab=\"maps\">Maps ({mapIds.Count})</button>");
        sb.Append($"<button data-tab=\"troops\">Troops ({index.Troops.Count})</button></span>");
        sb.Append(" <button id=\"theme\" title=\"Colors: follow the system, light or dark\">Theme: Auto</button></div>\n");

        WriteCommonEvents(sb, index);
        WriteMaps(sb, infos, mapIds, index);
        WriteTroops(sb, index);

        sb.Append("<script>").Append(HtmlResources.Get("theme.js")).Append(HtmlResources.Get("images.js")).Append(HtmlResources.Get("index.js")).Append("</script></body></html>\n");

        var path = Path.Combine(folder, FileName);
        await File.WriteAllTextAsync(path, sb.ToString(), new UTF8Encoding(false));

        return path;
    }

    private static string Encode(string text) => WebUtility.HtmlEncode(text ?? "");

    // the words the filter looks in: lower case
    private static string Text(string text) => Encode(text.ToLowerInvariant());

    private static string Label(string prefix, int id, string name) => string.IsNullOrWhiteSpace(name) ? $"{prefix}{id}" : $"{prefix}{id} {name}";

    private static string Pages(IEnumerable<DumpIndex.Page> pages) =>
        "<span class=\"pages\">" + string.Join(' ', pages.Select(page =>
            $"<a href=\"{Encode(page.Path)}\"{(page.Pictures != null ? $" data-pics=\"{Encode(page.Pictures)}\"" : "")}>P{page.Id}</a>")) + "</span>";

    private static void WriteCommonEvents(StringBuilder sb, DumpIndex index)
    {
        sb.Append("<section id=\"tab-ce\" class=\"tab\">\n");
        foreach (var (id, entry) in index.CommonEvents)
        {
            var label = Label("CE", id, entry.Name);
            sb.Append($"<div class=\"row ce\" data-t=\"{Text(label)}\"><a href=\"{Encode(entry.Path)}\">{Encode(label)}</a></div>\n");
        }

        sb.Append("</section>\n");
    }

    private static string EventLabel(int id, DumpIndex.EventEntry entry) => $"{Label("E", id, entry.Name)} ({entry.X}, {entry.Y})";

    private static void WriteMaps(StringBuilder sb, Dictionary<int, Data.Structures.Interfaces.IMapInfo> infos, List<int> mapIds, DumpIndex index)
    {
        sb.Append("<section id=\"tab-maps\" class=\"tab\"><div class=\"tree\">");
        sb.Append("<label class=\"treebox\"><input type=\"checkbox\" id=\"tree\" checked> Map tree</label><div id=\"maps\">\n");

        // the maps come in the order of their numbers; the page puts them into their parents (a tree) or back
        foreach (var id in mapIds)
        {
            infos.TryGetValue(id, out var info);
            index.Maps.TryGetValue(id, out var events);
            var label = Label("M", id, info?.Name ?? "");
            var eventTexts = string.Join(" | ", (events ?? []).Select(pair => EventLabel(pair.Key, pair.Value).ToLowerInvariant()));

            sb.Append($"<div class=\"map{(events == null ? " empty" : "")}\" data-id=\"{id}\" data-parent=\"{info?.Parent ?? 0}\" data-order=\"{info?.Order ?? id}\"");
            sb.Append($" data-open=\"{(info?.Expanded == true ? 1 : 0)}\" data-t=\"{Text(label)}\" data-e=\"{Encode(eventTexts)}\">");
            sb.Append($"<div class=\"row\"><span class=\"arrow\"></span><span class=\"name\">{Encode(label)}</span></div><div class=\"kids\"></div></div>\n");
        }

        sb.Append("</div></div><div class=\"events\" id=\"events\"><p class=\"hint\">Choose a map</p>\n");

        // the events of each map, a flat list; the page shows the one of the map that is chosen
        foreach (var id in mapIds)
        {
            infos.TryGetValue(id, out var info);
            index.Maps.TryGetValue(id, out var events);
            var label = Label("M", id, info?.Name ?? "");

            sb.Append($"<section class=\"map-events\" data-for=\"{id}\" data-t=\"{Text(label)}\" hidden><h3>{Encode(label)}</h3>");
            if (events == null)
                sb.Append("<p class=\"hint\">No events</p>");

            foreach (var (eventId, entry) in events ?? [])
            {
                var eventLabel = EventLabel(eventId, entry);
                sb.Append($"<div class=\"row event\" data-t=\"{Text(eventLabel)}\"><span class=\"elabel\">{Encode(eventLabel)}</span>{Pages(entry.Pages)}</div>");
            }

            sb.Append("</section>\n");
        }

        sb.Append("</div></section>\n");
    }

    private static void WriteTroops(StringBuilder sb, DumpIndex index)
    {
        sb.Append("<section id=\"tab-troops\" class=\"tab\">\n");
        foreach (var (id, troop) in index.Troops)
        {
            var label = Label("TE", id, troop.Name);
            var pages = string.Join(' ', troop.Pages.Select(page => "p" + page.Id));
            sb.Append($"<div class=\"row troop\" data-t=\"{Text(label + " " + pages)}\"><span class=\"elabel\">{Encode(label)}</span>{Pages(troop.Pages)}</div>\n");
        }

        sb.Append("</section>\n");
    }
}
