#!/usr/bin/env python
"""AssetStudio stage 1 - extract items / icons / faces / NPCs / monsters from the official
Conquer Online 6609 client into an offline viewer (out/index.html + out/data.json).

usage:
    python extract.py            # uses assetstudio.ini (client path), skips PNGs that already exist
    python extract.py --pick     # choose the client folder with a dialog (saved to assetstudio.ini)
    python extract.py --force    # re-convert every image
    python extract.py --client "D:\\Conquer Online 3.0" --out out

The client folder is only READ, never written.
"""
import argparse, configparser, json, os, re, shutil, sys, time

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
import dds  # noqa: E402
from wdf import WDF  # noqa: E402

try:
    sys.stdout.reconfigure(encoding='utf-8', errors='replace')
except Exception:
    pass

INI = os.path.join(HERE, 'assetstudio.ini')
DEFAULT_CLIENT = r'D:\Conquer Online 3.0'


# ---------------------------------------------------------------- config / args
def load_config():
    cp = configparser.ConfigParser()
    cp.read(INI, encoding='utf-8')
    if not cp.has_section('paths'):
        cp.add_section('paths')
    return cp


def save_config(cp):
    with open(INI, 'w', encoding='utf-8') as f:
        cp.write(f)


def pick_folder(initial):
    import tkinter as tk
    from tkinter import filedialog
    root = tk.Tk()
    root.withdraw()
    p = filedialog.askdirectory(title='Conquer client folder / فولدر الكلاينت', initialdir=initial if os.path.isdir(initial) else None)
    root.destroy()
    return p


# ---------------------------------------------------------------- client file helpers
def _dat_key():
    s, k = 0x2537, []
    for _ in range(128):
        s = (s * 214013 + 2531011) & 0xFFFFFFFF
        k.append(((s >> 16) & 0x7FFF) % 256)
    return k


DAT_KEY = _dat_key()


def looks_text(b):
    n = min(len(b), 1500)
    if n == 0:
        return True
    good = sum(1 for c in b[:n] if 32 <= c < 127 or c in (9, 10, 13) or c >= 0x80)
    ctrl = sum(1 for c in b[:n] if c < 32 and c not in (9, 10, 13))
    return ctrl == 0 and good / n > 0.85


def dat_decrypt(b):
    """TQ .dat cipher: MSVC rand() keystream seeded 0x2537, XOR then rotate right by (i & 7)."""
    k = DAT_KEY
    out = bytearray(len(b))
    for i, c in enumerate(b):
        n = c ^ k[i & 127]
        r = i & 7
        out[i] = ((n << (8 - r)) | (n >> r)) & 0xFF
    return bytes(out)


def decode_text(b):
    if b[:3] == b'\xef\xbb\xbf':
        b = b[3:]
    try:
        return b.decode('utf-8')
    except UnicodeDecodeError:
        return b.decode('gbk', 'replace')


def read_client_text(path):
    if not os.path.exists(path):
        return None
    b = open(path, 'rb').read()
    if not looks_text(b[:1500]):
        b = dat_decrypt(b)
    return decode_text(b)


def parse_ini(text):
    """returns list of (section, dict) preserving order (duplicate sections kept)."""
    out, cur, kv = [], None, None
    for line in text.splitlines():
        s = line.strip()
        if not s or s.startswith(';') or s.startswith('//'):
            continue
        if s.startswith('[') and ']' in s:
            cur = s[1:s.index(']')]
            kv = {}
            out.append((cur, kv))
        elif kv is not None and '=' in s:
            k, v = s.split('=', 1)
            kv.setdefault(k.strip(), v.strip())
    return out


def parse_ani(text):
    """ani text index -> {section: [frame paths]}"""
    res = {}
    for sec, kv in parse_ini(text):
        n = kv.get('FrameAmount')
        frames = []
        try:
            n = int(n)
        except (TypeError, ValueError):
            n = 1
        for i in range(max(1, n)):
            f = kv.get('Frame%d' % i)
            if f:
                frames.append(f.replace('\\', '/'))
        if frames and sec not in res:
            res[sec] = frames
    return res


def nice(s):
    return (s or '').replace('~', ' ').replace('`', "'").strip()


