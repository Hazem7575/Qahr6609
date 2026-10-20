# AssetStudio - stage 2 feasibility (3D character dresser)

Client studied: official Conquer Online 6609 (`D:\Conquer Online 3.0`). Everything below was checked on the real files.
The proof tool is `c3model.py` (reader, resolver, OBJ export). `preview_obj.py` is a small software renderer.
The output is in `stage2_proof\`.

**Result: it can be done.** I decoded the mesh format completely, enough for a static dresser. A garment exports to OBJ + PNG
with the bones applied (T-pose), and the textures line up. The animation format (ZKEY) is mostly understood:
the byte layout checks out on 200 of 200 motion chunks, but the meaning of the values still has to be confirmed.

## 1. Files and how an item id becomes a mesh + texture

The client has two sets of tables. The `.ini` files (`armor.ini`, `armet.ini`, `weapon.ini`, `3dobj.ini`, `3dtexture.ini`) are
**older and incomplete**. The binary `.dbc` files from 2017 (`armor.dbc`, `armet.dbc`, `weapon.dbc`, `mount.dbc`,
`3DObj.dbc`, `3DTexture.dbc`, `3dmotion.dbc`, `weaponmotion.dbc`, `mountmotion.dbc`) are the ones to use. For example,
armor.ini has about 955 sections and armor.dbc has 3,754 rows.

| table | format | content |
|---|---|---|
| `armor.dbc` `armet.dbc` `weapon.dbc` `mount.dbc` | `'MESH'`, uint32 count, then records `{uint32 id; uint32 partCount; part[partCount]{uint32 mesh, texture, mixTex; uint8 mixOpt, asb, adb, material}}` (16 bytes per part). The parser reaches EOF exactly. | armor 3,754 / armet 2,793 / weapon 13,292 / mount 1,223 rows. Mount: 1,004 rows have 2 parts (horse + armor/saddle). |
| `3DObj.dbc` `3DTexture.dbc` | `'RSDB'`, uint32 count, `{uint32 id, uint32 fileOffset}[count]`, then zero-terminated paths | 3,244 meshes / 22,413 textures |
| `3dmotion.dbc` `weaponmotion.dbc` `mountmotion.dbc` | same `'RSDB'` but the ids are **uint64** (12-byte entries) | 249,432 / 392,368 motions |

**Body/garment (armor slot):** `key = bodyDigit * 1000000 + itemId - itemId % 10`. The hundreds digit of the id is the colour,
and it stays in the key. Only the last digit (quality) is dropped.
`bodyDigit = player Body % 10`: 1 = 2001 small female, 2 = 2002 big female, 3 = 1003 small male, 4 = 1004 big male.
I checked this by rendering: bodies 1 and 2 come out female, body 3 comes out male.

Example: garment **181305 WhitePhoenix**, body 1, gives key 1181300 in armor.dbc. That row has mesh 1137020, which is
`c3/mesh/001137020.c3` (loose), and texture 1137320, which is `c3/texture/001137820.dds` (only inside **c3.wdf**).

**Headgear:** `armet.dbc`, same idea (key `1111000` = body digit + family; colour/quality handling still to be finished).
**Weapons:** `weapon.dbc`, key = the item id itself (e.g. 410003 gives mesh 410000, texture 410005).
**Mounts:** `mount.dbc` (`mounttype.dat` maps a mount look to `armor=8001xxxx`).
**Head/hair:** `c3/head`, `c3/hair` (head.ini is empty; the ids come from the character's face/hair numbers).

Test: I sampled every 7th garment/armor id (355 items) with bodies 1 and 3, so 710 lookups. **697 resolved to a dbc row and a mesh file
that parses, with its texture present.** The other 13 have no row for that body.

Where files live: `c3\mesh` (1,331 loose files, 54 MB) and `c3\texture` (203 MB loose). Many textures exist only in `c3.wdf`.
The WDF reader from stage 1 (`wdf.py`) opens them.

## 2. C3 container

`"MAXFILE C3 00001"` (16 bytes), then chunks `{char tag[4]; uint32 size; data}`. Chunk counts from scanning 2,231 files
(mesh/weapon/effect/monster/mount): `MOTI` 7253, `PHY4` 5262, `PHY ` 463, `PTC3` 238, `PTCL` 129, `SHAP` 66, `SMOT` 66,
`CAME` 20, `PHY3` 13.

A model file has one PHY chunk per part, followed by the same number of MOTI chunks in the same order.
- `v_body`: the real mesh.
- `v_armet`, `v_l_weapon`, `v_r_weapon`: 6-24 vertex helper boxes that mark where the headgear and the weapons attach.

### PHY4 (the usual mesh chunk), 40-byte vertex
```
uint32 nameLen; char name[nameLen]          // "v_body"
uint32 blendCount                           // 0..2 bones per vertex
uint32 normalVertCount, alphaVertCount
vertex[n+a] { float pos[3]; float u, v; uint32 color; uint32 bone[2]; float weight[2] }   // weights add up to 1
uint32 normalTriCount, alphaTriCount
uint16 index[(n+a)*3]                       // opaque triangles first, then alpha-blended triangles
uint32 texNameLen; char texName[]           // the artist's 3ds max path (GBK), not used
float bboxMin[3], bboxMax[3]
float initMatrix[16]                        // D3D row-vector matrix, translation in [12..14]
28 bytes (uint32 texRow, padding, "STEP" sub-tag) - ignored
```
### PHY (older): the same, but the vertex is 76 bytes: `float pos[4][3]` (4 morph targets, [0] = base), then u, v, color, bone[2], weight[2].
### PHY3: 13 files only. Not decoded yet.

### MOTI (skeleton)
```
uint32 boneCount, frameCount; char keyType[4]; uint32 keyCount
KKEY: key { uint32 frame; float matrix[boneCount][16] }      // in mesh files: the rest pose, 1 key
ZKEY: key { uint16 frame; { float q[4]; float t[3] }[boneCount] }   // motion files c3/<body>/<weapon>/<action>.c3
uint32 morphCount (+ morph weights)
```
- The ZKEY layout matches the chunk size on **200/200** motion chunks.
- `|q| = 1` on most keys, so q is a rotation quaternion. Its component order and the exact meaning of `t` are the open questions.
- Body skeletons have **81 bones**, and the motion files use the same 81.
- Motion key in `3dmotion.dbc`: `body(4 digits) + weaponType(3) + action(4)` maps to a path. Example: `10030000010` is `c3/1003/000/101.c3`.
  Body 2001 reuses folder `c3/1001/...`.

### Bind pose (verified)
`v_out = sum_i weight_i * (v * boneMatrix_i[key0]) * initMatrix`. After that, OBJ needs (x, -z, y) to get Y up.
The result is a clean T-pose with correct texture mapping. UV v must be flipped (`1 - v`) for OBJ.

### Effects
`PTCL`/`PTC3` (particles), `SHAP`/`SMOT` (ribbon/shape + its motion, e.g. `c3/effect/angelwing`) and `CAME` (camera) are not decoded.
Wings and auras use them, so stage 2 should first show wings as their wardrobe PNG from stage 1.

## 3. Proof (`stage2_proof\`)
- `181305_body1.obj/.mtl/.png`: WhitePhoenix, female small. Mesh 762 vertices / 956 triangles, texture 256x256 from c3.wdf.
- `181305_body3.*`: the same garment on the male small body.
- `preview_body1_front.png`, `preview_body1_side.png`, `preview_body3_front.png`: rendered by `preview_obj.py`.
  "Front" is the view down -Z; the model faces +Z, so this shows the back.
- I also checked a PHY4 garment: `python c3model.py 193305 1 <dir>` exports and renders correctly.

Commands:
```
python c3model.py <itemId> [bodyDigit 1-4] [outDir]      # OBJ + MTL + PNG
python preview_obj.py model.obj texture.png out.png [yawDegrees]
```

## 4. What the dresser needs, with an estimate

| step | work | estimate |
|---|---|---|
| Exporter: armor/garment + armet + weapons (L/R) + head/hair for one body, written to a compact JSON/glTF and PNGs, on demand or pre-baked for garments (409 x 4 bodies, about 60 MB) | readers done; finish the armet/head/hair key rules, mixTex/mixOpt (colour dye) | 2-3 days |
| Attach armet/weapons at the `v_armet` / `v_*_weapon` marker bones | skeleton is understood | 1 day |
| Offline WebGL viewer (inline JS, no CDN): orbit camera, body picker, slot pickers that reuse the stage-1 item list, alpha triangles drawn in a second pass | new | 3-4 days |
| Animations (stand/walk/attack): confirm ZKEY quaternion order and translation, interpolate keys, skin on the GPU | format half-decoded | 3-5 days |
| Mounts (2-part horse + armor, rider offset from `mounttype.dat`), PHY3, effects (PTCL/SHAP) for wings/auras | mostly new reverse-engineering | 4-7 days |

- **Static MVP** (T-pose or the first frame, garment/armor + headgear + weapons, rotate/zoom, pick items from the stage-1 list): about 1 week.
- **Full dresser** with animations and mounts: about 2.5-3.5 weeks. Particle effects for wings/auras are the riskiest part.

---

# Stage 2 built: Dressing Room (2026-10-11)

`extract3d.py` (run by `run.bat` after `extract.py`) writes `out/3d/`; `dresser.js` is the viewer tab "غرفة التجهيز / Dressing Room"
(hand-written WebGL 1, no library). Screenshots made with headless Chrome are in `stage2_screens/`.

## Corrections and new findings (supersede the parts above)
- **Skinning order is `v * initMatrix * bone[key0]`**, not `v * bone * initMatrix`. `v * initMatrix` alone is the bind pose,
  a T-pose, for every character mesh. With the old order the armor files came out distorted, because their first KKEY key is not
  identity: it is a **standing frame** (arms down, weapon helpers at the hips). Most garments have an identity first key, so they
  looked right either way. `c3model.bind_pose` now uses the corrected order.
- So the KKEY matrices are *relative* skinning matrices (bind^-1 * pose). Body meshes are exported in the bind pose with bone
  indices and weights. The viewer skins them on the CPU with a per-body pose stored in the manifest (`poses`):
  - `stand`: the first key of the naked body (1 = `001000000`, 3 = `003000000`) or of the first armor with a standing key
    (2 = `002130000`). Body 4 has no clean standing key (its armor keys are fighting poses), so it uses body 3's standing pose
    with the translations scaled by the height ratio (x1.111). That looks right.
  - `T`: identity bones. The helper boxes are taken from a mesh whose first key is identity.
- **Helpers / attachment points**: the helper parts (`v_armet`, `v_l_weapon`, `v_r_weapon`, mounts: `v_mount`, `v_head`, legs,
  `v_pet` ...) are boxes in their own bone space. The attachment matrix is `initMatrix * key0[0]` of the helper. Headgear/hair
  (`armet.dbc`) and weapons are drawn with that matrix as model matrix. Weapons extend along local +Y.
- **PHY3** (mounts, `c3/Mount`, `c3/MountSaddle`): same as PHY4 plus `float normal[3]` at the end of the vertex (52 bytes).
  The trailing data (texture name, bbox, init matrix, `STEP`) is the same. Mount.dbc rows have 1-2 parts (body + saddle/armor),
  posed with their own first key. `v_mount` is the rider point.
- **mounttype.dat** (encrypted .dat, decrypt as in stage 1): section = `mountItemId * 10000 + variant`, `armor=` = mount.dbc key.
  The 300000 Steed has 13 colour variants, each 200xxx mount armor has its own rows.
- **Hair** = `armet.dbc` key `body * 1000000 + 119000 + hairId` (hairId = colour*100 + style, e.g. 310). There is no separate head mesh:
  the face is part of every body mesh. `c3/head` only holds 2 non-player meshes.
- **Headgear** key = `body * 1000000 + id - id % 10` works for item families 111/112/113/114/115/116/118 (110+)/144/170.
  `armet.dbc` has no rows for 123, 141, 142, 143, 145, 148 or for 118000-118100. The client must convert those types in code.
  Not guessed. Earrings (117) have no model at all.
- **Weapons**: weapon.dbc key = item id (or `id - id % 10`). 10,221 of 10,228 weapon items resolve.
- **ZKEY** (motion files `c3/<look>/<weapon>/<action>.c3`, action 100 = stand) stores **absolute** bone transforms: `q = (x, y, z, w)`,
  `t` in a z-up space (the root sits at about +104 = hip height). I tried `invBind = K_stand * Z_stand^-1` with K from the naked body's
  standing KKEY and Z from `100.c3` frame 0, then posed with other actions (110, 250). Both matrix conventions gave twisted limbs.
  So the motion format is still open: a parent/child hierarchy or another quaternion layout is probably involved.

## Export format (`out/3d`)
- `index.js` / `index.json`: `bodies`, `poses`, `armor`/`armet`/`weapon`/`mount` rows (`key -> [[meshIndex, textureIndex], ...]`),
  `meshes`/`textures` (file names), `mounts` (`[item, variant, mount.dbc key, title, offset]`), `coverage`.
- `m/<name>.js` = `A3D.m(name, meta, base64(zlib(...)))`: int16-quantised positions and uvs (`meta.q` = min + step), uint16/32 indices
  (opaque triangles first, then alpha), and for bodies `uint8 {bone0, bone1, weight0}`. `meta.h` = helper matrices.
- `t/<name>.js` = `A3D.t(name, {w, h, f}, base64(zlib(DXT1/DXT3 blocks)))`. These are the original DXT blocks of the DDS.
  The 172 textures larger than 256 are decoded, box-filtered to 256 and re-encoded by a small DXT encoder. The viewer decodes the DXT
  blocks in JavaScript.
- Why JS files: a `file://` page can load `<script>` files, but it cannot `fetch()` files, and WebGL refuses `file://` images.
- `cache.json`: source signatures (loose file size/mtime or WDF entry). A re-run converts only what changed (the first run takes about
  2 minutes, later runs a few seconds).

