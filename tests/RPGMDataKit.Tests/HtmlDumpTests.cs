using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Commands.MovementCommands;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>The html form of a dump: folding blocks, the numbers of lines, the picture of a command, the text of the text dump kept.</summary>
public sealed class HtmlDumpTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static Storage Open(Version v)
    {
        var project = Path.Combine(AppContext.BaseDirectory, "Projects", v.Name);
        return v.Name == "MV" ? new MVStorage(project, "data") : new VXAceStorage(project, "Data");
    }

    private static ICommonEvent Made(Version v)
    {
        var commonEvent = EmptyCommandTests.EventOf(v);
        commonEvent.Replace(b => b
            .Text("a <b> & c")
            .IfSwitch(5, then: t => t.Add(new ShowPictureCommand { Layer = 1, Picture = "Pic" }), @else: e => e.Add(new WaitCommand { FrameCount = 30 })));

        return commonEvent;
    }

    private static async Task<string> Dump(Version v, DumpFormat format)
    {
        var commonEvent = Made(v);
        var writer = new StringWriter();
        var options = new DumpOptions { Format = format };
        var context = new EntityContext(Open(v));
        await ((IDumpable)commonEvent).Dump(writer, context, options);

        return writer.ToString();
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheTextLineIsTheJoinedPartsOfTheCommand(Version v)
    {
        var context = new EntityContext(Open(v));
        var options = new DumpOptions();

        foreach (var command in Made(v).NamedCommands)
        {
            var parts = command.GetTextParts(context, options);

            Assert.Equal(parts.ToString(), command.AsString(context, options));
            Assert.StartsWith($"({command.Code:D3}) ", command.AsString(context, options));
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheBlocksFoldAndTheLinesAreNumbered(Version v)
    {
        var html = await Dump(v, DumpFormat.Html);

        Assert.StartsWith("<!DOCTYPE html>", html);
        Assert.Contains("<script>", html);
        Assert.Contains("<details open><summary>", html);
        Assert.Contains("<span class=\"no\">000</span>", html);
        Assert.Contains("<span class=\"no\">003</span>", html);
        Assert.Equal(html.Split("<details").Length, html.Split("</details>").Length);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheTextIsEscaped(Version v)
    {
        var html = await Dump(v, DumpFormat.Html);

        Assert.Contains("a &lt;b&gt; &amp; c", html);
        Assert.DoesNotContain("a <b> & c", html);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheShowPictureLineHasThePictureForTheTooltip(Version v)
    {
        var html = await Dump(v, DumpFormat.Html);
        var expected = v.Name == "MV" ? "img/pictures/Pic" : "Graphics/Pictures/Pic";

        Assert.Contains($"data-pics=\"[{{&quot;k&quot;:&quot;pic&quot;,&quot;p&quot;:&quot;{expected}&quot;,&quot;i&quot;:0}}]\"", html);
        Assert.Equal(1, html.Split("data-pics=\"").Length - 1);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheTextDumpIsNotChanged(Version v)
    {
        var text = await Dump(v, DumpFormat.Text);

        Assert.DoesNotContain("<", text.Replace("<b>", ""));
        Assert.Contains("(101)", text);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheFaceOfTheTextAndTheImagesOfAnActorAreCutByTheirIndex(Version v)
    {
        var commonEvent = EmptyCommandTests.EventOf(v);
        commonEvent.Replace(b => b
            .Text(new ShowTextBaseCommand { Face = "Actor1", Offset = 5 }, "hi")
            .Add(new ChangeActorImagesCommand { ActorId = 1, CharacterImage = "Hero", CharacterImageOffset = 3, FaceImage = "Face2", FaceImageOffset = 6 }));
        var writer = new StringWriter();
        var context = new EntityContext(Open(v));

        await ((IDumpable)commonEvent).Dump(writer, context, new DumpOptions { Format = DumpFormat.Html });
        var html = writer.ToString();
        var faces = v.Name == "MV" ? "img/faces/" : "Graphics/Faces/";
        var characters = v.Name == "MV" ? "img/characters/" : "Graphics/Characters/";

        Assert.Contains($"&quot;k&quot;:&quot;face&quot;,&quot;p&quot;:&quot;{faces}Actor1&quot;,&quot;i&quot;:5", html);
        Assert.Contains($"&quot;k&quot;:&quot;char&quot;,&quot;p&quot;:&quot;{characters}Hero&quot;,&quot;i&quot;:3", html);
        Assert.Contains($"&quot;p&quot;:&quot;{faces}Face2&quot;,&quot;i&quot;:6", html);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheImageStepOfARouteHasTheImageAndEveryImageHasItsOpenAction(Version v)
    {
        var commonEvent = EmptyCommandTests.EventOf(v);
        commonEvent.Replace(b => b
            .MoveRoute(r => r.Add(new MoveChangeImageCommand { Image = "Hero", Offset = 2 }))
            .Add(new ChangeActorImagesCommand { ActorId = 1, CharacterImage = "Hero", FaceImage = "Face2" }));
        var writer = new StringWriter();

        await ((IDumpable)commonEvent).Dump(writer, new EntityContext(Open(v)), new DumpOptions { Format = DumpFormat.Html });
        var html = writer.ToString();

        Assert.Contains("&quot;i&quot;:2", html);
        // one action for the step of the route, two for the actor (the sprite and the face)
        Assert.Equal(3, html.Split("<a class=\"act\"").Length - 1);
        Assert.Contains(">sprite Hero</a>", html);
        Assert.Contains(">face Face2</a>", html);
    }
}
