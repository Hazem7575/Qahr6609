# مهمة: إصلاحات الأمان – المرحلة 1 (سيرفر Qahr6609)

أنت شغال على السورس المحلي في `D:\conquer\Qahr6609`، وهو سيرفر Conquer 6609 مكتوب بـ C# على .NET 4.8. ملف المشروع `Qahr6609.GameServer\GameServer.csproj`، والـ Build بيتعمل بـ `Qahr6609.Tools\Server\ServerCtl\step2_build.bat`.
المهمة إصلاحات دفاعية مركّزة اكتشفناها في فحص الكود. صلّح أصل المشكلة، وما تضيفش features جديدة.

## قواعد إلزامية
- اعمل git branch جديد باسم `security-phase1`، واعمل commit لكل بند لوحده برسالة واضحة.
- ما تغيّرش أي سلوك شرعي للاعب، وما تعيدش تنظيم الكود في المرحلة دي.
- السيرفر ممكن يكون شغال، فما تلمسش `Qahr6609.GameServer\bin\Debug\`. اعمل Build في `_buildcheck\game\` بس، ولازم يطلع `GAME EXIT 0` بعد كل بند.
- الـ csproj من النوع القديم، يعني أي ملف `.cs` جديد لازم تضيفه يدويًا كـ `<Compile Include>`.
- ما تكتبش في التقارير أو الكومنتات طرق استغلال. اكتب الإصلاح وسببه بس.
- لو لقيت حاجة مختلفة عن الوصف ده، وثّقها وكمّل.
- في الآخر اكتب `docs\security\PHASE1_REPORT.md` فيه: كل بند، والملفات اللي اتعدلت، وإزاي اتختبر، وإيه اللي محتاج تجربة جوه اللعبة.

## الأدوات المشتركة (ابدأ بيها)
اعمل ملف `Qahr6609.GameServer\Game\Security\Guard.cs` (namespace `Conquer6609.Security`) فيه:
1. `bool ValidAmount(ulong value, long max, out long amount)`: ترفض الصفر، وأي قيمة أكبر من `long.MaxValue`، وأي قيمة أكبر من `max`. لو القيمة سليمة، ترجعها في `amount` كـ `long`.
2. `int ClampCount(long requested, int max)` عشان تحد أي loop عدّاده جاي من الكلاينت.
3. `void Audit(string category, uint uid, string name, string details)`: يكتب لوج تدقيق في ملف يومي على background queue. ممنوع يسجل أي باسورد.
4. حالة الحوار لكل لاعب: `NpcDialogState` فيه:
   - `NpcId`
   - `HashSet<byte> OfferedOptions`
   - `DateTime At`
   - `RecordOption(client, npcId, optionId)`
   - `bool IsAllowed(client, npcId, option)`

## البنود

### 1) حماية الدخول للعبة
- **المشكلة:** سيرفر اللعبة بيقبل أي قيمة login تطلع UID في الرينج المسموح، ومش بيتأكد إن الحساب ده عدّى فعلًا على الـ AccountServer.
- **الأماكن:**
  - `Qahr6609.AccServer\Program.cs` (حوالي سطر 175، مكان إنشاء الـ identifier)
  - `Qahr6609.GameServer\GShield\HazemShield.cs` في `RequestLogin`
  - `Qahr6609.GameServer\Game\MsgServer\MsgLoginClient.cs` (سطور 60-90 و 217-284)
- **المطلوب:**
  1. الـ AccountServer، بعد ما الباسورد يطلع صح، يعمل token عشوائي 64-bit من `RNGCryptoServiceProvider`.
  2. يسجله في جدول جديد `login_tokens` فيه: `EntityID`, `Token`, `IP`, `ExpiresAt` (بعد 60 ثانية)، و `Used`. اعمل migration SQL في `docs\security\login_tokens.sql`.
  3. يبعته للكلاينت بنفس الحقل الحالي، أو حقل موجود في الـ Forward packet لو الحجم يسمح. **اتأكد من شكل الباكت الأول.** لو الكلاينت مش هيقبل تغيير الشكل، اربط الـ token بالـ UID والـ IP ووقت الصلاحية مع الحفاظ على شكل الباكت.
  4. سيرفر اللعبة يتأكد من الـ token في الداتابيز قبل `LoadCharacter`: يكون موجود، ولسه صالح، ومن نفس الـ IP، وما اتستخدمش. بعدها يعلّمه إنه اتستخدم. لو فشل، اقفل الاتصال وسجّل `Audit`.
  5. لو كان فيه منطق "طرد الجلسة القديمة"، يشتغل بس بعد ما الـ token يعدّي.

### 2) التحقق من المبالغ على 64-bit
أي مكان بيعمل `int x = (int)dwParam; if (x <= 0)` وبعدين يستخدم `dwParam` كامل، استبدله بـ `Guard.ValidAmount`، واستخدم نفس القيمة اللي اتحقق منها في كل العمليات. الأماكن:
- `MsgItemUsuagePacket.cs`: إيداع وسحب فلوس المخزن (حوالي 1653-1694). ضيف حد أقصى لـ `WHMoney`.
- `Role\Instance\Trade.cs`: إضافة فلوس وCPs للتريد (حوالي 38 و 62).
- `Role\Instance\Vendor.cs` في `AddItem`: السعر لازم يكون أكبر من صفر وأقل من حد أقصى، وده يتطبق في كل مسارات `AddItem` في `MsgItemUsuagePacket.cs` (حوالي 1483).
- `MsgNobility.cs`:
  - التحقق بالقيمة الكاملة.
  - تكلفة CPs والـ Bound CPs تتقرّب لفوق (ceil) ولازم تكون 1 على الأقل.
  - حد أقصى للتبرع.
- `MsgGuildProces.cs` (حوالي 174 و 203)، و `MsgAdvertiseGui.cs` (Donation لازم يكون أكبر من صفر ولحد سقف معيّن).
- دوّر على النمط ده في كل المشروع (`(int)dwParam` و `(int)UID` وما يشبهها) وصلّحه، وحط القائمة الكاملة في التقرير.

### 3) حوارات الـ NPC
- **المكان:** `Qahr6609.GameServer\Game\MsgNpc\Dialog.cs` و `Procesor.cs`.
- **المطلوب:**
  - `Dialog.AddOption` تسجل الخيار في `NpcDialogState` للاعب.
  - في `Procesor` قبل استدعاء الـ handler: لو `Option != 0`، والخيار مش من آخر حوار اتعرض لنفس الـ NPC، أو عدّى عليه أكتر من 5 دقايق، ارفض وسجّل `Audit`. استثني `Option 255` (إغلاق) وأي `Input` نصي متوقع.
  - شغّل الإعداد ده بـ config flag اسمه `StrictNpcOptions=1` في `shell.ini` عشان لو حصلت مشكلة تقدر ترجع فيه بسرعة.
- **الـ NPCs اللي في القائمة الطويلة في `Procesor.cs` (حوالي 499-552، أسماء Pack/Box/Packet/Token):**
  - كل handler لازم يتأكد إن أيتم الباك موجود في الشنطة **ويتشال** قبل ما يدّي أي مكافأة: `if (!client.Inventory.Remove(...)) return;`.
  - الـ handlers اللي مالهاش أيتم يفتحها (Garment/Mount/Accesory Packet) تتشال من القائمة، أو تتربط بأيتم.
  - عدّي على باقي `NpcHandler.cs` بنفس القاعدة: أي `Inventory.Remove(...)` نتيجته مش متشافة وجنبها `Add` أو مكافأة لازم تتصلح. ابدأ بالـ handlers اللي بتدّي CPs أو أيتمز غالية. حط القائمة في التقرير.

### 4) خيارات الـ GM المخفية في NpcHandler
- في الـ NPC اللي فيه إنشاء حساب وتغيير باسورد ومشاركة VIP (حوالي سطور 22050-22400):
  - امسح إنشاء الحساب وتغيير الباسورد من جوه اللعبة خالص.
  - مشاركة VIP: لازم تعيد التحقق من مستوى الـ VIP وتاريخ انتهائه في نفس الخطوة.
- `Player.VipLevel` (حوالي 3449-3463) ما يرجعش مستوى الـ VIP المشارَك بعد ما `ShareEnds` يعدّي، والإدخالات المنتهية تتشال.

### 5) صلاحيات الـ GM
- امسح `case "pass"` من `MsgMessage.cs` (حوالي 399-411).
- الصلاحية تتحمّل وقت الدخول من `accounts.State` عن طريق `EntityID = Player.UID`. القيم: 255 = PM، 3 = GM، 5 = Helper. أي قيمة تانية أو أي خطأ في الداتابيز = لاعب عادي.
- `GameMaster` و `ProjectManager` يتحددوا من القيمة دي.
- **قبل ما تمسح `@pass`:** اكتب في التقرير أمر SQL لصاحب السيرفر يحط `State=255` لحسابه.
- لوج الأوامر (حوالي 387-395) ما يكتبش وسيطات أي أمر من لاعب مش GM.
- أوامر الـ GM الحقيقية تتسجل بـ `Guard.Audit`، لأن الـ prefix الحالي `[GMLogs]` مالوش فرع في `Logs.cs` ومش بيتسجل أصلًا.
- استبدل الاستثناءات المبنية على `Name.Contains("[GM]")` في `Player.cs` (حوالي 3133 و 3192 و 3217 و 3284) بالتحقق من الصلاحية الفعلية.

### 6) Loops عدّادها من الكلاينت
- `MsgUpdateItem.cs` (حوالي 17-32): عدّاد `byte` على `Count` جاي من الكلاينت. حوّله لـ `int` وحطله حد أقصى معقول، ومتناسب مع طول الباكت.
- `MsgItemExtra.cs` (حوالي 241-262 و 333): نفس المشكلة بعدّاد `ushort`.
- `PrestigeRank\GenericRank.cs` (حوالي 18 و 147 و 195): `Count` من الكلاينت يتحدد بعدد الصفوف الحقيقي.
- `MsgSubClass.cs` (حوالي 41): حجم الـ array لازم يكون بين صفر وحد أقصى.
- `ServerSockets\Packet.cs` في `ReadUnsafe`: يتحقق من الحجم الحقيقي للباكت مش `MAX_SIZE`.

### 7) عجلة المكافآت `MsgTaskReward.cs`
- `Redraw` يشتغل بس لو فيه Draw معلّق.
- جدول الجوائز يتحفظ وقت الـ Draw، وما يتاخدش من الكلاينت بعد كده.
- `Times10Spinning` يصفّر `TaskReward` في الآخر.

### 8) `ClikerEntry` في `MsgDataPacket.cs` (حوالي 547-671)
- كل فرع يتأكد من: اسم أو ID الوحش المطلوب للكويست، والمسافة (3 خلايا أو أقل)، و `Boss == 0`.
- `MsgQuestList` (حوالي 87-100) ما يقبلش كويست مش معروض من NPC. اعمل whitelist.

### 9) الحركة
- **Jump** في `MsgDataPacket.cs` (حوالي 1496-1578):
  - رجّع `ValidLocation`.
  - ارفض أي إحداثيات برة الماب قبل ما تعدّل X/Y.
  - حد أقصى للمسافة: حوالي 18 خلية (إعداد قابل للتعديل).
  - فاصل زمني أدنى بين كل jump والتانية.
- **`SpeedProtect.JumpProdect`:** الفواصل اللي أقل من 100ms تتحسب مخالفة.
- **`MsgMovement.cs`:**
  - نادي `MovementProdect`.
  - رجّع `ValidLocation` على الخلية الجديدة.
  - وضع Steed بيشترط الـ Ride flag.
- **`HazemShield.SendForceClose`:** شيل `Thread.Sleep(2000)` من الـ thread بتاع الباكتات، واستبدله بإغلاق مؤجّل.

### 10) تكرار الأيتمز
- **`Inventory.cs`:** اعمل دالتين، `RemoveOne(uid)` و `RemoveWhole(uid)`، وكل واحدة ترجع `bool` حقيقي. حدّث `Update(REMOVE)` بحيث ترجع `false` لو مفيش حاجة اتشالت.
- **`MsgCoatStorage.cs` (Equip حوالي 186-221، و Update حوالي 151-180):**
  - الأيتم يتشال كله من الشنطة لما يدخل الوردروب.
  - التجديد بيشترط إن الـ `ITEM_ID` يطابق، وما تنزلش قيمة `Bound` أبدًا.
- **`MsgWarehouse.cs` (Sash، حوالي 183):** استخدم `Removefull: true`، وامنع إيداع الساش جوه ساش.
- **`MsgItemUsuagePacket.cs`:**
  - تبديل الجارمنت (حوالي 633-667): امنع تكرار الـ UIDs، والمواد تتشال قبل ما الجارمنت يتضاف.
  - Honor Shop (حوالي 1871): ارفض الكمية صفر، واحسب التكلفة بـ `checked`.
- **`MsgItemRefineOpt.cs`:** الأحجار من الشنطة بس، ومن غير UIDs مكررة.
- **`MsgUpdateItem.cs`:** في رفع الجودة، كل مادة لازم تكون DragonBall.
- **قاعدة عامة:** الأيتم اللي في تريد أو بوث ما ينفعش يتعمل عليه وردروب أو مخزن أو تطوير.

### 11) الحفظ وعدّاد UID
- **كل ملفات الحفظ** (`ServerDatabase.cs` في Items/Profs/Spells/Mail حوالي 1155 و 1253 و 1324 و 1358، و `RoleQuests.cs`، و `DBActions\Write.cs`):
  - اكتب في `.tmp` وبعدين استبدل الأصلي بـ `File.Replace` مع الاحتفاظ بنسخة `.bak`.
  - **في التحميل:** اتأكد من حجم الملف. لو الملف بايظ ارجع للـ `.bak`. لو الاتنين بايظين، ارفض الدخول وما تحمّلش شخصية فاضية.
- **في الحفظ:** خد snapshot الأول (`ToArray()`)، واكتب عدده الحقيقي.
- **`Database.Server.ITEM_Counter`:**
  - استخدم `Interlocked.Increment`. لو `Extensions.Counter` مش بيدعمها، اعمل wrapper.
  - عند تشغيل السيرفر، العدّاد يبدأ من أكبر قيمة بين المحفوظ في الإعدادات وأكبر UID موجود في `PlayersItems\*.bin`.
- **`RoleQuests.Load`:** اقفل الملف بعد القراءة.

### 12) الشات
- **`MsgMessage.cs`:**
  - بعد `Deserialize`: `msg._From = client.Player.Name;`.
  - حدّد `Color` و `ChatType` بقيم مسموحة بس.
  - اتحقق من طول `ReadStringList` قبل ما تقرا العناصر.
  - انقل لوج الشات بعد الـ Deserialize، وخلي الكتابة على background queue.

## الاختبارات
- اعمل `tests\Security.Tests\` كـ console exe بيعمل compile للأدوات المشتركة لوحدها، ويغطي:
  - `ValidAmount` (الحدود، الصفر، القيم الكبيرة)
  - `ClampCount`
  - `NpcDialogState`
  - الحفظ atomic والرجوع للـ `.bak` (بملفات مؤقتة)
  - عدّاد UID من threads متعددة من غير تكرار
- **Build:** `Qahr6609.Tools\Server\ServerCtl\step2_build.bat` لازم يطلع `GAME EXIT 0` للجيم و `AUTH EXIT 0` للـ auth.
- **دخول تجريبي على سيرفر test منفصل:** دخول، حركة، تريد، مخزن، NPC عادي، حفظ، ثم logout/login. اكتب النتيجة في التقرير. لو ما قدرتش تشغّل سيرفر test، اكتب ده صراحة.

## الترتيب
1. الأدوات المشتركة.
2. البنود 6 و 2 و 7 و 12 (صغيرة وأثرها فوري).
3. البنود 3 و 4 و 5.
4. البنود 10 و 8 و 9.
5. البند 11.
6. البند 1 (محتاج تنسيق مع شكل الكلاينت).
7. الاختبارات والتقرير.

Build بعد كل بند.
