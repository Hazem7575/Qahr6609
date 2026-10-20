"""Stage-2 proof of concept: TQ C3 model reader + item -> mesh/texture resolver + OBJ export.

C3 file = 16-byte signature "MAXFILE C3 00001" followed by chunks  {char tag[4]; uint32 size; byte data[size]}.

'PHY3' chunk (mounts, c3/Mount, c3/MountSaddle) = same as PHY4 plus float normal[3] at the end (52-byte vertex).

'PHY4' chunk = same as 'PHY ' but the vertex is 40 bytes: float pos[3]; float u, v; uint32 color;
    uint32 bone[2]; float weight[2]   (no morph targets) - the most common mesh chunk in this client.

'PHY ' chunk (skinned mesh, one per body part: v_body, v_armet, v_l_weapon, v_r_weapon ...):
    uint32 nameLen; char name[nameLen]
    uint32 blendCount                    (bones influencing a vertex, 0..2)
    uint32 normalVertexCount, alphaVertexCount
    vertex[normal+alpha], 76 bytes each:
        float pos[4][3]                  (4 morph targets; [0] = base shape)
        float u, v
        uint32 color                     (ARGB, usually 0)
        uint32 bone[2]                   (index into the MOTI bone list)
        float weight[2]
    uint32 normalTriCount, alphaTriCount
    uint16 tri[normal+alpha][3]          (normal = opaque pass, alpha = blended pass)
    uint32 texNameLen; char texName[]    (original 3ds max texture path, GBK; informational only)
    float bboxMin[3], bboxMax[3]
    float initMatrix[16]                 (row-vector D3D matrix: v' = v * M, translation in [12..14])
    ... 28 trailing bytes (uint32 texRow, padding, 'STEP' sub-tag) - not needed for a static export

'MOTI' chunk (skeleton animation, follows the PHY chunks in the same order):
    uint32 boneCount; uint32 frameCount
    char keyType[4]  'KKEY' / 'ZKEY' (seen; 'XKEY' exists in other TQ clients)
    uint32 keyCount
    KKEY key: uint32 frame; float matrix[boneCount][16]                (mesh files: rest pose, 1 key)
    ZKEY key: uint16 frame; {float q[4]; float t[3]}[boneCount]        (motion files c3/<body>/<weapon>/*.c3;
              size formula verified on 200/200 motion chunks, q[0..3] has |q| = 1 -> rotation quaternion)
    uint32 morphCount (+ float morphWeights[morphCount*frameCount] when > 0)

Pose used here: v_world = sum_i weight_i * ((v * initMatrix) * bone_i(key 0)); v * initMatrix alone = bind pose (T-pose).

Item id -> files (client 6609):
    garment / armor id  -> ini/armor.dbc (binary 'MESH' table, newer than armor.ini) key = body*1000000 + id - id%10
                           (body digit = player Body % 10: 1 = 2001 female small, 2 = 2002 female big,
                            3 = 1003 male small, 4 = 1004 male big - verified by rendering the exported meshes;
                            the hundreds digit of the id is the colour and stays in the key)
    record -> Mesh id, Texture id  ->  ini/3DObj.dbc / ini/3DTexture.dbc  ('RSDB' id->path string tables, newer than
                           3dobj.ini / 3dtexture.ini)  -> c3/mesh/*.c3 and c3/texture/*.dds (loose or inside c3.wdf)

usage:  python c3model.py <item id> [body 1-4] [out dir]
"""
import os, struct, sys, configparser

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
import dds  # noqa: E402
from wdf import WDF  # noqa: E402


# ------------------------------------------------------------------ dbc tables
def read_rsdb(buf):
    """'RSDB' table: uint32 count; {uint32 id, uint32 fileOffset}[count]; zero-terminated strings"""
    assert buf[:4] == b'RSDB', buf[:4]
    n = struct.unpack_from('<I', buf, 4)[0]
    out = {}
    # 3DObj/3DTexture use uint32 ids (8-byte entries); 3dmotion/weaponmotion/mountmotion use uint64 ids (12 bytes)
    wide = n and struct.unpack_from('<I', buf, 16)[0] == 8 + 12 * n
    fmt, step = ('<QI', 12) if wide else ('<II', 8)
    for i in range(n):
        k, off = struct.unpack_from(fmt, buf, 8 + step * i)
        e = buf.index(b'\0', off)
        out[k] = buf[off:e].decode('latin1').replace('\\', '/')
    return out


