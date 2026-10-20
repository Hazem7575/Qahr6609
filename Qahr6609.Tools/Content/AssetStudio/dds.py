"""Minimal DDS decoder + PNG writer (pure Python, no Pillow needed).

Supports what the TQ client uses for icons/faces/textures:
  * DXT1 (incl. 1-bit alpha), DXT2/DXT3 (explicit 4-bit alpha), DXT4/DXT5 (interpolated alpha)
  * uncompressed RGB/RGBA described by bit masks (A8R8G8B8, X8R8G8B8, R8G8B8, R5G6B5,
    A1R5G5B5, X1R5G5B5, A4R4G4B4, A8, L8)
Only the first mip level is decoded.  Pure Python (about 7 ms per 64x64 icon).

decode_dds(bytes) -> (width, height, rgba_bytes)
write_png(path, width, height, rgba_bytes)
"""
import struct, zlib

np = None  # pure Python on purpose: no third-party package needed (Pillow/numpy are not required)


class DDSError(Exception):
    pass


def _expand565(c):
    r = (c >> 11) & 31
    g = (c >> 5) & 63
    b = c & 31
    return ((r << 3) | (r >> 2), (g << 2) | (g >> 4), (b << 3) | (b >> 2))


def _color_block(blk, off, dxt1):
    c0, c1, bits = struct.unpack_from('<HHI', blk, off)
    r0, g0, b0 = _expand565(c0)
    r1, g1, b1 = _expand565(c1)
    if c0 > c1 or not dxt1:
        pal = [(r0, g0, b0, 255), (r1, g1, b1, 255),
               ((2 * r0 + r1) // 3, (2 * g0 + g1) // 3, (2 * b0 + b1) // 3, 255),
               ((r0 + 2 * r1) // 3, (g0 + 2 * g1) // 3, (b0 + 2 * b1) // 3, 255)]
    else:
        pal = [(r0, g0, b0, 255), (r1, g1, b1, 255),
               ((r0 + r1) // 2, (g0 + g1) // 2, (b0 + b1) // 2, 255), (0, 0, 0, 0)]
    return [pal[(bits >> (2 * i)) & 3] for i in range(16)]


def _alpha_dxt3(blk, off):
    a = struct.unpack_from('<Q', blk, off)[0]
    return [((a >> (4 * i)) & 15) * 17 for i in range(16)]


def _alpha_dxt5(blk, off):
    a0, a1 = blk[off], blk[off + 1]
    bits = int.from_bytes(blk[off + 2:off + 8], 'little')
    if a0 > a1:
        pal = [a0, a1] + [((7 - i) * a0 + i * a1) // 7 for i in range(1, 7)]
    else:
        pal = [a0, a1] + [((5 - i) * a0 + i * a1) // 5 for i in range(1, 5)] + [0, 255]
    return [pal[(bits >> (3 * i)) & 7] for i in range(16)]


def _decode_dxt_py(data, w, h, kind):
    out = bytearray(w * h * 4)
    bw, bh = max(1, (w + 3) // 4), max(1, (h + 3) // 4)
    bsize = 8 if kind == 1 else 16
    need = bw * bh * bsize
    if len(data) < need:
        raise DDSError('truncated DXT data')
    off = 0
    for by in range(bh):
        for bx in range(bw):
            if kind == 1:
                px = _color_block(data, off, True)
            else:
                al = _alpha_dxt3(data, off) if kind == 3 else _alpha_dxt5(data, off)
                px = _color_block(data, off + 8, False)
                px = [(p[0], p[1], p[2], al[i]) for i, p in enumerate(px)]
            off += bsize
            for j in range(4):
                y = by * 4 + j
                if y >= h:
                    break
                for i in range(4):
                    x = bx * 4 + i
                    if x >= w:
                        break
                    o = (y * w + x) * 4
                    out[o:o + 4] = bytes(px[j * 4 + i])
    return bytes(out)


def _mask_shift(m):
    if not m:
        return 0, 0
    s = 0
    while not (m >> s) & 1:
        s += 1
    bits = 0
    while (m >> (s + bits)) & 1:
        bits += 1
    return s, bits


def _decode_masked(data, w, h, bpp, rm, gm, bm, am, flags):
    bpp_b = bpp // 8
    pitch = w * bpp_b
    if len(data) < pitch * h:
        raise DDSError('truncated RGB data')
    chans = [_mask_shift(m) for m in (rm, gm, bm, am)]
    lum = bool(flags & 0x20000) and not (gm or bm)
    has_alpha = bool(am) and bool(flags & 0x1)
    out = bytearray(w * h * 4)
    for i in range(w * h):
        v = int.from_bytes(data[i * bpp_b:(i + 1) * bpp_b], 'little')
        px = []
        for k, (s, b) in enumerate(chans):
            px.append((255 if k == 3 else 0) if b == 0 else ((v >> s) & ((1 << b) - 1)) * 255 // ((1 << b) - 1))
        if lum:
            px[1] = px[2] = px[0]
        if not has_alpha:
            px[3] = 255
        out[i * 4:i * 4 + 4] = bytes(px)
    return bytes(out)


def decode_dds(buf):
    if len(buf) < 128 or buf[:4] != b'DDS ':
        raise DDSError('not a DDS file')
    h, w = struct.unpack_from('<II', buf, 12)
    pf_flags, fourcc, bpp, rm, gm, bm, am = struct.unpack_from('<I4s5I', buf, 80)
    data = buf[128:]
    if w <= 0 or h <= 0 or w > 8192 or h > 8192:
        raise DDSError('bad size')
    if pf_flags & 0x4:
        kind = {b'DXT1': 1, b'DXT2': 3, b'DXT3': 3, b'DXT4': 5, b'DXT5': 5}.get(fourcc)
        if not kind:
            raise DDSError('unsupported fourcc %r' % fourcc)
        return w, h, _decode_dxt_py(data, w, h, kind)
    return w, h, _decode_masked(data, w, h, bpp, rm, gm, bm, am, pf_flags)


def write_png(path, w, h, rgba):
    raw = bytearray()
    stride = w * 4
    for y in range(h):
        raw.append(0)
        raw += rgba[y * stride:(y + 1) * stride]

    def chunk(t, d):
        c = struct.pack('>I', len(d)) + t + d
        return c + struct.pack('>I', zlib.crc32(t + d) & 0xffffffff)
    png = b'\x89PNG\r\n\x1a\n' + chunk(b'IHDR', struct.pack('>IIBBBBB', w, h, 8, 6, 0, 0, 0)) + \
        chunk(b'IDAT', zlib.compress(bytes(raw), 6)) + chunk(b'IEND', b'')
    with open(path, 'wb') as f:
        f.write(png)


def decode_tga(buf):
    """uncompressed (type 2/3) and RLE (type 10/11) TGA, 8/16/24/32 bpp -> (w, h, rgba)"""
    idlen, cmtype, itype = buf[0], buf[1], buf[2]
    cmlen, cmbits = struct.unpack_from('<H', buf, 5)[0], buf[7]
    w, h, bpp, desc = struct.unpack_from('<HHBB', buf, 12)
    if itype not in (2, 3, 10, 11) or bpp not in (8, 16, 24, 32) or not w or not h:
        raise DDSError('unsupported image (not DDS/TGA)')
    pos = 18 + idlen + (cmlen * ((cmbits + 7) // 8) if cmtype else 0)
    bp = bpp // 8
    npx = w * h
    if itype in (2, 3):
        raw = buf[pos:pos + npx * bp]
    else:
        raw = bytearray()
        while len(raw) < npx * bp and pos < len(buf):
            hdr = buf[pos]; pos += 1
            cnt = (hdr & 0x7F) + 1
            if hdr & 0x80:
                raw += buf[pos:pos + bp] * cnt; pos += bp
            else:
                raw += buf[pos:pos + bp * cnt]; pos += bp * cnt
    if len(raw) < npx * bp:
        raise DDSError('truncated TGA')
    out = bytearray(npx * 4)
    top = bool(desc & 0x20)
    for y in range(h):
        sy = y if top else h - 1 - y
        for x in range(w):
            i = (sy * w + x) * bp
            o = (y * w + x) * 4
            if bp == 4:
                b, g, r, a = raw[i], raw[i + 1], raw[i + 2], raw[i + 3]
            elif bp == 3:
                b, g, r, a = raw[i], raw[i + 1], raw[i + 2], 255
            elif bp == 2:
                v = raw[i] | (raw[i + 1] << 8)
                r, g, b = ((v >> 10) & 31) * 255 // 31, ((v >> 5) & 31) * 255 // 31, (v & 31) * 255 // 31
                a = 255
            else:
                r = g = b = raw[i]; a = 255
            out[o:o + 4] = bytes((r, g, b, a))
    if bp == 4 and not any(out[3::4]):  # alpha channel present but empty -> treat as opaque
        out[3::4] = bytes([255]) * npx
    return w, h, bytes(out)
