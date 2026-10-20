# -*- coding: utf-8 -*-
"""
gapfinder - lists what the official 6609 client supports that the C# server does not answer.

Read-only: it only reads the client exe/logs, the server source and the PacketAudit logs,
and writes gapfinder_report.html + gapfinder_report.json next to this script.

Optional packages: capstone (confirms the message ids by disassembly), pefile (PE parsing).
Without them the tool still runs (built-in PE parser + byte patterns).
"""
import os, re, sys, glob, json, struct, time, html, array, bisect, configparser, collections, datetime

HERE = os.path.dirname(os.path.abspath(__file__))
T0 = time.time()

try:
    import capstone  # type: ignore
except Exception:
    capstone = None
try:
    import pefile  # type: ignore
except Exception:
    pefile = None

# known (class, id) pairs used to check the id extraction
KNOWN = {
    'CMsgTexasExInteractive': 3400, 'CMsgLeagueOpt': 2622, 'CMsgAction': 10010,
    'CMsgItem': 1009, 'CMsgTalk': 1004, 'CMsgWalk': 10005,
}


def log(*a):
    print('[%5.1fs]' % (time.time() - T0), *a, flush=True)


# ----------------------------------------------------------------------------------------------
# config
# ----------------------------------------------------------------------------------------------
def load_config():
    cp = configparser.ConfigParser(interpolation=None)
    cp.read(os.path.join(HERE, 'gapfinder.ini'), encoding='utf-8')

    def g(sec, key, default=''):
        try:
            v = cp.get(sec, key).strip()
        except Exception:
            v = default
        if v and not os.path.isabs(v) and sec == 'paths':
            v = os.path.join(HERE, v)
        return v
    return {
        'client_exe': g('paths', 'client_exe', r'D:\Conquer Online 3.0\Env_DX9\Conquer.exe'),
        'client_ini': g('paths', 'client_ini', r'D:\Conquer Online 3.0\ini'),
        'client_logs': g('paths', 'client_logs', r'D:\Conquer Online 3.0\debug\*.log'),
        'server_source': g('paths', 'server_source', r'D:\conquer\MeGa6609\Source'),
        'account_source': g('paths', 'account_source', r'D:\conquer\MeGa6609\AccountServer'),
        'packet_audit': g('paths', 'packet_audit',
                          r'D:\conquer\MeGa6609\Source\bin\Debug\ServerDataBase\Modules\PacketAudit\logs\*.log'),
        'compat_report': g('paths', 'compat_report', ''),
        'html': os.path.join(HERE, g('output', 'html', 'gapfinder_report.html') or 'gapfinder_report.html'),
        'json': os.path.join(HERE, g('output', 'json', 'gapfinder_report.json') or 'gapfinder_report.json'),
        'use_capstone': g('options', 'use_capstone', '1') != '0',
        'min_id': int(g('options', 'min_id', '1000') or 1000),
        'max_id': int(g('options', 'max_id', '65000') or 65000),
    }


# ----------------------------------------------------------------------------------------------
# 1. client exe
# ----------------------------------------------------------------------------------------------
class PE:
    def __init__(self, path):
        self.d = open(path, 'rb').read()
        d = self.d
        self.secs = []  # (name, va, vsize, rawoff, rawsize)
        if pefile is not None:
            p = pefile.PE(data=d, fast_load=True)
            self.base = p.OPTIONAL_HEADER.ImageBase
            for s in p.sections:
                self.secs.append((s.Name.rstrip(b'\0').decode('latin1'), s.VirtualAddress, s.Misc_VirtualSize,
                                  s.PointerToRawData, s.SizeOfRawData))
        else:
            pe = struct.unpack_from('<I', d, 0x3c)[0]
            nsec = struct.unpack_from('<H', d, pe + 6)[0]
            optsz = struct.unpack_from('<H', d, pe + 20)[0]
            magic = struct.unpack_from('<H', d, pe + 24)[0]
            if magic != 0x10b:
                raise ValueError('only 32-bit PE files are supported')
            self.base = struct.unpack_from('<I', d, pe + 24 + 28)[0]
            off = pe + 24 + optsz
            for i in range(nsec):
                n, vs, va, rs, ro = struct.unpack_from('<8sIIII', d, off + i * 40)
                self.secs.append((n.rstrip(b'\0').decode('latin1'), va, vs, ro, rs))

    def sec(self, name):
        for s in self.secs:
            if s[0] == name:
                return s
        return None

    def o2v(self, o):
        for n, va, vs, ro, rs in self.secs:
            if ro <= o < ro + rs:
                return self.base + va + o - ro
        return None

    def v2o(self, v):
        r = v - self.base
        for n, va, vs, ro, rs in self.secs:
            if va <= r < va + min(vs, rs):
                return ro + r - va
        return None

    def in_text(self, v):
        s = self.sec('.text')
        return s is not None and self.base + s[1] <= v < self.base + s[1] + s[2]


RX_CHECK = re.compile(rb'[\xb8-\xbf]([\x00-\xff]{2})\x00\x00[\x00-\xff]{0,8}?\x66[\x3b\x39]', re.S)
RX_ANY = re.compile(rb'[\xb8-\xbf]([\x00-\xff]{2})\x00\x00[\x00-\xff]{0,10}?(?:\x66\x89[\x40-\x7f][\x02\x06]|\x66[\x3b\x39])',
                    re.S)


def capstone_check(pe, va, lo, hi):
    """Disassemble the Create(buf) function and look for mov reg, imm ; cmp reg16, [..]. Returns id or None."""
    o = pe.v2o(va)
    if o is None:
        return None
    md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
    insns = list(md.disasm(pe.d[o:o + 0xa0], va))[:45]
    for i, ins in enumerate(insns):
        if ins.mnemonic in ('ret', 'retn'):
            break
        if ins.mnemonic == 'mov':
            m = re.match(r'^(e[a-d]x|e[sd]i), (0x[0-9a-f]+|\d+)$', ins.op_str)
            if not m:
                continue
            reg, val = m.group(1), int(m.group(2), 0)
            if not (lo <= val <= hi):
                continue
            r16 = reg[1:]
            for j in insns[i + 1:i + 5]:
                if j.mnemonic == 'cmp' and 'word ptr' in j.op_str and r16 in re.split(r'[ ,\[\]+]', j.op_str):
                    return val
                if j.mnemonic == 'cmp' and re.search(r'\b%s\b' % r16, j.op_str):
                    return val
    return None


