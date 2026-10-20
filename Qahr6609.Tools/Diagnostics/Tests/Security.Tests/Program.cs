// Self-contained tests for Qahr6609.GameServer/Game/Security/Guard.cs (no test framework in the project).
// Run: tests\Security.Tests\run_tests.bat (Windows, csc) or `mcs` on Mono.
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Conquer6609.Security;

static class Program
{
    static int failed, passed;
    static void Check(bool ok, string name) { if (ok) passed++; else { failed++; Console.WriteLine("FAIL: " + name); } }

    static int Main()
    {
        long a;
        Check(!Guard.ValidAmount(0UL, 100, out a), "zero rejected");
        Check(Guard.ValidAmount(100UL, 100, out a) && a == 100, "max accepted");
        Check(!Guard.ValidAmount(101UL, 100, out a), "above max rejected");
        Check(!Guard.ValidAmount(0x8000000000000001UL, long.MaxValue, out a), "value above long.MaxValue rejected");
        Check(!Guard.ValidAmount(0xFFFFFFFF00000001UL, long.MaxValue, out a), "high-bit value with small low word rejected");
        Check(!Guard.ValidAmount(-5L, 100), "negative long rejected");
        Check(Guard.ClampCount(-1, 64) == 0 && Guard.ClampCount(1000, 64) == 64 && Guard.ClampCount(5, 64) == 5, "ClampCount");
        long r;
        Check(!Guard.SafeMultiply(long.MaxValue, 2, out r), "SafeMultiply overflow detected");
        Check(Guard.SafeMultiply(3, 4, out r) && r == 12, "SafeMultiply ok");

        // NPC dialog options
        NpcDialogState.Begin(1, 500);
        NpcDialogState.RecordOption(1, 500, 3);
        Check(NpcDialogState.IsAllowed(1, 500, 3), "offered option allowed");
        Check(!NpcDialogState.IsAllowed(1, 500, 4), "not offered option rejected");
        Check(!NpcDialogState.IsAllowed(1, 501, 3), "option for another NPC rejected");
        Check(NpcDialogState.IsAllowed(1, 501, 0) && NpcDialogState.IsAllowed(1, 501, 255), "open/close always allowed");
        NpcDialogState.Begin(1, 500);
        Check(!NpcDialogState.IsAllowed(1, 500, 3), "new dialog clears previous options");
        NpcDialogState.Clear(1);
        Check(!NpcDialogState.IsAllowed(1, 500, 3), "cleared on logout");

        // SafeCounter: unique values under contention
        var c = new SafeCounter(1000);
        var seen = new ConcurrentDictionary<uint, byte>();
        Parallel.For(0, 200000, i => seen.TryAdd(c.Next, 0));
        Check(seen.Count == 200000 && c.Current == 201000, "SafeCounter unique under contention");
        c.EnsureAtLeast(500); Check(c.Current == 201000, "EnsureAtLeast never lowers");
        c.EnsureAtLeast(300000); Check(c.Current == 300000, "EnsureAtLeast raises");

        // Throttle
        Check(Guard.Throttle("t", 10000) && !Guard.Throttle("t", 10000), "Throttle");

        Console.WriteLine("passed " + passed + ", failed " + failed);
        return failed == 0 ? 0 : 1;
    }
}
