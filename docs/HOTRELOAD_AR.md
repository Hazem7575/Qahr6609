# Hot Reload في سيرفر MeGa (زي سيرفر قهر)

فيه نوعين.

## 1. تحديث ملفات الداتا والسيرفر شغال (من غير restart)

اكتب الأمر في كونسول السيرفر، أو في اللعبة بـ `@` (للـ ProjectManager بس)، أو ابعته على الـ admin pipe:

| الأمر | الملف | اللي بيحصل |
|---|---|---|
| `reloadnpcs` | `ServerDataBase\npcs.txt` | الـ NPCs اللي اتحركت أو اتضافت أو اتشالت أو اتغير شكلها تتطبق فوراً، واللاعبين اللي شايفينها يشوفوا التغيير |
| `reloadportals` | `portals.ini` | البوابات الجديدة تشتغل من أول خطوة جوه البوابة |
| `reloadshops` | `shops\*` | Shop.dat و emoneyshop و V2 و Honor و RacePoint و GoldenLeague |
| `reloaditems` | `itemtype.txt` | قيم الآيتمز (سعر، إحصائيات...). آيتمز اللاعبين مش بتتلمس. الآيتم الجديد (ID جديد) محتاج restart |
| `reloadmonsters` | `Monsters\*.ini` | قيم الوحوش، والوحوش الموجودة في الخريطة كمان. أماكن الـ spawn محتاجة restart |
| `reloadmapcells 1002` | خريطة الـ dmap + `map\walk` | أماكن المشي على السيرفر لخريطة واحدة |
| `reloadquests` | داتا QuestEngine | نفس `@quest reload` |
| `reloadall` | كل اللي فوق | |

الحماية:
- الملف بيتقري كامل الأول. لو فيه خطأ، مفيش أي حاجة بتتغير.
- لو الملف لسه بيتكتب، الـ reload بيستنى أو بيرفض.
- لو `npcs.txt` هيشيل أكتر من نص الـ NPCs، الـ reload بيترفض إلا لو كتبت `force` (مثلاً `reloadnpcs force`).
- كل عملية reload بتتسجّل في `ServerDataBase\hotreload_log.txt`.

**تحديث تلقائي (اختياري):** اعمل ملف `ServerDataBase\hotreload.txt` وحط فيه:
```
watch_npcs=1
watch_portals=1
watch_shops=1
watch_items=1
watch_monsters=1
debounce_ms=1000
```
أي ملف من دول يتحفظ، السيرفر يعمل الـ reload بتاعه لوحده بعد ثانية.

## 2. تعديل كود C# والسيرفر شغال (للتطوير بس)

1. اقفل السيرفر العادي.
2. شغّل `Qahr6609.Tools\Server\ServerCtl\dev_hotreload.bat`.
3. عدّل جوه أي دالة موجودة واحفظ الملف: التعديل يتطبق فوراً، وهيظهر في الكونسول سطر بنفسجي `[HotReload] code update applied live`.
4. لو ضفت دالة أو class جديدة، أو غيّرت parameters دالة: `dotnet watch` بيعمل build و restart لوحده.

الـ build ده بيطلع في `Qahr6609.GameServer\bin\Dev` من غير optimize، وبيشتغل من فولدر `bin\Debug` علشان يقرا نفس `shell.ini` ونفس الداتا. السيرفر العادي (`bin\Debug\GameServer.exe` و `apply.bat`) مش بيتأثر بيه.
