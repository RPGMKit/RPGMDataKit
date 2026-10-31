using System.Net;
using System.Text;
using System.Text.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Commands.MovementCommands;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Helpers.Dump;

/// <summary>The html form of a dump: one self-contained file per page, blocks fold, a picture shows in a tooltip.</summary>
public sealed class HtmlDumpWriter : IDumpWriter
{
    private static async Task BeginDocument(TextWriter writer, EntityContext context, DumpOptions options, string title, int? topEvent)
    {
        var root = Path.GetFullPath(context.Storage.ProjectPath);

        await writer.WriteLineAsync("<!DOCTYPE html>");
        await writer.WriteLineAsync("<html><head><meta charset=\"utf-8\">");
        await writer.WriteLineAsync($"<title>{Encode(title)}</title>");
        await writer.WriteLineAsync($"<style>{Css}</style></head>");
        // the file knows whether the common events can be expanded; the page has the checkbox only then
        // (data-top-ce: the common event that is dumped, the start of the chain of its calls)
        await writer.WriteLineAsync(options.ExpandCommonEvents
            ? "<body data-expand-common-events" + (topEvent != null ? $" data-top-ce=\"{topEvent}\"" : "") + ">"
            : "<body>");
        await writer.WriteLineAsync("<div class=\"bar\"><label>Project path: <input id=\"root\" size=\"60\" value=\""
                                    + Encode(root) + "\"></label> <button id=\"reset\">Reset</button> <button id=\"open\">Open all</button> <button id=\"close\">Close all</button>"
                                    + (options.ExpandCommonEvents ? " <label><input type=\"checkbox\" id=\"expand\" checked> Expand common events</label>" : "")
                                    + "</div>");
    }

    private static async Task EndDocument(TextWriter writer)
    {
        await writer.WriteLineAsync($"<script>{Script}</script></body></html>");
    }

    /// <summary>Writes a whole html document: the header of the page (the text lines the text dump has) and its commands.</summary>
    public async Task Write(TextWriter writer, string header, ICommandContainer container,
        EntityContext context, DumpOptions options)
    {
        var title = TitleOf(container, context, header);
        var state = new RenderState(context, options) { TopEvent = (container as ICommonEvent)?.Id };
        await BeginDocument(writer, context, options, title, state.TopEvent);
        await writer.WriteLineAsync($"<pre class=\"head\" id=\"top\">{Encode(header.TrimEnd())}</pre>");
        await writer.WriteLineAsync("<div class=\"cmds list\">");

        var lines = new StringBuilder();
        var at = 0;
        WriteLevel(lines, container.NamedCommands, ScopeOf(container.NamedCommands), state, ref at, -1);
        await writer.WriteAsync(lines.ToString());
        await writer.WriteLineAsync("</div>");

        // the common events that are called: the list of each is written once, the page puts a copy where a call is opened
        while (state.Pending.TryDequeue(out var eventId))
        {
            var called = context.Storage.CommonEvents.Get(eventId)!;
            lines.Clear();
            at = 0;
            WriteLevel(lines, called.NamedCommands, ScopeOf(called.NamedCommands), state, ref at, -1);
            await writer.WriteLineAsync($"<template id=\"ce-{eventId}\"><div class=\"list\">{lines}</div></template>");
        }

        await EndDocument(writer);
    }

    /// <summary>The title of the page: CE1 name, TE1 name (a troop), M1 E2 P3 name (a page of a map event).</summary>
    private static string TitleOf(ICommandContainer container, EntityContext context, string header)
    {
        var title = container switch
        {
            ICommonEvent commonEvent => $"CE{commonEvent.Id} {commonEvent.Name}",
            _ when context.MapEventContext is { } map => $"M{map.MapId} E{map.Event.Id} P{map.PageId} {map.Event.Name}",
            _ when context.TroopContext is { } troop => $"TE{troop.Troop.Id} {troop.Troop.Name}",
            _ => header.Split('\n')[0],
        };

        return title.Trim();
    }

