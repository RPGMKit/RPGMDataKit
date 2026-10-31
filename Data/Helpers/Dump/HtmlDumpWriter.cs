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
    private static async Task BeginDocument(TextWriter writer, EntityContext context, string title)
    {
        var root = Path.GetFullPath(context.Storage.ProjectPath);

        await writer.WriteLineAsync("<!DOCTYPE html>");
        await writer.WriteLineAsync("<html><head><meta charset=\"utf-8\">");
        await writer.WriteLineAsync($"<title>{Encode(title)}</title>");
        await writer.WriteLineAsync($"<style>{Css}</style></head><body>");
        await writer.WriteLineAsync("<div class=\"bar\"><label>Project path: <input id=\"root\" size=\"60\" value=\""
                                    + Encode(root) + "\"></label> <button id=\"reset\">Reset</button> <button id=\"open\">Open all</button> <button id=\"close\">Close all</button></div>");
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
        await BeginDocument(writer, context, title);
        await writer.WriteLineAsync($"<pre class=\"head\">{Encode(header.TrimEnd())}</pre>");
        await writer.WriteLineAsync("<div class=\"cmds\">");

        var commands = container.NamedCommands;
        var lines = new StringBuilder();
        var at = 0;
        WriteLevel(lines, commands, context, options, ref at, -1);
        await writer.WriteAsync(lines.ToString());

        await writer.WriteLineAsync("</div>");
        await EndDocument(writer);
    }

    private static int IndentOf(NamedCommand command) => command.Indent ?? 0;

    // a command followed by deeper commands opens a block; the commands of the same level go on after it
    private static void WriteLevel(StringBuilder sb, IList<NamedCommand> commands, EntityContext context,
        DumpOptions options, ref int at, int parentIndent)
    {
        while (at < commands.Count && IndentOf(commands[at]) > parentIndent)
        {
            var index = at;
            var command = commands[index];
            var line = Line(command, index, context, options);
            ++at;

            if (at < commands.Count && IndentOf(commands[at]) > IndentOf(command))
            {
                sb.Append("<details open><summary>").Append(line).Append("</summary>");
                WriteLevel(sb, commands, context, options, ref at, IndentOf(command));
                sb.Append("</details>");
            }
            else
                sb.Append(line);
        }
    }

    private static string Line(NamedCommand command, int index, EntityContext context, DumpOptions options)
    {
        var parts = command.GetTextParts(context, new DumpOptions
        {
            Format = DumpFormat.Text, WithIndent = false, AppendCommandCode = false, ExtendedFormat = options.ExtendedFormat,
        });

        var sb = new StringBuilder("<div class=\"cmd");
        if (command.IsSecondary)
            sb.Append(" sec");
        sb.Append('"');

        var images = ImagesOf(command, context);
        if (images.Count > 0)
            sb.Append(" data-pics=\"").Append(Encode(JsonSerializer.Serialize(images, JsonOptions))).Append('"');

        sb.Append('>');
        sb.Append("<span class=\"no\">").Append(index.ToString(NumberFormat(options))).Append("</span>");
        sb.Append("<span class=\"fold\"></span>");
        sb.Append("<span class=\"code\">").Append(command.Code.ToString("D3")).Append("</span>");
        sb.Append("<span class=\"ind\" style=\"width:").Append((IndentOf(command) * 1.4).ToString(System.Globalization.CultureInfo.InvariantCulture)).Append("em\"></span>");
        sb.Append("<span class=\"name\">").Append(Encode(parts.Name)).Append("</span>");
        sb.Append("<span class=\"sep\">").Append(Encode(parts.Separator)).Append("</span>");
        sb.Append("<span class=\"params\">").Append(Encode(parts.Parameters)).Append("</span>");
        if (images.Count > 0)
        {
            sb.Append("<span class=\"acts\">");
            for (var n = 0; n < images.Count; n++)
                sb.Append("<a class=\"act\" data-n=\"").Append(n).Append("\" title=\"Open in a new tab\">")
                    .Append(Encode(KindNames[images[n].K] + " " + images[n].P.Split('/').Last())).Append("</a>");
            sb.Append("</span>");
        }

        sb.Append("</div>");
        return sb.ToString();
    }

    private static string NumberFormat(DumpOptions options) =>
        string.IsNullOrWhiteSpace(options.LineNumberFormat) ? "D3" : options.LineNumberFormat;

    /// <summary>The images a command shows: the kind (how the sheet is cut), the path without an extension, the index in the sheet.</summary>
    private record ImageRef(string K, string P, int I);

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

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
        summary{cursor:pointer;list-style:none}
        summary::-webkit-details-marker{display:none}
        summary .fold::before{content:"\25B6"}
        details[open]>summary .fold::before{content:"\25BC"}
        .acts{margin-left:1em;user-select:none}
        .act{margin-left:.6em;padding:0 .5em;border:1px solid #9ab;border-radius:9px;font-size:11px;color:#06c;cursor:pointer;text-decoration:none}
        .act:hover{background:#06c;color:#fff}
        #tip{position:fixed;display:none;z-index:9;pointer-events:none;background:#fff;border:1px solid #888;border-radius:6px;padding:4px;box-shadow:0 4px 16px #0006}
        #tip .item{display:inline-block;vertical-align:top;margin:2px;text-align:center;font-size:11px}
        #tip canvas,#tip img{display:block;max-width:320px;max-height:320px;image-rendering:pixelated;background:repeating-conic-gradient(#8884 0% 25%,transparent 0% 50%) 50%/14px 14px}
        @media (prefers-color-scheme:dark){
        body{background:#1e1e1e;color:#ddd}.bar{background:#2b2b2b;border-color:#444}.head{background:#252526;border-color:#444}
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
          var act=ev.target.closest('.act');
          show(el,ev,act?+act.getAttribute('data-n'):null);
        });
        document.addEventListener('click',function(ev){
          var link=ev.target.closest&&ev.target.closest('.act');
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