## Counts (client 6609)
armor.dbc 3,416 rows for bodies 1-4 (garment rows: 423/423/377/377), armet.dbc 2,780 (hair 512/512/448/448, headgear 215 per body),
weapon.dbc 13,212, mount.dbc 1,223, mounttype 1,760 looks for 200 items. That is 2,454 meshes and 3,430 textures, 73 MB.
Stage-1 items with a model: garments 382 (female bodies) / 350 (male bodies) of 409, armor 2,070 of 2,070, headgear 1,487 of 3,136,
weapons 10,221 of 10,228, mounts 193 of 197.

## Not done in stage 2 (see "Stage 3" below)
Animations, particle/shape effects, dye colours, a riding pose and the headgear families missing from armet.dbc were done in
stage 3. The ZKEY paragraph above is superseded by "Stage 3, 1. Motion format".

---

# Stage 3: animations, NPC / monster models, missing headgear, effects, dye (2026-10-12)

New code: `anim3d.py` (called by `extract3d.py`, step "[stage 3]"), `dresser.js` (animation player, effects, model viewer),
`viewer.html` (NPC / monster cards). All new output goes into new folders/files next to the stage-2 output.
Screenshots (headless Chrome): `stage3_screens/`.

## 1. Motion format (solved, checked against the client code)
- A C3 motion file holds one `MOTI` per mesh part, in the order of the PHY chunks of the mesh it drives (the engine pairs
  them by index). Player motion files = 3 one-bone helper motions + the body motion, in the order of the naked body
  mesh (`v_armet, v_l_weapon, v_r_weapon, v_body` for 1003; `v_body` first for 2002).
