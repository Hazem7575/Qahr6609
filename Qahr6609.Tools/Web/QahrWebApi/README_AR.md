# QahrWebApi — ربط الموقع بسيرفر اللعبة (إرسال CPs / إعطاء آيتم / عرض الشنطة)

مجلد PHP بسيط يوضّح كيف موقعك يكلّم سيرفر اللعبة (Qahr6609.GameServer) بشكل آمن لعمل 3 أشياء فقط:

1. **إرسال CPs** للاعب (`sendCps`)
2. **إعطاء آيتم** للاعب (`giveItem`)
3. **عرض الشنطة (Inventory) والمعدّات** للاعب (`inventory`)

> **مقفول افتراضياً.** السيرفر ما يفتح أي بورت إلا إذا أنشأت الملف `ServerDataBase\webapi.ini` وكتبت فيه `enabled=1` ومفتاح صحيح.
> بدون الملف (أو `enabled=0`) السيرفر يكتب فقط ملف مثال `ServerDataBase\webapi.ini.example` ولا يسمع على أي بورت.

---

## الحماية (باختصار)

- **تشفير قوي:** كل طلب ورد مشفّر بـ **AES-256-GCM** (تشفير + توقيع، أي تعديل على الرسالة يُرفض).
- **مفتاح سرّي مشترك** (32 بايت عشوائي) بين الموقع والسيرفر، ومنه يُشتق مفتاحين بـ **HKDF-SHA256**: واحد للتشفير وواحد للكود.
- **كود متغيّر (Rotating code):** مع كل طلب يُرسل كود من 32 حرف يتغيّر **كل 30 ثانية** ومع كل طلب (مثل كود تطبيقات التحقق TOTP)،
  محسوب بـ HMAC-SHA256 من الوقت + رقم عشوائي للطلب. اللي ما عنده المفتاح ما يقدر يصنع كود صحيح.
- **تحقق من الطرفين:** السيرفر يرد بكود خاص فيه أيضاً، والموقع يتأكد منه قبل ما يصدّق الرد (ما أحد يقدر ينتحل السيرفر).
- **منع إعادة الإرسال (Replay):** كل طلب له nonce عشوائي، والسيرفر يرفض أي nonce مستخدم، ويرفض أي طلب عمره أكثر من 30 ثانية.
- **رقم الطلب `ref` (رقم الطلبية/الدفع):** السيرفر يحفظه في `ServerDataBase\webapi_refs.txt` ويرفض تكراره → ما ينصرف نفس الدفع مرتين.
- **قائمة IP مسموحة** (`allow_ips`)، **حد للطلبات بالدقيقة**، **حد أعلى للـ CPs بالطلب**، **قائمة آيتمات مسموحة** (اختياري).
- أي محاولة دخول فاشلة: تأخير نصف ثانية + تسجيل الـ IP في `ServerDataBase\webapi_log.txt`. كل عملية ناجحة تُسجّل (ref، اللاعب، العملية، الكمية).
- العمليات تمر بنفس دوال الـ AdminApi / PlayerStudio: قفل واحد، حفظ اللاعب مباشرة بعد التغيير، والـ CPs تنضاف على دفعات تحت حد الحظر التلقائي.

---

## طريقة التفعيل

1. **اصنع مفتاح** (واحد من الطريقتين):
   - في كونسول السيرفر اكتب: `webapi genkey`
   - أو: `php gen_key.php`
2. **في السيرفر:** انسخ `ServerDataBase\webapi.ini.example` إلى `ServerDataBase\webapi.ini` وعدّل:
   ```ini
   enabled=1
   bind=127.0.0.1
   port=5900
   key=المفتاح_اللي_طلع_لك
   allow_ips=127.0.0.1
   ops=cps,giveitem,inventory
   max_cps_per_request=100000
   cps_modes=add
   cps_offline=0
   allowed_items=
   max_item_count=10
   max_plus=12
   rate_per_minute=30
   ```
   ثم أعد تشغيل السيرفر. لازم يطلع: `[WebApi] listening on 127.0.0.1:5900`.
   (إذا المفتاح فاضي/قصير/غير عشوائي السيرفر **يرفض التشغيل** ويكتب السبب.)
3. **في الموقع:** انسخ `config.example.php` إلى `config.php` وحط **نفس المفتاح** و host/port.
4. **تأكد:**
   - في كونسول السيرفر: `webapi test` → لازم `ALL PASSED` (اختبار التشفير داخلياً بدون فتح بورت وبدون لمس أي لاعب).
   - في الموقع: `php selftest.php` → لازم `ALL PASSED` (يتأكد إن PHP عندك يشفّر بنفس طريقة السيرفر بالضبط).
   - `webapi status` في الكونسول يبيّن هل الـ API شغّال.