def read_mesh_dbc(buf):
    """'MESH' table (armor.dbc, armet.dbc, weapon.dbc, mount.dbc):
    uint32 count; record = uint32 id, uint32 partCount, part[partCount] {uint32 mesh, texture, mixTex;
    uint8 mixOpt, asb, adb, material}"""
    assert buf[:4] == b'MESH', buf[:4]
    n = struct.unpack_from('<I', buf, 4)[0]
    o = 8
    out = {}
    for _ in range(n):
        k, parts = struct.unpack_from('<II', buf, o)
        o += 8
        pl = []
        for _ in range(parts):
            mesh, tex, mix, mo, asb, adb, mat = struct.unpack_from('<IIIBBBB', buf, o)
            o += 16
            pl.append({'mesh': mesh, 'tex': tex, 'mix': mix, 'mixopt': mo, 'asb': asb, 'adb': adb, 'mat': mat})
        out[k] = pl
    assert o == len(buf), (o, len(buf))
    return out


# ------------------------------------------------------------------ c3 parsing
def parse_c3(buf):
    if not buf.startswith(b'MAXFILE C3'):
        raise ValueError('not a C3 file')
    o = 16
    phys, motis, other = [], [], []
    while o + 8 <= len(buf):
        tag, size = struct.unpack_from('<4sI', buf, o)
        body = buf[o + 8:o + 8 + size]
        if tag == b'PHY ':
            phys.append(parse_phy(body, 76))
        elif tag == b'PHY4':
            phys.append(parse_phy(body, 40))
        elif tag == b'PHY3':
            phys.append(parse_phy(body, 52, 12))
        elif tag == b'MOTI':
            motis.append(parse_moti(body))
        else:
            other.append((tag.decode('latin1'), size))
        o += 8 + size
    return phys, motis, other


def parse_phy(b, vsize=76, uvo=None):
    nl = struct.unpack_from('<I', b, 0)[0]
    name = b[4:4 + nl].decode('latin1')
    q = 4 + nl
    blend, nv, av = struct.unpack_from('<3I', b, q)
    q += 12
    verts = []
    if uvo is None:
        uvo = vsize - 28  # PHY : 4 morph positions (48 bytes) ; PHY4: 1 position (12 bytes)
    # PHY3 (mounts): 52-byte vertex = pos[3], u, v, color, bone[2], weight[2], normal[3]  (uvo = 12)
    for _ in range(nv + av):
        pos = struct.unpack_from('<3f', b, q)
        u, v = struct.unpack_from('<2f', b, q + uvo)
        color, b0, b1 = struct.unpack_from('<3I', b, q + uvo + 8)
        w0, w1 = struct.unpack_from('<2f', b, q + uvo + 20)
        verts.append({'pos': pos, 'uv': (u, v), 'bones': (b0, b1), 'w': (w0, w1)})
        q += vsize
    nt, at = struct.unpack_from('<2I', b, q)
    q += 8
    tris = [struct.unpack_from('<3H', b, q + 6 * i) for i in range(nt + at)]
    q += 6 * (nt + at)
    tl = struct.unpack_from('<I', b, q)[0]
    tex = b[q + 4:q + 4 + tl].decode('gbk', 'replace')
    q += 4 + tl
    bb = struct.unpack_from('<6f', b, q)
    q += 24
    init = struct.unpack_from('<16f', b, q)
    return {'name': name, 'blend': blend, 'nv': nv, 'av': av, 'verts': verts, 'ntri': nt, 'atri': at,
            'tris': tris, 'tex': tex, 'bbox': bb, 'init': init}


def parse_moti(b):
    bones, frames = struct.unpack_from('<II', b, 0)
    kt = b[8:12]
    res = {'bones': bones, 'frames': frames, 'type': kt.decode('latin1'), 'key0': None}
    if kt == b'KKEY':
        kc = struct.unpack_from('<I', b, 12)[0]
        if kc:
            q = 16 + 4  # skip frame index of key 0
            res['key0'] = [struct.unpack_from('<16f', b, q + 64 * i) for i in range(bones)]
        res['keys'] = kc
    return res


def xform(v, m):
    x, y, z = v
    return (x * m[0] + y * m[4] + z * m[8] + m[12],
            x * m[1] + y * m[5] + z * m[9] + m[13],
            x * m[2] + y * m[6] + z * m[10] + m[14])


def bind_pose(phy, moti):
    """v_out = sum_i weight_i * ((v * initMatrix) * bone_i[key 0])   (stage 2: init first, then the bone; with the
    order reversed the armor files - whose first key is a standing frame, not identity - come out distorted).
    v * initMatrix alone is the bind pose (a T-pose) of every character mesh."""
    pts = []
    mats = moti['key0'] if moti and moti.get('key0') else None
    for v in phy['verts']:
        p = xform(v['pos'], phy['init'])
        if mats:
            acc = [0.0, 0.0, 0.0]
            tw = 0.0
            for bi, w in zip(v['bones'], v['w']):
                if w > 0 and bi < len(mats):
                    t = xform(p, mats[bi])
                    acc = [acc[k] + w * t[k] for k in range(3)]
                    tw += w
            p = tuple(acc) if tw > 0 else p
        pts.append(p)
    return pts


