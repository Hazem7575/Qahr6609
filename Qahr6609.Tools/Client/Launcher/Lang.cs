using System;
using System.Collections.Generic;
using System.Windows;

namespace ScriptHazemLauncher
{
    /// <summary>Built-in UI strings in Arabic and English. Theme texts can override some of them (see Theme.Text).</summary>
    public static class Lang
    {
        public static string Current { get; private set; } = "ar";

        public static bool IsArabic { get { return Current == "ar"; } }

        public static FlowDirection Flow { get { return IsArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight; } }

        public static void Set(string code)
        {
            Current = string.Equals(code, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "ar";
        }

        private static readonly Dictionary<string, string[]> S = new Dictionary<string, string[]>
        {
            // key                      arabic                                          english
            { "Downloading",        new[] { "جاري التنزيل:",                              "Downloading:" } },
            { "TotalProgress",      new[] { "التقدم الكلي:",                              "Total Progress:" } },
            { "Checking",           new[] { "جاري البحث عن تحديثات...",                   "Checking for updates..." } },
            { "CheckingFiles",      new[] { "فحص الملفات",                                "Checking files" } },
            { "DownloadingUpdates", new[] { "جاري تنزيل التحديثات...",                    "Downloading updates..." } },
            { "UpToDate",           new[] { "اللعبة محدّثة وجاهزة",                        "The game is up to date" } },
            { "Updated",            new[] { "تم التحديث ({0} ملف)",                        "Update complete ({0} files)" } },
            { "UrlEmpty",           new[] { "رابط التحديثات فاضي في launcher.ini",         "UpdateUrl is empty in launcher.ini" } },
            { "UpdateOffline",      new[] { "سيرفر التحديثات مش متاح",                     "Update server is not reachable" } },
            { "PlayAnyway",         new[] { "تقدر تلعب بالنسخة الحالية",                   "you can still play the current version" } },
            { "UpdateError",        new[] { "خطأ في التحديث: ",                            "Update error: " } },
            { "BadDownload",        new[] { "الملف {0} اتنزل ناقص أو بايظ، جرّب تاني.",     "{0} was not downloaded correctly, please try again." } },
            { "LauncherUpdated",    new[] { "اللانشر اتحدّث، هيتقفل ويفتح تاني.",           "The launcher was updated and will restart." } },
            { "Online",             new[] { "السيرفر شغال",                                "Online" } },
            { "Offline",            new[] { "السيرفر مقفول",                               "Offline" } },
            { "SrvOnline",          new[] { "شغال",                                         "Online" } },
            { "SrvOffline",         new[] { "مقفول",                                        "Offline" } },
            { "SrvMaintenance",     new[] { "صيانة",                                        "Maintenance" } },
            { "LoginOffline",       new[] { "سيرفر الدخول مقفول",                            "Login server offline" } },
            { "ServersOnline",      new[] { "السيرفرات الشغالة: {0}",                         "Servers online: {0}" } },
            { "PickServer",         new[] { "اختار السيرفر (هيظهر أول واحد في اللعبة)",        "Pick a server (it is listed first in the game)" } },
            { "ServerListFailed",   new[] { "مقدرتش أحدّث قائمة السيرفرات:\n",               "Could not update the server list:\n" } },
            { "Unknown",            new[] { "حالة السيرفر غير معروفة",                      "Server status unknown" } },
            { "Posted",             new[] { "نُشر",                                         "Posted" } },
            { "By",                 new[] { "بواسطة",                                       "by" } },
            { "GameNotFound",       new[] { "مش لاقي ملف اللعبة:\n",                        "Game file not found:\n" } },
            { "GameFailed",         new[] { "اللعبة مشتغلتش:\n",                            "The game could not start:\n" } },
            { "SettingsTitle",      new[] { "الإعدادات",                                    "Settings" } },
            { "ScreenMode",         new[] { "وضع الشاشة",                                   "Screen mode" } },
            { "Fullscreen",         new[] { "ملء الشاشة",                                   "Fullscreen" } },
            { "Windowed",           new[] { "نافذة",                                        "Windowed" } },
            { "Custom",             new[] { "مقاس مخصص",                                    "Custom size" } },
            { "Width",              new[] { "العرض",                                        "Width" } },
            { "Height",             new[] { "الطول",                                        "Height" } },
            { "MaxFps",             new[] { "أقصى FPS",                                     "Max FPS" } },
            { "CloseOnPlay",        new[] { "اقفل اللانشر لما اللعبة تشتغل",                "Close the launcher when the game starts" } },
            { "Save",               new[] { "حفظ",                                          "Save" } },
            { "Cancel",             new[] { "إلغاء",                                        "Cancel" } },
            { "BadSize",            new[] { "العرض والطول لازم يكونوا أرقام (أقل حاجة 640×480).", "Width and height must be numbers (at least 640×480)." } },
            { "BadFps",             new[] { "الـ FPS لازم يكون رقم بين 30 و 1000.",          "FPS must be a number between 30 and 1000." } },
            { "SaveFailed",         new[] { "مقدرتش أحفظ الإعدادات:\n",                     "Could not save the settings:\n" } },
            { "UnexpectedError",    new[] { "حصل خطأ غير متوقع:\n",                         "Unexpected error:\n" } },
            { "TipSettings",        new[] { "الإعدادات",                                    "Settings" } },
            { "TipMinimize",        new[] { "تصغير",                                        "Minimize" } },
            { "TipClose",           new[] { "إغلاق",                                        "Close" } },
            { "TipLanguage",        new[] { "English",                                      "العربية" } },
            { "PlayReady",          new[] { "ابدأ اللعب",                                   "PLAY NOW!" } },
            { "PlayUpdate",         new[] { "تحديث",                                        "UPDATE" } },
            { "PlayUpdateSub",      new[] { "التحديث متوقف - اضغط للمتابعة",                 "paused - click to resume" } },
            { "PlayPause",          new[] { "إيقاف مؤقت",                                   "PAUSE" } },
            { "PlayMaintenance",    new[] { "صيانة",                                        "MAINTENANCE" } },
            { "PlayMaintenanceSub", new[] { "السيرفر تحت الصيانة",                           "server under maintenance" } },
            { "Paused",             new[] { "التحديث متوقف مؤقتاً",                           "Update paused" } },
            { "Left",               new[] { "متبقي",                                        "left" } },
            { "Of",                 new[] { "من",                                           "of" } },
            { "MaintenanceAsk",     new[] { "السيرفر تحت الصيانة دلوقتي. تشغّل اللعبة برضه؟",  "The server is under maintenance. Start the game anyway?" } },
            { "DirectX",            new[] { "نسخة الجرافيك",                                 "Graphics (DirectX)" } },
            { "Language",           new[] { "اللغة",                                        "Language" } },
            { "Repair",             new[] { "إصلاح ملفات اللعبة",                            "Repair game files" } },
            { "RepairHint",         new[] { "يفحص كل الملفات من الأول وينزّل التالف",          "re-checks every file and downloads broken ones" } },
            { "RepairBusy",         new[] { "استنى التحديث الحالي يخلص",                     "wait for the current update to finish" } },
            { "GameOffline",        new[] { "سيرفر اللعبة مقفول",                            "Game server offline" } },
            { "Resolution",         new[] { "المقاس",                                       "Resolution" } },
            { "AfterStart",         new[] { "بعد التشغيل",                                   "After start" } },
            { "GameFiles",          new[] { "الملفات",                                       "Game files" } },
            { "RepairAsk",          new[] { "هيتم فحص كل ملفات اللعبة من الأول وتنزيل أي ملف ناقص أو تالف. ممكن ياخد شوية وقت. نبدأ؟", "Every game file will be checked again and missing or broken files downloaded. This can take a while. Start now?" } },
            { "Ok",                 new[] { "تمام",                                         "OK" } },
            { "StartAnyway",        new[] { "شغّل برضه",                                    "Start anyway" } },
            { "PickServerTitle",    new[] { "اختار السيرفر",                                 "Choose a server" } },
            { "ErrorTitle",         new[] { "حصلت مشكلة",                                    "Something went wrong" } },
            { "NoticeTitle",        new[] { "تنبيه",                                         "Notice" } },
            { "LangButton",         new[] { "EN",                                           "ع" } },

            // ---- system check + crash reporting (appended) ----
            { "SysCheckLabel",      new[] { "فحص النظام",                                    "System check" } },
            { "SysCheckTitle",      new[] { "فحص النظام",                                    "System check" } },
            { "SysCheckShow",       new[] { "عرض الفحص",                                     "Show check" } },
            { "PreCheck",           new[] { "افحص قبل اللعب",                                 "Check before play" } },
            { "Recheck",            new[] { "إعادة الفحص",                                   "Re-check" } },
            { "SysCheckAllOk",      new[] { "كل المتطلبات موجودة ✓",                          "All requirements are present ✓" } },
            { "SysCheckBlocked",    new[] { "في متطلب أساسي ناقص، مش هينفع تشغّل اللعبة لحد ما يتصلّح.", "A required component is missing - the game cannot start until it is fixed." } },
            { "SysCheckContinue",   new[] { "تقدر تكمل برضه، بس ممكن اللعبة متشتغلش صح.",       "You can continue, but the game may not run correctly." } },
            { "SysOs",              new[] { "نظام التشغيل",                                   "Operating system" } },
            { "SysDx",              new[] { "DirectX",                                       "DirectX" } },
            { "SysDxOk",            new[] { "متاح",                                           "available" } },
            { "SysDxMissing",       new[] { "ملف DirectX الأساسي مش موجود في النظام",          "the core DirectX DLL is missing from the system" } },
            { "SysDxxMissing",      new[] { "مكتبة D3DX مش موجودة جنب اللعبة",                 "the D3DX helper DLL is missing next to the game" } },
            { "SysDxFix",           new[] { "نزّل حزمة DirectX end-user runtimes.",            "Install the DirectX end-user runtimes." } },
            { "SysVc",              new[] { "مكتبات Visual C++",                              "Visual C++ runtime" } },
            { "SysVcBundled",       new[] { "موجودة مع اللعبة (VC++ 2008)",                   "shipped with the client (VC++ 2008)" } },
            { "SysVcInstalled",     new[] { "متسطّبة على الجهاز",                              "installed on this PC" } },
            { "SysVcMissing",       new[] { "مكتبات VC++ 2008 مش موجودة",                     "the VC++ 2008 runtime is missing" } },
            { "SysVcFix",           new[] { "نزّل Visual C++ 2008 Redistributable (x86).",     "Install the Visual C++ 2008 Redistributable (x86)." } },
            { "SysDisk",            new[] { "المساحة الفاضية",                                "Free disk space" } },
            { "SysDiskOk",          new[] { "{0} ميجا فاضية على {1}",                          "{0} MB free on {1}" } },
            { "SysDiskLow",         new[] { "المساحة قليلة: {0} ميجا بس على {1}",              "low space: only {0} MB on {1}" } },
            { "SysDiskFix",         new[] { "فضّي شوية مساحة على القرص.",                      "Free up some disk space." } },
            { "SysRequired",        new[] { "ملف مطلوب",                                      "Required file" } },
            { "SysReqOk",           new[] { "موجود",                                          "present" } },
            { "SysReqMissing",      new[] { "مش موجود",                                        "missing" } },
            { "SysReqFix",          new[] { "نزّل ملفات اللعبة من جديد أو اعمل إصلاح.",         "Re-download the game files or run Repair." } },
            { "SysAdmin",           new[] { "صلاحيات المدير",                                  "Administrator rights" } },
            { "SysAdminYes",        new[] { "اللانشر شغّال كمدير",                             "the launcher is running as administrator" } },
            { "SysAdminNo",         new[] { "شغّال عادي (مش مطلوب في الغالب)",                 "running normally (usually not required)" } },
            { "CrashTitle",         new[] { "اللعبة قفلت فجأة",                               "The game closed unexpectedly" } },
            { "CrashBody",          new[] { "يبدو إن اللعبة قفلت بشكل غير طبيعي. تحب نحفظ تقرير بالتفاصيل؟", "The game appears to have crashed. Do you want to save a report with the details?" } },
            { "CrashExit",          new[] { "كود الخروج: {0}",                                "Exit code: {0}" } },
            { "OpenReport",         new[] { "افتح التقرير",                                   "Open report" } },
            { "CrashSent",          new[] { "تم إرسال التفاصيل لفريق السيرفر تلقائيًا.",       "The details were sent to the server team automatically." } },
            { "LauncherCrashBody",  new[] { "حصل خطأ في اللانشر واتسجّل في تقرير.",            "The launcher hit an error and a report was saved." } },

            // ---- server-driven news tags / maintenance banner / event countdown / ping (appended) ----
            { "TagUpdate",          new[] { "تحديث",                                         "UPDATE" } },
            { "TagEvent",           new[] { "إيفنت",                                          "EVENT" } },
            { "TagNews",            new[] { "خبر",                                            "NEWS" } },
            { "MaintBannerTitle",   new[] { "السيرفر تحت الصيانة",                            "Server under maintenance" } },
            { "MaintBannerDefault", new[] { "السيرفر مقفول مؤقتاً للصيانة. تقدر تحدّث الملفات لكن اللعب هيرجع بعد الصيانة.", "The server is temporarily down for maintenance. You can still update your files; play will return after maintenance." } },
            { "MaintUntil",         new[] { "المتوقع يرجع:",                                  "Expected back:" } },
            { "EventIn",            new[] { "بعد",                                            "in" } },
            { "EventsHeader",       new[] { "الإيفنتات",                                      "EVENTS" } },
            { "EventsTip",          new[] { "الإيفنتات",                                      "Events" } },
            { "EventsScheduleTitle",new[] { "جدول الإيفنتات",                                 "Events Schedule" } },
            { "EventsEmpty",        new[] { "لا توجد إيفنتات حالياً",                          "No events right now" } },
            { "ColEvent",           new[] { "الحدث",                                          "Event" } },
            { "ColDay",             new[] { "اليوم",                                          "Day" } },
            { "ColTime",            new[] { "الوقت",                                          "Time" } },
            { "ColCountdown",       new[] { "المتبقي",                                        "Countdown" } },
            { "EventDaily",         new[] { "يومياً",                                         "Daily" } },
            { "PlayersWord",        new[] { "لاعب",                                           "players" } },
            { "PingMs",             new[] { "مللي ثانية",                                     "ms" } },

            // ---- background music / slideshow (appended) ----
            { "MusicLabel",         new[] { "الموسيقى",                                       "Music" } },
            { "MusicMute",          new[] { "كتم",                                            "Mute" } },
            { "Mute",               new[] { "اكتم الموسيقى",                                  "Mute music" } },
            { "Unmute",             new[] { "شغّل الموسيقى",                                   "Unmute music" } },
        };

        /// <summary>Translated string for the current language (key itself when unknown).</summary>
        public static string T(string key)
        {
            string[] v;
            if (!S.TryGetValue(key, out v)) return key;
            return IsArabic ? v[0] : v[1];
        }

        public static string T(string key, object arg)
        {
            return string.Format(T(key), arg);
        }
    }
}
