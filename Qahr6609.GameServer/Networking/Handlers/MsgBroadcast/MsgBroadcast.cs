using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static unsafe void GetBroadcast(this ServerSockets.Packet stream, out  MsgBroadcast.BroadTyp Action, out uint dwParam, out string[] Strings)
        {
            Action = (MsgBroadcast.BroadTyp)stream.ReadUInt32();
            dwParam = stream.ReadUInt32();

            Strings = stream.ReadStringList();

        }
        public static unsafe ServerSockets.Packet BroadcastCreate(this ServerSockets.Packet stream, MsgBroadcast.BroadTyp Action, uint dwParam, string[] Strings)
        {
            stream.InitWriter();

            stream.Write((uint)Action);
            stream.Write(dwParam);

            if (Strings != null)
                stream.Write(Strings);


            stream.Finalize(GamePackets.MsgBroadcast);

            return stream;
        }
    }

    public unsafe struct MsgBroadcast
    {
        public enum BroadTyp : byte
        {
            ReleaseSoonMessages = 1,
            MyMessages = 2,
            BroadcastMessage = 3,
            Urgen15CPs = 4,
            Urgen5CPs = 5
        }


        [PacketAttribute(GamePackets.MsgBroadcast)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {

                BroadTyp Action;
                uint dwParam;
                string[] Strings;

                stream.GetBroadcast(out Action, out dwParam, out Strings);

                lock (MsgTournaments.MsgBroadcast.SyncRoot)
                {
                    switch (Action)
                    {
                        case BroadTyp.Urgen5CPs:
                            {
                                for (int c = 0; c < MsgTournaments.MsgBroadcast.Broadcasts.Count; c++)
                                {
                                    var broadcast = MsgTournaments.MsgBroadcast.Broadcasts[c];
                                    if (broadcast.ID == dwParam)
                                    {
                                        if (c != 0)
                                        {
                                            if (user.InTrade)
                                                break;
                                            if (user.Player.ConquerPoints > 5)
                                            {
                                                broadcast.SpentCPs += 5;
                                                user.Player.ConquerPoints -= 5;
                          

                                                if (MsgTournaments.MsgBroadcast.Broadcasts[c - 1].SpentCPs <= broadcast.SpentCPs)
                                                {

                                                    MsgTournaments.MsgBroadcast.Broadcasts[c] = MsgTournaments.MsgBroadcast.Broadcasts[c - 1];
                                                    MsgTournaments.MsgBroadcast.Broadcasts[c - 1] = broadcast;
                                                }
                                                else
                                                {
                                                    MsgTournaments.MsgBroadcast.Broadcasts[c] = broadcast;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        case BroadTyp.Urgen15CPs:
                            {

                                for (int c = 0; c < MsgTournaments.MsgBroadcast.Broadcasts.Count; c++)
                                {
                                    var broadcast = MsgTournaments.MsgBroadcast.Broadcasts[c];
                                    if (broadcast.ID == dwParam)
                                    {
                                        if (c != 0)
                                        {
                                            if (user.InTrade)
                                                break;
                                            if (user.Player.ConquerPoints > 15)
                                            {
                                                broadcast.SpentCPs += 15;
                                                user.Player.ConquerPoints -= 15;
                                     

                                                for (int b = c - 1; b > 0; b--)
                                                    MsgTournaments.MsgBroadcast.Broadcasts[b] = MsgTournaments.MsgBroadcast.Broadcasts[b - 1];

                                                MsgTournaments.MsgBroadcast.Broadcasts[0] = broadcast;
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case BroadTyp.ReleaseSoonMessages:
                            {

                                const int max = 10;
                                int offset = (int)(dwParam * max);
                                int count = Math.Min(max, Math.Max(0, MsgTournaments.MsgBroadcast.Broadcasts.Count - offset));

                                ushort total = 0;

                                for (int x = 0; x < count; x++)
                                {
                                    if (MsgTournaments.MsgBroadcast.Broadcasts.Count > offset + x)
                                    {
                                        if (x % 5 == 0 || x == 0)
                                        {
                                            uint add = (uint)count;
                                            if (count > 5)
                                            {
                                                if (x > 4)
                                                    add = (uint)(count - x);
                                                else
                                                    add = 5;
                                            }

                                            if (x != 0)
                                                user.Send(stream.FinalizeBroadcastlist());
                                            stream.BroadcastlistCreate(dwParam, total, (ushort)add);

                                            total++;

                                        }
                                        var element = MsgTournaments.MsgBroadcast.Broadcasts[offset + x];
                                        stream.AddItemBroadcastlist(element, (uint)(offset + x));
                                    }
                                }
                                user.Send(stream.FinalizeBroadcastlist());

                                break;
                            }
                        case BroadTyp.MyMessages:
                            {
                                var MyMessages = MsgTournaments.MsgBroadcast.Broadcasts.Where(p => p.EntityID == user.Player.UID).ToArray();

                                const int max = 10;
                                int offset = (int)(dwParam * max);
                                int count = Math.Min(max, Math.Max(0, MyMessages.Length - offset));


                                ushort total = 0;

                                for (int x = 0; x < count; x++)
                                {
                                    if (MyMessages.Length > offset + x)
                                    {
                                        if (x % 5 == 0 || x == 0)
                                        {
                                            uint add = (uint)count;
                                            if (count > 5)
                                            {
                                                if (x > 4)
                                                    add = (uint)(count - x);
                                                else
                                                    add = 5;
                                            }
                                            if (x != 0)
                                                user.Send(stream.FinalizeBroadcastlist());
                                            stream.BroadcastlistCreate(dwParam, total, (ushort)add);

                                            total++;

                                        }
                                        var element = MyMessages[offset + x];
                                        stream.AddItemBroadcastlist(element, (uint)(offset + x));
                                    }
                                }
                                user.Send(stream.FinalizeBroadcastlist());

                                break;
                            }
                        case BroadTyp.BroadcastMessage:
                            {
                                if (MsgTournaments.MsgBroadcast.Broadcasts.Count == MsgTournaments.MsgBroadcast.MaxBroadcasts)
                                {
                                    user.SendSysMesage("You cannot send any broadcasts for now. The limit has been reached. Wait until a broadcast is chopped down.");
                               
                                    break;
                                }

                                if (Strings == null)
                                    break;
                                if (Strings.Length == 0)
                                    return;
                                if (Strings[0] == null)
                                    break;

                                if (user.InTrade)
                                    break;

                                if (user.Player.ConquerPoints >= 5)
                                {

                                    if (user.Player.IsStillBanned)
                                    {
                                        if (user.Player.PermenantBannedChat)
                                        {
                                            user.SendSysMesage("Sorry, you still banned from chatting Permenatly.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                                        }
                                        else
                                        {
                                            user.SendSysMesage("Sorry, you still banned from chatting till " + user.Player.BannedChatStamp.ToString(), MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                                        }
                                        return;
                                    }
                                    user.Player.ConquerPoints -= 5;
                           

                                    MsgTournaments.MsgBroadcast.BroadcastStr broadcast = new MsgTournaments.MsgBroadcast.BroadcastStr();
                                    broadcast.EntityID = user.Player.UID;

                                    broadcast.EntityName = user.Player.Name;
                                    if (user.Player.InUnion)
                                        broadcast.UnionRank = (uint)Role.Instance.Union.Member.GetRank(user.Player.UnionMemeber.Rank);

                                    broadcast.ID = MsgTournaments.MsgBroadcast.BroadcastCounter.Next;
                                    if (Strings[0].Length > 80)
                                        Strings[0] = Strings[0].Remove(80);
                                
                                
                                    broadcast.Message = Strings[0];
                                    string[] lines = broadcast.Message.Split(new string[]{"["},  StringSplitOptions.RemoveEmptyEntries);

                                    for (int x = 0; x < lines.Length; x++)
                                    {
                                        string str = lines[x];
                                        if (str.Contains("Item "))
                                        {
                                            string[] line = str.Split(' ');//"[Item ", StringSplitOptions.None);
                                            if (line != null && line.Length > 2)
                                            {
                                                uint UID = 0;
                                                if (uint.TryParse(line[2], out UID))
                                                {
                                                    MsgGameItem msg_item;
                                                    if (user.TryGetItem(UID, out msg_item))
                                                    {
                                                        Program.GlobalItems.Add(msg_item);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (MsgTournaments.MsgBroadcast.Broadcasts.Count == 0)
                                    {
                                        if (MsgTournaments.MsgBroadcast.CurrentBroadcast.EntityID == 1)
                                        {
                                            MsgTournaments.MsgBroadcast.CurrentBroadcast = broadcast;
                                            MsgTournaments.MsgBroadcast.LastBroadcast = DateTime.Now;
                                            if (user.Player.InUnion)
                                                 Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(Strings[0], "ALLUSERS", user.Player.Name, MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream,(uint)Role.Instance.Union.Member.GetRank(user.Player.UnionMemeber.Rank)));
                                            else
                                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(Strings[0], "ALLUSERS", user.Player.Name, MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));

                                       

                                            user.Send(stream.BroadcastCreate(BroadTyp.BroadcastMessage, dwParam, Strings));

                                            break;
                                        }
                                    }

                                    MsgTournaments.MsgBroadcast.Broadcasts.Add(broadcast);
                                    dwParam = (uint)MsgTournaments.MsgBroadcast.Broadcasts.Count;

                                    user.Send(stream.BroadcastCreate(BroadTyp.BroadcastMessage, dwParam, Strings));
                                    break;
                                }

                                break;
                            }
                    }
                }

            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
    }
}