    private sealed class RenderState(EntityContext context, DumpOptions options)
    {
        public EntityContext Context { get; } = context;
        public DumpOptions Options { get; } = options;

        /// <summary>The common event that is dumped; a call of it is always a recursion, so its list is not written again.</summary>
        public int? TopEvent { get; init; }

        /// <summary>The called common events whose lists are to be written, and the ones that are queued already.</summary>
        public Queue<int> Pending { get; } = new();

        private readonly HashSet<int> _queued = [];

        public void Need(int eventId)
        {
            if (eventId != TopEvent && _queued.Add(eventId))
                Pending.Enqueue(eventId);
        }
    }

    /// <summary>The labels and the jumps of one list: a jump goes to the first label of its name in its own list, as the engine does.</summary>
    private sealed class ListScope
    {
        /// <summary>The index of the first label of each name.</summary>
        public Dictionary<string, int> Labels { get; } = new(StringComparer.Ordinal);

        /// <summary>For the index of a label, the indexes of the jumps that go to it.</summary>
        public Dictionary<int, List<int>> Jumpers { get; } = new();
    }

    private static ListScope ScopeOf(IList<NamedCommand> commands)
    {
        var scope = new ListScope();

        for (var i = 0; i < commands.Count; i++)
            if (commands[i] is LabelCommand label)
                scope.Labels.TryAdd(label.Label ?? "", i);

        for (var i = 0; i < commands.Count; i++)
        {
            if (commands[i] is not JumpToLabelCommand jump || !scope.Labels.TryGetValue(jump.Label ?? "", out var target))
                continue;

            if (!scope.Jumpers.TryGetValue(target, out var jumpers))
                scope.Jumpers[target] = jumpers = [];
            jumpers.Add(i);
        }

        return scope;
    }

    private static int IndentOf(NamedCommand command) => command.Indent ?? 0;

    // a command followed by deeper commands opens a block; the commands of the same level go on after it
    private static void WriteLevel(StringBuilder sb, IList<NamedCommand> commands, ListScope scope, RenderState state,
        ref int at, int parentIndent)
    {
        while (at < commands.Count && IndentOf(commands[at]) > parentIndent)
        {
            var index = at;
            var command = commands[index];
            var line = Line(command, index, scope, state);
            ++at;

            if (state.Options.ExpandCommonEvents && command is CallCommonEventCommand call
                && state.Context.Storage.CommonEvents.Get(call.EventId) != null)
            {
                // closed and empty: the page fills it from the list of the event when it is opened
                state.Need(call.EventId);
                sb.Append($"<details class=\"ce\" data-ce=\"{call.EventId}\"><summary>").Append(line).Append("</summary></details>");
            }
            else if (at < commands.Count && IndentOf(commands[at]) > IndentOf(command))
            {
                sb.Append("<details open><summary>").Append(line).Append("</summary>");
                WriteLevel(sb, commands, scope, state, ref at, IndentOf(command));
                sb.Append("</details>");
            }
            else
                sb.Append(line);
        }
    }