def analyse_exe(cfg):
    out = {'ok': False, 'messages': [], 'dialogs': [], 'notes': [], 'capstone': capstone is not None,
           'pefile': pefile is not None}
    path = cfg['client_exe']
    if not path or not os.path.isfile(path):
        out['notes'].append('client exe not found: %s' % path)
        return out
    pe = PE(path)
    d = pe.d
    lo_id, hi_id = cfg['min_id'], cfg['max_id']
    out['exe'] = path
    out['exe_size'] = len(d)

    # RTTI type descriptors: TypeDescriptor = {pVFTable, spare, name}
    names = {}
    for m in re.finditer(rb'\.\?AV(C(?:Msg|Dlg)[A-Za-z0-9_]*)@@\x00', d):
        v = pe.o2v(m.start())
        if v:
            names[v - 8] = m.group(1).decode('latin1')
    msg_names = sorted({n for n in names.values() if n.startswith('CMsg')})
    dlg_names = sorted({n for n in names.values() if n.startswith('CDlg')})
    out['dialogs'] = dlg_names
    log('client exe: %d CMsg classes, %d CDlg classes in RTTI' % (len(msg_names), len(dlg_names)))

    rd = pe.sec('.rdata')
    tx = pe.sec('.text')
    if rd is None or tx is None:
        out['notes'].append('no .rdata/.text section: name matching only')
        out['messages'] = [{'name': n, 'id': None, 'conf': 'none', 'method': 'name only'} for n in msg_names]
        return out
    rd0, rd1 = rd[3], rd[3] + rd[4]
    rd0 -= rd0 % 4
    dw = array.array('I')
    dw.frombytes(d[rd0:rd1 - (rd1 - rd0) % 4])
    tds = set(names)
    # CompleteObjectLocator (x86) = {sig 0, offset 0, cdOffset, pTypeDescriptor, pClassHierarchy}
    col2td = {}
    for i in range(3, len(dw)):
        t = dw[i]
        if t in tds and dw[i - 3] == 0 and dw[i - 2] == 0:
            col2td[pe.o2v(rd0 + (i - 3) * 4)] = t
    vtables = {}
    for i in range(len(dw)):
        if dw[i] in col2td:
            vtables.setdefault(col2td[dw[i]], []).append(pe.o2v(rd0 + i * 4) + 4)
    log('RTTI: %d object locators, %d vtables' % (len(col2td), sum(len(v) for v in vtables.values())))

    t0, t1 = tx[3], tx[3] + tx[4]

    def vt_funcs(vt, n=8):
        o = pe.v2o(vt)
        r = []
        for k in range(n):
            if o is None or o + 4 * k + 4 > len(d):
                break
            f = struct.unpack_from('<I', d, o + 4 * k)[0]
            if not pe.in_text(f):
                break
            r.append(f)
        return r

    cls = {}
    for td, n in names.items():
        if not n.startswith('CMsg'):
            continue
        vts = vtables.get(td, [])
        c = cls.setdefault(n, {'name': n, 'vtables': [], 'funcs': [], 'refs': []})
        for vt in vts:
            c['vtables'].append(vt)
            if not c['funcs']:
                c['funcs'] = vt_funcs(vt)
            pat = struct.pack('<I', vt)
            o = d.find(pat, t0, t1)
            while o != -1:
                c['refs'].append(pe.o2v(o))
                o = d.find(pat, o + 1, t1)
    # base-class functions are shared by many classes
    slot_count = collections.Counter()
    for c in cls.values():
        for k, f in enumerate(c['funcs'][:6]):
            slot_count[(k, f)] += 1
    base_process = {f for (k, f), cnt in slot_count.items() if k == 4 and cnt >= 5}
    use_cs = cfg['use_capstone'] and capstone is not None

    msgs = []
    agree = disagree = 0
    for n in msg_names:
        c = cls.get(n, {'funcs': [], 'refs': [], 'vtables': []})
        e = {'name': n, 'id': None, 'conf': 'none', 'method': '', 'alt': None,
             'vtable': ('0x%X' % c['vtables'][0]) if c['vtables'] else None, 'direction': '?'}
        is_pb = n.endswith('PB')
        if is_pb:
            e['kind'] = 'protobuf-body'
        else:
            e['kind'] = 'message'
        f = c['funcs']
        a_id = b_id = cs_id = None
        # A: Create(buf) override (vtable slot 1) compares the type of the received buffer
        if len(f) > 1 and slot_count[(1, f[1])] == 1:
            o = pe.v2o(f[1])
            m = RX_CHECK.search(d, o, o + 0x80) if o else None
            while m:
                v = struct.unpack('<H', m.group(1))[0]
                if lo_id <= v <= hi_id:
                    a_id = v
                    break
                m = RX_CHECK.search(d, m.end(), o + 0x80)
            if use_cs:
                try:
                    cs_id = capstone_check(pe, f[1], lo_id, hi_id)
                except Exception:
                    cs_id = None
        # B: first type write/check after the constructor/destructor (same .cpp file)
        if c['refs']:
            start = pe.v2o(max(c['refs']))
            m = RX_ANY.search(d, start, start + 0x400) if start else None
            while m:
                v = struct.unpack('<H', m.group(1))[0]
                if lo_id <= v <= hi_id:
                    b_id = v
                    break
                m = RX_ANY.search(d, m.end(), start + 0x400)
        if a_id is not None:
            e['id'] = a_id
            e['method'] = 'Create(buf) type compare'
            if cs_id == a_id:
                e['method'] += ' + capstone'
            if b_id is None or b_id == a_id:
                e['conf'] = 'high'
                if b_id == a_id:
                    agree += 1
            else:
                e['conf'] = 'medium'
                e['alt'] = b_id
                disagree += 1
        elif cs_id is not None:
            e['id'], e['conf'], e['method'] = cs_id, 'high', 'capstone'
        elif b_id is not None and not is_pb:
            e['id'], e['conf'], e['method'] = b_id, 'medium', 'type write after constructor'
        if len(f) > 4:
            e['direction'] = 'client sends only' if f[4] in base_process else 'client receives'
        elif f:
            e['direction'] = '?'
        msgs.append(e)

    # protobuf body classes take the id of their wrapper message
    by_name = {e['name']: e for e in msgs}
    for e in msgs:
        if e['kind'] == 'protobuf-body':
            w = by_name.get(e['name'][:-2])
            if w:
                e['wrapper'] = w['name']
                if w['id'] is not None and e['id'] is None:
                    e['id'], e['conf'], e['method'] = w['id'], w['conf'], 'id of wrapper ' + w['name']
                    e['direction'] = w['direction']
    # validation
    val = []
    for n, want in KNOWN.items():
        g = by_name.get(n)
        val.append({'name': n, 'expected': want, 'got': g['id'] if g else None,
                    'conf': g['conf'] if g else 'missing', 'ok': bool(g and g['id'] == want)})
    out['validation'] = val
    out['method_agreement'] = {'agree': agree, 'disagree': disagree}
    out['messages'] = msgs
    out['ok'] = True
    out['base_process_funcs'] = ['0x%X' % x for x in base_process]
    log('ids: %d/%d message classes have an id (%d high, %d medium); A/B agree %d, disagree %d; known pairs %d/%d'
        % (sum(1 for e in msgs if e['id'] is not None), len(msgs),
           sum(1 for e in msgs if e['conf'] == 'high'), sum(1 for e in msgs if e['conf'] == 'medium'),
           agree, disagree, sum(v['ok'] for v in val), len(val)))
    return out


# ----------------------------------------------------------------------------------------------
# 2. server source
# ----------------------------------------------------------------------------------------------
def strip_comments(t):
    t = re.sub(r'/\*.*?\*/', '', t, flags=re.S)
    return re.sub(r'(?m)//.*$', '', t)


def cs_files(root):
    for dp, dn, fn in os.walk(root):
        dn[:] = [x for x in dn if x.lower() not in ('bin', 'obj', '.vs', 'packages')]
        for f in fn:
            if f.endswith('.cs'):
                yield os.path.join(dp, f)


def num(s):
    s = s.strip()
    return int(s, 16) if s.lower().startswith('0x') else int(s)


def parse_enum(text, name):
    m = re.search(r'enum\s+%s\b[^{]*\{(.*?)\}' % re.escape(name), text, re.S)
    r = {}
    if not m:
        return r
    cur = -1
    for part in m.group(1).split(','):
        part = part.strip()
        if not part:
            continue
        mm = re.match(r'^(\w+)\s*(?:=\s*(.+))?$', part, re.S)
        if not mm:
            continue
        if mm.group(2):
            try:
                cur = num(re.sub(r'\(\w+\)', '', mm.group(2)))
            except Exception:
                continue
        else:
            cur += 1
        r[mm.group(1)] = cur
    return r


