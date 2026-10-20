using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgDisCity
    {

        private ProcesType Mode;
        private DateTime FinishTime = new DateTime();
        private DateTime TeleportToMap4 = new DateTime();

        private int PlayersMap2 = 0;
        private int PlayersMap3 = 0;

        public List<uint> RewardPlayers = new List<uint>();
        
        private Role.GameMap Map1;
        private Role.GameMap Map2;
        private Role.GameMap Map3;
        private Role.GameMap Map4;

        public MsgDisCity()
        {
            Mode = ProcesType.Dead;
        }

        public bool IsInDisCity(uint Map)
        {

            return 2021 == Map || 2022 == Map || 2023 == Map || 2024 == Map;
        }
        public bool AllowJoin() { return Mode == ProcesType.Idle; }

        public void CreateMaps()
        {
            if (Map1 == null) Map1 = Database.Server.ServerMaps[2021];
            if (Map2 == null) Map2 = Database.Server.ServerMaps[2022];
            if (Map3 == null) Map3 = Database.Server.ServerMaps[2023];
            if (Map4 == null) Map4 = Database.Server.ServerMaps[2024];
        }

        public void Open()
        {
            if (Mode == ProcesType.Dead)
            {
                RewardPlayers.Clear();
                CreateMaps();
                Mode = ProcesType.Idle;

                MsgSchedules.SendInvitation("DisCity", "Experience,Special Rewards", 297, 150, 1002, 0, 60);


                FinishTime = DateTime.Now.AddMinutes(5);
                PlayersMap2 = PlayersMap3 = 0;
                TeleportToMap4 = DateTime.Now.AddMinutes(35);
            }
        }

        public void CheckUp()
        {
            if (Mode == ProcesType.Idle)
            {
                if (DateTime.Now > FinishTime)
                {
                    // sign-up 19:00-19:05, the battle lasts until 20:30 (DailyActivity.ini "19:00-20:30 Dis City")
                    FinishTime = DateTime.Now.AddMinutes(85);

                    MsgSchedules.SendSysMesage("DisCity has started , all players receive 5xp Balls! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);


                    Mode = ProcesType.Alive;

                    foreach (var user in Map1.Values)
                        user.GainExpBall(600 * 5, true, Role.Flags.ExperienceEffect.angelwing);
                    foreach (var user in Map2.Values)
                        user.GainExpBall(600 * 5, true, Role.Flags.ExperienceEffect.angelwing);
                }
            }
            else if (Mode == ProcesType.Alive)
            {
                if (DateTime.Now > TeleportToMap4)
                {
                    if (!Map4.ContainMobID(66432))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, Map4, 66432, 143, 146, 18, 18, 1);
                        }
                    }
                    TeleportToMap4 = DateTime.Now.AddMinutes(9999);
                    bool teleported = false;
                    foreach (var user in Database.Server.GamePoll.Values)
                    {
                        if (user.Player.Map == Map3.ID)
                        {
                            user.Teleport(151, 278, Map4.ID);
                            teleported = true;
                        }
                    }
                    // Server-wide notice once (it was sent once per teleported player).
                    if (teleported)
                        MsgSchedules.SendSysMesage("All Players of Dis City Stage 3 has teleported to Stage 4!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                }
                if (DateTime.Now > FinishTime)
                {
                    bool teleported = false;
                    foreach (var user in Database.Server.GamePoll.Values)
                    {
                        if (user.Player.Map == Map3.ID || user.Player.Map == Map4.ID || user.Player.Map == Map2.ID || user.Player.Map == Map1.ID)
                        {
                            user.Teleport(532, 485, 1020);
                            teleported = true;
                        }
                    }
                    // Server-wide notice once (it was sent once per teleported player).
                    if (teleported)
                        MsgSchedules.SendSysMesage("DisCity has ended. All Players of Dis City has teleported to ApeCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    Mode = ProcesType.Dead;
                }
            }
        }
        public void KillTheUltimatePluto(Client.GameClient client)
        {
            MsgSchedules.SendSysMesage("" + client.Player.Name + "~has~defeated~UltimatePluto!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
        }
        public void RewardDarkHorn(Client.GameClient client, ServerSockets.Packet stream)
        {
            client.Inventory.Remove(790001, 1, stream);
            client.Teleport(400, 400, 1002);
            client.Inventory.Add(stream, 3003235, 1);
            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 1);
            client.Inventory.Add(stream, Database.ItemType.StarDrill, 1);
            client.Inventory.Add(stream, 3005129, 1);

            MsgSchedules.SendSysMesage("" + client.Player.Name + " has claimed the reward from DisCity.[5PowerExpBalls, 2DBScrolls, 1StarDrill and 3000ChiPoints.]", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);


        }
        public void TeleportMap1(ServerSockets.Packet stream, Client.GameClient client)
        {
            ushort x = 0;
            ushort y = 0;
            Map1.GetRandCoord(ref x, ref y);
            client.Teleport(x, y, Map1.ID);

                if (!RewardPlayers.Contains(client.Player.UID))
                {
                    client.SendSysMesage("You~entered~Dis~City~and~received~1~hour~of~Heaven`s~Blessing~and~a~bottle~of~Exp~Potion(B)");
                    client.Inventory.Add(stream, 723017, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket);
                    client.Player.AddHeavenBlessing(stream, (int)(60 * 60));
                    RewardPlayers.Add(client.Player.UID);
                }
            
        }
        public void TeleportToMap2(Client.GameClient client)
        {
            if (PlayersMap2 == 60)
            {
                client.SendSysMesage("To protect the unrelated persons, I`ll send those who can`t enter HellGate back.");
            }
            else
            {
                PlayersMap2 += 1;
                MsgSchedules.SendSysMesage("No." + PlayersMap2.ToString() + " Knight " + client.Player.Name + " " + ((client.Player.MyGuild != null) ? "of (" + client.Player.MyGuild.GuildName + ")".ToString() : "") + " has fought through HellGate and entered HellHall.", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);
                client.Teleport(214, 336, Map2.ID);
                client.GainExpBall(3 * 600, true, Role.Flags.ExperienceEffect.angelwing);
                client.Player.KillersDisCity = 0;
                client.HeroRewards.AddGoal(404);
            }
        }
        public void TeleportToMap3(Client.GameClient client)
        {
            PlayersMap3 += 1;

            MsgSchedules.SendSysMesage("No." + PlayersMap2.ToString() + " Knight " + client.Player.Name + " " + ((client.Player.MyGuild != null) ? "of (" + client.Player.MyGuild.GuildName + ")".ToString() : "") + "has entered the left flank of HellCloister!", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);


            client.Teleport(300, 650, Map3.ID);
            client.GainExpBall(4 * 600, true, Role.Flags.ExperienceEffect.angelwing);
            client.Player.KillersDisCity = 0;
        }

        public int KillsMap2Records(byte _class)
        {
            if (_class >= 10 && _class <= 15 || _class >= 50 && _class <= 55 || _class >= 60 && _class <= 65)
                return 800;
            else if (_class >= 20 && _class <= 25)
                return 900;
            else if (_class >= 40 && _class <= 45 || _class >= 70 && _class <= 75)
                return 1300;
            else if (_class <= 135)
                return 600;
            else
                return 1000;
        }
    }
}
