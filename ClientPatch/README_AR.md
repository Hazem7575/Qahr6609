# ClientPatch — باتش الكلاينت (المصدر الوحيد)

الفولدر ده فيه **بس** الحاجات اللي بتخلي كلاينت Conquer 6609 الأصلي (بتاع TQ) يدخل ويلعب على سيرفرنا،
**من غير ما يغير شكل أي حاجة في الكلاينت**: مفيش صور ولا موديلات ولا خيول ولا إفكتات ولا ini بتغير شكل حاجة موجودة.

```
ClientPatch\
  files\            <- الملفات اللي بتتحط في فولدر الكلاينت (نفس المسارات بالظبط). الـ PatchServer بيوزع من هنا بس.
  optional\         <- حاجات اختيارية بتتنسخ بالإيد مرة واحدة (مش بتتوزع أوتوماتيك)
  manifest.txt      <- كل ملف: المسار، الحجم، md5، وليه موجود
  make_manifest.ps1 <- يعيد كتابة manifest.txt بعد أي تعديل في files\
  README_AR.md      <- الملف ده
```

## 1) لازم عشان الكلاينت يدخل السيرفر (REQUIRED-TO-CONNECT)

| الملف | بيعمل إيه |
|---|---|
| `Env_DX9\Conquer.exe` و `Env_DX8\Conquer.exe` | نفس exe بتاع TQ بالظبط + حاجة واحدة زيادة: بيحمّل `MeGaShield.dll`. السيرفر بيقارن الـ md5 بتاعه بـ `Qahr6609.GameServer\bin\Debug\Files\Env_DX*\Conquer.exe`. لو مختلف اللاعب بيتطرد ("Modified Client Files"). |
| `Env_DX9\MeGaShield.dll` و `Env_DX8\MeGaShield.dll` | الحماية + مفتاح التشفير بتاع اللوجين (`HazemConfig.ini LoginKey`). السيرفر بيفحص الـ md5 بتاعه برضه (`_GShield.cs` → `ShieldFile()`). |
| `Env_DX*\GuardShield.dll`, `Env_DX*\HMProtect.dll`, `Env_DX8\EasyHook32.dll`, `Env_DX8\mscvr100d.dll`, `Env_DX8\D3DX8_56.dll` | ملفات مساعدة للحماية. الـ MeGaShield متشفر فمش واضح بيحملها ولا لأ، فسيبناها احتياطي. ملفات جديدة، مش بتغير حاجة. |
| باقي `Env_DX9\*.dll` و `Env_DX8\*.dll` و `Microsoft.VC90.*` | محرك اللعبة (كود بس، مفيش صور ولا ini) — نفس المجموعة اللي شغالة دلوقتي مع الـ exe المعدل. الأصلية بتاعة TQ في الفولدرين دول اتمسحت فوقها قبل كده من غير نسخة احتياطية، فمش متأكدين إن الأصلية هتشتغل؛ عشان كده دي اللي بنوزعها. |
| `config.ini` | `GameIP=26.31.69.47` — الـ MeGaShield بيقرا منه IP السيرفر. |
| `Guard.dat` | قايمة السيرفرات اللي بتظهر في شاشة الدخول (الاسم `MeGa` → سيرفر الحسابات `26.31.69.47:9961`). `Server.dat` بتاع TQ مش مستخدم. لتغيير الـ IP: `Qahr6609.Tools\Server\Cluster\configure.bat -GuardDat` (بيكتب هنا) أو `Qahr6609.Tools\Client\Guard\guardtool.exe`. اللانشر كمان بيكتبه لوحده من `/servers` قبل ما يشغل اللعبة. |
| `GuardAr.dat` | **مش متأكدين** هو لازم ولا لأ (ملف من أيام MeGa، مش قايمة سيرفرات). صغير وجديد، فسيبناه. |
| `Start_DX9.bat` و `Start_DX8.bat` | تشغيل اللعبة من غير لانشر (`blacknull` بيمنع الأوتوباتش بتاع TQ). |

## 2) مميزات السيرفر (FEATURE) — بتزود حاجة ومش بتغير شكل حاجة موجودة

