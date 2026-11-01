// the theme follows the system (the @media rules of the styles that ask for prefers-color-scheme); the button makes those rules always or never true
(function(){
var rules=[];
for(var s=0;s<document.styleSheets.length;s++){
  try{
    var list=document.styleSheets[s].cssRules;
    for(var r=0;r<list.length;r++)
      if(list[r].type===CSSRule.MEDIA_RULE&&/prefers-color-scheme/.test(list[r].media.mediaText))rules.push({rule:list[r],auto:list[r].media.mediaText});
  }catch(e){}
}
var modes=['auto','light','dark'],button=document.getElementById('theme'),theme='auto';
function apply(mode){
  rules.forEach(function(x){x.rule.media.mediaText=mode==='dark'?'all':mode==='light'?'not all':x.auto;});
  button.textContent='Theme: '+mode.charAt(0).toUpperCase()+mode.slice(1);
}
try{theme=localStorage.getItem('rpgmdump-theme')||'auto';}catch(e){}
if(modes.indexOf(theme)<0)theme='auto';
apply(theme);
button.onclick=function(){
  theme=modes[(modes.indexOf(theme)+1)%modes.length];
  try{localStorage.setItem('rpgmdump-theme',theme);}catch(e){}
  apply(theme);
};
})();
