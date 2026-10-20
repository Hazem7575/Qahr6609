"""AssetStudio stage 3 - animations, NPC and monster models, software thumbnails (used by extract3d.py).

Motion format (verified by rendering, see STAGE2_NOTES.md "Stage 3"):
    A C3 file holds one MOTI chunk per mesh part, in the same order as the PHY chunks of the mesh it animates
    (the engine pairs them by index; player motion files hold 3 one-bone helper motions + the body motion, in the
    order of the naked body mesh, e.g. v_armet, v_l_weapon, v_r_weapon, v_body for 1003).
    MOTI = uint32 boneCount, uint32 frameCount, then either
      'KKEY' uint32 keyCount, key{uint32 frame; float m[bones][16]}
      'XKEY' uint32 keyCount, key{uint16 frame; float m[bones][12]}      (4x3: rows 0-2 rotation, row 3 translation)
      'ZKEY' uint32 keyCount, key{uint16 frame; {float qx,qy,qz,qw, tx,ty,tz}[bones]}
      (old, no tag) float m[bones][frames][16]  (bone-major, one matrix per frame, monsters / NPCs)
    Every key is the final SKINNING matrix of the bone: v_out = sum w_i * (v * initMatrix * M_i).  ZKEY:
    M = D3DXMatrixRotationQuaternion(q) with the translation in row 3.  No hierarchy, no inverse bind.
    Player ZKEY motions live in a space turned 90 degrees around z (and offset by the hip height); the viewer
    multiplies by G^-1 per body (G = K1^-1 * Z1, from the naked body's own first key and the stand motion) so the
    animated body lands where the static one stood.

Output (all new, next to the stage-2 files):
    out/3d/a/<name>.js   A3D.a(name, meta, b64(zlib(...)))   one animation per motion file
        meta = {f: frames, p: [[type, bones, keys, tmin x3, tstep x3, rotScale], ...]}  (one entry per MOTI)
        data per part: uint16 keyFrame[keys] (padded to an even count), then int16 tracks, bone-major:
          for each bone, for each component, the values of all keys DELTA-coded (value[k] = sum of d[0..k], int16
          wrap-around); components: type 'q': qx,qy,qz,qw (* meta.qs) + t, type 'm': 9 rotation values
          (/ meta.qs * rotScale) + t, type 's' (matrix keys that are rotation * uniform scale): q + t +
          scale*1000; t is quantised: tmin + (v + 32768) * tstep
    out/3d/n/<name>.js   A3D.m(name, meta, ...)  NPC / monster meshes: all parts in one buffer, skinned with
        global bone = partBase[part] + bone (meta.s = 2: uint16 bone0, bone1, weight0*65535 per vertex)
    out/3d/th/<name>.png  textured software-rendered thumbnail of an NPC / monster model
    out/3d/anim.js        window.ASSET3D_ANIM = {...}  motion tables (players, mounts) - loaded on demand
    out/3d/npcs.js        window.ASSET3D_NPCS = {...}  NPC / monster lists - loaded on demand
"""
import array, base64, json, math, os, struct, sys, time, zlib

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
import c3model as c3  # noqa: E402
import dds  # noqa: E402

ANIMVERSION = 2
QS = 4095            # quaternion / rotation quantisation (13 bits; translations use 8000 steps per range)
NPCVERSION = 1
THUMBVERSION = 1


# ---------------------------------------------------------------- C3 chunks
def chunks(buf):
    o = 16
    while o + 8 <= len(buf):
        tag, size = struct.unpack_from('<4sI', buf, o)
        yield tag, buf[o + 8:o + 8 + size]
        o += 8 + size


def parse_moti(b):
    """-> {'t': 'q'|'m', 'bones', 'frames', 'keys': [(frame, values)]} ; values: list per bone of
    (qx,qy,qz,qw,tx,ty,tz) for 'q', or 16-float row-major matrices for 'm'"""
    bones, frames = struct.unpack_from('<II', b, 0)
    kt = b[8:12]
    keys = []
    if kt in (b'KKEY', b'XKEY', b'ZKEY'):
        kc = struct.unpack_from('<I', b, 12)[0]
        q = 16
        for _ in range(kc):
            if kt == b'KKEY':
                fr = struct.unpack_from('<I', b, q)[0]; q += 4
                ms = [struct.unpack_from('<16f', b, q + 64 * j) for j in range(bones)]; q += 64 * bones
                keys.append((fr, ms))
            elif kt == b'XKEY':
                fr = struct.unpack_from('<H', b, q)[0]; q += 2
                ms = []
                for j in range(bones):
                    m = struct.unpack_from('<12f', b, q + 48 * j)
                    ms.append((m[0], m[1], m[2], 0.0, m[3], m[4], m[5], 0.0, m[6], m[7], m[8], 0.0, m[9], m[10], m[11], 1.0))
                q += 48 * bones
                keys.append((fr, ms))
            else:
                fr = struct.unpack_from('<H', b, q)[0]; q += 2
                keys.append((fr, [struct.unpack_from('<7f', b, q + 28 * j) for j in range(bones)])); q += 28 * bones
        return {'t': 'q' if kt == b'ZKEY' else 'm', 'bones': bones, 'frames': frames, 'keys': keys}
    if len(b) >= 8 + 64 * bones * frames and bones and frames:
        M = [struct.unpack_from('<16f', b, 8 + 64 * i) for i in range(bones * frames)]
        keys = [(f, [M[j * frames + f] for j in range(bones)]) for f in range(frames)]
        return {'t': 'm', 'bones': bones, 'frames': frames, 'keys': keys}
    return None


def motis_of(buf):
    out = []
    for tag, d in chunks(buf):
        if tag == b'MOTI':
            try:
                out.append(parse_moti(d))
            except struct.error:
                out.append(None)
    return out


def qmat(q, t=(0.0, 0.0, 0.0)):
    """D3DXMatrixRotationQuaternion (row-vector), translation in row 3"""
    x, y, z, w = q
    return [1 - 2 * (y * y + z * z), 2 * (x * y + z * w), 2 * (x * z - y * w), 0.0,
            2 * (x * y - z * w), 1 - 2 * (x * x + z * z), 2 * (y * z + x * w), 0.0,
            2 * (x * z + y * w), 2 * (y * z - x * w), 1 - 2 * (x * x + y * y), 0.0, t[0], t[1], t[2], 1.0]


def matmul(a, b):
    return [sum(a[r * 4 + k] * b[k * 4 + c] for k in range(4)) for r in range(4) for c in range(4)]


def matinv(m):
    a = [list(m[i * 4:i * 4 + 4]) + [1.0 if i == j else 0.0 for j in range(4)] for i in range(4)]
    for c in range(4):
        p = max(range(c, 4), key=lambda r: abs(a[r][c]))
        if abs(a[p][c]) < 1e-12:
            return None
        a[c], a[p] = a[p], a[c]
        pv = a[c][c]
        a[c] = [x / pv for x in a[c]]
        for r in range(4):
            if r != c:
                f = a[r][c]
                a[r] = [x - f * y for x, y in zip(a[r], a[c])]
    return [a[i][4 + j] for i in range(4) for j in range(4)]