def analyse_server(cfg):
    out = {'ok': False, 'constants': {}, 'handlers': [], 'actions': {}, 'action_handlers': [], 'notes': []}
    root = cfg['server_source']
    if not root or not os.path.isdir(root):
        out['notes'].append('server source not found: %s' % root)
        return out
    texts = {}
    for p in cs_files(root):
        try:
            raw = open(p, 'rb').read().decode('utf-8-sig', 'replace')
        except Exception:
            continue
        texts[p] = strip_comments(raw)
    rel = lambda p: os.path.relpath(p, root)
    gp = os.path.join(root, 'Game', 'GamePackets.cs')
    consts = {}  # name -> id from GamePackets
    if gp in texts:
        for m in re.finditer(r'\b([A-Za-z_]\w*)\s*=\s*(0x[0-9A-Fa-f]+|\d+)\b', texts[gp]):
            consts[m.group(1)] = num(m.group(2))
    else:
        out['notes'].append('Game\\GamePackets.cs not found')
    # every other ushort/int const and the PacketTypes enum (for [Packet(Types.PacketTypes.X)] / local consts)
    other = collections.defaultdict(set)
    for p, t in texts.items():
        for m in re.finditer(r'\bconst\s+(?:ushort|short|int|uint|UInt16|Int32)\s+([^;]+);', t):
            for mm in re.finditer(r'\b([A-Za-z_]\w*)\s*=\s*(0x[0-9A-Fa-f]+|\d+)\b', m.group(1)):
                other[mm.group(1)].add(num(mm.group(2)))
        if 'enum PacketTypes' in t:
            for k, v in parse_enum(t, 'PacketTypes').items():
                other[k].add(v)
                consts.setdefault(k, v)
    handlers = []
    rx = re.compile(r'\[\s*(?:[\w.]*\.)?(?:PacketAttribute|Packet)\s*\(\s*([^\]]*?)\s*\)\s*\]')
    for p, t in texts.items():
        for m in rx.finditer(t):
            arg = re.sub(r'\(\s*(?:ushort|UInt16|short|int|uint)\s*\)', '', m.group(1)).strip()
            ident = arg.split('.')[-1].strip()
            pid = None
            if re.fullmatch(r'0x[0-9A-Fa-f]+|\d+', ident):
                pid = num(ident)
            elif arg.startswith('GamePackets.') or '.GamePackets.' in arg:
                pid = consts.get(ident)
            if pid is None:
                if ident in consts:
                    pid = consts[ident]
                elif len(other.get(ident, ())) == 1:
                    pid = next(iter(other[ident]))
            line = t.count('\n', 0, m.start()) + 1
            # method name after the attribute
            mm = re.search(r'\b(?:void|bool|int)\s+(\w+)\s*\(', t[m.end():m.end() + 400])
            handlers.append({'arg': arg, 'id': pid, 'file': rel(p), 'line': line,
                             'method': mm.group(1) if mm else ''})
            if pid is not None and ident not in consts and pid not in consts.values():
                consts.setdefault(ident, pid)
    # 10010 actions
    dp = os.path.join(root, 'Game', 'MsgServer', 'MsgDataPacket.cs')
    actions = parse_enum(texts.get(dp, ''), 'ActionType')
    ah = []
    rxa = re.compile(r'\[\s*(?:[\w.]*\.)?(?:DataAttribute|Data)\s*\(\s*(?:[\w.]*\.)?ActionType\.(\w+)\s*\)\s*\]')
    for p, t in texts.items():
        for m in rxa.finditer(t):
            ah.append({'name': m.group(1), 'id': actions.get(m.group(1)), 'file': rel(p)})
    # 1009 item usage sub types
    iu = os.path.join(root, 'Game', 'MsgServer', 'MsgItemUsuagePacket.cs')
    item_subs = parse_enum(texts.get(iu, ''), 'ItemUsuageID')
    item_cases = set(re.findall(r'case\s+ItemUsuageID\.(\w+)\s*:', texts.get(iu, '')))
    out.update(ok=True, constants=consts, handlers=handlers, actions=actions, action_handlers=ah,
               item_subs=item_subs, item_cases=sorted(item_cases), files=len(texts))
    log('server: %d .cs files, %d packet constants, %d packet handlers, %d ActionType values, %d action handlers'
        % (len(texts), len(consts), len(handlers), len(actions), len(ah)))
    return out


LOGIN_RX = re.compile(r'Login|Connect|EncryptCode|Proxy|SRP6|PicKey|Pickey|KeyLogin|Facebook|HostName|Legality|'
                      r'AccountSoftKb|ServerList|CrossSwitch|Disconnect|CheatingProgram|VerifyCheck|UserIPInfo')


def analyse_account(cfg):
    """Packet ids the AccountServer reads or writes (it replaced the official login with its own 1952 packet)."""
    root = cfg.get('account_source')
    ids = set()
    if not root or not os.path.isdir(root):
        return {'ok': False, 'ids': [], 'path': root}
    for p in cs_files(root):
        t = strip_comments(open(p, 'rb').read().decode('utf-8-sig', 'replace'))
        for m in re.finditer(r'_?p(?:acket)?ID\s*==\s*(?:\(\w+\)\s*)?(\d{3,5})\b', t, re.I):
            ids.add(int(m.group(1)))
        for m in re.finditer(r'WriteUInt16\(\s*(\d{3,5})\s*,\s*2\s*,', t):
            ids.add(int(m.group(1)))
    log('account server: packet ids %s' % sorted(ids))
    return {'ok': True, 'ids': sorted(ids), 'path': root}


def client_ids(client):
    r = collections.defaultdict(list)
    for e in client.get('messages', []):
        if e['id'] is not None and e['kind'] == 'message':
            r[str(e['id'])].append(e['name'])
    return r


def norm(n):
    n = re.sub(r'^C(?=Msg)', '', n)
    n = re.sub(r'^Msg', '', n)
    n = re.sub(r'PB$', '', n)
    return n.lower()


