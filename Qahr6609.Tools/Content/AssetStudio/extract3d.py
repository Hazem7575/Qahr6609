#!/usr/bin/env python
"""AssetStudio stage 2 - export 3D models for the Dressing Room tab (out/3d/).

Reads the official client READ-ONLY (same client folder as extract.py, from assetstudio.ini):
    ini/armor.dbc  armet.dbc  weapon.dbc  mount.dbc   (item key -> parts {mesh id, texture id})
    ini/3DObj.dbc  3DTexture.dbc                      (id -> c3/... path, loose or inside c3.wdf)
    ini/mounttype.dat                                 (mount armor item -> mount.dbc key)
and writes
    out/3d/index.json + index.js      manifest (the viewer loads index.js: file:// pages cannot fetch JSON)
    out/3d/m/<name>.js                one mesh per C3 file: posed vertices (bind pose = first KKEY key, a T-pose),
                                      uvs, opaque + alpha triangles, attachment helpers (v_armet, v_l_weapon, ...)
    out/3d/t/<name>.js                one texture per DDS file (deduplicated): the DXT1/DXT3/DXT5 blocks of the first
                                      mip, textures larger than 256 are decoded, box-filtered down to 256 and re-encoded
    out/3d/cache.json                 source signatures -> re-runs only convert what changed

Mesh/texture files are JavaScript (A3D.m(...) / A3D.t(...)) holding zlib-compressed base64 data, because a page opened
from file:// can load <script> files but not fetch() binary files, and WebGL refuses file:// images.
Binary mesh layout (little endian, after zlib): int16 pos[n*3] and int16 uv[n*2], quantised with meta.q
(value = min + (int16 + 32768) * step; pad to 4 bytes), uint16|uint32 index[(o+a)*3] (o opaque triangles first, then
a alpha-blended triangles), and for character bodies (meta.s) uint8 {bone0, bone1, weight0*255}[n] in the bind pose.

usage:  python extract3d.py [--force] [--client DIR] [--out DIR] [--only armor,armet,weapon,mount]
"""
import argparse, array, base64, configparser, json, os, struct, sys, time, zlib

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
import c3model as c3  # noqa: E402
import dds  # noqa: E402
from wdf import WDF, string_id  # noqa: E402

try:
    sys.stdout.reconfigure(encoding='utf-8', errors='replace')
except Exception:
    pass

VERSION = 5          # bump when the output format changes (forces a full re-export)
MAXTEX = 256
DYETEX = 128     # dye colour variants (stage 3) are smaller
TEXVERSION = 1    # same for textures
BODIES = {1: (2001, 'f', 'Female small'), 2: (2002, 'f', 'Female large'),
          3: (1003, 'm', 'Male small'), 4: (1004, 'm', 'Male large')}
IDENT = (1.0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 1.0)
# headgear item families for which the client loads no mesh and keeps the hair (switch in Conquer.exe, see STAGE2_NOTES)
HAIR_HEAD = {117, 118, 123, 141, 142, 143, 145, 148, 177, 178}


# ---------------------------------------------------------------- client access
class Client:
    def __init__(self, root):
        self.root = root
        self.wdfs = []
        for n in ('c3.wdf', 'data.wdf'):
            p = os.path.join(root, n)
            if os.path.exists(p):
                self.wdfs.append((WDF(p), int(os.path.getmtime(p))))
        ini = lambda n: open(os.path.join(root, 'ini', n), 'rb').read()
        self.obj = c3.read_rsdb(ini('3DObj.dbc'))
        self.tex = c3.read_rsdb(ini('3DTexture.dbc'))
        self.dbc = {k: c3.read_mesh_dbc(ini(k + '.dbc')) for k in ('armor', 'armet', 'weapon', 'mount')}

    def locate(self, rel):
        """-> (signature, reader) or (None, None)"""
        p = os.path.join(self.root, rel.replace('/', os.sep))
        if os.path.isfile(p):
            st = os.stat(p)
            return 'L%d_%d' % (st.st_size, int(st.st_mtime)), lambda: open(p, 'rb').read()
        uid = string_id(rel)
        for w, mt in self.wdfs:
            e = w.index.get(uid)
            if e:
                return 'W%d_%d_%d' % (mt, e[0], e[1]), lambda w=w, uid=uid: w.read_uid(uid)
        return None, None