- Key types: `KKEY` (uint32 frame + float[16] per bone), `XKEY` (uint16 frame + float[12] = 4x3 rows), `ZKEY` (uint16 frame
  + quaternion x,y,z,w + translation), and the old untagged form (`bones, frames`, then float[16] per bone per frame,
  **bone-major**) used by monsters and NPCs.
- **Every key is the final skinning matrix of the bone**: `v_out = sum w_i * (v * initMatrix * M_i)`. There is no hierarchy
  and no inverse bind. Proof from `C3_CORE_DLL.dll` (disassembled with a tiny Go program built on the x86asm package that
  ships with the Go toolchain; kept in the scratch folder only):
  - `Motion_Load` (0x100095d0), ZKEY branch: per bone it sets an identity matrix, calls `D3DXMatrixRotationQuaternion`
    (0x10019d05) and writes `_41,_42,_43 = t`. So the quaternion order is x,y,z,w (D3DX) and the matrix is row-vector.
  - `Phy_Calculate_CPU` (0x1000c0a0) loops over the list of used bones (`phy+0x158`) and calls `Motion_GetMatrix_Blend`
    (0x1000a150), which only lerps two `Motion_GetMatrix` results. No parent chain anywhere.
  - The stage-2 attempt (`invBind = K_stand * Z_stand^-1`) invented an inverse bind; that is what twisted the limbs.
