using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Database
{
    
    public class Roulettes
    {
        public class NumberInformation
        {
            //Number 37 is 00
            //Number 0 is 0
            public List<byte> Red = new List<byte>() { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
            public List<byte> Black = new List<byte>() { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 };
            public List<byte> Line1 = new List<byte>() { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34 };
            public List<byte> Line2 = new List<byte>() { 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35 };
            public List<byte> Line3 = new List<byte>() { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 };

            public bool IsOdd(byte Number)
            {
                if (Number == 0 || Number >= 37)
                    return false;
                return Number % 2 != 0;
            }
            public bool IsEven(byte Number)
            {
                if (Number == 0 || Number >= 37)
                    return false;
                return Number % 2 == 0;
            }
            public bool IsSmall(byte Number)
            {
                return Number >= 1 && Number <= 18;
            }
            public bool IsBig(byte Number)
            {
                return Number >= 19 && Number <= 36;
            }
            public bool IsFront(byte Number)
            {
                return Number >= 1 && Number <= 12;
            }
            public bool IsMiddle(byte Number)
            {
                return Number >= 13 && Number <= 24;
            }
            public bool IsBack(byte Number)
            {
                return Number >= 25 && Number <= 36;
            }
        }

        public unsafe class RouletteTable : NumberInformation
        {
            public Random Rand;
            public const uint MapID = 1858, MaxPlayerOnTable = 5, MaxTimerStamp = 30;

            public Extensions.Time32 Stamp;

            public Extensions.Time32 TimerStamp = new Extensions.Time32();
            public int GetTimerStamp
            {
                get
                {
                    int value = TimerStamp.AllSeconds - Extensions.Time32.Now.AllSeconds;
                    if (value < 0)
                        return 0;
                    else
                        return value;
                }
            }
            public Extensions.Time32 StampRound = new Extensions.Time32();
            public byte LuckyNumber = 1;

            public class Member
            {
                public Client.GameClient Owner;
                public uint MyCost
                {
                    get
                    {
                        long Cost = 0;
                        Cost += MyLuckNumber.Values.Sum(p => p.BetPrice);
                        Cost += MyLuckExtra.Values.Sum(p => p.BetPrice);
                        return (uint)Cost;
                    }

                }
                public Game.MsgServer.MsgRouletteOpenGui.Color Color;
                public uint Betting = 0;
                public uint Winning = 0;

                public uint smalwin = 0;
                public uint Noumber = 0;
                public ConcurrentDictionary<byte, Game.MsgServer.MsgRouletteCheck.Item> MyLuckNumber = new ConcurrentDictionary<byte, Game.MsgServer.MsgRouletteCheck.Item>();
                public ConcurrentDictionary<byte, Game.MsgServer.MsgRouletteCheck.Item> MyLuckExtra = new ConcurrentDictionary<byte, Game.MsgServer.MsgRouletteCheck.Item>();

                public void ShareBetting(RouletteTable table)
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        table.SendPacketTable(stream.RouletteShareBettingCreate(this, Color));

                    }
                }
            }
            public ConcurrentDictionary<uint, Member> RegistredPlayers;
            public ConcurrentDictionary<uint, Client.GameClient> ClientsWatch;
            public Game.MsgServer.MsgRouletteTable SpawnPacket;

         

            public RouletteTable()
            {
                Rand = new System.Random();
                TimerStamp = Extensions.Time32.Now.AddSeconds((int)MaxTimerStamp);
                ClientsWatch = new ConcurrentDictionary<uint, Client.GameClient>();
                RegistredPlayers = new ConcurrentDictionary<uint, Member>();
                SpawnPacket = new MsgRouletteTable();
            }
            public void AddWatch(ServerSockets.Packet stream, Client.GameClient client)
            {
                if (RegistredPlayers.Count == 0)
                {
                    client.SendSysMesage("No player plays here.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                   
                    return; 
                }
                if (client.PlayRouletteUID != 0)
                {
                    client.SendSysMesage("You are play another table. Please quit before you spectate this one.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                    return;
                }
                if (client.WatchRoulette != 0)
                {
                    client.SendSysMesage("You are spectating another table. Please quit before you spectate this one.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                    return;
                }
                if (!ClientsWatch.ContainsKey(client.Player.UID))
                {
                    client.WatchRoulette = SpawnPacket.UID;
                    ClientsWatch.TryAdd(client.Player.UID, client);


                    SendPacketTable(stream.RouletteScreenCreate(client.Player.UID));


                    client.Send(stream.RouletteOpenGuiCreate(SpawnPacket.MoneyType, Game.MsgServer.MsgRouletteOpenGui.Color.Watch
                        , (byte)GetTimerStamp, RegistredPlayers.Values.Where(p => p.Owner.Player.UID != client.Player.UID).ToArray()));

                }
            }
            public void RemoveWatch(uint UID)
            {
                Client.GameClient client;
                if (ClientsWatch.TryRemove(UID, out client))
                {
                    client.WatchRoulette = 0;

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        client.Send(stream.RouletteSignUpCreate(MsgRouletteSignUp.ActionJoin.Quit, client.Player.UID));
                    }
                }
            }
            public unsafe void AddPlayer(ServerSockets.Packet stream, Client.GameClient client)
            {
                if (RegistredPlayers.Count >= MaxPlayerOnTable)
                {
                    client.SendSysMesage("Sorry, this game table is full.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                    return;
                }
                if (client.PlayRouletteUID != 0)
                {
                    client.SendSysMesage("You are play another table. Please quit before you playing this one.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                    return;
                }
                if (!RegistredPlayers.ContainsKey(client.Player.UID))
                {
                    client.PlayRouletteUID = SpawnPacket.UID;

                    Member player = new Member();
                    player.Owner = client;
                    GeneratePlayerColor(out player.Color);
                    RegistredPlayers.TryAdd(player.Owner.Player.UID, player);

                    ApplyTime(stream, player);

                    if (RegistredPlayers.Count > 1)
                    {
                        ShareTableBetting(stream, client);
                        foreach (var user in RegistredPlayers.Values)
                        {
                            if (user.Owner.Player.UID != client.Player.UID)
                            {
                                user.Owner.Send(stream.RoulettedAddNewPlayerCreate(client.Player.UID, client.Player.Mesh, player.Color, client.Player.Name));
                            }
                        }
                        foreach (var user in ClientsWatch.Values)
                            user.Send(stream.RoulettedAddNewPlayerCreate(client.Player.UID, client.Player.Mesh, player.Color, client.Player.Name));
                    }
                }
            }
            public void ShareTableBetting(ServerSockets.Packet stream, Client.GameClient client)
            {
                foreach (var use in RegistredPlayers.Values)
                {
                    if (use.MyLuckExtra.Count != 0 || use.MyLuckNumber.Count != 0)
                        client.Send(stream.RouletteShareBettingCreate(use, use.Color));
                }
            }
            public void RemovePlayer(Client.GameClient client)
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    SendPacketTable(stream.RouletteSignUpCreate(MsgRouletteSignUp.ActionJoin.Quit, client.Player.UID));
                }


                Member player;
                if (RegistredPlayers.TryRemove(client.Player.UID, out player))
                {
                    client.PlayRouletteUID = 0;

                    if (RegistredPlayers.Count == 0)
                    {
                        foreach (var user in ClientsWatch.Values)
                            RemoveWatch(user.Player.UID);
                    }
                }
            }
            public unsafe void SendPacketTable(ServerSockets.Packet packet)
            {
                foreach (var player in RegistredPlayers.Values)
                    player.Owner.Send(packet);
                foreach (var user in ClientsWatch.Values)
                    user.Send(packet);
            }
            public bool ContainColor(Game.MsgServer.MsgRouletteOpenGui.Color color)
            {
                foreach (var player in RegistredPlayers.Values)
                {
                    if (player.Color == color)
                        return true;
                }
                return false;
            }
            public void GeneratePlayerColor(out Game.MsgServer.MsgRouletteOpenGui.Color color)
            {
                color = Game.MsgServer.MsgRouletteOpenGui.Color.None;
                for (int x = 0; x < 5; x++)
                {
                    if (!ContainColor((Game.MsgServer.MsgRouletteOpenGui.Color)x))
                    {
                        color = (Game.MsgServer.MsgRouletteOpenGui.Color)x;
                        break;
                    }
                }
            }
            public void ApplyTime(ServerSockets.Packet stream, Member player)
            {
                if (RegistredPlayers.Count == 1)
                    player.Owner.Send(stream.RouletteOpenGuiCreate(SpawnPacket.MoneyType, player.Color, (byte)GetTimerStamp, new Member[0]));
                else
                    player.Owner.Send(stream.RouletteOpenGuiCreate(SpawnPacket.MoneyType, player.Color, (byte)GetTimerStamp, RegistredPlayers.Values.Where(p => p.Owner.Player.UID != player.Owner.Player.UID).ToArray()));
            }
            public void ApplayNumberWinner(Client.GameClient client)
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Send(stream.RouletteNoWinnerCreate((byte)LuckyNumber));
                }
            }
            private bool Reset = false;
            /// <summary>Map the table stands on (Roulettes.txt column 4).</summary>
            public ushort TableMap = RoomMapID;

            /// <summary>Bets are accepted only during the countdown, not while the wheel spins/pays out.</summary>
            public bool BettingOpen
            {
                get { return !Reset && Extensions.Time32.Now <= TimerStamp; }
            }

            public void work(Extensions.Time32 clock)
            {
                if (clock > Stamp)
                {
                    Extensions.Time32 TimerNow = Extensions.Time32.Now;
                    if (RegistredPlayers.Count > 0)
                    {

                        if (TimerNow > TimerStamp)
                        {
                            if (TimerNow > StampRound && Reset == true)
                            {
                                ResetRoulette();
                            }
                            else if (!Reset)
                            {
                                //create Numbers lucky;
                                GenerateLuckyNumber();

                                Reset = true;
                                StampRound = TimerNow.AddSeconds(3);
                                foreach (var player in RegistredPlayers.Values)
                                {
                                    ApplayNumberWinner(player.Owner);
                                    GetRewrad(player,true);
                                }
                                foreach (var user in ClientsWatch.Values)
                                {
                                    ApplayNumberWinner(user);
                                }
                            }
                        }
                    }
                    else
                    {
                        ResetRoulette();
                    }

                    Stamp.Value = clock.Value;
                }
            }
            public void ResetRoulette()
            {
                foreach (var player in RegistredPlayers.Values)
                {
                    player.MyLuckExtra.Clear();
                    player.MyLuckNumber.Clear();
                }
                Reset = false;
                TimerStamp = Extensions.Time32.Now.AddSeconds((int)MaxTimerStamp);
            }
            public void CheckUpMember(Member player)
            {
                switch (SpawnPacket.MoneyType)
                {
                    case Game.MsgServer.MsgRouletteTable.TableType.ConquerPoints:
                        {
                            uint Cost = player.MyCost;
                            if (player.Owner.Player.ConquerPoints >= Cost)
                            {
                                player.Owner.Player.ConquerPoints -= Cost;
                     
                            }
                            else
                            {
                                player.Owner.CreateBoxDialog("You do not have " + Cost.ToString() + " ConquerPoints with you.");
                                RegistredPlayers.TryRemove(player.Owner.Player.UID, out player);
                                player.Owner.PlayRouletteUID = 0;
                            }
                            break;
                        }
                    case Game.MsgServer.MsgRouletteTable.TableType.Money:
                        {
                            uint Cost = player.MyCost;
                            if (player.Owner.Player.Money >= Cost)
                            {
                                player.Owner.Player.Money -= Cost;
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    player.Owner.Player.SendUpdate(stream, player.Owner.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                                }
                            }
                            else
                            {
                                player.Owner.CreateBoxDialog("You do not have " + Cost.ToString() + " silvers with you.");
                                RegistredPlayers.TryRemove(player.Owner.Player.UID, out player);
                                player.Owner.PlayRouletteUID = 0;
                            }
                            break;
                        }
                }
            }
            public bool Rate(int value)
            {
                return value > Rand.Next(0, 100);
            }



           

            /// <summary>
            /// Official wheel: 38 equally likely pockets (0, 1-36 and 00 = 37), independent of the bets on the table.
            /// (The old code picked the pocket that paid the players the least most of the time.)
            /// </summary>
            public void GenerateLuckyNumber()
            {
                var Array = RegistredPlayers.Values.ToArray();
                foreach (var user in Array)
                    user.smalwin = 0;
                lock (Rand)
                    LuckyNumber = (byte)Rand.Next(0, 38);
            }
           
            public bool CountainNo(byte noumber)
            {
                var Array = RegistredPlayers.Values.ToArray();
                bool contain = false;
                foreach (var player in Array)
                {
                    if (player.MyLuckNumber.ContainsKey(noumber))
                    {
                        contain = true;
                    }
                    if (CheckLines(player))
                        contain = true;
                }
                return contain;
            }
            public bool CheckLines(Member amember)
            {
                foreach (var item in amember.MyLuckExtra.Values)
                {
                    switch (item.Number)
                    {
                        case 152:
                            {
                                if (Black.Contains(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 151:
                            {
                                if (Red.Contains(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 112:
                            {
                                if (IsOdd(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 111:
                            {
                                if (IsEven(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 141:
                            {
                                if (IsSmall(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 142:
                            {
                                if (IsBig(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 121:
                            {
                                if (IsFront(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 122:
                            {
                                if (IsMiddle(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 123:
                            {
                                if (IsBack(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 131:
                            {
                                if (Line1.Contains(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 132:
                            {
                                if (Line2.Contains(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 133:
                            {
                                if (Line3.Contains(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                    }
                }
                return false;
            }
            public void GetRewrad(Member Member, bool Update =false)
            {
                // Winnings are summed in 64-bit so large bets cannot wrap around.
                long winning = 0, betting = 0;
           
                foreach (var item in Member.MyLuckNumber.Values)
                {
                    if (item.Number == LuckyNumber)
                    {
                        winning += (long)item.BetPrice * 36;
                        betting += item.BetPrice;
                    }
                }
                foreach (var item in Member.MyLuckExtra.Values)
                {
                    switch (item.Number)
                    {
                        case 152:
                            {
                                if (Black.Contains(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 2;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                        case 151:
                            {
                                if (Red.Contains(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 2;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                        case 112:
                            {
                                if (IsOdd(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 2;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                        case 111:
                            {
                                if (IsEven(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 2;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                        case 141:
                            {
                                if (IsSmall(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 2;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                        case 142:
                            {
                                if (IsBig(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 2;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                        case 121:
                            {
                                if (IsFront(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 3;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                        case 122:
                            {
                                if (IsMiddle(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 3;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                        case 123:
                            {
                                if (IsBack(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 3;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                        case 131:
                            {
                                if (Line1.Contains(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 3;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                        case 132:
                            {
                                if (Line2.Contains(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 3;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                        case 133:
                            {
                                if (Line3.Contains(LuckyNumber))
                                {
                                    winning += (long)item.BetPrice * 3;
                                    betting += item.BetPrice;
                                }
                                break;
                            }
                    }
                }
                Member.Winning = (uint)Math.Min(winning, (long)uint.MaxValue);
                Member.Betting = (uint)Math.Min(betting, (long)uint.MaxValue);
               // foreach (var item in Member.MyLuckNumber.Values)
                {
                    if (Member.Winning != 0)
                    {
                        if (Member.smalwin > Member.Winning || Member.smalwin == 0)
                        {
                            Member.smalwin = Member.Winning;
                            //Console.WriteLine(Member.Winning + " " + LuckyNumber);
                            Member.Noumber = LuckyNumber;
                        }
                    }
                }
                if (winning > 0 && Update)
                {
                    string logs = "[Roulette-" + SpawnPacket.MoneyType.ToString() + "] "+Member.Owner.Player.Name+" ";
                    logs += winning;

                    switch (SpawnPacket.MoneyType)
                    {
                        case Game.MsgServer.MsgRouletteTable.TableType.Money:
                            {
                        
                                // Credit is clamped to the per-operation money limit.
                                long credit = Math.Min(winning, Conquer6609.Security.Guard.MaxMoneyAmount);
                                if (credit < winning)
                                    Conquer6609.Security.Guard.Audit("roulette-win", Member.Owner.Player.UID, Member.Owner.Player.Name, "winnings " + winning + " clamped to " + credit);
                                Member.Owner.Player.Money += credit;
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    Member.Owner.Player.SendUpdate(stream,Member.Owner.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                                }
                                break;
                            }
                        case Game.MsgServer.MsgRouletteTable.TableType.ConquerPoints:
                            {
                               
                                // Credit is clamped to the per-operation CP limit and the account CP limit.
                                long credit = Math.Min(winning, Conquer6609.Security.Guard.MaxCpsAmount);
                                long room = (long)int.MaxValue - Member.Owner.Player.ConquerPoints;
                                if (credit > room)
                                    credit = Math.Max(0, room);
                                if (credit < winning)
                                    Conquer6609.Security.Guard.Audit("roulette-win", Member.Owner.Player.UID, Member.Owner.Player.Name, "winnings " + winning + " clamped to " + credit);
                                if (credit > 0)
                                    Member.Owner.Player.ConquerPoints += (uint)credit;
         
                                break;
                            }
                    }
                    Database.ServerDatabase.LoginQueue.Enqueue(logs);
                }
            }
        }
        public static Dictionary<uint, RouletteTable> RoulettesPoll = new Dictionary<uint, RouletteTable>();

        // Called on logout: a disconnected client must not stay in any table (as player or spectator).
        // Pending bets of a player who leaves mid-round are dropped, exactly like a normal quit.
        public static void OnDisconnect(Client.GameClient client)
        {
            if (client == null || client.Player == null)
                return;
            uint uid = client.Player.UID;
            foreach (var table in RoulettesPoll.Values)
            {
                RouletteTable.Member member;
                if (table.RegistredPlayers.TryGetValue(uid, out member) && member.Owner == client)
                    table.RemovePlayer(client);
                Client.GameClient watcher;
                if (table.ClientsWatch.TryGetValue(uid, out watcher) && watcher == client)
                {
                    if (table.ClientsWatch.TryRemove(uid, out watcher))
                        client.WatchRoulette = 0;
                }
            }
        }

        /// <summary>Map of the Roulette Room (client map/map/poker05); tables are only spawned on this map (RoleView).</summary>
        public const ushort RoomMapID = 3852;
        private static System.Threading.Timer TableTimer;
        private static int TableTimerBusy = 0;

        /// <summary>
        /// Roulettes.txt (ServerDataBase): first line is a header, then one table per line:
        /// UID,TableNumber,MoneyType(1 = Silver, 2 = CPs),MapID,X,Y,Mesh
        /// A missing/empty file disables the roulette (logged) instead of stopping the server.
        /// </summary>
        internal static void Load()
        {
            string path = Program.ServerConfig.DbLocation + "Roulettes.txt";
            if (!File.Exists(path))
            {
                MyConsole.WriteLine("[Roulette] " + path + " not found. The Roulette Room has no tables (roulette disabled).");
                return;
            }
            string[] baseText = File.ReadAllLines(path);
            //x = 1..... first line is info
            for (int x = 1; x < baseText.Length; x++)
            {
                string text = baseText[x].Trim();
                if (text.Length == 0 || text.StartsWith("//") || text.StartsWith("#"))
                    continue;
                try
                {
                    Database.DBActions.ReadLine line = new DBActions.ReadLine(text, ',');
                    RouletteTable Roulette = new RouletteTable();
                    Roulette.SpawnPacket.UID = line.Read((uint)0);
                    Roulette.SpawnPacket.TableNumber = line.Read((uint)0);
                    Roulette.SpawnPacket.MoneyType = (Game.MsgServer.MsgRouletteTable.TableType)line.Read((uint)0);
                    Roulette.TableMap = line.Read((ushort)0);
                    Roulette.SpawnPacket.X = line.Read((ushort)0);
                    Roulette.SpawnPacket.Y = line.Read((ushort)0);
                    Roulette.SpawnPacket.Mesh = line.Read((ushort)0);
                    if (Roulette.SpawnPacket.UID == 0
                        || (Roulette.SpawnPacket.MoneyType != Game.MsgServer.MsgRouletteTable.TableType.Money
                        && Roulette.SpawnPacket.MoneyType != Game.MsgServer.MsgRouletteTable.TableType.ConquerPoints))
                    {
                        MyConsole.WriteLine("[Roulette] Roulettes.txt line " + (x + 1) + " skipped: bad UID or MoneyType.");
                        continue;
                    }
                    if (Roulette.TableMap != RoomMapID)
                        MyConsole.WriteLine("[Roulette] table " + Roulette.SpawnPacket.UID + " is on map " + Roulette.TableMap + "; only tables on map " + RoomMapID + " are shown to players.");
                    if (!RoulettesPoll.ContainsKey(Roulette.SpawnPacket.UID))
                        RoulettesPoll.Add(Roulette.SpawnPacket.UID, Roulette);
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[Roulette] Roulettes.txt line " + (x + 1) + " skipped: " + e.Message);
                }
            }
            MyConsole.WriteLine("Loading: " + RoulettesPoll.Count + " Roulette tables");
            if (RoulettesPoll.Count > 0 && TableTimer == null)
                TableTimer = new System.Threading.Timer(OnTableTimer, null, 1000, 1000);
        }

        // Drives every table once per second: countdown -> spin -> payout -> next round.
        private static void OnTableTimer(object state)
        {
            if (System.Threading.Interlocked.Exchange(ref TableTimerBusy, 1) == 1)
                return;
            try
            {
                var clock = Extensions.Time32.Now;
                foreach (var table in RoulettesPoll.Values)
                {
                    try
                    {
                        table.work(clock);
                    }
                    catch (Exception e)
                    {
                        MyConsole.WriteException(e);
                    }
                }
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref TableTimerBusy, 0);
            }
        }
    }
}