    private static string Line(NamedCommand command, int index, ListScope scope, RenderState state)
    {
        var context = state.Context;
        var options = state.Options;
        // the switches and the variables the command names, as the formatter of its description reports them
        var used = new SortedSet<string>(StringComparer.Ordinal);
        context.NameFormatter.OnReference = (kind, id) => used.Add((kind == ReferenceKind.Switch ? "s" : "v") + id);
        NamedCommand.CommandTextParts parts;
        try
        {
            parts = command.GetTextParts(context, new DumpOptions
            {
                Format = DumpFormat.Text, WithIndent = false, AppendCommandCode = false, ExtendedFormat = options.ExtendedFormat,
            });
        }
        finally
        {
            context.NameFormatter.OnReference = null;
        }

        // \V[n] in a text is a variable too
        if (command is ShowTextLineCommand)
            foreach (System.Text.RegularExpressions.Match match in VariableCode.Matches(parts.Parameters))
                used.Add("v" + match.Groups[1].Value);

        var sb = new StringBuilder("<div class=\"cmd");
        if (command.IsSecondary)
            sb.Append(" sec");
        if (command is CallCommonEventCommand)
            sb.Append(" call");
        else if (command is ExitEventProcessingCommand)
            sb.Append(" exit");
        else if (command is CommentCommand or CommentContinueCommand)
            sb.Append(" comment");

        // labels and jumps: where a jump goes, and from where a label is jumped to
        var number = NumberFormat(options);
        var refs = "";
        string? labelAttributes = null;
        // (the indexes are those of the list the line is in; the page looks for them inside that list only, because a list can be copied into the page many times)
        if (command is LabelCommand && scope.Jumpers.TryGetValue(index, out var jumpers))
        {
            labelAttributes = $" data-lbl=\"{index}\" data-from=\"{string.Join(' ', jumpers)}\"";
            refs = "\u2190 " + string.Join(' ', jumpers.Select(j =>
                $"<a class=\"ref\" data-kind=\"jmp\" data-n=\"{j}\">{j.ToString(number)}</a>"));
        }
        else if (command is JumpToLabelCommand jump)
        {
            if (scope.Labels.TryGetValue(jump.Label ?? "", out var target))
            {
                labelAttributes = $" data-jmp=\"{index}\" data-to=\"{target}\"";
                refs = $"\u2192 <a class=\"ref\" data-kind=\"lbl\" data-n=\"{target}\">{target.ToString(number)}</a>";
                sb.Append(" jump");
            }
            else
                sb.Append(" broken-jump");
        }

        // (--lvl: the indent of the command; the page adds the indent of the call that the list is opened under)
        sb.Append("\" style=\"--lvl:").Append(IndentOf(command)).Append('"');
        if (used.Count > 0)
            sb.Append(" data-refs=\"").Append(string.Join(' ', used)).Append('"');
        sb.Append(labelAttributes);

        var images = ImagesOf(command, context);
        if (images.Count > 0)
            sb.Append(" data-pics=\"").Append(Encode(JsonSerializer.Serialize(images, JsonOptions))).Append('"');

        sb.Append('>');
        sb.Append("<span class=\"no\">").Append(index.ToString(NumberFormat(options))).Append("</span>");
        sb.Append("<span class=\"fold\"></span>");
        sb.Append("<span class=\"code\">").Append(command.Code.ToString("D3")).Append("</span>");
        sb.Append("<span class=\"ind\"></span>");
        sb.Append("<span class=\"name\">").Append(Encode(parts.Name)).Append("</span>");
        sb.Append("<span class=\"sep\">").Append(Encode(parts.Separator)).Append("</span>");
        sb.Append("<span class=\"params\">").Append(Encode(parts.Parameters)).Append("</span>");
        if (refs != "")
            sb.Append("<span class=\"refs\">").Append(refs).Append("</span>");

        var sounds = SoundsOf(command, context);
        if (images.Count + sounds.Count > 0)
        {
            sb.Append("<span class=\"acts\">");
            // the kind (sprite, face, ...) is written only for a command that holds several images, whether they are all set or not
            var withKinds = command is ChangeActorImagesCommand;
            for (var n = 0; n < images.Count; n++)
                sb.Append("<a class=\"act\" data-n=\"").Append(n).Append("\" title=\"Open in a new tab\">")
                    .Append(Encode((withKinds ? KindNames[images[n].K] + " " : "") + images[n].P.Split('/').Last())).Append("</a>");
            foreach (var sound in sounds)
                sb.Append("<a class=\"act snd\" data-p=\"").Append(Encode(sound.P)).Append("\" data-k=\"").Append(sound.K)
                    .Append("\" data-v=\"").Append(sound.V).Append("\" data-pt=\"").Append(sound.Pt)
                    .Append("\" title=\"Play / stop\">\u25B6 ").Append(Encode(sound.P.Split('/').Last())).Append("</a>");
            sb.Append("</span>");
        }

        sb.Append("</div>");
        return sb.ToString();
    }

    private static readonly System.Text.RegularExpressions.Regex VariableCode =
        new(@"\\[vV]\[(\d+)\]", System.Text.RegularExpressions.RegexOptions.Compiled);

