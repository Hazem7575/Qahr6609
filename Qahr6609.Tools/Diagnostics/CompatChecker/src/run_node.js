const fs=require('fs'),path=require('path');
const E=require('./engine.js');
function src(root){return{
 async read(p){try{return new Uint8Array(fs.readFileSync(path.join(root,p)))}catch(e){return null}},
 async walk(dir,exts){const out=[];const base=path.join(root,dir);function w(d){let es;try{es=fs.readdirSync(d,{withFileTypes:true})}catch(e){return}for(const e of es){const f=path.join(d,e.name);if(e.isDirectory())w(f);else if(exts.some(x=>e.name.toLowerCase().endsWith(x)))out.push(path.relative(base,f).split(path.sep).join('/'))}}w(base);return out}}}
(async()=>{
 const r=await E.analyze({client:src(process.argv[2]),server:src(process.argv[3])},(p,m)=>{});
 fs.writeFileSync(process.argv[4],JSON.stringify(r));
 console.log(JSON.stringify(r.meta.roots),r.meta.ms+'ms',JSON.stringify(r.totals));
 for(const c of r.categories){console.log('\n##',c.title,JSON.stringify(c.counts));for(const f of c.findings.filter(f=>f.sev!=='ok').slice(0,14))console.log(' ',f.sev,'|',f.title,'|',f.client,'|',f.server)}
})().catch(e=>{console.error(e);process.exit(1)});
