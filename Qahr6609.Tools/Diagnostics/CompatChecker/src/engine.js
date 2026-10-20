/* Conquer client <-> server compatibility checker — analysis engine.
 * Pure JS, runs in the browser (File System Access API) and in Node (for testing).
 * fs = { client: Src, server: Src }, Src = { read(path) -> Promise<Uint8Array|null>, walk(dir, exts) -> Promise<string[]> }
 * Paths always use "/" and are relative to the folder the user picked.
 */
(function (root) {
  'use strict';

  // ───────────────────────── text helpers ─────────────────────────
  const utf8 = new TextDecoder('utf-8', { fatal: true });
  let gbk;
  try { gbk = new TextDecoder('gbk'); } catch (e) {
    // runtimes built without full ICU: use an injected decoder (iconv-lite in the exe) or latin1
    gbk = { decode: b => (globalThis.GBK_DECODE ? globalThis.GBK_DECODE(b) : new TextDecoder('latin1').decode(b)) };
  }
  function decode(bytes) {
    if (!bytes) return null;
    if (bytes[0] === 0xEF && bytes[1] === 0xBB && bytes[2] === 0xBF) bytes = bytes.subarray(3);
    try { return utf8.decode(bytes); } catch (e) { return gbk.decode(bytes); }
  }

  // TQ ".dat" cipher (srand(0x2537) MSVC rand keystream, XOR + rotate)
  const DAT_KEY = (() => {
    let s = 0x2537; const k = new Uint8Array(128);
    for (let i = 0; i < 128; i++) { s = (Math.imul(s, 214013) + 2531011) >>> 0; k[i] = ((s >>> 16) & 0x7fff) % 256; }
    return k;
  })();
  function datDecrypt(b) {
    const out = new Uint8Array(b.length);
    for (let i = 0; i < b.length; i++) {
      const n = b[i] ^ DAT_KEY[i & 127], r = i & 7;
      out[i] = ((n << (8 - r)) | (n >> r)) & 0xff;
    }
    return out;
  }
  function looksText(b) {
    let p = 0; const n = Math.min(b.length, 1500);
    for (let i = 0; i < n; i++) { const c = b[i]; if ((c >= 32 && c < 127) || c === 9 || c === 10 || c === 13) p++; }
    return n === 0 || p / n > 0.85;
  }
  // returns text of a client data file, decrypting if needed
  function readDataText(bytes) {
    if (!bytes) return null;
    if (looksText(bytes)) return decode(bytes);
    const d = datDecrypt(bytes);
    if (looksText(d)) return decode(d);
    return null; // binary
  }

  function lineOf(text, idx) { let n = 1; for (let i = 0; i < idx && i < text.length; i++) if (text.charCodeAt(i) === 10) n++; return n; }
  function lineIndex(text) { const a = [0]; for (let i = 0; i < text.length; i++) if (text.charCodeAt(i) === 10) a.push(i + 1); return a; }
  function lineAt(idx, li) { let lo = 0, hi = li.length - 1; while (lo < hi) { const m = (lo + hi + 1) >> 1; if (li[m] <= idx) lo = m; else hi = m - 1; } return lo + 1; }

  // ───────────────────────── INI ─────────────────────────
  function parseIni(text) {
    const sections = new Map(); let cur = null; const order = [];
    const lines = text.split(/\r?\n/);
    for (let i = 0; i < lines.length; i++) {
      const raw = lines[i]; const l = raw.trim();
      if (!l || l.startsWith('//') || l.startsWith(';')) continue;
      const m = /^\[(.+?)\]/.exec(l);
      if (m) {
        cur = { name: m[1], kv: {}, all: [], line: i + 1 };
        if (!sections.has(m[1])) { sections.set(m[1], cur); order.push(m[1]); }
        else cur = sections.get(m[1]); // merge duplicates like the Windows API does (first wins per key)
        continue;
      }
      if (!cur) continue;
      const eq = l.indexOf('=');
      if (eq < 0) continue;
      const k = l.slice(0, eq).trim(), v = l.slice(eq + 1).trim();
      cur.all.push([k, v, i + 1]);
      if (!(k in cur.kv)) cur.kv[k] = v;
    }
    return { sections, order };
  }

  // rows of a TQ table (@@, @, comma or whitespace separated)
  function parseRows(text) {
    const rows = []; const lines = text.split(/\r?\n/);
    for (let i = 0; i < lines.length; i++) {
      const l = lines[i].trim();
      if (!l || l.startsWith('//') || l.startsWith(';') || l.startsWith('[')) continue;
      let cols;
      if (l.includes('@@')) cols = l.split('@@');
      else if (l.includes('@')) cols = l.split('@');
      else if (l.includes(',') && !/\s/.test(l)) cols = l.split(',');
      else cols = l.split(/\s+/);
      while (cols.length && cols[cols.length - 1].trim() === '') cols.pop();
      if (!cols.length) continue;
      rows.push({ cols: cols.map(c => c.trim()), line: i + 1 });
    }
    return rows;
  }
  const normVal = v => (/^-?\d+$/.test(v) ? String(parseInt(v, 10)) : v);

  // ───────────────────────── C# helpers ─────────────────────────
  function stripComments(src) {
    // keep line structure; drop // and /* */ comments but not inside strings
    let out = ''; let i = 0; const n = src.length;
    while (i < n) {
      const c = src[i], d = src[i + 1];
      if (c === '"' ) { let j = i + 1; if (src[i - 1] === '@') { while (j < n && !(src[j] === '"' && src[j + 1] !== '"')) { if (src[j] === '"') j++; j++; } } else { while (j < n && src[j] !== '"' && src[j] !== '\n') { if (src[j] === '\\') j++; j++; } } out += src.slice(i, j + 1); i = j + 1; continue; }
      if (c === "'") { let j = i + 1; while (j < n && src[j] !== "'" && src[j] !== '\n') { if (src[j] === '\\') j++; j++; } out += src.slice(i, j + 1); i = j + 1; continue; }
      if (c === '/' && d === '/') { while (i < n && src[i] !== '\n') { out += ' '; i++; } continue; }
      if (c === '/' && d === '*') { while (i < n && !(src[i] === '*' && src[i + 1] === '/')) { out += src[i] === '\n' ? '\n' : ' '; i++; } out += '  '; i += 2; continue; }
      out += c; i++;
    }
    return out;
  }
  function blockFrom(src, idx) { // returns [start,end) of the {...} block starting at first { after idx
    const s = src.indexOf('{', idx); if (s < 0) return null;
    let depth = 0;
    for (let i = s; i < src.length; i++) { if (src[i] === '{') depth++; else if (src[i] === '}') { depth--; if (!depth) return [s, i + 1]; } }
    return [s, src.length];
  }
  // #if X ... #endif blocks: we keep everything (cannot know defines) but remember them
  function activeDefines(csproj) {
    const m = /<DefineConstants>([^<]*)<\/DefineConstants>/.exec(csproj || '');
    return m ? m[1].split(/[;,]/).map(s => s.trim()).filter(Boolean) : [];
  }

  // ───────────────────────── findings ─────────────────────────
  const SEV = { error: 3, warn: 2, info: 1, ok: 0 };
  function F(sev, title, o) { return Object.assign({ sev, title }, o || {}); }

  // ───────────────────────── loaders ─────────────────────────
  async function resolveRoots(fs, log) {
    // client: folder containing ini/npc.ini (or the ini folder itself)
    let c = null;
    for (const p of ['', 'Patch/']) if (await fs.client.read(p + 'ini/npc.ini')) { c = p + 'ini/'; break; }
    if (!c && await fs.client.read('npc.ini')) c = '';
    // server: folder containing Game/MsgNpc/NpcID.cs (repo root or Source)
    let s = null;
    for (const p of ['', 'Source/']) if (await fs.server.read(p + 'Game/MsgNpc/NpcID.cs')) { s = p; break; }
    let db = null;
    if (s !== null) for (const p of ['bin/Debug/ServerDataBase/', 'bin/Release/ServerDataBase/', 'ServerDataBase/', 'bin/x64/Debug/ServerDataBase/', 'bin/x86/Debug/ServerDataBase/'])
      if (await fs.server.read(s + p + 'Npcs.txt') || await fs.server.read(s + p + 'npcs.txt')) { db = s + p; break; }
    return { ini: c, src: s, db };
  }

  async function analyze(fs, onProgress) {
    const progress = (p, msg) => onProgress && onProgress(p, msg);
    const t0 = Date.now();
    const roots = await resolveRoots(fs);
    const meta = { roots, warnings: [], files: { client: [], server: [] } };
    if (roots.ini === null) throw new Error('CLIENT_NOT_FOUND');
    if (roots.src === null) throw new Error('SERVER_NOT_FOUND');
    if (roots.db === null) throw new Error('DB_NOT_FOUND');

    const cache = new Map();
    async function cRead(name) { const k = 'c:' + name; if (!cache.has(k)) { const b = await fs.client.read(roots.ini + name); cache.set(k, b); if (b) meta.files.client.push(name); } return cache.get(k); }
    async function sRead(name) { const k = 's:' + name; if (!cache.has(k)) { const b = await fs.server.read(roots.db + name); cache.set(k, b); if (b) meta.files.server.push('ServerDataBase/' + name); } return cache.get(k); }
    const cText = async n => readDataText(await cRead(n));
    const sText = async n => readDataText(await sRead(n));
    const cPath = n => 'Client/ini/' + n, sPath = n => 'ServerDataBase/' + n;

    // ── source code index
    progress(0.03, 'بقرا كود السيرفر…');
    const csFiles = (await fs.server.walk(roots.src, ['.cs'])).filter(p => !/(^|\/)(bin|obj|packages|\.vs)\//.test(p));
    const src = new Map(); // rel -> {raw, code, li}
    let done = 0;
    for (const rel of csFiles) {
      const b = await fs.server.read(roots.src + rel);
      if (b) { const raw = decode(b); src.set(rel, { raw, code: stripComments(raw), li: null }); }
      if (++done % 40 === 0) progress(0.03 + 0.22 * done / csFiles.length, 'بقرا كود السيرفر… ' + done + '/' + csFiles.length);
    }
    const L = (f, idx) => { const e = src.get(f); if (!e.li) e.li = lineIndex(e.raw); return lineAt(idx, e.li); };
    const ref = (file, line) => ({ side: 'server', file: 'Source/' + file, line });
    const cref = (file, line) => ({ side: 'client', file: 'ini/' + file, line });
    const sref = (file, line) => ({ side: 'server', file: 'ServerDataBase/' + file, line });
    const allCode = () => src.entries();
    function findInCode(re) { const out = []; for (const [f, e] of allCode()) { re.lastIndex = 0; let m; while ((m = re.exec(e.code))) { out.push({ f, m, idx: m.index }); if (!re.global) break; } } return out; }
    meta.csCount = src.size;

    // ── maps
    progress(0.27, 'بقارن الخرايط…');
    function parseGameMap(b) {
      const maps = new Map(); if (!b) return maps;
      const dv = new DataView(b.buffer, b.byteOffset, b.byteLength); let o = 0;
      const n = dv.getInt32(o, true); o += 4;
      for (let i = 0; i < n && o + 8 <= b.length; i++) {
        const id = dv.getInt32(o, true); o += 4; const len = dv.getInt32(o, true); o += 4;
        const path = new TextDecoder('ascii').decode(b.subarray(o, o + len)); o += len; const puzzle = dv.getInt32(o, true); o += 4;
        maps.set(id, { id, path, puzzle });
      }
      return maps;
    }
    const cMaps = parseGameMap(await cRead('GameMap.dat'));
    const sMaps = parseGameMap(await sRead('ini/GameMap.dat'));
    // map aliases created in GameMap.LoadMaps (LoadMap(3830, mapFile, 1780))
    const mapAlias = new Map();
    for (const { m } of findInCode(/LoadMap\(\s*(\d+)\s*,\s*\w+\s*,\s*(\d+)\s*\)/g)) mapAlias.set(+m[1], +m[2]);
    const serverHasMap = id => sMaps.has(id) || mapAlias.has(id);
    const clientHasMap = id => cMaps.has(id) || (mapAlias.has(id) && cMaps.has(mapAlias.get(id)));
    const gmEx = parseIni((await cText('GameMapEx.ini')) || '');
    const sGmEx = parseIni((await sText('GameMapEx.ini')) || '');
    const mapName = id => { const s = gmEx.sections.get(String(id)) || sGmEx.sections.get(String(id)); return s && s.kv.Name ? s.kv.Name : (cMaps.get(id) || sMaps.get(id) || {}).path ? ((cMaps.get(id) || sMaps.get(id)).path.split('/').pop().replace(/\.\w+$/, '')) : ''; };
    const mapLabel = id => { const n = mapName(id); return n ? `${id} (${n})` : String(id); };

    // ── NPCs (server)
    progress(0.33, 'بقرا الـ NPCs…');
    const sNpcs = new Map(); const dupNpc = [];
    function addNpc(n) { if (sNpcs.has(n.uid)) dupNpc.push([sNpcs.get(n.uid), n]); else sNpcs.set(n.uid, n); }
    const npcsTxt = decode(await sRead('Npcs.txt') || await sRead('npcs.txt')) || '';
    let npcsBlank = 0; { const ls = npcsTxt.split(/\r?\n/); let last = ls.length - 1; while (last >= 0 && !ls[last].trim()) last--; for (let i = 0; i < last; i++) if (!ls[i]) { npcsBlank = i + 1; break; } }
    meta.npcsBlank = npcsBlank;
    npcsTxt.split(/\r?\n/).forEach((l, i) => { const p = l.split(','); if (p.length < 6 || !/^\d+$/.test(p[0].trim())) return; addNpc({ uid: +p[0], type: +p[1], mesh: +p[2], map: +p[3], x: +p[4], y: +p[5], file: 'Npcs.txt', line: i + 1, name: '' }); });
    const sobTxt = decode(await sRead('SobNpcs.txt')) || '';
    sobTxt.split(/\r?\n/).forEach((l, i) => { const p = l.split(','); if (p.length < 7 || !/^\d+$/.test(p[0].trim())) return; addNpc({ uid: +p[0], name: p[1], type: +p[2], mesh: +p[3], map: +p[4], x: +p[5], y: +p[6], file: 'SobNpcs.txt', line: i + 1, sob: true }); });

    // NpcID enum
    const npcEnum = new Map(); const npcEnumName = new Map();
    for (const [f, e] of allCode()) {
      const m = /enum\s+NpcID\b[^{]*/.exec(e.code); if (!m) continue;
      const blk = blockFrom(e.code, m.index); const body = e.code.slice(blk[0] + 1, blk[1] - 1);
      let last = -1;
      for (const part of body.split(',')) {
        const mm = /^\s*([A-Za-z_]\w*)\s*(?:=\s*([^,]+))?\s*$/.exec(part); if (!mm) continue;
        let v; const ex = (mm[2] || '').trim();
        if (!ex) v = last + 1;
        else if (/^0x[0-9a-f]+$/i.test(ex)) v = parseInt(ex, 16);
        else if (/^\d+$/.test(ex)) v = +ex;
        else if (npcEnum.has(ex)) v = npcEnum.get(ex);
        else continue;
        npcEnum.set(mm[1], v); if (!npcEnumName.has(v)) npcEnumName.set(v, mm[1]); last = v;
      }
    }
    // scripts: [NpcAttribute(NpcID.X)] / [NpcAttribute((NpcID)123)]
    const scripted = new Map(); // uid -> {file,line,name}
    for (const { f, m, idx } of findInCode(/NpcAttribute\(\s*(?:\(\s*[\w.]*NpcID\s*\)\s*(\d+)|[\w.]*NpcID\.(\w+))\s*\)/g)) {
      const uid = m[1] ? +m[1] : npcEnum.get(m[2]);
      if (uid === undefined) continue;
      if (!scripted.has(uid)) scripted.set(uid, { file: f, line: L(f, idx), name: m[2] || '' });
    }
    // hard-coded dispatch in Procesor.cs (ranges / ==)
    for (const { f, m } of findInCode(/npcid\s*>=\s*(\d+)\s*&&\s*(?:\(\s*\w+\s*\)\s*)?[\w.]*npcid\s*<=\s*(\d+)/g)) for (let u = +m[1]; u <= +m[2]; u++) if (!scripted.has(u)) scripted.set(u, { file: f, line: 0, name: 'range' });
    for (const { f, m, idx } of findInCode(/npcid\s*==\s*(\d{2,})\b/g)) if (!scripted.has(+m[1])) scripted.set(+m[1], { file: f, line: L(f, idx), name: '' });
    meta.scriptCount = scripted.size;

    // client NPC looks
    const lookTypes = new Set();
    for (const f of ['npc.ini', 'npcex.ini', 'terrainnpc.ini']) { const t = await cText(f); if (t) for (const m of t.matchAll(/^\s*\[NpcType(\d+)\]/gm)) lookTypes.add(+m[1]); }
    const npcxText = await cText('NpcX.ini'); const npcX = new Set(); if (npcxText) for (const m of npcxText.matchAll(/^\s*\[(\d+)\]/gm)) npcX.add(+m[1]);
    const npcIni = parseIni((await cText('npc.ini')) || '');
    const npcLookName = mesh => { const s = npcIni.sections.get('NpcType' + Math.floor(mesh / 10)); return s ? s.kv.Name || '' : ''; };
    const lookKnown = mesh => lookTypes.has(Math.floor(mesh / 10)) || npcX.has(Math.floor(mesh / 10)) || npcX.has(mesh) || lookTypes.has(mesh);

    // shops
    const cShop = parseIni((await cText('Shop.dat')) || '');
    const sShop = parseIni((await sText('shops/Shop.dat')) || '');
    function shopMap(ini) { const m = new Map(); for (const k of ini.order) { const s = ini.sections.get(k); if (!/^Shop\d+$/.test(k) || !s.kv.ID) continue; const items = []; for (const [kk, v] of s.all) if (/^Item\d+$/.test(kk)) items.push(+v); m.set(+s.kv.ID, { id: +s.kv.ID, name: s.kv.Name || '', type: s.kv.Type, money: s.kv.MoneyType, items, line: s.line }); } return m; }
    const cShops = shopMap(cShop), sShops = shopMap(sShop);

    const npcLabel = uid => { const n = sNpcs.get(uid); const nm = (n && n.name) || npcEnumName.get(uid) || (n ? npcLookName(n.mesh) : '') || ''; return nm ? `${uid} (${nm})` : String(uid); };

    // ───────────── category builders ─────────────
    const cats = [];
    const cat = (id, title, blurb, icon) => { const c = { id, title, blurb, icon, findings: [] }; cats.push(c); return c; };

    // ── 1. Event "Go" buttons
    progress(0.42, 'بفحص أزرار الأحداث…');
    {
      const C = cat('go', 'أزرار Go بتاعة الأحداث', 'لما اللاعب يدوس Go في شباك الأحداث، الكلاينت بيبعت رقم الحدث للسيرفر (Bulletin). لو السيرفر مش عارف الرقم، الزرار مايعملش أي حاجة.', 'go');
      const act = parseIni((await cText('Activity.ini')) || '');
      const cEvents = new Map();
      for (const k of act.order) { const s = act.sections.get(k); if (!/^t\d+$/.test(k) || !s.kv.id) continue; const id = +s.kv.id; if (!cEvents.has(id)) cEvents.set(id, { id, name: s.kv.Name, type: +s.kv.type, date: s.kv.Date, enter: s.kv.Enter1, line: s.line }); }
      // server Bulletin handler (+ one level of delegation, e.g. ActivityBulletin.Go(client, msg, data->dwParam))
      const hits = findInCode(/DataAttribute\(\s*ActionType\.Bulletin\s*\)\s*\]/g);
      const sCases = new Map(); const sNotAvail = new Map(); let bulletinRef = null;
      const scanBody = (f, start, end) => {
        const e = src.get(f); const body = e.code.slice(start, end);
        for (const m of body.matchAll(/case\s+(\d+)\s*:/g)) {
          const at = start + m.index; const next = body.slice(m.index, m.index + 600);
          const tp = /Teleport\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)/.exec(next);
          const rawLine = e.raw.split('\n')[L(f, at) - 1] || ''; const cm = /\/\/\s*(.+)$/.exec(rawLine);
          if (!sCases.has(+m[1])) sCases.set(+m[1], { id: +m[1], file: f, line: L(f, at), tp: tp ? { x: +tp[1], y: +tp[2], map: +tp[3] } : null, note: cm ? cm[1].trim() : '' });
        }
        // table style: Add("name", <npc>, map, x, y, id1, id2, ...)
        for (const m of body.matchAll(/\bAdd\(\s*"([^"]*)"\s*,\s*([^,]+?)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*((?:,\s*\d+\s*)+)\)/g)) {
          const npcExpr = m[2].trim(); let npc = 0; const nm = /NpcID\.(\w+)/.exec(npcExpr); if (nm) npc = npcEnum.get(nm[1]) || 0; else if (/^\d+$/.test(npcExpr)) npc = +npcExpr;
          for (const id of m[6].split(',').map(x => x.trim()).filter(Boolean)) if (!sCases.has(+id)) sCases.set(+id, { id: +id, file: f, line: L(f, start + m.index), tp: { x: +m[4], y: +m[5], map: +m[3] }, npc, npcName: nm ? nm[1] : '', note: m[1] });
        }
        for (const m of body.matchAll(/NotAvailable\[\s*(\d+)\s*\]\s*=\s*"([^"]*)"/g)) sNotAvail.set(+m[1], { name: m[2], file: f, line: L(f, start + m.index) });
      };
      for (const { f, idx } of hits) {
        const e = src.get(f); const blk = blockFrom(e.code, idx); bulletinRef = ref(f, L(f, idx));
        scanBody(f, blk[0], blk[1]);
        const body = e.code.slice(blk[0], blk[1]);
        for (const m of body.matchAll(/(\w+)\.(\w+)\s*\([^;]*dwParam/g)) {
          for (const [f2, e2] of allCode()) { const cm2 = new RegExp('\\bclass\\s+' + m[1] + '\\b').exec(e2.code); if (!cm2) continue; const cb = blockFrom(e2.code, cm2.index); scanBody(f2, cb[0], cb[1]); }
        }
      }
      meta.bulletin = { client: cEvents.size, server: sCases.size, notAvailable: sNotAvail.size };
      if (!hits.length) C.findings.push(F('error', 'مش لاقي الكود اللي بيستقبل زرار Go في السيرفر', { why: 'مفيش دالة عليها [DataAttribute(ActionType.Bulletin)] في السورس، فكل أزرار Go مش هتشتغل.', fix: 'ضيف handler للـ ActionType.Bulletin في MsgDataPacket.cs.' }));
      const byName = new Map(); for (const ev of [...cEvents.values()].sort((a, b) => a.id - b.id)) { if (!byName.has(ev.name)) byName.set(ev.name, []); byName.get(ev.name).push(ev); }
      for (const [name, evs] of byName) {
        const na = evs.filter(ev => !sCases.has(ev.id) && sNotAvail.has(ev.id));
        if (na.length) C.findings.push(F(na.every(ev => ev.type === 2) ? 'ok' : 'info', `زرار Go بتاع «${name}» بيقول للاعب إن الحدث مش متاح`, { client: `الحدث رقم ${na.map(e => e.id).join('، ')}`, server: `NotAvailable[${na[0].id}] = "${sNotAvail.get(na[0].id).name}"`, key: 'gona:' + name, why: 'مقصود: السيرفر بيرد برسالة إن الحدث مش شغال بدل ما الزرار يسكت.', refs: [cref('Activity.ini', na[0].line), ref(sNotAvail.get(na[0].id).file, sNotAvail.get(na[0].id).line)] }));
        const miss = evs.filter(ev => !sCases.has(ev.id) && !sNotAvail.has(ev.id)), have = evs.filter(ev => sCases.has(ev.id));
        const ids = list => list.map(e => e.id).join('، ');
        if (miss.length) {
          const seasonal = miss.every(ev => ev.type === 2);
          C.findings.push(F(seasonal ? 'warn' : 'error', `زرار Go بتاع «${name}» مش شغال`, {
            client: miss.length > 1 ? `${miss.length} أرقام: ${ids(miss)} (لكل مستوى رقم)` : `الحدث رقم ${miss[0].id}`, server: `مفيش case ${ids(miss)} في Bulletin`, key: 'go:' + name,
            why: seasonal ? 'ده حدث موسمي بتاريخ قديم في الكلاينت، فغالباً مش ظاهر للاعبين. لو فعّلته لازم تضيفله case.' : 'اللاعب هيدوس Go ومش هيحصل أي حاجة، لأن السيرفر بيروح للـ default في الـ switch.',
            fix: miss.map(ev => `case ${ev.id}:`).join('\n') + ` // ${name}\n    client.Teleport(X, Y, MAP);\n    client.Player.SendString(msg, MsgStringPacket.StringID.Effect, true, "movego");\n    break;`,
            refs: [...miss.slice(0, 3).map(ev => cref('Activity.ini', ev.line)), bulletinRef].filter(Boolean)
          }));
        }
        for (const ev of have) {
          const sc = sCases.get(ev.id);
          const npcAt = sc.npc ? sNpcs.get(sc.npc) : null;
          const npcHere = npcAt && (npcAt.map === sc.tp.map);
          const fixedOk = sc.tp && sc.tp.x > 0 && serverHasMap(sc.tp.map) && !/jail/i.test(mapName(sc.tp.map));
          const bad = sc.tp && sc.tp.x > 0 && !clientHasMap(sc.tp.map);
          const dead = sc.npc && !npcHere && !fixedOk;
          const jailGuard = /Map\s*==\s*5000/.test((src.get(sc.file) || {}).code || '');
          const jail = npcHere && /jail/i.test(mapName(npcAt.map)) && !jailGuard;
          if (npcHere && !jail && /jail/i.test(mapName(npcAt.map))) { C.findings.push(F('info', `زرار Go بتاع «${name}» بيقول للاعب إن الحدث مش متاح`, { client: `الحدث رقم ${ev.id}`, server: `NPC ${sc.npcName || sc.npc} متشال على ${mapLabel(npcAt.map)}، والكود بيرفض خرايط السجن`, key: 'go:' + ev.id, refs: [cref('Activity.ini', ev.line), ref(sc.file, sc.line)] })); continue; }
          const sev = bad || jail ? 'error' : dead ? 'warn' : 'ok';
          C.findings.push(F(sev, jail ? `زرار Go بتاع «${name}» بيودّي اللاعب لخريطة السجن` : bad ? `زرار Go بتاع «${name}» بيودّي لخريطة مش موجودة في الكلاينت` : dead ? `زرار Go بتاع «${name}» هيقول «مش متاح»: الـ NPC مش موجود` : `زرار Go بتاع «${name}» شغال`, {
            client: `الحدث رقم ${ev.id}`, server: (sc.npc ? `NPC ${sc.npcName || sc.npc}${npcHere ? ` على ${mapLabel(npcAt.map)} (${npcAt.x}, ${npcAt.y})` : ' مش متحط على الخريطة'}` : `case ${ev.id}`) + (sc.tp && sc.tp.x ? ` · المكان الاحتياطي ${mapLabel(sc.tp.map)} (${sc.tp.x}, ${sc.tp.y})` : ''), key: 'go:' + ev.id,
            why: jail ? `الـ NPC متشال على ${mapLabel(npcAt.map)}، والكود بيدوّر عليه على نفس الخريطة فبيلاقيه، فاللاعب هيتنقل جنبه جوه السجن.` : bad ? 'الخريطة دي مش في GameMap.dat بتاع الكلاينت، فاللاعب هيقع أو الشاشة هتفضل سودا.' : dead ? 'الكود بيدوّر على الـ NPC ومش هيلاقيه، ومفيش مكان احتياطي، فاللاعب هياخد رسالة إن الحدث مش متاح.' : undefined,
            fix: jail ? `خلّي الحدث ده في NotAvailable، أو انقل الـ NPC ${sc.npcName || sc.npc} لخريطته الحقيقية وغيّر رقم الخريطة في Add(...).` : undefined,
            refs: [cref('Activity.ini', ev.line), ref(sc.file, sc.line), ...(npcAt ? [sref(npcAt.file, npcAt.line)] : [])]
          }));
        }
      }
      for (const sc of sCases.values()) if (!cEvents.has(sc.id)) C.findings.push(F('info', `السيرفر بيستقبل الحدث ${sc.id}${sc.note ? ' (' + sc.note + ')' : ''} والكلاينت مش بيبعته`, {
        client: 'مفيش حدث رقم ' + sc.id + ' في Activity.ini', server: `${sc.id}` + (sc.tp ? ` → ${mapLabel(sc.tp.map)}` : ''), key: 'go:s' + sc.id,
        why: 'مش مشكلة، بس الكود ده عمره ما هيتنفذ من شباك الأحداث.', refs: [ref(sc.file, sc.line)]
      }));
    }

    // ── 2. Event schedules
    progress(0.5, 'بقارن مواعيد الأحداث…');
    {
      const C = cat('time', 'مواعيد الأحداث', 'شباك الأحداث في الكلاينت بيعرض للاعب يوم وساعة كل حدث. لو السيرفر بيشغّل الحدث في وقت تاني، اللاعب هيجي في الميعاد المكتوب ومش هيلاقي حاجة.', 'clock');
      const DAYS = ['', 'الاتنين', 'التلات', 'الأربع', 'الخميس', 'الجمعة', 'السبت', 'الحد'];
      const DOW = { Monday: 1, Tuesday: 2, Wednesday: 3, Thursday: 4, Friday: 5, Saturday: 6, Sunday: 7 };
      // server: parse MsgSchedules (condition stack: case DayOfWeek / if (... Hour/Minute/DayOfWeek ...))
      const starts = []; let schedRef = null; const gatedFlag = {};
      const ALLD = [1, 2, 3, 4, 5, 6, 7];
      function condOf(c) {
        const o = { hrs: [], min: null, days: null, gated: /CustomEvents\s*&&/.test(c), monthly: /\bDay\s*==\s*\d+/.test(c) && !/DayOfWeek\s*==/.test(c) };
        o.hrs = [...c.matchAll(/Hour\s*==\s*(\d+)/g)].map(x => +x[1]);
        const ge = /Hour\s*>=\s*(\d+)/.exec(c); if (!o.hrs.length && ge) o.hrs = [+ge[1]]; const lt = /Hour\s*<\s*(\d+)/.exec(c); o.hrEnd = ge && lt ? +lt[1] : null;
        const mn = /Minute\s*==\s*(\d+)/.exec(c); const ml = /Minute\s*<\s*(\d+)/.exec(c); o.min = mn ? +mn[1] : (ml ? 0 : null);
        const eq = [...c.matchAll(/DayOfWeek\s*==\s*DayOfWeek\.(\w+)/g)].map(x => DOW[x[1]]).filter(Boolean);
        const ne = [...c.matchAll(/DayOfWeek\s*!=\s*DayOfWeek\.(\w+)/g)].map(x => DOW[x[1]]).filter(Boolean);
        if (eq.length) o.days = eq; else if (ne.length) o.days = ALLD.filter(d => !ne.includes(d));
        o.relevant = o.hrs.length || o.min !== null || o.days || o.monthly || o.gated;
        return o;
      }
      for (const [f, e] of allCode()) {
        if (!/class\s+MsgSchedules\b/.test(e.code)) continue;
        const cm = /static\s+bool\s+CustomEvents\s*=\s*(true|false)/.exec(e.code); if (cm) { gatedFlag.value = cm[1] === 'true'; gatedFlag.ref = ref(f, L(f, cm.index)); }
        const lines = e.code.split(/\r?\n/); let depth = 0; const stack = []; let pendingTournament = null;
        for (let i = 0; i < lines.length; i++) {
          let ln = lines[i]; const lineNo = i + 1;
          const dm = /case\s+DayOfWeek\.(\w+)\s*:/.exec(ln);
          if (dm) { while (stack.length && stack[stack.length - 1].isCase && stack[stack.length - 1].depth >= depth) stack.pop(); stack.push({ hrs: [], min: null, days: [DOW[dm[1]]], depth, open: false, isCase: true, relevant: true }); }
          const im = /\b(else\s+)?if\s*\(/.exec(ln);
          if (im) {
            // collect the full (possibly multi-line) condition
            let txt = ln.slice(im.index + im[0].length - 1), j = i, bal = 0, cond = '';
            outer: for (;;) { for (const ch of txt) { cond += ch; if (ch === '(') bal++; else if (ch === ')') { bal--; if (!bal) break outer; } } if (++j >= lines.length) break; txt = lines[j]; cond += ' '; }
            if (im[1]) { while (stack.length && !stack[stack.length - 1].isCase && stack[stack.length - 1].depth === depth && !stack[stack.length - 1].open) stack.pop(); }
            const o = condOf(cond); o.depth = depth; o.open = false; o.line = lineNo;
            const rest = (lines[j] || '').slice((lines[j] || '').indexOf(')') + 1);
            o.inline = /;\s*$/.test(rest.trim()) && j === i && !/\{/.test(rest); // if (...) Statement();
            stack.push(o);
          }
          const tm = /Tournaments\[\s*TournamentType\.(\w+)\s*\]/.exec(ln); if (tm) pendingTournament = tm[1];
          const sm = /(\w+)\.(Start|Open)\s*\(/.exec(ln);
          if (sm) {
            let name = sm[1]; if (name === 'CurrentTournament' && pendingTournament) name = pendingTournament;
            const act = stack.filter(c => c.relevant);
            if (act.some(c => c.hrs.length || c.min !== null)) {
              let days = null, hrs = [], min = null, gated = false, monthly = false, hrEnd = null;
              for (const c of act) { if (c.days) days = days ? days.filter(d => c.days.includes(d)) : c.days.slice(); if (c.hrs.length) { hrs = c.hrs; hrEnd = c.hrEnd || null; } if (c.min !== null) min = c.min; gated = gated || c.gated; monthly = monthly || c.monthly; }
              starts.push({ name, days, hrs, min, hrEnd, gated, monthly, file: f, line: lineNo });
            }
          }
          // braces
          for (const ch of ln) {
            if (ch === '{') { depth++; const top = stack[stack.length - 1]; if (top && !top.open && top.depth === depth - 1) { top.open = true; top.blockDepth = depth; } }
            else if (ch === '}') { depth--; while (stack.length && stack[stack.length - 1].open && depth < stack[stack.length - 1].blockDepth) stack.pop(); }
          }
          // a brace-less if applies to the next statement only
          if (/;\s*$/.test(ln.trim()) && !im) { while (stack.length && !stack[stack.length - 1].open && !stack[stack.length - 1].isCase && stack[stack.length - 1].line < lineNo && stack[stack.length - 1].depth === depth) stack.pop(); }
          if (im && stack.length && stack[stack.length - 1].inline) stack.pop();
          if (/^\s*break\s*;/.test(ln)) { /* end of a case: its block closes with the next } */ }
        }
        schedRef = ref(f, 1);
      }
      meta.schedules = starts.length;
      const MAP = [
        [/horse\s*rac/i, ['SteedRace']], [/^team\s*pk/i, ['TeamPkTournament']], [/^elite\s*pk/i, ['ElitePkTournament']],
        [/skill\s*team/i, ['SkillTeamPkTournament']], [/capture\s*the\s*flag/i, ['CaptureTheFlag']], [/power\s*arena/i, ['PowerArena']],
        [/class\s*pk/i, ['ClassPkWar']], [/dis\s*city/i, ['DisCity']], [/weekly\s*pk|monthly\s*pk/i, ['PkWar']], [/guild\s*pk|flame\s*lit/i, ['GuildWar']],
        [/treasure\s*in\s*(the\s*)?blue/i, ['SquidwardOctopus', 'TreasureInBlue']], [/battlefield/i, ['BattleField']], [/kingdom\s*war/i, ['UnionWar']]
      ];
      const fmt = (h, m) => `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}`;
      const act = parseIni((await cText('Activity.ini')) || '');
      const seen = new Set(); const rows = [];
      for (const k of act.order) {
        const s = act.sections.get(k); if (!/^t\d+$/.test(k) || +s.kv.type !== 1) continue;
        const name = s.kv.Name || ''; const date = s.kv.Date || '';
        const days = date.split('|').map(Number).filter(Boolean);
        const enters = [s.kv.Enter1, s.kv.Enter2].filter(Boolean).map(v => v.split('~')[0]);
        if (enters.every(e => e === '00:01')) continue; // all-day activities
        const key = name + '|' + date + '|' + enters.join(',');
        if (seen.has(key)) continue; seen.add(key);
        const mp = MAP.find(([re]) => re.test(name));
        const clientTxt = `${days.map(d => DAYS[d]).join('، ')} — ${enters.join(' و ')}`;
        if (!mp) { rows.push({ name, s, sev: 'info', title: `«${name}»: مش عارف أربطه بحدث في السيرفر`, client: clientTxt, server: '—', key: 'time:' + key, why: 'الاسم ده مالوش مقابل معروف في MsgSchedules، فاتأكد منه بنفسك.', refs: [cref('Activity.ini', s.line)] }); continue; }
        const sv = starts.filter(x => mp[1].includes(x.name) && !x.monthly);
        if (!sv.length) {
          let inner = '';
          for (const [f, e] of allCode()) { if (!mp[1].some(n => new RegExp('class\\s+Msg' + n.replace(/Tournament$/, '') + '\\b|class\\s+' + n + '\\b').test(e.code))) continue; const hs = [...e.code.matchAll(/Hour\s*==\s*(\d+)/g)].map(x => +x[1]); if (hs.length) { inner = [...new Set(hs)].join('، '); break; } }
          rows.push({ name, s, sev: inner ? 'info' : 'warn', title: inner ? `«${name}»: الميعاد محدد جوه كلاس الحدث نفسه` : `«${name}»: السيرفر مش بيشغّل الحدث ده`, client: clientTxt, server: inner ? 'الساعات اللي في الكود: ' + inner : 'مفيش ' + mp[1].join('/') + '.Start() في MsgSchedules', key: 'time:' + key, why: inner ? 'مش قادر أطلّع اليوم والدقيقة بالظبط، راجعهم بنفسك.' : 'اللاعب هيشوف الحدث في الشباك وعمره ما هيبدأ.', refs: [cref('Activity.ini', s.line), schedRef].filter(Boolean) });
          continue;
        }
        const tmin = t => { const [h, m] = t.split(':').map(Number); return h * 60 + m; };
        const near = (a, b) => Math.abs(a - b) <= 10 || (b <= a && a - b <= 20);
        const sTimes = x => (x.hrs.length ? x.hrs : []).map(h => h * 60 + (x.min || 0));
        const dayMatch = x => !x.days || days.every(d => x.days.includes(d));
        const timeMatch = x => enters.some(en => sTimes(x).some(t => near(tmin(en), t)) || (x.hrEnd && x.hrs.length && tmin(en) >= x.hrs[0] * 60 && tmin(en) < x.hrEnd * 60));
        const full = sv.filter(x => dayMatch(x) && timeMatch(x));
        const gated = full.length ? full.every(x => x.gated) && gatedFlag.value === false : sv.every(x => x.gated) && gatedFlag.value === false;
        const issues = [];
        if (!full.length) { if (!sv.some(dayMatch)) issues.push('اليوم مختلف'); if (!sv.some(timeMatch)) issues.push('الساعة مختلفة'); if (!issues.length) issues.push('اليوم والساعة مش في نفس الشرط'); }
        if (gated) issues.push('الحدث متقفل (CustomEvents = false)');
        const DN = x => x.days ? (x.days.length === 7 ? 'كل يوم' : x.days.length === 6 ? 'كل يوم ما عدا ' + DAYS[ALLD.find(d => !x.days.includes(d))] : x.days.map(d => DAYS[d]).join('، ')) : 'كل يوم';
        const serverTxt = [...new Set(sv.map(x => `${DN(x)} ${x.hrs.length ? (x.hrEnd ? fmt(x.hrs[0], 0) + '–' + fmt(x.hrEnd, 0) : x.hrs.map(h => fmt(h, x.min || 0)).join(' و ')) : 'كل ساعة دقيقة ' + x.min}${x.gated ? ' (CustomEvents)' : ''}`))].join(' | ');
        const sev = issues.length ? 'error' : 'ok';
        rows.push({ name, s, sev, title: sev === 'ok' ? `«${name}» ميعاده مظبوط` : `«${name}»: ${issues.join(' و')}`, client: clientTxt, server: serverTxt, key: 'time:' + key,
          why: sev === 'ok' ? undefined : (gated ? 'الكود موجود بس متقفل بـ CustomEvents، فالحدث مش هيبدأ.' : 'اللاعب هيجي في الميعاد المكتوب في الكلاينت ومش هيلاقي الحدث.'),
          fix: sev === 'ok' ? undefined : 'يا إما تعدّل الشرط في MsgSchedules يطابق الكلاينت، يا إما تعدّل Date و Enter1 في Activity.ini (والكلاينت لازم يتحدّث عند اللاعبين).',
          refs: [cref('Activity.ini', s.line), ...(full.length ? full : sv).slice(0, 3).map(x => ref(x.file, x.line))] });
      }
      // a second slot of the same event (US/EU) that the server does not run is only informational when another slot matches
      const base = n => n.replace(/\s*\((US|EU)\)\s*$/i, '').trim();
      const okNames = new Set(rows.filter(r => r.sev === 'ok').map(r => base(r.name)));
      for (const r of rows) {
        if (r.sev === 'error' && okNames.has(base(r.name)) && !/CustomEvents/.test(r.title)) { r.sev = 'info'; r.title = `«${r.name}»: الكلاينت فيه ميعاد تاني السيرفر مش بيشغّله`; r.why = 'الحدث شغال في ميعاد واحد من اللي في الكلاينت. اللاعب اللي هيجي في الميعاد التاني مش هيلاقي حاجة.'; }
        C.findings.push(F(r.sev, r.title, r));
      }
    }

    // ── 3. NPCs
    progress(0.6, 'بفحص الـ NPCs…');
    {
      const C = cat('npc', 'الـ NPCs', 'كل NPC في السيرفر لازم يكون شكله موجود في الكلاينت، وواقف على خريطة الكلاينت يعرفها، ولو اللاعب بيكلّمه لازم يكون ليه كود في السيرفر.', 'npc');
      const dupBy = new Map(); for (const [a, b] of dupNpc) { if (a.map !== b.map) continue; if (!dupBy.has(b.uid)) dupBy.set(b.uid, [a]); dupBy.get(b.uid).push(b); }
      for (const [uid, arr] of dupBy) C.findings.push(F('error', `رقم NPC متكرر على نفس الخريطة: ${uid}`, { client: '—', server: arr.map(n => `${n.file}:${n.line}`).join('، ') + ` — خريطة ${mapLabel(arr[0].map)}`, key: 'dup:' + uid, why: 'السيرفر هيحمّل واحد بس منهم والباقي هيختفي.', fix: 'غيّر رقم الـ NPC الزيادة لرقم مش مستخدم.', refs: arr.slice(0, 4).map(n => sref(n.file, n.line)) }));
      if (false) for (const [a, b] of dupNpc) C.findings.push(F('error', `رقم NPC متكرر: ${b.uid}`, { client: '—', server: `${a.file}:${a.line} و ${b.file}:${b.line}`, key: 'dup:' + b.uid, why: 'السيرفر هيحمّل واحد بس منهم والتاني هيختفي أو يتلخبط.', fix: 'غيّر رقم واحد منهم لرقم مش مستخدم.', refs: [sref(a.file, a.line), sref(b.file, b.line)] }));
      if (npcsBlank) C.findings.push(F('error', `سطر فاضي في Npcs.txt بيوقف تحميل الـ NPCs`, { client: '—', server: `Npcs.txt سطر ${npcsBlank}`, key: 'npcsblank', why: 'LoadNpcs بيقف عند أول سطر فاضي، فكل الـ NPCs اللي بعده مش هتظهر في اللعبة.', fix: 'امسح السطر الفاضي.', refs: [sref('Npcs.txt', npcsBlank)] }));
      const missLook = [], noScript = [], badMapC = [], badMapS = [], shopIssues = [];
      const parked = [];
      for (const n of sNpcs.values()) {
        if (/jail/i.test(mapName(n.map))) { parked.push(n); continue; }
        if (!serverHasMap(n.map)) badMapS.push(n);
        else if (!clientHasMap(n.map)) badMapC.push(n);
        if (!n.sob && n.mesh && !lookKnown(n.mesh)) missLook.push(n);
        if (n.sob && n.mesh && n.type === 2 && !lookKnown(n.mesh)) missLook.push(n);
        if ((n.type === 2 || n.type === 32) && !n.sob && !scripted.has(n.uid)) noScript.push(n);
        if (n.type === 1 && !n.sob && !scripted.has(n.uid)) { const cs = cShops.has(n.uid), ss = sShops.has(n.uid); if (!cs || !ss) shopIssues.push([n, cs, ss]); }
      }
      if (parked.length) C.findings.push(F('info', `${parked.length} NPC متشالين على خرايط السجن`, { client: '—', server: [...new Set(parked.map(n => mapLabel(n.map)))].join('، '), key: 'parked', why: 'دول NPCs اتنقلوا لخريطة السجن عشان يختفوا من اللعبة، فمش بفحص شكلهم ولا الكود بتاعهم.', list: parked.map(n => `${npcLabel(n.uid)} — ${mapLabel(n.map)} (${n.x}, ${n.y})`) }));
      for (const n of badMapS) C.findings.push(F('error', `NPC ${npcLabel(n.uid)} على خريطة السيرفر مش بيحمّلها`, { client: '—', server: `خريطة ${n.map} مش في ServerDataBase/ini/GameMap.dat`, key: 'npcmap:' + n.uid, why: 'السيرفر بيتجاهل أي NPC على خريطة مش محمّلة، فالـ NPC مش هيظهر خالص.', fix: 'صلّح رقم الخريطة في السطر، أو ضيف الخريطة للـ GameMap.dat بتاع السيرفر.', refs: [sref(n.file, n.line)] }));
      for (const n of badMapC) C.findings.push(F('error', `NPC ${npcLabel(n.uid)} على خريطة الكلاينت مايعرفهاش`, { client: `خريطة ${n.map} مش في ini/GameMap.dat`, server: `${n.file}: خريطة ${mapLabel(n.map)} (${n.x}, ${n.y})`, key: 'npcmapc:' + n.uid, why: 'اللاعب مش هيقدر يدخل الخريطة دي أصلاً.', refs: [sref(n.file, n.line)] }));
      for (const n of missLook) C.findings.push(F('warn', `شكل NPC ${npcLabel(n.uid)} مش موجود في الكلاينت`, { client: `مفيش NpcType${Math.floor(n.mesh / 10)} في npc.ini / NpcX.ini / npcex.ini / terrainnpc.ini`, server: `Mesh ${n.mesh} على ${mapLabel(n.map)} (${n.x}, ${n.y})`, key: 'look:' + n.uid, why: 'الغالب إن الـ NPC هيبقى مخفي أو مكانه فاضي، واللاعب مش هيعرف يدوس عليه.', fix: 'استخدم Mesh موجود في npc.ini، أو ضيف [NpcType' + Math.floor(n.mesh / 10) + '] للكلاينت.', refs: [sref(n.file, n.line)] }));
      const nsBy = new Map(); for (const n of noScript) { if (!nsBy.has(n.map)) nsBy.set(n.map, []); nsBy.get(n.map).push(n); }
      for (const [mp, arr] of nsBy) if (arr.length > 1) C.findings.push(F('warn', `${arr.length} NPC على ${mapLabel(mp)} بيتكلموا ومالهمش كود`, { client: [...new Set(arr.map(n => npcLookName(n.mesh)).filter(Boolean))].slice(0, 6).join('، ') || '—', server: arr.slice(0, 8).map(n => npcLabel(n.uid)).join('، ') + (arr.length > 8 ? '…' : ''), key: 'script:m' + mp, why: 'اللاعب هيدوس على أي واحد منهم ومش هيظهرله أي كلام. لو دول ديكور بس، غيّر نوعهم لـ 0 بدل 2.', fix: 'لكل NPC: ضيف رقمه في NpcID enum ودالة عليها [NpcAttribute(NpcID.اسمه)]، أو امسحه.', list: arr.map(n => `${npcLabel(n.uid)} — نوع ${n.type} (${n.x}, ${n.y}) — ${n.file}:${n.line}`), refs: arr.slice(0, 5).map(n => sref(n.file, n.line)) }));
      for (const n of noScript.filter(n => nsBy.get(n.map).length === 1)) C.findings.push(F('warn', `NPC ${npcLabel(n.uid)} بيتكلم ومالوش كود`, { client: npcLookName(n.mesh) ? 'الشكل: ' + npcLookName(n.mesh) : '—', server: `${n.file}: نوع ${n.type} على ${mapLabel(n.map)} (${n.x}, ${n.y})`, key: 'script:' + n.uid, why: 'اللاعب هيدوس على الـ NPC ومش هيظهرله أي كلام.', fix: `ضيف للـ NpcID enum سطر بالرقم ${n.uid}، وبعدين دالة عليها [NpcAttribute(NpcID.اسمه)]. أو امسح الـ NPC لو مش محتاجه.`, refs: [sref(n.file, n.line)] }));
      for (const [n, cs, ss] of shopIssues) C.findings.push(F(cs || ss ? 'warn' : 'error', `محل ${npcLabel(n.uid)} ${!cs && !ss ? 'مالوش بضاعة في الناحيتين' : !cs ? 'الكلاينت مش عارف بضاعته' : 'السيرفر مش عارف بضاعته'}`, { client: cs ? 'موجود في Shop.dat' : 'مش موجود في ini/Shop.dat', server: ss ? 'موجود في shops/Shop.dat' : 'مش موجود في shops/Shop.dat', key: 'shopnpc:' + n.uid, why: !cs ? 'شباك المحل هيفتح فاضي.' : 'اللاعب هيشوف بضاعة ومش هيقدر يشتريها.', fix: 'ضيف [ShopN] بنفس ID = ' + n.uid + ' في الملف الناقص.', refs: [sref(n.file, n.line)] }));
      // client links to NPCs (quests, daily activity, navigation)
      progress(0.68, 'بفحص روابط الـ NPCs في الكلاينت…');
      const links = new Map(); // uid -> [{map,x,y,src,line,name}]
      const addLink = (uid, map, x, y, srcf, line, name) => { if (!uid) return; if (!links.has(uid)) links.set(uid, []); links.get(uid).push({ map, x, y, src: srcf, line, name }); };
      const qc = await cText('Questinfo.ini');
      if (qc) {
        const li = lineIndex(qc);
        for (const m of qc.matchAll(/^(BeginNpcId|FinishNpcId)=(\d+),(\d+),(\d+),(\d+),([^,\r\n]*)/gm)) addLink(+m[2], +m[3], +m[4], +m[5], 'Questinfo.ini', lineAt(m.index, li), m[6]);
        for (const m of qc.matchAll(/\[npc\s+([^,\]]*),(\d+),(\d+),(\d+),(\d+)/g)) addLink(+m[2], +m[3], +m[4], +m[5], 'Questinfo.ini', lineAt(m.index, li), m[1]);
      }
      const da = await cText('DailyActivity.ini');
      if (da) { const li = lineIndex(da); for (const m of da.matchAll(/\[npc,([^,]*),(\d+),(\d+),(\d+),(\d+)/g)) addLink(+m[2], +m[3], +m[4], +m[5], 'DailyActivity.ini', lineAt(m.index, li), m[1]); }
      const md = null; // MapDestination.dat npcid is not always an NPC UID (places use it too), so it is not used
      if (md) { const ini = parseIni(md); for (const k of ini.order) { const s = ini.sections.get(k); if (s.kv.npcid && +s.kv.npcid) addLink(+s.kv.npcid, +(s.kv.mapid || k.split('-')[0]), +s.kv.posx, +s.kv.posy, 'MapDestination.dat', s.line, (s.kv.title || '').replace(/~/g, ' ')); } }
      for (const [uid, arr] of [...links.entries()].sort((a, b) => a[0] - b[0])) {
        const n = sNpcs.get(uid); const first = arr[0];
        const srcs = [...new Set(arr.map(a => a.src))].join('، ');
        const nm = first.name || npcEnumName.get(uid) || '';
        const label = nm ? `${uid} (${nm})` : String(uid);
        if (!n) {
          const sc = scripted.has(uid);
          C.findings.push(F(sc ? 'info' : 'warn', `الكلاينت بيدل اللاعب على NPC ${label} والسيرفر مش حاطه`, { client: `${srcs}: ${mapLabel(first.map)} (${first.x}, ${first.y})${arr.length > 1 ? ` — ${arr.length} مرة` : ''}`, server: sc ? 'ليه كود بس مش موجود في Npcs.txt (ممكن يتحط من الكود)' : 'مش موجود في Npcs.txt ولا SobNpcs.txt', key: 'link:' + uid, why: 'اللاعب هيدوس على اسم الـ NPC في المهمة/الدليل ويمشي لمكان فاضي.', fix: `ضيف سطر في Npcs.txt: ${uid},2,MESH,${first.map},${first.x},${first.y}`, refs: arr.slice(0, 3).map(a => cref(a.src, a.line)) }));
          continue;
        }
        const wrong = arr.filter(a => a.map !== n.map && !(mapAlias.get(n.map) === a.map));
        const far = arr.filter(a => a.map === n.map && Math.hypot(a.x - n.x, a.y - n.y) > 12);
        if (wrong.length) C.findings.push(F('warn', `NPC ${label} واقف على خريطة غير اللي الكلاينت بيقول عليها`, { client: `${mapLabel(wrong[0].map)} (${wrong[0].x}, ${wrong[0].y}) — ${[...new Set(wrong.map(a => a.src))].join('، ')}`, server: `${mapLabel(n.map)} (${n.x}, ${n.y})`, key: 'linkmap:' + uid, why: 'المهمة أو الدليل هيودّي اللاعب لخريطة تانية.', refs: [cref(wrong[0].src, wrong[0].line), sref(n.file, n.line)] }));
        else if (far.length) { const a = far[0]; const d = Math.round(Math.hypot(a.x - n.x, a.y - n.y)); C.findings.push(F('warn', `NPC ${label} بعيد ${d} خطوة عن المكان اللي الكلاينت بيقول عليه`, { client: `${mapLabel(a.map)} (${a.x}, ${a.y}) — ${[...new Set(far.map(a => a.src))].join('، ')}`, server: `(${n.x}, ${n.y})`, key: 'linkfar:' + uid, why: 'زرار المشي التلقائي في المهمة هيوقف اللاعب في مكان بعيد عن الـ NPC.', fix: `انقل الـ NPC لـ (${a.x}, ${a.y}) في ${n.file}، أو عدّل الإحداثيات في الكلاينت.`, refs: [cref(a.src, a.line), sref(n.file, n.line)] })); }
        else C.findings.push(F('ok', `NPC ${label} في مكانه`, { client: `${mapLabel(first.map)} (${first.x}, ${first.y})`, server: `(${n.x}, ${n.y})`, key: 'linkok:' + uid }));
      }
      meta.npcs = { server: sNpcs.size, scripts: scripted.size, links: links.size };
    }

    // ── 4. Maps
    progress(0.74, 'بقارن الخرايط…');
    {
      const C = cat('map', 'الخرايط', 'السيرفر بيحمّل الخرايط من ServerDataBase/ini/GameMap.dat والكلاينت من ini/GameMap.dat. أي خريطة بيستخدمها السيرفر لازم تكون عند الكلاينت بنفس الملف.', 'map');
      const used = new Map(); // map -> [ref]
      const use = (m, r, why) => { if (!used.has(m)) used.set(m, []); used.get(m).push({ r, why }); };
      for (const n of sNpcs.values()) use(n.map, sref(n.file, n.line), 'NPC ' + n.uid);
      const portals = parseIni((await sText('portals.ini')) || '');
      for (const k of portals.order) for (const [kk, v, line] of portals.sections.get(k).all) if (/^Portal(Enter|Exit)\d+$/.test(kk)) { const p = v.split(/\s+/).map(Number); use(p[0], sref('portals.ini', line), 'بوابة ' + kk); }
      const swapped = new Map();
      for (const { f, m, idx } of findInCode(/\.Teleport\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d{2,5})\s*[,)]/g)) { use(+m[3], ref(f, L(f, idx)), `Teleport(${m[1]}, ${m[2]})`); if (!serverHasMap(+m[3]) && serverHasMap(+m[1])) swapped.set(+m[3], `Teleport(${m[1]}, ${m[2]}, ${m[3]})`); }
      for (const { f, m, idx } of findInCode(/SendInvitation\(\s*"[^"]*"\s*,\s*"[^"]*"\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)/g)) use(+m[3], ref(f, L(f, idx)), 'دعوة حدث');
      for (const [id, arr] of [...used.entries()].sort((a, b) => a[0] - b[0])) {
        if (!id) continue;
        const sOk = serverHasMap(id), cOk = clientHasMap(id);
        if (sOk && cOk) continue;
        const uses = [...new Set(arr.map(a => a.why.replace(/\d+$/, '').trim()))].slice(0, 4).join('، ');
        C.findings.push(F('error', `خريطة ${mapLabel(id)} مستخدمة بس ${!cOk && !sOk ? 'مش موجودة في الناحيتين' : !cOk ? 'الكلاينت مايعرفهاش' : 'السيرفر مش بيحمّلها'}`, {
          client: cOk ? (cMaps.get(id) || {}).path || 'موجودة' : 'مش في ini/GameMap.dat', server: sOk ? ((sMaps.get(id) || {}).path || 'خريطة فرعية من ' + mapAlias.get(id)) : 'مش في ServerDataBase/ini/GameMap.dat', key: 'mapuse:' + id,
          why: `مستخدمة في ${arr.length} مكان (${uses}). ` + (!cOk ? 'اللاعب اللي يتنقل ليها هيقع أو يفضل على شاشة سودا.' : 'النقل ليها هيفشل في السيرفر.'),
          fix: swapped.has(id) ? `شكل الأرقام متبدلة. الترتيب الصح Teleport(X, Y, MAP)، والكود كاتب ${swapped.get(id)}.\nغالباً المقصود Teleport(${swapped.get(id).match(/\d+/g)[1]}, ${swapped.get(id).match(/\d+/g)[2]}, ${swapped.get(id).match(/\d+/g)[0]}).` : (!cOk && sOk ? 'ضيف الخريطة لـ ini/GameMap.dat في الكلاينت (ومعاها ملف الماب)، أو غيّر الكود يودّي لخريطة موجودة.' : 'غيّر رقم الخريطة لخريطة موجودة، أو ضيفها للناحية الناقصة.'),
          refs: arr.slice(0, 5).map(a => a.r)
        }));
      }
      // file path differences
      const walkDir = roots.db + 'map/walk/';
      const walkFiles = new Set((await fs.server.walk(walkDir, ['.walk'])).map(p => p.split('/').pop().replace(/\.walk$/i, '').toLowerCase()));
      let same = 0;
      for (const [id, c] of cMaps) {
        const s = sMaps.get(id); if (!s) continue;
        const cp = c.path.toLowerCase().replace(/\.\w+$/, ''), sp = s.path.toLowerCase().replace(/\.\w+$/, '');
        if (cp === sp) { same++; continue; }
        const base = cp.split('/').pop(); const ov = walkFiles.has(base);
        C.findings.push(F(ov ? 'ok' : 'warn', `خريطة ${mapLabel(id)} ملفها مختلف بين الناحيتين`, { client: c.path, server: s.path + (ov ? ' + ' + base + '.walk' : ''), key: 'mappath:' + id, why: ov ? 'فيه ملف walk بيصحح أماكن المشي، فالموضوع متغطي.' : 'أماكن المشي ممكن تختلف، فاللاعب يتمنع يمشي أو ينط في أماكن مفتوحة عنده.', refs: [] }));
      }
      const onlyC = [...cMaps.keys()].filter(id => !sMaps.has(id)), onlyS = [...sMaps.keys()].filter(id => !cMaps.has(id));
      if (onlyS.length) C.findings.push(F('info', `${onlyS.length} خريطة عند السيرفر بس`, { client: 'مش موجودين', server: onlyS.slice(0, 40).map(mapLabel).join('، ') + (onlyS.length > 40 ? '…' : ''), key: 'mapsonlys', why: 'مش مشكلة طول ما مفيش حاجة بتنقل اللاعب ليهم (الجزء اللي فوق بيفحص ده).', list: onlyS.map(mapLabel) }));
      if (onlyC.length) C.findings.push(F('info', `${onlyC.length} خريطة عند الكلاينت بس`, { client: onlyC.slice(0, 40).map(mapLabel).join('، ') + (onlyC.length > 40 ? '…' : ''), server: 'مش بيحمّلها', key: 'mapsonlyc', why: 'أي نقل ليهم هيفشل، بس غالباً محدش بيستخدمهم.', list: onlyC.map(mapLabel) }));
      meta.maps = { client: cMaps.size, server: sMaps.size, same };
    }

    // ── 5. Items & shops
    progress(0.8, 'بقارن الأيتمز والمحلات…');
    let cItems = new Map(), sItems = new Map();
    {
      const C = cat('item', 'الأيتمز والمحلات', 'أي أيتم السيرفر بيدّيه أو بيبيعه لازم يكون موجود في itemtype.dat بتاع الكلاينت، وبضاعة كل محل لازم تبقى نفسها في الناحيتين.', 'item');
      const ci = await cText('itemtype.dat'); const si = await sText('itemtype.txt');
      if (ci) for (const r of parseRows(ci)) if (/^\d+$/.test(r.cols[0])) cItems.set(+r.cols[0], { name: r.cols[1], cols: r.cols, line: r.line });
      // the client also loads ini/ItemtypeSub.dat (newer items, e.g. Windwalker gear) on top of itemtype.dat
      const cis = await cText('ItemtypeSub.dat');
      if (cis) for (const r of parseRows(cis)) if (/^\d+$/.test(r.cols[0]) && !cItems.has(+r.cols[0])) cItems.set(+r.cols[0], { name: r.cols[1], cols: r.cols, line: r.line, sub: true });
      if (si) for (const r of parseRows(si)) if (/^\d+$/.test(r.cols[0])) sItems.set(+r.cols[0], { name: r.cols[1], cols: r.cols, line: r.line });
      const itemName = id => (sItems.get(id) || cItems.get(id) || {}).name || '';
      const itemLabel = id => itemName(id) ? `${id} ${itemName(id)}` : String(id);
      const onlyS = [...sItems.keys()].filter(id => !cItems.has(id));
      const onlyC = [...cItems.keys()].filter(id => !sItems.has(id));
      if (onlyS.length) C.findings.push(F('error', `${onlyS.length} أيتم في السيرفر والكلاينت مايعرفهمش`, { client: 'مش في ini/itemtype.dat', server: onlyS.slice(0, 12).map(itemLabel).join('، ') + (onlyS.length > 12 ? '…' : ''), key: 'itemonlys', why: 'لو اللاعب أخد واحد منهم، الأيتم هيظهر من غير اسم وشكل، وأحياناً الكلاينت بيقفل.', fix: 'ضيف السطور دي لـ itemtype.dat بتاع الكلاينت (بأداة تشفير الـ dat) أو امسحهم من السيرفر.', list: onlyS.map(itemLabel), refs: [sref('itemtype.txt', sItems.get(onlyS[0]).line)] }));
      if (onlyC.length) C.findings.push(F('info', `${onlyC.length} أيتم في الكلاينت والسيرفر مايعرفهمش`, { client: onlyC.slice(0, 12).map(id => `${id} ${cItems.get(id).name}`).join('، ') + (onlyC.length > 12 ? '…' : ''), server: 'مش في itemtype.txt', key: 'itemonlyc', why: 'مش بيبوّظ حاجة، بس أي حاجة في الكلاينت بتعرضهم (شوب، مهمة، جايزة) مش هتشتغل.', list: onlyC.map(id => `${id} ${cItems.get(id).name}`) }));
      const nameDiff = [...sItems.keys()].filter(id => cItems.has(id) && cItems.get(id).name !== sItems.get(id).name);
      if (nameDiff.length) C.findings.push(F('info', `${nameDiff.length} أيتم اسمه مختلف بين الناحيتين`, { client: nameDiff.slice(0, 6).map(id => `${id} ${cItems.get(id).name}`).join('، '), server: nameDiff.slice(0, 6).map(id => `${id} ${sItems.get(id).name}`).join('، '), key: 'itemname', why: 'رسايل السيرفر (زي "أخدت X") هتكتب اسم غير اللي ظاهر في الشنطة.', list: nameDiff.map(id => `${id}: ${cItems.get(id).name} ⇄ ${sItems.get(id).name}`) }));
      // price column diff (col 7 = price in TQ itemtype)
      const PRICE = 7; const priceDiff = [...sItems.keys()].filter(id => cItems.has(id) && cItems.get(id).cols[PRICE] !== undefined && normVal(cItems.get(id).cols[PRICE]) !== normVal(sItems.get(id).cols[PRICE] || ''));
      if (priceDiff.length) C.findings.push(F('info', `${priceDiff.length} أيتم سعره مختلف`, { client: priceDiff.slice(0, 5).map(id => `${itemLabel(id)}: ${cItems.get(id).cols[PRICE]}`).join('، '), server: priceDiff.slice(0, 5).map(id => `${sItems.get(id).cols[PRICE]}`).join('، '), key: 'itemprice', why: 'السعر اللي ظاهر للاعب في المحل مش هو اللي هيتخصم.', list: priceDiff.map(id => `${itemLabel(id)}: كلاينت ${cItems.get(id).cols[PRICE]} ⇄ سيرفر ${sItems.get(id).cols[PRICE]}`) }));

      // shops
      for (const id of new Set([...cShops.keys(), ...sShops.keys()])) {
        const c = cShops.get(id), s = sShops.get(id);
        const nm = (c || s).name || '';
        const lab = `محل ${id}${nm ? ' (' + nm + ')' : ''}`;
        if (!s) { C.findings.push(F('warn', `${lab} في الكلاينت بس`, { client: `${c.items.length} أيتم`, server: 'مش في shops/Shop.dat', key: 'shop:' + id, why: 'اللاعب هيشوف البضاعة ومحدش هيبيعهاله.', refs: [cref('Shop.dat', c.line)] })); continue; }
        if (!c) { const used = sNpcs.has(id); C.findings.push(F(used ? 'warn' : 'info', `${lab} في السيرفر بس`, { client: 'مش في ini/Shop.dat', server: `${s.items.length} أيتم`, key: 'shop:' + id, why: used ? 'الـ NPC موجود بس شباك المحل هيفتح فاضي.' : 'مفيش NPC بالرقم ده، فالمحل مش مستخدم غالباً.', refs: [sref('shops/Shop.dat', s.line)] })); continue; }
        const cSet = new Set(c.items), sSet = new Set(s.items);
        const notSold = c.items.filter(i => !sSet.has(i)), hidden = s.items.filter(i => !cSet.has(i));
        const notInClient = s.items.filter(i => cItems.size && !cItems.has(i)), notInServer = s.items.filter(i => sItems.size && !sItems.has(i));
        if (notSold.length) C.findings.push(F('error', `${lab}: ${notSold.length} أيتم ظاهر للاعب ومش بيتباع`, { client: notSold.map(itemLabel).join('، '), server: 'مش في قايمة المحل', key: 'shopns:' + id, why: 'اللاعب هيحاول يشتري والسيرفر هيرفض.', fix: 'ضيفهم للمحل في shops/Shop.dat أو امسحهم من ini/Shop.dat.', refs: [cref('Shop.dat', c.line), sref('shops/Shop.dat', s.line)] }));
        if (hidden.length) C.findings.push(F('info', `${lab}: ${hidden.length} أيتم بيتباع ومش ظاهر`, { client: 'مش ظاهرين في الشباك', server: hidden.map(itemLabel).join('، '), key: 'shophid:' + id, why: 'بضاعة محدش هيعرف يشتريها.', refs: [cref('Shop.dat', c.line), sref('shops/Shop.dat', s.line)] }));
        if (notInServer.length) C.findings.push(F('error', `${lab}: ${notInServer.length} أيتم مش موجود في itemtype بتاع السيرفر`, { client: '—', server: notInServer.join('، '), key: 'shopnis:' + id, why: 'الشراء هيفشل أو هيدّي أيتم بايظ.', refs: [sref('shops/Shop.dat', s.line)] }));
        if (notInClient.length && !notInServer.length) C.findings.push(F('error', `${lab}: ${notInClient.length} أيتم مش موجود في itemtype بتاع الكلاينت`, { client: notInClient.join('، '), server: '—', key: 'shopnic:' + id, why: 'الأيتم هيظهر في الشنطة من غير اسم أو شكل.', refs: [sref('shops/Shop.dat', s.line)] }));
        if (!notSold.length && !hidden.length && !notInClient.length && !notInServer.length) C.findings.push(F('ok', `${lab} مطابق`, { client: `${c.items.length} أيتم`, server: `${s.items.length} أيتم`, key: 'shopok:' + id }));
      }
      // CP mall (emoneyShopV2)
      const ems = t => { const set = new Map(); if (!t) return set; t.split(/\r?\n/).forEach((l, i) => { const m = /^\s*(\d{4,})\s+item/.exec(l); if (m && !set.has(+m[1])) set.set(+m[1], i + 1); }); return set; };
      const ce = ems(await cText('emoneyShopV2.ini')), se = ems(await sText('shops/emoneyShopV2.ini'));
      if (ce.size || se.size) {
        const a = [...ce.keys()].filter(i => !se.has(i)), b = [...se.keys()].filter(i => !ce.has(i));
        const bad = [...ce.keys()].filter(i => sItems.size && !sItems.has(i));
        if (a.length) C.findings.push(F('error', `متجر الـ CP: ${a.length} أيتم ظاهر للاعب والسيرفر مش بيبيعه`, { client: a.slice(0, 15).map(itemLabel).join('، ') + (a.length > 15 ? '…' : ''), server: 'مش في shops/emoneyShopV2.ini', key: 'ems-a', why: 'اللاعب هيدوس شراء ومش هيحصل حاجة.', list: a.map(itemLabel), refs: [cref('emoneyShopV2.ini', ce.get(a[0]))] }));
        if (b.length) C.findings.push(F('info', `متجر الـ CP: ${b.length} أيتم السيرفر بيبيعه ومش ظاهر`, { client: 'مش ظاهرين', server: b.slice(0, 15).map(itemLabel).join('، ') + (b.length > 15 ? '…' : ''), key: 'ems-b', list: b.map(itemLabel), refs: [sref('shops/emoneyShopV2.ini', se.get(b[0]))] }));
        if (bad.length) C.findings.push(F('error', `متجر الـ CP: ${bad.length} أيتم مش موجود في itemtype بتاع السيرفر`, { client: bad.slice(0, 15).join('، '), server: 'مش في itemtype.txt', key: 'ems-bad', list: bad.map(String) }));
        if (!a.length && !b.length && !bad.length) C.findings.push(F('ok', 'متجر الـ CP مطابق', { client: ce.size + ' أيتم', server: se.size + ' أيتم', key: 'ems-ok' }));
      }
      meta.items = { client: cItems.size, server: sItems.size };
    }

    // ── 6. Data tables
    progress(0.88, 'بقارن جداول الداتا…');
    {
      const C = cat('data', 'جداول الداتا', 'ملفات موجودة في الناحيتين ولازم يبقوا نسخة واحدة: لو السيرفر بيحسب بقيمة والكلاينت بيعرض قيمة تانية، اللاعب هيشوف أرقام غلط.', 'table');
      const allCodeText = [...src.values()].map(e => e.code).join('\n');
      const serverReads = name => { const base = name.replace(/\.\w+$/, ''); return new RegExp('["\\\\/]' + base.replace(/[.*+?^${}()|[\]\\]/g, '\\$&') + '\\.(txt|ini|dat)"', 'i').test(allCodeText); };
      const TABLES = [
        { c: 'title_type.dat', s: 'title_type.txt', key: 1, label: 'الألقاب (Titles)' },
        { c: 'magictype.dat', s: 'magictype.txt', key: 2, label: 'المهارات (magictype)' },
        { c: 'magictypeop.dat', s: 'magictypeop.txt', key: 2, label: 'ترقية المهارات (magictypeop)' },
        { c: 'ItemtypeSub.dat', s: 'ItemtypeSub.txt', key: 1, label: 'ItemtypeSub' },
        { c: 'coat_storage_type.dat', s: 'coat_storage_type.txt', key: 1, label: 'دولاب الملابس (coat storage)' },
        { c: 'exchange_shop_goods.dat', s: 'exchange_shop_goods.txt', key: 1, label: 'Exchange Shop' },
        { c: 'operating_prize.dat', s: 'operating_prize.txt', key: 1, label: 'جوايز الشحن (operating prize)' },
        { c: 'task_reward_type.dat', s: 'task_reward_type.ini', key: 1, label: 'جوايز المهام (task reward)' },
        { c: 'item_refine_upgrade.dat', s: 'item_refine_upgrade.txt', key: 2, label: 'ترقية الـ Refinery' },
        { c: 'itemtype.dat', s: 'itemtype.txt', key: 1, label: 'itemtype (كل الأعمدة)' }
      ];
      for (const T of TABLES) {
        const ct = await cText(T.c), st = await sText(T.s);
        if (!ct || !st) { if (ct || st) C.findings.push(F('info', `${T.label}: الملف موجود في ناحية واحدة بس`, { client: ct ? T.c : 'مش موجود', server: st ? T.s : 'مش موجود', key: 'tbl-miss:' + T.c })); continue; }
        const k = r => r.cols.slice(0, T.key).map(normVal).join('|');
        const cr = new Map(), sr = new Map();
        for (const r of parseRows(ct)) if (/^\d/.test(r.cols[0])) cr.set(k(r), r);
        for (const r of parseRows(st)) if (/^\d/.test(r.cols[0])) sr.set(k(r), r);
        const onlyC = [...cr.keys()].filter(x => !sr.has(x)), onlyS = [...sr.keys()].filter(x => !cr.has(x));
        const diff = []; const colCount = {};
        for (const [kk, a] of cr) { const b = sr.get(kk); if (!b) continue; const n = Math.max(a.cols.length, b.cols.length); const cols = []; for (let i = T.key; i < n; i++) if (normVal(a.cols[i] || '0') !== normVal(b.cols[i] || '0')) cols.push(i); if (cols.length) { diff.push([kk, a, b, cols]); cols.forEach(i => colCount[i] = (colCount[i] || 0) + 1); } }
        const reads = serverReads(T.s);
        const sev = (onlyC.length || onlyS.length || diff.length) ? (reads ? 'warn' : 'info') : 'ok';
        const topCols = Object.entries(colCount).sort((a, b) => b[1] - a[1]).slice(0, 4).map(([c, n]) => `عمود ${+c + 1} (${n})`).join('، ');
        const parts = []; if (onlyC.length) parts.push(`${onlyC.length} سطر في الكلاينت بس`); if (onlyS.length) parts.push(`${onlyS.length} سطر في السيرفر بس`); if (diff.length) parts.push(`${diff.length} سطر قيمه مختلفة`);
        C.findings.push(F(sev, sev === 'ok' ? `${T.label} مطابق (${cr.size} سطر)` : `${T.label}: ${parts.join('، ')}`, {
          client: `${T.c} — ${cr.size} سطر`, server: `${T.s} — ${sr.size} سطر` + (reads ? '' : ' (السيرفر مش بيقرا الملف ده في الكود)'), key: 'tbl:' + T.c,
          why: sev === 'ok' ? undefined : (reads ? 'القيم دي السيرفر بيستخدمها، فاللي ظاهر للاعب مش هو اللي بيحصل.' + (topCols ? ' أكتر أعمدة مختلفة: ' + topCols + '.' : '') : 'السيرفر مش بيقرا الملف ده، فالفرق مش بيأثر غالباً.'),
          list: sev === 'ok' ? undefined : [...onlyC.slice(0, 300).map(x => 'كلاينت بس: ' + x + ' ' + (cr.get(x).cols[T.key] || '')), ...onlyS.slice(0, 300).map(x => 'سيرفر بس: ' + x + ' ' + (sr.get(x).cols[T.key] || '')), ...diff.slice(0, 300).map(([kk, a, b, cols]) => `${kk}: ` + cols.slice(0, 4).map(i => `عمود ${i + 1}: ${a.cols[i] ?? '∅'} ⇄ ${b.cols[i] ?? '∅'}`).join(' · '))],
          refs: [cref(T.c, 1), sref(T.s, 1)]
        }));
      }
      // INI pairs
      const INIS = [
        { f: 'ActivityTask.ini', label: 'مهام النشاط اليومي (ActivityTask)' },
        { f: 'OperateActivity.ini', label: 'OperateActivity' },
        { f: 'StageGoal.dat', s: 'StageGoal.txt', label: 'أهداف المراحل (StageGoal)' },
        { f: 'GameMapEx.ini', label: 'أسماء الخرايط (GameMapEx)' },
        { f: 'NeiGongInfo.ini', label: 'الـ Inner Power (NeiGongInfo)' },
        { f: 'SubProfessionInfo.ini', label: 'الـ Sub-classes (SubProfessionInfo)' }
      ];
      for (const T of INIS) {
        const ct = await cText(T.f), st = await sText(T.s || T.f);
        if (!ct || !st) continue;
        const a = parseIni(ct), b = parseIni(st);
        const onlyC = a.order.filter(x => !b.sections.has(x)), onlyS = b.order.filter(x => !a.sections.has(x));
        const diffs = [];
        for (const sec of a.order) { const x = a.sections.get(sec), y = b.sections.get(sec); if (!y) continue; for (const kk of new Set([...Object.keys(x.kv), ...Object.keys(y.kv)])) { if (normVal(x.kv[kk] ?? '∅') !== normVal(y.kv[kk] ?? '∅')) diffs.push(`[${sec}] ${kk}: ${x.kv[kk] ?? '∅'} ⇄ ${y.kv[kk] ?? '∅'}`); } }
        const reads = serverReads(T.s || T.f);
        const sev = (onlyC.length || onlyS.length || diffs.length) ? (reads ? 'warn' : 'info') : 'ok';
        const parts = []; if (onlyC.length) parts.push(`${onlyC.length} قسم في الكلاينت بس`); if (onlyS.length) parts.push(`${onlyS.length} قسم في السيرفر بس`); if (diffs.length) parts.push(`${diffs.length} قيمة مختلفة`);
        C.findings.push(F(sev, sev === 'ok' ? `${T.label} مطابق` : `${T.label}: ${parts.join('، ')}`, { client: `${T.f} — ${a.order.length} قسم`, server: `${T.s || T.f} — ${b.order.length} قسم` + (reads ? '' : ' (السيرفر مش بيقرا الملف ده)'), key: 'ini:' + T.f, why: sev === 'ok' ? undefined : (reads ? 'الشباك في الكلاينت هيعرض حاجة والسيرفر هيحسب حاجة تانية.' : 'السيرفر مش بيقرا الملف ده، فالفرق غالباً مش مؤثر.'), list: sev === 'ok' ? undefined : [...onlyC.map(x => 'كلاينت بس: [' + x + ']'), ...onlyS.map(x => 'سيرفر بس: [' + x + ']'), ...diffs.slice(0, 400)], refs: [cref(T.f, 1), sref(T.s || T.f, 1)] }));
      }
      // Quests by MissionId
      const qid = t => { const m = new Map(); if (!t) return m; const ini = parseIni(t); for (const k of ini.order) { const s = ini.sections.get(k); if (s.kv.MissionId) m.set(+s.kv.MissionId, { name: s.kv.Name, line: s.line, begin: s.kv.BeginNpcId, finish: s.kv.FinishNpcId }); } return m; };
      const cq = qid(await cText('Questinfo.ini')), sq = qid(await sText('Questinfo.ini'));
      if (cq.size && sq.size) {
        const onlyC = [...cq.keys()].filter(x => !sq.has(x)), onlyS = [...sq.keys()].filter(x => !cq.has(x));
        const npcDiff = [...cq.keys()].filter(x => sq.has(x) && ((cq.get(x).begin || '') !== (sq.get(x).begin || '') || (cq.get(x).finish || '') !== (sq.get(x).finish || '')));
        if (onlyC.length) C.findings.push(F('warn', `${onlyC.length} مهمة ظاهرة في الكلاينت ومش في Questinfo بتاع السيرفر`, { client: onlyC.slice(0, 8).map(x => `${x} ${cq.get(x).name}`).join('، '), server: 'مش موجودة', key: 'q-c', why: 'اللاعب هيشوفها في شباك المهام والسيرفر مش هيعرف يتابعها.', list: onlyC.map(x => `${x} ${cq.get(x).name}`), refs: [cref('Questinfo.ini', cq.get(onlyC[0]).line)] }));
        if (onlyS.length) C.findings.push(F('info', `${onlyS.length} مهمة في السيرفر بس`, { client: 'مش موجودة', server: onlyS.slice(0, 8).map(x => `${x} ${sq.get(x).name}`).join('، '), key: 'q-s', why: 'المهام دي مش هتظهر في شباك المهام عند اللاعب.', list: onlyS.map(x => `${x} ${sq.get(x).name}`) }));
        if (npcDiff.length) C.findings.push(F('info', `${npcDiff.length} مهمة مكان الـ NPC بتاعها مختلف بين نسختين Questinfo`, { client: npcDiff.slice(0, 3).map(x => `${x}: ${cq.get(x).begin}`).join('، '), server: npcDiff.slice(0, 3).map(x => `${sq.get(x).begin}`).join('، '), key: 'q-npc', why: 'اللي بيهم هو مكان الـ NPC الحقيقي في Npcs.txt، وده متفحوص في قسم الـ NPCs.', list: npcDiff.map(x => `${x} ${cq.get(x).name}: ${cq.get(x).begin} ⇄ ${sq.get(x).begin}`) }));
        if (!onlyC.length && !onlyS.length && !npcDiff.length) C.findings.push(F('ok', `المهام (Questinfo) مطابقة — ${cq.size} مهمة`, { key: 'q-ok', client: cq.size + ' مهمة', server: sq.size + ' مهمة' }));
      }
    }

    progress(1, 'خلصت');
    // summary
    for (const c of cats) {
      c.findings.sort((a, b) => SEV[b.sev] - SEV[a.sev]);
      c.counts = { error: 0, warn: 0, info: 0, ok: 0 }; for (const f of c.findings) c.counts[f.sev]++;
    }
    const totals = { error: 0, warn: 0, info: 0, ok: 0 }; for (const c of cats) for (const k in totals) totals[k] += c.counts[k];
    meta.ms = Date.now() - t0; meta.at = new Date().toISOString();
    return { version: 1, meta, totals, categories: cats };
  }

  const api = { analyze, datDecrypt, parseIni, parseRows, decode };
  if (typeof module !== 'undefined' && module.exports) module.exports = api; else root.CompatEngine = api;
})(typeof self !== 'undefined' ? self : this);