def match(client, server, account, runtime_ids):
    consts = server.get('constants', {})
    auth_ids = set(account.get('ids', []))
    by_id = collections.defaultdict(list)
    for k, v in consts.items():
        by_id[v].append(k)
    by_norm = {norm(k): k for k in consts}
    handled = {h['id'] for h in server.get('handlers', []) if h['id'] is not None}
    handler_of = collections.defaultdict(list)
    for h in server.get('handlers', []):
        if h['id'] is not None:
            handler_of[h['id']].append('%s (%s:%d)' % (h['method'], h['file'], h['line']))
    msgs = client.get('messages', [])
    client_by_id = collections.defaultdict(list)
    for e in msgs:
        if e['id'] is not None and e['kind'] == 'message':
            client_by_id[e['id']].append(e['name'])
    missing, both, nested = [], [], []
    name_confirm = name_conflict = 0
    for e in msgs:
        if e['kind'] == 'protobuf-body' and e.get('wrapper'):
            continue  # reported through the wrapper
        if e['kind'] == 'protobuf-body' and e['id'] is None:
            nested.append(e['name'])  # sub-message inside another protobuf message
            continue
        sid = by_id.get(e['id'], []) if e['id'] is not None else []
        sname = by_norm.get(norm(e['name']))
        row = dict(e)
        row['server_by_id'] = sid
        row['server_by_name'] = sname
        row['server_name_id'] = consts.get(sname) if sname else None
        row['category'] = 'login' if LOGIN_RX.search(e['name']) else ''
        row['auth_server'] = e['id'] in auth_ids
        row['seen_runtime'] = e['id'] in runtime_ids
        if sname and e['id'] is not None:
            if consts.get(sname) == e['id']:
                name_confirm += 1
                row['name_check'] = 'same id'
            elif not sid:
                name_conflict += 1
                row['name_check'] = 'server uses %s' % consts.get(sname)
            else:
                row['name_check'] = 'id known as %s' % ', '.join(sid)
        tid = e['id'] if e['id'] is not None else row['server_name_id']
        row['handled'] = tid in handled if tid is not None else False
        row['handlers'] = handler_of.get(tid, []) if tid is not None else []
        if not sid and not sname:
            missing.append(row)
        else:
            both.append(row)
    no_handler = []
    for k, v in sorted(consts.items(), key=lambda kv: kv[1]):
        if v not in handled:
            cl = client_by_id.get(v, [])
            dirs = sorted({x['direction'] for x in msgs if x['name'] in cl})
            seen = v in runtime_ids
            no_handler.append({'name': k, 'id': v, 'client': cl, 'client_direction': dirs, 'seen_runtime': seen,
                               'priority': 'high' if ('client sends only' in dirs or seen) else ('check' if not cl else 'low')})
    unknown_handlers = sorted({h['id'] for h in server.get('handlers', []) if h['id'] is not None}
                              - set(client_by_id))
    return {'missing': missing, 'both': both, 'no_handler': no_handler, 'protobuf_nested': nested,
            'name_confirm': name_confirm, 'name_conflict': name_conflict,
            'handled_ids_not_in_client': unknown_handlers, 'client_by_id': {str(k): v for k, v in client_by_id.items()}}


# ----------------------------------------------------------------------------------------------
# 3. PacketAudit logs
# ----------------------------------------------------------------------------------------------
def varint(b, i):
    r = s = 0
    while True:
        if i >= len(b) or s > 63:
            raise ValueError
        c = b[i]
        i += 1
        r |= (c & 0x7f) << s
        s += 7
        if not c & 0x80:
            return r, i


def pb_decode(b, depth=0):
    """Decode protobuf wire format. Returns list of fields or None when it does not look like protobuf."""
    if not b:
        return None
    i, fields = 0, []
    try:
        while i < len(b):
            key, i = varint(b, i)
            fn, wt = key >> 3, key & 7
            if fn < 1 or fn > 2000:
                return None
            if wt == 0:
                v, i = varint(b, i)
                fields.append({'f': fn, 't': 'varint', 'v': v})
            elif wt == 1:
                if i + 8 > len(b):
                    return None
                fields.append({'f': fn, 't': 'fixed64', 'v': struct.unpack_from('<Q', b, i)[0]})
                i += 8
            elif wt == 5:
                if i + 4 > len(b):
                    return None
                fields.append({'f': fn, 't': 'fixed32', 'v': struct.unpack_from('<I', b, i)[0]})
                i += 4
            elif wt == 2:
                ln, i = varint(b, i)
                if i + ln > len(b):
                    return None
                sub = b[i:i + ln]
                i += ln
                ent = {'f': fn, 't': 'bytes', 'v': sub.hex()}
                if sub and all(32 <= x < 127 for x in sub):
                    ent = {'f': fn, 't': 'string', 'v': sub.decode('ascii')}
                elif depth < 3:
                    nested = pb_decode(sub, depth + 1)
                    if nested:
                        ent = {'f': fn, 't': 'message', 'v': nested}
                fields.append(ent)
            else:
                return None
    except ValueError:
        return None
    return fields


def pb_text(fields):
    parts = []
    for f in fields:
        if f['t'] == 'message':
            parts.append('%d:{%s}' % (f['f'], pb_text(f['v'])))
        elif f['t'] == 'string':
            parts.append('%d:"%s"' % (f['f'], f['v']))
        else:
            parts.append('%d:%s' % (f['f'], f['v']))
    return ' '.join(parts)


def analyse_audit(cfg, server, client_by_id):
    handled = {h['id'] for h in server.get('handlers', []) if h['id'] is not None}
    act_handled = {h['id'] for h in server.get('action_handlers', []) if h['id'] is not None}
    item_cases = {server.get('item_subs', {}).get(n) for n in server.get('item_cases', [])}
    out = {'ok': False, 'keys': [], 'files': [], 'notes': []}
    files = sorted(glob.glob(cfg['packet_audit'])) if cfg['packet_audit'] else []
    if not files:
        out['notes'].append('no PacketAudit logs: %s' % cfg['packet_audit'])
        return out
    rx = re.compile(r'^(\d\d:\d\d:\d\d(?:\.\d+)?)\s+NEW unhandled (\S+) from (.+?) map (\d+), (\d+) bytes: (.*)$')
    rxt = re.compile(r'^(\d\d:\d\d:\d\d(?:\.\d+)?)\s+totals: (.*)$')
    keys = {}
    consts_by_id = collections.defaultdict(list)
    for k, v in server.get('constants', {}).items():
        consts_by_id[v].append(k)
    act_by_id = {v: k for k, v in server.get('actions', {}).items()}
    sub_by_id = {v: k for k, v in server.get('item_subs', {}).items()}
    for f in files:
        day = re.sub(r'\D', '', os.path.basename(f))[:8]
        out['files'].append(f)
        for line in open(f, 'rb').read().decode('utf-8', 'replace').splitlines():
            m = rx.match(line.strip())
            if m:
                ts, key, who, mp, size, hx = m.groups()
                stamp = (day + ' ' + ts) if day else ts
                k = keys.get(key)
                if not k:
                    k = keys[key] = {'key': key, 'count': 0, 'first': stamp, 'last': stamp, 'players': set(),
                                     'maps': set(), 'size': int(size), 'hex': hx.strip(), 'totals': 0}
                k['count'] += 1
                k['last'] = stamp
                k['players'].add(who)
                k['maps'].add(mp)
                continue
            m = rxt.match(line.strip())
            if m:
                for part in m.group(2).split(','):
                    mm = re.match(r'\s*(\S+) x(\d+)', part)
                    if mm and mm.group(1) in keys:
                        keys[mm.group(1)]['totals'] = max(keys[mm.group(1)]['totals'], int(mm.group(2)))
    rows = []
    for k in keys.values():
        key = k['key']
        base = key.split('/')[0]
        pid = int(re.match(r'\d+', base).group(0)) if re.match(r'\d+', base) else None
        sub = None
        mm = re.match(r'^(\d+):(\d+)$', base)
        what = ''
        if mm:
            pid, sub = int(mm.group(1)), int(mm.group(2))
            if pid == 10010:
                what = 'ActionType.' + act_by_id[sub] if sub in act_by_id else 'action %d (not in ActionType)' % sub
            elif pid == 1009:
                what = 'ItemUsuageID.' + sub_by_id[sub] if sub in sub_by_id else 'item usage %d (not in ItemUsuageID)' % sub
        truncated = k['hex'].endswith('...')
        b = bytes.fromhex(re.sub(r'[^0-9A-Fa-f]', '', k['hex'].replace('...', '')))
        pb = None
        body_len = None
        if len(b) >= 4:
            ln = struct.unpack_from('<H', b, 0)[0]
            body = b[4:ln] if ln <= len(b) else b[4:]
            body_len = ln - 4
            if ln <= len(b) and 0 < len(body) <= 4096:
                pb = pb_decode(body)
                if pb and len(body) > 40 and len(pb) < 2:
                    pb = None
        rows.append({
            'key': key, 'id': pid, 'sub': sub, 'what': what, 'count': max(k['count'], k['totals']),
            'sessions': k['count'], 'first': k['first'], 'last': k['last'], 'players': sorted(k['players']),
            'maps': sorted(k['maps']), 'size': k['size'], 'hex': k['hex'], 'truncated': truncated,
            'client': client_by_id.get(str(pid), []) if pid is not None else [],
            'note': '' if (pid is None or client_by_id.get(str(pid))) else 'no CMsg class with this id in Conquer.exe (sent by another DLL, e.g. the shield?)',
            'server': consts_by_id.get(pid, []) if pid is not None else [],
            'protobuf': pb_text(pb) if pb else '', 'body_len': body_len,
            'handled_now': (sub in act_handled) if pid == 10010 and sub is not None else
                           (sub in item_cases) if pid == 1009 and sub is not None else (pid in handled),
        })
    rows.sort(key=lambda r: (-r['count'], r['key']))
    out['keys'] = rows
    out['ok'] = True
    log('PacketAudit: %d log file(s), %d unhandled keys' % (len(files), len(rows)))
    return out


