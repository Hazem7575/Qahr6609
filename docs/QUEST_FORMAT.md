# QuestEngine data format (generic official quests)

> ملخص للمالك: ده شرح ملفات الكويستات اللي الـ QuestEngine بيقراها (`quests.ini` و `quests_*.ini`).
> أي كويست رسمي ملوش سكريبت في السورس يتكتب هنا كداتا بس، والموديول بيضيف "Quest: ..." / "Complete: ..."
> لأول ديالوج لأي NPC (حتى لو عنده سكريبت) وبعدين "Something else" بيرجع لسكريبت الـ NPC العادي.

Engine: `Qahr6609.GameServer\Modules\QuestEngine\QuestEngine.cs`.
Keep this file in sync with `QuestEngine.Parse()`.

## 1. Files and loading

* Folder: `<DbLocation>\Modules\QuestEngine\` (live: `Qahr6609.GameServer\bin\Debug\ServerDataBase\Modules\QuestEngine\`).
* Loaded at start-up and by `@quest reload`: **`quests.ini` first, then every `quests_*.ini` in alphabetical order**
  (`quests_daily.ini`, `quests_region.ini`, `quests_story.ini`, ...).
* A quest id may be defined only once. A later file that repeats an id is **ignored for that quest** and listed as
  `DUPLICATE` in the console and in `load_report.txt`.
* A quest section that fails to parse is skipped (`BAD ...` with the reason); the rest still loads.
* After every load the engine writes **`load_report.txt`** in the same folder: bad/duplicate quests and warnings
  (NPC not standing at the given map/x/y, unknown item ids, monster names that match no monster, prerequisites
  that do not exist, quest ids that are not in the server `Questinfo.ini`). `@quest check` shows the first lines in game.
  Always reload and read this report after editing data.
* Encoding: Windows-1252. Lines starting with `;`, `#` or `//` are comments. Keys are case-insensitive.
  A value runs to the end of the line (there are no inline comments: `;` inside a value is part of the value).
* Every `[section]` that contains `Id=` is one quest. The section name itself is free (use the MissionId or a counter).
  Sections without `Id=` (e.g. `[Quests] Count=..`) are ignored, except `[NpcAlias]` (see 2.6).

## 2. Quest keys

### 2.1 Identity

| Key | Required | Meaning |
|---|---|---|
| `Id` | yes | Official `MissionId` from the client `ini\Questinfo.ini`. Must also be in the server `ServerDataBase\Questinfo.ini`, otherwise the client Quest List window does not show it (warning in the report). Keep the official id so the client shows the official name/text/[npc] links. |
| `Name` | no | Shown in NPC options ("Quest: Name", max 32 chars) and system messages. Default `Quest <Id>`. |
| `Type` | no | Official `TypeId` (info only; the window type comes from the client file). |
| `Content` | no | Long text shown by the begin NPC when the player opens the quest (official `Content`). |
| `Desc` | no | Short text (official `IntentionDesp`), shown after accepting. Used when `Content` is empty. |
| `Prize` | no | Reward text shown before accepting (official `Prize`; `[item id,...]` tags are replaced by item names, `\n` by a space). If empty the engine builds the text from the P* keys. |
| `Note`, `NeedsNpc`, anything else | no | Ignored (free notes for maintainers). |

### 2.2 NPCs

| Key | Meaning |
|---|---|
| `Begin=npcId,map,x,y,Name` | NPC that offers the quest. `map,x,y` = where it stands (used for the path guide and the load check). `Name` is used in messages ("talk to Name"). Official `BeginNpcId` without the 6th field (map name), leading zeros are fine (`0004`). **`0,0,0,0,Name` = no begin NPC**: then use `AutoAccept=1` or `StartItem=`. |
| `Finish=npcId,map,x,y,Name` | NPC that completes it. Missing = same as `Begin`. **npcId 0 = finishes by itself** as soon as all objectives are done (no NPC talk, reward given automatically). |
| `NpcAlias=official:serverUid[,official:serverUid...]` | This quest uses `serverUid` wherever the data says `official` (Begin, Finish, objective NPCs). For NPCs whose official uid is taken by another object on this server (e.g. QuizHolder 6168 = a Training Ground stake in SobNpcs.txt). |

