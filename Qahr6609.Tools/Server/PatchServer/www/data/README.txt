PatchServer launcher content - how the owner edits it
======================================================

Everything the launcher shows (news, maintenance banner, event countdown) is
driven from the three files in this folder. Edit them with any text editor and
save as UTF-8 (no BOM). The PatchServer re-reads a file within ~1 second of a
change - no restart needed. A missing or broken file never breaks the launcher:
the news falls back to the theme's built-in text, the banner turns off, and the
countdown hides.

The launcher caches the last good news feed on the player's PC, so even if the
PatchServer is offline the player still sees the most recent news.

-----------------------------------------------------------------------------
1) news.json        ->  served at  http://<server>:8800/news.json
-----------------------------------------------------------------------------
List of posts, newest first (the PatchServer sorts by "date" for you). Only the
first post is shown in full; the others are reached with the < > arrows. The 3
thumbnails under the post are the "image" of the 3 most recent posts that have
one.

{
  "items": [
    {
      "tag": "event",                 // update | event | news  (colours the label)
      "title_ar": "عنوان الخبر",
      "title_en": "News title",       // optional; Arabic is used when missing
      "body_ar": "النص بالعربي",      // \n = new line, **bold**, [text](https://link)
      "body_en": "English text",      // optional
      "date": "2026-10-11T12:00:00Z", // ISO-8601; controls the ordering
      "author": "Hazem",
      "image": "news/img/1.jpg",      // optional; put the file under www\news\img\
      "link": "https://..."           // optional; opens when the title is clicked
    }
  ]
}

Images: drop the files in  www\news\img\  and reference them as "news/img/<file>".
A full http(s):// URL works too. Keep images small (the launcher scales them to
~300 px wide). No HTML is executed - only plain text with **bold** and [..](link).

-----------------------------------------------------------------------------
2) status.json      ->  served at  http://<server>:8800/status.json
-----------------------------------------------------------------------------
Turns the amber maintenance banner on/off and (optionally) carries a player
count. The per-server "servers" array is filled in live by the PatchServer; you
only edit the banner block.

{
  "maintenance": true,               // true = show the amber banner + lock PLAY
  "until": "2026-10-12T18:00:00Z",   // optional; shown as "back at ..." (ISO-8601)
  "message_ar": "السيرفر تحت الصيانة لتركيب التحديث.",
  "message_en": "The server is under maintenance for the new patch."
}

Player count (optional): the game server does not report a count today, so the
launcher shows only the ping by default. When you can supply a number, add a
"players" field to a server row in status.json (the launcher shows it next to
the status pill when it is >= 0). Leave it out or use -1 for "unknown".

-----------------------------------------------------------------------------
3) events.json      ->  served at  http://<server>:8800/events.json
-----------------------------------------------------------------------------
Drives the small "<event> in HH:MM:SS" countdown. Mix repeating weekly events
with one-off dates; the launcher always counts down to the nearest one and
rolls to the next when it passes. Empty file = no countdown.

{
  "weekly": [
    { "name_ar": "حرب النقابات", "name_en": "Guild War", "day": "Sat",          "time": "20:00" },
    { "name_ar": "معركة المملكة", "name_en": "Kingdom War", "day": "daily",       "time": "20:30" },
    { "name_ar": "مدينة الشياطين", "name_en": "Demon City", "day": "Mon,Wed,Fri", "time": "19:00" }
  ],
  "events": [
    { "name_ar": "افتتاح الموسم", "name_en": "Season opening", "time": "2026-10-15T20:00:00" }
  ]
}

  day  : Sun Mon Tue Wed Thu Fri Sat  (or 0..6, 0 = Sunday, or "daily"/"everyday")
         several days: a comma list "Mon,Wed,Fri" or a JSON array ["Mon","Wed"].
  time : HH:mm in the players' local time (same region as the server here).
  one-off "events": ISO-8601 date-time; past entries are ignored automatically.

-----------------------------------------------------------------------------
4) crashes\            <-  written here by the server (you only READ it)
-----------------------------------------------------------------------------
When the game closes abnormally, the launcher saves a crash report on the
player's PC AND uploads it here (POST /crash) so you get it without the player
having to send a screenshot. Reports land in:

  www\data\crashes\<date>\crash_<time>_<ip>_<kind>.txt

one folder per day. Each file starts with a line the server added (received
time, player IP, launcher version, optional note) and then the report itself:
exit code, OS / DirectX / VC++ runtime / disk, the system-check results, and
the last lines of the client's own log/dump. No passwords are ever included -
the launcher never handles the game login.

In the PatchServer console type  crashes  to see how many arrived and where.

Turn it off or change the size cap in PatchServer.ini:
  [Crash]
  Enabled=1        ; 0 = reject uploads (reports then stay on the player's PC only)
  MaxBytes=524288  ; largest report accepted (abuse cap; reports are small text)

On the launcher side it is controlled by launcher.ini:
  [SystemCheck]
  CrashUpload=1    ; 0 = keep the report local, do not upload
  CrashUploadUrl=  ; empty = send to the same patch server (UpdateUrl + "crash")