    private static string NumberFormat(DumpOptions options) =>
        string.IsNullOrWhiteSpace(options.LineNumberFormat) ? "D3" : options.LineNumberFormat;

    /// <summary>The images a command shows: the kind (how the sheet is cut), the path without an extension, the index in the sheet.</summary>
    private record ImageRef(string K, string P, int I);

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    /// <summary>A sound a command plays: its kind (bgm, bgs, me, se), the path without an extension, and the volume and pitch of the command.</summary>
    private record SoundRef(string K, string P, int V, int Pt);

    private static List<SoundRef> SoundsOf(NamedCommand command, EntityContext context)
    {
        var mv = context.Version == StorageVersion.MV;

        SoundRef? Make(string kind, IAudioFile? file) => file == null || string.IsNullOrWhiteSpace(file.Name)
            ? null
            : new SoundRef(kind, (mv ? "audio/" + kind : "Audio/" + kind.ToUpperInvariant()) + "/" + file.Name, file.Volume, file.Pitch);

        var sound = command switch
        {
            PlayBGMCommand c => Make("bgm", c.SoundData),
            PlayBGSCommand c => Make("bgs", c.SoundData),
            PlayMECommand c => Make("me", c.SoundData),
            PlaySECommand c => Make("se", c.SoundData),
            ChangeBattleBGMCommand c => Make("bgm", c.SoundData),
            ChangeVehicleBGMCommand c => Make("bgm", c.SoundData),
            ChangeVictoryMECommand c => Make("me", c.SoundData),
            ChangeDefeatME c => Make("me", c.SoundData),
            MovementRouteCommand { MoveCommand: MovePlaySeCommand step } => Make("se", step.SoundData),
            _ => null,
        };

        return sound == null ? [] : [sound];
    }

    private static List<ImageRef> ImagesOf(NamedCommand command, EntityContext context)
    {
        var mv = context.Version == StorageVersion.MV;
        var images = new List<ImageRef>();

        void Add(string kind, string folderMv, string folderVx, string? name, int index = 0)
        {
            if (!string.IsNullOrWhiteSpace(name))
                images.Add(new ImageRef(kind, (mv ? "img/" + folderMv : "Graphics/" + folderVx) + "/" + name, index));
        }

        switch (command)
        {
            case ShowPictureCommand show:
                Add("pic", "pictures", "Pictures", show.Picture);
                break;
            case ShowTextBaseCommand text:
                Add("face", "faces", "Faces", text.Face, text.Offset);
                break;
            case ChangeActorImagesCommand actor:
                Add("char", "characters", "Characters", actor.CharacterImage, actor.CharacterImageOffset);
                Add("face", "faces", "Faces", actor.FaceImage, actor.FaceImageOffset);
                if (mv)
                    Add("sv", "sv_actors", "", actor.SwBattler);
                break;
            case ChangeVehicleImageCommand vehicle:
                Add("char", "characters", "Characters", vehicle.Image, vehicle.Offset);
                break;
            case MovementRouteCommand { MoveCommand: MoveChangeImageCommand step }:
                Add("char", "characters", "Characters", step.Image, step.Offset);
                break;
            case ChangeParallaxCommand parallax:
                Add("pic", "parallaxes", "Parallaxes", parallax.Image);
                break;
        }

        return images;
    }

    private static readonly Dictionary<string, string> KindNames = new()
    {
        ["pic"] = "picture", ["face"] = "face", ["char"] = "sprite", ["sv"] = "battler",
    };

    private static string Css => Resource("dump.css");

    private static string Script => Resource("dump.js");

    // the style and the script of the page are files of the project (Html/), packed into the dll
    private static string Resource(string name)
    {
        using var stream = typeof(HtmlDumpWriter).Assembly.GetManifestResourceStream("html-dump/" + name)
                           ?? throw new InvalidOperationException($"The resource html-dump/{name} is missing");
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    private static string Encode(string text) => WebUtility.HtmlEncode(text);




}