# ----------------------------------------------------------------------------------------------
# 4. client debug logs
# ----------------------------------------------------------------------------------------------
LOG_RULES = [
    ('msg', 'catch error in process msg', re.compile(r'catch error in process msg:(\d+)')),
    ('itemtype', 'Read Error in ItemType.dat', re.compile(r'Read Error in ItemType\.dat, itemtypeID=(-?\d+)')),
    ('map', 'GameMapInfo not found', re.compile(r'GameMapInfo \((-?\d+)\) not found')),
    ('ani', 'Not Found in [ani/...]', re.compile(r'\[([^\]]+)\] Not Found [iI]n \[([^\]]+)\]')),
    ('file', 'file not found', re.compile(r'file (\S+) not found')),
    ('check', 'CHECK / ASSERT / IF_NOT', re.compile(r'(CHECK|ASSERT|IF_NOT)\s*\((.*)\)\S*\s+in\s+(.*?)$')),
]
RX_STAMP = re.compile(r'\s*--\s*\w{3} (\w{3}) +(\d+) (\d\d:\d\d:\d\d) (\d{4})\s*$')


def analyse_client_logs(cfg, client_by_id, client_names):
    out = {'ok': False, 'groups': {}, 'other': [], 'files': [], 'notes': []}
    files = sorted(glob.glob(cfg['client_logs'])) if cfg['client_logs'] else []
    if not files:
        out['notes'].append('no client logs: %s' % cfg['client_logs'])
        return out
    groups = {r[0]: {} for r in LOG_RULES}
    other = {}
    ini_root = os.path.dirname(cfg['client_ini']) if cfg['client_ini'] else ''
    for f in files:
        out['files'].append(f)
        text = open(f, 'rb').read().decode('gbk', 'replace')
        for line in text.splitlines():
            line = line.replace('\u2605', '').replace('\ufffd', '').strip()
            if not line:
                continue
            ms = RX_STAMP.search(line)
            stamp = '%s %s %s %s' % (ms.group(4), ms.group(1), ms.group(2), ms.group(3)) if ms else ''
            body = RX_STAMP.sub('', line)
            hit = False
            for gid, title, rx in LOG_RULES:
                m = rx.search(body)
                if not m:
                    continue
                hit = True
                if gid == 'ani':
                    if body.startswith('Ani Index'):
                        break  # same event logged twice; count the AniIndexInfo line only
                    key = '%s in %s' % (m.group(1), m.group(2))
                elif gid == 'check':
                    src = m.group(3)
                    src = re.sub(r'^.*?cq2clientcn[^\\]*\\', '', src)
                    key = '%s(%s) @ %s' % (m.group(1), m.group(2), src)
                else:
                    key = m.group(1)
                g = groups[gid].setdefault(key, {'key': key, 'count': 0, 'first': stamp, 'last': stamp,
                                                  'sample': body[:300]})
                g['count'] += 1
                if stamp:
                    g['last'] = stamp
                break
            if not hit and re.search(r'error|failed|can not|cannot|not found', body, re.I):
                k = re.sub(r'\d+', 'N', body)[:200]
                g = other.setdefault(k, {'key': k, 'count': 0, 'first': stamp, 'last': stamp, 'sample': body[:300]})
                g['count'] += 1
                if stamp:
                    g['last'] = stamp
    # hints
    for k, g in groups['msg'].items():
        g['hint'] = ', '.join(client_by_id.get(k, [])) or 'no client class with this id'
    for k, g in groups['check'].items():
        m = re.search(r'network\\(msg\w+)\.cpp', k, re.I)
        if m:
            cand = [n for n in client_names if n.lower() == 'c' + m.group(1).lower()]
            g['hint'] = ', '.join(cand)
    for k, g in groups['file'].items():
        if ini_root:
            g['hint'] = 'exists in client folder' if os.path.exists(os.path.join(ini_root, k)) else 'missing in client folder'
    for k, g in groups['map'].items():
        g['hint'] = 'map id sent by the server that the client GameMap.dat does not have'
    out['groups'] = {gid: sorted(v.values(), key=lambda x: -x['count']) for gid, v in groups.items()}
    out['titles'] = {r[0]: r[1] for r in LOG_RULES}
    out['other'] = sorted(other.values(), key=lambda x: -x['count'])[:40]
    out['ok'] = True
    log('client logs: %d file(s), %s' % (len(files), ', '.join('%s %d' % (k, len(v)) for k, v in out['groups'].items())))
    return out


# ----------------------------------------------------------------------------------------------
# 5. dialogs, compat summary
# ----------------------------------------------------------------------------------------------
def dialog_groups(dlgs, missing):
    groups = collections.defaultdict(list)
    for n in dlgs:
        rest = n[4:]
        m = re.match(r'[A-Z0-9]+(?=[A-Z][a-z])|[A-Z][a-z0-9]*', rest)
        p = m.group(0) if m else (rest or '(none)')
        groups[p].append(n)
    miss_names = [(e['name'], norm(e['name'])) for e in missing]
    res = []
    for p, l in groups.items():
        rel = [x for x, nx in miss_names if nx.startswith(p.lower())] if len(p) >= 3 else []
        res.append({'prefix': p, 'count': len(l), 'dialogs': sorted(l), 'missing_messages': rel})
    res.sort(key=lambda x: (-x['count'], x['prefix']))
    return res


def compat_summary(cfg):
    p = cfg['compat_report']
    if not p or not os.path.isfile(p):
        return {'ok': False, 'path': p}
    try:
        j = json.load(open(p, encoding='utf-8'))
    except Exception as ex:
        return {'ok': False, 'path': p, 'error': str(ex)}
    cats = []
    for c in j.get('categories', []):
        cnt = collections.Counter(f.get('sev', '?') for f in c.get('findings', []))
        cats.append({'id': c.get('id'), 'title': c.get('title'), 'counts': dict(cnt), 'total': len(c.get('findings', []))})
    return {'ok': True, 'path': p, 'totals': j.get('totals', {}), 'categories': cats,
            'date': datetime.datetime.fromtimestamp(os.path.getmtime(p)).strftime('%Y-%m-%d %H:%M')}


