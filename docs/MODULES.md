# الموديولز (Modules / Plugins)

## يعني إيه موديول؟

الموديول هو **فولدر** جوه `Qahr6609.GameServer\Modules\<Name>\` فيه فيتشر كاملة (كلاس أو أكتر).
السيرفر بيلاقيه لوحده وقت الـ build والتشغيل — مفيش أي سويتش في `shell.ini` ولا تسجيل في أي مكان:

* **الفولدر موجود = الموديول شغال.**
* **عايز تشيله؟** اعمله zip وامسح الفولدر (فيه bat بيعمل ده)، وبعدين rebuild.

ليه كده؟ عشان كل فيتشر جديدة تبقى في مكان واحد، متلمسش كود اللعبة الأصلي، وتقدر تقفلها/ترجعها في دقيقة.

الـ csproj فيه سطر واحد بيضم أي ملف `.cs` جوه `Qahr6609.GameServer\Modules` (ما عدا `_off`):

```xml
<Compile Include="Modules\**\*.cs" Exclude="Modules\_off\**" />
```

> ملحوظة Visual Studio: ضيف الملفات بإنك تحطها في الفولدر بالـ Explorer، مش من "Add > New Item".
> لو VS فك السطر ده لأسماء ملفات، رجّعه زي ما هو فوق.

---

## إزاي أعمل موديول جديد؟

```bat
Qahr6609.Tools\Server\ServerCtl\new_module.bat DailyGift
```

ده بينسخ `Qahr6609.Tools\Server\ServerCtl\module_template\` لـ `Qahr6609.GameServer\Modules\DailyGift\DailyGiftModule.cs` ويغير الاسم جوه الكود.
(ممكن تديله رقم NPC تاني: `Qahr6609.Tools\Server\ServerCtl\new_module.bat DailyGift 8160555` — لو مدتهوش هو بيختار رقم فاضي من 8160000 لـ 8169999).

بعد كده: `Qahr6609.Tools\Server\ServerCtl\step2_build.bat` وشغّل السيرفر. هتلاقي في الكونسول سطر زي:

```
[Modules] Loaded 1: DailyGift v1.0
```

ولو موديول وقع وهو بيحمّل هتلاقي سطر أحمر `[Modules] FAILED ...` والسيرفر بيكمل عادي من غيره.

الاسم: حروف إنجليزي وأرقام و `_`، ويبدأ بحرف. فيه أسماء ممنوعة لأنها بتخبط في كود السيرفر (`Game`, `Client`, `Role`, `Database`, `Program` ...) — الـ bat هيقولك.

أقل موديول ممكن:

```csharp
using Conquer6609.Framework.Modules;

namespace Conquer6609.Modules.DailyGift
{
    public sealed class DailyGiftModule : ModuleBase
    {
        public override string Name { get { return "DailyGift"; } }

