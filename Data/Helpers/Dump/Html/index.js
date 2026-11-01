(function(){
function $(id){return document.getElementById(id);}
var filter=$('filter'),treeBox=$('tree'),mapsRoot=$('maps'),eventsPane=$('events');
var mapList=[].slice.call(mapsRoot.querySelectorAll('.map')),byId={};
mapList.forEach(function(m){byId[m.getAttribute('data-id')]=m;});
var tabs=['ce','maps','troops'],current='ce',selected=null;
var treePane=document.querySelector('.tree'),stateKey='rpgmindex-state:'+window.rpgmInitial,saved={},scrollPos={};
try{saved=JSON.parse(sessionStorage.getItem(stateKey)||'{}')||{};}catch(e){}
scrollPos=saved.scroll||{};
try{history.scrollRestoration='manual';}catch(e){}

// the height of the tab of the maps is what the bar leaves
function sizePanes(){document.documentElement.style.setProperty('--bar-h',document.querySelector('.bar').offsetHeight+'px');}
window.addEventListener('resize',sizePanes);

// the place is kept for the way back from a page (the browser keeps it for the tab only): the tab, the map, the branches that are open, the filter and where the lists are scrolled;
// the address has the tab and the map too (#maps/12)
function saveState(){
  try{
    sessionStorage.setItem(stateKey,JSON.stringify({
      tab:current,map:selected?selected.getAttribute('data-id'):null,filter:filter.value,scroll:scrollPos,
      open:mapList.filter(function(m){return m.classList.contains('open');}).map(function(m){return m.getAttribute('data-id');}),
      treeScroll:treePane.scrollTop,eventsScroll:eventsPane.scrollTop
    }));
  }catch(e){}
}
function save(){
  try{history.replaceState(null,'','#'+current+(current==='maps'&&selected?'/'+selected.getAttribute('data-id'):''));}catch(e){}
  saveState();
}
var saveTimer=0;
function saveSoon(){clearTimeout(saveTimer);saveTimer=setTimeout(saveState,200);}
window.addEventListener('scroll',function(){if(current!=='maps')scrollPos[current]=window.scrollY;saveSoon();});
treePane.addEventListener('scroll',saveSoon);
eventsPane.addEventListener('scroll',saveSoon);
window.addEventListener('pagehide',saveState);
document.addEventListener('visibilitychange',function(){if(document.visibilityState==='hidden')saveState();});
function showTab(name){
  current=name;
  tabs.forEach(function(t){
    $('tab-'+t).classList.toggle('active',t===name);
    document.querySelector('[data-tab="'+t+'"]').classList.toggle('active',t===name);
  });
  sizePanes();save();
  window.scrollTo(0,name==='maps'?0:scrollPos[name]||0);
}
document.querySelectorAll('[data-tab]').forEach(function(b){b.addEventListener('click',function(){showTab(b.getAttribute('data-tab'));});});

// every second row of a list is a little darker, counting the rows that can be seen: a filter or a folded branch does not break the sequence
function stripe(rows,seen){
  var n=0;
  rows.forEach(function(r){if(!seen||seen(r))r.classList.toggle('alt',n++%2===1);});
}
function visibleRow(r){return !r.hidden;}
// a row of a map can be seen when its map is not hidden and every map around it is open
function mapRowSeen(row){
  var m=row.parentElement;
  if(m.hidden)return false;
  for(var p=m.parentElement.closest('.map');p;p=p.parentElement.closest('.map'))if(p.hidden||!p.classList.contains('open'))return false;
  return true;
}
function restripe(){
  stripe([].slice.call(document.querySelectorAll('#tab-ce .row')),visibleRow);
  stripe([].slice.call(document.querySelectorAll('#tab-troops .row')),visibleRow);
  stripe([].slice.call(eventsPane.querySelectorAll('.map-events:not([hidden]) .event')),visibleRow);
  stripe(mapList.map(function(m){return m.querySelector(':scope > .row');}),mapRowSeen);
}

// the maps come in the order of their numbers; as a tree they go into the list of their parent, in the order of the editor
function arrange(){
  var tree=treeBox.checked;
  // a parent that is not a map of the list, or that leads back to the map (a damaged list), is no parent
  function parentOf(m){
    var p=byId[m.getAttribute('data-parent')];
    for(var seen=0,x=p;x&&seen<5000;x=byId[x.getAttribute('data-parent')],seen++)if(x===m)return null;
    return p||null;
  }
  mapList.slice().sort(function(a,b){
    return tree?(+a.getAttribute('data-order'))-(+b.getAttribute('data-order')):(+a.getAttribute('data-id'))-(+b.getAttribute('data-id'));
  }).forEach(function(m){
    var p=tree?parentOf(m):null;
    (p?p.querySelector(':scope > .kids'):mapsRoot).appendChild(m);
  });
  mapList.forEach(function(m){m.classList.toggle('haskids',m.querySelector(':scope > .kids').children.length>0);});
  restripe();
}
treeBox.checked=true;
try{if(localStorage.getItem('rpgmindex-tree')==='0')treeBox.checked=false;}catch(e){}
treeBox.addEventListener('change',function(){try{localStorage.setItem('rpgmindex-tree',treeBox.checked?'1':'0');}catch(e){}arrange();});
mapList.forEach(function(m){if(m.getAttribute('data-open')==='1')m.classList.add('open');});
arrange();

// a map is chosen: its events are the right side
function selectMap(m){
  if(selected)selected.classList.remove('selected');
  selected=m;
  if(m){
    m.classList.add('selected');
    for(var p=m.parentElement.closest('.map');p;p=p.parentElement.closest('.map'))p.classList.add('open');
  }
  eventsPane.querySelectorAll('.map-events').forEach(function(s){s.hidden=!m||s.getAttribute('data-for')!==m.getAttribute('data-id');});
  eventsPane.querySelector('.hint').hidden=!!m;
  applyFilter();save();
}
mapsRoot.addEventListener('click',function(ev){
  var row=ev.target.closest('.row');
  if(!row)return;
  var m=row.parentElement;
  if(ev.target.closest('.arrow')&&m.classList.contains('haskids')){m.classList.toggle('open');restripe();saveSoon();return;}
  selectMap(m);
});

// the filter: a thing stays when each word is in its text; a map when each word is in its name or in the name of one of its events
function words(){return filter.value.trim().toLowerCase().split(/\s+/).filter(Boolean);}
function allIn(list,text){return list.every(function(w){return text.indexOf(w)>=0;});}
var timer=0;
function applyFilter(){
  var w=words(),none=!w.length;
  document.querySelectorAll('#tab-ce .row, #tab-troops .row').forEach(function(r){r.hidden=!none&&!allIn(w,r.getAttribute('data-t'));});
  // the maps: the deepest first, because a map that holds a map that stays, stays too (shown pale)
  for(var i=mapList.length-1;i>=0;i--){
    var m=mapList[i],ok=none||allIn(w,m.getAttribute('data-t'))||allIn(w,m.getAttribute('data-t')+' | '+m.getAttribute('data-e'));
    var kidsStay=!!m.querySelector(':scope > .kids > .map:not([hidden])');
    m.hidden=!(ok||kidsStay);
    m.classList.toggle('context',!none&&!ok&&kidsStay);
    if(!none&&kidsStay)m.classList.add('open');
  }
  // the events of the chosen map
  if(selected){
    var own=selected.getAttribute('data-t');
    eventsPane.querySelectorAll('.map-events:not([hidden]) .event').forEach(function(r){r.hidden=!none&&!allIn(w,own+' '+r.getAttribute('data-t'));});
  }
  restripe();
}
filter.addEventListener('input',function(){clearTimeout(timer);timer=setTimeout(function(){applyFilter();saveState();},150);});

// the place: from the last visit if there is one, else from the address
var parts=location.hash.replace('#','').split('/');
var tab=tabs.indexOf(saved.tab)>=0?saved.tab:tabs.indexOf(parts[0])>=0?parts[0]:'ce';
var mapId=saved.tab?saved.map:parts[1];
if(saved.open)mapList.forEach(function(m){m.classList.toggle('open',saved.open.indexOf(m.getAttribute('data-id'))>=0);});
if(saved.filter)filter.value=saved.filter;
showTab(tab);
if(tab==='maps'&&mapId&&byId[mapId])selectMap(byId[mapId]);
applyFilter();
requestAnimationFrame(function(){
  treePane.scrollTop=saved.treeScroll||0;
  eventsPane.scrollTop=saved.eventsScroll||0;
  if(tab!=='maps')window.scrollTo(0,scrollPos[tab]||0);
  else if(!saved.treeScroll&&selected)selected.querySelector('.row').scrollIntoView({block:'center'});
});
})();
