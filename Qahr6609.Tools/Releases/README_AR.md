# أدوات قهر الجاهزة (Releases)

نسخ جاهزة (exe بس، من غير سورس) من أدوات سيرفر قهر، متظبطة تشتغل على Qahr6609. شغّل كل أداة من ملف الـ `.bat` بتاعها: هو اللي بيفتحها على داتا السيرفر والكلاينت.

| الأداة | بتعمل إيه | ملاحظات على 6609 |
|---|---|---|
| `MapStudio.bat` | الخرايط: الـ NPCs والوحوش والبوابات، وتعديل `npcs.txt` | زرار **Apply live** بيطبّق على السيرفر وهو شغال (AdminApi) |
| `ConquerStudio.bat` | متصفّح أشكال كلاينت 6609: NPCs بموديل 3D والتأثير مدموج فيه ووش الديالوج (AddAvatar) ومكانه في السيرفر، وحوش، تأثيرات، أيتمات، وجوه | بيقرا `data.wdf` / `c3.wdf` و `ini\c3.wdb` (مش المفكوك بس). exe واحد self-contained. شوف تحت |
| `DatCrypt.bat` | فك وتشفير ملفات `.dat` بتاعة الكلاينت + دمج أيتمات جديدة في `itemtype.txt` | أزرار emoneyShopV2 / new_shop_goods خاصة بـ 7957 |
| `PacketStudio.bat` | الباكتات لايف بين السيرفر والكلاينت + تسجيل + حقن باكت تجربة | بيشتغل في **وضع التطوير** بس. أسامي الباكتات معمولة لـ 7957، فأغلب باكتات 6609 بتظهر hex |
| `PlayerStudio.bat` | إدارة اللاعيبة: الأونلاين وكل الحسابات (حتى الأوفلاين)، اللفل/الخبرة/الريبورن، الفلوس/CPs، إدي أيتم أو باكدج، نقل، إحياء، طرد، بان، رسايل، الشنطة/المعدات/السكلات، وإعلان/هدية للكل/حفظ الكل | بيكلم السيرفر عن طريق AdminApi (`adminapi.txt`) — شوف تحت |

`CLIENT` في أول كل `.bat` = مكان الكلاينت (الافتراضي `D:\Conquer Online 3.0`). غيّره لو الكلاينت عندك في مكان تاني.

