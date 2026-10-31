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