- Player ZKEY motions live in a space turned 90 degrees about z with the hips lifted (`Rz(90) + (0,0,h)`); the dresser
  multiplies by `G^-1` per body (`anim.js bodies[b].gi`; h puts the lowest vertex of the first stand frame on the ground).
  Monster / NPC / effect motions need nothing; mount ZKEY motions get `mountGi` (from the mount's own first key).
- Skeleton (84 bones for 1003/1004/2002, 81 for 2001): 0 root, 1-8 pelvis/spine/head, 9-15 left arm, 16-23 right arm,
  **24-41 three cloth chains (cape / back flaps)**, 42-51 legs, 52-83 skirt chains. In the bare-hand and weapon
  stand / walk / run motions the leg bones keep one wide, slightly crouched stance in every frame (only the cloth chains
  swing); jumps, kicks and skills do animate the legs. This is the data exactly as the client code uses it.
- Motion tables (`3dmotion.dbc`, uint64 keys): `look * 10^7 + weaponType * 10^4 + action`. Looks: 2001 / 2002 (files under
  `c3/1001`, `c3/1002`), 1003, 1004; **riding motions: looks 71001-71004 (`c3/7100x/<weapon>/<action>.c3`)**; monsters:
  `lookface * 10^7 + action` (weapon 000); NPCs: npc.ini `StandByMotion` / `RestMotion` / `BlazeMotion` are 3dmotion keys.
  `mountmotion.dbc` key = `mount.dbc key * 1000 + action` (only the base key, variant 0000, has rows).
- Action numbers used for labels: 100 stand, 101 idle, 110/111 walk, 120/121 run, 130 jump, 150 happy, 160 angry,
  170 sad, 190 wave, 200 bow, 210 kneel, 230 cool, 250 sit, 270 faint, 1-8 dances, 401-409 attacks, 5xx casting,
  70x XP skills. The client has no names for the others; they are listed as "Action N".
- Export (`out/3d/a/*.js`, `A3D.a`): per part `[type, bones, keys, tmin x3, tstep x3, rotScale]`, uint16 key frames, then
  int16 tracks **bone-major, delta-coded along the keys** (static bones compress to nothing). `q` = quaternion (13 bit) +
  translation (8000 steps per range); matrix keys that are rotation x uniform scale become `s` (quaternion + t + scale),
  the rest stay `m` (9 rotation values + t).
- Viewer: keys interpolated (quaternion nlerp, matrix lerp), 30 frames per second x speed, CPU skinning per frame.

## 2. Riding
- Rider = riding motion (71001-71004) without the body correction: the pelvis stays at the origin and the legs hang
  straight. The client draws it on the mount's `v_mount` helper; here the hips go on the saddle point (translation of the
  animated `v_mount` = helper init * mount motion * mountGi), 0.56 x body height above the feet. Approximate: the legs go
  through the mount body like in stage 2.
