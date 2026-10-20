
using Conquer6609.Client;
using Conquer6609.Database;
using Conquer6609.Game.MsgMonster;
using Conquer6609.Game.MsgServer.AttackHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conquer6609.Game.MsgTournaments
{
    class BossHandler
    {
        public static Extensions.Time32 Stamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.TournamentsStamp);
        #region SpawinTime
        class SpawinTime
        {
            public static DateTime Now
            {
                get
                {
                    return DateTime.Now;
                }
            }

            public static bool SnowBanshe
            {
                get
                {
                    return (Now.Minute == 01) && Now.Second == 3;
                }
            }
            public static bool Nemesis
            {
                get
                {
                    return (Now.Minute == 13) && Now.Second == 3;
                }
            }

            public static bool ThrillingSpook
            {
                get
                {
                    return (Now.Minute == 32) && Now.Second == 3;
                }
            }
            public static bool TeratoDragon
            {
                get
                {
                    return (Now.Minute == 48) && Now.Second == 3;
                }
            }
            //
            public static bool BloodyBanshee
            {
                get
                {
                    return (Now.Minute == 8) && Now.Second == 3;
                }
            }
            public static bool ChillingSpook
            {
                get
                {
                    return (Now.Minute == 23) && Now.Second == 3;
                }
            }

            public static bool DragonWraith
            {
                get
                {
                    return (Now.Minute == 38) && Now.Second == 3;
                }
            }
            public static bool SnowBansheeSoul
            {
                get
                {
                    return (Now.Minute == 17) && Now.Second == 3;
                }
            }
            public static bool QueenOfEvil
            {
                get
                {
                    return (Now.Minute == 52) && Now.Second == 3;
                }
            }
            public static bool Delhab
            {
                get
                {
                    return (Now.Minute == 12) && Now.Second == 3;
                }
            }

            public static bool DarkWizard
            {
                get
                {
                    return (Now.Minute == 42) && Now.Second == 3;
                }
            }
            public static bool NetherTyrant
            {
                get
                {
                    return (Now.Minute == 57) && Now.Second == 3;
                }
            }
         
        }
        #endregion
        public static bool SnowBanshe = true;
        public static bool Nemesis = true;
        public static bool TeratoDragon = true;
        public static bool ThrillingSpook = true;
        public static bool DarkmoonDemon = true;
        /// <summary>
        /// ///
        /// new map
        public static bool BloodyBanshee = true;
        public static bool ChillingSpook = true;
        public static bool NetherTyrant = true;
        public static bool DragonWraith = true;
        public static bool SnowBansheeSoul = true;
        public static bool QueenOfEvil = true;
        public static bool Delhab = true;
        public static bool DarkWizard = true;
 
        /// </summary>
        /// <summary>Maps whose bosses a module spawns itself (e.g. Dragon Island 10137 by Modules\DragonIsland): the
        /// fixed schedule below and the boss rewards of Death.cs leave them alone while the module is loaded.</summary>
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, string> ModuleOwnedMaps = new System.Collections.Concurrent.ConcurrentDictionary<uint, string>();
        internal static DateTime NextLavaBeast;
        internal static int LavaBeastsCount = 0;
        #region LavaBeast
        public static List<Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords> LavaBeast = new List<Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords>() { 
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(313,250) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(262,241) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(205,231) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(241,201) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(209,143) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(280,176) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(328,166) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(328,120) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(329,067) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(297,087) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(401,185) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(441,225) { },
     
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(620,549) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(644,599) { },
      
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(326,475) { },
   
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(201,435) { },
     
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(120,314) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(096,256) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(461,683) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(467,735) { },
        new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(534,670) { },
     
        };
        #endregion
        public static void SpawnLavaBeast()
        {
            var Map = Database.Server.ServerMaps[2056];
            int Loc = Program.GetRandom.Next(0, LavaBeast.Count);
            var spawnLoc = LavaBeast[Loc];
            LavaBeast.RemoveAt(Loc);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                string msg = "LavaBeast has spawned in FrozenGrotto6! Hurry find it and kill it.";
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                Database.Server.AddMapMonster(stream, Map, 20055, (ushort)spawnLoc.X, (ushort)spawnLoc.Y, 1, 1, 1);
            }
        }
        internal static void CheckUp(Extensions.Time32 clock)
        {
            // Throttled boss HP bars / harm rankings (see MonsterRole.TakeBossHpSlot / SendScore).
            Game.MsgMonster.MonsterRole.FlushBossUpdates();
            if (clock > Stamp)
            {
                DateTime Now64 = DateTime.Now;

                #region BigMonster
                if (!ModuleOwnedMaps.ContainsKey(10137))// a module (Modules\DragonIsland) runs the island bosses itself
                {
                #region Map 10137
                #region Nemesis
                if (SpawinTime.Nemesis && Nemesis)
                {
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(20300, 1))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 20300, 580, 370, 18, 18, 1);
                            string Messaj = "The Nemesis have spawned in the DragonIslnd on (580, 370,) ! Hurry to kill them. Drop [DragonSoul, DragonBalls].";
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The NemesisTyrant have spawned in the Nemesis on (580, 370) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                user.Player.MessageBox(Messaj, new Action<Client.GameClient>(p => p.Teleport(585, 365, 10137)), null, 60);

                            }
                            Nemesis = false;

                        }
                    }
                }
                #endregion
                #region ThrillingSpook
                if (SpawinTime.ThrillingSpook && ThrillingSpook)
                {
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(20160, 1))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Server.AddMapMonster(stream, map, 20160, 349, 635, 3, 3, 1,0, true, Conquer6609.Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            string Messaj = "The ThrillingSpook have spawned in the DragonIslnd on (349, 635)  ! Hurry to kill them. Drop [DragonSoul, DragonBalls].";
                            MyConsole.WriteLine("-------------------------------------------");
                            MyConsole.WriteLine("[DragonIsland] Thrilling Spook Has Spawned.");
                            MyConsole.WriteLine("-------------------------------------------");
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The ThrillingSpook have spawned in the BloodShedSea on (568,584) ! Hurry to kill them. Drop [DragonSoul, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.SlideCrosTheServer).GetArray(stream));
                            foreach (var user in Database.Server.GamePoll.Values)
                                user.Player.MessageBox(Messaj, new Action<Client.GameClient>(p => p.Teleport(349, 635, 10137)), null, 60);
                            ThrillingSpook = false;
                        }
                    }
                }
                #endregion
                #region Snow Banshee
                if (SpawinTime.SnowBanshe && SnowBanshe)
                {
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(20070, 1))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 20070, 658, 718, 3, 3, 1, 0, true, Conquer6609.Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Snow Banshee] has appeared in Dragon Island Go and kill it now.", Game.MsgServer.MsgMessage.MsgColor.orange, Game.MsgServer.MsgMessage.ChatMode.SlideCrosTheServer).GetArray(stream));
                            MyConsole.WriteLine("----------------------------------------");
                            MyConsole.WriteLine("[DragonIsland] Snow Banshee Has Spawned.");
                            MyConsole.WriteLine("----------------------------------------");
                            foreach (var user in Database.Server.GamePoll.Values)
                                user.Player.MessageBox("[Snow Banshee] has appeared in Dragon Island Go and kill it now.", new Action<Client.GameClient>(p => p.Teleport(658, 718, 10137)), null, 60);
                            SnowBanshe = false;
                        }
                    }
                }
                #endregion
                #region TeratoDragon
                if (SpawinTime.TeratoDragon && TeratoDragon)
                {
                    var Map = Database.Server.ServerMaps[10137];

                    if (!Map.ContainMobID(20060, 1))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[TeratoDragon] has appeared in Dragon Island Go and kill it now.", Game.MsgServer.MsgMessage.MsgColor.orange, Game.MsgServer.MsgMessage.ChatMode.SlideCrosTheServer).GetArray(stream));
                            MyConsole.WriteLine("----------------------------------------");
                            MyConsole.WriteLine("[DragonIsland] TeratoDragon Has Spawned.");
                            MyConsole.WriteLine("----------------------------------------");
                            Database.Server.AddMapMonster(stream, Map, 20060, 452, 482, 1, 1, 1);
                            foreach (var user in Database.Server.GamePoll.Values)
                                user.Player.MessageBox("[TeratoDragon] has appeared in Dragon Island Go and kill it now.", new Action<Client.GameClient>(p => p.Teleport(452, 482, 10137)), null, 60);
                            TeratoDragon = false;

                        }
                    }


                }
                #endregion
                #endregion 
                #region New Map 
                #region BloodyBanshee
                if (SpawinTime.BloodyBanshee && BloodyBanshee)
                {
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(29370, 1))//BloodyBanshee
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 29370, 310, 136, 18, 18, 1, 0, true, Conquer6609.Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            string Messaj = "BloodyBanshee have spawned in the DragonIsland on (310, 136) ! Hurry to kill them. Drop [DragonSoul, DragonBalls].";
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The BloodyBanshee have spawned on (310, 136) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                user.Player.MessageBox(Messaj, new Action<Client.GameClient>(p => p.Teleport(318, 136, 10137)), null, 60);

                            }
                            BloodyBanshee = false;

                        }
                    }
                }
                #endregion
                #region ChillingSpook
                if (SpawinTime.ChillingSpook && ChillingSpook)
                {
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(29360, 1))//ChillingSpook
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 29360, 442, 172, 18, 18, 1, 0, true, Conquer6609.Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            string Messaj = "ChillingSpook have spawned in the DragonIsland on (442, 172) ! Hurry to kill them. Drop [DragonSoul, DragonBalls].";
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The ChillingSpook have spawned on (442, 172) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                user.Player.MessageBox(Messaj, new Action<Client.GameClient>(p => p.Teleport(450, 172, 10137)), null, 60);

                            }
                            ChillingSpook = false;

                        }
                    }
                }
                #endregion
                #region NetherTyrant
                if (SpawinTime.NetherTyrant && NetherTyrant)
                {
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(29300, 1))//NetherTyrant
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 29300, 592, 214, 18, 18, 1,0, true, Conquer6609.Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            string Messaj = "NetherTyrant have spawned in the DragonIsland on (592, 214) ! Hurry to kill them. Drop [DragonSoul, DragonBalls].";
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The NetherTyrant have spawned on (592, 214) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                user.Player.MessageBox(Messaj, new Action<Client.GameClient>(p => p.Teleport(592, 222, 10137)), null, 60);

                            }
                            NetherTyrant = false;

                        }
                    }
                }
                #endregion
                #region DragonWraith
                if (SpawinTime.DragonWraith && DragonWraith)
                {
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(29363, 1))//NetherTyrant
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 29363, 238, 220, 18, 18, 1,0, true, Conquer6609.Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            string Messaj = "DragonWraith have spawned in the DragonIsland on (238, 220) ! Hurry to kill them. Drop [DragonSoul, DragonBalls].";
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The DragonWraith have spawned on (238, 220) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                user.Player.MessageBox(Messaj, new Action<Client.GameClient>(p => p.Teleport(246, 220, 10137)), null, 60);

                            }
                            DragonWraith = false;

                        }
                    }
                }
                #endregion
                #region SnowBansheeSoul
                if (SpawinTime.SnowBansheeSoul && SnowBansheeSoul)
                {
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(20170, 1))//SnowBansheeSoul
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 20170, 334, 220, 18, 18, 1, 0, true, Conquer6609.Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            string Messaj = "SnowBansheeSoul have spawned in the DragonIsland on (334, 220) ! Hurry to kill them. Drop [DragonSoul, DragonBalls].";
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The SnowBansheeSoul have spawned on (334, 220) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                user.Player.MessageBox(Messaj, new Action<Client.GameClient>(p => p.Teleport(342, 220, 10137)), null, 60);

                            }
                            SnowBansheeSoul = false;

                        }
                    }
                }
                #endregion
                #region QueenOfEvil
                if (SpawinTime.QueenOfEvil && QueenOfEvil)
                {
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(20061, 1))//QueenOfEvil
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 20061, 418, 256, 18, 18, 1, 0, true, Conquer6609.Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            string Messaj = "QueenOfEvil have spawned in the DragonIsland on (418, 256) ! Hurry to kill them. Drop [DragonSoul, DragonBalls].";
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The QueenOfEvil have spawned on (418, 256) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                user.Player.MessageBox(Messaj, new Action<Client.GameClient>(p => p.Teleport(426, 256, 10137)), null, 60);

                            }
                            QueenOfEvil = false;

                        }
                    }
                }
                #endregion
                #region Delhab
                if (SpawinTime.Delhab && Delhab)
                {
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(20063, 1))//Delhab
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 20063, 502, 256, 18, 18, 1, 0, true, Conquer6609.Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            string Messaj = "Delhab have spawned in the DragonIsland on (502, 256) ! Hurry to kill them. Drop [DragonSoul, DragonBalls].";
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The Delhab have spawned on (502, 256) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                user.Player.MessageBox(Messaj, new Action<Client.GameClient>(p => p.Teleport(510, 256, 10137)), null, 60);

                            }
                            Delhab = false;

                        }
                    }
                }
                #endregion
                #region DarkWizard
                if (SpawinTime.DarkWizard && DarkWizard)
                {
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(20064, 1))//DarkWizard
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 20064, 160, 304, 18, 18, 1, 0, true, Conquer6609.Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            string Messaj = "DarkWizard have spawned in the DragonIsland on (160, 304) ! Hurry to kill them. Drop [DragonSoul, DragonBalls].";
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The DarkWizard have spawned on (160, 304) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                user.Player.MessageBox(Messaj, new Action<Client.GameClient>(p => p.Teleport(168, 304, 10137)), null, 60);

                            }
                            DarkWizard = false;

                        }
                    }
                }
                #endregion
                                   
                #endregion 
                } // ModuleOwnedMaps
                #endregion
                #region Ganodema
                if (Now64.Minute == 48 && Now64.Second == 11)//re-spawn ganoderma
                {
                    var map = Database.Server.ServerMaps[1011];
                    if (!map.ContainMobID(3130, 0))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 3130, 667, 753, 18, 18, 1);
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The Ganodema have spawned in the forest/canyon! Hurry to kill them. Drop [Special Items 50% change.].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));


                        }
                    }
                }
                #endregion
                #region Titan
                else if (Now64.Minute == 17 && Now64.Second == 12)//re-spawn titan
                {
                    var map = Database.Server.ServerMaps[1020];
                    if (!map.ContainMobID(3134, 0))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 3134, 419, 618, 18, 18, 1);
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The Titan have spawned in the forest/canyon! Hurry to kill them. Drop [Special Items 50% change.].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        }

                    }
                }
                #endregion
                #region SnakeKing
                else if (Now64.Minute == 38 && Now64.Second < 1)//re-spawn SnakeKing
                {
                    var map = Database.Server.ServerMaps[1063];
                    if (!map.ContainMobID(3102, 0))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 3102, 84, 64, 0, 0, 1);
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The SnakeKing have spawned in the BirdIslan(SnakeIsland NPC)! Hurry to kill them. Drop [Special Items 50% change.].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        }

                    }
                }
                #endregion
                #region LavaBeasts
                if (DateTime.Now > NextLavaBeast && LavaBeastsCount > 0)
                {
                    LavaBeastsCount--;
                    if (LavaBeastsCount > 0)
                        NextLavaBeast = DateTime.Now.AddMinutes(5);
                    SpawnLavaBeast();
                }
                #endregion
                
            }

        }
 
       
    }
}