# ------------------------------------------------------------------ resolver
class Client:
    def __init__(self, root):
        self.root = root
        self.wdf = [WDF(os.path.join(root, n)) for n in ('c3.wdf', 'data.wdf') if os.path.exists(os.path.join(root, n))]
        ini = lambda n: open(os.path.join(root, 'ini', n), 'rb').read()
        self.armor = read_mesh_dbc(ini('armor.dbc'))
        self.obj = read_rsdb(ini('3DObj.dbc'))
        self.tex = read_rsdb(ini('3DTexture.dbc'))

    def read(self, rel):
        p = os.path.join(self.root, rel.replace('/', os.sep))
        if os.path.exists(p):
            return open(p, 'rb').read(), 'loose'
        for w in self.wdf:
            b = w.read(rel)
            if b is not None:
                return b, os.path.basename(w.path)
        return None, None

    def armor_parts(self, item_id, body):
        key = body * 1000000 + item_id - item_id % 10
        return key, self.armor.get(key)


def export(item_id, body=1, outdir=None):
    cp = configparser.ConfigParser()
    cp.read(os.path.join(HERE, 'assetstudio.ini'), encoding='utf-8')
    root = cp.get('paths', 'client', fallback=r'D:\Conquer Online 3.0')
    outdir = outdir or os.path.join(HERE, 'stage2_proof')
    os.makedirs(outdir, exist_ok=True)
    cl = Client(root)
    key, parts = cl.armor_parts(item_id, body)
    print('item %d body %d -> armor.dbc key %d -> %s' % (item_id, body, key, parts))
    if not parts:
        return 1
    p = parts[0]
    mpath, tpath = cl.obj.get(p['mesh']), cl.tex.get(p['tex'])
    print('  mesh %d -> %s\n  texture %d -> %s' % (p['mesh'], mpath, p['tex'], tpath))
    mbuf, mwhere = cl.read(mpath)
    tbuf, twhere = cl.read(tpath) if tpath else (None, None)
    print('  mesh from %s (%s bytes), texture from %s (%s bytes)' % (mwhere, mbuf and len(mbuf), twhere, tbuf and len(tbuf)))
    phys, motis, other = parse_c3(mbuf)
    base = os.path.join(outdir, '%d_body%d' % (item_id, body))
    texname = os.path.basename(base) + '.png'
    if tbuf:
        w, h, px = dds.decode_dds(tbuf)
        dds.write_png(os.path.join(outdir, texname), w, h, px)
        print('  texture %dx%d -> %s' % (w, h, texname))
    with open(base + '.mtl', 'w') as f:
        f.write('newmtl garment\nKd 1 1 1\nmap_Kd %s\n' % texname)
    with open(base + '.obj', 'w') as f:
        f.write('# Conquer Online item %d (body %d) exported by AssetStudio c3model.py\n' % (item_id, body))
        f.write('mtllib %s\n' % os.path.basename(base + '.mtl'))
        vo = 1
        for i, ph in enumerate(phys):
            if not ph['name'].startswith('v_body') and len(phys) > 1 and ph['nv'] < 30:
                continue  # helper boxes (v_armet / v_l_weapon / v_r_weapon attachment markers)
            moti = motis[i] if i < len(motis) else None
            pts = bind_pose(ph, moti)
            f.write('o %s\nusemtl garment\n' % ph['name'])
            for x, y, z in pts:
                f.write('v %.4f %.4f %.4f\n' % (x, -z, y))  # C3 is Z-down/Y-forward -> OBJ Y-up
            for v in ph['verts']:
                f.write('vt %.5f %.5f\n' % (v['uv'][0], 1.0 - v['uv'][1]))
            for a, b, c in ph['tris']:
                f.write('f %d/%d %d/%d %d/%d\n' % (a + vo, a + vo, b + vo, b + vo, c + vo, c + vo))
            print('  %-12s verts %d+%d tris %d+%d bones(moti) %s' % (ph['name'], ph['nv'], ph['av'], ph['ntri'], ph['atri'],
                                                                     moti and moti['bones']))
            vo += len(pts)
    print('  wrote', base + '.obj')
    return 0


if __name__ == '__main__':
    iid = int(sys.argv[1]) if len(sys.argv) > 1 else 181305
    body = int(sys.argv[2]) if len(sys.argv) > 2 else 1
    sys.exit(export(iid, body, sys.argv[3] if len(sys.argv) > 3 else None))