def mat_to_qs(m, tol=2e-3):
    """row-vector matrix (16) -> (qx, qy, qz, qw, scale) when its 3x3 part is a rotation times a uniform scale
    (the inverse of qmat); None otherwise"""
    r = [m[0], m[1], m[2], m[4], m[5], m[6], m[8], m[9], m[10]]
    s = [math.sqrt(r[0] ** 2 + r[1] ** 2 + r[2] ** 2), math.sqrt(r[3] ** 2 + r[4] ** 2 + r[5] ** 2),
         math.sqrt(r[6] ** 2 + r[7] ** 2 + r[8] ** 2)]
    sc = sum(s) / 3
    if sc < 1e-6 or max(abs(x - sc) for x in s) > tol * max(1.0, sc):
        return None
    a = [x / sc for x in r]
    # orthogonality and no mirror
    if abs(a[0] * a[3] + a[1] * a[4] + a[2] * a[5]) > tol or abs(a[0] * a[6] + a[1] * a[7] + a[2] * a[8]) > tol:
        return None
    det = a[0] * (a[4] * a[8] - a[5] * a[7]) - a[1] * (a[3] * a[8] - a[5] * a[6]) + a[2] * (a[3] * a[7] - a[4] * a[6])
    if det < 0:
        return None
    m00, m01, m02, m10, m11, m12, m20, m21, m22 = a
    tr = m00 + m11 + m22
    if tr > 0:
        w = math.sqrt(1 + tr) / 2
        x, y, z = (m12 - m21) / (4 * w), (m20 - m02) / (4 * w), (m01 - m10) / (4 * w)
    elif m00 > m11 and m00 > m22:
        x = math.sqrt(1 + m00 - m11 - m22) / 2
        w, y, z = (m12 - m21) / (4 * x), (m01 + m10) / (4 * x), (m02 + m20) / (4 * x)
    elif m11 > m22:
        y = math.sqrt(1 + m11 - m00 - m22) / 2
        w, x, z = (m20 - m02) / (4 * y), (m01 + m10) / (4 * y), (m12 + m21) / (4 * y)
    else:
        z = math.sqrt(1 + m22 - m00 - m11) / 2
        w, x, y = (m01 - m10) / (4 * z), (m02 + m20) / (4 * z), (m12 + m21) / (4 * z)
    q = (x, y, z, w)
    back = qmat(q)
    if max(abs(back[i] - a[i2]) for i, i2 in zip((0, 1, 2, 4, 5, 6, 8, 9, 10), range(9))) > 5 * tol:
        return None
    return x, y, z, w, sc


def key_matrices(mo, ki=0):
    if not mo or not mo['keys']:
        return None
    ks = mo['keys'][min(ki, len(mo['keys']) - 1)][1]
    if mo['t'] == 'q':
        return [qmat(z[:4], z[4:7]) for z in ks]
    return [list(m) for m in ks]


# ---------------------------------------------------------------- animation export
def build_anim(buf):
    """motion file -> (meta, bytes) ; None when it has no usable MOTI"""
    ms = motis_of(buf)
    if not ms or not any(m and m['keys'] for m in ms):
        return None, None
    parts, data = [], bytearray()
    prevq = {}
    frames = 1
    for m in ms:
        if not m or not m['keys']:
            parts.append(['-', 0, 0, 0, 0, 0, 1, 1, 1, 1])
            continue
        frames = max(frames, m['frames'], m['keys'][-1][0] + 1)
        nb, nk = m['bones'], len(m['keys'])
        fin = lambda x: x if x == x and abs(x) < 1e7 else 0.0     # some monster files hold NaN keys
        m['keys'] = [(f, [tuple(fin(x) for x in z) for z in ks]) for f, ks in m['keys']]
        if m['t'] == 'm':
            conv = []
            for f, ks in m['keys']:
                row = []
                for z in ks:
                    qs = mat_to_qs(z)
                    if qs is None:
                        break
                    row.append(qs[:4] + (z[12], z[13], z[14], qs[4]))
                if len(row) != len(ks):
                    conv = None
                    break
                conv.append((f, row))
            if conv:
                m = dict(m, t='s', keys=conv)   # rotation + translation + uniform scale (8 components)
        if m['t'] in ('q', 's'):
            ts = [z[4:7] for _, ks in m['keys'] for z in ks]
            rs = 1.0
        else:
            ts = [(mm[12], mm[13], mm[14]) for _, ks in m['keys'] for mm in ks]
            rs = max(1e-6, max(abs(mm[i]) for _, ks in m['keys'] for mm in ks for i in (0, 1, 2, 4, 5, 6, 8, 9, 10)))
            rs = float('%.6g' % (rs * 1.0001))
        tmin = [min(t[k] for t in ts) for k in range(3)]
        tmax = [max(t[k] for t in ts) for k in range(3)]
        tmin = [float('%.6g' % (x - abs(x) * 1e-5 - 1e-5)) for x in tmin]
        st = [float('%.6g' % max((hi - lo) / 8000.0, 1e-6)) for lo, hi in zip(tmin, tmax)]
        fr = array.array('H', [min(65535, k[0]) for k in m['keys']])
        if len(fr) % 2:
            fr.append(0)
        qt = lambda v, k: max(-32768, min(32767, int(round((v - tmin[k]) / st[k])) - 32768))
        qr = lambda v: max(-QS, min(QS, int(round(v * QS))))
        nc = {'q': 7, 's': 8}.get(m['t'], 12)
        tracks = [[[0] * nk for _ in range(nc)] for _ in range(nb)]
        for ki, (_, ks) in enumerate(m['keys']):
            for j, z in enumerate(ks):
                if m['t'] in ('q', 's'):
                    n = math.sqrt(sum(c * c for c in z[:4])) or 1.0
                    q = [c / n for c in z[:4]]
                    if ki and sum(a * b for a, b in zip(q, prevq[j])) < 0:
                        q = [-c for c in q]          # keep neighbouring keys in the same hemisphere
                    prevq[j] = q
                    v = [qr(c) for c in q] + [qt(z[4 + k], k) for k in range(3)]
                    if m['t'] == 's':
                        v.append(max(-32767, min(32767, int(round(z[7] * 1000)))))   # scale * 1000
                else:
                    v = [qr(z[i] / rs) for i in (0, 1, 2, 4, 5, 6, 8, 9, 10)] + [qt(z[12 + k], k) for k in range(3)]
                for c in range(nc):
                    tracks[j][c][ki] = v[c]
        vals = array.array('h')
        for j in range(nb):              # bone-major, one track per component, delta along the keys (static
            for c in range(nc):          # bones become runs of zeros that zlib removes)
                prev = 0
                for x in tracks[j][c]:
                    d = x - prev
                    vals.append(((d + 32768) & 0xFFFF) - 32768)
                    prev = x
        if sys.byteorder != 'little':
            fr.byteswap(); vals.byteswap()
        data += fr.tobytes() + vals.tobytes()
        parts.append([m['t'], nb, nk] + tmin + st + [rs])
    return {'f': frames, 'qs': QS, 'p': parts}, bytes(data)


def write_js(path, fn, name, meta, data):
    b64 = base64.b64encode(zlib.compress(data, 9)).decode('ascii')
    with open(path, 'w', encoding='ascii') as f:
        f.write('A3D.%s(%s,%s,"%s");\n' % (fn, json.dumps(name), json.dumps(meta, separators=(',', ':')), b64))


def short_name(path):
    s = path.lower().replace('\\', '/')
    if s.startswith('c3/'):
        s = s[3:]
    s = os.path.splitext(s)[0]
    return ''.join(ch if ch.isalnum() else '_' for ch in s)


# ---------------------------------------------------------------- NPC / monster meshes
def is_helper(ph, nparts):
    n = ph['name'].lower()
    if nparts < 2 or not n.startswith('v_') or n.startswith('v_body'):
        return False
    if n.startswith('v_armet') and ph['nv'] + ph['av'] > 40:
        return False
    return ph['nv'] + ph['av'] <= 40


