using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgTournaments;
using Conquer6609.Game.Conquer6609_Work;
using System.IO;
using Conquer6609.Role;

namespace Conquer6609.Game.MsgServer
{
    public class MsgMessage
    {


        public enum MsgColor : uint
        {
            black = 0x000000,// 	0,0,0
            blue = 0x0000ff,// 	0,0,255
            orange = 0xffa500,// 	255,165,0

            white = 0xffffff,//	255,255,255
            whitesmoke = 0xf5f5f5,// 	245,245,245
            yellow = 0xffff00,// 	255,255,0
            yellowgreen = 0x9acd32,//	154,205,50
            violet = 0xee82ee,//	238,130,238
            purple = 0x800080,//	128,0,128
            red = 0xff0000,//	255,0,0
            pink = 0xffc0cb,// 	255,192,203
            lightyellow = 0xffffe0,// 	255,255,224
            cyan = 0x00ffff,// 	0,255,255
            blueviolet = 0x8a2be2,// 	138,43,226
            antiquewhite = 0xfaebd7,// 	250,235,215
        }
        public enum ChatMode : uint
        {
            Talk = 2000,
            Whisper = 2001,
            Team = 2003,
            Guild = 2004,
            TopLeftSystem = 2005,
            Clan = 2006,
            System = 2000,//2007,
            Friend = 2009,
            Center = 2011,
            TopLeft = 2012,
            Service = 2014,
            Tip = 2015,
            CrossServerIcon = 2016,
            Ally = 2025,
            WebSite = 2105,
            World = 2021,
            Qualifier = 2022,
            Study = 2024,
            JianHu = 2026,
            InnerPower = 2027,
            PopUP = 2100,
            Dialog = 2101,
            CrosTheServer2 = 2400,
            SlideCrosTheServer = 2401,
            CrosTheServer = 2402,
            FirstRightCorner = 2108,
            ContinueRightCorner = 2109,
            SystemWhisper = 2110,
            GuildAnnouncement = 2111,

            Agate = 2115,
            BroadcastMessage = 2500,
            Monster = 2600,
            SlideFromRight = 100000,
            HawkMessage = 2104,
            SlideFromRightRedVib = 1000000,
            WhiteVibrate = 10000000
        }

        public string _From;
        public string _To;
        public ChatMode ChatType;
        public uint Color;
        public string __Message;
        public string ServerName = string.Empty;

        public uint Mesh;
        public uint MessageUID1 = 0;
        public uint MessageUID2 = 0;
        public MsgMessage(string _Message, uint Mesh, string __To, string __From, MsgColor _Color, ChatMode _ChatType)
        {
            this.Mesh = Mesh;
            this.__Message = _Message;
            this._To = __To;
            this._From = __From;
            this.Color = (uint)_Color;
            this.ChatType = _ChatType;
        }
        public MsgMessage(string _Message, MsgColor _Color, ChatMode _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = "ALL";
            this._From = "SYSTEM";
            this.Color = (uint)_Color;
            this.ChatType = _ChatType;
        }
        public MsgMessage(string _Message, string __To, MsgColor _Color, ChatMode _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = __To;
            this._From = "SYSTEM";
            this.Color = (uint)_Color;
            this.ChatType = _ChatType;
        }
        public MsgMessage(string _Message, string __To, string __From, MsgColor _Color, ChatMode _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = __To;
            this._From = __From;
            this.Color = (uint)_Color;
            this.ChatType = _ChatType;
        }
        public MsgMessage()
        {
            this.Mesh = 0;
        }
        public unsafe void Deserialize(ServerSockets.Packet stream)
        {
            stream.ReadUInt32();
            Color = stream.ReadUInt32();
            ChatType = (ChatMode)stream.ReadUInt32();
            MessageUID1 = stream.ReadUInt32();
            MessageUID2 = stream.ReadUInt32();
            Mesh = stream.ReadUInt32();//24
            uint unknow = stream.ReadUInt32();//28
            byte unknow2 = stream.ReadUInt8();//32
            byte unknow3 = stream.ReadUInt8();//33
            string[] str = stream.ReadStringList();//34

            // missing strings in the list are treated as empty
            _From = StringAt(str, 0);
            _To = StringAt(str, 1);
            __Message = StringAt(str, 3);
        }
        private static string StringAt(string[] list, int index)
        {
            if (list == null || index < 0 || index >= list.Length || list[index] == null)
                return "";
            return list[index];
        }
        // a + b saturating at uint.MaxValue
        internal static uint ClampAddUInt(uint a, uint b)
        {
            ulong sum = (ulong)a + b;
            return sum > uint.MaxValue ? uint.MaxValue : (uint)sum;
        }
        // channels a player may send on; anything else is not relayed
        private static bool IsPlayerChannel(ChatMode mode)
        {
            switch (mode)
            {
                case ChatMode.Talk:
                case ChatMode.Whisper:
                case ChatMode.Team:
                case ChatMode.Guild:
                case ChatMode.Clan:
                case ChatMode.Friend:
                case ChatMode.World:
                case ChatMode.Ally:
                case ChatMode.HawkMessage:
                case ChatMode.CrosTheServer:
                    return true;
            }
            return false;
        }
        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, uint Rank = 0)
        {
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);//4
            stream.Write(this.Color);//8
            stream.Write((uint)this.ChatType);//12