# ---------------------------------------------------------------- image source (loose -> data.wdf -> c3.wdf)
class Source:
    def __init__(self, client):
        self.client = client
        self.wdfs = []
        for n in ('data.wdf', 'c3.wdf'):
            p = os.path.join(client, n)
            if os.path.exists(p):
                try:
                    self.wdfs.append((n, WDF(p)))
                except Exception as e:
                    print('  ! cannot open %s: %s' % (n, e))
        self.stats = {'loose': 0, 'data.wdf': 0, 'c3.wdf': 0, 'missing': 0}

    def read(self, rel):
        p = os.path.join(self.client, rel.replace('/', os.sep))
        if os.path.exists(p):
            self.stats['loose'] += 1
            return open(p, 'rb').read(), 'loose'
        for n, w in self.wdfs:
            b = w.read(rel)
            if b is not None:
                self.stats[n] += 1
                return b, n
        self.stats['missing'] += 1
        return None, None


def convert(buf, dst_noext):
    """buf = file bytes (DDS / TGA / JPEG). returns written relative extension or None"""
    if buf[:3] == b'\xff\xd8\xff':
        with open(dst_noext + '.jpg', 'wb') as f:
            f.write(buf)
        return '.jpg'
    if buf[:8] == b'\x89PNG\r\n\x1a\n':
        with open(dst_noext + '.png', 'wb') as f:
            f.write(buf)
        return '.png'
    if buf[:4] == b'DDS ':
        w, h, px = dds.decode_dds(buf)
    else:
        w, h, px = dds.decode_tga(buf)
    dds.write_png(dst_noext + '.png', w, h, px)
    return '.png'


class ImageStore:
    """converts each client image once; maps client frame path -> out relative path"""

    def __init__(self, src, outdir, force):
        self.src, self.outdir, self.force = src, outdir, force
        self.done = {}
        self.used = set()
        self.errors = []
        self.converted = 0
        self.reused = 0

    def get(self, rel, group):
        key = rel.lower()
        if key in self.done:
            return self.done[key]
        stem = re.sub(r'[^A-Za-z0-9_.-]+', '_', os.path.splitext(rel.split('/')[-1])[0])
        sub = os.path.join('img', group)
        os.makedirs(os.path.join(self.outdir, sub), exist_ok=True)
        # unique name inside the group (different client folders can share a file name)
        name, k = stem, 1
        while (group, name.lower()) in self.used:
            name = '%s_%d' % (stem, k)
            k += 1
        self.used.add((group, name.lower()))
        dst = os.path.join(self.outdir, sub, name)
        if not self.force:
            for ext in ('.png', '.jpg'):
                if os.path.exists(dst + ext):
                    r = (sub + '/' + os.path.basename(dst) + ext).replace(os.sep, '/')
                    self.done[key] = r
                    self.reused += 1
                    return r
        buf, where = self.src.read(rel)
        if buf is None:
            self.done[key] = None
            return None
        try:
            ext = convert(buf, dst)
        except Exception as e:
            self.errors.append('%s (%s): %s' % (rel, where, e))
            self.done[key] = None
            return None
        self.converted += 1
        r = (sub + '/' + os.path.basename(dst) + ext).replace(os.sep, '/')
        self.done[key] = r
        return r


# ---------------------------------------------------------------- categories
WEAPON_TYPES = {
    '410': 'Blade', '420': 'Sword', '421': 'Backsword', '422': 'Novelty weapon', '430': 'Hook', '440': 'Whip',
    '450': 'Axe', '460': 'Hammer', '480': 'Club', '481': 'Scepter', '490': 'Dagger', '500': 'Bow',
    '510': 'Glaive', '511': 'Scythe', '530': 'Poleaxe', '540': 'Longhammer', '560': 'Spear', '561': 'Wand',
    '562': 'Pickaxe / tool', '580': 'Halbert', '601': 'Katana', '610': 'Prayer Beads', '611': 'Rapier',
    '612': 'Pistol', '613': 'Throwing Knife', '614': 'Cross Saber (epic)', '616': 'Claw (epic)',
    '617': 'Nunchaku', '619': 'Hossu', '620': 'Backsword (epic)', '622': 'Pillar (epic)', '624': 'Fist',
    '626': 'Handfan', '900': 'Shield',
}
HEADGEAR = {'111': 'Warrior Helmet', '112': 'Ninja Veil', '113': 'Archer Hat', '114': 'Taoist Cap',
            '115': 'Event Cap', '116': 'Event Cap', '117': 'Earring', '118': 'Trojan Coronet', '123': 'Ninja Hood',
            '141': 'Headband', '142': 'Archer Plume', '143': 'Monk Headband', '144': 'Pirate Headgear',
            '145': 'Pirate Hood', '148': 'Dragon-Warrior Headband', '170': 'Windwalker Hat'}