### شرح الإعدادات

| المفتاح | المعنى |
|---|---|
| `enabled` | `1` تشغيل، `0` إيقاف (الافتراضي) |
| `bind` | العنوان اللي يسمع عليه. خلّه `127.0.0.1`. عنوان ثاني فقط إذا الموقع على جهاز ثاني في **شبكة داخلية موثوقة** |
| `port` | البورت (5900) |
| `key` | المفتاح السري (base64 لـ 32 بايت) |
| `allow_ips` | الـ IPs المسموح لها تتصل (مفصولة بفاصلة). أي IP ثاني ينقطع فوراً |
| `ops` | العمليات المسموحة: `cps`, `giveitem`, `inventory` |
| `max_cps_per_request` | أكبر كمية CPs في طلب واحد |
| `cps_modes` | `add` فقط (الافتراضي). ممكن تضيف `sub` أو `set` لكن لا تضيفها إلا إذا تحتاجها |
| `cps_offline` | `1` = يقدر يضيف CPs للاعب وهو أوفلاين (يكتب في ملفه مع نسخة احتياطية). `0` = لازم يكون أونلاين |
| `allowed_items` | آيتمات مسموحة: أرقام أو نطاقات `720027,1088000-1088002`. فاضي = أي آيتم |
| `max_item_count` | أكبر عدد للآيتم في طلب |
| `max_plus` | أعلى بلس مسموح (0-12) |
| `rate_per_minute` | عدد الطلبات بالدقيقة (لكل الموقع) |

---

## الملفات

| الملف | وظيفته |
|---|---|
| `QahrApi.php` | الكلاس الأساسي: `sendCps($player,$amount,$ref)`، `giveItem($player,$item,$count,$ref,$plus=0,$bound=false)`، `inventory($player)`. يشفّر ويرسل ويتحقق من رد السيرفر ويفك تشفيره |
| `config.example.php` | مثال الإعدادات. انسخه إلى `config.php` (**لا ترفعه على git** — موجود في `.gitignore`) |
| `_common.php` | دوال مشتركة للأمثلة (تحميل الإعدادات، escape للمخرجات، CSRF) |
| `send_cps.php` | مثال إرسال CPs: `php send_cps.php اسم_اللاعب 1000 order-123` |
| `give_item.php` | مثال إعطاء آيتم: `php give_item.php اسم_اللاعب 1088000 1 order-124` |
| `inventory.php` | مثال عرض الشنطة: `php inventory.php اسم_اللاعب` |
| `gen_key.php` | يطلع مفتاح جديد (من سطر الأوامر فقط) |
| `selftest.php` | اختبار بدون اتصال: يتأكد إن PHP عندك يطابق تشفير السيرفر |
| `.htaccess` / `web.config` / `index.php` | حماية المجلد: يمنعون فتح `config.php` وباقي الملفات من المتصفح |

الأمثلة تشتغل من **سطر الأوامر**. كصفحات ويب تشتغل فقط إذا حطيت `'web_examples' => true` في `config.php` —
وهذا **بعد** ما تحميها بتسجيل دخول الأدمن حق موقعك.

### استخدامه في موقعك (مثال بعد نجاح الدفع)

```php
require 'QahrApi.php';
$c = require '/مسار/خارج/الموقع/qahr_config.php';
$api = new QahrApi($c['host'], $c['port'], $c['key']);
try {
    $r = $api->sendCps($playerName, 5000, 'order-' . $orderId);   // ref = رقم الطلبية (فريد)
    if ($r['ok']) { /* تم */ }
    elseif ($r['error'] === 'duplicate_ref') { /* هذا الطلب انصرف من قبل */ }
    elseif ($r['error'] === 'player_offline') { /* اطلب منه يدخل اللعبة وأعد المحاولة بنفس ref */ }
} catch (QahrApiException $e) { /* السيرفر مقفول أو الشبكة أو المفتاح غلط */ }
```

### رسائل الخطأ (`$r['error']`)

`unauthorized` (مفتاح/كود/وقت غلط أو طلب مكرر) · `rate_limited` · `op_not_allowed` · `invalid_request` · `invalid_amount` ·
`invalid_item` · `invalid_ref` · `duplicate_ref` · `player_not_found` · `player_offline` · `no_space` (الشنطة ممتلئة) ·
`busy_retry` (حاول بعد ثواني) · `shutting_down` · `failed` · `uncertain`.
إذا فشل الطلب (غير `duplicate_ref` و `uncertain`) الـ ref **ما ينحفظ**، فتقدر تعيد نفس الطلب بنفس الـ ref.
`uncertain` = خطأ غير متوقع في السيرفر في نص العملية: **ممكن** تكون اتنفذت، فالـ ref ينحفظ (ما ينصرف مرتين أبداً)
ويتعلّم `UNCERTAIN` في `webapi_refs.txt` — راجع `adminapi_log.txt` يدوياً قبل ما تعطي اللاعب أي شي.
لو PHP قال "السيرفر ما ردّ" (timeout): أعد المحاولة **بنفس الـ ref** — لو كانت اتنفذت هترجع `duplicate_ref`.

