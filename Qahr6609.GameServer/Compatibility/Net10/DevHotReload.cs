// ============================================================================
//  [DevHotReload] .NET Hot Reload observer (same as Qahr's).
//
//  The .NET runtime calls this handler ONLY when a hot-reload delta is applied to the running process, which only
//  happens when the server was started through `dotnet watch` (Qahr6609.Tools\Server\ServerCtl\dev_hotreload.bat). A normal start of
//  bin\Debug\GameServer.exe never applies a delta, so this code never runs there.
//
//  It prints a line so the developer sees that a code edit landed live and how many types changed. It does NOT
//  touch game state or caches: applying the new method bodies is the runtime's job; this only reports it.
// ============================================================================
using System;

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(Conquer6609.Net10Dev.DevHotReload))]

namespace Conquer6609.Net10Dev
{
    internal static class DevHotReload
    {
        // called after a delta is applied; updatedTypes may be null (the runtime did not list them)
        internal static void UpdateApplication(Type[] updatedTypes)
        {
            try
            {
                int n = updatedTypes == null ? 0 : updatedTypes.Length;
                string names = "";
                if (updatedTypes != null && updatedTypes.Length > 0)
                {
                    var sb = new System.Text.StringBuilder();
                    for (int i = 0; i < updatedTypes.Length && i < 6; i++)
                    {
                        if (i > 0) sb.Append(", ");
                        sb.Append(updatedTypes[i].Name);
                    }
                    if (updatedTypes.Length > 6) sb.Append(" (+").Append(updatedTypes.Length - 6).Append(" more)");
                    names = "  -> " + sb;
                }
                string msg = "[HotReload] code update applied live" + (n > 0 ? " (" + n + " type(s))" : "") + names +
                             "  -  method-body edits are live; data edits still need their reload* command.";
                try { MyConsole.WriteLine(msg, ConsoleColor.Magenta); }
                catch { Console.WriteLine(msg); }
            }
            catch { }// a diagnostics print must never disturb the running server
        }

        // optional hook the runtime may call before UpdateApplication; no game cache is cleared automatically
        internal static void ClearCache(Type[] updatedTypes) { }
    }
}