ARMOR = {'101': 'Windwalker Windrobe', '130': 'Trojan Armor', '131': 'Warrior Armor', '132': 'Coat',
         '133': 'Archer Coat', '134': 'Taoist Robe', '135': 'Ninja Vest', '136': 'Monk Frock',
         '137': 'Event Coat', '138': 'Dragon-Warrior Suit', '139': 'Pirate Armor'}
JEWELRY = {'120': 'Necklace', '121': 'Bag', '150': 'Ring', '151': 'Heavy Ring', '152': 'Bracelet', '160': 'Boots'}
ACCESSORY = {'350': '2-handed accessory', '360': '1-handed accessory', '370': 'Bow accessory', '380': 'Shield accessory'}
SOUL = {'800': 'Weapon soul', '801': 'Weapon soul', '820': 'Headgear soul', '821': 'Necklace soul',
        '822': 'Armor soul', '823': 'Ring soul', '824': 'Boots soul'}

CATEGORIES = [  # key, English, Arabic (order used by the viewer)
    ('garment', 'Garments', 'الجارمنت (ملابس)'),
    ('weapon', 'Weapons', 'الأسلحة'),
    ('headgear', 'Headgear', 'الخوذ والقبعات'),
    ('armor', 'Armor', 'الدروع'),
    ('mount', 'Mounts', 'الخيول ودروعها'),
    ('accessory', 'Accessories', 'إكسسوارات السلاح'),
    ('jewelry', 'Jewelry & Boots', 'مجوهرات وأحذية'),
    ('wing', 'Wings', 'الأجنحة'),
    ('talisman', 'Talismans', 'التعويذات'),
    ('gem', 'Gems', 'الجواهر'),
    ('soul', 'Souls', 'الأرواح'),
    ('material', 'Materials', 'الخامات'),
    ('potion', 'Potions & Scrolls', 'أدوية وسكرولات'),
    ('pack', 'Packs & Gifts', 'باكدجات وهدايا'),
    ('quest', 'Quest & Event Items', 'أيتمات مهام وإيفنتات'),
    ('other', 'Others', 'أخرى'),
]


def categorize(iid, typename):
    s = str(iid)
    p3 = s[:3]
    t = nice(typename)
    tl = t.lower()
    if len(s) == 6:
        if '181' <= p3 <= '194':
            return 'garment', 'Garment'
        if p3 in WEAPON_TYPES:
            return 'weapon', WEAPON_TYPES[p3]
        if p3 in HEADGEAR:
            return 'headgear', HEADGEAR[p3]
        if p3 in ARMOR:
            return 'armor', ARMOR[p3]
        if p3 in JEWELRY:
            return 'jewelry', JEWELRY[p3]
        if p3 in ACCESSORY:
            return 'accessory', ACCESSORY[p3]
        if p3 == '200':
            return 'mount', 'Mount armor'
        if p3 == '300':
            return 'mount', 'Steed'
        if p3 == '204':
            return 'wing', t or 'Wing'
        if p3 in ('201', '202', '203'):
            return 'talisman', t or 'Talisman'
        if p3 == '700':
            return 'gem', 'Gem'
        if p3 in SOUL:
            return 'soul', SOUL[p3]
        if p3 == '730':
            return 'material', '+Stone'
        if p3 == '725':
            return 'other', 'Book' if 'book' in tl else (t or 'Book')
        if p3 == '726':
            return 'other', 'Furniture'
        if p3 in ('751', '752', '753', '754', '755', '756', '757', '758'):
            return 'other', 'Flowers & gifts'
        if p3 == '780':
            return 'other', 'Point card'
    else:
        p4 = s[:4]
        if p4 in ('1000', '1001', '1002', '1003', '1004'):
            return 'potion', t or 'Potion'
        if p4 == '1060':
            return 'potion', 'Scroll'
        if p4 == '1050':
            return 'weapon', 'Arrow'
        if p4 in ('1072',):
            return 'material', 'Ore'
        if p4 in ('1088', '1200', '1090', '1091', '1100', '3009'):
            return 'material', t or 'Rare'
        if p4 == '2100':
            return 'talisman', t or 'Talisman'
        if s.startswith('5000'):
            return 'weapon', 'Arrow'
    if 'wing' in tl:
        return 'wing', t
    if tl in ('gem',):
        return 'gem', 'Gem'
    if 'pack' in tl or 'gift' in tl or 'box' in tl or 'bag' == tl or 'chest' in tl:
        return 'pack', t or 'Pack'
    if s[:1] in ('7', '3') or 'quest' in tl:
        return 'quest', t or 'Quest item'
    return 'other', t or 'Other'