            stream.Write(MessageUID1);//16
            stream.Write(MessageUID2);//20
            stream.Write(Mesh);//24
            stream.Write((uint)Rank);//28 
            stream.Write((byte)0);//32
            stream.Write((byte)0);
            stream.Write(_From, _To, string.Empty, __Message, string.Empty, string.Empty, ServerName);
            stream.Finalize(GamePackets.Chat);
            return stream;

        }

        [PacketAttribute(GamePackets.Chat)]
        public unsafe static void MsgHandler(Client.GameClient client, ServerSockets.Packet packet)
        {

            if (client.Player.IsStillBanned)
            {
                if (client.Player.PermenantBannedChat)
                {
                    client.SendSysMesage("Sorry, you still banned from chatting Permenatly.", ChatMode.System, MsgColor.white);
                }
                else
                {
                    client.SendSysMesage("Sorry, you still banned from chatting till " + client.Player.BannedChatStamp.ToString(), ChatMode.System, MsgColor.white);
                }
                return;
            }

            MsgMessage msg = new MsgMessage();
            msg.Deserialize(packet);
            // the sender is always the character bound to this connection
            msg._From = client.Player.Name;
            // '@' commands are audited by ChatCommands, not written to the chat log
            if (!msg.__Message.StartsWith("@"))
            {
                string log = ("[Chat]" + msg._From + " Speks to " + msg._To + " " + msg.__Message + " [Time] " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                Logs.OnDequeue(log);
            }
            if (!ChatCommands(client, msg))
            {
                if (!IsPlayerChannel(msg.ChatType))
                    return;
                try
                {
                    string[] lines = msg.__Message.Split(new string[] { "[" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int x = 0; x < lines.Length; x++)
                    {
                        string str = lines[x];
                        if (str.Contains("Item "))
                        {
                            string[] line = str.Split(' ');
                            if (line != null && line.Length > 2)
                            {
                                uint UID = 0;
                                if (uint.TryParse(line[2], out UID))
                                {
                                    MsgGameItem msg_item;
                                    if (client.TryGetItem(UID, out msg_item))
                                    {
                                        Program.GlobalItems.Add(msg_item);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MyConsole.WriteException(e);
                }

                if (msg.ChatType == ChatMode.Whisper && msg._To.Contains("[GM]"))
                {//لوحه الشات
                    // kept per player in a capped, thread-safe history (read by the Co_Panle chat page)
                    Co_Panle.AddChatLine(client.Player.Name, msg._From + "--->" + msg.__Message);
                    client.SendWhisper(msg._From + "--->" + msg.__Message + "," + "", msg._From, "[GM]");
                }
                msg.Mesh = client.Player.Mesh;
                switch (msg.ChatType)
                {
                    case ChatMode.CrosTheServer:
                        {
                            if (client.Inventory.Contain(3002218, 1))
                            {
                                // forward the server-built message (enforced sender), not the raw client packet
                                msg.GetArray(packet);
                                if (client.Player.InUnion)
                                    MsgInterServer.StaticConnexion.Send(packet);
                                else
                                    MsgInterServer.StaticConnexion.Send(packet);

                                client.Inventory.Remove(3002218, 1, packet);
                            }
                            break;
                        }
                    case ChatMode.Ally:
                        {
                            if (client.Player.MyGuild != null)
                            {
                                // members of every allied guild (same receivers as Guild.SendPacket per ally),
                                // packet built once, one pass over the online players
                                List<uint> allyIds = null;
                                foreach (var ally in client.Player.MyGuild.Ally)
                                {
                                    if (allyIds == null)
                                        allyIds = new List<uint>(4);
                                    allyIds.Add(ally.Value.Info.GuildID);
                                }
                                if (allyIds != null)
                                {
                                    msg.GetArray(packet);
                                    foreach (var kv in Database.Server.GamePoll)
                                    {
                                        var user = kv.Value;
                                        uint guildId = user.Player.GuildID;
                                        for (int i = 0; i < allyIds.Count; i++)
                                        {
                                            if (allyIds[i] == guildId)
                                                user.Send(packet);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case ChatMode.HawkMessage:
                        {
                            if (client.IsVendor)
                            {
                                client.MyVendor.HalkMeesaje = msg;

                                client.Player.View.SendView(msg.GetArray(packet), true);
                            }
                            break;
                        }
                    case ChatMode.Team:
                        {
                            if (client.Team != null)
                                client.Team.SendTeam(msg.GetArray(packet), client.Player.UID);
                            break;
                        }
                    case MsgMessage.ChatMode.Talk:
                        {
                            client.Player.View.SendView(msg.GetArray(packet), false);
                            break;
                        }
                    case MsgMessage.ChatMode.World:
                        {
                            if (Extensions.Time32.Now > client.Player.LastWorldMessaj.AddSeconds(15))
                            {
                                client.Player.LastWorldMessaj = Extensions.Time32.Now;
                                // same packet for everyone outside a union: built once; union members still get
                                // their own rank in it (built in a second buffer)
                                msg.GetArray(packet);
                                ServerSockets.RecycledPacket rankRec = null;
                                try
                                {
                                    foreach (var kv in Database.Server.GamePoll)
                                    {
                                        var user = kv.Value;
                                        if (user.Player.UID != client.Player.UID)
                                        {
                                            if (user.Player.InUnion)
                                            {
                                                if (rankRec == null)
                                                    rankRec = new ServerSockets.RecycledPacket();
                                                user.Send(msg.GetArray(rankRec.GetStream(), (uint)Role.Instance.Union.Member.GetRank(user.Player.UnionMemeber.Rank)));
                                            }
                                            else
                                                user.Send(packet);
                                        }
                                    }
                                }
                                finally
                                {
                                    if (rankRec != null)
                                        rankRec.Dispose();
                                }
                            }
                            break;
                        }
                    case ChatMode.Whisper:
                        {

                            bool send = false;
                            if (Co_Panle.On && msg._To.Contains("[GM]"))
                            {//لوحه الشات
                                send = true;
                                client.SendSysMesage("The GM Hase Resave Your Massege Plase Wait.", ChatMode.System, MsgColor.white);

                            }
                            //else if (!Co_Panle.On && msg._To.Contains("[GM]"))
                            //{
                            //    send = true;
                            //    var msgg = new MsgMessage("مدير العبه ليس متصل الان من فضلك العوده فى وقت اخر شكرا ", 1371006, client.Player.Name, "[GM]", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Whisper);
                            //    Program.SendGlobalPackets.Enqueue(msgg.GetArray(new ServerSockets.RecycledPacket().GetStream()));
                            //    client.SendSysMesage("The GM Not Online Now Plase Come Back Later.", ChatMode.System, MsgColor.white);
                            //}
                            foreach (var kv in Database.Server.GamePoll)
                            {
                                var user = kv.Value;
                                if (user.Player.Name == msg._To)
                                {
                                    send = true;
                                    if (Game.MsgServer.BlackList.Blocks(user.Player.UID, client.Player.Name))
                                        break;//on the receiver's black list: dropped
                                    msg.Mesh = client.Player.Mesh;
                                    user.Send(msg.GetArray(packet));
                                    send = true;
                                    break;
                                }
                            }
                            if (!send)
                            {
                                client.SendSysMesage("The player is not online.", ChatMode.System, MsgColor.white);

                            }
                            break;
                        }
                    case ChatMode.Guild:
                        {
                            if (client.Player.MyGuild != null)
                            {
                                // same receivers as Guild.SendPacket, without copying the whole player list
                                uint guildId = client.Player.MyGuild.Info.GuildID;
                                msg.GetArray(packet);
                                foreach (var kv in Database.Server.GamePoll)
                                {
                                    var user = kv.Value;
                                    if (user.Player.GuildID == guildId)
                                        user.Send(packet);
                                }
                            }
                            break;
                        }
                    case ChatMode.Friend:
                        {
                            System.Collections.Concurrent.ConcurrentDictionary<uint, Role.Instance.Associate.Member> friends;
                            if (client.Player.Associate.Associat.TryGetValue(Role.Instance.Associate.Friends, out friends))
                            {
                                // look the (few) friends up by UID instead of scanning every online player
                                msg.GetArray(packet);
                                foreach (var friend in friends)
                                {
                                    Client.GameClient user;
                                    if (Database.Server.GamePoll.TryGetValue(friend.Key, out user) && user.Player.UID == friend.Key)
                                        user.Send(packet);
                                }
                            }
                            break;
                        }
                    case ChatMode.Clan:
                        {
                            if (client.Player.MyClan != null)
                                client.Player.MyClan.Send(msg.GetArray(packet));
                            break;
                        }
                }

            }
        }
        public static uint TestGui = 0;


        public static unsafe bool ChatCommands(Client.GameClient client, MsgMessage msg)
        {
            // GM rights come only from the account record; chat cannot grant them
            bool staff = client.ProjectManager || client.GameMaster;
            msg.__Message = msg.__Message.Replace("#60", "").Replace("#61", "").Replace("#62", "").Replace("#63", "").Replace("#64", "").Replace("#65", "").Replace("#66", "").Replace("#67", "").Replace("#68", "");

            // '@' commands are [GmCommand] methods (Source/Commands, modules, built-ins) run by CommandRegistry,
            // which also writes the gm-command / player-command / gm-denied audit lines.
            if (msg.__Message.StartsWith("@"))
            {
                string[] data = msg.__Message.Substring(1).Split(' ');
                var info = Conquer6609.Framework.Commands.CommandRegistry.Find(data[0]);
                // accounts without GM rights only reach the commands they may use; a denied command keeps the old
                // behaviour below (gm-denied audit, text goes on as normal chat)
                if (info != null && Conquer6609.Commands.CommandSupport.Matches(info, data[0])
                    && (staff || Conquer6609.Framework.Commands.CommandRegistry.CanUse(client, info.Role)))
                {
                    bool handled;
                    Conquer6609.Framework.Commands.CommandRegistry.TryExecute(client, data, out handled);
                    if (handled)
                        return true;
                }
            }

            if (!staff)
            {
                if (msg.__Message.StartsWith("@"))
                {
                    // only the command word is recorded for accounts without GM rights
                    string Message = msg.__Message.Substring(1);
                    int space = Message.IndexOf(' ');
                    string command = space >= 0 ? Message.Substring(0, space) : Message;
                    if (command.Length > 32)
                        command = command.Substring(0, 32);
                    if (Conquer6609.Security.Guard.Throttle("gm-denied:" + client.Player.UID, 1000))
                        Conquer6609.Security.Guard.Audit("gm-denied", client.Player.UID, client.Player.Name, "@" + command);
                }
                return false;
            }
            if (msg.__Message.StartsWith("@"))
            {
                // unknown command from a GM/PM: audited and swallowed, as before
                string Message = msg.__Message.Substring(1);
                Conquer6609.Security.Guard.Audit("gm-command", client.Player.UID, client.Player.Name, "@" + Message);
                return true;
            }
            return false;
        }
    }
}
