using Conquer6609.Client;
using Conquer6609.Database;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgTournaments;
using Conquer6609.Role.Instance;
using Conquer6609.Role;
using Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using Conquer6609.Bot;
using Conquer6609.ServerSockets;

namespace Conquer6609.BotManager
{
    public unsafe class ConquerAIHandle
    {//البوت الى بيلعب قصداك
        public static void Action()
        {
            foreach (var Bot in ConquerAI.Pool.Values)
            {
                if (Bot != null)
                {
                    try
                    {
                        Bot.MyBot.Buscar();
                        Bot.MyBot.Brincar();
                        Bot.MyBot.Atacar();
                        Bot.MyBot.ConteoExtra();
                        Bot.MyBot.Remover();
                        Bot.MyBot.TerminarHit();
                    }
                    catch (Exception e)
                    {
                        // one broken bot must not stop the others (or the loop) every tick
                        if (global::Conquer6609.Security.Guard.Throttle("bot-ai-error", 60000))
                            Console.WriteLine("[ConquerAI] " + e.Message);
                    }
                }
            }
            UnlimitedArenaRooms.Sweep();
        }
        // Unique bot name: "<owner>[AI]", then "<owner>[AI]2", "<owner>[AI]3"... (null when none is free).
        private static string UniqueBotName(string baseName)
        {
            for (int i = 1; i <= 1000; i++)
            {
                string name = i == 1 ? baseName : baseName + i;
                bool used = false;
                foreach (var ExitsClient in ConquerAI.Pool.Values)
                {
                    if (ExitsClient.Player.Name == name)
                    {
                        used = true;
                        break;
                    }
                }
                if (!used)
                    return name;
            }
            return null;
        }
        public static Counter UIDCounter = new Counter(60000000);
        public static Equipment Equipment;

