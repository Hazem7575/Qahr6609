r"""Generates the launcher v2 theme textures (paper card, smoky frame, character crop).
Run:  python gen_textures.py            (writes into Launcher\themes\default\images and ...\ramadan\images)
All art is procedural except the character, which is cropped from the owner's art (D:\صور\zy-hds-20211201.png).
Design units: the launcher canvas is 1060 x 625; textures are made at SCALE x design size."""
import os, sys, math, random
import numpy as np
from PIL import Image, ImageFilter, ImageDraw

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "Launcher", "themes"))
SCALE = 1.5
FRAME_W, FRAME_H = 880, 488          # frame in design units
CARD_W, CARD_H = 880, 344            # card incl. torn edges (design), placed at frame y=12
PAD = 60                              # shadow margin around the frame PNG (design)

def S(v): return int(round(v * SCALE))

def fractal(w, h, octaves=5, seed=0, base=6):
    rng = np.random.default_rng(seed)
    acc = np.zeros((h, w), np.float32); amp = 1.0; tot = 0
    for o in range(octaves):
        cw, ch = max(2, int(w / (base * 2 ** (octaves - o)) ) + 2), max(2, int(h / (base * 2 ** (octaves - o))) + 2)
        small = Image.fromarray((rng.random((ch, cw)) * 255).astype(np.uint8))
        big = np.asarray(small.resize((w, h), Image.BICUBIC), np.float32) / 255.0
        acc += big * amp; tot += amp; amp *= 0.55
    acc /= tot
    acc = (acc - acc.min()) / (acc.max() - acc.min() + 1e-6)
    return acc

def torn_edge(w, seed, amp_lo=4.0, amp_hi=1.6):
    rng = np.random.default_rng(seed)
    x = np.arange(w)
    lo = np.interp(x, np.linspace(0, w, 24), rng.normal(0, 1, 24)) * amp_lo
    mid = np.interp(x, np.linspace(0, w, 140), rng.normal(0, 1, 140)) * amp_hi * 1.4
    hi = rng.normal(0, 1, w) * amp_hi * 0.22
    return lo + mid + hi

