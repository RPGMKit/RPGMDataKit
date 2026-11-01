using RPGMDataKit.Data.Helpers.Dump;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>The index of an html dump: what it lists, the links, the tree of the maps.</summary>
public sealed class HtmlDumpIndexTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static Storage Open(Version v)
    {
        var project = Path.Combine(AppContext.BaseDirectory, "Projects", v.Name);
        return v.Name == "MV" ? new MVStorage(project, "data") : new VXAceStorage(project, "Data");
    }

    private static string TempFolder()
    {
        var folder = Path.Combine(Path.GetTempPath(), "rpgmdump-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        foreach (var sub in new[] { "CE", "ME", "TE" })
            Directory.CreateDirectory(Path.Combine(folder, sub));

        return folder;
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheIndexListsTheEntitiesWithLinksToTheirFilesAndTheMapsAsATree(Version v)
    {
        var storage = Open(v);
        var folder = TempFolder();
        try
        {
            var index = new DumpIndex(folder);
            index.AddCommonEvent(1, "Status", Path.Combine(folder, "CE", "CE_001.html"));
            index.AddMapPage(2, 5, "Door <b>", 12, 8, 0, Path.Combine(folder, "ME", "M002_E005_P0.html"));
            index.AddMapPage(2, 5, "Door <b>", 12, 8, 1, Path.Combine(folder, "ME", "M002_E005_P1.html"), "[{\"k\":\"char\",\"p\":\"img/characters/Hero\",\"i\":1}]");
            index.AddTroopPage(3, "Bats", 0, Path.Combine(folder, "TE", "T003_P0.html"));

            var path = await HtmlIndexWriter.Write(folder, storage, index);
            var html = await File.ReadAllTextAsync(path);

            Assert.Equal(Path.Combine(folder, "index.html"), path);
            // the order: the common events, the maps, the troops
            var order = new[] { "Common events (1)", "Maps (", "Troops (1)" }.Select(text => html.IndexOf(text, StringComparison.Ordinal)).ToArray();
            Assert.True(order[0] > 0 && order[0] < order[1] && order[1] < order[2]);
            Assert.Contains("<a href=\"CE/CE_001.html\">CE1 Status</a>", html);
            Assert.Contains("<a href=\"TE/T003_P0.html\">P0</a>", html);
            // an event: the number, the name (escaped) and the coordinates, then the links to all of its pages
            Assert.Contains("<span class=\"elabel\">E5 Door &lt;b&gt; (12, 8)</span><span class=\"pages\"><a href=\"ME/M002_E005_P0.html\">P0</a> <a href=\"ME/M002_E005_P1.html\" data-pics=\"", html);
            // the page with a character has it for the tooltip of its link, the page without one has none
            Assert.Contains("<a href=\"ME/M002_E005_P1.html\" data-pics=\"[{&quot;k&quot;:&quot;char&quot;,&quot;p&quot;:&quot;img/characters/Hero&quot;,&quot;i&quot;:1}]\">P1</a>", html);
            Assert.Contains("<a href=\"ME/M002_E005_P0.html\">P0</a>", html);
            // three tabs; the events of a map are on the right of the maps, one block for each map
            Assert.Contains("id=\"tab-ce\"", html);
            Assert.Contains("id=\"tab-maps\"", html);
            Assert.Contains("id=\"tab-troops\"", html);
            Assert.Contains("<section class=\"map-events\" data-for=\"2\"", html);
            // the maps hold what the page needs for the tree and the list
            Assert.Contains("class=\"map\" data-id=\"2\" data-parent=", html);
            Assert.Contains("id=\"tree\" checked", html);
            Assert.Contains("id=\"filter\"", html);
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task AnHtmlDumpWritesTheIndexAndEveryPageLinksToIt(Version v)
    {
        var storage = Open(v);
        var folder = TempFolder();
        try
        {
            await DumpHelper.Dump(folder, storage, new DumpOptions { Format = DumpFormat.Html });

            Assert.True(File.Exists(Path.Combine(folder, "index.html")));
            var page = Directory.GetFiles(Path.Combine(folder, "CE"), "*.html").First();
            Assert.Contains("<a href=\"../index.html\">Index</a>", await File.ReadAllTextAsync(page));
            // the file of a page that the index lists is there
            var html = await File.ReadAllTextAsync(Path.Combine(folder, "index.html"));
            Assert.Contains("href=\"CE/CE_001.html\"", html);
            Assert.True(File.Exists(Path.Combine(folder, "CE", "CE_001.html")));
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task ATextDumpHasNoIndex(Version v)
    {
        var storage = Open(v);
        var folder = TempFolder();
        try
        {
            await DumpHelper.Dump(folder, storage, new DumpOptions { Format = DumpFormat.Text });

            Assert.False(File.Exists(Path.Combine(folder, "index.html")));
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }
}
