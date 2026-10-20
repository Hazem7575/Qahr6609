// PacketAudit module: finds what the official 6609 client asks for that this server ignores.
// Every incoming packet type with no handler (Program.Game_Receive) and every 10010 DataMap action with no
// [DataAttribute] handler (MsgDataPacket.Process) used to be dropped without a trace; the client then simply
// gets no answer (a button "does nothing"). Each new kind is written once with a hex dump to
// <DbLocation>\Modules\PacketAudit\logs\yyyyMMdd.log, and the totals again every 10 minutes.
// GM: @packets (top unhandled kinds in game). Cost: one dictionary lookup per unhandled packet only.
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;

namespace Conquer6609.Modules.PacketAudit
{
    public sealed class PacketAuditModule : ModuleBase
    {
        public override string Name { get { return "PacketAudit"; } }

        static PacketAuditModule instance;
        static readonly ConcurrentDictionary<string, int> counts = new ConcurrentDictionary<string, int>();
        static Extensions.Time32 nextSummary = Extensions.Time32.Now.AddMinutes(10);
        static int changed;

        public override void OnLoad()
        {
            instance = this;
            string hex = ConfigString("OutScanHex", "00A4CFEE").Replace(" ", "");
            try
            {
                var bytes = new byte[hex.Length / 2];
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                scanPattern = bytes.Length >= 3 ? bytes : null;
            }
            catch { scanPattern = null; }
            Log("watching unhandled client packets" + (scanPattern != null ? ", hunting " + hex + " in outgoing packets" : ""));
        }

        public override void OnTick(Extensions.Time32 clock)
        {
            if (clock < nextSummary)
                return;
            nextSummary = clock.AddMinutes(10);
            if (System.Threading.Interlocked.Exchange(ref changed, 0) != 0)
                Log("totals: " + Summary(40));
            Log("outgoing: " + OutgoingSummary());
        }

        public override void OnShutdown()
        {
            if (counts.Count > 0)
                Log("totals at shutdown: " + Summary(200));
            Log("outgoing at shutdown: " + OutgoingSummary());
        }

        static string Summary(int max)
        {
            return string.Join(", ", counts.OrderByDescending(x => x.Value).Take(max).Select(x => x.Key + " x" + x.Value));
        }

        // ---- outgoing side ----
        // Inventory of every packet type the server sends (count + sizes seen), written with the totals, to
        // compare with what the 6609 client expects. OutScanHex (shell.ini [Module.PacketAudit]) = a byte pattern
        // to hunt in outgoing packets (default 00A4CFEE = the itemtype -288381952 the client reports as
        // "Read Error in ItemType.dat"): the first hit per packet type/offset is logged with a hex dump.
        sealed class OutStat { public long Count; public int Min = int.MaxValue, Max; }
        static readonly ConcurrentDictionary<ushort, OutStat> outgoing = new ConcurrentDictionary<ushort, OutStat>();
        static readonly ConcurrentDictionary<string, byte> outHits = new ConcurrentDictionary<string, byte>();
        static byte[] scanPattern;

        public static unsafe void Outgoing(Client.GameClient client, ServerSockets.Packet msg)
        {
            var self = instance;
            if (self == null || msg == null || msg.Memory == null || msg.Size < 4)
                return;
            int size = msg.Size;
            ushort type = *(ushort*)(msg.Memory + 2);
            var st = outgoing.GetOrAdd(type, t => new OutStat());
            System.Threading.Interlocked.Increment(ref st.Count);
            if (size < st.Min) st.Min = size;
            if (size > st.Max) st.Max = size;
            var pat = scanPattern;
            if (pat == null || pat.Length == 0)
                return;
            byte* p = msg.Memory;
            for (int i = 0; i + pat.Length <= size; i++)
            {
                int k = 0;
                while (k < pat.Length && p[i + k] == pat[k]) k++;
                if (k != pat.Length)
                    continue;
                if (outHits.Count < 200 && outHits.TryAdd(type + "@" + i, 0))
                {
                    string who = client != null && client.Player != null ? client.Player.Name : "?";
                    self.Log("OUT-SCAN hit in packet " + type + " at offset " + i + " (size " + size + ") to " + who + ": " + Hex(msg, 160));
                }
                break;
            }
        }

        static string OutgoingSummary()
        {
            return string.Join(", ", outgoing.OrderBy(x => x.Key).Select(x => x.Key + " x" + x.Value.Count + " [" + x.Value.Min + (x.Value.Max != x.Value.Min ? "-" + x.Value.Max : "") + "]"));
        }