def build_model(buf):
    """all PHY parts in one buffer, skinned (global bone = partBase + bone) in the bind pose (v * init).
    returns (meta, data, pose0) ; pose0 = posed positions with the file's own first keys (thumbnails)"""
    phys, _, _ = c3.parse_c3(buf)
    ms = motis_of(buf)
    base, pb = 0, []
    for i in range(len(phys)):
        pb.append(base)
        m = ms[i] if i < len(ms) else None
        base += m['bones'] if m else 0
    pos, uv, skin, opq, alp, helpers, posed = [], [], [], [], [], {}, []
    for i, ph in enumerate(phys):
        m = ms[i] if i < len(ms) else None
        k0 = key_matrices(m, 0)
        if is_helper(ph, len(phys)):
            helpers[ph['name']] = [i, [round(x, 5) for x in ph['init']]]
            continue
        if not ph['verts']:
            continue
        vb = len(pos)
        for v in ph['verts']:
            p = tuple(x if x == x and abs(x) < 1e7 else 0.0 for x in c3.xform(v['pos'], ph['init']))
            pos.append(p)
            (b0, b1), (w0, w1) = v['bones'], v['w']
            w0, w1 = max(w0, 0.0), max(w1, 0.0)
            ok0 = w0 > 0 and m and b0 < m['bones']
            ok1 = w1 > 0 and m and b1 < m['bones']
            if not ok0 and not ok1:
                skin.append((65535, 65535, 65535))
                posed.append(p)
                continue
            if not ok0:
                b0, w0 = b1, w1
                ok1, w1 = False, 0.0
            tw = w0 + (w1 if ok1 else 0.0)
            skin.append((pb[i] + b0, pb[i] + b1 if ok1 else 65535, int(round(65535 * w0 / tw))))
            if k0:
                acc = [0.0, 0.0, 0.0]
                for bi, w in ((b0, w0), (b1, w1 if ok1 else 0.0)):
                    if w > 0:
                        t = c3.xform(p, k0[bi])
                        acc = [acc[k] + w * t[k] for k in range(3)]
                posed.append(tuple((a / tw) if a == a and abs(a) < 1e7 else 0.0 for a in acc))
            else:
                posed.append(p)
        uv.extend(v['uv'] for v in ph['verts'])
        nv = len(ph['verts'])
        for k, t in enumerate(ph['tris']):
            if max(t) >= nv:
                continue
            (opq if k < ph['ntri'] else alp).append((t[0] + vb, t[1] + vb, t[2] + vb))
    if not pos or not (opq or alp):
        return None, None, None
    n = len(pos)
    bmin = [min(p[k] for p in posed) for k in range(3)]
    bmax = [max(p[k] for p in posed) for k in range(3)]
    gmin = [min(min(p[k] for p in pos), bmin[k]) for k in range(3)]
    gmax = [max(max(p[k] for p in pos), bmax[k]) for k in range(3)]
    umin = [min(t[k] for t in uv) for k in range(2)]
    umax = [max(t[k] for t in uv) for k in range(2)]
    q = [round(x, 5) for x in [min(p[k] for p in pos) for k in range(3)] + umin]
    pmax = [max(p[k] for p in pos) for k in range(3)]
    st = [float('%.6g' % (((hi - lo) / 65535.0) or 1e-6)) for lo, hi in zip(q, pmax + umax)]
    qa = array.array('h', [max(-32768, min(32767, int(round((p[k] - q[k]) / st[k])) - 32768)) for p in pos for k in range(3)])
    qa.extend(max(-32768, min(32767, int(round((t[k] - q[3 + k]) / st[3 + k])) - 32768)) for t in uv for k in range(2))
    if len(qa) % 2:
        qa.append(0)
    ia = array.array('H' if n < 65536 else 'I', [x for t in opq + alp for x in t])
    sk = array.array('H', [x for t in skin for x in t])
    if len(sk) % 2:
        sk.append(0)
    if sys.byteorder != 'little':
        qa.byteswap(); ia.byteswap(); sk.byteswap()
    meta = {'n': n, 'o': len(opq), 'a': len(alp), 'i': ia.itemsize, 'b': [round(x, 2) for x in bmin + bmax],
            'q': q + st, 's': 2, 'pb': pb, 'nb': base, 'pn': [p['name'] for p in phys]}
    if helpers:
        meta['h'] = helpers
    pad = b'' if (len(ia) * ia.itemsize) % 4 == 0 else b'\0\0'
    return meta, qa.tobytes() + ia.tobytes() + pad + sk.tobytes(), (posed, uv, opq, alp)


# ---------------------------------------------------------------- software thumbnails
def _png(path, w, h, rgba):
    raw = b''.join(b'\0' + bytes(rgba[y * w * 4:(y + 1) * w * 4]) for y in range(h))

    def ch(t, d):
        return struct.pack('>I', len(d)) + t + d + struct.pack('>I', zlib.crc32(t + d) & 0xffffffff)
    with open(path, 'wb') as f:
        f.write(b'\x89PNG\r\n\x1a\n' + ch(b'IHDR', struct.pack('>IIBBBBB', w, h, 8, 6, 0, 0, 0)) +
                ch(b'IDAT', zlib.compress(raw, 9)) + ch(b'IEND', b''))


def render_thumb(path, parts, size=128, yaw=-35.0, pitch=12.0):
    """parts: list of (posed points (C3 space), uvs, triangles, texture (w, h, rgba) or None).
    Writes a transparent PNG: textured, Lambert-lit, z-buffered, 2x supersampled."""
    S = size * 2
    img = bytearray(S * S * 4)
    zb = [1e30] * (S * S)
    ca, sa = math.cos(math.radians(yaw)), math.sin(math.radians(yaw))
    cp, sp = math.cos(math.radians(pitch)), math.sin(math.radians(pitch))
    P = []
    for pts, _, _, _ in parts:
        pp = []
        for x, y, z in pts:
            X, Y, Z = -x, -z, y          # C3 -> GL (y up), the model faces -Z
            X, Z = X * ca + Z * sa, -X * sa + Z * ca
            Y, Z = Y * cp - Z * sp, Y * sp + Z * cp
            pp.append((X, Y, Z))
        P.append(pp)
    allp = [p for pp in P for p in pp]
    if not allp:
        return False
    xs, ys = [p[0] for p in allp], [p[1] for p in allp]
    cx, cy = (max(xs) + min(xs)) / 2, (max(ys) + min(ys)) / 2
    sc = (S * 0.9) / max(max(xs) - min(xs), max(ys) - min(ys), 1e-3)
    L = (0.35, 0.55, -0.75)
    ln = math.sqrt(sum(c * c for c in L))
    L = tuple(c / ln for c in L)
    for (pts, uvs, tris, tex), pp in zip(parts, P):
        Q = [(S / 2 + (p[0] - cx) * sc, S / 2 - (p[1] - cy) * sc, p[2]) for p in pp]
        tw, th, tp = tex if tex else (1, 1, bytes((180, 180, 190, 255)))
        for a, b, c in tris:
            A, B, C = Q[a], Q[b], Q[c]
            pa, pb_, pc = pp[a], pp[b], pp[c]
            u = (pb_[0] - pa[0], pb_[1] - pa[1], pb_[2] - pa[2])
            v = (pc[0] - pa[0], pc[1] - pa[1], pc[2] - pa[2])
            nx, ny, nz = u[1] * v[2] - u[2] * v[1], u[2] * v[0] - u[0] * v[2], u[0] * v[1] - u[1] * v[0]
            nl = math.sqrt(nx * nx + ny * ny + nz * nz) or 1.0
            d = abs(nx * L[0] + ny * L[1] + nz * L[2]) / nl
            sh = 0.45 + 0.6 * d
            area = (B[0] - A[0]) * (C[1] - A[1]) - (C[0] - A[0]) * (B[1] - A[1])
            if abs(area) < 1e-9:
                continue
            x0, x1 = max(0, int(min(A[0], B[0], C[0]))), min(S - 1, int(max(A[0], B[0], C[0])) + 1)
            y0, y1 = max(0, int(min(A[1], B[1], C[1]))), min(S - 1, int(max(A[1], B[1], C[1])) + 1)
            ua, ub, uc = uvs[a], uvs[b], uvs[c]
            for yy in range(y0, y1 + 1):
                py = yy + 0.5
                for xx in range(x0, x1 + 1):
                    px = xx + 0.5
                    w0 = ((B[0] - px) * (C[1] - py) - (C[0] - px) * (B[1] - py)) / area
                    w1 = ((C[0] - px) * (A[1] - py) - (A[0] - px) * (C[1] - py)) / area
                    w2 = 1.0 - w0 - w1
                    if w0 < 0 or w1 < 0 or w2 < 0:
                        continue
                    z = w0 * A[2] + w1 * B[2] + w2 * C[2]
                    i = yy * S + xx
                    if z >= zb[i]:
                        continue
                    tu = w0 * ua[0] + w1 * ub[0] + w2 * uc[0]
                    tv = w0 * ua[1] + w1 * ub[1] + w2 * uc[1]
                    ti = ((int(tv * th) % th) * tw + int(tu * tw) % tw) * 4
                    if tp[ti + 3] < 100:
                        continue
                    zb[i] = z
                    o = i * 4
                    img[o] = min(255, int(tp[ti] * sh))
                    img[o + 1] = min(255, int(tp[ti + 1] * sh))
                    img[o + 2] = min(255, int(tp[ti + 2] * sh))
                    img[o + 3] = 255
    out = bytearray(size * size * 4)
    for y in range(size):
        for x in range(size):
            acc = [0, 0, 0, 0]
            for dy in (0, 1):
                for dx in (0, 1):
                    o = ((2 * y + dy) * S + 2 * x + dx) * 4
                    a = img[o + 3]
                    acc[0] += img[o] * a; acc[1] += img[o + 1] * a; acc[2] += img[o + 2] * a; acc[3] += a
            o = (y * size + x) * 4
            if acc[3]:
                out[o], out[o + 1], out[o + 2] = acc[0] // acc[3], acc[1] // acc[3], acc[2] // acc[3]
                out[o + 3] = acc[3] // 4
    _png(path, size, size, out)
    return True


