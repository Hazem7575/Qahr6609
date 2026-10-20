// Bridge for the website API (WebApi.cs): runs the three website ops through the SAME PlayerStudio functions as the
// AdminApi ("cps", "giveitem", "player"), so the behaviour is identical: one lock (playerOpLock), SafeSave right after a
// change, offline edits with FlushPendingSaves + timestamped backup, anti-auto-ban stepping, adminapi_log.txt logging.
// This file only ADDS an internal entry point; the AdminApi wire protocol / ops / secret are unchanged.
using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Conquer6609
{
    public static partial class AdminApi
    {
        /// <summary>
        /// cps: {uid|name, amount, mode(add|sub|set)}; offlineAdd = website setting cps_offline (offline players get the
        /// CPs written to Users\uid.ini with the AdminApi's OfflineWrite: backup, flush, not-online check).
        /// giveitem: {uid|name, id, amount, plus, bound} (online only, as AdminApi giveitem).
        /// inventory: AdminApi "player" reduced to name/uid/level/cps/inventory/equipment.
        /// </summary>
        internal static WebApiResult WebOp(string op, JObject args, bool offlineAdd)
        {
            var a = new A(args);
            try
            {
                lock (playerOpLock)
                {
                    // the shutdown save may already be running: a change now could be lost (the website gets busy_retry)
                    if (Program.OnMainternance) throw new AdminError("server is shutting down");
                    JObject r;
                    switch (op)
                    {
                        case "cps":
                            {
                                var online = FindOnline(a);
                                string mode = a.Str("mode", "add").ToLowerInvariant();
                                if (online == null)
                                {
                                    if (!offlineAdd) throw new AdminError("player is not online");
                                    r = WebOfflineCps(a, mode);
                                }
                                else
                                    r = CpsOp(a, false);
                                break;
                            }
                        case "giveitem":
                            r = GiveItem(a);
                            break;
                        case "inventory":
                            r = WebInventory(PlayerInfo(a));
                            break;
                        default:
                            throw new AdminError("unknown op");
                    }
                    bool ok = r != null && r["ok"] != null && (bool)r["ok"];
                    return new WebApiResult { Ok = ok, Code = ok ? "" : "failed", Detail = r != null ? (string)(r["message"] ?? r["error"]) : "", Data = r };
                }
            }
            catch (AdminError e)// a checked refusal: nothing was changed
            {
                Log("web:" + op, TargetOf(a) + " FAILED: " + e.Message, false);
                return new WebApiResult { Ok = false, Code = WebErrorCode(e.Message), Detail = e.Message };
            }
            catch (Exception e)// unexpected game-side error: the change MAY have happened -> the website ref stays used
            {
                Log("web:" + op, TargetOf(a) + " ERROR: " + e.GetType().Name + ": " + (e.Message ?? "").Replace("\r", " ").Replace("\n", " "), false);
                return new WebApiResult { Ok = false, Uncertain = op != "inventory", Code = op != "inventory" ? "uncertain" : "failed", Detail = "server error: " + e.Message };
            }
        }

        // offline add/sub: current value read from Users\uid.ini AFTER the pending saves were flushed, then the AdminApi
        // OfflineWrite (set) under the same lock - it re-checks that the player did not log in meanwhile.
        static JObject WebOfflineCps(A a, string mode)
        {
            uint uid = ResolveOfflineUid(a);
            if (Database.Server.GamePoll.ContainsKey(uid))
                throw new AdminError("player is online or still being saved - try again in a few seconds");
            Database.ServerDatabase.FlushPendingSaves(uid, null);
            var row = ReadOfflineIni(uid);
            if (row == null) throw new AdminError("player not found");
            ulong old = (ulong)Math.Max(0L, (long)row["cps"]);
            ulong nv = Apply(mode, old, a.ULong("amount"), (ulong)CpsCap);
            var args = new JObject { ["uid"] = uid, ["allowOffline"] = true };
            var r = OfflineWrite(new A(args), "cps", new Dictionary<string, string> { { "ConquerPoints", nv.ToString(CultureInfo.InvariantCulture) } });
            r["old"] = old;
            r["new"] = nv;
            return r;
        }

        static JObject WebInventory(JObject info)
        {
            var p = info["player"] as JObject;
            if (p == null) throw new AdminError("player not found");
            bool online = info["online"] != null && (bool)info["online"];
            var d = new JObject
            {
                ["ok"] = true,
                ["online"] = online,
                ["uid"] = p["uid"],
                ["name"] = p["name"],
                ["level"] = p["level"],
                ["cps"] = p["cps"],
            };
            if (online)
            {
                d["inventory"] = p["inventory"];
                d["inventoryMax"] = p["inventoryMax"];
                d["equipment"] = p["equipment"];
            }
            else
                d["message"] = "player is offline: the bag is only readable while the player is online";
            return d;
        }

        static string WebErrorCode(string msg)
        {
            msg = msg ?? "";
            if (msg.Contains("is online or still being saved") || msg.Contains("saved a moment ago") || msg.Contains("shutting down")) return "busy_retry";
            if (msg.Contains("not online")) return "player_offline";
            if (msg.Contains("not found")) return "player_not_found";
            if (msg.Contains("inventory space")) return "no_space";
            if (msg.Contains("item id") || msg.Contains("stackable")) return "invalid_item";
            if (msg.Contains("'")) return "invalid_request";
            return "failed";
        }
    }
}