        public static GameClient CreateBots(Packet stream, GameClient User, BotLevel Level, BotSkills attacker, BotType Type)
        {
            GameClient Bot = Clases();
            Bot.MyBot.Bot = Bot;
            Bot.MyBot.Client = User;
            Bot.MyBot.SetLevel(Level);
            Equipment = new Equipment(Bot.MyBot);
            switch (Type)
            {
                case BotType.MyAi:
                    {
                        string name = UniqueBotName(User.Player.Name + "[AI]");
                        if (name == null)
                            return null;
                        Bot.Player.Name = name;
                        Bot.Player.Body = (ushort)ConquerAI.Body[Program.GetRandom.Next(0, ConquerAI.Body.Length)];
                        Bot.Player.Class = 15;
                        Bot.Map = User.Map;
                        User.Player.ProtectAttack(7 * 1000);
                        Bot.Player.ProtectAttack(7 * 1000);
                        Bot.Player.DynamicID = User.Player.DynamicID;
                        Bot.Player.X = (ushort)User.Player.X;
                        Bot.MyBot.Segundos = 0;
                        Bot.MyBot.Conteo2 = Extensions.Time32.Now.AddSeconds(1);
                        Bot.MyBot.Conteo = Extensions.Time32.Now.AddSeconds(5);
                        Bot.Player.Y = (ushort)User.Player.Y;
                        Bot.Player.Map = (ushort)User.Player.Map;
                        Bot.Player.Level = (ushort)User.Player.Level;
                        Bot.Player.Face = (ushort)User.Player.Face;
                        Bot.MyBot.Attack = attacker;
                        Bot.Player.Hair = (ushort)User.Player.Hair;
                        Bot.Player.PkMode = Flags.PKMode.PK;
                        Bot = RellenarInformacion(Bot, stream);
                        User.Send(Bot.Player.GetArray(stream, false));
                        Bot.MySpells.Add(stream, 1045, 4, 0, 0);
                        Bot.MySpells.Add(stream, 1046, 4, 0, 0);
                        Bot.MySpells.Add(stream, 1115, 4, 0, 0);
                        Bot.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonTail, 4, 0, 0);
                        Bot.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ViperFang, 4, 0, 0);

                        Bot.MySpells.Add(stream, 1095, 4, 0, 0);
                        Bot.MySpells.Add(stream, 1090, 4, 0, 0);
                        Bot.MySpells.Add(stream, 10381, 4, 0, 0);
                        Bot.MySpells.Add(stream, 12080, 4, 0, 0);
                        Bot.FullLoading = true;
                        Bot.Player.CompleteLogin = true;
                        Bot.MyBot.Bot = Bot;
                        Bot.Map.AddAI(Bot);
                        break;
                    }
                case BotType.AI:
                    {
                        string name = UniqueBotName(User.Player.Name + "[AI]");
                        if (name == null)
                            return null;
                        User.Player.ProtectAttack(10 * 1000);
                        Bot.Player.ProtectAttack(10 * 1000);
                        Bot.Player.Name = name;
                        Bot.Player.Body = (ushort)ConquerAI.Body[Program.GetRandom.Next(0, ConquerAI.Body.Length)];
                        Bot.Player.Class = 15;
                        Bot.Map = User.Map;
                        Bot.Player.DynamicID = User.Player.DynamicID;
                        Bot.Player.X = (ushort)User.Player.X;
                        Bot.MyBot.Segundos = 0;
                        Bot.MyBot.Conteo2 = Extensions.Time32.Now.AddSeconds(1);
                        Bot.MyBot.Conteo = Extensions.Time32.Now.AddSeconds(5);
                        Bot.Player.Y = (ushort)User.Player.Y;
                        Bot.Player.Map = (ushort)User.Player.Map;
                        Bot.Player.Level = (ushort)User.Player.Level;
                        Bot.Player.Face = (ushort)User.Player.Face;
                        Bot.MyBot.Attack = attacker;
                        Bot.Player.Hair = (ushort)User.Player.Hair;
                        Bot.Player.PkMode = Role.Flags.PKMode.PK;
                        Bot = RellenarInformacion(Bot, stream);
                        User.Send(Bot.Player.GetArray(stream, false));
                        Bot.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader, 4, 0, 0);
                        Bot.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword, 4, 0, 0);
                        Bot.MySpells.Add(stream, 1115, 4, 0, 0);
                        Bot.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonTail, 4, 0, 0);
                        Bot.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ViperFang, 4, 0, 0);
                        Bot.MySpells.Add(stream, 1095, 4, 0, 0);
                        Bot.MySpells.Add(stream, 1090, 4, 0, 0);
                        Bot.FullLoading = true;
                        Bot.Player.CompleteLogin = true;
                        Bot.MyBot.Bot = Bot;
                        Bot.Map.AddAI(Bot);
                        Bot.MyBot.Type = Type;
                        break;
                    }
            }

            return null;
        }
        public static GameClient Clases()
        {
            GameClient pclient = new GameClient(null);
            pclient.Fake = true;
            pclient.Player = new Player(pclient);
            pclient.Inventory = new Inventory(pclient);
            pclient.Equipment = new Equip(pclient);
            pclient.Warehouse = new Warehouse(pclient);
            pclient.MySpells = new Spell(pclient);
            pclient.MyProfs = new Proficiency(pclient);
            pclient.Status = new MsgStatus();
            pclient.MyVendor = new Vendor(pclient);
            pclient.Player.SubClass = new SubClass();
            pclient.Player.Nobility = new Nobility(pclient);
            pclient.Player.View = new RoleView(pclient);
            pclient.Player.Associate = new Associate.MyAsociats(pclient.Player.UID);
            pclient.Player.MyClan = new Clan();
            pclient.Player.QuestGUI = new Quests(pclient.Player);
            pclient.Team = new Team(pclient);
            pclient.MyTrade = new Trade(pclient);
            pclient.Confiscator = new Confiscator();
            pclient.ExtraStatus = new ConcurrentDictionary<RoleStatus.StatuTyp, RoleStatus>();
            pclient.ArenaStatistic = new MsgArena.User();
            pclient.DemonExterminator = new DemonExterminator();
            pclient.Player.MyChi = new Role.Instance.Chi(pclient.Player.UID);
            return pclient;
        }
        public static GameClient RellenarInformacion(GameClient pclient, Packet stream)
        {
            pclient.Player.HitPoints = pclient.MyBot.HP + 50000;
            pclient.Status.MaxHitpoints = (uint)pclient.Player.HitPoints;
            pclient.Player.VipLevel = 4;
            pclient.Player.NobilityRank = Nobility.NobilityRank.Earl;
            pclient.Player.Angle = Flags.ConquerAngle.SouthEast;
            pclient.Player.ServerID = (ushort)GroupServerList.MyServerInfo.ID;
            DataCore.AtributeStatus.GetStatus(pclient.Player);
            pclient.Player.CountryID = (ushort)Program.GetRandom.Next(1, 50);
            pclient.Player.UID = UIDCounter.Next;
            pclient.Player.Reborn = (byte)Program.GetRandom.Next(1, 2);
            pclient.Player.FirstClass = 135;
            pclient.Player.SecoundeClass = 25;
            pclient.Player.ProtectAttack(10 * 1000);
            pclient.Player.Flowers = new Flowers(pclient.Player.UID, pclient.Player.Name);
            pclient.MyHouse = new House(pclient.Player.UID);
            pclient.Player.Action = Flags.ConquerAction.Sit;
            Equipment.GetRandomEquipment((byte)pclient.Player.Class).Send();
            return pclient;
        }
    }
}