def decode_texture(buf):
    if buf[:4] == b'DDS ':
        w, h, px = dds.decode_dds(buf)
    else:
        w, h, px = dds.decode_tga(buf)
    while max(w, h) > 128:   # thumbnails do not need more
        nw, nh = max(1, w // 2), max(1, h // 2)
        out = bytearray(nw * nh * 4)
        for y in range(nh):
            for x in range(nw):
                o, s = (y * nw + x) * 4, ((2 * y) * w + 2 * x) * 4
                out[o:o + 4] = px[s:s + 4]
        w, h, px = nw, nh, bytes(out)
    return w, h, px


# ---------------------------------------------------------------- tables
def read_simpleobj(buf):
    """3DSimpleObj.dbc: 'SIMO', uint32 count, {uint32 id, uint32 parts, {uint32 mesh, uint32 texture}[parts]}"""
    assert buf[:4] == b'SIMO'
    n = struct.unpack_from('<I', buf, 4)[0]
    o, out = 8, {}
    for _ in range(n):
        k, pc = struct.unpack_from('<II', buf, o)
        o += 8
        out[k] = [struct.unpack_from('<II', buf, o + 8 * j) for j in range(pc)]
        o += 8 * pc
    return out


# names shown in the action selector (number -> (english, arabic)); everything else is "Action N"
ACTION_NAMES = {
    1: ('Dance 1', 'رقصة 1'), 2: ('Dance 2', 'رقصة 2'), 3: ('Dance 3', 'رقصة 3'), 4: ('Dance 4', 'رقصة 4'),
    5: ('Dance 5', 'رقصة 5'), 6: ('Dance 6', 'رقصة 6'), 7: ('Dance 7', 'رقصة 7'), 8: ('Dance 8', 'رقصة 8'),
    100: ('Stand', 'واقف'), 101: ('Idle', 'استعداد'), 102: ('Idle 2', 'استعداد 2'), 103: ('Idle 3', 'استعداد 3'),
    105: ('Stand (alt)', 'واقف 2'), 110: ('Walk (left)', 'مشي (شمال)'), 111: ('Walk (right)', 'مشي (يمين)'),
    120: ('Run (left)', 'جري (شمال)'), 121: ('Run (right)', 'جري (يمين)'), 130: ('Jump', 'نط'),
    131: ('Jump 2', 'نط 2'), 132: ('Jump 3', 'نط 3'), 140: ('Laugh', 'ضحك'), 150: ('Happy', 'مبسوط'),
    160: ('Angry', 'زعلان'), 170: ('Sad', 'حزين'), 180: ('Excited', 'متحمس'), 190: ('Wave', 'سلام بالإيد'),
    200: ('Bow', 'انحناء'), 210: ('Kneel', 'ركوع'), 220: ('Kneel 2', 'ركوع 2'), 230: ('Cool', 'كول'),
    240: ('Swim', 'عوم'), 250: ('Sit', 'قاعد'), 260: ('Meditate', 'تأمل'), 270: ('Faint', 'إغماء'),
    280: ('Lie down', 'نايم'), 300: ('Hurt', 'اتضرب'), 310: ('Hit', 'ضربة'), 320: ('Dodge', 'تفادي'),
    330: ('Die', 'موت'), 331: ('Dead', 'ميت'), 401: ('Attack 1', 'هجوم 1'), 402: ('Attack 2', 'هجوم 2'),
    403: ('Attack 3', 'هجوم 3'), 404: ('Attack 4', 'هجوم 4'), 405: ('Attack 5', 'هجوم 5'),
    501: ('Cast', 'تعويذة'), 502: ('Cast 2', 'تعويذة 2'), 503: ('Cast 3', 'تعويذة 3'),
    700: ('XP skill 1', 'مهارة XP 1'), 701: ('XP skill 2', 'مهارة XP 2'), 702: ('XP skill 3', 'مهارة XP 3'),
    703: ('XP skill 4', 'مهارة XP 4'), 704: ('XP skill 5', 'مهارة XP 5'),
}
MONSTER_ACTIONS = (100, 101, 110, 401, 330)


def progress(i, n, label, t0):
    if i == n or i % 100 == 0:
        el = time.time() - t0
        eta = el / i * (n - i) if i else 0
        sys.stdout.write('\r  %-9s %5d/%d  %4.0fs  eta %3.0fs   ' % (label, i, n, el, eta))
        sys.stdout.flush()
        if i == n:
            sys.stdout.write('\n')


# ---------------------------------------------------------------- driver (called by extract3d.py)
# motion looks of each body: the current client uses looks 1-4 (c3/0001..0004, 130 weapon types, already in mesh space);
# 2001/2002/1003/1004 (c3/1001..1004) are an older set (45 weapon types, turned space, legs not animated in walk/run)
BODY_LOOKS = {1: (1, 2001), 2: (2, 2002), 3: (3, 1003), 4: (4, 1004)}
RIDE = {1: 71001, 2: 71002, 3: 71003, 4: 71004}   # riding motions of each body (c3/7100x/<weapon>/<action>.c3)
RIDE_ACTIONS = (100, 101, 110, 111, 120, 121, 130, 250)


def pose_points(phy, mats):
    out = []
    for v in phy['verts']:
        p = c3.xform(v['pos'], phy['init'])
        acc, tw = [0.0, 0.0, 0.0], 0.0
        for bi, w in zip(v['bones'], v['w']):
            if w > 0 and mats and bi < len(mats):
                t = c3.xform(p, mats[bi])
                acc = [acc[k] + w * t[k] for k in range(3)]
                tw += w
        out.append(tuple(a / tw for a in acc) if tw else p)
    return out


def body_correction(cl, b, stand_buf):
    """G^-1 for the player motions of body b (row-vector 4x4 as 16 floats). The ZKEY motions are turned 90 degrees
    around z and lifted to the hip height compared with the meshes' own keys: G = Rz(90) + (0, 0, h); h puts the
    lowest vertex of the naked body in the first stand frame on z = 0 (the ground)."""
    try:
        sig, rd = cl.locate(cl.obj[cl.dbc['armor'][b * 1000000][0]['mesh']])
        phys, _, _ = c3.parse_c3(rd())
        ms = motis_of(stand_buf)
    except Exception:
        return None, None
    names = [p['name'] for p in phys]
    if 'v_body' not in names:
        return None, None
    bi = names.index('v_body')
    zi = max(range(len(ms)), key=lambda i: ms[i]['bones'] if ms[i] else 0)
    Z = key_matrices(ms[zi], 0)
    K = key_matrices(motis_of(rd())[bi], 0)
    helpers = {p['name']: [round(x, 5) for x in p['init']] for p in phys if is_helper(p, len(phys))}
    if K and len(K) > 1 and len(Z) > 1 and max(abs(a - b) for a, b in zip(K[1][:12], Z[1][:12])) < 0.05:
        # looks 1-4: the motion is in the space of the mesh's own keys already
        return [1.0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 1.0], {'parts': names, 'body': bi, 'motionBody': zi, 'helpers': helpers}
    G0 = [0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0]
    Gi = matinv(G0)
    pts = pose_points(phys[bi], [matmul(m, Gi) for m in Z])
    Gi[14] = -max(p[2] for p in pts)      # z is down: the lowest point (largest z) goes to 0
    return [round(x, 5) for x in Gi], {'parts': names, 'body': bi, 'motionBody': zi, 'helpers': helpers}


def zkey_correction(mesh_buf, motion_buf):
    """G^-1 (16 floats) that brings a ZKEY motion onto the mesh's own first key (its static pose), or None when the
    motion is not ZKEY. G = K_j^-1 * Z_j on the most used bone that is invertible in both (body = part with most
    bones)."""
    km, zm = motis_of(mesh_buf), motis_of(motion_buf)
    if not km or not zm:
        return None
    bi = max(range(len(km)), key=lambda i: km[i]['bones'] if km[i] else 0)
    if bi >= len(zm) or not zm[bi] or not km[bi] or zm[bi]['t'] != 'q' or zm[bi]['bones'] != km[bi]['bones']:
        return None
    phys, _, _ = c3.parse_c3(mesh_buf)
    use = {}
    if bi < len(phys):
        for v in phys[bi]['verts']:
            for bb, w in zip(v['bones'], v['w']):
                if w > 0.5:
                    use[bb] = use.get(bb, 0) + 1
    K, Z = key_matrices(km[bi], 0), key_matrices(zm[bi], 0)
    for j in sorted(range(min(len(K), len(Z))), key=lambda j: -use.get(j, 0)):
        ki = matinv(K[j])
        if ki and matinv(Z[j]):
            g = matinv(matmul(ki, Z[j]))
            return [round(x, 5) for x in g] if g else None
    return None


def mount_corrections(cl, man, mt, failed):
    """per mount.dbc key with motions: G^-1 for its ZKEY motions (see zkey_correction)"""
    res = {}
    for mk, d in mt.items():
        p = d.get(100) or d.get(101)
        if not p or str(mk) not in man.get('mount', {}):
            continue
        try:
            sig, rd = cl.locate(cl.obj[cl.dbc['mount'][mk][0]['mesh']])
            msig, mrd = cl.locate(p)
            g = zkey_correction(rd(), mrd())
        except Exception as e:
            failed.append('mount %s (%s)' % (mk, e))
            continue
        if g:
            res[str(mk)] = g
    return res


def export_stage3(e3, cl, d3, man, client, force=False, thumbs=True):
    """writes out/3d/a, n, th, anim.js, npcs.js (new files only; index.js / m / t of stage 2 are not touched here
    except that NPC / monster textures are added to t/ with the same converter). Returns a summary dict."""
    t_start = time.time()
    for sub in ('a', 'n', 'th', 't'):
        os.makedirs(os.path.join(d3, sub), exist_ok=True)
    cache_p = os.path.join(d3, 'cache3.json')
    cache = {'av': ANIMVERSION, 'nv': NPCVERSION, 'hv': THUMBVERSION, 'fv': FXVERSION, 'a': {}, 'n': {}, 'h': {}, 't': {}, 'p': {}}
    if not force and os.path.exists(cache_p):
        try:
            c = json.load(open(cache_p, encoding='utf-8'))
            for k, v in (('a', 'av'), ('n', 'nv'), ('h', 'hv'), ('p', 'fv')):
                if c.get(v) == cache[v]:
                    cache[k] = c.get(k, {})
            cache['t'] = c.get('t', {})
        except Exception:
            pass
    ini = lambda n: open(os.path.join(client, 'ini', n), 'rb').read()
    mot = c3.read_rsdb(ini('3dmotion.dbc'))
    mmot = c3.read_rsdb(ini('mountmotion.dbc'))
    failed = []
    alist, aidx = [], {}

    def anim(path):
        """motion file -> index in alist (exported on demand, cached); -1 when unusable"""
        key = path.lower()
        if key in aidx:
            return aidx[key]
        sig, rd = cl.locate(path)
        name = 'a_' + short_name(path)
        fp = os.path.join(d3, 'a', name + '.js')
        ce = cache['a'].get(key)
        ok = False
        if sig and ce and ce[0] == sig and (not ce[1] or os.path.exists(fp)):
            ok = bool(ce[1])
        elif sig:
            try:
                meta, data = build_anim(rd())
            except Exception as e:
                meta = None
                failed.append('%s (%s)' % (path, e))
            if meta:
                write_js(fp, 'a', name, meta, data)
                ok = True
            cache['a'][key] = [sig, ok]
        aidx[key] = len(alist) if ok else -1
        if ok:
            alist.append(name)
        return aidx[key]

    out = {'v': ANIMVERSION, 'names': {str(k): v for k, v in ACTION_NAMES.items()}, 'bodies': {}, 'mounts': {}}
    # -- players: body -> 'm' (on foot) / 'r' (riding) -> weapon type -> action -> anim
    todo = []
    for b, (look, alt) in BODY_LOOKS.items():
        for lk, kind, prim in ((look, 'm', 1), (RIDE[b], 'r', 1)):
            for k, p in mot.items():
                if k // 10000000 == lk and (kind == 'm' or k % 10000 in RIDE_ACTIONS):
                    todo.append((b, kind, (k // 10000) % 1000, k % 10000, p, prim))
    paths = sorted({t[4].lower(): t[4] for t in todo}.values())
    t0 = time.time()
    for i, p in enumerate(paths, 1):
        anim(p)
        progress(i, len(paths), 'motions', t0)
    tables = {}
    for b, kind, w, a, p, prim in sorted(todo, key=lambda x: -x[5]):
        ai = aidx.get(p.lower(), -1)
        if ai >= 0:
            tables.setdefault(b, {'m': {}, 'r': {}})[kind].setdefault(str(w), {}).setdefault(str(a), ai)
    for b, (look, alt) in BODY_LOOKS.items():
        st = mot.get(look * 10000000 + 100) or mot.get(alt * 10000000 + 100)
        sig, rd = cl.locate(st) if st else (None, None)
        gi, info = body_correction(cl, b, rd()) if sig else (None, None)
        tb = tables.get(b, {'m': {}, 'r': {}})
        out['bodies'][str(b)] = {'look': look, 'gi': gi, 'info': info, 'm': tb['m'], 'r': tb['r']}
    # -- mounts: mountmotion.dbc key = mount.dbc key * 1000 + action
    mkeys = set(int(k) for k in man.get('mount', {}))
    mt = {}
    for k, p in mmot.items():
        mk, a = k // 1000, k % 1000
        if mk in mkeys and a in (100, 101, 110, 111, 120, 121, 130, 250):
            mt.setdefault(mk, {})[a] = p
    mpaths = sorted({p.lower(): p for d in mt.values() for p in d.values()}.values())
    t0 = time.time()
    for i, p in enumerate(mpaths, 1):
        anim(p)
        progress(i, len(mpaths), 'mounts', t0)
    for mk, d in mt.items():
        e = {str(a): aidx[p.lower()] for a, p in d.items() if aidx.get(p.lower(), -1) >= 0}
        if e:
            out['mounts'][str(mk)] = e
    out['mountGi'] = mount_corrections(cl, man, mt, failed)

    # -- NPCs and monsters
    npcs = export_npcs(e3, cl, d3, man, client, mot, anim, cache, failed, thumbs)

    out['anims'] = alist
    with open(os.path.join(d3, 'anim.js'), 'w', encoding='utf-8') as f:
        f.write('window.ASSET3D_ANIM=' + json.dumps(out, separators=(',', ':'), ensure_ascii=False) + ';\n')
    with open(cache_p, 'w', encoding='utf-8') as f:
        json.dump(cache, f, separators=(',', ':'))
    nb = sum(len(v) for bd in out['bodies'].values() for t in ('m', 'r') for v in bd[t].values())
    print('  animations: %d files, %d player action entries, %d mounts with motions, %d problems, %.0fs' % (
        len(alist), nb, len(out['mounts']), len(failed), time.time() - t_start))
    return {'anims': len(alist), 'failed': failed, 'npcs': npcs}


def server_monster_names(client):
    """lookface -> [names] from the game server's monster type files (read-only; optional, used only for labels).
    Looked up in assetstudio.ini [paths] server_monsters, else ..\\..\\Source\\bin\\Debug\\ServerDataBase\\Monsters"""
    import configparser
    cp = configparser.ConfigParser()
    cp.read(os.path.join(HERE, 'assetstudio.ini'), encoding='utf-8')
    d = cp.get('paths', 'server_monsters', fallback='') or os.path.normpath(
        os.path.join(HERE, '..', '..', 'Source', 'bin', 'Debug', 'ServerDataBase', 'Monsters'))
    out = {}
    if not os.path.isdir(d):
        return out, None
    for fn in os.listdir(d):
        try:
            t = open(os.path.join(d, fn), 'rb').read().decode('latin1')
        except OSError:
            continue
        kv = dict(l.split('=', 1) for l in t.splitlines() if '=' in l)
        try:
            lf = int(kv.get('lookface', '0').strip() or 0)
        except ValueError:
            continue
        nm = kv.get('name', '').strip()
        if lf and nm:
            out.setdefault(lf, [])
            if nm not in out[lf]:
                out[lf].append(nm)
    return out, d


def export_npcs(e3, cl, d3, man, client, mot, anim, cache, failed, thumbs=True):
    import extract
    ini = lambda n: open(os.path.join(client, 'ini', n), 'rb').read()
    simo = read_simpleobj(ini('3DSimpleObj.dbc'))
    secs = extract.parse_ini(extract.read_client_text(os.path.join(client, 'ini', 'npc.ini')) or '')
    mnames, mdir = server_monster_names(client)
    models, mlist, midx, tlist, tidx = {}, [], {}, [], {}
    t_conv = 0

    def mesh(path):
        key = path.lower()
        if key in midx:
            return midx[key]
        sig, rd = cl.locate(path)
        name = 'n_' + short_name(path)
        fp = os.path.join(d3, 'n', name + '.js')
        ce = cache['n'].get(key)
        if sig and ce and ce[0] == sig and (not ce[1] or os.path.exists(fp)):
            res = ce[1]
        elif sig:
            res = None
            try:
                meta, data, _ = build_model(rd())
                if meta:
                    write_js(fp, 'm', name, meta, data)
                    res = {'n': meta['n'], 'b': meta['b']}
            except Exception as e:
                failed.append('%s (%s)' % (path, e))
            cache['n'][key] = [sig, res]
        else:
            res = None
        midx[key] = len(mlist) if res else -1
        if res:
            mlist.append(name)
        return midx[key]

    def tex(path, maxsize=None):
        nonlocal t_conv
        key = path.lower()
        if key in tidx:
            return tidx[key]
        sig, rd = cl.locate(path)
        name = 't_' + short_name(path)
        fp = os.path.join(d3, 't', name + '.js')
        ce = cache['t'].get(key)
        ok = False
        if sig and ce and ce[0] == sig and (not ce[1] or os.path.exists(fp)):
            ok = ce[1]
        elif sig:
            if os.path.exists(fp) and not ce:
                ok = True   # already written by the stage-2 exporter (same converter, same name)
            else:
                try:
                    keep = e3.MAXTEX
                    if maxsize:
                        e3.MAXTEX = maxsize
                    try:
                        meta, data = e3.build_texture(rd())
                    finally:
                        e3.MAXTEX = keep
                    write_js(fp, 't', name, meta, data)
                    ok = True
                    t_conv += 1
                except Exception as e:
                    failed.append('%s (%s)' % (path, e))
            cache['t'][key] = [sig, ok]
        tidx[key] = len(tlist) if ok else -1
        if ok:
            tlist.append(name)
        return tidx[key]

    def model(key, parts, motions):
        """parts: [(mesh path, texture path)], motions: {action: path} -> model entry (or None)"""
        ps = []
        for mp, tp in parts:
            mi = mesh(mp) if mp else -1
            if mi < 0:
                continue
            ps.append([mi, tex(tp) if tp else -1, mp, tp])
        if not ps:
            return None
        an = {}
        for a, p in motions.items():
            ai = anim(p)
            if ai >= 0:
                an[str(a)] = ai
        m = {'p': [[x[0], x[1]] for x in ps], 'a': an, '_src': [(x[2], x[3]) for x in ps]}
        st = motions.get('100') or motions.get(100) or (list(motions.values())[0] if motions else None)
        if st:
            try:
                g = zkey_correction(cl.locate(ps[0][2])[1](), cl.locate(st)[1]())
                if g:
                    m['gi'] = g
            except Exception:
                pass
        models[key] = m
        return m

    # NPC looks (npc.ini NpcType -> SimpleObjID -> 3DSimpleObj.dbc parts)
    npcs = []
    t0 = time.time()
    rows = [(s, kv) for s, kv in secs if s.startswith('NpcType') and s[7:].isdigit()]
    for i, (s, kv) in enumerate(rows, 1):
        look = kv.get('SimpleObjID', '').strip()
        lk = int(look) if look.isdigit() else None
        key = 's%d' % lk if lk is not None else None
        if key and key not in models and lk in simo:
            parts = [(cl.obj.get(m), cl.tex.get(t)) for m, t in simo[lk]]
            mo = {}
            for a, f in (('100', 'StandByMotion'), ('101', 'RestMotion'), ('190', 'BlazeMotion')):
                v = kv.get(f, '').strip()
                if v.isdigit() and int(v) in mot:
                    mo[a] = mot[int(v)]
            model(key, parts, mo)
        npcs.append([int(s[7:]), extract.nice(kv.get('Name', '')), look, extract.nice(kv.get('Note', '')),
                     key if key in models else None])
        progress(i, len(rows), 'npcs', t0)
    # monsters (armor.dbc rows lookface * 1000000 with lookface > 4; motions lookface * 10^7 + action)
    mons = []
    arm = cl.dbc['armor']
    looks = sorted(k // 1000000 for k in arm if k // 1000000 > 4 and k % 1000000 == 0)
    t0 = time.time()
    for i, lf in enumerate(looks, 1):
        parts = [(cl.obj.get(p['mesh']), cl.tex.get(p['tex'])) for p in arm[lf * 1000000]]
        mo = {a: mot[lf * 10000000 + a] for a in MONSTER_ACTIONS if lf * 10000000 + a in mot}
        key = 'm%d' % lf
        if model(key, parts, mo):
            mons.append([lf, mnames.get(lf, [])])
        progress(i, len(looks), 'monsters', t0)

    # mounts: the same skinned format (n_<name> next to the static m_<name> of stage 2) so the viewer can animate them
    mpaths = sorted({cl.obj[p['mesh']].lower(): cl.obj[p['mesh']] for k in man.get('mount', {})
                     for p in cl.dbc['mount'].get(int(k), []) if cl.obj.get(p['mesh'])}.values())
    t0 = time.time()
    for i, p in enumerate(mpaths, 1):
        mesh(p)
        progress(i, len(mpaths), 'mountmesh', t0)

    # effects (wings, item glows / auras): share the mesh / texture / animation exporters
    fx = None
    try:
        fx = export_effects(e3, cl, d3, client, anim, mesh, tex, cache, failed)
    except Exception as ex:
        failed.append('effects (%s)' % ex)
        import traceback
        traceback.print_exc()

    # thumbnails
    nth = 0
    if thumbs:
        t0 = time.time()
        keys = sorted(models)
        for i, key in enumerate(keys, 1):
            m = models[key]
            sigs = []
            for mp, tp in m['_src']:
                sigs.append(cl.locate(mp)[0] or '')
                sigs.append((cl.locate(tp)[0] if tp else '') or '')
            sig = '|'.join(sigs)
            fn = 'th_%s.png' % key
            fp = os.path.join(d3, 'th', fn)
            ce = cache['h'].get(key)
            if ce and ce[0] == sig and (not ce[1] or os.path.exists(fp)):
                ok = ce[1]
            else:
                ok = False
                try:
                    ok = make_thumb(cl, m['_src'], fp)
                    nth += 1
                except Exception as e:
                    failed.append('thumb %s (%s)' % (key, e))
                cache['h'][key] = [sig, ok]
            if ok:
                m['th'] = fn
            progress(i, len(keys), 'thumbs', t0)
    for m in models.values():
        m.pop('_src', None)
    out = {'v': NPCVERSION, 'npcs': npcs, 'monsters': mons, 'models': models, 'meshes': mlist, 'textures': tlist,
           'monsterNames': mdir and os.path.basename(os.path.dirname(os.path.dirname(mdir)))}
    with open(os.path.join(d3, 'npcs.js'), 'w', encoding='utf-8') as f:
        f.write('window.ASSET3D_NPCS=' + json.dumps(out, separators=(',', ':'), ensure_ascii=False) + ';\n')
    if fx:
        with open(os.path.join(d3, 'fx.js'), 'w', encoding='utf-8') as f:
            f.write('window.ASSET3D_FX=' + json.dumps(fx, separators=(',', ':'), ensure_ascii=False) + ';\n')
        print('  effects: %d exported (%s), wings %d types, item maps: %s' % (
            len(fx['defs']), ', '.join('%s %d' % kv for kv in fx['stats'].items()), len(fx['wings']),
            ', '.join('%s %d' % (k, len(v)) for k, v in fx['maps'].items())))
    print('  npcs: %d NpcTypes (%d with a model), monsters: %d looks (%d named from %s), models %d, meshes %d, '
          'new textures %d, thumbnails made %d' % (len(npcs), sum(1 for n in npcs if n[4]), len(mons),
                                                 sum(1 for m in mons if m[1]), mdir or '-', len(models), len(mlist),
                                                 t_conv, nth))
    return {'npcs': len(npcs), 'npcModels': sum(1 for n in npcs if n[4]), 'monsters': len(mons), 'models': len(models)}


def make_thumb(cl, src, fp):
    parts = []
    for mp, tp in src:
        sig, rd = cl.locate(mp)
        if not sig:
            continue
        meta, data, pz = build_model(rd())
        if not meta:
            continue
        posed, uv, opq, alp = pz
        tx = None
        if tp:
            ts, tr = cl.locate(tp)
            if ts:
                try:
                    tx = decode_texture(tr())
                except Exception:
                    tx = None
        parts.append((posed, uv, opq + alp, tx))
    return render_thumb(fp, parts) if parts else False


# ---------------------------------------------------------------- effects (wings, glows, auras): out/3d/fx.js, p/
FXVERSION = 2
FXTEX = 128          # effect textures are reduced to 128 px (glows / sprites do not need more)


def read_effects(buf):
    """3DEffect.dbc: 'EFFE', uint32 count, records {char name[32]; uint16 parts; uint32 delay, loopTime,
    frameInterval, loopInterval; float offset[3]; uint8 ?, colorEnable, lev, ?; part[parts] {uint32 effectObj
    (3DEffectobj.dbc id -> c3 file), uint32 texture (3DTexture.dbc), float scale, uint8 asb, adb (D3DBLEND: 5/6 =
    alpha blend, 5/2 = additive), uint16 pad}}  (66 + 16 * parts bytes)"""
    assert buf[:4] == b'EFFE'
    n = struct.unpack_from('<I', buf, 4)[0]
    o, out = 8, {}
    for _ in range(n):
        name = buf[o:o + 32].split(b'\0')[0].decode('latin1')
        amt, delay, loop, fi, li, x, y, z, b0, ce, lev, b3 = struct.unpack_from('<HIIII3f4B', buf, o + 32)
        parts = [struct.unpack_from('<IIfBBH', buf, o + 66 + 16 * k) for k in range(amt)]
        out[name] = {'delay': delay, 'loop': loop, 'fi': fi, 'li': li, 'off': (x, y, z), 'parts': parts}
        o += 66 + 16 * amt
    return out


def _ptc_chain(d, q, ids):
    frames, starts = 0, []
    while q + 4 <= len(d):
        n = struct.unpack_from('<I', d, q)[0]
        step = 4 + (2 * n if ids else 0) + 20 * n + 64
        if n > 5000 or q + step > len(d) or abs(struct.unpack_from('<f', d, q + step - 4)[0] - 1.0) > 1e-3:
            break
        starts.append(q)
        q += step
        frames += 1
    return starts, len(d) - q


def parse_particles(tag, d):
    """PTCL / PTC3 chunk = baked particle system: uint32 nameLen, name, uint32 texLen, tex (artist path), header
    (PTCL: uint32 spriteRows, maxCount, frameCount; PTC3: longer, varies), then per frame {uint32 n; [PTC3: uint16
    id[n]]; float pos[n][3]; float age[n]; float size[n]; float matrix[16]}. The header length of PTC3 varies, so it
    is found by requiring the frame chain to use up the chunk. Returns {'row', 'frames': [(pos, age, size, matrix)]}"""
    nl = struct.unpack_from('<I', d, 0)[0]
    q = 4 + nl
    tl = struct.unpack_from('<I', d, q)[0]
    q += 4 + tl
    ids = tag == b'PTC3'
    row = struct.unpack_from('<I', d, q)[0] if q + 4 <= len(d) else 1
    for h in ([12] if not ids else []) + list(range(4, 260)):
        if q + h + 4 > len(d):
            break
        starts, rest = _ptc_chain(d, q + h, ids)
        if starts and rest <= 64 and (h == 12 or len(starts) > 1):
            frames = []
            for s in starts:
                n = struct.unpack_from('<I', d, s)[0]
                p = s + 4 + (2 * n if ids else 0)
                pos = struct.unpack_from('<%df' % (3 * n), d, p)
                age = struct.unpack_from('<%df' % n, d, p + 12 * n)
                size = struct.unpack_from('<%df' % n, d, p + 16 * n)
                m = struct.unpack_from('<16f', d, p + 20 * n)
                frames.append((pos, age, size, m))
            return {'row': max(1, min(16, row)), 'frames': frames}
    return None


def build_particles(buf):
    """all PTCL / PTC3 chunks of an effect file -> (meta, bytes). meta.s[i] = [spriteRows, counts per frame,
    pmin x3, pstep x3, sizeMax]; data per system: float32 matrix[16] per frame, then int16 per frame:
    pos (pmin + (v + 32768) * pstep) x3n, age (v / 30000) xn, size (v / 32767 * sizeMax) xn (padded to 4 bytes)"""
    systems, data = [], bytearray()
    fin = lambda x: x if x == x and abs(x) < 1e7 else 0.0
    for tag, d in chunks(buf):
        if tag not in (b'PTCL', b'PTC3'):
            continue
        try:
            ps = parse_particles(tag, d)
        except struct.error:
            ps = None
        if not ps:
            continue
        frs = ps['frames']
        allp = [fin(x) for pos, _, _, _ in frs for x in pos]
        if allp:
            pmin = [min(allp[k::3]) for k in range(3)]
            pmax = [max(allp[k::3]) for k in range(3)]
        else:
            pmin, pmax = [0.0] * 3, [1.0] * 3
        st = [float('%.6g' % max((hi - lo) / 65000.0, 1e-5)) for lo, hi in zip(pmin, pmax)]
        pmin = [float('%.6g' % x) for x in pmin]
        smax = max([abs(fin(x)) for _, _, size, _ in frs for x in size] + [1e-3])
        mats = array.array('f', [fin(x) for _, _, _, m in frs for x in m])
        vals = array.array('h')
        for pos, age, size, m in frs:
            n = len(age)
            vals.extend(max(-32768, min(32767, int(round((fin(pos[i]) - pmin[i % 3]) / st[i % 3])) - 32768)) for i in range(3 * n))
            vals.extend(max(-32768, min(32767, int(round(fin(a) * 30000)))) for a in age)
            vals.extend(max(-32767, min(32767, int(round(fin(x) / smax * 32767)))) for x in size)
        if len(vals) % 2:
            vals.append(0)
        if sys.byteorder != 'little':
            mats.byteswap(); vals.byteswap()
        data += mats.tobytes() + vals.tobytes()
        systems.append([ps['row'], [len(f[1]) for f in frs]] + pmin + st + [float('%.6g' % smax)])
    if not systems:
        return None, None
    return {'s': systems}, bytes(data)


def effect_kinds(buf):
    tags = set(t for t, _ in chunks(buf))
    return {'mesh': bool(tags & {b'PHY ', b'PHY4', b'PHY3'}), 'ptc': bool(tags & {b'PTCL', b'PTC3'}),
            'shape': b'SHAP' in tags}


def read_action_effects(text):
    """Action3DEffect.ini -> {section: [(key parts list, [(effect, sx, sy, sz)])]}"""
    secs, cur = {}, None
    for line in text.splitlines():
        line = line.strip()
        if line.startswith('[') and line.endswith(']'):
            cur = line[1:-1].lower()
            secs.setdefault(cur, [])
            continue
        if not cur or '=' not in line or line.startswith('//'):
            continue
        k, v = line.split('=', 1)
        effs = []
        for e in v.split(','):
            f = e.strip().split('~')
            if not f[0] or f[0].lower() == 'none':
                continue
            sc = [float(x) / 100.0 if x.replace('.', '').isdigit() else 1.0 for x in (f[1:4] + ['100'] * 3)[:3]]
            effs.append((f[0], sc))
        if effs:
            secs[cur].append((k.strip().split('.'), effs))
    return secs


IDLE_ACTIONS = ('9999', '0100', '0101')


def export_effects(e3, cl, d3, client, anim, mesh_fn, tex_fn, cache, failed):
    """wings (info.ini WingEffectInfo: wing item family -> _p_24_wingN_on/up/down/off), item glows from
    Action3DEffect.ini ([body] garments/armor, [weapon], [mount], [wing]) for idle actions; writes fx.js and p/"""
    import extract
    os.makedirs(os.path.join(d3, 'p'), exist_ok=True)
    fxdb = read_effects(open(os.path.join(client, 'ini', '3DEffect.dbc'), 'rb').read())
    eobj = c3.read_rsdb(open(os.path.join(client, 'ini', '3DEffectobj.dbc'), 'rb').read())
    act = read_action_effects(open(os.path.join(client, 'ini', 'Action3DEffect.ini'), 'rb').read().decode('gbk', 'replace'))
    info = (extract.read_client_text(os.path.join(client, 'ini', 'info.ini')) or '')
    wings = {}
    sec = info[info.find('[WingEffectInfo]'):]
    sec = sec[:sec.find('\n[', 5)] if '\n[' in sec[5:] else sec
    import re
    for fam, state, lvl, val in re.findall(r'^(\d+)Wing([A-Za-z]+?)(\d+)=(\S+)', sec, re.M):
        wings.setdefault(fam, {}).setdefault(lvl, {})[state.lower()] = val
    need = set()
    for fam, lv in wings.items():
        for l, st in lv.items():
            for s in ('on', 'up'):
                if st.get(s):
                    need.add(st[s])
    maps = {'body': {}, 'weapon': {}, 'mount': {}, 'wing': {}}
    for secname, rows in act.items():
        if secname not in maps:
            continue
        for key, effs in rows:
            if len(key) < 4 or key[1] not in IDLE_ACTIONS:
                continue
            try:
                if secname == 'mount':
                    item = 80000000 + int(key[2]) * 1000 + int(key[3])
                    who = '*'
                else:
                    item = int(key[2]) * 1000 + int(key[3])
                    who = key[0][0] if secname == 'body' and key[0][1:] != '999' and key[0] != '9999' else '*'
                    if secname == 'body' and key[0].startswith('000'):
                        who = key[0][3]
            except ValueError:
                continue
            k = '%s|%d' % (who, item)
            lst = maps[secname].setdefault(k, [])
            for e, sc in effs:
                if [e, sc] not in lst:
                    lst.append([e, [round(x, 3) for x in sc]])
                need.add(e)
    # export the effects
    defs, plist, pidx = {}, [], {}
    stats = {'mesh': 0, 'ptc': 0, 'shape_only': 0, 'missing': 0}

    def particles(path):
        key = path.lower()
        if key in pidx:
            return pidx[key]
        sig, rd = cl.locate(path)
        name = 'p_' + short_name(path)
        fp = os.path.join(d3, 'p', name + '.js')
        ce = cache['p'].get(key)
        ok = False
        if sig and ce and ce[0] == sig and (not ce[1] or os.path.exists(fp)):
            ok = ce[1]
        elif sig:
            try:
                meta, data = build_particles(rd())
                if meta:
                    write_js(fp, 'p', name, meta, data)
                    ok = True
            except Exception as ex:
                failed.append('%s (%s)' % (path, ex))
            cache['p'][key] = [sig, ok]
        pidx[key] = len(plist) if ok else -1
        if ok:
            plist.append(name)
        return pidx[key]

    t0 = time.time()
    names = sorted(need)
    for i, nm in enumerate(names, 1):
        r = fxdb.get(nm)
        progress(i, len(names), 'effects', t0)
        if not r:
            stats['missing'] += 1
            continue
        parts = []
        for eid, tid, scale, asb, adb, _ in r['parts']:
            path = eobj.get(eid)
            tpath = cl.tex.get(tid)
            if not path:
                continue
            sig, rd = cl.locate(path)
            if not sig:
                continue
            try:
                kinds = effect_kinds(rd())
            except Exception:
                continue
            ti = tex_fn(tpath, FXTEX) if tpath else -1
            if kinds['mesh']:
                mi = mesh_fn(path)
                if mi >= 0:
                    parts.append({'m': mi, 'a': anim(path), 't': ti, 'b': [asb, adb], 's': round(scale, 3)})
                    stats['mesh'] += 1
            if kinds['ptc']:
                pi = particles(path)
                if pi >= 0:
                    parts.append({'p': pi, 't': ti, 'b': [asb, adb], 's': round(scale, 3)})
                    stats['ptc'] += 1
            if kinds['shape'] and not kinds['mesh'] and not kinds['ptc']:
                stats['shape_only'] += 1
        if parts:
            defs[nm] = {'p': parts, 'fi': r['fi'] or 33, 'o': [round(x, 2) for x in r['off']], 'd': r['delay']}
    # keep only the references that resolved
    for m in maps.values():
        for k in list(m):
            m[k] = [e for e in m[k] if e[0] in defs]
            if not m[k]:
                del m[k]
    wl = {}
    for fam, lv in wings.items():
        e = {}
        for l, st in lv.items():
            for s, v in st.items():
                if v in defs:
                    e.setdefault(l, {})[s] = v
        if e:
            wl[fam] = e
    return {'v': FXVERSION, 'defs': defs, 'particles': plist, 'wings': wl, 'maps': maps, 'stats': stats}