def card(path, tint=(238, 235, 229), pattern=0.035, seed=7):
    w, h = S(CARD_W), S(CARD_H)
    rng = np.random.default_rng(seed)
    blot = fractal(w, h, 5, seed + 1, 3)
    grain = rng.normal(0, 1, (h, w)).astype(np.float32)
    rgb = np.zeros((h, w, 3), np.float32)
    for i, c in enumerate(tint):
        rgb[:, :, i] = c + (blot - 0.5) * 14 + grain * 2.2
    # fibers
    fib = Image.new("L", (w, h), 0); d = ImageDraw.Draw(fib)
    for _ in range(int(w * h / 2600)):
        x0, y0 = rng.random() * w, rng.random() * h
        ang = rng.normal(0, 0.5); ln = rng.random() * S(28) + S(6)
        d.line([(x0, y0), (x0 + math.cos(ang) * ln, y0 + math.sin(ang) * ln)], fill=int(rng.random() * 90 + 40), width=1)
    fib = np.asarray(fib.filter(ImageFilter.GaussianBlur(0.6)), np.float32) / 255.0
    rgb -= fib[:, :, None] * 7
    # faint seigaiha (wave) pattern, stronger towards the bottom
    pat = Image.new("L", (w, h), 0); d = ImageDraw.Draw(pat)
    r = S(26); step = r
    for row, yy in enumerate(range(-r, h + r, int(step * 0.5))):
        off = (row % 2) * r
        for xx in range(-2 * r + off, w + 2 * r, 2 * r):
            for k in (1.0, 0.78, 0.56, 0.34):
                rr = r * k
                d.arc([xx - rr, yy - rr, xx + rr, yy + rr], 180, 360, fill=255, width=max(1, S(1)))
    pat = np.asarray(pat.filter(ImageFilter.GaussianBlur(0.7)), np.float32) / 255.0
    ramp = np.linspace(0.35, 1.0, h)[:, None]
    rgb -= pat[:, :, None] * pattern * 255 * ramp[:, :, None] * (0.5 + blot[:, :, None])
    # alpha: torn top + bottom edges with dry-brush streaks
    top = S(10) + torn_edge(w, seed + 3)
    bot = h - S(12) + torn_edge(w, seed + 4, 5.0, 2.0)
    yy = np.arange(h)[:, None]
    alpha = ((yy >= top[None, :]) & (yy <= bot[None, :])).astype(np.float32)
    am = Image.fromarray((alpha * 255).astype(np.uint8)); d = ImageDraw.Draw(am)
    for edge, sign in ((top, -1), (bot, 1)):
        # dry-brush hairs: thin, short, partly transparent paper fibres sticking out of the edge
        for _ in range(int(w / S(5))):
            x0 = rng.random() * w; ln = rng.random() ** 2 * S(46) + S(4)
            xi = int(min(w - 1, max(0, x0)))
            out = rng.random() * S(4.5)
            y0 = edge[xi] + sign * out
            d.line([(x0, y0), (x0 + ln, y0 + rng.normal(0, 0.4))], fill=int(150 + rng.random() * 105), width=1)
        # a few short gaps just inside the edge
        for _ in range(int(w / S(40))):
            x0 = rng.random() * w; ln = rng.random() ** 2 * S(30) + S(3)
            xi = int(min(w - 1, max(0, x0)))
            y0 = edge[xi] - sign * (rng.random() * S(2.5) + 1)
            d.line([(x0, y0), (x0 + ln, y0)], fill=int(rng.random() * 120), width=1)
    am = am.filter(ImageFilter.GaussianBlur(0.55))
    alpha = np.asarray(am, np.float32) / 255.0
    # slight darkening near the edges
    dist_t = np.clip((yy - top[None, :]) / S(14), 0, 1)
    dist_b = np.clip((bot[None, :] - yy) / S(14), 0, 1)
    edge_shade = 1 - (1 - np.minimum(dist_t, dist_b)) * 0.10
    rgb *= edge_shade[:, :, None]
    out = np.dstack([np.clip(rgb, 0, 255), alpha * 255]).astype(np.uint8)
    Image.fromarray(out, "RGBA").save(path, optimize=True)
    print("card", path, out.shape)

def frame(path, base, smoke, glow, glow2, seed=11):
    W, H = S(FRAME_W + 2 * PAD), S(FRAME_H + 2 * PAD)
    fx0, fy0, fx1, fy1 = S(PAD), S(PAD), S(PAD + FRAME_W), S(PAD + FRAME_H)
    fw, fh = fx1 - fx0, fy1 - fy0
    n1 = fractal(fw, fh, 6, seed, 2); n2 = fractal(fw, fh, 4, seed + 5, 4)
    yy, xx = np.mgrid[0:fh, 0:fw].astype(np.float32)
    u, v = xx / fw, yy / fh
    rgb = np.zeros((fh, fw, 3), np.float32)
    sm = np.clip((n1 - 0.45) * 2.2, 0, 1) ** 1.5
    # glow blobs: band (lower left/centre) and a soft one top-left
    g1 = np.exp(-(((u - 0.40) / 0.30) ** 2 + ((v - 0.86) / 0.16) ** 2))
    g2 = np.exp(-(((u - 0.12) / 0.20) ** 2 + ((v - 0.95) / 0.12) ** 2))
    g3 = np.exp(-(((u - 0.75) / 0.35) ** 2 + ((v - 0.05) / 0.10) ** 2))
    for i in range(3):
        rgb[:, :, i] = base[i] + sm * smoke[i] + (g1 * (0.55 + n2 * 0.9)) * glow[i] + g2 * glow2[i] * n2 + g3 * smoke[i] * 0.5
    vign = 1 - 0.35 * (np.abs(u - 0.5) * 2) ** 3
    rgb *= vign[:, :, None]
    img = np.zeros((H, W, 4), np.float32)
    # outer shadow
    m = Image.new("L", (W, H), 0); ImageDraw.Draw(m).rectangle([fx0 + S(4), fy0 + S(8), fx1 - S(4), fy1 + S(4)], fill=235)
    m = np.asarray(m.filter(ImageFilter.GaussianBlur(S(22))), np.float32) / 255.0
    img[:, :, 3] = m * 255 * 0.85
    img[fy0:fy1, fx0:fx1, :3] = rgb
    img[fy0:fy1, fx0:fx1, 3] = 255
    Image.fromarray(np.clip(img, 0, 255).astype(np.uint8), "RGBA").save(path, optimize=True)
    print("frame", path, (W, H))