# ----------------------------------------------------------------------------------------------
# HTML
# ----------------------------------------------------------------------------------------------
def esc(x):
    if isinstance(x, (list, tuple, set)):
        x = ', '.join(str(i) for i in x)
    return html.escape('' if x is None else str(x))


def table(cols, rows, ltr_cols=()):
    h = ['<div class="tw"><table class="sortable"><thead><tr>']
    for c in cols:
        h.append('<th>%s</th>' % esc(c[0]))
    h.append('</tr></thead><tbody>')
    for r in rows:
        h.append('<tr>')
        for i, c in enumerate(cols):
            v = c[1](r)
            cls = ' class="ltr"' if i in ltr_cols else ''
            h.append('<td%s>%s</td>' % (cls, v if isinstance(v, Raw) else esc(v)))
        h.append('</tr>')
    h.append('</tbody></table></div>')
    if not rows:
        return '<p class="muted">لا يوجد شيء هنا.</p>'
    return ''.join(h)


class Raw(str):
    pass


def badge(conf):
    return Raw('<span class="b %s">%s</span>' % (esc(conf), esc(conf)))


CSS = """
:root{--bg:#f6f7f9;--card:#fff;--fg:#1d2330;--mut:#667085;--line:#e3e6eb;--acc:#2563eb;--hi:#16a34a;--med:#d97706;--low:#dc2626;--code:#f1f3f6}
@media (prefers-color-scheme: dark){:root{--bg:#12151b;--card:#1b1f27;--fg:#e6e8ec;--mut:#9aa3b2;--line:#2b313c;--acc:#60a5fa;--code:#232833}}
*{box-sizing:border-box}body{margin:0;background:var(--bg);color:var(--fg);font:15px/1.6 "Segoe UI",Tahoma,Arial,sans-serif}
main{max-width:1200px;margin:0 auto;padding:16px}
h1{font-size:24px;margin:8px 0}h2{font-size:19px;margin:0 0 6px}
section{background:var(--card);border:1px solid var(--line);border-radius:10px;padding:14px 16px;margin:14px 0}
.muted,.ex{color:var(--mut)}.ex{margin:0 0 10px}
.tw{overflow-x:auto;max-height:620px;overflow-y:auto;border:1px solid var(--line);border-radius:6px}
table{border-collapse:collapse;width:100%;font-size:13.5px}
th,td{padding:5px 8px;border-bottom:1px solid var(--line);text-align:start;vertical-align:top}
th{position:sticky;top:0;background:var(--code);cursor:pointer;white-space:nowrap}
td.ltr{direction:ltr;text-align:left;font-family:Consolas,monospace;font-size:12.5px;word-break:break-all}
.cards{display:flex;flex-wrap:wrap;gap:10px}.card{flex:1 1 170px;border:1px solid var(--line);border-radius:8px;padding:10px;background:var(--code)}
.card b{display:block;font-size:24px}.b{padding:1px 7px;border-radius:9px;font-size:12px;color:#fff;background:var(--mut)}
.b.high{background:var(--hi)}.b.medium{background:var(--med)}.b.none,.b.low{background:var(--low)}.b.check{background:var(--acc)}
code{direction:ltr;unicode-bidi:embed;background:var(--code);padding:0 4px;border-radius:4px;font-family:Consolas,monospace}
nav a{margin-inline-end:12px;color:var(--acc);text-decoration:none}details summary{cursor:pointer;color:var(--acc)}
"""

JS = """
document.querySelectorAll('table.sortable th').forEach(function(th,i){th.addEventListener('click',function(){
var t=th.closest('table'),b=t.tBodies[0],r=[].slice.call(b.rows),d=th.dataset.d==='1'?-1:1;th.dataset.d=d===1?'1':'0';
r.sort(function(x,y){var a=x.cells[i].innerText,c=y.cells[i].innerText,na=parseFloat(a),nc=parseFloat(c);
if(!isNaN(na)&&!isNaN(nc))return (na-nc)*d;return a.localeCompare(c)*d});r.forEach(function(x){b.appendChild(x)})})});
"""