Any NPC works - no C# handler is needed. Every click on an NPC that stands on the player's screen goes to the
engine first (core hook `ModuleManager.NpcTalk`, COORDINATION R1). NPCs **with** a C# script get the quest options
added in front of their own first dialog (+ "Something else." which runs their script); NPCs **without** a script show
the engine dialog only (and a short "Quest updated" dialog when the click only counted for a `talk`/`npc`/`deliver`
objective). The NPC must be spawned (Npcs.txt, a normal NPC - SobNpcs/statues/guild-war objects do not go through
the hook) at the given position (checked in `load_report.txt`). Engine option ids are 200-249.
Not supported (yet): summoning quest monsters, a rebirth reward. Quest monsters need ordinary spawns.

### 2.3 Who can take it (gates)

| Key | Default | Meaning |
|---|---|---|
| `LvMin`, `LvMax` | 0, 0 | Level range. `LvMax=0` = no maximum. Ignored for `StartItem` quests. |
| `RebornMin`, `RebornMax` | 0, 255 | Reborn count range. |
| `Prof` | 0 | Exact class ids, comma or `|` separated (official `Profession`, e.g. `10,11,12,13,14,15`). `0` = all. |
| `ProfGroup` | 0 | Class groups = class / 10: 1 Trojan, 2 Warrior, 4 Archer, 5 Ninja, 6 Monk, 7 Pirate, 8 Dragon Warrior, 10 Taoist (100/101), 13 Water, 14 Fire, 16 Windwalker. |
| `Sex` | 0 | 0 all, 1 male, 2 female. |
| `Pre` | 0 | Prerequisite quests, **any one** of them finished (official `Prequest`, `650|655|656` or `650,655`). The prerequisite may be an engine quest or a scripted quest (it reads the player's quest list). |
| `PreAll` | 0 | Prerequisites that must **all** be finished. |
| `Exclude` | 0 | Not offered if the player has any of these quests (taken or finished) - for branching chains. |
| `Start`, `End` | - | `yyyy-MM-dd`: available from Start 00:00 until the end of End (official `ActivityBeginTime/EndTime`). |
| `Season` | - | Yearly window `mm-dd..mm-dd`, both days inclusive, every year; may wrap the year end (`12-13..01-03`). Use this for festival quests (official `ActivityBeginTime/EndTime` are 2009-2017 dates; `ActivityBegin`/`ActivityEnd` keys are **not read**). An accepted quest can still be handed in after the window closes. |
| `Days` | - | Weekdays it can be taken: `1`=Monday ... `7`=Sunday, e.g. `Days=6,7`. |
| `Hours` | - | Time windows `HH:mm-HH:mm[,HH:mm-HH:mm]` (end exclusive), e.g. `Hours=20:30-21:30`. |
| `NpcOnly` | 0 | 1 = accepting from the Quest List window is undone (must be taken at the begin NPC). |

Gates are checked when the NPC offers the quest, when it is accepted, and for quests accepted from the client
Quest List window (the window accept is undone with a message when a gate fails). A player can run 5 quests at once.

### 2.4 How it starts and repeats

| Key | Meaning |
|---|---|
| (default) | Offered by the begin NPC on the first click ("Quest: Name" -> text + reward -> "I accept."), or accepted from the Quest List window. |
| `AutoAccept=1` | Accepted automatically (tick, ~2 s) as soon as the player passes the gates (typically `Pre` finished). For official quests that start on an event/kill, Begin NPC 0. |
| `StartItem=<itemId>` | Accepted automatically when the player has this item in the bag; **LvMin/LvMax are ignored**. |
| `GiveItems=id:count[;id:count]` | Items handed to the player on accept (bound), e.g. a letter to deliver. |
| `TeleportOnAccept=map,x,y` | Teleports the player there right after accepting (instead of the path guide). |
| `TeleportOnFinish=map,x,y` | Teleports the player there after the reward. |
| `Reset=daily` (= `Daily=1`) | Can be taken again after the next server-day 00:00 once finished. |
| `Reset=weekly` (= `Weekly=1`) | Again from the next Monday 00:00. |
| `Reset=repeat` (= `Repeat=1`) | Can be taken again right after finishing. |

Without Reset a quest is done once per character. A finished repeatable quest stays "finished" in the list (so
chains that need it as `Pre` keep working) until it is taken again.

### 2.5 Rewards

| Key | Meaning |
|---|---|
| `PExp=n` | Raw experience (`client.IncreaseExperience`). |
| `PExpBall=n` | Experience in exp-ball units where **600 = one EXP ball** (`GainExpBall`): 100 = 1/6 ball, 20 = 1/30, 1200 = 2 balls. |
| `PSilver=n` | Silver. |
| `PCps=n` | **Bound** CPs. |
| `PStudy=n` | Study points. |
| `PChi=n` | Chi points. |
| `PItems=id:count:plus:bound[;...]` | Items. `count` (default 1, max 40), `plus` (0-12), `bound` 1 = bound. Short forms `id` and `id:count` work. Unknown ids are skipped (warning). |
| `PItemMode=all` | (default) all PItems. |
| `PItemMode=choose` | The finish NPC shows a "Choose your reward" list (max 10). Auto-finished quests fall back to `class`. |
| `PItemMode=class` | The first item the player's class can equip (official "one of the following weapons"). |
| `PItemMode=random` (= `PRandom=1`) | One random item of the list. |

Notes: the core `QuestGUI.FinishQuest` already gives every finished quest 1 EXP ball (600) below level 130 -
do not add it again. Items needed for the quest (`item`/`collect` objectives) are taken away when the quest is
completed; the inventory must have room for the reward items.

### 2.6 Global NPC aliases

```ini
[NpcAlias]
6168=61680      ; official QuizHolder uid -> uid spawned on this server
```
Any file may have a `[NpcAlias]` section; it applies to all quests in all files (a quest's own `NpcAlias=` wins).

## 3. Objectives

```ini
Obj=2
Obj1=kill|10|1|Pheasant|1002|341|401|Pheasants
Obj2=talk|4|1002|284|266|Armorer
```
`Obj=n` then `Obj1`..`Objn`, fields separated by `|`. No objectives (`Obj=0`) = "go and talk to the finish NPC".
All objectives must be done; then the finish NPC shows "Complete: Name".

**Slots.** The client quest window shows counters with `[chk_quest MissionId,slot,?,count]` in the official
`Intention` text: `slot` is 1-based and must be the official one so the window shows the right counter. Where a
field `slot` exists, write the official slot; `0` = the engine picks a free slot itself (use 0 when the official
text has no `chk_quest` for it). Item counters are shown by the client itself via `[chk_item ...]` and have no slot.

`map,x,y` of an objective is used for the path guide ("Next: ..." box after accepting; OK walks there with the
client's auto-path) and for place checks. `map 0` = no place (no guide).

| Kind | Fields | Done when |
|---|---|---|
| `talk` | `talk|npcId|map|x|y|label` | The player clicked that NPC while the quest runs (own slot, automatic). |
| `npc` | `npc|npcId|count|slot|map|x|y|label` | Clicked that NPC `count` times (official counted visits). |
| `kill` | `kill|count|slot|Monster|map|x|y|label[|flag]` | Killed `count` monsters named `Monster` (the server monster name, case/space/symbol-insensitive: `Pheasant`, `SandMonster`). `*` = any monster. Flag `onmap` = only kills on `map` count; `near` = only within 40 cells of x,y. Without flag the name counts anywhere. |
| `spot` | `spot|count|slot|map|x|y|label` | Monsters of any kind killed within 40 cells of x,y, **or** standing within 4 cells of x,y (sets it complete). For official targets that have no clear monster name. |
| `visit` | `visit|slot|map|x|y|radius|label` | The player stood within `radius` cells of x,y (not in an instance). |
| `item` | `item|itemId|count|map|x|y|Monster|label[|flag]` | Has `count` of the item in the bag (taken at completion). **Without Monster**: the item is handed over on accept when the spot is at the begin NPC (within 5 cells), picked up every 5 s while standing within 3 cells of the spot, and drops (50%) from kills within 40 cells. **With Monster** (drop only): 50% drop from that monster (and from kills near the spot); the spot is only the path guide. Flag `drop` / `pickup` forces either behaviour. |
| `collect` | `collect|itemId|count|Monster|map|x|y|chance|label` | Has `count` items; they drop (into the bag) with `chance`% per kill of `Monster` (`*` any) on `map` (0 = any map). Taken at completion. |
| `have` | `have|itemId|count|map|x|y|label` | Has the items (bought, crafted...). **Not** taken at completion. |
| `deliver` | `deliver|npcId|itemId|count|map|x|y|label` | Clicked `npcId` while having the items: they are taken and the objective is done (bring X to someone other than the finish NPC; for the finish NPC use `item`). |
| `use` | `use|itemId|count|slot|map|x|y|radius|label` | Used the item `count` times (each use consumes one; the normal item effect does not run). `radius 0` or map 0 = anywhere; else only within `radius` cells of x,y (the player is guided there otherwise). |
| `level` | `level|n|slot` | Player level >= n (counter = current level). |
| `reborn` | `reborn|n|slot` | Reborn count >= n. |
| `pk` | `pk|count|slot|map|x|y|label` | Killed `count` players (`map` 0 = anywhere). |
| escort | - | Not supported. |

Labels are free text for messages ("Pheasants 3/10", "Next: kill 10 Pheasants").

### Item ids, monster names, NPC ids
* Items: `ServerDataBase\itemtype.txt` (= official itemtype for the official ids). Quest items are added bound.
* Monsters: names as in the server monster table (`Monsters\*.ini` / cq_monstertype) - the report warns on unknown names.
* NPCs: uid of the spawned NPC (Npcs.txt / SobNpcs.txt). Use the official uid; use `NpcAlias` when it differs.

## 4. Converting an official Questinfo.ini entry

| Questinfo.ini | quests ini |
|---|---|
| `MissionId` | `Id` |
| `Name`, `TypeId`, `Content`, `IntentionDesp`, `Prize` | `Name`, `Type`, `Content`, `Desc`, `Prize` (+ P* keys for the real reward) |
| `Lv_min`, `Lv_max`, `Profession`, `Sex`, `Prequest` | `LvMin`, `LvMax`, `Prof`, `Sex`, `Pre` |
| `BeginNpcId`, `FinishNpcId` (`id,map,x,y,name,mapname`) | `Begin`, `Finish` (`id,map,x,y,name`) |
| `ActivityBeginTime`, `ActivityEndTime` | `Season=mm-dd..mm-dd` (yearly; the official years are long past). `Start`/`End` only for real future dates. |
| `ActivityType=2` (daily quests) | `Reset=daily` (or weekly when the name says so) |
| `IntentionN` with `[mob Name,map,x,y][chk_quest id,slot,?,count]` | `kill|count|slot|Name|map|x|y|label` |
| `IntentionN` with `[npc Name,id,map,x,y]` | `talk|id|map|x|y|Name` (or the finish NPC itself: no objective) |
| `IntentionN` with `[item_t ...][chk_item id,count]` | `item` / `collect` / `have` |

## 5. Example

```ini
[608]
Id=608
Name=Hunt monster 2
Type=1
LvMin=4
LvMax=50
Pre=607
Begin=2001,1002,307,338,TCCaptain
Finish=2001,1002,307,338,TCCaptain
Content=Hunt 10 Pheasants and then report back to the TC Captain.
Desc=TCCaptain asks you to hunt 10 Pheasants.
Prize=EXP: 3000
PExp=3000
Obj=1
Obj1=kill|10|1|Pheasant|1002|341|401|Pheasants

[6127]
Id=6127
Name=The Heavy Hammer (Daily)
LvMin=67
LvMax=76
Begin=8437,1000,472,345,GeneralZhuGe
Reset=daily
PExpBall=100
PSilver=2000
PItems=1088000:1:0:1;1088001:1:0:1
PItemMode=random
Obj=2
Obj1=collect|720978|1|HillMonster|1000|500|500|30|Heavy Hammer
Obj2=kill|15|2|SandMonster|1000|637|568|Sand Monsters|onmap
```

## 6. GM commands

`@quest` (active engine quests + counters), `@quest info <id>`, `@quest npc <npcId>` (what that NPC offers you and why
not), `@quest accept <id>`, `@quest done <id>` (fill objectives), `@quest finish <id>` (mark finished, no reward - to
test chains), `@quest reset <id>` (remove from your list), `@quest reload` (re-read all files), `@quest check` (report).