        public override void OnLogin(Client.GameClient client)
        {
            client.SendSysMesage("Welcome " + client.Player.Name);
        }
    }
}
```

لازم الكلاس يكون `public` أو `internal`، مش `abstract`، وعنده constructor **`public`** من غير باراميترز (constructor `private` أو `internal` مش هيتشاف والموديول مش هيتحمل).

---

## الهوكس (Hooks) — كل واحدة بتتنده إمتى

اعمل `override` للي محتاجه بس. الموديول اللي مش عامل override لهوك معين **مش بيتنده خالص** للهوك ده (مفيش أي تكلفة).

| Hook | بتتنده إمتى |
|---|---|
| `void OnLoad()` | مرة واحدة بعد ما داتا السيرفر تتحمل. لو رمت exception الموديول مش هيشتغل. هنا تقرا ملفاتك وإعداداتك. |
| `void OnShutdown()` | مرة واحدة وإنت بتقفل السيرفر (بالترتيب العكسي للـ Priority). |
| `void OnSave()` | مع الحفظ الدوري للعالم. ممكن تيجي من thread حفظ في الخلفية — لازم تكون thread-safe. (المانجر مش بيشغل اتنين OnSave في نفس الوقت.) ده المكان الوحيد اللي مسموح فيه بكتابة ملفات صغيرة. |
| `void OnTick(Extensions.Time32 clock)` | كل ثانية تقريبًا (KernelThread). |
| `void OnLogin(Client.GameClient c)` | بعد ما اللاعب يخلص دخول (بعد CompleteLogin). |
| `void OnLogout(Client.GameClient c)` | لما اللاعب يخرج / يفصل. |
| `bool OnMove(Client.GameClient c, ushort x, ushort y)` | قبل كل خطوة مشي، مشي بالحصان، أو نطة لـ (x,y). رجّع `false` عشان **تمنع** الحركة — وساعتها إنت المسئول ترجّع اللاعب مكانه (مثلًا `client.Pullback()` أو teleport). |
| `uint OnMonsterDamage(Client.GameClient attacker, Game.MsgMonster.MonsterRole target, uint damage)` | ضرب لاعب لوحش. رجّع الضرر بعد التعديل (أو نفس الرقم). لو أكتر من موديول، الضرر بيعدي عليهم بالترتيب. |
| `void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster)` | لاعب قتل وحش. |
| `void OnPlayerKilled(Role.Player killer, Role.Player victim)` | مع **كل** نداء لـ `Player.Dead` (أي موتة للاعب، مش بس من لاعب تاني)، في **أول** معالجة الموت قبل أي حاجة تانية. `killer` ممكن يبقى `null` (مثلًا وحش أو سبب تاني). |
| `bool OnItemUse(Client.GameClient c, Game.MsgServer.MsgGameItem item)` | لاعب استخدم آيتم. رجّع `true` = "أنا اتعاملت معاه" والسيرفر مش هيعمل التصرف العادي بتاعه. |

خصائص تانية:

| | |
|---|---|
| `string Name` | اسم الموديول (لازم). هو اسم فولدر الداتا واسم السكشن في shell.ini. |
| `string Version` | افتراضي `"1.0"`. |
| `int Priority` | افتراضي `0`. الأقل بيشتغل الأول (في التحميل وفي كل الهوكس). |

> **تنبيه:** الهوكس بتشتغل لما الكود الأساسي بيناديها (`ModuleManager.Login(...)` إلخ). لو هوك مش بيتنده، اتأكد إن نداءها موجود في الكور.

---

## NPC جوه الموديول

مش محتاج تعدل `NpcID.cs` — اعمل cast لرقم:

```csharp
public const uint NpcUid = 8160123;

[Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)NpcUid)]
public static void OnNpcTalk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
{
    var dialog = new Game.MsgNpc.Dialog(client, stream);
    switch (Option)
    {
        case 0:   // أول ضغطة على الـ NPC
            dialog.AddText("Hello!");
            dialog.AddOption("Give me a gift", 1);
            dialog.AddOption("Bye.", 255);
            dialog.AddAvatar(0).FinalizeDialog();
            break;
        case 1:
            // ...
            break;
    }
}
```

* التوقيع لازم يبقى بالظبط كده (`static void` و 5 باراميترز بالترتيب ده).
* رقم الـ NPC لازم يكون **مش مستخدم** (لو اتكرر، الكونسول هيطبع error والأولاني هو اللي هيشتغل).
* الـ NPC نفسه لازم يكون موجود في الخريطة بنفس الـ UID: يا إما تضيفه في ملف الـ NPCs زي أي NPC، يا إما تعمله spawn في `OnLoad` (التمبليت فيه مثال `SpawnNpc()` بيقرا `NpcMap/NpcX/NpcY/NpcLook` من shell.ini).
* الـ options بتتسجل أوتوماتيك من `AddOption` (حماية StrictNpcOptions شغالة عادي).

## Packet جوه الموديول

نفس فكرة الـ NPC:

```csharp
[PacketAttribute(12345)]   // packet id جديد مش مستخدم
public static void MyPacket(Client.GameClient user, ServerSockets.Packet stream) { ... }
```

بيتنده على thread السوكت — خليه سريع جدًا ومتعملش فيه I/O.

## أوامر الشات (Commands) جوه الموديول

```csharp
using Conquer6609.Framework.Commands;

