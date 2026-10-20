using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static class Achievement
    {

        public static unsafe void GetAchievement(this ServerSockets.Packet stream, out ClientAchievement.TypeAchievement action, out uint UID, out uint Flag)
        {
            action = (ClientAchievement.TypeAchievement)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            Flag = stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet AchievementCreate(this ServerSockets.Packet stream, ClientAchievement.TypeAchievement action, uint UID, uint[] Flags)
        {
            stream.InitWriter();

            stream.Write((uint)action);
            stream.Write(UID);
            stream.Write(Flags.Length);

            for (int x = 0; x < Flags.Length; x++)
            {
                stream.Write(Flags[x]);
            }

            stream.Finalize(GamePackets.Achievement);
            return stream;
        }
        public static unsafe ServerSockets.Packet AchievementCreate(this ServerSockets.Packet stream, ClientAchievement.TypeAchievement action, uint UID, uint Flag)
        {
            stream.InitWriter();

            stream.Write((uint)action);
            stream.Write(UID);
            stream.Write(Flag);

            stream.Write((uint)0);

            stream.Finalize(GamePackets.Achievement);
            return stream;
        }
    }
    public class ClientAchievement
    {
        public const int File_Size = 13;

        public enum TypeAchievement : uint
        {
            BigHash = 0,
            CheakView = 1,
            SendQueue = 2
        }



        [PacketAttribute(GamePackets.Achievement)]
        public static unsafe void AchievementHandler(Client.GameClient client, ServerSockets.Packet stream)
        {
            TypeAchievement action;
            uint UID;
            uint Flag;

            stream.GetAchievement(out action, out UID, out Flag);


            switch (action)
            {
                case TypeAchievement.CheakView:
                    {
                        Role.IMapObj obj = null;
                        if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            Role.Player target = obj as Role.Player;
                            if (target != null)
                            {

                                client.Send(stream.AchievementCreate(TypeAchievement.CheakView, target.UID, target.Achievement.MyFlags()));
                            }

                        }
                        break;
                    }
                case TypeAchievement.SendQueue:
                    {
                        // Official: achievements are decided by the server. The client may only report
                        // an achievement whose condition the server can confirm right now; anything else is ignored.
                        if (CanClientReport(client, Flag))
                            client.Player.Achievement.CreateFlaID((int)Flag, client, stream);
                        break;
                    }
            }
        }

        // Travel achievements (detected by the client when entering a region) -> map(s) the player must be on.
        private static readonly Dictionary<uint, uint[]> TravelMaps = new Dictionary<uint, uint[]>()
        {
            {10612, new uint[]{1002}}, {10613, new uint[]{1002}}, {10614, new uint[]{1002}},
            {10617, new uint[]{1011}}, {10618, new uint[]{1011}}, {10619, new uint[]{1011}},
            {10620, new uint[]{1013}}, {10621, new uint[]{1016}}, {10622, new uint[]{1011}},
            {10625, new uint[]{1020}}, {10626, new uint[]{1020}},
            {10629, new uint[]{1015}}, {10631, new uint[]{1015}},
            {10701, new uint[]{1000}}, {10702, new uint[]{1000}}, {10703, new uint[]{1300}},
            {10708, new uint[]{1205}}, {10709, new uint[]{1202}}, {10710, new uint[]{1201}}, {10711, new uint[]{1204}},
            {10712, new uint[]{1216}}, {10713, new uint[]{1207}}, {10714, new uint[]{1208}}, {10715, new uint[]{1210}},
            {10716, new uint[]{1211}}, {10717, new uint[]{1212}}, {10718, new uint[]{1219}}, {10719, new uint[]{1217}},
            {10720, new uint[]{1215}}, {10721, new uint[]{1214}},
            {10801, new uint[]{1926}}, {10802, new uint[]{1927}}, {10803, new uint[]{1999}},
            {10804, new uint[]{2054}}, {10805, new uint[]{2055}}, {10806, new uint[]{2056}},
            {10807, new uint[]{1039}}, {10808, new uint[]{1806}},
            {10809, new uint[]{1100, 1101, 1102, 1103, 1104, 1105, 1106, 1107, 1108, 1109}},
            {10810, new uint[]{1038}}, {10812, new uint[]{1036}},
        };
        // Promotion achievements -> required class id.
        private static readonly Dictionary<uint, byte> PromotionClass = new Dictionary<uint, byte>()
        {
            {10813, 21}, {10814, 22}, {10815, 23}, {10816, 24}, {10817, 25},
            {10819, 11}, {10820, 12}, {10821, 13}, {10822, 14}, {10823, 15},
            {10824, 101}, {10825, 142}, {10826, 143}, {10827, 144}, {10828, 145},
            {10829, 101}, {10830, 132}, {10831, 133}, {10832, 134}, {10901, 135},
            {10903, 41}, {10904, 42}, {10905, 43}, {10906, 44}, {10907, 45},
            {10908, 51}, {10909, 52}, {10910, 53}, {10911, 54}, {10912, 55},
            {10913, 61}, {10914, 62}, {10915, 63}, {10916, 64}, {10917, 65},
            {11205, 71}, {11206, 72}, {11207, 73}, {11208, 74}, {11209, 75},
            {11216, 81}, {11217, 82}, {11218, 83}, {11219, 84}, {11220, 85},
            {11304, 161}, {11305, 162}, {11306, 163}, {11307, 164}, {11308, 165},
        };
        private static int ClassLine(byte cls)
        {
            if (cls >= 100 && cls <= 101) return 10;//plain Taoist (both Taoist lines start here)
            return cls / 10;
        }
        private static bool ReachedClass(byte cls, byte required)
        {
            if (cls == 0) return false;
            int need = ClassLine(required), have = ClassLine(cls);
            if (need == 10)
                return cls == 101 || have == 13 || have == 14;
            return have == need && cls >= required;
        }
        private static bool HasEquip(Client.GameClient client, Role.Flags.ConquerItem pos)
        {
            Game.MsgServer.MsgGameItem item;
            return client.Equipment != null && client.Equipment.TryGetEquip(pos, out item);
        }
        private static bool HasAssociate(Role.Player player, byte type)
        {
            System.Collections.Concurrent.ConcurrentDictionary<uint, Role.Instance.Associate.Member> list;
            return player.Associate != null && player.Associate.Associat.TryGetValue(type, out list) && list.Count > 0;
        }
        public static bool CanClientReport(Client.GameClient client, uint id)
        {
            var p = client.Player;
            if (p == null || p.Achievement == null)
                return false;
            uint[] maps;
            if (TravelMaps.TryGetValue(id, out maps))
                return maps.Contains(p.Map);
            byte cls;
            if (PromotionClass.TryGetValue(id, out cls))
                return ReachedClass(p.Class, cls) || ReachedClass(p.FirstClass, cls) || ReachedClass(p.SecoundeClass, cls);
            var rank = p.GuildRank;
            switch (id)
            {
                case 10106: return p.Money >= 3000000;
                case 10107: return HasEquip(client, Role.Flags.ConquerItem.Steed);
                case 10418: return HasEquip(client, Role.Flags.ConquerItem.Fan) || HasEquip(client, Role.Flags.ConquerItem.Tower);
                case 10419: return HasEquip(client, Role.Flags.ConquerItem.Garment);
                case 10123: return HasAssociate(p, Role.Instance.Associate.Friends);
                case 10124: return HasAssociate(p, Role.Instance.Associate.Apprentice);
                case 10125: return p.MyMentor != null || HasAssociate(p, Role.Instance.Associate.Mentor);
                case 10126: return HasAssociate(p, Role.Instance.Associate.Partener);
                case 10129: return p.Spouse != null && p.Spouse != "None" && p.Spouse != "";
                case 10218: return p.MyGuild != null;
                case 10219: return p.MyGuild != null && rank == Role.Flags.GuildMemberRank.GuildLeader;
                case 10221: return p.MyGuild != null && (rank == Role.Flags.GuildMemberRank.HDeputyLeader || rank == Role.Flags.GuildMemberRank.HonoraryManager
                                || rank == Role.Flags.GuildMemberRank.HonorarySuperv || rank == Role.Flags.GuildMemberRank.HonorarySteward);
                case 10222: return p.MyGuild != null && (rank == Role.Flags.GuildMemberRank.DeputyLeader || rank == Role.Flags.GuildMemberRank.HDeputyLeader);
                case 10223: return p.MyGuild != null && (rank == Role.Flags.GuildMemberRank.Manager || rank == Role.Flags.GuildMemberRank.HonoraryManager);
                case 10224: return p.MyGuild != null && (ushort)rank >= 840 && (ushort)rank <= 859;
                case 10225: return p.MyGuild != null && (rank == Role.Flags.GuildMemberRank.Steward || rank == Role.Flags.GuildMemberRank.HonorarySteward);
                case 10304: return p.MyClan != null;
                case 10310: return p.Level >= 130 || p.Reborn > 0;
                case 10311: return p.Level >= 140 || p.FirstRebornLevel >= 140 || p.SecoundeRebornLevel >= 140;
                case 10312: return p.BattlePower >= 100;
                case 10313: return p.BattlePower >= 200;
                case 10314: return p.BattlePower >= 300;
                case 10315:
                case 11030: return p.Reborn >= 1;
                case 10316:
                case 11031: return p.Reborn >= 2;
                case 10317: return p.VirtutePoints >= 5000;
                case 10318: return p.NobilityRank != Role.Instance.Nobility.NobilityRank.Serf;
            }
            return false;
        }


        private Extensions.BitVector32 BitVector32;

        public int Score()
        {
            return BitVector32.Count();
        }
        public ClientAchievement(uint[] Db_value, uint UID)
        {
            BitVector32 = new Extensions.BitVector32(32 * File_Size);

            for (int x = 0; x < Db_value.Length; x++)
                BitVector32.bits[x] = Db_value[x];
        }
        public unsafe void CreateFlaID(int id, Client.GameClient client, ServerSockets.Packet stream)
        {
            int row = (id / 100) % 100 - 1, col = id % 100 - 1;
            if (row < 0 || row >= File_Size || col < 0 || col >= 32)
                return;
            AddFlag(row * 32 + col, client, stream);
        }

        public unsafe void AddFlag(int flag, Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!BitVector32.Contain(flag))
            {
                BitVector32.Add(flag);

                ShowScreen(flag, client, stream);
                Send(client, stream);
            }
        }
        public unsafe void Send(Client.GameClient client, ServerSockets.Packet stream)
        {
            client.Send(stream.AchievementCreate(TypeAchievement.BigHash, client.Player.UID, client.Player.Achievement.MyFlags()));
        }
        private unsafe void ShowScreen(int flag, Client.GameClient client, ServerSockets.Packet stream)
        {
            uint FLAG_ID = (uint)(10100 + (uint)(100 * (byte)(flag / 32)) + (byte)(flag % 32) + 1);

            client.Send(stream.AchievementCreate(TypeAchievement.SendQueue, client.Player.UID, FLAG_ID));
            string Mesajje = "" + client.Player.Name + " received [Achievement " + FLAG_ID + "]! ";
            if (client.Player.MyGuild != null)
                client.Player.MyGuild.SendMessajGuild(Mesajje);
            else
            {
                 Game.MsgServer.MsgMessage mesaj = new Game.MsgServer.MsgMessage(Mesajje, client.Player.Name, MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.System);
                client.Send(mesaj.GetArray(stream));
            }
        }


        public uint[] MyFlags()
        {
            uint[] Flags = new uint[File_Size];
            for (byte x = 0; x < BitVector32.bits.Length; x++)
                Flags[x] = BitVector32.bits[x];

            return Flags;
        }
        public void Save(Database.AchievementCollection achiev)
        {
            for (int x = 0; x < File_Size; x++)
                achiev.Value[x] = this.BitVector32.bits[x];
        }
    }
}


