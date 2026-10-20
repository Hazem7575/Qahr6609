# إعدادات الأداء — `shell.ini`

كل الإعدادات دي اختيارية. لو مش موجودة في `shell.ini` السيرفر بيشتغل بالقيمة الافتراضية،
و**القيم الافتراضية بتخلي اللعب زي ما كان بالظبط** (شبه كونكر الأصلي). التحسينات اللي مش بتبان
للاعب (الكونسول، الحفظ، الشبكة، التسريبات، الأقفال) شغالة على طول ومش محتاجة إعداد.

## 1) إعدادات بتغير حاجة في اللعب — مقفولة افتراضياً

جربها على سيرفر تجربة الأول، وفعّلها واحدة واحدة.

```ini
[Performance]
; سكور البول/جيلد وور/تريجر ثيف بيتبعت مرة في الثانية بدل مع كل ضربة (أكبر مصدر لاج في الحروب)
ScoreboardThrottle=0
; شريط HP البوس: أقل مدة بين تحديثين بالمللي ثانية (250 كويس). 0 = مع كل ضربة
BossHpBarMs=0
; ترتيب ضرر البوس يتبعت مرة في الثانية بدل كل ضربة
BossScoreThrottle=0
; ذكاء الوحوش: 0 = زي دلوقتي، 1 = كل وحش يتحسب مرة واحدة في الدورة (أخف في الزحمة)، 2 = 1 + السرعة المكتوبة في الكود (أنشط)
MonsterAiMode=0
; الخطوة المرفوضة: 0 = ريفرش كامل كل مرة، 1 = ريفرش مرة واحدة كل PullbackCooldownMs
SoftPullback=0
PullbackCooldownMs=500
; ستانس/اتجاه متكرر نفسه خلال المدة دي ميتبعتش للي حواليك
StanceCooldownMs=0
; CastPray: يبعت لصاحبه بس
PrayStanceLocal=0
; حد تكرار لكل نوع باكت (يرمي الزيادة بهدوء). شوف Security_Logs (packet-rate) قبل ما تفعله
PacketRateLimits=0
PacketRateWalk=40
PacketRateJump=15
PacketRateAttack=30
PacketRateChat=10
PacketRateItem=40
PacketRateNpc=20
PacketRateAction=60
; فصل اللاعب لو الباكتات بتاعته عملت أكتر من N خطأ في 10 ثواني (0 = تسجيل بس)
MaxHandlerExceptions=0
; الأرينا: يعمل كل الماتشات المتاحة مرة واحدة بدل ماتش كل 6 ثواني
ArenaPairAllPerRun=0
; الأوتو هانت يدور على أقرب وحش الأول
AutoHuntNearbySearch=0
; حذف وحوش الـ instances المتسابة (احذر مع الـ instances متعددة الأدوار)
InstanceSweep=0
; البوت اللي ملقاش هدف يتشال بعد كذا ثانية
BotIdleTimeoutSec=0
; تجاهل طلب قايمة الساش/البوث المكرر خلال المدة دي
SashListThrottleMs=0
VendingListThrottleMs=0

[Gameplay]
; إصلاح ثغرة سرعة الضرب مع البروك (بيبطّأ الضرب عند Monk/Warrior/... لأن الثغرة كانت بتسرّعه)
ProcStampFix=0
; البروك والجيمز واستهلاك الديورابيليتي يحصلوا بس للضربة المقبولة
ProcsAfterSpeedCheck=0
; كول داون للمانا بوشن (HP بوشن عنده 300)
ManaPotionCooldownMs=0
```

**الترتيب اللي ننصح بيه للتجربة:** `ScoreboardThrottle=1` و `BossHpBarMs=250` و `BossScoreThrottle=1`
(مش هتفرق مع اللاعب تقريباً وبتشيل أكبر لاج في الحروب) ← بعدين `MonsterAiMode=1` ← بعدين `SoftPullback=1`
← بعد أسبوع من مراجعة `Security_Logs` فعّل `PacketRateLimits=1`.

## 2) إعدادات داخلية (مش بتبان للاعب) — شغالة افتراضياً

| المفتاح | الافتراضي | المعنى |
|---|---|---|
| AsyncConsole | 1 | الكونسول بيكتب في الخلفية (الضغط على الشاشة السودا مبقاش بيهنج السيرفر) |
| ConsoleQueueMax | 20000 | أقصى عدد سطور مستنية |
| SocketLoopMs | 10 | دورة استقبال الباكتات (كانت 20) |
| TimerResolution1ms | 0 | 1 = دقة تايمر ويندوز 1ms |
| HandshakeTimeoutSec | 60 | قفل الاتصال اللي ماكملش الدخول (0 = مقفول) |
| DHKeyPoolSize | 16 | مفاتيح تشفير جاهزة مسبقاً |
| PacketPoolMax | 2048 | أقصى باكتات محفوظة لإعادة الاستخدام |
| GenericPoolSleepMs | 10 | |
| PlayerSaveStagger | 1 | حفظ اللاعبين موزع على الوقت بدل الكل مرة واحدة |
| PlayerSaveIntervalSec | 250 | كل لاعب بيتحفظ كل كام ثانية |
| PlayerIniDeltaSave | 1 | يكتب بس القيم اللي اتغيرت في ملف اللاعب |
| PlayerIniFullSaveEvery | 12 | كتابة كاملة كل كام حفظة (والخروج دايماً كامل) |
| MySqlConnectTimeoutSec / MySqlCommandTimeoutSec / MySqlReaderTimeoutSec | 3 / 5 / 30 | |
| ArenaRankRebuildSec | 15 | ترتيب الأرينا بيتحسب كل كام ثانية (وبيتحسب فوراً قبل أي عرض أو مكافأة) |
| FloorItemSweep | 1 | الآيتمز اللي على الأرض بتختفي بعد مدتها حتى لو مفيش حد قريب |
| HouseFurnitureOnView | 1 | عفش البيت بيتبعت لما يظهر بس |
| SurroundingsSpawns | 0 | 1 = يرجع الإرسال المكرر القديم |

أي مفتاح تغيره محتاج رستارت للسيرفر.
