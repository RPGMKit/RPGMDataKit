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

    private static string Encode(string text) => WebUtility.HtmlEncode(text);

    private const string Css = """
        body{font:13px/1.45 Consolas,monospace;margin:0;background:#fff;color:#222}
        .bar{position:sticky;top:0;background:#eee;padding:6px 10px;border-bottom:1px solid #bbb;z-index:2}
        .head{margin:0;padding:8px 10px;background:#f6f6f6;border-bottom:1px solid #ddd}
        .cmds{padding:4px 0}
        .cmd{white-space:pre-wrap;padding:0 10px}
        .cmd:hover{background:#eef4ff}
        .no{display:inline-block;min-width:3.2em;color:#888;text-align:right;margin-right:.8em;user-select:none}
        .code{display:inline-block;min-width:3em;color:#aaa;margin-right:.6em;user-select:none}
        .name{font-weight:bold}
        .sec .name{font-weight:normal}
        .cmd[data-pics] .params{color:#0a5;text-decoration:underline dotted}
        .fold{display:inline-block;width:1.3em;color:#888;user-select:none}
        .ind{display:inline-block}
        .call .name,.call .params{color:#0b6fa8}
        .exit .name{color:#c0392b}
        details.ce>summary .cmd{border-left:3px solid #6a9fd8}
        .recursion{cursor:pointer;color:#a60}
        .rec{margin-left:1em;font-size:11px}
        .flash{animation:flash 1.5s}
        @keyframes flash{from{background:#ffd54f}to{background:transparent}}
        body.no-ce details.ce>summary .fold::before{content:""}
        body.no-ce details.ce>:not(summary){display:none}
        body.no-ce .rec{display:none}
        body.no-ce .recursion{cursor:default}
        summary{cursor:pointer;list-style:none}
        summary::-webkit-details-marker{display:none}
        summary .fold::before{content:"\25B6"}
        details[open]>summary .fold::before{content:"\25BC"}
        .acts{margin-left:1em;user-select:none}
        .act{margin-left:.6em;padding:0 .5em;border:1px solid #9ab;border-radius:9px;font-size:11px;color:#06c;cursor:pointer;text-decoration:none}
        .act:hover{background:#06c;color:#fff}
        .snd{color:#a05a00;border-color:#c9a063}
        .snd:hover{background:#a05a00;color:#fff}
        .snd.playing{background:#a05a00;color:#fff}
        #tip{position:fixed;display:none;z-index:9;pointer-events:none;background:#fff;border:1px solid #888;border-radius:6px;padding:4px;box-shadow:0 4px 16px #0006}
        #tip .item{display:inline-block;vertical-align:top;margin:2px;text-align:center;font-size:11px}
        #tip canvas,#tip img{display:block;max-width:320px;max-height:320px;image-rendering:pixelated;background:repeating-conic-gradient(#8884 0% 25%,transparent 0% 50%) 50%/14px 14px}
        @media (prefers-color-scheme:dark){
        body{background:#1e1e1e;color:#ddd}.bar{background:#2b2b2b;border-color:#444}.head{background:#252526;border-color:#444}
        .call .name,.call .params{color:#5ab4ea}.exit .name{color:#ff7b72}
        .cmd:hover{background:#2a3445}#tip{background:#222;border-color:#666}}
        """;

    private const string Script = """
        (function(){
        var rootInput=document.getElementById('root'),tip=document.createElement('div');
        tip.id='tip';document.body.appendChild(tip);
        var initial=rootInput.value,key='rpgmdump-root:'+initial;
        try{var saved=localStorage.getItem(key);if(saved)rootInput.value=saved;}catch(e){}
        rootInput.addEventListener('change',function(){try{localStorage.setItem(key,rootInput.value);}catch(e){}});
        document.getElementById('reset').onclick=function(){rootInput.value=initial;try{localStorage.removeItem(key);}catch(e){}};
        var expand=document.getElementById('expand');
        function applyExpand(){document.body.classList.toggle('no-ce',!!expand&&!expand.checked);}
        if(expand){
          // one setting for all the files, it is a habit of the reader
          try{var was=localStorage.getItem('rpgmdump-expand');if(was!==null)expand.checked=was==='1';}catch(e){}
          expand.addEventListener('change',function(){try{localStorage.setItem('rpgmdump-expand',expand.checked?'1':'0');}catch(e){}applyExpand();});
          applyExpand();
        }
        document.addEventListener('click',function(ev){
          var line=ev.target.closest&&ev.target.closest('.recursion');
          if(!line||document.body.classList.contains('no-ce'))return;
          var target=document.getElementById(line.getAttribute('data-goto'));
          if(!target)return;
          target.scrollIntoView({block:'center'});
          target.classList.remove('flash');void target.offsetWidth;target.classList.add('flash');
        });
        // sounds: one plays at a time; the volume and the pitch are the ones of the command; bgm and bgs loop
        var soundExts=['.ogg','.m4a','.mp3','.wav','.mid','.wma'],playing=null;
        function stopSound(){
          if(!playing)return;
          playing.audio.pause();playing.link.classList.remove('playing');
          playing.link.textContent=playing.link.textContent.replace('\u25A0','\u25B6');playing=null;
        }
        document.addEventListener('click',function(ev){
          var link=ev.target.closest&&ev.target.closest('.snd');
          if(!link)return;
          ev.preventDefault();
          if(playing&&playing.link===link){stopSound();return;}
          stopSound();
          var rel=link.getAttribute('data-p'),kind=link.getAttribute('data-k'),i=0,audio=new Audio();
          audio.volume=Math.max(0,Math.min(1,+link.getAttribute('data-v')/100));
          audio.preservesPitch=false;audio.playbackRate=Math.max(0.25,Math.min(4,+link.getAttribute('data-pt')/100));
          audio.loop=kind==='bgm'||kind==='bgs';
          audio.onerror=function(){
            if(++i<soundExts.length)audio.src=base()+rel+soundExts[i];
            else{stopSound();link.title='Cannot play: '+base()+rel;}
          };
          audio.onended=function(){if(playing&&playing.audio===audio)stopSound();};
          audio.src=base()+rel+soundExts[0];
          playing={audio:audio,link:link};
          link.classList.add('playing');link.textContent=link.textContent.replace('\u25B6','\u25A0');
          audio.play().catch(function(){});
        });
        function setAll(open){document.querySelectorAll('details').forEach(function(d){d.open=open;});}
        document.getElementById('open').onclick=function(){setAll(true);};
        document.getElementById('close').onclick=function(){setAll(false);};
        var exts=['.png','.jpg','.jpeg','.bmp'];
        function base(){
          var r=rootInput.value.trim().replace(/\\/g,'/');
          if(!/^[a-z][a-z0-9+.-]*:\/\//i.test(r)) r=(r.charAt(0)==='/'?'file://':'file:///')+r.split('/').map(function(p,i){return i===0&&/^[a-z]:$/i.test(p)?p:encodeURIComponent(p);}).join('/');
          return r.endsWith('/')?r:r+'/';
        }
        // where the cell of a sheet is: [column, row, columns, rows, scale]
        function cell(item,img){
          var n=item.p.split('/').pop();
          if(item.k==='face')return [item.i%4,Math.floor(item.i/4),4,2,1];
          if(item.k==='sv')return [0,0,9,6,2];
          if(item.k==='char'){
            if(/^[!]?[$]/.test(n))return [1,0,3,4,2];
            return [(item.i%4)*3+1,Math.floor(item.i/4)*4,12,8,2];
          }
          return null;
        }
        function load(rel,done){
          var i=0,img=new Image();
          img.onload=function(){done(img);};
          img.onerror=function(){if(++i<exts.length)img.src=base()+rel+exts[i];else done(null,base()+rel);};
          img.src=base()+rel+exts[0];
        }
        // only: the number of the one image to show (the action of an image); none shows all the images of the command
        function show(el,ev,only){
          var items=JSON.parse(el.getAttribute('data-pics')),token=el._token=(el._token||0)+1;
          if(only!=null)items=[items[only]];
          tip.textContent='';tip.style.display='block';move(ev);
          items.forEach(function(item,at){
            var box=document.createElement('div');box.className='item';tip.appendChild(box);
            load(item.p,function(img,missing){
              if(el._token!==token)return;
              if(!img){box.textContent='not found: '+missing;move(ev);return;}
              var c=cell(item,img),out;
              if(!c)out=img;
              else{
                var w=img.naturalWidth/c[2],h=img.naturalHeight/c[3];
                out=document.createElement('canvas');out.width=w;out.height=h;
                out.style.width=w*c[4]+'px';out.style.height=h*c[4]+'px';
                out.getContext('2d').drawImage(img,c[0]*w,c[1]*h,w,h,0,0,w,h);
              }
              box.appendChild(out);
              if(only==null&&items.length>1||c){var cap=document.createElement('div');cap.textContent=item.p.split('/').pop()+(c?' #'+item.i:'');box.appendChild(cap);}
              move(ev);
            });
          });
        }
        function move(ev){
          var x=ev.clientX+16,y=ev.clientY+16;
          tip.style.left=Math.min(x,window.innerWidth-tip.offsetWidth-4)+'px';
          tip.style.top=Math.max(4,Math.min(y,window.innerHeight-tip.offsetHeight-4))+'px';
        }
        document.addEventListener('mouseover',function(ev){
          var el=ev.target.closest&&ev.target.closest('[data-pics]');
          if(!el)return;
          var act=ev.target.closest('.act:not(.snd)');
          show(el,ev,act?+act.getAttribute('data-n'):null);
        });
        document.addEventListener('click',function(ev){
          var link=ev.target.closest&&ev.target.closest('.act:not(.snd)');
          if(!link)return;
          ev.preventDefault();
          var item=JSON.parse(link.closest('[data-pics]').getAttribute('data-pics'))[+link.getAttribute('data-n')];
          var win=window.open('','_blank');if(!win)return;
          load(item.p,function(img,missing){if(img)win.location=img.src;else win.document.write('not found: '+missing);});
        });
        document.addEventListener('mousemove',function(ev){if(tip.style.display==='block')move(ev);});
        document.addEventListener('mouseout',function(ev){var el=ev.target.closest&&ev.target.closest('[data-pics]');if(el)tip.style.display='none';});
        })();
        """;
}