# ---------------------------------------------------------------- main extraction
def progress(i, n, label, t0, every=500):
    if i % every == 0 or i == n:
        el = time.time() - t0
        eta = (el / i * (n - i)) if i else 0
        print('\r  %-12s %6d / %-6d  %5.1f%%  %4.0fs  eta %4.0fs ' % (label, i, n, 100.0 * i / max(1, n), el, eta), end='', flush=True)
        if i == n:
            print()


def load_professions(client):
    t = read_client_text(os.path.join(client, 'ini', 'ProfessionalName.ini')) or ''
    m = {}
    for line in t.splitlines():
        c = line.split(',')
        if len(c) >= 2 and c[0].strip().isdigit():
            m[int(c[0])] = nice(c[1])
    return m


def load_items(client):
    rows = {}
    src = {}
    for fn in ('itemtype.dat', 'ItemtypeSub.dat'):
        t = read_client_text(os.path.join(client, 'ini', fn))
        if not t:
            print('  ! %s not found' % fn)
            continue
        n = 0
        for line in t.splitlines():
            line = line.strip()
            if not line or not line[0].isdigit() or '@@' not in line:
                continue
            c = line.split('@@')
            if c[0] in rows:
                continue
            rows[c[0]] = c
            src[c[0]] = fn
            n += 1
        print('  %-16s %6d rows' % (fn, n))
    return rows, src


def col(c, i, default=''):
    return c[i] if i < len(c) else default


def to_int(v, d=0):
    try:
        return int(v)
    except (TypeError, ValueError):
        return d


def icon_keys(iid):
    """ItemMinIcon.ani section names the client tries for an item type id.
    Equipment with a colour digit (hundreds) is stored as  colour*1000000 + id  (colours 3..9);
    everything else as Item<id>."""
    yield 'Item%d' % iid
    if iid < 1000000:
        for c in (3, 4, 5, 6, 7, 8, 9, 2, 1):
            yield 'Item%d' % (c * 1000000 + iid)
        # last digit = quality; a quality without its own entry looks like a sibling quality
        b = iid - iid % 10
        for q in (b, b + 3, b + 4, b + 5, b + 6, b + 7, b + 8, b + 9):
            if q == iid:
                continue
            yield 'Item%d' % q
            for c in (3, 4, 5, 6, 7, 8, 9):
                yield 'Item%d' % (c * 1000000 + q)