def character(src, path):
    im = Image.open(src).convert("RGBA")
    box = (230, 12, 1150, 885)                    # figure + flames + ribbons
    im = im.crop(box)
    a = np.asarray(im, np.float32).copy()
    h, w = a.shape[:2]
    # soft left fade so the cut cape tail never shows a hard edge
    fade = np.clip(np.arange(w) / 70.0, 0, 1)
    a[:, :, 3] *= fade[None, :]
    # make the bottom fade a little longer (the art already fades)
    vf = np.clip((h - np.arange(h)) / 60.0, 0, 1)
    a[:, :, 3] *= vf[:, None]
    im = Image.fromarray(np.clip(a, 0, 255).astype(np.uint8), "RGBA")
    im.save(path, optimize=True)
    print("character", path, im.size)

if __name__ == "__main__":
    d = os.path.join(ROOT, "default", "images"); r = os.path.join(ROOT, "ramadan", "images")
    card(os.path.join(d, "card_paper.png"))
    frame(os.path.join(d, "frame_smoke.png"), base=(16, 9, 8), smoke=(34, 12, 9), glow=(66, 13, 7), glow2=(40, 11, 5))
    card(os.path.join(r, "card_paper.png"), tint=(240, 234, 220), pattern=0.03, seed=21)
    frame(os.path.join(r, "frame_night.png"), base=(10, 12, 30), smoke=(34, 30, 70), glow=(78, 56, 22), glow2=(36, 30, 70), seed=5)
    src = r"D:\صور\zy-hds-20211201.png"
    if os.path.exists(src):
        character(src, os.path.join(d, "char_flame.png"))

def logo(src, path, pad=40):
    r"""Official Conquer logo (client data2\playskin\Conquer\IMAGE_LOGO.png) + baked soft shadow and warm glow."""
    im = Image.open(src).convert("RGBA")
    im = im.crop(im.getchannel("A").getbbox())
    w, h = im.size
    W, H = w + 2 * pad, h + 2 * pad
    a = Image.new("L", (W, H), 0); a.paste(im.getchannel("A"), (pad, pad))
    glow = np.asarray(a.filter(ImageFilter.GaussianBlur(16)), np.float32) / 255.0
    shadow = np.asarray(a.filter(ImageFilter.GaussianBlur(6)), np.float32) / 255.0
    shadow = np.roll(shadow, 4, axis=0)
    out = np.zeros((H, W, 4), np.float32)
    # warm glow
    ga = np.clip(glow * 1.6, 0, 1) * 0.55
    out[:, :, 0] = 255; out[:, :, 1] = 120; out[:, :, 2] = 30; out[:, :, 3] = ga * 255
    base = Image.fromarray(np.clip(out, 0, 255).astype(np.uint8), "RGBA")
    sh = np.zeros((H, W, 4), np.uint8); sh[:, :, 3] = (np.clip(shadow * 1.3, 0, 1) * 200).astype(np.uint8)
    base.alpha_composite(Image.fromarray(sh, "RGBA"))
    base.alpha_composite(im, (pad, pad))
    base.save(path, optimize=True)
    print("logo", path, base.size)

if __name__ == "__main__":
    lsrc = r"D:\conquer-3d\client\data2\playskin\Conquer\IMAGE_LOGO.png"
    if os.path.exists(lsrc):
        logo(lsrc, os.path.join(ROOT, "default", "images", "logo_conquer.png"))
