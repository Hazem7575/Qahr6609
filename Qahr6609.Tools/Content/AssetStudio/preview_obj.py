"""Tiny software renderer for the stage-2 proof: textured OBJ -> PNG (orthographic, z-buffer, no lighting).
usage: python preview_obj.py model.obj texture.png out.png [yaw_degrees]"""
import sys, os, struct, math
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import dds
obj = sys.argv[1]; texpng = sys.argv[2]; out = sys.argv[3]; yaw = float(sys.argv[4]) if len(sys.argv) > 4 else 0
import zlib
def read_png(p):
    b = open(p, 'rb').read(); o = 8; idat = b''; w = h = 0
    while o < len(b):
        n, t = struct.unpack_from('>I4s', b, o); d = b[o + 8:o + 8 + n]
        if t == b'IHDR': w, h = struct.unpack_from('>II', d)
        if t == b'IDAT': idat += d
        o += 12 + n
    raw = zlib.decompress(idat); st = w * 4; px = bytearray(w * h * 4); prev = bytearray(st)
    for y in range(h):
        f = raw[y * (st + 1)]; line = bytearray(raw[y * (st + 1) + 1:(y + 1) * (st + 1)])
        for i in range(st):
            a = line[i - 4] if i >= 4 else 0; up = prev[i]; c = prev[i - 4] if i >= 4 else 0
            if f == 1: line[i] = (line[i] + a) & 255
            elif f == 2: line[i] = (line[i] + up) & 255
            elif f == 3: line[i] = (line[i] + (a + up) // 2) & 255
            elif f == 4:
                p_ = a + up - c; pa, pb, pc = abs(p_ - a), abs(p_ - up), abs(p_ - c)
                line[i] = (line[i] + (a if pa <= pb and pa <= pc else (up if pb <= pc else c))) & 255
        px[y * st:(y + 1) * st] = line; prev = line
    return w, h, px
tw, th, tp = read_png(texpng)
V = []; VT = []; F = []
for l in open(obj):
    s = l.split()
    if not s: continue
    if s[0] == 'v': V.append(tuple(map(float, s[1:4])))
    elif s[0] == 'vt': VT.append(tuple(map(float, s[1:3])))
    elif s[0] == 'f': F.append([tuple(int(x) - 1 for x in p.split('/')) for p in s[1:4]])
ca, sa = math.cos(math.radians(yaw)), math.sin(math.radians(yaw))
V = [(x * ca + z * sa, y, -x * sa + z * ca) for x, y, z in V]
xs = [v[0] for v in V]; ys = [v[1] for v in V]
W = H = 512; pad = 20
sc = min((W - 2 * pad) / (max(xs) - min(xs)), (H - 2 * pad) / (max(ys) - min(ys)))
cx = (max(xs) + min(xs)) / 2; cy = (max(ys) + min(ys)) / 2
P = [(W / 2 + (x - cx) * sc, H / 2 - (y - cy) * sc, z) for x, y, z in V]
img = bytearray([40, 44, 56, 255] * (W * H)); zb = [-1e9] * (W * H)
for f in F:
    (a, ta), (b, tb), (c, tc) = f
    x0, y0, z0 = P[a]; x1, y1, z1 = P[b]; x2, y2, z2 = P[c]
    den = (y1 - y2) * (x0 - x2) + (x2 - x1) * (y0 - y2)
    if abs(den) < 1e-9: continue
    for yy in range(max(0, int(min(y0, y1, y2))), min(H - 1, int(max(y0, y1, y2)) + 1)):
        for xx in range(max(0, int(min(x0, x1, x2))), min(W - 1, int(max(x0, x1, x2)) + 1)):
            px_, py_ = xx + .5, yy + .5
            l0 = ((y1 - y2) * (px_ - x2) + (x2 - x1) * (py_ - y2)) / den
            l1 = ((y2 - y0) * (px_ - x2) + (x0 - x2) * (py_ - y2)) / den
            l2 = 1 - l0 - l1
            if l0 < 0 or l1 < 0 or l2 < 0: continue
            z = l0 * z0 + l1 * z1 + l2 * z2
            i = yy * W + xx
            if z <= zb[i]: continue
            u = l0 * VT[ta][0] + l1 * VT[tb][0] + l2 * VT[tc][0]
            v = l0 * VT[ta][1] + l1 * VT[tb][1] + l2 * VT[tc][1]
            tx = int((u % 1) * tw) % tw; ty = int(((1 - v) % 1) * th) % th
            k = (ty * tw + tx) * 4
            if tp[k + 3] < 40: continue
            zb[i] = z
            img[i * 4:i * 4 + 4] = bytes((tp[k], tp[k + 1], tp[k + 2], 255))
dds.write_png(out, W, H, bytes(img))
print('ok', len(V), len(F))
