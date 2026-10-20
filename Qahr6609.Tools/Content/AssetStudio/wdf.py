"""TQ WDF archive reader (c3.wdf / data.wdf of the Conquer Online client).

File layout (little endian):
    header : char magic[4] = 'PFDW', uint32 count, uint32 index_offset
    index  : count * { uint32 uid, uint32 offset, uint32 size, uint32 space }
File names are not stored; each entry is addressed by uid = string_id(path).

string_id() is the classic "WDF" string hash (cloudwu's Westward Journey tools; TQ uses the same one):
  1. normalise the path: lower case, '\\' -> '/'  (e.g. 'data/playerface/253.dds')
     (lower case + '/' is the only variant that matched: 28 hits on loose client files vs 0 for the
      backslash / original-case variants; every name that misses is a loose file that is not packed)
  2. copy it into uint32 m[70] (zero padded, max 256 bytes), find the first zero dword i,
     m[i] = 0x9BE74448, m[i+1] = 0x66F42C48
  3. v = 0xF4FA8928, esi = 0x37A8470E, edi = 0x7758B42B; for every dword: rol v,1; ebx = 0x267B0B11 ^ v;
     esi ^= m; edi ^= m; two 32x32->64 multiplies with masks 0x2040801/0xBFEF7FDF and 0x804021/0x7DFEFBFF and
     add-with-carry folding (the original x86 asm is translated exactly below); result = esi ^ edi.
verify() checks a few names that are known to be packed.
"""
import struct, os

M32 = 0xFFFFFFFF


def string_id(name):
    s = name.replace(chr(92), '/').lower().encode('latin1', 'replace')[:256]
    raw = s + b'\0' * (280 - len(s))
    m = list(struct.unpack('<70I', raw))
    i = 0
    while i < 64 and m[i]:
        i += 1
    m[i] = 0x9BE74448; i += 1
    m[i] = 0x66F42C48; i += 1
    v = 0xF4FA8928
    esi = 0x37A8470E
    edi = 0x7758B42B
    for ecx in range(i):
        v = ((v << 1) | (v >> 31)) & M32
        ebx = 0x267B0B11 ^ v
        eax = m[ecx]
        esi ^= eax
        edi ^= eax
        edx = ((ebx + edi) & M32 | 0x2040801) & 0xBFEF7FDF
        p = esi * edx                      # mul edx  -> edx:eax
        lo, hi = p & M32, p >> 32
        t = lo + hi                        # adc eax,edx (CF from mul is 0 here: mul sets CF if hi!=0)
        cf = 1 if hi != 0 else 0           # mul sets CF=OF=1 when the high half is non-zero
        t = t + cf
        carry = t >> 32
        eax = t & M32
        eax = (eax + carry) & M32          # adc eax,0
        edx = ((ebx + esi) & M32 | 0x804021) & 0x7DFEFBFF
        esi = eax
        p = edi * edx
        lo, hi = p & M32, p >> 32
        hi2 = (hi + hi) & M32              # add edx,edx
        cf = (hi + hi) >> 32
        t = lo + hi2 + cf                  # adc eax,edx
        eax = t & M32
        if t >> 32:                        # jnc _skip / add eax,2
            eax = (eax + 2) & M32
        edi = eax
    return (esi ^ edi) & M32


class WDF:
    def __init__(self, path):
        self.path = path
        self.f = open(path, 'rb')
        magic, n, off = struct.unpack('<4sII', self.f.read(12))
        if magic != b'PFDW':
            raise ValueError('not a WDF file: %s' % path)
        self.f.seek(off)
        data = self.f.read(16 * n)
        self.index = {}
        for k in range(n):
            uid, o, size, space = struct.unpack_from('<4I', data, 16 * k)
            self.index[uid] = (o, size)

    def __contains__(self, name):
        return string_id(name) in self.index

    def read(self, name):
        e = self.index.get(string_id(name))
        if not e:
            return None
        self.f.seek(e[0])
        return self.f.read(e[1])

    def read_uid(self, uid):
        e = self.index.get(uid)
        if not e:
            return None
        self.f.seek(e[0])
        return self.f.read(e[1])


KNOWN = {  # names that are only inside the archives of the 6609 client (used to check the hash)
    'data.wdf': ['data/PlayerFace/253.dds', 'data/ItemMinIcon/080010.dds'],
    'c3.wdf': ['c3/texture/001137820.dds', 'c3/texture/002137320.dds'],
}


def verify(client):
    ok = True
    for arc, names in KNOWN.items():
        p = os.path.join(client, arc)
        if not os.path.exists(p):
            continue
        w = WDF(p)
        for n in names:
            hit = n in w
            ok &= hit
            print('  %-9s %-32s 0x%08X %s' % (arc, n, string_id(n), 'found' if hit else 'MISSING'))
    return ok


if __name__ == '__main__':
    import sys
    root = sys.argv[1] if len(sys.argv) > 1 else r'D:\Conquer Online 3.0'
    print('WDF hash check:', 'OK' if verify(root) else 'FAILED')
