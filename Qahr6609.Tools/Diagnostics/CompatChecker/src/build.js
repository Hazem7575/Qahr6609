const fs=require('fs');
const ui=fs.readFileSync('ui.html','utf8'), eng=fs.readFileSync('engine.js','utf8');
const rep=process.argv[2]?fs.readFileSync(process.argv[2],'utf8'):'null';
let rj=JSON.parse(rep); if(rj){rj.meta.clientName='Conquer Online 3.0';rj.meta.serverName='MeGa6609';}
const out=ui.replace('/*ENGINE*/',()=>eng.replace(/<\/script/gi,'<\\/script')).replace('/*PRELOAD*/',()=>JSON.stringify(rj).replace(/</g,'\\u003c'));
fs.writeFileSync(process.argv[3]||'CompatChecker.html',out); console.log('ok',out.length);
