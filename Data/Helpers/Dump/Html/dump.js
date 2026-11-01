(function(){
var expand=document.getElementById('expand');
function applyExpand(){document.body.classList.toggle('no-ce',!!expand&&!expand.checked);if(typeof scheduleRestripe==='function')scheduleRestripe();}
if(expand){
  // one setting for all the files, it is a habit of the reader
  try{var was=localStorage.getItem('rpgmdump-expand');if(was!==null)expand.checked=was==='1';}catch(e){}
  expand.addEventListener('change',function(){try{localStorage.setItem('rpgmdump-expand',expand.checked?'1':'0');}catch(e){}applyExpand();});
  applyExpand();
}
// a list of commands is a .list; its lines are numbered from 0, and a list can be on the page many times (a common event is copied where a call is opened),
// so a label or a jump is looked for by its index inside its own list
function inList(list,attr,n){
  var found=list.querySelectorAll('['+attr+'="'+n+'"]');
  for(var i=0;i<found.length;i++)if(found[i].closest('.list')===list)return found[i];
  return null;
}
function goTo(target){
  if(!target)return;
  for(var p=target.parentElement;p;p=p.parentElement)if(p.tagName==='DETAILS')p.open=true;
  target.scrollIntoView({block:'center'});
  target.classList.remove('flash');void target.offsetWidth;target.classList.add('flash');
}
// the common events that are open around a node (the nearest first), and the one that is dumped
function chainOf(node){
  var ids=[];
  for(var p=node;p;p=p.parentElement)if(p.tagName==='DETAILS'&&p.classList.contains('ce'))ids.push(p.getAttribute('data-ce'));
  var top=document.body.getAttribute('data-top-ce');
  if(top!==null)ids.push(top);
  return ids;
}
// a call of an event that is open already is not opened again: it is a line that leads to where that event was opened
function markRecursions(root,chain){
  root.querySelectorAll('details.ce').forEach(function(d){
    if(chain.indexOf(d.getAttribute('data-ce'))<0)return;
    var line=d.querySelector('summary > .cmd');
    line.classList.add('recursion');
    line.setAttribute('data-goto-ce',d.getAttribute('data-ce'));
    var note=document.createElement('span');note.className='rec';note.textContent='\u21BB recursion: click to go to where it began';
    line.appendChild(note);
    d.replaceWith(line);
  });
}
function recursionTarget(line,id){
  for(var p=line.parentElement;p;p=p.parentElement)
    if(p.tagName==='DETAILS'&&p.classList.contains('ce')&&p.getAttribute('data-ce')===id)return p.querySelector('summary > .cmd');
  return document.getElementById('top');
}
markRecursions(document,chainOf(document.body));
restripe();
// the stripes go by the lines that can be seen: folding a block, opening a call or the checkbox of the calls does not break the sequence.
// (the page comes with the stripes of the numbers of the lines in their lists, which is what shows without the script)
function shown(el){
  var noCalls=document.body.classList.contains('no-ce');
  for(var p=el.parentElement;p;p=p.parentElement){
    if(p.tagName!=='DETAILS')continue;
    var inSummary=p.firstElementChild&&p.firstElementChild.contains(el);
    if(inSummary)continue;
    if(!p.open||(noCalls&&p.classList.contains('ce')))return false;
  }
  return true;
}
function restripe(){
  var n=0;
  document.querySelectorAll('.cmd').forEach(function(el){if(shown(el))el.classList.toggle('alt',n++%2===1);});
}
var stripeTimer=0;
function scheduleRestripe(){clearTimeout(stripeTimer);stripeTimer=setTimeout(restripe,0);}
document.addEventListener('toggle',scheduleRestripe,true);
// a call is filled from the list of its event when it is opened the first time
function fill(d){
  if(d.querySelector(':scope > .ce-body'))return false;
  var tpl=document.getElementById('ce-'+d.getAttribute('data-ce'));
  if(!tpl)return false;
  var call=d.querySelector('summary > .cmd');
  var body=document.createElement('div');body.className='ce-body';
  body.style.setProperty('--pad',parseFloat(call.style.getPropertyValue('--lvl'))||0);
  body.appendChild(tpl.content.cloneNode(true));
  d.appendChild(body);
  markRecursions(body,chainOf(d));
  return true;
}
document.addEventListener('toggle',function(ev){
  var d=ev.target;
  if(d.classList&&d.classList.contains('ce')&&d.open)fill(d);
},true);
document.addEventListener('click',function(ev){
  var el=ev.target.closest?ev.target.closest('.ref,.jump,.recursion'):null;
  if(!el)return;
  var line=el.closest('.cmd'),list=line.closest('.list');
  if(el.classList.contains('ref'))goTo(inList(list,el.getAttribute('data-kind')==='jmp'?'data-jmp':'data-lbl',el.getAttribute('data-n')));
  else if(el.classList.contains('jump'))goTo(inList(list,'data-lbl',el.getAttribute('data-to')));
  else if(!document.body.classList.contains('no-ce'))goTo(recursionTarget(el,el.getAttribute('data-goto-ce')));
});
// the target of a jump, or all the jumps to a label, are lit while the line is under the mouse
function mark(line,on){
  var list=line.closest('.list'),lit=[];
  if(line.hasAttribute('data-to'))lit.push(inList(list,'data-lbl',line.getAttribute('data-to')));
  if(line.hasAttribute('data-from'))line.getAttribute('data-from').split(' ').forEach(function(n){lit.push(inList(list,'data-jmp',n));});
  lit.forEach(function(el){if(el)el.classList.toggle('hl',on);});
}
document.addEventListener('mouseover',function(ev){var l=ev.target.closest&&ev.target.closest('.cmd[data-to],.cmd[data-from]');if(l)mark(l,true);});
document.addEventListener('mouseout',function(ev){var l=ev.target.closest&&ev.target.closest('.cmd[data-to],.cmd[data-from]');if(l)mark(l,false);});
// the other lines that use a switch or a variable of the line under the mouse
var lit=[];
function light(line,on){
  if(!on){lit.forEach(function(el){el.classList.remove('same');});lit=[];return;}
  line.getAttribute('data-refs').split(' ').forEach(function(ref){
    document.querySelectorAll('.cmd[data-refs~="'+ref+'"]').forEach(function(el){
      if(el!==line&&lit.indexOf(el)<0){el.classList.add('same');lit.push(el);}
    });
  });
}
document.addEventListener('mouseover',function(ev){var l=ev.target.closest&&ev.target.closest('.cmd[data-refs]');if(l){light(l,false);light(l,true);}});
document.addEventListener('mouseout',function(ev){var l=ev.target.closest&&ev.target.closest('.cmd[data-refs]');if(l)light(l,false);});
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
    if(++i<soundExts.length)audio.src=rpgmBase()+rel+soundExts[i];
    else{stopSound();link.title='Cannot play: '+rpgmBase()+rel;}
  };
  audio.onended=function(){if(playing&&playing.audio===audio)stopSound();};
  audio.src=rpgmBase()+rel+soundExts[0];
  playing={audio:audio,link:link};
  link.classList.add('playing');link.textContent=link.textContent.replace('\u25B6','\u25A0');
  audio.play().catch(function(){});
});
// "Open all" opens the calls of common events too, all the way down. The calls together can hold many times the lines of the page (a tree of calls, written out, can run to millions),
// so the lines that would be added are counted from the lists of the events first, and the reader is asked when there are many
var openAllWarn=50000;
function expandedLines(id,stack,sizes){
  if(id in sizes)return sizes[id];
  if(stack.indexOf(id)>=0)return 0;
  var tpl=document.getElementById('ce-'+id);
  if(!tpl)return 0;
  var n=tpl.content.querySelectorAll('.cmd').length;
  tpl.content.querySelectorAll('details.ce').forEach(function(d){n+=expandedLines(d.getAttribute('data-ce'),stack.concat([id]),sizes);});
  return sizes[id]=n;
}
function closedCalls(){return [].slice.call(document.querySelectorAll('details.ce')).filter(function(d){return !d.querySelector(':scope > .ce-body')&&!d.hasAttribute('data-nolist');});}
function openAllCalls(){
  var sizes={},total=0;
  closedCalls().forEach(function(d){total+=expandedLines(d.getAttribute('data-ce'),chainOf(d),sizes);});
  if(total>openAllWarn&&!confirm('Opening all the calls of common events adds about '+total.toLocaleString()+' lines to the page, which can be very slow. Open them anyway?'))return;
  // a pass opens the calls that are closed; what they hold is closed, and the next pass opens it (a recursion is not a call any more)
  for(var pass=closedCalls();pass.length;pass=closedCalls())pass.forEach(function(d){d.open=true;if(!fill(d))d.setAttribute('data-nolist','');});
  document.querySelectorAll('details.ce').forEach(function(d){d.open=true;});
}
function setAll(open){
  document.querySelectorAll(open?'details:not(.ce)':'details').forEach(function(d){d.open=open;});
  if(open&&!document.body.classList.contains('no-ce'))openAllCalls();
  scheduleRestripe();
}
// the lines to mark are those of the address (page.html#lines=12,40,55); the numbers are those of the lines of the page itself, not of a call that is opened in it.
// The page is scrolled to the first of them, and the bar has a link to each
function pickedLines(){
  var found=[],texts=[],hash=(location.hash||'').replace(/^#/,'');
  hash.split('&').forEach(function(part){var eq=part.indexOf('=');if(part.slice(0,eq)==='lines')texts.push(decodeURIComponent(part.slice(eq+1)).replace(/,/g,' '));});
  texts.join(' ').split(/\s+/).forEach(function(s){var n=parseInt(s,10);if(!isNaN(n)&&n>=0&&found.indexOf(n)<0&&/^\d+$/.test(s))found.push(n);});
  return found;
}
function pick(){
  var top=document.querySelector('.cmds.list'),bar=document.getElementById('picked');
  document.querySelectorAll('.cmd.pick').forEach(function(el){el.classList.remove('pick');});
  if(bar)bar.textContent='';
  var targets=[];
  pickedLines().forEach(function(n){var el=top&&inList(top,'data-line',n);if(el){el.classList.add('pick');targets.push({n:n,el:el});}});
  if(!bar||!targets.length)return targets;
  bar.appendChild(document.createTextNode('Marked: '));
  targets.forEach(function(t){
    var a=document.createElement('a');a.href='#';a.className='act';a.textContent=t.el.querySelector('.no').textContent.trim();
    a.onclick=function(ev){ev.preventDefault();goTo(t.el);};
    bar.appendChild(a);
  });
  return targets;
}
var picked=pick();
// (opened at the first of them, in the middle of the window; the blocks around it are opened)
// (a reload would put the page back where it was scrolled, not at the lines)
if(picked.length){try{history.scrollRestoration='manual';}catch(e){}goTo(picked[0].el);}
window.addEventListener('hashchange',function(){var again=pick();if(again.length)goTo(again[0].el);});
document.getElementById('open').onclick=function(){setAll(true);};
document.getElementById('close').onclick=function(){setAll(false);};
})();