def short_name(path):
    s = path.lower().replace('\\', '/')
    if s.startswith('c3/'):
        s = s[3:]
    s = os.path.splitext(s)[0]
    return ''.join(ch if ch.isalnum() else '_' for ch in s)


# ---------------------------------------------------------------- mesh export
def matmul(a, b):
    """row-major 4x4 product a*b (row-vector convention: v * a * b)"""
    return [sum(a[r * 4 + k] * b[k * 4 + cc] for k in range(4)) for r in range(4) for cc in range(4)]


def is_helper(ph, nparts):
    n = ph['name'].lower()
    if nparts < 2 or not n.startswith('v_') or n.startswith('v_body'):
        return False
    if n.startswith('v_armet') and ph['nv'] + ph['av'] > 40:
        return False  # a real helmet part named v_armet01
    return ph['nv'] + ph['av'] <= 40


def pose_vertex(p, bones, ws, mats):
    acc, tw = [0.0, 0.0, 0.0], 0.0
    for bi, w in zip(bones, ws):
        if w > 0 and bi < len(mats):
            t = c3.xform(p, mats[bi])
            acc = [acc[k] + w * t[k] for k in range(3)]
            tw += w
    return tuple(acc) if tw > 0 else p


def build_mesh(buf, skinned=False):
    """skinned=True (character bodies: armor.dbc): vertices stay in the bind pose (v * initMatrix = T-pose) and the bone
    indices/weights are exported, the viewer poses them with the pose of the body (T-pose or standing).
    Otherwise the vertices are posed here with the first key of the file: v * initMatrix * bone (verified: this order
    gives the standing pose of the armor files; for most garments/weapons/hair the key is identity)."""
    phys, motis, _ = c3.parse_c3(buf)
    helpers, pos, uv, skin, opq, alp = {}, [], [], [], [], []
    for i, ph in enumerate(phys):
        moti = motis[i] if i < len(motis) else None
        if is_helper(ph, len(phys)):
            m = moti['key0'][0] if moti and moti.get('key0') else IDENT
            helpers[ph['name']] = [round(x, 4) for x in matmul(list(ph['init']), list(m))]
            continue
        if not ph['verts']:
            continue
        base = len(pos)
        mats = moti['key0'] if moti and moti.get('key0') else []
        for v in ph['verts']:
            p = c3.xform(v['pos'], ph['init'])
            if skinned:
                b0, b1 = v['bones']
                w0, w1 = v['w']
                w0, w1 = (w0 if w0 > 0 else 0.0), (w1 if w1 > 0 else 0.0)
                if w0 + w1 <= 0:
                    skin.append((255, 255, 255))
                else:
                    skin.append((min(b0, 254) if w0 > 0 else 255, min(b1, 254) if w1 > 0 else 255,
                                 int(round(255 * w0 / (w0 + w1)))))
            else:
                p = pose_vertex(p, v['bones'], v['w'], mats)
            pos.append(p)
        uv.extend(v['uv'] for v in ph['verts'])
        nv = len(ph['verts'])
        for k, t in enumerate(ph['tris']):
            if max(t) >= nv:
                continue
            (opq if k < ph['ntri'] else alp).append((t[0] + base, t[1] + base, t[2] + base))
    if not pos or not (opq or alp):
        return None, None
    n = len(pos)
    bmin = [min(p[k] for p in pos) for k in range(3)]
    bmax = [max(p[k] for p in pos) for k in range(3)]
    umin = [min(t[k] for t in uv) for k in range(2)]
    umax = [max(t[k] for t in uv) for k in range(2)]
    # 16-bit quantisation: value = min + (q + 32768) * step   (step = range / 65535)
    q = [round(x, 5) for x in bmin + umin]
    st = [((hi - lo) / 65535.0) or 1e-6 for lo, hi in zip(bmin + umin, bmax + umax)]
    st = [float('%.6g' % x) for x in st]
    qa = array.array('h', [max(-32768, min(32767, int(round((p[k] - q[k]) / st[k])) - 32768)) for p in pos for k in range(3)])
    qa.extend(max(-32768, min(32767, int(round((t[k] - q[3 + k]) / st[3 + k])) - 32768)) for t in uv for k in range(2))
    if len(qa) % 2:
        qa.append(0)   # keep the index array 4-byte aligned
    ia = array.array('H' if n < 65536 else 'I', [x for t in opq + alp for x in t])
    if sys.byteorder != 'little':
        qa.byteswap(); ia.byteswap()
    meta = {'n': n, 'o': len(opq), 'a': len(alp), 'i': ia.itemsize, 'b': [round(x, 2) for x in bmin + bmax], 'q': q + st}
    if helpers:
        meta['h'] = helpers
    data = qa.tobytes() + ia.tobytes()
    if skinned:
        meta['s'] = 1   # + uint8 {bone0, bone1, weight0*255}[n] (255 = no bone)
        data += bytes(x for t in skin for x in t)
    return meta, data