- Mounts are exported a second time as skinned meshes (`n/n_<name>.js`, next to the static `m/m_<name>.js`) and play
  mountmotion 100/101/110/111/120/121/130/250 (the dresser maps the chosen action onto these).

## 3. Headgear families 117/118/123/141/142/143/145/148 (solved)
`Conquer.exe` (`Env_DX9`, not packed): the armet look function (code around 0x98bc40-0x98bfe0, constant 0x1D0D8 = 119000)
strips the quality digit, takes `family = id % 1000000 / 1000` and switches on `family - 117` (byte table 0x98c008, jump
table 0x98bffc):
- families **117, 118, 123, 141, 142, 143, 145, 148, 177, 178** -> handler 0x98bdc8: the head slot goes back to the
  **hair** (`body * 1000000 + 119000 + hair`), i.e. the client draws no model for these headgears;
- family 119 -> hair; every other family -> `body * 1000000 + id - id % 10` (the armet.dbc rows).
The armet.dbc rows 3118110+ (coronets) and the meshes 3143xxx are therefore not used for player headgear (3145xxx are the
meshes of the ninja veils, family 112). The dresser does the same as the client (the status line says so) and the
coverage counts these families as "drawn as hair".

## 4. NPC and monster models
- `3DSimpleObj.dbc`: `'SIMO'`, uint32 count, `{uint32 id, uint32 parts, {uint32 mesh, uint32 texture}[parts]}`.
  npc.ini `SimpleObjID` -> parts -> 3DObj/3DTexture. 2,728 of 2,771 NpcTypes have a model (355 looks).
