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
    private static async Task BeginDocument(TextWriter writer, EntityContext context, DumpOptions options, string title)
    {
        var root = Path.GetFullPath(context.Storage.ProjectPath);

        await writer.WriteLineAsync("<!DOCTYPE html>");
        await writer.WriteLineAsync("<html><head><meta charset=\"utf-8\">");
        await writer.WriteLineAsync($"<title>{Encode(title)}</title>");
        await writer.WriteLineAsync($"<style>{Css}</style></head>");
        // the file knows whether the common events can be expanded; the page has the checkbox only then
        await writer.WriteLineAsync(options.ExpandCommonEvents ? "<body data-expand-common-events>" : "<body>");
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
        var title = header.Split('\n')[0].Trim();
        await BeginDocument(writer, context, options, title);
        await writer.WriteLineAsync($"<pre class=\"head\" id=\"top\">{Encode(header.TrimEnd())}</pre>");
        await writer.WriteLineAsync("<div class=\"cmds\">");

        var state = new RenderState(context, options);
        // a common event that is dumped is the start of the chain of its own calls
        if (container is ICommonEvent commonEvent)
            state.Chain.Add((commonEvent.Id, "top"));

        var lines = new StringBuilder();
        var at = 0;
        WriteLevel(lines, container.NamedCommands, state, ref at, -1, 0);
        await writer.WriteAsync(lines.ToString());

        await writer.WriteLineAsync("</div>");
        await EndDocument(writer);
    }

    private sealed class RenderState(EntityContext context, DumpOptions options)
    {
        public EntityContext Context { get; } = context;
        public DumpOptions Options { get; } = options;

        /// <summary>The number of the next expanded call, for the id of its line.</summary>
        public int Next;

        /// <summary>The common events that are open now (the one that is dumped, the calls expanded around the line) and the ids of their lines.</summary>
        public List<(int EventId, string Anchor)> Chain { get; } = [];
    }

    private static int IndentOf(NamedCommand command) => command.Indent ?? 0;

    // a command followed by deeper commands opens a block; the commands of the same level go on after it;
    // shift: the indent of the call that the list is expanded under
    private static void WriteLevel(StringBuilder sb, IList<NamedCommand> commands, RenderState state, ref int at,
        int parentIndent, int shift)
    {
        while (at < commands.Count && IndentOf(commands[at]) > parentIndent)
        {
            var index = at;
            var command = commands[index];
            ++at;

            if (state.Options.ExpandCommonEvents && command is CallCommonEventCommand call
                && state.Context.Storage.CommonEvents.Get(call.EventId) is { } called)
            {
                var start = state.Chain.FindIndex(link => link.EventId == call.EventId);
                if (start >= 0)
                {
                    // the event is in the chain of the calls already: not expanded, the line leads to where the chain began
                    sb.Append(Line(command, index, state, shift, null, state.Chain[start].Anchor));
                    continue;
                }

                var anchor = "ce-" + state.Next++;
                sb.Append("<details class=\"ce\"><summary>").Append(Line(command, index, state, shift, anchor, null)).Append("</summary>");
                state.Chain.Add((call.EventId, anchor));
                var inner = 0;
                WriteLevel(sb, called.NamedCommands, state, ref inner, -1, shift + IndentOf(command) + 1);
                state.Chain.RemoveAt(state.Chain.Count - 1);
                sb.Append("</details>");
                continue;
            }

            var line = Line(command, index, state, shift, null, null);

            if (at < commands.Count && IndentOf(commands[at]) > IndentOf(command))
            {
                sb.Append("<details open><summary>").Append(line).Append("</summary>");
                WriteLevel(sb, commands, state, ref at, IndentOf(command), shift);
                sb.Append("</details>");
            }
            else
                sb.Append(line);
        }
    }

    private static string Line(NamedCommand command, int index, RenderState state, int shift, string? anchor, string? gotoAnchor)
    {
        var context = state.Context;
        var options = state.Options;
        var parts = command.GetTextParts(context, new DumpOptions
        {
            Format = DumpFormat.Text, WithIndent = false, AppendCommandCode = false, ExtendedFormat = options.ExtendedFormat,
        });

        var sb = new StringBuilder("<div class=\"cmd");
        if (command.IsSecondary)
            sb.Append(" sec");
        if (command is CallCommonEventCommand)
            sb.Append(" call");
        else if (command is ExitEventProcessingCommand)
            sb.Append(" exit");
        if (gotoAnchor != null)
            sb.Append(" recursion");
        sb.Append('"');
        if (anchor != null)
            sb.Append(" id=\"").Append(anchor).Append('"');
        if (gotoAnchor != null)
            sb.Append(" data-goto=\"").Append(gotoAnchor).Append('"');

        var images = ImagesOf(command, context);
        if (images.Count > 0)
            sb.Append(" data-pics=\"").Append(Encode(JsonSerializer.Serialize(images, JsonOptions))).Append('"');

        sb.Append('>');
        sb.Append("<span class=\"no\">").Append(index.ToString(NumberFormat(options))).Append("</span>");
        sb.Append("<span class=\"fold\"></span>");
        sb.Append("<span class=\"code\">").Append(command.Code.ToString("D3")).Append("</span>");
        sb.Append("<span class=\"ind\" style=\"width:").Append(((IndentOf(command) + shift) * 1.4).ToString(System.Globalization.CultureInfo.InvariantCulture)).Append("em\"></span>");
        sb.Append("<span class=\"name\">").Append(Encode(parts.Name)).Append("</span>");
        sb.Append("<span class=\"sep\">").Append(Encode(parts.Separator)).Append("</span>");
        sb.Append("<span class=\"params\">").Append(Encode(parts.Parameters)).Append("</span>");
        var sounds = SoundsOf(command, context);
        if (images.Count + sounds.Count > 0)
        {
            sb.Append("<span class=\"acts\">");
            for (var n = 0; n < images.Count; n++)
                sb.Append("<a class=\"act\" data-n=\"").Append(n).Append("\" title=\"Open in a new tab\">")
                    .Append(Encode(KindNames[images[n].K] + " " + images[n].P.Split('/').Last())).Append("</a>");
            foreach (var sound in sounds)
                sb.Append("<a class=\"act snd\" data-p=\"").Append(Encode(sound.P)).Append("\" data-k=\"").Append(sound.K)
                    .Append("\" data-v=\"").Append(sound.V).Append("\" data-pt=\"").Append(sound.Pt)
                    .Append("\" title=\"Play / stop\">\u25B6 ").Append(sound.K).Append(' ').Append(Encode(sound.P.Split('/').Last())).Append("</a>");
            sb.Append("</span>");
        }

        if (gotoAnchor != null)
            sb.Append("<span class=\"rec\">\u21BB recursion: click to go to where it began</span>");

        sb.Append("</div>");
        return sb.ToString();
    }

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