## ServerControl — لوحة تحكم السيرفر (برنامج واحد لكل حاجة)
`ServerControl\ServerControl.exe` (واختصار على سطح المكتب: **Qahr6609 Server Control**). بيعرف مكان كل حاجة لوحده من `Qahr6609.Shared\HazemConfig.ini`.
- **لوحة التحكم:** GameServer و AccServer و PatchServer: تشغيل / إيقاف آمن (بيحفظ عن طريق AdminPipe) / ريستارت / حفظ. المعالج والرامات لكل سيرفر برسم آخر 6 دقايق، ومعالج ورامات الجهاز كله، وعدد الأونلاين، وزرار **تطوير / برودكشن**.
- **إعدادات السيرفر:** كل مفاتيح `HazemConfig.ini` و `shell.ini` و `AuthConfig.ini` و `PatchServer.ini` بالشرح اللي في الملف. الحفظ بيعمل نسخة `.bak` ومش بيغيّر غير القيم اللي عدلتها. الباسوردات والمفاتيح مخفية. «طبّق على كل الملفات» = `configure` (من الأساسي للباقي).
- **الأدوات:** كل أدوات `Releases` وسكربتات السيرفر بزرار.
- **شبابيك الكلاينت (6609):** يفتح أي شباك في الكلاينت على شخصية أونلاين لايف (عن طريق AdminApi). ◀ ▶ لتجربة الأرقام واحد واحد، و«سمّي» يحفظ اسم الشباك.
- **الكونسول / البيلد / النسخ الاحتياطي / الأمان:** زي ServerControl بتاع قهر.
- إعداداته في `%AppData%\ScriptHazem\ServerControl.Qahr6609\` (منفصلة عن نسخة قهر).
- محتاج **.NET 10 Desktop Runtime**.

## PlayerStudio على 6609
- شغّله من `PlayerStudio.bat`: البرنامج مالوش باراميترات، فالـ bat بيكتب في `%AppData%\PlayerStudio\settings.json` فولدر الداتا (`ServerDataBase`: `itemtype.txt` و `packages.json` و `adminapi.txt`) وفولدر الكلاينت (للأيقونات)، والباقي (اللغة، التحديث) بيفضل زي ما هو. **الملف ده مشترك مع PlayerStudio بتاع قهر**: لو فتحت نسخة قهر بعده غيّر الفولدر من الإعدادات جوه البرنامج.
- البورت والـ secret بيتقروا من `adminapi.txt` لوحدهم («خد البورت والسر من ملف السيرفر» متعلّمة).
- العمليات في السيرفر: `Infrastructure\Threading\AdminApi.Players.cs`. كل عملية بتتسجل في `ServerDataBase\adminapi_log.txt`، وتعديلات الأوفلاين (لفل/فلوس/CPs/مكان بس) بتاخد نسخة في `ServerDataBase\BackUp\AdminApi\`.
- الباكدجات في `ServerDataBase\packages.json` (البرنامج بيعمله من «تعديل الباكدجات»؛ أرقام الأيتمات لازم تكون من itemtype بتاع 6609).
- فرق 6609: عمود الـ stack في itemtype هنا 47 والبرنامج بيقرا 46، فمش هيقفل خانات الـ +/bless للأيتمات اللي بتتكدّس — السيرفر هو اللي بيرفض («stackable items cannot have plus/bless/enchant/sockets»). الأيقونات: الكلاينت ده مفهوش tpi، فبيظهر بس اللي موجود ملفات مفكوكة في `data\ItemMinIcon` (حوالي 70%)، والباقي بيظهر أول حرف.

## ConquerStudio على 6609
- شغّله من `ConquerStudio.bat`: بيبعتله `--client` (الكلاينت) و `--db` (`ServerDataBase`) و `--source` (`Qahr6609.GameServer`). قراية بس، مابيكتبش في الكلاينت ولا السيرفر.
- **الـ NPCs:** كل أنواع `npc.ini` + `NpcX.ini`. المعاينة = الموديل 3D بالحركة والتأثير بتاع `Effect=` مدموج فيه، و NpcX بالجسم والخوذة والسلاح في الإيد. فوق في الركن **وش الديالوج**: `AddAvatar(n)` من سكربت الـ NPC (`[NpcAttribute]` في السورس) وصورته من `ani\NpcFace.Ani`. في الخصائص: كل مكان في `Npcs.txt` (uid، الخريطة، x,y، الاتجاه، اسم السكربت). فلتر «موجود في السيرفر» = اللي `Npcs.txt` بيحطهم (`mesh / 10` = النوع).
- **الوجوه:** كل `NpcFace` / `PlayerFace`، وفلتر «مستخدم في NPCs السيرفر» بالـ NPCs اللي بتستخدم كل وش.
- **الوحوش** من `Monsters\*.ini` (لوك كل وحش)، و**التأثيرات** من `c3.wdb` + `3DEffect.ini` (اللي ملفاتها مش في الكلاينت في فلتر لوحده).
- الأرقام (selftest 2026-10-19): 2,984 نوع NPC، **2,940 بيترسموا 3D**؛ 755 من 771 نوع ليهم `Effect=` التأثير اتحل واترسم؛ 501 نوع موجودين في السيرفر (498 بيترسموا، 305 ليهم وش ديالوج)؛ 908 وش (507 NpcFace + 401 PlayerFace) منهم 104 وش مستخدمين في سكربتات السيرفر؛ 1,641 وحش (1,288 مربوطين بموديل)؛ 72,547 أيقونة أيتم.
- اختبار: `ConquerStudio\ConquerStudio.exe --selftest "D:\Conquer Online 3.0" "<ServerDataBase>" report.txt` (آخر سطر `RESULT: PASS`، و exit code 0).
- مش بيتحل: NPCs ملفاتهم مش في الكلاينت (مسارات `kingmap\...`)، كام تأثير اسمه مش في الجداول، الوش للـ NPCs اللي مالهاش سكربت أو وشها متغيّر (Ramadan)، الشعر والحصان في NpcX. التفاصيل في `ConquerStudio\شرح_ConquerStudio.txt`.
- النسخة القديمة (ملفات مفكوكة بس، محتاجة .NET 10) متشالة `ConquerStudio\ConquerStudio.old.exe`. إعداداته في `%AppData%\ConquerStudio6609\`.

## وضع التطوير / البرودكشن
`bin\Debug\ServerDataBase\servermode.txt`:
```
mode=development     أدوات التجربة شغالة: PacketStudio (بث الباكتات + الحقن)
mode=production      مقفولة (ده الافتراضي لو الملف مش موجود)
```
السيرفر بيقرا الملف كل 5 ثواني، فمش محتاج ريستارت. أو من كونسول السيرفر: `mode dev` أو `mode prod` (و `mode` لوحدها بتعرض الوضع).
**على سيرفر حقيقي للاعيبة خليه production.**

## إعدادات السيرفر للأدوات (مش في git لأن فيها secrets)
- `bin\Debug\ServerDataBase\adminapi.txt` — `port=5899` و `secret=` (16 حرف أو أكتر). ده اللي MapStudio بيستخدمه للتطبيق اللايف. امسحه = الـ API يتقفل.
- `bin\Debug\packetbridge.ini` — إعدادات PacketStudio (`secret=`, `port=17957`, `inject_port=17958`, `devmode=1`). **الحقن** بيحتاج كمان اسم شخصية التجربة في `allow=` (مثال: `allow=TestHero`). شرح كل الإعدادات في `PacketStudio\packetbridge.sample.ini` و `PacketStudio\INTEGRATION_EN.txt`.

## مش موجودين هنا (ليه)
- UiExplorer: معمول لواجهات كلاينت 7957 (Lua/FGUI)، و 6609 مفيهوش الواجهات دي.
- PuppetTheater: ده مهرجان Halloween لسيرفر 7957 (سورس)، مش أداة.

## للنشر
الأدوات نفسها (الـ exe) مش في git عشان حجمها (حوالي 78 MB). انشرها zip في GitHub Releases. فولدر `MapStudio\source` اللي بيتعمل أول ما تشغّل `MapStudio.bat` ده junction لسورس السيرفر: **متحطوش في الـ zip**.