using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Commands.IfCommands;
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

    // ---- the common events that a page calls ----

    private static ICommonEvent Calling(Version v, int id, Storage storage, params int[] calls)
    {
        var commonEvent = EmptyCommandTests.EventOf(v);
        commonEvent.Replace(b =>
        {
            b.Text("in " + id);
            foreach (var call in calls)
                b.Add(new CallCommonEventCommand { EventId = call });
        });
        storage.CommonEvents.Set(id, commonEvent);

        return commonEvent;
    }

    private static async Task<string> DumpExpanded(Storage storage, ICommonEvent commonEvent, bool expand = true)
    {
        var writer = new StringWriter();

        await ((IDumpable)commonEvent).Dump(writer, new EntityContext(storage),
            new DumpOptions { Format = DumpFormat.Html, ExpandCommonEvents = expand });

        return writer.ToString();
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheTextDumpRefusesToExpandTheCommonEvents(Version v)
    {
        var storage = Open(v);
        var commonEvent = Calling(v, 1, storage);

        await Assert.ThrowsAsync<NotSupportedException>(() => ((IDumpable)commonEvent).Dump(new StringWriter(),
            new EntityContext(storage), new DumpOptions { Format = DumpFormat.Text, ExpandCommonEvents = true }));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task WithoutTheOptionACallIsALineAndThePageHasNoCheckbox(Version v)
    {
        var storage = Open(v);
        Calling(v, 2, storage);
        var html = await DumpExpanded(storage, Calling(v, 1, storage, 2), expand: false);

        Assert.DoesNotContain("details class=\"ce\"", html);
        Assert.DoesNotContain("id=\"expand\"", html);
        Assert.DoesNotContain("data-expand-common-events", html);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task ACallIsAnEmptyClosedBlockAndTheListOfItsEventIsInATemplateWithItsOwnNumbers(Version v)
    {
        var storage = Open(v);
        Calling(v, 2, storage);
        var html = await DumpExpanded(storage, Calling(v, 1, storage, 2));

        Assert.Contains("<body data-expand-common-events data-top-ce=\"1\">", html);
        Assert.Contains("id=\"expand\" checked", html);
        Assert.Contains("<details class=\"ce\" data-ce=\"2\"><summary>", html);
        Assert.DoesNotContain("<details class=\"ce\" data-ce=\"2\" open", html);
        Assert.Contains("</summary></details>", html);
        // the list of the called event starts from 000 again
        Assert.Contains("<template id=\"ce-2\"><div class=\"list\"><div class=\"cmd", html);
        Assert.Contains("<span class=\"no\">000</span>", html.Substring(html.IndexOf("<template id=\"ce-2\"", StringComparison.Ordinal)));
        Assert.Contains("in 2", html);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheListOfACalledEventIsWrittenOnceWhoeverCallsIt(Version v)
    {
        var storage = Open(v);
        // A calls B twice and C; B and C call D; D calls A (the page of A)
        Calling(v, 4, storage, 1);
        Calling(v, 3, storage, 4);
        Calling(v, 2, storage, 4);
        var a = Calling(v, 1, storage, 2, 2, 3);

        var html = await DumpExpanded(storage, a);

        foreach (var id in new[] { 2, 3, 4 })
            Assert.Equal(1, html.Split($"<template id=\"ce-{id}\"").Length - 1);
        // the dumped event is the start of every chain: its own list is not written again
        Assert.DoesNotContain("<template id=\"ce-1\"", html);
        // the calls are there for the page to open (3 on the page, 1 in the list of 2, 1 in 3, 1 in 4 that calls the event of the page)
        Assert.Equal(3, html.Split("<details class=\"ce\" data-ce=\"2\"").Length - 1 + html.Split("<details class=\"ce\" data-ce=\"3\"").Length - 1);
        Assert.Contains("<details class=\"ce\" data-ce=\"1\"", html);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task CallAndExitHaveTheirOwnClasses(Version v)
    {
        var storage = Open(v);
        var commonEvent = EmptyCommandTests.EventOf(v);
        commonEvent.Replace(b => b.Add(new CallCommonEventCommand { EventId = 2 }).Add(new ExitEventProcessingCommand()));

        var html = await DumpExpanded(storage, commonEvent, expand: false);

        Assert.Equal(1, html.Split("<div class=\"cmd call\"").Length - 1);
        Assert.Equal(1, html.Split("<div class=\"cmd exit\"").Length - 1);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task EveryLineOfACommentHasTheCommentClass(Version v)
    {
        var commonEvent = EmptyCommandTests.EventOf(v);
        commonEvent.Replace(b => b.Comment("first", "second", "third"));

        var html = await DumpExpanded(Open(v), commonEvent, expand: false);

        // the first line is the comment, the others are its continuation (a secondary line has its class too)
        Assert.Equal(3, html.Split("comment\" style=").Length - 1);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task ASoundHasAPillWithItsPathVolumeAndPitch(Version v)
    {
        var commonEvent = EmptyCommandTests.EventOf(v);
        commonEvent.Replace(b => b
            .Add(new PlayBGMCommand { SoundData = SoundOf<IBGM>(v, "Theme", 80, 110) })
            .Add(new PlaySECommand { SoundData = SoundOf<ISE>(v, "Hit", 90, 100) })
            .MoveRoute(r => r.Add(new MovePlaySeCommand { SoundData = SoundOf<ISE>(v, "Step", 70, 100) })));

        var html = await DumpExpanded(Open(v), commonEvent, expand: false);
        var bgm = v.Name == "MV" ? "audio/bgm/Theme" : "Audio/BGM/Theme";

        Assert.Contains($"data-p=\"{bgm}\" data-k=\"bgm\" data-v=\"80\" data-pt=\"110\"", html);
        Assert.Contains(">\u25B6 Hit</a>", html);
        Assert.Contains(">\u25B6 Step</a>", html);
    }

    private static T SoundOf<T>(Version v, string name, int volume, int pitch) where T : class, IAudioFile
    {
        var sound = v.New<T>();
        sound.Name = name;
        sound.Volume = volume;
        sound.Pitch = pitch;

        return sound;
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task AJumpAndItsLabelLeadToEachOther(Version v)
    {
        var commonEvent = EmptyCommandTests.EventOf(v);
        commonEvent.Replace(b => b
            .Add(new JumpToLabelCommand { Label = "A" })
            .Add(new JumpToLabelCommand { Label = "Nowhere" })
            .Add(new LabelCommand { Label = "A" })
            .Add(new JumpToLabelCommand { Label = "A" })
            .Add(new LabelCommand { Label = "A" }));

        var html = await DumpExpanded(Open(v), commonEvent, expand: false);

        // the jumps 0 and 3 go to the first label (2); the second label of the name is never the target
        Assert.Contains("data-jmp=\"0\" data-to=\"2\"", html);
        Assert.Contains("data-jmp=\"3\" data-to=\"2\"", html);
        Assert.Contains("data-lbl=\"2\" data-from=\"0 3\"", html);
        Assert.DoesNotContain("data-lbl=\"4\"", html);
        Assert.Contains("cmd broken-jump", html);
        Assert.Contains("data-kind=\"jmp\" data-n=\"3\"", html);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheLabelsOfACalledListAreItsOwn(Version v)
    {
        var storage = Open(v);
        var called = EmptyCommandTests.EventOf(v);
        called.Replace(b => b.Add(new LabelCommand { Label = "A" }).Add(new JumpToLabelCommand { Label = "A" }));
        storage.CommonEvents.Set(2, called);
        var commonEvent = EmptyCommandTests.EventOf(v);
        commonEvent.Replace(b => b.Add(new JumpToLabelCommand { Label = "A" }).Add(new CallCommonEventCommand { EventId = 2 }));

        var html = await DumpExpanded(storage, commonEvent);
        var page = html[..html.IndexOf("<template", StringComparison.Ordinal)];
        var template = html[html.IndexOf("<template", StringComparison.Ordinal)..];

        // a jump does not go to a label of another list: the outer one has no label, the called one has its own
        Assert.Contains("cmd broken-jump", page);
        Assert.Contains("data-lbl=\"0\" data-from=\"1\"", template);
        Assert.DoesNotContain("data-lbl", page);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task EveryLineHasTheSwitchesAndVariablesItNames(Version v)
    {
        var commonEvent = EmptyCommandTests.EventOf(v);
        commonEvent.Replace(b => b
            .IfSwitch(5)
            .IfVariableWithVariable(7, IfVariableCommand.OperatorType.Gte, 9)
            .Add(new ControlSwitchCommand { StartSwitchId = 3, EndSwitchId = 4 })
            .Text("hp \\V[12] and \\v[13]"));

        var html = await DumpExpanded(Open(v), commonEvent, expand: false);

        Assert.Contains("data-refs=\"s5\"", html);
        Assert.Contains("data-refs=\"v7 v9\"", html);
        Assert.Contains("data-refs=\"s3 s4\"", html);
        Assert.Contains("data-refs=\"v12 v13\"", html);
    }

    private static string TitleOfDump(string html) =>
        html.Split("<title>")[1].Split("</title>")[0];

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheTitleOfACommonEventIsCeNumberName(Version v)
    {
        var storage = Open(v);
        var commonEvent = Calling(v, 7, storage);
        commonEvent.Name = "Chat";

        Assert.Equal("CE7 Chat", TitleOfDump(await DumpExpanded(storage, commonEvent)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheTitleOfATroopPageIsTeNumberName(Version v)
    {
        var storage = Open(v);
        var troop = v.New<ITroop>();
        troop.Name = "Bats";
        storage.Troops.Set(5, troop);
        var page = troop.Pages.First()!;
        var writer = new StringWriter();

        await ((IDumpable)page).Dump(writer, new EntityContext(storage, new TroopContext { Troop = troop, PageId = 0, EventPage = page }),
            new DumpOptions { Format = DumpFormat.Html });

        Assert.Equal("TE5 Bats", TitleOfDump(writer.ToString()));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheTitleOfAMapEventPageIsMapEventPageName(Version v)
    {
        var storage = Open(v);
        var map = v.New<IMap>();
        var mapEvent = v.New<IEvent>();
        mapEvent.Id = 12;
        mapEvent.Name = "Door";
        var page = mapEvent.Pages.First()!;
        var writer = new StringWriter();

        await ((IDumpable)page).Dump(writer,
            new EntityContext(storage, new MapEventContext { MapId = 3, Map = map, Event = mapEvent, PageId = 1, EventPage = page }),
            new DumpOptions { Format = DumpFormat.Html });

        Assert.Equal("M3 E12 P1 Door", TitleOfDump(writer.ToString()));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task TheKindOfAnImageIsInThePillOnlyWhereACommandHoldsSeveralImages(Version v)
    {
        var commonEvent = EmptyCommandTests.EventOf(v);
        commonEvent.Replace(b => b
            .Add(new ShowPictureCommand { Layer = 1, Picture = "Pic" })
            // only the sprite is set, the face is empty: the pill has the kind all the same
            .Add(new ChangeActorImagesCommand { ActorId = 1, CharacterImage = "Hero", FaceImage = "" }));

        var html = await DumpExpanded(Open(v), commonEvent, expand: false);

        Assert.Contains(">Pic</a>", html);
        Assert.DoesNotContain(">picture Pic</a>", html);
        Assert.Contains(">sprite Hero</a>", html);
    }
}