- Monsters: the client's monster.dat has no lookface (TypeID is a type, not a look); the models are the armor.dbc rows
  `lookface * 1000000` (lookface > 4): 334 looks. Names come from the server's monster type files (read-only, optional:
  `assetstudio.ini [paths] server_monsters`, default `Qahr6609.GameServer\bin\Debug\ServerDataBase\Monsters`): 163 looks named.
- Meshes `n/n_<name>.js`: all parts in one buffer, skinned with a global bone index (`meta.pb` = first bone of each part,
  `meta.pn` = part names, uint16 bones), so the old / KKEY / XKEY / ZKEY motions can all drive them.
- Thumbnails `th/th_<key>.png` (128 px, transparent): textured, z-buffered software renderer `anim3d.render_thumb`
  (2x supersampled), first frame of the model's own motion. 685 models, about 3 minutes the first time, cached after.

## 5. Effects (approximate)
- `3DEffect.dbc`: `'EFFE'`, uint32 count, records `{char name[32]; uint16 parts; uint32 delay, loopTime, frameInterval,
  loopInterval; float offset[3]; uint8 ?, colorEnable, lev, ?; part[parts] {uint32 effectObj, uint32 texture, float scale,
  uint8 asb, uint8 adb, uint16 pad}}` (66 + 16 x parts bytes). `3DEffectobj.dbc` is an RSDB id -> c3 path table.
- Wings: `info.ini [WingEffectInfo]` maps the wing item families 204-221 to `_p_24_wingN_on/up/down/off`; these are animated
  PHY4 meshes with their own KKEY motion. Item glows / auras: `Action3DEffect.ini` sections `[body]`
  (`body.action.family.sub`), `[weapon]`, `[mount]` (`8.action.NNN0.variant` -> mount.dbc `80000000 + NNN0*1000 + variant`)
  and `[wing]`; values `effect~sx~sy~sz~?`, comma separated. Only idle actions (9999 / 0100 / 0101) are exported.
- Particles `PTCL` / `PTC3` are baked: name, artist texture path, header (PTCL: sprite rows, max count, frames; PTC3: a
  longer header whose length varies - found by requiring the frame chain to use up the chunk), then per frame
  `{uint32 n; [PTC3: uint16 id[n]]; float pos[n][3]; float age[n]; float size[n]; float matrix[16]}`. Drawn as
  camera-facing quads, sprite cell = age x rows^2.
- Blending: the D3DBLEND factors of each part are mapped 1:1 to WebGL (7/8 = destination alpha = 1 / 0 here).
- Exported: 1,054 effects (2,436 mesh parts, 1,292 particle systems), 15 wing types; item maps: weapons 1,830,
  garments/armor 94, mounts 318. **Not done**: SHAP/SMOT ribbons (457 effects use only those), CAME cameras, PTCX, the
  texture UV animation stored after `STEP` in the PHY chunks, and the exact attachment of effects to bones (wings are put
  at 2/3 of the body height, weapon glows on the weapon helper, mount effects on the mount root, auras on the character root).

## 6. Dye
`mixTex` / `mixOpt` are 0 in every armor/armet row. The dyed looks are separate textures: the hundreds digit of the texture
id is the colour (3 = default, 4-9). Rows whose texture has the variants get a `dye` table in the manifest (texture index ->
indices for the colours 3..9); the variants are exported at 128 px. Weapons have almost no colour variants.