IDENT16 = list(IDENT)


def is_ident(m, eps=1e-3):
    return all(abs(m[i] - IDENT16[i]) < eps for i in range(16))


def body_poses(cl, armor_rows):
    """per body: T-pose helpers (from a mesh whose first key is identity) and, when one can be found, a standing pose:
    the bone matrices of the first key of the naked body / an armor whose first key is the standing frame
    (arms down: the posed width is less than half of the T-pose width, depth and height stay about the same)."""
    out = {}
    for b in (1, 2, 3, 4):
        keys = [k for k in sorted(armor_rows) if k // 1000000 == b]
        fam = lambda k: (k // 1000) % 1000
        order = [k for k in keys if k == b * 1000000] + [k for k in keys if 130 <= fam(k) <= 139 or fam(k) == 101] + \
                [k for k in keys if 181 <= fam(k) <= 194]
        seen, pose, ref = set(), {'T': None, 'stand': None}, None
        for k in order:
            path = cl.obj.get(armor_rows[k][0]['mesh'])
            if not path or path.lower() in seen:
                continue
            seen.add(path.lower())
            sig, rd = cl.locate(path)
            if not sig:
                continue
            try:
                phys, motis, _ = c3.parse_c3(rd())
            except Exception:
                continue
            bi = next((i for i, q in enumerate(phys) if q['name'].lower() == 'v_body' and i < len(motis) and motis[i].get('key0')), None)
            if bi is None:
                continue
            k0 = motis[bi]['key0']
            hel = {}
            for i, q in enumerate(phys):
                if is_helper(q, len(phys)) and i < len(motis) and motis[i].get('key0'):
                    hel[q['name']] = [round(x, 4) for x in matmul(list(q['init']), list(motis[i]['key0'][0]))]
            ph = phys[bi]
            bind = [c3.xform(v['pos'], ph['init']) for v in ph['verts']]
            if ref is None:
                ref = (len(k0), [max(p[j] for p in bind) - min(p[j] for p in bind) for j in range(3)])
            if len(k0) != ref[0]:
                continue
            if sum(not is_ident(m) for m in k0) <= 3:
                if not pose['T'] and all(h in hel for h in ('v_armet', 'v_r_weapon', 'v_l_weapon')):
                    pose['T'] = {'h': hel, 'from': path}
            elif not pose['stand']:
                posed = [pose_vertex(p, v['bones'], v['w'], k0) for p, v in zip(bind, ph['verts'])]
                ext = [max(p[j] for p in posed) - min(p[j] for p in posed) for j in range(3)]
                if ext[0] < 0.5 * ref[1][0] and ext[1] < 2.0 * ref[1][1] + 15 and abs(ext[2] - ref[1][2]) < 0.06 * ref[1][2]:
                    pose['stand'] = {'bones': [[round(x, 5) for x in m] for m in k0], 'h': hel, 'from': path}
            if pose['T'] and pose['stand']:
                break
        pose['_h'] = ref and ref[1][2]
        pose['_n'] = ref and ref[0]
        out[str(b)] = pose
    # a body without its own standing frame borrows the one of the other size of the same sex, scaled by the height
    # ratio (the bone matrices keep their rotation, translations are scaled)
    for b, o in (('4', '3'), ('3', '4'), ('2', '1'), ('1', '2')):
        P, Q = out[b], out[o]
        if not P['stand'] and Q['stand'] and P['_n'] == Q['_n'] and P['_h'] and Q['_h']:
            k = P['_h'] / Q['_h']
            sc = lambda m: [round(x * k, 5) if 12 <= i <= 14 else x for i, x in enumerate(m)]
            P['stand'] = {'bones': [sc(m) for m in Q['stand']['bones']], 'h': {n: sc(m) for n, m in Q['stand']['h'].items()},
                          'from': '%s (body %s, scaled x%.3f)' % (Q['stand']['from'], o, k)}
    for P in out.values():
        P.pop('_h', None)
        P.pop('_n', None)
    return out


# ---------------------------------------------------------------- texture export
def _q565(r, g, b):
    return ((r * 31 + 127) // 255) << 11 | ((g * 63 + 127) // 255) << 5 | ((b * 31 + 127) // 255)


def encode_dxt(w, h, px, alpha):
    """small bounding-box DXT1 / DXT3 encoder (only used for the few textures that are downscaled)"""
    out = bytearray()
    for by in range(0, h, 4):
        for bx in range(0, w, 4):
            blk = []
            for j in range(4):
                for i in range(4):
                    o = ((min(by + j, h - 1)) * w + min(bx + i, w - 1)) * 4
                    blk.append(px[o:o + 4])
            if alpha:
                a = 0
                for i, p in enumerate(blk):
                    a |= ((p[3] * 15 + 127) // 255) << (4 * i)
                out += struct.pack('<Q', a)
            rs, gs, bs = [p[0] for p in blk], [p[1] for p in blk], [p[2] for p in blk]
            c0 = _q565(max(rs), max(gs), max(bs))
            c1 = _q565(min(rs), min(gs), min(bs))
            if c0 < c1:
                c0, c1 = c1, c0
            if c0 == c1:
                out += struct.pack('<HHI', c0, c1, 0)
                continue
            e0, e1 = dds._expand565(c0), dds._expand565(c1)
            pal = [e0, e1, tuple((2 * e0[k] + e1[k]) // 3 for k in range(3)), tuple((e0[k] + 2 * e1[k]) // 3 for k in range(3))]
            bits = 0
            for i, p in enumerate(blk):
                best, bi = 1 << 30, 0
                for k, q in enumerate(pal):
                    d = (p[0] - q[0]) ** 2 + (p[1] - q[1]) ** 2 + (p[2] - q[2]) ** 2
                    if d < best:
                        best, bi = d, k
                bits |= bi << (2 * i)
            out += struct.pack('<HHI', c0, c1, bits)
    return bytes(out)


def half(w, h, px):
    nw, nh = max(1, w // 2), max(1, h // 2)
    out = bytearray(nw * nh * 4)
    for y in range(nh):
        r0, r1 = (2 * y) * w * 4, (2 * y + 1) * w * 4
        for x in range(nw):
            a, b = r0 + 8 * x, r1 + 8 * x
            o = (y * nw + x) * 4
            for k in range(4):
                out[o + k] = (px[a + k] + px[a + 4 + k] + px[b + k] + px[b + 4 + k] + 2) >> 2
    return nw, nh, bytes(out)


def build_texture(buf):
    if buf[:4] != b'DDS ':
        w, h, px = dds.decode_tga(buf)
        fmt = None
    else:
        h, w = struct.unpack_from('<II', buf, 12)
        pf_flags, fourcc = struct.unpack_from('<I4s', buf, 80)
        fmt = {b'DXT1': 'DXT1', b'DXT2': 'DXT3', b'DXT3': 'DXT3', b'DXT4': 'DXT5', b'DXT5': 'DXT5'}.get(fourcc) if pf_flags & 4 else None
        if fmt and max(w, h) <= MAXTEX:
            size = max(1, (w + 3) // 4) * max(1, (h + 3) // 4) * (8 if fmt == 'DXT1' else 16)
            return {'w': w, 'h': h, 'f': fmt}, buf[128:128 + size]
        w, h, px = dds.decode_dds(buf)
    while max(w, h) > MAXTEX:
        w, h, px = half(w, h, px)
    if w % 4 or h % 4:
        return {'w': w, 'h': h, 'f': 'RGBA'}, px
    alpha = fmt != 'DXT1' and any(a != 255 for a in px[3::4])
    return {'w': w, 'h': h, 'f': 'DXT3' if alpha else 'DXT1'}, encode_dxt(w, h, px, alpha)


def write_js(path, fn, name, meta, data):
    b64 = base64.b64encode(zlib.compress(data, 9)).decode('ascii')
    with open(path, 'w', encoding='ascii') as f:
        f.write('A3D.%s(%s,%s,"%s");\n' % (fn, json.dumps(name), json.dumps(meta, separators=(',', ':')), b64))


# ---------------------------------------------------------------- main
def load_mounttypes(client):
    try:
        import extract
        t = extract.read_client_text(os.path.join(client, 'ini', 'mounttype.dat')) or ''
        secs = extract.parse_ini(t)
    except Exception as e:  # noqa
        print('  mounttype.dat: %s' % e)
        return []
    out = []
    for s, kv in secs:
        if not s.isdigit() or not kv.get('armor', '').isdigit():
            continue
        sid = int(s)
        out.append([sid // 10000, sid % 10000, int(kv['armor']), (kv.get('Title') or '').replace('~', ' '),
                    int(kv.get('Offset', '0') or 0) if kv.get('Offset', '0').lstrip('-').isdigit() else 0])
    return out


def progress(i, n, label, t0):
    if i == n or i % 200 == 0:
        el = time.time() - t0
        eta = el / i * (n - i) if i else 0
        sys.stdout.write('\r  %-9s %5d/%d  %4.0fs  eta %3.0fs   ' % (label, i, n, el, eta))
        sys.stdout.flush()
        if i == n:
            sys.stdout.write('\n')


def main():
    ap = argparse.ArgumentParser(description='AssetStudio 3D exporter (Dressing Room)')
    ap.add_argument('--client')
    ap.add_argument('--out')
    ap.add_argument('--force', action='store_true', help='convert everything again')
    ap.add_argument('--only', help='comma list of tables: armor,armet,weapon,mount (default all)')
    ap.add_argument('--no-stage3', action='store_true', help='skip animations / NPC / monster models (stage 3)')
    ap.add_argument('--no-thumbs', action='store_true', help='skip the NPC / monster thumbnails (software renderer)')
    a = ap.parse_args()
    cp = configparser.ConfigParser()
    cp.read(os.path.join(HERE, 'assetstudio.ini'), encoding='utf-8')
    client = a.client or cp.get('paths', 'client', fallback=r'D:\Conquer Online 3.0')
    out = a.out or cp.get('paths', 'out', fallback='out')
    if not os.path.isabs(out):
        out = os.path.join(HERE, out)
    if not os.path.isfile(os.path.join(client, 'ini', 'armor.dbc')):
        print('ERROR: %s has no ini\\armor.dbc' % client)
        return 2
    d3 = os.path.join(out, '3d')
    for sub in ('m', 't'):
        os.makedirs(os.path.join(d3, sub), exist_ok=True)
    T0 = time.time()
    print('AssetStudio 3D exporter')
    print('  client : %s\n  output : %s' % (client, d3))
    cl = Client(client)
    tables = (a.only or 'armor,armet,weapon,mount').split(',')

    cache_p = os.path.join(d3, 'cache.json')
    cache = {'v': VERSION, 'tv': TEXVERSION, 'm': {}, 't': {}}
    if not a.force and os.path.exists(cache_p):
        try:
            c = json.load(open(cache_p, encoding='utf-8'))
            if c.get('v') == VERSION:
                cache['m'] = c.get('m', {})
            if c.get('tv') == TEXVERSION:
                cache['t'] = c.get('t', {})
        except Exception:
            pass

    # -- rows to export
    rows = {}
    for tb in ('armor', 'armet', 'weapon', 'mount'):
        if tb not in tables:
            continue
        r = cl.dbc[tb]
        if tb in ('armor', 'armet'):
            r = {k: v for k, v in r.items() if 1 <= k // 1000000 <= 4}
        rows[tb] = r
        print('  %-7s %6d rows' % (tb + '.dbc', len(r)))
    mpaths, tpaths, skinned = set(), set(), set()
    for tb, r in rows.items():
        for parts in r.values():
            for p in parts:
                if cl.obj.get(p['mesh']):
                    mpaths.add(cl.obj[p['mesh']])
                    if tb == 'armor':
                        skinned.add(cl.obj[p['mesh']].lower())
                if cl.tex.get(p['tex']):
                    tpaths.add(cl.tex[p['tex']])
    # dye colours (stage 3): an armor / headgear texture whose hundreds digit is 3 has the dyed looks at digits 4..9
    # (mixTex / mixOpt of the dbc tables are unused); the variants are exported at DYETEX px
    dye, dyepaths = {}, set()
    for tb in ('armor', 'armet'):
        for parts in rows.get(tb, {}).values():
            for p in parts:
                t = p['tex']
                if (t // 100) % 10 != 3 or not cl.tex.get(t) or t in dye:
                    continue
                vs = [cl.tex.get(t - 300 + c * 100) for c in range(3, 10)]
                if all(vs[1:]):
                    dye[t] = vs
                    for v in vs[1:]:
                        if v not in tpaths:
                            dyepaths.add(v.lower())
                        tpaths.add(v)

    # -- meshes
    print('[1/3] meshes (%d files)' % len(mpaths))
    mres, conv, reused, failed = {}, 0, 0, []
    t0 = time.time()
    for i, path in enumerate(sorted(mpaths), 1):
        key = path.lower()
        sig, reader = cl.locate(path)
        name = 'm_' + short_name(path)
        fp = os.path.join(d3, 'm', name + '.js')
        ce = cache['m'].get(key)
        if sig and ce and ce[0] == sig and (ce[2] is None or os.path.exists(fp)):
            reused += 1
            mres[key] = (name, ce[2]) if ce[2] else None
        elif not sig:
            mres[key] = None
            failed.append(path + ' (missing)')
        else:
            try:
                meta, data = build_mesh(reader(), key in skinned)
            except Exception as e:
                meta, data = None, None
                failed.append('%s (%s)' % (path, e))
            if meta:
                write_js(fp, 'm', name, meta, data)
                lite = {k: meta[k] for k in ('n', 'b')}
                if 'h' in meta:
                    lite['h'] = sorted(meta['h'])
                mres[key] = (name, lite)
            else:
                mres[key] = None
            cache['m'][key] = [sig, name, mres[key] and mres[key][1]]
            conv += 1
        progress(i, len(mpaths), 'meshes', t0)
    print('  converted %d, reused %d, unusable %d' % (conv, reused, sum(1 for v in mres.values() if not v)))

    # -- textures
    print('[2/3] textures (%d files)' % len(tpaths))
    tres, tconv, treused = {}, 0, 0
    t0 = time.time()
    for i, path in enumerate(sorted(tpaths), 1):
        key = path.lower()
        sig, reader = cl.locate(path)
        name = 't_' + short_name(path)
        fp = os.path.join(d3, 't', name + '.js')
        ce = cache['t'].get(key)
        if sig and ce and ce[0] == sig and (not ce[2] or os.path.exists(fp)):
            treused += 1
            tres[key] = name if ce[2] else None
        elif not sig:
            tres[key] = None
        else:
            try:
                if key in dyepaths:
                    global MAXTEX
                    keep, MAXTEX = MAXTEX, DYETEX
                    try:
                        meta, data = build_texture(reader())
                    finally:
                        MAXTEX = keep
                else:
                    meta, data = build_texture(reader())
                write_js(fp, 't', name, meta, data)
                tres[key] = name
            except Exception as e:
                failed.append('%s (%s)' % (path, e))
                tres[key] = None
            cache['t'][key] = [sig, name, bool(tres[key])]
            tconv += 1
        progress(i, len(tpaths), 'textures', t0)
    print('  converted %d, reused %d, unusable %d' % (tconv, treused, sum(1 for v in tres.values() if not v)))
    with open(cache_p, 'w', encoding='utf-8') as f:
        json.dump(cache, f, separators=(',', ':'))

    # -- manifest
    print('[3/3] manifest')
    mlist, mindex, tlist, tindex = [], {}, [], {}

    def mi(name):
        if name not in mindex:
            mindex[name] = len(mlist)
            mlist.append(name)
        return mindex[name]

    def ti(name):
        if name not in tindex:
            tindex[name] = len(tlist)
            tlist.append(name)
        return tindex[name]

    man = {'v': VERSION, 'generated': time.strftime('%Y-%m-%d %H:%M'), 'maxTex': MAXTEX,
           'bodies': {str(b): {'look': v[0], 'sex': v[1], 'name': v[2]} for b, v in BODIES.items()}}
    helpers_of = {}
    for tb, r in rows.items():
        o = {}
        for k in sorted(r):
            parts = []
            for p in r[k]:
                mp = cl.obj.get(p['mesh'])
                m = mres.get(mp.lower()) if mp else None
                if not m:
                    continue
                tp = cl.tex.get(p['tex'])
                t = tres.get(tp.lower()) if tp else None
                parts.append([mi(m[0]), ti(t) if t else -1])
                if m[1].get('h'):
                    helpers_of[m[0]] = m[1]['h']
            if parts:
                o[str(k)] = parts
        man[tb] = o
    if 'armor' in rows:
        print('  body poses ...')
        man['poses'] = body_poses(cl, rows['armor'])
        for b, p in sorted(man['poses'].items()):
            print('    body %s: T-pose helpers from %s, standing pose from %s' % (
                b, p['T'] and p['T']['from'], p['stand'] and p['stand']['from']))
    # dye table: texture index -> texture indices for the colours 3..9 (-1 = not exported)
    dmap = {}
    for t, vs in dye.items():
        names = [tres.get(v.lower()) if v else None for v in vs]
        if names[0] and sum(1 for n in names if n) >= 4:
            dmap[str(ti(names[0]))] = [ti(n) if n else -1 for n in names]
    man['dye'] = dmap
    man['meshes'] = mlist
    man['textures'] = tlist
    man['mounts'] = [m for m in load_mounttypes(client) if str(m[2]) in man.get('mount', {})]
    # body defaults: naked body + first hair of each body
    armet = man.get('armet', {})
    for b in BODIES:
        bd = man['bodies'][str(b)]
        bd['naked'] = str(b * 1000000) if str(b * 1000000) in man.get('armor', {}) else None
        bd['hair'] = sorted(int(k) % 1000 for k in armet if int(k) // 1000 == b * 1000 + 119)
        nk = bd['naked'] and man['armor'][bd['naked']][0][0]
        bd['attach'] = helpers_of.get(mlist[nk], []) if nk is not None else []

    # coverage of the stage-1 item list
    cov = {}
    dj = os.path.join(out, 'data.json')
    if os.path.exists(dj):
        items = json.load(open(dj, encoding='utf-8')).get('items', [])
        arm, wpn = man.get('armor', {}), man.get('weapon', {})
        mnt_items = {m[0] for m in man['mounts']}
        for it in items:
            iid, cat = it['id'], it['cat']
            base = iid - iid % 10
            if cat in ('garment', 'armor', 'headgear'):
                tb = armet if cat == 'headgear' else arm
                for b in BODIES:
                    # headgear families the client draws as the hair (Conquer.exe armet look switch, stage 3)
                    if cat == 'headgear' and (base // 1000) % 1000 in HAIR_HEAD or str(b * 1000000 + base) in tb:
                        cov.setdefault(cat, {}).setdefault(str(b), 0)
                        cov[cat][str(b)] += 1
                cov.setdefault(cat, {}).setdefault('items', 0)
                cov[cat]['items'] += 1
            elif cat == 'weapon':
                cov.setdefault('weapon', {'items': 0, 'all': 0})
                cov['weapon']['items'] += 1
                if str(iid) in wpn or str(base) in wpn:
                    cov['weapon']['all'] += 1
            elif cat == 'mount':
                cov.setdefault('mount', {'items': 0, 'all': 0})
                cov['mount']['items'] += 1
                if iid in mnt_items:
                    cov['mount']['all'] += 1
    man['coverage'] = cov
    js = json.dumps(man, separators=(',', ':'))
    with open(os.path.join(d3, 'index.json'), 'w', encoding='utf-8') as f:
        f.write(js)
    with open(os.path.join(d3, 'index.js'), 'w', encoding='utf-8') as f:
        f.write('window.ASSET3D_INDEX=' + js + ';\n')
    with open(os.path.join(d3, 'failed.txt'), 'w', encoding='utf-8') as f:
        f.write('# 3D files that could not be exported\n' + '\n'.join(failed) + '\n')

    # stage 3: animations (a/, anim.js), NPC / monster / skinned mount meshes (n/, npcs.js), thumbnails (th/)
    if not a.no_stage3 and 'mount' in rows:
        print('[stage 3] animations, NPC and monster models')
        import anim3d
        r3 = anim3d.export_stage3(sys.modules[__name__], cl, d3, man, client, force=a.force, thumbs=not a.no_thumbs)
        failed.extend(r3['failed'])
        with open(os.path.join(d3, 'failed.txt'), 'w', encoding='utf-8') as f:
            f.write('# 3D files that could not be exported\n' + '\n'.join(failed) + '\n')

    # the viewer pages (same as extract.py does) so running this script alone also updates the page
    import shutil
    if os.path.exists(os.path.join(out, 'data.js')):
        shutil.copyfile(os.path.join(HERE, 'viewer.html'), os.path.join(out, 'index.html'))
    shutil.copyfile(os.path.join(HERE, 'dresser.js'), os.path.join(out, 'dresser.js'))

    total = 0
    for root, _, files in os.walk(d3):
        total += sum(os.path.getsize(os.path.join(root, x)) for x in files)
    print('  rows: ' + ', '.join('%s %d' % (k, len(man.get(k, {}))) for k in ('armor', 'armet', 'weapon', 'mount')))
    print('  mounts listed: %d   meshes %d   textures %d   problems %d (failed.txt)' % (
        len(man['mounts']), len(mlist), len(tlist), len(failed)))
    for k, v in cov.items():
        print('  coverage %-9s %s' % (k, v))
    print('  size of out\\3d: %.1f MB   time %.0fs' % (total / 1048576.0, time.time() - T0))
    return 0


if __name__ == '__main__':
    sys.exit(main())
