// CompatChecker.exe - scans the Conquer client and server folders and opens the report in the browser.
'use strict';
const fs = require('fs'), path = require('path'), cp = require('child_process'), os = require('os');
const iconv = require('iconv-lite');
global.GBK_DECODE = b => iconv.decode(Buffer.from(b.buffer, b.byteOffset, b.byteLength), 'gbk');
const Engine = require('./engine.js');
const UI = fs.readFileSync(path.join(__dirname, 'ui.html'), 'utf8');
const ENGINE_SRC = fs.readFileSync(path.join(__dirname, 'engine.js'), 'utf8');

const exeDir = process.pkg ? path.dirname(process.execPath) : __dirname;
const iniPath = path.join(exeDir, 'CompatChecker.ini');
const DEF = { client: 'D:\\Conquer Online 3.0', server: 'D:\\conquer\\MeGa6609' };

function readIni() {
  const cfg = Object.assign({}, DEF);
  try {
    for (const l of fs.readFileSync(iniPath, 'utf8').split(/\r?\n/)) {
      const m = /^\s*(client|server)\s*=\s*(.+?)\s*$/i.exec(l); if (m) cfg[m[1].toLowerCase()] = m[2];
    }
  } catch (e) {
    fs.writeFileSync(iniPath, '; Folders the checker compares. Change them if your folders move.\r\nclient=' + DEF.client + '\r\nserver=' + DEF.server + '\r\n');
  }
  return cfg;
}
// command line: CompatChecker.exe [client] [server]
const cfg = readIni();
if (process.argv[2]) cfg.client = process.argv[2];
if (process.argv[3]) cfg.server = process.argv[3];

function src(root) {
  return {
    async read(p) { try { return new Uint8Array(fs.readFileSync(path.join(root, ...p.split('/')))); } catch (e) { return null; } },
    async walk(dir, exts) {
      const out = []; const base = path.join(root, ...dir.split('/').filter(Boolean));
      const SKIP = new Set(['bin', 'obj', 'packages', '.vs', '.git', 'node_modules']);
      (function w(d, rel) {
        let es; try { es = fs.readdirSync(d, { withFileTypes: true }); } catch (e) { return; }
        for (const e of es) {
          if (e.isDirectory()) { if (!SKIP.has(e.name.toLowerCase())) w(path.join(d, e.name), rel + e.name + '/'); }
          else if (exts.some(x => e.name.toLowerCase().endsWith(x))) out.push(rel + e.name);
        }
      })(base, '');
      return out;
    }
  };
}
function bar(p, msg) {
  const n = Math.round(p * 30);
  process.stdout.write('\r  [' + '#'.repeat(n) + '.'.repeat(30 - n) + '] ' + String(Math.round(p * 100)).padStart(3) + '%   ');
}
function pause(code) {
  process.stdout.write('\r\nPress Enter to close...');
  try { process.stdin.setRawMode && process.stdin.setRawMode(false); } catch (e) {}
  process.stdin.resume(); process.stdin.once('data', () => process.exit(code));
}
const ERR = {
  CLIENT_NOT_FOUND: 'Client folder has no ini\\npc.ini. Edit "client=" in CompatChecker.ini.',
  SERVER_NOT_FOUND: 'Server folder has no Game\\MsgNpc\\NpcID.cs (Source). Edit "server=" in CompatChecker.ini.',
  DB_NOT_FOUND: 'Found the source but no ServerDataBase under bin\\Debug. Build the server once.'
};
(async () => {
  console.log('');
  console.log('  Conquer compatibility checker');
  console.log('  client: ' + cfg.client);
  console.log('  server: ' + cfg.server);
  console.log('');
  for (const k of ['client', 'server']) if (!fs.existsSync(cfg[k])) { console.log('  ! ' + k + ' folder not found: ' + cfg[k]); console.log('    Edit ' + iniPath); return pause(1); }
  try {
    const rep = await Engine.analyze({ client: src(cfg.client), server: src(cfg.server) }, bar);
    rep.meta.clientName = path.basename(cfg.client); rep.meta.serverName = path.basename(cfg.server);
    const html = UI.replace('/*ENGINE*/', () => ENGINE_SRC.replace(/<\/script/gi, '<\\/script'))
      .replace('/*PRELOAD*/', () => JSON.stringify(rep).replace(/</g, '\\u003c'))
      .replace('<html lang="ar" dir="rtl">', '<html lang="ar" dir="rtl" data-fresh="1">');
    let out = path.join(exeDir, 'CompatReport.html');
    try { fs.writeFileSync(out, html); } catch (e) { out = path.join(os.tmpdir(), 'CompatReport.html'); fs.writeFileSync(out, html); }
    const t = rep.totals;
    console.log('\r\n');
    console.log('  errors: ' + t.error + '   warnings: ' + t.warn + '   info: ' + t.info + '   ok: ' + t.ok + '   (' + (rep.meta.ms / 1000).toFixed(1) + 's)');
    console.log('  report: ' + out);
    if (process.platform === 'win32') cp.exec('start "" "' + out + '"');
    setTimeout(() => process.exit(0), 1500);
  } catch (e) {
    console.log('\r\n  ! ' + (ERR[e.message] || e.stack || e.message));
    pause(1);
  }
})();