def build_html(R):
    ex, sv, mt, au, cl = R['client'], R['server'], R['match'], R['audit'], R['client_logs']
    P = []
    P.append('<!doctype html><html lang="ar" dir="rtl"><head><meta charset="utf-8">'
             '<meta name="viewport" content="width=device-width,initial-scale=1"><title>GapFinder 6609</title>'
             '<style>%s</style></head><body><main>' % CSS)
    P.append('<h1>GapFinder: الكلاينت الرسمي 6609 مقابل السيرفر</h1>')
    P.append('<p class="muted">اتعمل %s في %.1f ثانية. الأداة بتقرا بس (مش بتغيّر أي حاجة). capstone: %s، pefile: %s.</p>'
             % (esc(R['generated']), R['seconds'], 'موجود' if ex.get('capstone') else 'مش متسطّب (استخدمنا byte patterns)',
                'موجود' if ex.get('pefile') else 'مش متسطّب (parser داخلي)'))
    P.append('<nav><a href="#sum">الملخص</a><a href="#missing">ناقص في السيرفر</a><a href="#runtime">باكتات بدون رد</a>'
             '<a href="#nohandler">ثوابت بدون handler</a><a href="#both">المعروف للطرفين</a><a href="#logs">لوج الكلاينت</a>'
             '<a href="#dlg">الشبابيك</a><a href="#acc">دقة الأرقام</a><a href="#compat">compat</a></nav>')
    msgs = ex.get('messages', [])
    real = [e for e in msgs if e['kind'] == 'message']
    with_id = [e for e in real if e['id'] is not None]
    miss = mt.get('missing', [])
    miss_id = [e for e in miss if e['id'] is not None]
    nh_high = [x for x in mt.get('no_handler', []) if x['priority'] == 'high']
    P.append('<section id="sum"><h2>الملخص</h2><div class="cards">')
    for label, v in [('رسائل في الكلاينت (CMsg)', len(real)), ('ليها رقم مستخرج', len(with_id)),
                     ('رسائل الكلاينت اللي السيرفر مايعرفهاش', len(miss)), ('منهم برقم معروف', len(miss_id)),
                     ('باكتات وصلت السيرفر من غير رد', len(au.get('keys', []))),
                     ('الكلاينت بيبعتها والسيرفر مالوش handler', len(nh_high)),
                     ('ثوابت في السيرفر من غير handler', len(mt.get('no_handler', []))),
                     ('شبابيك (CDlg) في الكلاينت', len(ex.get('dialogs', [])))]:
        P.append('<div class="card"><b>%s</b>%s</div>' % (v, esc(label)))
    P.append('</div></section>')

    # missing
    P.append('<section id="missing"><h2>1) رسائل في الكلاينت والسيرفر مالوش أي ثابت ليها (%d)</h2>' % len(miss))
    P.append('<p class="ex">دي رسائل (packets) الكلاينت الرسمي بيعرفها، والسيرفر مافيهوش لا رقمها في <code>GamePackets.cs</code> '
             'ولا اسم شبهها. يعني الميزة دي في اللعبة الرسمية مش موجودة عندنا خالص. «الكلاينت بيبعت بس» معناها إن الكلاينت '
             'مش بيستقبلها، فلازم السيرفر يرد عليها لما اللاعب يدوس الزرار. «الكلاينت بيستقبل» معناها السيرفر هو اللي المفروض يبعتها.</p>')
    P.append('<p class="ex">الترتيب: اللي الكلاينت بعتها فعلاً للسيرفر الأول، بعدها «الكلاينت بيبعت بس»، بعدها الباقي. '
             'الرسائل اللي في عمود «الفئة» مكتوب عليها login تخص الدخول الرسمي، والسيرفر بتاعنا عامل دخول خاص بيه '
             '(باكت 1952 في AccountServer) فغالباً مش محتاجينها.</p>')
    rows = sorted(miss, key=lambda e: (not e.get('seen_runtime'), e['direction'] != 'client sends only',
                                       e.get('category') == 'login', e['id'] is None, e['id'] or 0))
    P.append(table([('الرقم', lambda e: e['id'] if e['id'] is not None else '?'), ('الكلاس', lambda e: e['name']),
                    ('الثقة', lambda e: badge(e['conf'])), ('الاتجاه', lambda e: dir_ar(e['direction'])),
                    ('النوع', lambda e: 'protobuf' if e['kind'] != 'message' or msg_has_pb(e, msgs) else 'binary'),
                    ('الفئة', lambda e: e.get('category') or '-'),
                    ('وصل السيرفر؟', lambda e: 'أيوه' if e.get('seen_runtime') else '-'),
                    ('AccountServer', lambda e: 'بيستخدمه' if e.get('auth_server') else '-')],
                   rows, ltr_cols=(1,)))
    nested = mt.get('protobuf_nested', [])
    if nested:
        P.append('<details><summary>كلاسات protobuf داخلية (جزء من رسالة تانية، مالهاش رقم) - %d</summary><p class="ltr"><code>%s</code></p></details>'
                 % (len(nested), esc(nested)))
    P.append('</section>')

    # runtime
    P.append('<section id="runtime"><h2>2) باكتات الكلاينت بعتها فعلاً والسيرفر ماردش عليها (%d)</h2>' % len(au.get('keys', [])))
    P.append('<p class="ex">من لوجات موديول PacketAudit في السيرفر: كل نوع باكت وصل ومالوش handler بيتسجل مرة. '
             'العدد = مرات ظهوره (كل تشغيل للسيرفر بيسجّله تاني). لو الباكت protobuf بنفك الحقول (رقم الحقل:القيمة).</p>')
    P.append(table([('المفتاح', lambda r: r['key']), ('العدد', lambda r: r['count']),
                    ('الكلاس في الكلاينت', lambda r: r['client'] or r['what'] or r.get('note') or '-'),
                    ('اسم في السيرفر', lambda r: r['server'] or '-'), ('أول مرة', lambda r: r['first']),
                    ('آخر مرة', lambda r: r['last']), ('الحجم', lambda r: r['size']),
                    ('protobuf', lambda r: r['protobuf'] or '-'),
                    ('ليه handler دلوقتي؟', lambda r: 'أيوه (اتضاف بعد اللوج)' if r['handled_now'] else 'لأ'),
                    ('hex', lambda r: r['hex'][:150])],
                   au.get('keys', []), ltr_cols=(0, 2, 3, 7, 9)))
    for n in au.get('notes', []):
        P.append('<p class="muted">%s</p>' % esc(n))
    P.append('</section>')

    # no handler
    nh = mt.get('no_handler', [])
    P.append('<section id="nohandler"><h2>3) ثوابت في GamePackets من غير handler (%d)</h2>' % len(nh))
    P.append('<p class="ex">رقم معرّف في السيرفر بس مفيش ميثود عليها <code>[Packet(...)]</code>. ده طبيعي لو السيرفر بس '
             'هو اللي بيبعت الرسالة دي. الأولوية «high» = الكلاينت بيبعت الرسالة دي ومش بيستقبلها، فأكيد محتاجة handler. '
             '«check» = مش لاقيين الرقم في الكلاينت (يمكن رقم قديم أو غلط) - اتأكد بنفسك.</p>')
    P.append(table([('الرقم', lambda x: x['id']), ('الاسم في السيرفر', lambda x: x['name']),
                    ('الكلاس في الكلاينت', lambda x: x['client'] or '-'),
                    ('الاتجاه', lambda x: ', '.join(dir_ar(d) for d in x['client_direction']) or '-'),
                    ('الأولوية', lambda x: badge(x['priority']))],
                   sorted(nh, key=lambda x: ({'high': 0, 'check': 1, 'low': 2}[x['priority']], x['id'])), ltr_cols=(1, 2)))
    P.append('</section>')

    # both
    both = mt.get('both', [])
    P.append('<section id="both"><h2>4) رسائل معروفة للطرفين (%d)</h2>' % len(both))
    P.append('<p class="ex">الرقم أو الاسم موجود في الناحيتين. عمود «مقارنة الاسم» بيبيّن لو السيرفر حاطط رقم مختلف للاسم '
             'نفسه (ممكن يكون غلط زي LeagueOpt اللي كان 26222 بدل 2622).</p>')
    P.append(table([('الرقم', lambda e: e['id'] if e['id'] is not None else '?'), ('الكلاس', lambda e: e['name']),
                    ('الثقة', lambda e: badge(e['conf'])), ('في السيرفر بالرقم', lambda e: e['server_by_id'] or '-'),
                    ('في السيرفر بالاسم', lambda e: e['server_by_name'] or '-'),
                    ('مقارنة الاسم', lambda e: e.get('name_check', '-')),
                    ('handler', lambda e: 'أيوه' if e['handled'] else 'لأ'), ('الاتجاه', lambda e: dir_ar(e['direction']))],
                   sorted(both, key=lambda e: (e['id'] or 0)), ltr_cols=(1, 3, 4, 5)))
    hn = mt.get('handled_ids_not_in_client', [])
    if hn:
        P.append('<p class="muted">handlers في السيرفر لأرقام مالهاش كلاس في الكلاينت (ممكن تبقى باكتات الحماية أو أرقام غلط): '
                 '<code>%s</code></p>' % esc(hn))
    P.append('</section>')

    # client logs
    P.append('<section id="logs"><h2>5) مشاكل في لوج الكلاينت (debug\\*.log)</h2>')
    P.append('<p class="ex">الكلاينت بيكتب في اللوج كل حاجة مش فاهمها. «catch error in process msg:N» يعني السيرفر بعت '
             'الرسالة N بشكل غلط. «Read Error in ItemType.dat» يعني السيرفر بعت أيتم الكلاينت مايعرفوش. '
             '«GameMapInfo not found» يعني خريطة مش موجودة عند الكلاينت.</p>')
    titles_ar = {'msg': 'أخطاء في معالجة رسائل السيرفر', 'itemtype': 'أيتمز مش موجودة في itemtype.dat',
                 'map': 'خرايط مش موجودة', 'ani': 'صور/أنيميشن ناقصة', 'file': 'ملفات ناقصة', 'check': 'CHECK / ASSERT'}
    for gid, rows in cl.get('groups', {}).items():
        P.append('<h3>%s (%d نوع، %d مرة)</h3>' % (esc(titles_ar.get(gid, gid)), len(rows), sum(r['count'] for r in rows)))
        P.append(table([('المفتاح', lambda r: r['key']), ('العدد', lambda r: r['count']),
                        ('ملاحظة', lambda r: r.get('hint', '')), ('أول مرة', lambda r: r['first']),
                        ('آخر مرة', lambda r: r['last'])], rows, ltr_cols=(0, 2)))
    if cl.get('other'):
        P.append('<details><summary>أخطاء تانية (%d)</summary>' % len(cl['other']))
        P.append(table([('السطر', lambda r: r['sample']), ('العدد', lambda r: r['count'])], cl['other'], ltr_cols=(0,)))
        P.append('</details>')
    for n in cl.get('notes', []):
        P.append('<p class="muted">%s</p>' % esc(n))
    P.append('</section>')

    # dialogs
    dg = R['dialog_groups']
    P.append('<section id="dlg"><h2>6) شبابيك الكلاينت (CDlg) متقسّمة حسب البداية (%d مجموعة)</h2>' % len(dg))
    P.append('<p class="ex">للمعلومة بس: كل شباك في الكلاينت = ميزة. لو مجموعة شبابيك ليها رسائل ناقصة في السيرفر '
             'هتلاقيها في العمود الأخير.</p>')
    P.append(table([('البداية', lambda g: g['prefix']), ('العدد', lambda g: g['count']),
                    ('رسائل ناقصة في السيرفر', lambda g: g['missing_messages'] or '-'),
                    ('الشبابيك', lambda g: ', '.join(g['dialogs']))], dg, ltr_cols=(0, 2, 3)))
    P.append('</section>')

    # accuracy
    P.append('<section id="acc"><h2>7) دقة استخراج الأرقام من Conquer.exe</h2>')
    ag = ex.get('method_agreement', {})
    P.append('<p class="ex">الطريقة: من RTTI بنلاقي الـ vtable بتاعة كل كلاس، وبعدين دالة <code>Create(buf)</code> '
             '(الخانة 1 في الـ vtable) بتقارن نوع الباكت برقم ثابت (<code>mov reg, imm ; cmp reg16, [..]</code>) - ده «high». '
             'لو الكلاس بيستخدم Create بتاعة الأب، بناخد أول رقم بيتكتب كنوع بعد الـ constructor - ده «medium». '
             'الطريقتين اتفقوا في %d كلاس واختلفوا في %d. وكمان %d اسم في السيرفر ليه نفس الرقم، و%d اسم السيرفر حاطله رقم مختلف.</p>'
             % (ag.get('agree', 0), ag.get('disagree', 0), mt.get('name_confirm', 0), mt.get('name_conflict', 0)))
    P.append(table([('الكلاس', lambda v: v['name']), ('المتوقع', lambda v: v['expected']), ('المستخرج', lambda v: v['got']),
                    ('الثقة', lambda v: badge(v['conf'])), ('صح؟', lambda v: 'صح' if v['ok'] else 'غلط')],
                   ex.get('validation', []), ltr_cols=(0,)))
    conflicts = [e for e in both if e.get('name_check', '').startswith('server uses')]
    if conflicts:
        P.append('<h3>أسماء السيرفر بيستخدم لها رقم غير اللي في الكلاينت (%d)</h3>' % len(conflicts))
        P.append(table([('الكلاس', lambda e: e['name']), ('رقم الكلاينت', lambda e: e['id']), ('الثقة', lambda e: badge(e['conf'])),
                        ('اسم السيرفر', lambda e: e['server_by_name']), ('رقم السيرفر', lambda e: e['server_name_id']),
                        ('بديل', lambda e: e.get('alt') or '-')], conflicts, ltr_cols=(0, 3)))
    P.append('<details><summary>كل رسائل الكلاينت (%d)</summary>' % len(msgs))
    P.append(table([('الكلاس', lambda e: e['name']), ('الرقم', lambda e: e['id'] if e['id'] is not None else '?'),
                    ('الثقة', lambda e: badge(e['conf'])), ('الطريقة', lambda e: e['method']),
                    ('بديل', lambda e: e.get('alt') or ''), ('الاتجاه', lambda e: dir_ar(e['direction'])),
                    ('vtable', lambda e: e.get('vtable') or '')], msgs, ltr_cols=(0, 3, 6)))
    P.append('</details></section>')

    # compat
    cp = R['compat']
    P.append('<section id="compat"><h2>8) ملخص تقرير compat_checker</h2>')
    if cp.get('ok'):
        P.append('<p class="ex">من <code>%s</code> (%s). التفاصيل في <code>_tools\\compat_checker\\CompatChecker.html</code>.</p>'
                 % (esc(cp['path']), esc(cp['date'])))
        P.append(table([('القسم', lambda c: c['title']), ('error', lambda c: c['counts'].get('error', 0)),
                        ('warn', lambda c: c['counts'].get('warn', 0)), ('info', lambda c: c['counts'].get('info', 0)),
                        ('ok', lambda c: c['counts'].get('ok', 0)), ('الكل', lambda c: c['total'])], cp['categories']))
    else:
        P.append('<p class="muted">التقرير مش موجود: %s</p>' % esc(cp.get('path')))
    P.append('</section>')
    notes = ex.get('notes', []) + sv.get('notes', [])
    if notes:
        P.append('<section><h2>ملاحظات</h2><ul>%s</ul></section>' % ''.join('<li>%s</li>' % esc(n) for n in notes))
    P.append('<script>%s</script></main></body></html>' % JS)
    return ''.join(P)


