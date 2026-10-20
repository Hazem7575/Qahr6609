using Conquer6609.Game.MsgServer.Reward;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateRewardInfoPanel(this ServerSockets.Packet stream, List<Mode> Modes)
        {
            stream.InitWriter();//28
            stream.Write((ushort)0);
            stream.Write((ushort)Modes.Count);
            for (int x = 0; x < Modes.Count; x++)
            {
                stream.Write((uint)Modes[x]);
                stream.Write((ushort)1);
                stream.ZeroFill(7);
            }
            stream.Finalize(GamePackets.MsgOperatingActInfo);
            return stream;
        }
        public static unsafe ServerSockets.Packet CreateRewardInfoPacket(this ServerSockets.Packet stream, byte Count)
        {
            stream.InitWriter();
            stream.Write((ushort)1);
            stream.Write(Count);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemReward(this ServerSockets.Packet stream, Database.ClientRewards.Rewards Reward)
        {
            stream.Write((uint)Reward.Mode);
            stream.Write((ushort)Reward.Claim);
            stream.Write((uint)Reward.Points);
            stream.Write((ushort)0);
            return stream;
        }
        public static unsafe void GetRewardAction(this ServerSockets.Packet stream, out byte Action, out Mode Mode)
        {
            Action = stream.ReadUInt8();
            Mode = (Reward.Mode)stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet RewardInfoFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgOperatingActInfo);
            return stream;
        }
    }
    public static class RewardSystem
    {
        public static void SendReward(Client.GameClient user, ServerSockets.Packet stream, bool login = false)
        {
            stream.CreateRewardInfoPacket((byte)RewardSystem.ids.Count);
            foreach (var id in ids)
            {
                if (user.Rewards.ContainsKey(id))
                {
                    stream.AddItemReward(user.Rewards[id]);
                }
                else
                {
                    stream.AddItemReward(new Database.ClientRewards.Rewards() { Points = 1, Mode = id, Claim = 0 });
                }
            }
            user.Send(stream.RewardInfoFinalize());
        }
        public static List<Mode> ids;
        public static Dictionary<Game.MsgServer.Reward.Mode, Dictionary<uint, Reward.Reward>> Rewards = new Dictionary<Reward.Mode, Dictionary<uint, Reward.Reward>>();
        public static void Load()
        {
            WindowsAPI.IniFile IniFile = new WindowsAPI.IniFile("\\OperateActivity.ini");
            int count = IniFile.ReadInt32("OperateActivity", "Num", 0);
            ids = new List<Mode>(count);
            for (int x = 0; x < count; x++)
            {
                ids.Add((Mode)IniFile.ReadUInt32(x.ToString(), "id", 0));
            }
            IniFile = null;
            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "operating_prize.txt");
            foreach (var bas_line in baseText)
            {
                Database.DBActions.ReadLine line = new Database.DBActions.ReadLine(bas_line, '@');
                Reward.Reward Item = new Reward.Reward();
                Item.UID = line.Read((uint)0);
                Item.ID = (Mode)line.Read((uint)0);
                Item.number = line.Read((uint)0);
                Item.NeedPoints = line.Read((uint)0);
                Item.prize = line.Read((uint)0);
                Item.prizevalue = line.Read((uint)0);
                if (Rewards.ContainsKey(Item.ID))
                {
                    Rewards[Item.ID].Add(Item.number, Item);
                }
                else
                {
                    Rewards.Add(Item.ID, new Dictionary<uint, Reward.Reward>(10));
                    Rewards[Item.ID].Add(Item.number, Item);
                }
            }
            baseText = null;
        }
    }
}