def main():
    ap = argparse.ArgumentParser(description='AssetStudio extractor')
    ap.add_argument('--pick', action='store_true', help='choose the client folder with a dialog')
    ap.add_argument('--client', help='client folder (overrides assetstudio.ini)')
    ap.add_argument('--out', help='output folder (default: out next to this script)')
    ap.add_argument('--force', action='store_true', help='re-convert all images')
    a = ap.parse_args()

    cp = load_config()
    client = a.client or cp.get('paths', 'client', fallback=DEFAULT_CLIENT)
    if a.pick:
        p = pick_folder(client)
        if not p:
            print('cancelled')
            return 1
        client = os.path.normpath(p)
    if not os.path.isfile(os.path.join(client, 'ini', 'itemtype.dat')):
        print('ERROR: "%s" does not look like a Conquer client (ini\\itemtype.dat missing).' % client)
        print('       run:  python extract.py --pick')
        return 2
    out = os.path.abspath(a.out or cp.get('paths', 'out', fallback=os.path.join(HERE, 'out')))
    cp.set('paths', 'client', client)
    if not cp.has_option('paths', 'out'):
        cp.set('paths', 'out', 'out')
    save_config(cp)
    if not os.path.isabs(out):
        out = os.path.join(HERE, out)
    os.makedirs(out, exist_ok=True)

    T0 = time.time()
    print('AssetStudio extractor')
    print('  client : %s' % client)
    print('  output : %s' % out)
    print('  decoder: %s' % ('numpy' if dds.np is not None else 'pure python'))

    print('[1/6] archives')
    src = Source(client)
    for n, w in src.wdfs:
        print('  %-10s %6d entries' % (n, len(w.index)))
    store = ImageStore(src, out, a.force)

    print('[2/6] ani indexes')
    anis = {}
    for name in ('ItemMinIcon.Ani', 'PlayerFace.Ani', 'NpcFace.Ani', 'Control.Ani'):
        p = os.path.join(client, 'ani', name)
        t = read_client_text(p)
        anis[name] = parse_ani(t) if t else {}
        print('  %-16s %6d sections' % (name, len(anis[name])))
    item_ani = anis['ItemMinIcon.Ani']

    print('[3/6] items')
    profs = load_professions(client)
    rows, rowsrc = load_items(client)
    items = []
    missing = []
    t0 = time.time()
    ids = sorted(rows, key=lambda x: int(x))
    for i, sid in enumerate(ids, 1):
        c = rows[sid]
        iid = int(sid)
        cat, sub = categorize(iid, col(c, 53))
        key = next((k for k in icon_keys(iid) if k in item_ani), None)
        icon = None
        frame = None
        if key:
            frame = item_ani[key][0]
            icon = store.get(frame, 'items')
        prof = to_int(col(c, 2))
        it = {
            'id': iid,
            'name': nice(col(c, 1)),
            'cat': cat,
            'sub': sub,
            'type': nice(col(c, 53)),
            'lvl': to_int(col(c, 4)),
            'sex': to_int(col(c, 5)),
            'prof': prof,
            'profName': profs.get(prof, '') if prof else '',
            'profLv': to_int(col(c, 3)),
            'cp': to_int(col(c, 37)),
            'cpBound': to_int(col(c, 38)),
            'gold': to_int(col(c, 12)),
            'desc': nice(col(c, 54)) if nice(col(c, 54)).lower() != 'none' else '',
            'icon': icon,
            'ani': key,
            'src': rowsrc.get(sid),
            'stats': {k: to_int(col(c, j)) for k, j in (
                ('str', 6), ('agi', 7), ('maxAtk', 14), ('minAtk', 15), ('def', 16), ('dodge', 18),
                ('hp', 19), ('mp', 20), ('dura', 22), ('mAtk', 30), ('mDef', 31), ('range', 32),
                ('crit', 40), ('sCrit', 41), ('immunity', 42), ('pen', 43), ('block', 44),
                ('breakth', 45), ('counter', 46), ('stack', 47), ('sort', 59)) if to_int(col(c, j))},
        }
        items.append(it)
        if not icon:
            missing.append((iid, it['name'], cat, key, frame))
        progress(i, len(ids), 'items', t0)

    print('[4/6] faces + wardrobe')
    faces = {'player': [], 'npc': []}
    for kind, name in (('player', 'PlayerFace.Ani'), ('npc', 'NpcFace.Ani')):
        secs = anis[name]
        keys = [k for k in secs if re.match(r'^Face\d+$', k)]
        keys.sort(key=lambda k: int(k[4:]))
        t0 = time.time()
        for i, k in enumerate(keys, 1):
            img = store.get(secs[k][0], 'faces_' + kind)
            faces[kind].append({'id': int(k[4:]), 'img': img, 'file': secs[k][0]})
            progress(i, len(keys), kind + ' faces', t0, 50)
    wardrobe = []
    st = read_client_text(os.path.join(client, 'ini', 'Storage.ini')) or ''
    ctrl = anis['Control.Ani']
    for sec, kv in parse_ini(st):
        if '-' in sec and kv.get('ID'):
            a_sec = kv.get('AniSection', '')
            kind = 'Wing' if a_sec.startswith('WingStor') else ('Title' if a_sec.startswith('TitleStor') else 'Other')
            img = None
            if a_sec in ctrl:
                img = store.get(ctrl[a_sec][0], 'wardrobe')
            intro = kv.get('Intro', '').replace(chr(92) + 'n', chr(10)).split(chr(10))  # literal "\n" in Storage.ini
            wardrobe.append({'id': to_int(kv.get('ID')), 'kind': kind, 'name': nice(intro[0]),
                             'intro': nice(' '.join(intro[1:])),
                             'how': nice(kv.get('GetWayText3', kv.get('GetWayText1', kv.get('GetWayText2', '')))),
                             'ani': a_sec, 'img': img})
    print('  wardrobe entries: %d' % len(wardrobe))

    print('[5/6] npcs + monsters')
    npcs = []
    nt = read_client_text(os.path.join(client, 'ini', 'npc.ini')) or ''
    for sec, kv in parse_ini(nt):
        m = re.match(r'^NpcType(\d+)$', sec)
        if m and 'Name' in kv:
            npcs.append({'type': int(m.group(1)), 'name': nice(kv.get('Name')), 'look': kv.get('SimpleObjID', ''),
                         'zoom': to_int(kv.get('ZoomPercent', 100), 100), 'note': nice(kv.get('Note', '')),
                         'motion': kv.get('StandByMotion', '')})
    monsters = []
    mt = read_client_text(os.path.join(client, 'ini', 'monster.dat')) or ''
    for sec, kv in parse_ini(mt):
        if 'TypeID' not in kv:
            continue
        monsters.append({'name': nice(sec), 'look': to_int(kv.get('TypeID')), 'level': to_int(kv.get('Level')),
                         'life': to_int(kv.get('MaxLife')), 'boss': to_int(kv.get('Boss')),
                         'zoom': to_int(kv.get('ZoomPercent', 100), 100), 'armet': kv.get('Armet', '0'),
                         'rw': kv.get('RWeapon', '0'), 'lw': kv.get('LWeapon', '0'), 'mount': kv.get('Mount', '0'),
                         'note': nice(kv.get('Note', ''))})
    print('  npc looks: %d   monsters: %d' % (len(npcs), len(monsters)))

    print('[6/6] writing data + viewer')
    cats = []
    for key, en, ar in CATEGORIES:
        its = [x for x in items if x['cat'] == key]
        subs = {}
        for x in its:
            subs[x['sub']] = subs.get(x['sub'], 0) + 1
        cats.append({'key': key, 'en': en, 'ar': ar, 'count': len(its),
                     'withIcon': sum(1 for x in its if x['icon']),
                     'subs': sorted(subs.items(), key=lambda kv: (-kv[1], kv[0]))})
    data = {
        'generated': time.strftime('%Y-%m-%d %H:%M'),
        'client': client,
        'categories': cats,
        'items': items,
        'faces': faces,
        'wardrobe': wardrobe,
        'npcs': npcs,
        'monsters': monsters,
        'stats': {'images': dict(src.stats), 'converted': store.converted, 'reused': store.reused,
                  'imageErrors': len(store.errors)},
    }
    with open(os.path.join(out, 'data.json'), 'w', encoding='utf-8') as f:
        json.dump(data, f, ensure_ascii=False, separators=(',', ':'))
    # file:// pages cannot fetch() JSON in Chrome/Edge -> the viewer loads the same data as a script
    with open(os.path.join(out, 'data.js'), 'w', encoding='utf-8') as f:
        f.write('window.ASSET_DATA=')
        json.dump(data, f, ensure_ascii=False, separators=(',', ':'))
        f.write(';\n')
    shutil.copyfile(os.path.join(HERE, 'viewer.html'), os.path.join(out, 'index.html'))
    shutil.copyfile(os.path.join(HERE, 'dresser.js'), os.path.join(out, 'dresser.js'))  # stage 2 (Dressing Room tab)

    with open(os.path.join(out, 'missing_icons.txt'), 'w', encoding='utf-8') as f:
        f.write('# items without a resolvable icon: id, name, category, ani section, frame file\n')
        for m in missing:
            f.write('%d\t%s\t%s\t%s\t%s\n' % (m[0], m[1], m[2], m[3] or '-', m[4] or '-'))
        if store.errors:
            f.write('\n# image decode errors\n')
            for e in store.errors:
                f.write(e + '\n')

    print()
    print('=' * 64)
    print('%-22s %7s %7s %7s' % ('category', 'items', 'icon', 'no icon'))
    for c in cats:
        print('%-22s %7d %7d %7d' % (c['en'], c['count'], c['withIcon'], c['count'] - c['withIcon']))
    tot = len(items)
    wi = sum(1 for x in items if x['icon'])
    print('%-22s %7d %7d %7d' % ('TOTAL', tot, wi, tot - wi))
    print('faces: player %d (%d with image), npc %d (%d with image)' % (
        len(faces['player']), sum(1 for x in faces['player'] if x['img']),
        len(faces['npc']), sum(1 for x in faces['npc'] if x['img'])))
    print('wardrobe: %d (%d with image)   npc looks: %d   monsters: %d' % (
        len(wardrobe), sum(1 for x in wardrobe if x['img']), len(npcs), len(monsters)))
    print('images read this run: %s ; converted %d, reused (already in out) %d, decode errors %d' % (
        src.stats, store.converted, store.reused, len(store.errors)))
    print('missing icon list: %s' % os.path.join(out, 'missing_icons.txt'))
    print('done in %.0fs -> %s' % (time.time() - T0, os.path.join(out, 'index.html')))
    return 0


if __name__ == '__main__':
    sys.exit(main())