ملاحظات: إعطاء الآيتم وعرض الشنطة يحتاجون اللاعب **أونلاين**. اللاعب يُكتب بالاسم أو برقم الـ UID.

---

## ملاحظات أمان مهمة

- **المفتاح سر.** لا ترسله لأحد ولا ترفعه على git ولا تحطه في صفحة يقدر أحد يفتحها. إذا تسرّب: `webapi genkey`، غيّره في
  `webapi.ini` و `config.php`، وأعد تشغيل السيرفر.
- **غيّر المفتاح كل فترة** (مثلاً كل كم شهر).
- **لا تفتح البورت 5900 على الإنترنت أبداً.** الموقع لازم يكون على نفس الجهاز (`127.0.0.1`) أو في شبكة داخلية موثوقة، وخلّ الفايروول يمنع البورت من الخارج.
- **الموقع لازم يكون HTTPS.**
- **مكان `config.php`:** الأفضل **خارج مجلد الموقع** (web root): حط مساره في متغير البيئة `QAHR_WEBAPI_CONFIG`، أو سمّه
  `qahr_config.php` في المجلد اللي فوق هذا المجلد. لو خليته هنا: ملف `.htaccess` (Apache) و `web.config` (IIS) الموجودين
  يمنعون فتح أي ملف من المتصفح غير صفحات الأمثلة الثلاث؛ في nginx لازم تضيف `deny` بنفسك.
- `example.php` و `gen_key.php` و `selftest.php` من **سطر الأوامر فقط** (من المتصفح ترجع 404).
- الأمثلة كصفحات ويب: لازم `web_examples=true` **و** IP الزائر في `web_allow_ips` (الافتراضي: نفس الجهاز بس) **و** تسجيل دخول الأدمن حقك.
- `allowed_items` فاضي = أي آيتم. الأفضل تحط قائمة الآيتمات اللي بتبيعها بس.
- السيرفر يرفض مفتاح اختبار `selftest.php` (البايتات 0..31) — لازم مفتاح عشوائي من `webapi genkey`.
- **صفحات الأدمن** اللي تستخدم هذا الكلاس لازم تكون محمية بتسجيل دخول الأدمن حق موقعك. الكلاس نفسه ما يتحقق مين يستخدم الموقع.
- لا تعطِ CPs إلا بعد ما يتأكد الدفع من بوابة الدفع (من الـ callback/webhook حقها، مو من صفحة يرجع لها المستخدم).
- ساعة الجهازين لازم تكون مضبوطة (الفرق المسموح 30 ثانية).
- السجلات: `ServerDataBase\webapi_log.txt` (كل محاولة)، `ServerDataBase\webapi_refs.txt` (كل ref انصرف)، و `adminapi_log.txt` (نفس عمليات AdminApi).

---

## البروتوكول (للمبرمج)

TCP، سطر JSON واحد بكل اتجاه:

```
طلب:  {"v":1,"ts":<unix>,"nonce":b64(16),"iv":b64(12),"ct":b64,"tag":b64(16),"code":"32 hex"}
encKey  = HKDF-SHA256(key, 32, info="qahr-webapi-enc",  salt="")
codeKey = HKDF-SHA256(key, 32, info="qahr-webapi-code", salt="")
code    = hex(HMAC-SHA256(codeKey, BE64(floor(ts/30)) || nonce))[0..32]     ← يقبل النافذة ±1
AAD     = "v1|" + ts + "|" + nonce_b64
رد:   {"v":1,"ts":<server unix>,"nonce":<نفس nonce الطلب>,"iv":جديد,"ct","tag","code"}
       code = hex(HMAC-SHA256(codeKey, "srv|" || BE64(floor(ts/30)) || nonce))[0..32]
       AAD  = "v1r|" + ts + "|" + nonce_b64
الطلب الداخلي (مشفّر):
  {"op":"cps","player":"name|uid","mode":"add","amount":1000,"ref":"order-123"}
  {"op":"giveitem","player":"..","item":1088000,"count":1,"plus":0,"bound":false,"ref":"order-124"}
  {"op":"inventory","player":".."}
```

كود السيرفر: `Qahr6609.GameServer\Infrastructure\Threading\WebApi.cs` (+ `AdminApi.Web.cs` اللي يمرّر العمليات لدوال AdminApi).
أوامر الكونسول: `webapi status` | `webapi genkey` | `webapi test`.