[GmCommand("gift", "dailygift", Role = CommandRole.GM, Help = "send today's gift", Usage = "@gift name")]
static void GiftCommand(Client.GameClient client, string[] args)
{
    // args[0] = "gift" (اسم الأمر) ، args[1] = أول باراميتر ...
}
```

* `Role`: `CommandRole.Player` (أي لاعب) ، `CommandRole.GM` (GM أو PM) ، `CommandRole.ProjectManager` (PM بس).
* ممكن أكتر من اسم (alias) في نفس الـ attribute.
* `Sensitive = true` لو الأمر فيه باسورد أو حاجة سرية — اللوج هيكتب اسم الأمر بس.
* كل أمر بيتسجل في `Security_Logs` (`gm-command`)، ولو حد معندوش صلاحية بيتسجل `gm-denied`.
* لو الاسم متكرر: تحذير في الكونسول والأولاني يكسب.
* أوامر جاهزة: `@modules` (حالة الموديولز: شغال / faulted / load-failed وعدد الأخطاء) و `@help` أو `@commands` (الأوامر المسجلة اللي تقدر تستخدمها؛ `@help gift` بيوريك تفاصيل أمر).

---

## الداتا واللوجز والإعدادات

| إيه | فين |
|---|---|
| ملفات الموديول | `DataPath` = `<DbLocation>\Modules\<Name>\` (بيتعمل لوحده أول ما تستخدمه) |
| اللوج | `Log("...")` ← `<DbLocation>\Modules\<Name>\logs\yyyyMMdd.log` — async، مش بيعطل اللعبة أبدًا |
| الإعدادات | `shell.ini` سكشن `[Module.<Name>]` |

```ini
[Module.DailyGift]
Greeting=Welcome {name}!
NpcMap=1002
NpcX=300
NpcY=280
NpcLook=1080
```

```csharp
int map     = ConfigInt("NpcMap", 0);
long limit  = ConfigLong("Limit", 1000);
bool loud   = ConfigBool("Loud", false);      // 1 / true / on
string text = ConfigString("Greeting", "");
```

كل مفتاح لازم يبقى ليه قيمة افتراضية معقولة في الكود. الإعدادات بتتقري مرة واحدة (cache).
القيمة **مينفعش يبقى فيها `;`** — أي حاجة بعد `;` بتتعتبر تعليق وبتتشال.

لما تقفل موديول، فولدر الداتا بتاعه **بيفضل موجود** — لو رجعته هيلاقي داتاه.

---

## قفل / تشغيل موديول

```bat
Qahr6609.Tools\Server\ServerCtl\module_off.bat DailyGift
```
بيعمل zip للفولدر في `Qahr6609.GameServer\Modules\_off\DailyGift.zip` (لو موجود قبل كده بيزود تاريخ ووقت في الاسم) وبعدين يمسح الفولدر.

```bat
Qahr6609.Tools\Server\ServerCtl\module_on.bat DailyGift
```
بيرجّع الفولدر من أحدث zip (والـ zip بيفضل زي ما هو).

**بعد الاتنين لازم rebuild:** `Qahr6609.Tools\Server\ServerCtl\step2_build.bat`

فولدر `_off` فيه zips بس — متحطش فيه ملفات `.cs`.

---

## الأمان والأخطاء

* أي exception من موديول **مش بتوقع السيرفر**: بتتكتب في لوج الموديول، وبيطلع سطر أحمر في الكونسول (مرة كل 30 ثانية بالكتير لكل موديول).
* لو موديول رمى **100 exception في 60 ثانية** بيتعلّم `faulted` لحد ما السيرفر يعيد تشغيل (سطر في الكونسول + `module-faulted` في Security_Logs). شوفه بـ `@modules`.
  * اللي بيقف: هوكس الجيم بلاي بس (`OnTick`، `OnLogin`، `OnMove`، `OnMonsterDamage`، `OnMonsterKilled`، `OnPlayerKilled`، `OnItemUse`).
  * اللي بيفضل شغال: أوامره (`[GmCommand]`)، الـ NPCs والـ packets بتاعته، و`OnSave` و`OnLogout` و`OnShutdown` (عشان داتاه متضيعش).
* لو `OnMove` رمى exception، الحركة بتعدي عادي (مش بتتمنع).

## القواعد (مهم)

1. **متعملش I/O بيستنى في الهوكس** (ملفات، MySQL، نت). استخدم `Log()` أو اعمل queue/thread خاص بيك. الاستثناء: `OnLoad` و `OnSave` و `OnShutdown`.
2. **Thread-safety:** الهوكس بتتنده من أكتر من thread (السوكت، MapGroupThread، KernelThread، thread الحفظ). استخدم `ConcurrentDictionary` و `Interlocked` و `lock` لأي حالة مشتركة.
3. **خليها سريعة:** `OnMove` و `OnMonsterDamage` بيتندهوا آلاف المرات في الثانية. متعملش LINQ ولا allocations ولا بحث في لستة كبيرة فيهم. لو مش محتاج الهوك، متعملهوش override.
4. متغيرش gameplay اللاعبين من غير ما تقصد — الموديول بيأثر على السيرفر كله.
5. الكلاسات المساعدة خليها جوه namespace الموديول (`Conquer6609.Modules.<Name>`) عشان متخبطش في موديول تاني.
6. موديول عايز يكلم موديول تاني: `ModuleManager.Get<OtherModule>()` (بيرجع `null` لو مش شغال).

## موديول جاهز في فولدر (Drop-in) — من غير build

ده اللي بيخلّي حد ياخد مهرجان كامل كفولدر واحد ويحطه في السيرفر وخلاص: السيرفر بيعمل compile للكود (Roslyn) وهو بيقوم، وبيقرا الداتا كلها من الفولدر نفسه.

```
bin\Debug\ServerDataBase\Modules\<Name>\
    *.cs  (أو code\*.cs)          الكود: ModuleBase + [NpcAttribute] + [PacketAttribute] + [GmCommand] + [DataAttribute]
    content\
        itemtype.txt               أيتمات جديدة (نفس سطور itemtype.txt)
        maps\gamemap.txt           مابات جديدة: "<map id> <ملف dmap>" أو "<map id> <ملف dmap> <base map>" لنسخة من ماب
        maps\<map id>.ini          معلومات الماب ([info] type, portal0_x/y, reborn_map, color)
        npcs.txt                   NPCs: uid,type,mesh,map,x,y[,dynamic]
        Monsters\*.ini             أنواع وحوش (cq_monstertype)
        MonsterSpawns\<map>\<فولدر>\*.ini   أماكن ظهور الوحوش (cq_generator)
        portals.ini                بوابات
    data\                          داتا الموديول وإعداداته (DataPath)
    logs\                          Log()
    .build\                        cache الـ compile (بيتعمل لوحده، ممكن تمسحه)
