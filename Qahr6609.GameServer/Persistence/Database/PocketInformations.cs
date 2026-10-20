using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    /// <summary>Loads the Texas Hold'em tables (ServerDataBase/PokerTables.txt) into the poker rooms.</summary>
    public class PocketInformations
    {
        /// <summary>Last client ShowHandTable row whose TexasHoldEm.ini layout (TexasTableIndex row+1) has 10 chairs.</summary>
        public const uint MaxTexasTableId = 248;

        /// <summary>Where the poker NPCs / PokerTeleporter put players (auto placed tables are built around it).</summary>
        private static void Anchor(uint map, out ushort x, out ushort y)
        {
            switch (map)
            {
                case 1858: x = 157; y = 119; break;// Poker Room (silver), client map/map/poker
                case 1860: x = 58; y = 66; break;  // VIP Hall (CPs), client map/map/poker02
                default: x = 100; y = 100; break;
            }
        }

        /// <summary>
        /// PokerTables.txt, one table per line (lines starting with # are comments):
        /// UID,TableId,MapID,X,Y,Mesh,Limit,Money,BigBlind,MaxSeats,MaxStake
        ///  UID      table role id (unique, not used by any NPC/monster)
        ///  TableId  client ini/ShowHandTable.dat row (Data0, 0..279). The client shows "Table: TableId+1", takes the chair
        ///           layout from ini/TexasHoldEm.ini [TexasTableIndex TableId+1] and its own copy of Limit (Data1),
        ///           Money (Data2) and BigBlind (Data3) from that row, so those columns must equal the row.
        ///           Rows 0..248 are the 10-chair (9 players + dealer) Texas Hold'em layouts; 249..279 are 6-chair ShowHand.
        ///  X,Y      0,0 = place the table automatically on free floor near the room entrance
        ///  Mesh     9007 = client NpcType900 "Table", 9047 = NpcType904 "VIPTable" (last digit = facing)
        ///  Limit    0 = Fixed Limit, 1 = No Limit
        ///  Money    0 = Silver, 1 = CPs
        ///  BigBlind big blind (= min bet; small blind is half; 10 x big blind is needed to sit down)
        ///  MaxSeats 2..9
        ///  MaxStake most silver/CPs a player brings into one hand, 0 = everything he carries (official)
        /// </summary>
        public static void LoadPokerTables()
        {
            if (!Game.MsgServer.Poker.PokerManager.Enabled)
            {
                MyConsole.WriteLine("[Poker] disabled in shell.ini ([Poker] Enabled=0).");
                return;
            }
            string path = Program.ServerConfig.DbLocation + "PokerTables.txt";
            if (!File.Exists(path))
            {
                MyConsole.WriteLine("[Poker] " + path + " not found: no Texas Hold'em tables.");
                return;
            }
            string[] lines = File.ReadAllLines(path);
            int loaded = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string text = lines[i].Trim();
                if (text.Length == 0 || text.StartsWith("#") || text.StartsWith("//") || char.IsLetter(text[0]))
                    continue;
                try
                {
                    string[] f = text.Split(',');
                    if (f.Length < 9)
                    {
                        MyConsole.WriteLine("[Poker] PokerTables.txt line " + (i + 1) + " skipped: not enough columns.");
                        continue;
                    }
                    var table = new Game.MsgServer.Poker.PokerTable();
                    table.UID = uint.Parse(f[0].Trim());
                    table.TableId = uint.Parse(f[1].Trim());
                    uint number = table.TableId + 1;
                    table.MapID = uint.Parse(f[2].Trim());
                    ushort x = ushort.Parse(f[3].Trim());
                    ushort y = ushort.Parse(f[4].Trim());
                    table.Mesh = uint.Parse(f[5].Trim());
                    table.NoLimit = int.Parse(f[6].Trim()) == 1;
                    table.MoneyType = (byte)(int.Parse(f[7].Trim()) == 1 ? 1 : 0);
                    table.BigBlind = long.Parse(f[8].Trim());
                    table.MaxSeats = f.Length > 9 ? Math.Max(2, Math.Min(Game.MsgServer.Poker.PokerTable.MaxSeatsLimit, int.Parse(f[9].Trim()))) : Game.MsgServer.Poker.PokerTable.MaxSeatsLimit;
                    table.MaxStake = f.Length > 10 ? Math.Max(0, long.Parse(f[10].Trim())) : 0;
                    if (table.UID == 0 || table.BigBlind < 2 || table.BigBlind > int.MaxValue)
                    {
                        MyConsole.WriteLine("[Poker] PokerTables.txt line " + (i + 1) + " skipped: bad UID/BigBlind.");
                        continue;
                    }
                    if (table.TableId > MaxTexasTableId)
                    {
                        MyConsole.WriteLine("[Poker] PokerTables.txt line " + (i + 1) + " skipped: TableId " + table.TableId + " is not a 10-chair Texas Hold'em row of the client (0.." + MaxTexasTableId + ").");
                        continue;
                    }
                    if (!Server.ServerMaps.ContainsKey(table.MapID))
                    {
                        MyConsole.WriteLine("[Poker] PokerTables.txt line " + (i + 1) + " skipped: map " + table.MapID + " not loaded.");
                        continue;
                    }
                    ushort fx, fy;
                    if (x == 0 && y == 0)
                    {
                        ushort ax, ay;
                        Anchor(table.MapID, out ax, out ay);
                        if (!Game.MsgServer.Poker.PokerManager.FindSpot(table.MapID, ax, ay, out fx, out fy, 6))
                        {
                            MyConsole.WriteLine("[Poker] table " + number + ": no free floor near " + ax + "," + ay + " on map " + table.MapID + ", skipped.");
                            continue;
                        }
                    }
                    else if (!Game.MsgServer.Poker.PokerManager.FindSpot(table.MapID, x, y, out fx, out fy))
                    {
                        fx = x; fy = y;
                        MyConsole.WriteLine("[Poker] table " + number + ": " + x + "," + y + " on map " + table.MapID + " is not free floor (kept).");
                    }
                    else if (fx != x || fy != y)
                        MyConsole.WriteLine("[Poker] table " + number + ": moved from " + x + "," + y + " to free floor " + fx + "," + fy + ".");
                    table.X = fx;
                    table.Y = fy;
                    if (!Game.MsgServer.Poker.PokerManager.Register(table))
                    {
                        MyConsole.WriteLine("[Poker] PokerTables.txt line " + (i + 1) + " skipped: duplicate UID/TableId or bad position.");
                        continue;
                    }
                    loaded++;
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[Poker] PokerTables.txt line " + (i + 1) + " skipped: " + e.Message);
                }
            }
            MyConsole.WriteLine("Loading: " + loaded + " Texas Hold'em tables");
            foreach (var t in Game.MsgServer.Poker.PokerManager.TablesByUID.Values.OrderBy(t => t.TableId))
                ConsoleQueue.EnqueueFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "[Poker] table " + (t.TableId + 1) + " map " + t.MapID + " at " + t.X + "," + t.Y + " " + (t.NoLimit ? "No Limit" : "Fixed Limit")
                    + " " + t.CurrencyName + " blinds " + (t.BigBlind / 2) + "/" + t.BigBlind + Environment.NewLine);// details: Exceptions log file
            Game.MsgServer.Poker.PokerManager.StartTicker();
            SpawnRoomHostess();
        }

        /// <summary>
        /// The Poker Room (1858) has no NPC in Npcs.txt: put the Casino Hostess (6298, client NpcType680 "CasinoHostess")
        /// next to the entrance so players can go back to Twin City or to the VIP Hall. Npcs.txt wins if it defines her.
        /// </summary>
        private static void SpawnRoomHostess()
        {
            const uint MapId = 1858;
            Role.GameMap map;
            uint uid = (uint)Game.MsgNpc.NpcID.PokerCasinoHostess;
            if (!Server.ServerMaps.TryGetValue(MapId, out map) || map == null || map.View.MapContain(Role.MapObjectType.Npc, uid))
                return;
            ushort ax, ay;
            Anchor(MapId, out ax, out ay);
            int[][] offsets = { new[] { -3, -3 }, new[] { 3, -3 }, new[] { -3, 3 }, new[] { 3, 3 }, new[] { -4, 0 }, new[] { 0, -4 }, new[] { 4, 0 }, new[] { 0, 4 }, new[] { -2, -2 }, new[] { 2, -2 } };
            foreach (var o in offsets)
            {
                ushort x = (ushort)(ax + o[0]), y = (ushort)(ay + o[1]);
                if (!map.ValidLocation(x, y))
                    continue;
                bool nearTable = false;
                foreach (var t in Game.MsgServer.Poker.PokerManager.TablesByUID.Values)
                    if (t.MapID == MapId && Math.Max(Math.Abs(t.X - x), Math.Abs(t.Y - y)) < 4)
                        nearTable = true;
                if (nearTable)
                    continue;
                var npc = Game.MsgNpc.Npc.Create();
                npc.UID = uid;
                npc.NpcType = Role.Flags.NpcType.Talker;
                npc.Mesh = 6807;
                npc.Map = MapId;
                npc.X = x;
                npc.Y = y;
                map.AddNpc(npc);
                MyConsole.WriteLine("[Poker] Casino Hostess placed in the Poker Room at " + x + "," + y + ".");
                return;
            }
        }
    }
}