        // ---- trace mode: every packet a chosen player sends, for a short time (GM @packets trace [seconds]) ----
        static uint traceUid;
        static DateTime traceUntil;

        /// <summary>Called for every incoming packet (Program.Game_Receive); cheap unless a trace is running.</summary>
        public static unsafe void Incoming(Client.GameClient client, ushort packetId, ServerSockets.Packet stream)
        {
            if (traceUid == 0 || client == null || client.Player == null || client.Player.UID != traceUid)
                return;
            if (DateTime.Now > traceUntil)
            {
                traceUid = 0;
                if (instance != null) instance.Log("trace finished");
                return;
            }
            if (instance != null)
                instance.Log("TRACE in " + packetId + " (" + stream.Size + " bytes): " + Hex(stream, 96));
        }

        /// <summary>Incoming packet type with no handler.</summary>
        public static unsafe void Unhandled(Client.GameClient client, ushort packetId, ServerSockets.Packet stream)
        {
            // small packets (button clicks, protobuf actions): every different body is its own entry, so each
            // button of a window shows up once (key = id + body without the 8-byte seal)
            string key = packetId.ToString();
            if (stream != null && stream.Memory != null && stream.Size > 4 && stream.Size <= 48)
            {
                var sb = new StringBuilder(key).Append('/');
                for (int i = 4; i < stream.Size - 8; i++)
                    sb.Append(stream.Memory[i].ToString("X2"));
                key = sb.ToString();
            }
            Record(client, key, stream);
        }

        /// <summary>10010 action with no handler.</summary>
        public static unsafe void UnhandledAction(Client.GameClient client, ushort action, ServerSockets.Packet stream)
        {
            Record(client, "10010:" + action, stream);
        }

        /// <summary>Sub-action of another packet (e.g. 1009 ItemUsage) that falls into the handler's default case.</summary>
        public static unsafe void UnhandledSub(Client.GameClient client, string packet, uint subType, ServerSockets.Packet stream)
        {
            Record(client, packet + ":" + subType, stream);
        }

        static unsafe void Record(Client.GameClient client, string key, ServerSockets.Packet stream)
        {
            var self = instance;
            if (self == null)
                return;
            int n = counts.AddOrUpdate(key, 1, (k, v) => v + 1);
            System.Threading.Interlocked.Exchange(ref changed, 1);
            if (n != 1)
                return;
            string who = client != null && client.Player != null ? client.Player.Name + " map " + client.Player.Map : "?";
            self.Log("NEW unhandled " + key + " from " + who + ", " + stream.Size + " bytes: " + Hex(stream, 96));
        }

        static unsafe string Hex(ServerSockets.Packet stream, int max)
        {
            if (stream == null || stream.Memory == null)
                return "";
            int len = Math.Min(stream.Size, max);
            var sb = new StringBuilder(len * 3);
            for (int i = 0; i < len; i++)
                sb.Append(stream.Memory[i].ToString("X2")).Append(i % 4 == 3 ? "  " : " ");
            if (stream.Size > max)
                sb.Append("...");
            return sb.ToString().TrimEnd();
        }

        [GmCommand("packets", Role = CommandRole.GM, Help = "client packets the server ignores (PacketAudit): @packets | @packets trace [seconds]", Usage = "@packets")]
        static void PacketsCommand(Client.GameClient client, string[] args)
        {
            int secs;
            if (args.Length > 1 && args[1].Equals("trace", StringComparison.OrdinalIgnoreCase))
            {
                secs = args.Length > 2 && int.TryParse(args[2], out secs) ? Math.Max(5, Math.Min(secs, 300)) : 60;
                traceUid = client.Player.UID;
                traceUntil = DateTime.Now.AddSeconds(secs);
                if (instance != null) instance.Log("trace started for " + client.Player.Name + " (" + secs + " s)");
                client.SendSysMesage("Packet trace on for " + secs + " s: every packet you send is logged (Modules/PacketAudit/logs).");
                return;
            }
            if (counts.Count == 0)
            {
                client.SendSysMesage("No unhandled client packets since the server started.");
                return;
            }
            foreach (var kv in counts.OrderByDescending(x => x.Value).Take(12))
                client.SendSysMesage("unhandled " + kv.Key + " x" + kv.Value);
        }
    }
}
