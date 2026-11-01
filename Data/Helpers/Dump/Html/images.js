(function(){
var rootInput=document.getElementById('root'),tip=document.createElement('div');
tip.id='tip';document.body.appendChild(tip);
var initial=rootInput.value,key='rpgmdump-root:'+initial;
try{var saved=localStorage.getItem(key);if(saved)rootInput.value=saved;}catch(e){}
rootInput.addEventListener('change',function(){try{localStorage.setItem(key,rootInput.value);}catch(e){}});
document.getElementById('reset').onclick=function(){rootInput.value=initial;try{localStorage.removeItem(key);}catch(e){}};

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
  // a tile of an event: 16 x 16 tiles on the sheet, the engine's own arithmetic (Sprite_Character.updateTileFrame)
  if(item.k==='tile')return [Math.floor(item.i/128)%2*8+item.i%8,Math.floor(item.i%256/8)%16,16,16,2];
  if(item.k==='char'){
    // the frame of the page: its direction (2 down, 4 left, 6 right, 8 up; the rows) and its pattern (the columns); the middle frame facing down when it is not given
    var row=Math.max(0,Math.min(3,(item.d!=null?item.d:2)/2-1)),pattern=item.n!=null?item.n:1;
    if(/^[!]?[$]/.test(n))return [pattern,row,3,4,2];
    return [(item.i%4)*3+pattern,Math.floor(item.i/4)*4+row,12,8,2];
  }
  return null;
}
function load(rel,done){
  var i=0,img=new Image();
  img.onload=function(){done(img);};
  img.onerror=function(){if(++i<exts.length)img.src=base()+rel+exts[i];else done(null,base()+rel);};
  img.src=base()+rel+exts[0];
}
// the picture of an item: the whole image, or the cell of a sheet cut out
function draw(item,img){
  var c=cell(item,img);
  if(!c)return img;
  var w=img.naturalWidth/c[2],h=img.naturalHeight/c[3],out=document.createElement('canvas');
  out.width=w;out.height=h;
  out.style.width=w*c[4]+'px';out.style.height=h*c[4]+'px';
  out.getContext('2d').drawImage(img,c[0]*w,c[1]*h,w,h,0,0,w,h);
  return out;
}
// the picture of the page in the corner of its header
function renderHeads(){
  document.querySelectorAll('.headpic[data-pics]').forEach(function(host){
    host.textContent='';
    JSON.parse(host.getAttribute('data-pics')).forEach(function(item){
      load(item.p,function(img,missing){
        if(!img){host.title='not found: '+missing;return;}
        host.title=item.p.split('/').pop()+' #'+item.i+' (click: the whole image in a new tab)';
        host._src=img.src;
        host.appendChild(draw(item,img));
      });
    });
  });
}
// a click on the picture of the header opens the whole image (the sheet) in a new tab
document.addEventListener('click',function(ev){
  var host=ev.target.closest&&ev.target.closest('.headpic[data-pics]');
  if(host&&host._src)window.open(host._src,'_blank');
});
rootInput.addEventListener('change',renderHeads);
document.getElementById('reset').addEventListener('click',renderHeads);
renderHeads();
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
      var c=cell(item,img),out=draw(item,img);
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
window.rpgmBase=base;
window.rpgmInitial=initial;
})();