```

- أول تشغيل بيعمل compile (حوالي ثانية)، وبعد كده بيحمّل من `.build` في 1ms لحد ما ملف يتغير أو السيرفر يتعمله build جديد.
- لو فيه خطأ في الكود، السيرفر بيطبع الملف والسطر والرسالة، وبيقوم عادي من غير الموديول ده.
- أي ID موجود قبل كده (في السيرفر أو في موديول تاني) بيترفض وبيتطبع مع اسم الموديول (`skipped: the server already has it`).
- أرقام NPC جديدة: `[NpcAttribute((Game.MsgNpc.NpcID)8155001)]`، مش محتاج تعدّل `NpcID.cs`.
- لو اسم الفولدر بيبدأ بـ `_`، الموديول بيتقفل (`_Ramadan` مثلًا). عشان تشيل الموديول، امسح الفولدر.
- الـ namespace: لو كتبت الكود جوه `Conquer6609.*`، هتقدر تكتب أسماء أنواع السيرفر قصيرة زي كود السيرفر بالظبط. لو برّاه، اكتب الاسم كامل (`Conquer6609.Client.GameClient`).
- الكلاينت (NPC looks, maps, effects) بيتوزّع لوحده مع الباتش، والسيرفر مالوش دعوة بيه.
- `shell.ini [Modules] Runtime=0` بيقفل compile الكود، و`Content=0` بيقفل قراية `content\`.

### نفس الموديول جوه السورس (build عادي)

لو عايز الموديول يبقى جزء من السورس ويتبني معاه، حط نفس الفولدر في:

```
Qahr6609.GameServer\Modules\<Name>\        (أو Modules\Custom\<Name>\)
    *.cs                الكود: بيتبني جوه GameServer.exe مع الـ build
    content\            بيتنسخ مع كل build لـ bin\Debug\ServerDataBase\Modules\<Name>\content\
    data\               بيتنسخ لـ ServerDataBase\Modules\<Name>\ (الملفات اللي مش موجودة بس، علشان ما يمسحش اللي اتحفظ)
```

- اعمل build عادي (`step2_build.bat` أو Visual Studio) وبعدين شغّل السيرفر.
- خلي اسم الفولدر هو نفس `Name` بتاع الموديول، علشان `DataPath` يشاور على نفس الفولدر.
- ما تحطش نفس الموديول في المكانين (السورس و `ServerDataBase\Modules`)، وإلا الكود هيتسجل مرتين.
- لو مسحت موديول من السورس، امسح كمان `bin\Debug\ServerDataBase\Modules\<Name>\content`، لأن الـ build بيضيف الملفات ومبيمسحش.
- لو اسم الفولدر بيبدأ بـ `_` (`Modules\_Fest`)، الموديول بيتقفل: لا بيتبني ولا بيتنسخ.
مثال كامل: `Qahr6609.GameServer\Modules\Ramadan\` (حدث رمضان). فيه الكود (`RamadanModule.cs` و`code\`)، و7 وحوش في `content\Monsters\`، وإعدادات الحدث وأماكن الـ NPCs في `data\event.ini`. انسخ الفولدر ده في سيرفر تاني واعمل build، أو حطه في `ServerDataBase\Modules\` من غير build.
