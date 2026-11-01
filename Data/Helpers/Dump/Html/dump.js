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
document.addEventListener('toggle',function(ev){
  var d=ev.target;
  if(!d.classList||!d.classList.contains('ce')||!d.open||d.querySelector(':scope > .ce-body'))return;
  var tpl=document.getElementById('ce-'+d.getAttribute('data-ce'));
  if(!tpl)return;
  var call=d.querySelector('summary > .cmd');
  var body=document.createElement('div');body.className='ce-body';
  body.style.setProperty('--pad',parseFloat(call.style.getPropertyValue('--lvl'))||0);
  body.appendChild(tpl.content.cloneNode(true));
  d.appendChild(body);
  markRecursions(body,chainOf(d));
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
// the calls of common events are not opened by "Open all": together they hold many times the lines of the page
function setAll(open){document.querySelectorAll(open?'details:not(.ce)':'details').forEach(function(d){d.open=open;});}
document.getElementById('open').onclick=function(){setAll(true);};
document.getElementById('close').onclick=function(){setAll(false);};
})();