| الملف | الميزة | التعديل |
|---|---|---|
| `ini\globallotterycondition.ini` | الشجرة الذهبية (Golden Tree) | الملف الأصلي فاضي (`Amount=0`) فالشباك بيبقى فاضي والكلاينت بيرفض القطف. **ماتكتبوش بإيدك**: السيرفر بيطلعه في `ServerDataBase\Modules\GoldenTree\client\ini\globallotterycondition.ini` (مع كل تشغيل أو `@goldentree reload`) — انسخه من هناك لهنا لو اتغير. |
| `ini\GUI.ini` | أيقونة النرد (Dice / Sic Bo) + عداد الشجرة الذهبية | الملف الأصلي بالظبط مع **4 سطور بس**: `[579-1429]` (x=10220,y=170 → x=224,y=178) و `[579-5481]` زرار النرد (x=10130,y=122 → x=285,y=132). الاتنين كانوا مستخبيين برا الشاشة، مفيش أي حاجة تانية اتحركت. |

اتأكدنا إن دول **موجودين في الكلاينت الأصلي** ومش محتاجين باتش: آيتمات الشجرة والمهرجان (AnniversaryStar 3303191،
Celestial 3008705-3008719 كلهم في `ini\ItemtypeSub.dat` الأصلي)، BlissBoy `[NpcType4676]`، IronTong `NpcX [5022]`،
NPCs الـ Celestial (4020-4022)، الماب 10005.

## 3) اللي اتشال ومش هيتوزع تاني (LOOK-CHANGING)

`ini\c3.wdb` (خلى كل الخيول مخفية)، `ini\3DEffect.ini`، `3DEffectObj.ini`، `3dtexture.ini`، `Questinfo.ini`، `npc.ini`، `NpcX.ini`،
`itemtype.dat` المعدل (+5938 سطر و1966 سطر متغير)، ملفات رمضان (`c3\rmd\*`، `c3\effect\Ramadan\*`، `rmd\5.c3`)،
`client_config.dat`، `version.dat`، `AutoPatch\*`، و `TQAnp.dll`/`TQPlat.dll` القديمة اللي في فولدر الكلاينت الرئيسي.
دي ملفات كاملة من نسخة كلاينت تانية، مش إضافات. حدث رمضان مقفول (`Enabled=0`)؛ لو رجع لازم يتعمل له باتش
جديد = الملف الأصلي + الأقسام الجديدة بس. النسخ القديمة محفوظة في `D:\conquer\_off\qahr6609_patch_cleanup_20261020\`.

## 4) حاجات اختيارية (`optional\`، بتتنسخ بالإيد)

| الملف | ليه مش في `files\` |
|---|---|
| `optional\Settings.ini` | إعدادات الشاشة والفريمات بتاعة MeGaShield. اللاعب واللانشر بيعدلوه، فلو اتوزع أوتوماتيك كان هيرجع للإعدادات القديمة كل مرة. انسخه مرة واحدة لو مش موجود. |
| `optional\launcher\launcher.ini` | إعدادات اللانشر (`UpdateUrl=http://26.31.69.47:8800/`). اللانشر نفسه: `_release\scripthazemLauncher.exe` (بيتعمل بـ `Qahr6609.Tools\Client\LauncherArt\build_launcher.bat`). اللانشر **مش لازم** للعب. |

## 5) إزاي تطبقه على كلاينت بإيدك

1. كلاينت 6609 أصلي (`version.dat` = 6609). ماتشغلش `Play.exe` ولا `AutoPatch.exe` بتوع TQ.
2. انسخ **محتوى** `ClientPatch\files\` جوه فولدر الكلاينت ووافق على الاستبدال (ماعدا `README.txt` مش لازم).
3. (اختياري) انسخ `optional\Settings.ini` لفولدر الكلاينت لو مش موجود.
4. شغّل `Start_DX9.bat` (أو `Start_DX8.bat`)، أو اللانشر.

## 6) الـ PatchServer واللانشر

- `Qahr6609.Tools\Server\PatchServer\PatchServer.ini` فيه `Files=..\..\..\..\..\ClientPatch\files` (نسبي من `bin\Release`).
  يعني الـ PatchServer بيبني `manifest.json` من الفولدر ده وبيوزع `/files/...` منه بس. `www\files` مابقاش مستخدم.
- بعد أي تعديل في `files\`: اكتب `rebuild` في شباك الـ PatchServer (أو اقفله وافتحه)، وشغّل `make_manifest.ps1`.
- اللانشر بيقارن md5 كل ملف في الـ manifest وبينزل المختلف بس. **مش بيمسح** ملفات قديمة نزلها قبل كده
  (زي c3.wdb أو 3DEffect.ini) — دي لازم ترجع من النسخة الأصلية بإيدك (شوف التقرير).
- ماتحطش هنا ملفات بيعدلها اللاعب (Settings.ini، GameSetup.Ini) ولا ملفات بتغير شكل الكلاينت.