def dir_ar(d):
    return {'client sends only': 'الكلاينت بيبعت بس', 'client receives': 'الكلاينت بيستقبل', '?': '?'}.get(d, d)


def msg_has_pb(e, msgs):
    return any(x['name'] == e['name'] + 'PB' for x in msgs)


# ----------------------------------------------------------------------------------------------
def main():
    cfg = load_config()
    log('gapfinder start (capstone %s, pefile %s)' % ('yes' if capstone else 'no', 'yes' if pefile else 'no'))
    try:
        client = analyse_exe(cfg)
    except Exception as ex:
        client = {'ok': False, 'messages': [], 'dialogs': [], 'notes': ['exe analysis failed: %r' % ex]}
        log('exe analysis failed:', ex)
    server = analyse_server(cfg)
    account = analyse_account(cfg)
    cids = client_ids(client)
    audit = analyse_audit(cfg, server, cids)
    runtime_ids = {r['id'] for r in audit.get('keys', []) if r['id'] is not None and not r['handled_now']}
    m = match(client, server, account, runtime_ids)
    names = [e['name'] for e in client.get('messages', [])]
    clogs = analyse_client_logs(cfg, m['client_by_id'], names)
    dg = dialog_groups(client.get('dialogs', []), m['missing'])
    comp = compat_summary(cfg)
    R = {'generated': datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S'), 'config': cfg,
         'client': client, 'server': {k: v for k, v in server.items()}, 'match': m, 'audit': audit,
         'client_logs': clogs, 'dialog_groups': dg, 'compat': comp, 'account': account}
    R['seconds'] = time.time() - T0
    with open(cfg['json'], 'w', encoding='utf-8') as f:
        json.dump(R, f, ensure_ascii=False, indent=1, default=lambda o: sorted(o) if isinstance(o, set) else str(o))
    with open(cfg['html'], 'w', encoding='utf-8') as f:
        f.write(build_html(R))
    log('wrote %s' % cfg['html'])
    log('wrote %s' % cfg['json'])
    miss = m['missing']
    log('SUMMARY: %d client messages missing on the server (%d with id), %d runtime-unhandled keys, '
        '%d server constants without handler (%d where the client only sends)'
        % (len(miss), sum(1 for e in miss if e['id'] is not None), len(audit.get('keys', [])),
           len(m['no_handler']), sum(1 for x in m['no_handler'] if x['priority'] == 'high')))


if __name__ == '__main__':
    main()
