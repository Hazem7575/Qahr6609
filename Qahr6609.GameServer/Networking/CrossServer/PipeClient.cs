using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using System.Net.Sockets;
using System.Net;
using Conquer6609.MsgInterServer.Packets;

namespace Conquer6609.MsgInterServer
{
    public class PipeClient
    {
        public bool Alive { get; private set; }
        public ServerSockets.SecuritySocket Socket;
        public Client.GameClient Owner;
        public uint NewUserID = 0;
        public string ConnectToAddress = "";
        public ushort ConnectToPort = 0;
        public PipeClient()
        {

        }
        public PipeClient(ServerSockets.SecuritySocket _socket, Client.GameClient obj)
        {
            Socket = _socket;
            Alive = true;
            Owner = obj;
            Socket.Client = this;
        }
        public void CompleteConnected()
        {
            // Socket.CanSend = Socket.Alive = Socket.CanReceive = true;
            Owner.PipeClient = this;
            using (var rec = new ServerSockets.RecycledPacket())
            {

                var stream = rec.GetStream();

                if (Owner.Player.CheckTransfer)
                {
                    Send(stream.InterServerCheckTransferCreate(0, Owner.Player.UID));
                }
                else
                {
                    if (Owner.Player.OnTransfer)
                        Send(stream.HeroInfo(Owner.Player, 1));
                    else
                        Send(stream.HeroInfo(Owner.Player, 0));
                    foreach (var item in Owner.Inventory.ClientItems.Values)
                        item.Send(this, stream);
                    foreach (var item in Owner.Equipment.ClientItems.Values)
                        item.Send(this, stream);

                    Send(stream.SubClassCreate(MsgSubClass.Action.ShowGUI, Owner.Player.SubClass.StudyPoints, 0, Owner.Player.SubClass.src.Values.ToArray()));

                    if (Owner.Player.OnTransfer == false)
                    {
                        Owner.Send(stream.GuildInformationCreate(new MsgGuildInformation()
                        {
                            GuildID = Owner.Player.GuildID,
                            LeaderName = ""

                        }));
                        if (Owner.Player.MyChi != null)
                            Send(stream.ChiInfoCreate(MsgChiInfo.Action.InterServerStatus, Owner.Player.MyChi));
                    }



                    //send spells---
                    foreach (var spell in Owner.MySpells.ClientSpells.Values)
                        Send(stream.SpellCreate(spell));
                    //--
                    //send profs--
                    foreach (var prof in Owner.MyProfs.ClientProf.Values)
                        Send(stream.ProficiencyCreate(prof.ID, prof.Level, prof.Experience, prof.UID));
                    //----


                    if (Owner.Player.OnTransfer)
                    {
                     
                        if (Owner.Player.MyJiangHu != null)
                            Send(stream.InterServerJiangHuCreate(Owner.Player.MyJiangHu.ToString()));
                        if (Owner.Player.MyChi != null)
                            Send(stream.InterServerChiCreate((uint)Owner.Player.MyChi.ChiPoints, Owner.Player.MyChi.Dragon.ToString(), Owner.Player.MyChi.Phoenix.ToString()
                                , Owner.Player.MyChi.Turtle.ToString(), Owner.Player.MyChi.Tiger.ToString()));

                        if (Owner.Player.InnerPower != null)
                            Send(stream.InterServerInnerPowerCreate(Owner.Player.InnerPower.ToString()));

                        if (Owner.Achievement != null)
                        {
                            Owner.Player.Achievement.Save(Owner.Achievement);
                            Send(stream.InterServerAchievementCreate(Owner.Achievement.ToString()));
                        }
                        if (Owner.Player.SpecialTitles.Count > 0)
                        {
                            Send(stream.InterServerSpecialTitlesCreate(Database.ServerDatabase.GetSpecialTitles(Owner)));
                        }
                        Send(stream.InterServerRoleInfoCreate(Owner.Player));
                    }
                    else
                    {
                        //send guild--
                        if (Owner.Player.MyGuild != null && Owner.Player.MyGuildMember != null)
                        {
                            Send(stream.GuildInfoCreate((uint)(Database.GroupServerList.MyServerInfo.ID * 100000 + Owner.Player.MyGuild.Info.GuildID), Owner.Player.GuildRank, Owner.Player.MyGuild.GuildName
                                , Owner.Player.MyGuild.Info.LeaderName));
                        }
                        //----
                        //send union
                        if (Owner.Player.InUnion)
                        {



                            Send(stream.UnionInfoCreate((uint)(Database.GroupServerList.MyServerInfo.ID * 100000 + Owner.Player.MyUnion.UID), Owner.Player.UnionMemeber.Rank
                                , Owner.Player.MyUnion.Name, Owner.Player.MyUnion.Emperor, Owner.Player.MyUnion.IsKingdom));
                        }

                        //----

                        //send quests
                        if (ConnectToAddress == Database.GroupServerList.InterServer.IPAddress && ConnectToPort == Database.GroupServerList.InterServer.Port)
                        {
                            if (Owner.Player.SetLocationType == 0)
                            {
                                AcceptQuest(stream, Owner, 35024);
                                AcceptQuest(stream, Owner, 35007);
                                AcceptQuest(stream, Owner, 35025);
                                AcceptQuest(stream, Owner, 35028);
                                AcceptQuest(stream, Owner, 35034);
                            }
                        }
                        //---
                    }
                }

                Owner.Player.View.Clear(stream);
            }
            Owner.Map.Denquer(Owner);

        }

        public void Send(ServerSockets.Packet msg)
        {
            if (Alive)
                Socket.Send(msg);
        }

        /// <summary>Relays one client packet to the Realm inside a RelayClient envelope (pipe v2): the Realm only ever
        /// runs it through the client packet handlers. `stream` is the whole received client packet.</summary>
        public unsafe void SendClientPacket(ServerSockets.Packet stream)
        {
            if (!Alive)
                return;
            int size = stream.Size;
            if (size < 4 || size + 12 + PacketTypes.PipeFrameOverhead > PipeConfig.MaxFrameSize)
                return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var env = rec.GetStream();
                env.InitWriter();
                env.WriteUnsafe(stream.Memory, size);
                env.Finalize(PacketTypes.RelayClient);
                Socket.Send(env);
            }
        }
        public void Disconnect()
        {
            if (Alive)
            {
                Alive = false;
                Socket.Disconnect();
            }

        }

        public static void Connect(Client.GameClient user, string IPAddres, ushort Port)
        {
            // pipe v2 needs the shared secret ([InterServer] Secret, >= 32 chars); without it visits are off
            if (!PipeConfig.SecretUsable)
            {
                user.CreateBoxDialog("Cross-server travel is not available on this server.");
                return;
            }
            if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);

            LoginQueue.Enqueue(new PipeClient() { Owner = user, ConnectToAddress = IPAddres, ConnectToPort = Port });
        }
     
        public unsafe static void FilterPackets(ServerSockets.SecuritySocket obj, ServerSockets.Packet stream)
        {
            var pipe = obj.Client as PipeClient;
            ushort PacketID = stream.ReadUInt16();

            try
            {
                if (pipe == null)
                    return;
                var codec = obj.Pipe;
                if (codec == null || !codec.Ready)
                {
                    // pipe v2 handshake: Hello -> our proof; AuthOk -> check the Realm's proof, then the setup snapshot
                    bool ok = false;
                    if (codec != null && PacketID == PacketTypes.PipeHello)
                        ok = PipeHandshake.AnswerHello(obj, stream, Database.GroupServerList.MyServerInfo.ID);
                    else if (codec != null && PacketID == PacketTypes.PipeAuthOk && PipeHandshake.AcceptAuthOk(obj, stream))
                    {
                        ok = true;
                        pipe.CompleteConnected();
                    }
                    if (!ok)
                    {
                        Conquer6609.Security.Guard.Audit("interserver", pipe.Owner != null && pipe.Owner.Player != null ? pipe.Owner.Player.UID : 0, "",
                            "pipe handshake with " + obj.RemoteIp + " failed at packet " + PacketID);
                        obj.Disconnect();
                    }
                    return;
                }
                switch (PacketID)
                {
                    case PacketTypes.InterServer_CheckTransfer:
                        {
                            if (!PipeConfig.AllowLegacyTransfer)
                                break;
                            uint type;
                            uint UID;
                            stream.GetInterServerCheckTransfer(out type, out UID);
                            switch (type)
                            {
                                case 1:
                                    {
                                        pipe.Owner.CreateDialog(stream, "Transfer Failed ! is look like you already have account on " + pipe.Owner.Player.TransferToServer + ", please delete it then come back at me .", "Let me check.");
                                        pipe.Disconnect();
                                        break;
                                    }
                                case 2:
                                    {
                                        pipe.Owner.Player.CheckTransfer = false;
                                        pipe.Owner.Player.OnTransfer = true;
                                        if (pipe.Owner.Player.ConquerPoints > 200000)
                                            pipe.Owner.Player.ConquerPoints -= 200000;
                                        pipe.CompleteConnected();
                                        break;
                                    }
                                case 3://complete transfer
                                    {

                                        string MSG = "Success ! " + pipe.Owner.Player.Name + " has successfully transferred to " + pipe.Owner.Player.TransferToServer + " side .";
                                        Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                        pipe.Owner.Player.Delete = true;
                                        pipe.Owner.Socket.Disconnect();
                                        break;
                                    }
                            }
                            break;
                        }

                    case PacketTypes.InterServer_EliteRank:
                        {



                            break;
                        }
                    case PacketTypes.InterServer_ConnectionInfo:
                        {
                            uint ServerID;
                            uint Type;
                            stream.GetConnectionInfo(out Type, out ServerID);
                            if (Type == 1)
                            {
                                var ServerInfo = Database.GroupServerList.GetServer(ServerID);

                                pipe.Owner.Send(stream.MsgInterServerIdentifier(0, pipe.NewUserID, pipe.Owner.Player.UID, pipe.Owner.GetAllMainItems().ToArray()));
                                pipe.Owner.Send(stream.MsgInterServerIdentifier(1, pipe.Owner.Player.UID, pipe.Owner.Player.UID, pipe.Owner.GetAllMainItems().ToArray()));
                                RemoveQuest(stream, pipe.Owner, 35024);
                                RemoveQuest(stream, pipe.Owner, 35007);
                                RemoveQuest(stream, pipe.Owner, 35025);
                                RemoveQuest(stream, pipe.Owner, 35034);
                                RemoveQuest(stream, pipe.Owner, 35028);
                                pipe.NewUserID = 0;
                                pipe.Disconnect();
                                pipe.Owner.PipeClient = null;
                                if (ServerInfo != null)
                                    Connect(pipe.Owner, ServerInfo.IPAddress, ServerInfo.Port);
                            }
                            break;
                        }
                    case Game.GamePackets.MsgInterServerIdentifier:
                        {
                            if (pipe.Owner.Player.OnTransfer)
                            {
                                var action = new ActionQuery()
                                {
                                    ObjId = pipe.Owner.Player.UID,
                                    Type = ActionType.CompleteLogin
                                };
                                pipe.Send(stream.ActionCreate(&action));

                            }
                            else
                            {
                                uint mode, dwparam1, dwparam2;
                                stream.GetInterServerIdentifier(out mode, out dwparam1, out dwparam2);
                                pipe.NewUserID = dwparam2;
                                stream.Seek(stream.Size);
                                pipe.Owner.Send(stream.MsgInterServerIdentifier(0, pipe.Owner.Player.UID, dwparam2, pipe.Owner.GetAllMainItems().ToArray()));
                                var _server = Database.GroupServerList.GetServer(pipe.ConnectToAddress, pipe.ConnectToPort);
                                if (_server != null)
                                    pipe.Owner.Player.SendString(stream, MsgStringPacket.StringID.ServerName, 0, false, _server.Name);
                                else
                                {
                                    /*   if (pipe.Owner.Player.SetLocationType == 1)
                                       {
                                           pipe.Owner.Player.SendString(stream, MsgStringPacket.StringID.ServerName, 0, false, "CSElitePKTournament");
                                       }
                                       else*/
                                    pipe.Owner.Player.SendString(stream, MsgStringPacket.StringID.ServerName, 0, false, Database.GroupServerList.InterServer.Name);
                                }

                                if (pipe.Owner.Player.MyChi != null)
                                {
                                    Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(pipe.Owner, MsgChiInfo.Action.Upgrade, pipe.Owner, 142);
                                    foreach (var chipower in pipe.Owner.Player.MyChi)
                                    {
                                        if (!chipower.UnLocked || chipower.UID == 0)
                                            continue;// same rule as the login (MsgLoginHandler)
                                        pipe.Owner.Player.MyChi.SendQueryUpdate(pipe.Owner, chipower, stream);
                                    }
                                }
                                if (pipe.Owner.Player.MyJiangHu != null)
                                {
                                    pipe.Send(stream.StatusJiangHuCreate(pipe.Owner.Player.MyJiangHu.statusclient));
                                    pipe.Owner.Player.MyJiangHu.SendInfo(pipe.Owner, MsgJiangHuInfo.JiangMode.InfoStauts, false);
                                    pipe.Owner.Player.MyJiangHu.SendStatus(stream, pipe.Owner, pipe.Owner);
                                }
                                var action = new ActionQuery()
                                {
                                    ObjId = pipe.Owner.Player.UID,
                                    Type = ActionType.CompleteLogin
                                };
                                pipe.Send(stream.ActionCreate(&action));
                            }
                            break;
                        }
                    case Game.GamePackets.Item:
                        {
                            MsgGameItem item;
                            stream.GetItemPacketPacket(out item);
                            if (item.Mode == Role.Flags.ItemMode.Update)
                            {
                                MsgGameItem ClientItem;
                                if (pipe.Owner.TryGetItem(item.UID, out ClientItem))
                                {
                                    if (ClientItem.Position == 0)
                                    {
                                        if (item.StackSize >= 1)
                                            ClientItem.StackSize = item.StackSize;
                                    }
                                }
                            }
                            stream.Seek(stream.Size);
                            pipe.Owner.Send(stream);
                            break;
                        }
                    case Game.GamePackets.Usage:
                        {
                            MsgItemUsuagePacket.ItemUsuageID action;
                            uint id;

                            ulong dwParam;
                            uint timestamp;
                            uint dwParam2;
                            uint dwParam3;
                            uint dwparam4;//unknow
                            List<uint> args;


                            stream.GetUsageItem(out action, out id, out dwParam, out timestamp, out dwParam2, out dwParam3, out dwparam4, out args);

                            if (action == MsgItemUsuagePacket.ItemUsuageID.RemoveInventory)
                            {
                                MsgGameItem item;
                                if (pipe.Owner.Inventory.TryGetItem(id, out item))
                                {
                                    if (item.ITEM_ID >= 1000000 && item.ITEM_ID <= 1000040
                                        || item.ITEM_ID >= 1002000 && item.ITEM_ID <= 1002030
                                        || item.ITEM_ID == 1002050
                                        || item.ITEM_ID == 725065 || item.ITEM_ID == 1003010
                                        || item.ITEM_ID >= 1001000 && item.ITEM_ID <= 1001040
                                        || item.ITEM_ID == 1002030 || item.ITEM_ID == 1002040
                                        || item.ITEM_ID == 725066 || item.ITEM_ID == 1004010)
                                    {
                                        pipe.Owner.Inventory.ClientItems.TryRemove(id, out item);
                                    }
                                }
                            }
                            stream.Seek(stream.Size);
                            pipe.Owner.Send(stream);
                            break;
                        }
                    case Game.GamePackets.Update:
                        {
                            MsgUpdate.DataType Action;
                            ulong Value;
                            stream.GetUpdatePacket(out Action, out Value);

                            switch (Action)
                            {
                                // Balances from the peer may never grow by more than one operation's limit at a time.
                                case MsgUpdate.DataType.ConquerPoints:
                                    {
                                        ulong cur = pipe.Owner.Player.ConquerPoints;
                                        ulong max = Math.Min(cur + (ulong)Conquer6609.Security.Guard.MaxCpsAmount, (ulong)PipeServer.Security.MaxPeerConquerPoints);
                                        if (Value > max)
                                        {
                                            Conquer6609.Security.Guard.Audit("interserver", pipe.Owner.Player.UID, pipe.Owner.Player.Name, "peer CP update clamped " + Value + " -> " + max);
                                            Value = Math.Max(max, cur);
                                        }
                                        pipe.Owner.Player.ConquerPoints = (uint)Math.Min(Value, (ulong)uint.MaxValue);
                                        break;
                                    }
                                case MsgUpdate.DataType.Money:
                                    {
                                        long cur = pipe.Owner.Player.Money;
                                        long max = cur > long.MaxValue - Conquer6609.Security.Guard.MaxMoneyAmount ? long.MaxValue : cur + Conquer6609.Security.Guard.MaxMoneyAmount;
                                        long val = PipeServer.Security.ClampMoney(Value);
                                        if (val > max)
                                        {
                                            Conquer6609.Security.Guard.Audit("interserver", pipe.Owner.Player.UID, pipe.Owner.Player.Name, "peer money update clamped " + Value + " -> " + max);
                                            val = Math.Max(max, cur);
                                        }
                                        pipe.Owner.Player.Money = val;
                                        break;
                                    }
                                case MsgUpdate.DataType.InnerPowerPotency: pipe.Owner.Player.InnerPower.Potency = (int)Math.Min(Value, (ulong)int.MaxValue); break;
                                case MsgUpdate.DataType.Level:
                                    {
                                        // Level from the peer is bounded to the valid range.
                                        ulong lvl = Math.Min(Value, (ulong)PipeServer.Security.MaxPeerLevel);
                                        if (lvl < 1) lvl = 1;
                                        pipe.Owner.Player.Level = (byte)lvl;
                                        break;
                                    }
                                case MsgUpdate.DataType.Experience: pipe.Owner.Player.Experience = (ulong)Value; break;
                            }
                            stream.Seek(stream.Size);
                            pipe.Owner.Send(stream);
                            break;
                        }
                    case PacketTypes.InterServer_CreateItem:
                        {
                            uint ID;
                            stream.GetInterCreateItem(out ID);
                            // Only items that exist in the item table may be created from the peer.
                            if (Database.Server.ItemsBase != null && Database.Server.ItemsBase.ContainsKey(ID))
                                pipe.Owner.Inventory.Add(stream, ID);
                            else
                                Conquer6609.Security.Guard.Audit("interserver", pipe.Owner.Player.UID, pipe.Owner.Player.Name, "peer item create rejected " + ID);
                            break;
                        }
                    case Game.GamePackets.QuestList:
                        {
                            MsgQuestList.QuestMode Mode;
                            ushort Count;
                            uint QuestID;
                            MsgQuestList.QuestListItem.QuestStatus QuestaMode;
                            uint QuestTimer;

                            stream.GetQuestList(out Mode, out Count, out QuestID, out QuestaMode, out QuestTimer);
                            if (QuestID == 35028 && QuestaMode == MsgQuestList.QuestListItem.QuestStatus.Finished)
                            {
                                if (pipe.Owner.Player.QuestGUI.CheckQuest(QuestID, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    pipe.Owner.Inventory.Add(stream, 3600029);
                                    pipe.Owner.Player.QuestGUI.FinishQuest(QuestID);
                                }
                            }
                            /*if (QuestID == 35034 && QuestaMode == MsgQuestList.QuestListItem.QuestStatus.Finished)
                            {
                                if (pipe.Owner.Player.QuestGUI.CheckQuest(QuestID, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    pipe.Owner.Inventory.Add(stream, 35034);
                                    pipe.Owner.Player.QuestGUI.FinishQuest(QuestID);
                                }
                            }*/
                            stream.Seek(stream.Size);
                            pipe.Owner.Send(stream);
                            break;
                        }
                    case Game.GamePackets.QuestData:
                        {
                            uint UnKnow; uint UID;
                            uint[] intents;
                            stream.GetQuestData(out UnKnow, out UID, out intents);
                            //if (user.OnInterServer)
                            {
                                if (Database.QuestInfo.IsKingDomMission(UID))
                                {
                                    pipe.Owner.Player.QuestGUI.SetKingDomQuestObjectives(stream, UID, intents);
                                }
                            }
                            stream.Seek(stream.Size);
                            pipe.Owner.Send(stream);
                            break;

                        }
                    default:
                        {
                            stream.Seek(stream.Size);

                            pipe.Owner.Send(stream);
                            break;
                        }
                }

            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
            finally
            {
                ServerSockets.PacketRecycle.Reuse(stream);
            }
        }

        public static void AcceptQuest(ServerSockets.Packet stream, Client.GameClient user, uint MissionID)
        {
            Game.MsgServer.MsgQuestList.QuestListItem Quest;
            /*if (user.Player.QuestGUI.CheckQuest(MissionID, MsgQuestList.QuestListItem.QuestStatus.Finished))
                   return;*/
            if (MissionID == 35028)
            {
                if (user.Player.QuestGUI.AcceptKingDomMission(Database.QuestInfo.GetFinishQuest((ushort)Game.MsgNpc.NpcID.Crystal, user.Player.Class, MissionID), 0, out Quest))
                    CreateQuest(stream, user, MissionID,Quest);
                return;
            }
            if (user.Player.QuestGUI.AcceptKingDomMission(Database.QuestInfo.GetFinishQuest((ushort)Game.MsgNpc.NpcID.KingdomMissionEnvoy, user.Player.Class, MissionID), 0,out Quest))
                CreateQuest(stream, user, MissionID,Quest);
        }
        public static void RemoveQuest(ServerSockets.Packet stream, Client.GameClient user, uint MissionID)
        {
            stream.QuestListCreate(MsgQuestList.QuestMode.QuitQuest, 1);
            stream.AddItemQuestList(new MsgQuestList.QuestListItem() { UID = MissionID, Status = MsgQuestList.QuestListItem.QuestStatus.Available });
            stream.QuestListFinalize();
            user.Send(stream);
        }

        public static void CreateQuest(ServerSockets.Packet stream, Client.GameClient user, uint MissionID, Game.MsgServer.MsgQuestList.QuestListItem Quest)
        {
        //    if (user.Player.QuestGUI.CheckQuest(MissionID, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
            {
                stream.QuestListCreate(MsgQuestList.QuestMode.AcceptQuest, 1);
                stream.AddItemQuestList(new MsgQuestList.QuestListItem() { UID = MissionID, Status = MsgQuestList.QuestListItem.QuestStatus.Accepted });
                stream.QuestListFinalize();
                user.PipeClient.Send(stream);
                user.PipeClient.Send(stream.MsgQuestDataCreate(0, Quest.UID, Quest.Intentions));
            }
        }

        public static ExecuteLogin LoginQueue = new ExecuteLogin();

        public class ExecuteLogin : ConcurrentSmartThreadQueue<PipeClient>
        {

            public ExecuteLogin()
                : base(5)
            {
                Start(10);
            }
            public void TryEnqueue(PipeClient obj)
            {
                base.Enqueue(obj);
            }
            protected unsafe override void OnDequeue(PipeClient obj, int time)
            {
                try
                {
                    var socket = new ServerSockets.SecuritySocket(new Action<ServerSockets.SecuritySocket>(Disconnect), new Action<ServerSockets.SecuritySocket, ServerSockets.Packet>(FilterPackets));
                    Socket _socket;
                    if (socket.Connect(obj.ConnectToAddress, obj.ConnectToPort, out _socket))
                    {
                        // pipe v2: the codec is attached before the socket is polled; the Realm sends PipeHello first,
                        // FilterPackets answers with the HMAC proof and sends the snapshot only after the Realm's proof.
                        socket.Pipe = new PipeCodec(false, PipeLinkType.Relay);
                        socket.OnInterServer = true;
                        socket.Create(_socket);
                        var pipe = new PipeClient(socket, obj.Owner);
                        pipe.ConnectToAddress = obj.ConnectToAddress;
                        pipe.ConnectToPort = obj.ConnectToPort;
                        PipeHandshake.ArmTimeout(socket, "pipe client");
                        socket.ConnectFull = true;
                    }
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
            }
        }







        public static void Disconnect(ServerSockets.SecuritySocket obj)
        {
            var pipe = obj.Client as PipeClient;
            if (pipe == null)
                return;
            if (pipe.Owner != null && pipe.Owner.PipeClient == pipe && pipe.NewUserID == 0)
            {
                // link closed before the Realm accepted the visit: back to the home map
                pipe.Owner.PipeClient = null;
                if (pipe.Owner.Player != null && pipe.Owner.Socket != null && pipe.Owner.Socket.Alive)
                    pipe.Owner.Teleport(pipe.Owner.Player.X, pipe.Owner.Player.Y, pipe.Owner.Player.Map);
            }
            if (pipe.NewUserID != 0)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var msg = rec.GetStream();
                    if (!pipe.Owner.Player.CheckTransfer)
                    {
                        pipe.Owner.Send(msg.MsgInterServerIdentifier(0, pipe.NewUserID, pipe.Owner.Player.UID, pipe.Owner.GetAllMainItems().ToArray()));
                        pipe.Owner.Send(msg.MsgInterServerIdentifier(1, pipe.Owner.Player.UID, pipe.Owner.Player.UID, pipe.Owner.GetAllMainItems().ToArray()));


                        if (pipe.Owner.Player.SetLocationType == 0)
                        {
                            RemoveQuest(msg, pipe.Owner, 35024);
                            RemoveQuest(msg, pipe.Owner, 35007);
                            RemoveQuest(msg, pipe.Owner, 35025);
                            RemoveQuest(msg, pipe.Owner, 35034);
                            RemoveQuest(msg, pipe.Owner, 35028);
                        }
                        pipe.Owner.Player.Stamina = 100;
                        pipe.Owner.Player.SendUpdate(msg, pipe.Owner.Player.Stamina, MsgUpdate.DataType.Stamina);
                        pipe.Owner.Player.SetLocationType = 0;


                        pipe.Owner.Player.CreateHeavenBlessPacket(msg, false);

                        if (pipe.Owner.Player.MyGuild != null && pipe.Owner.Player.MyGuildMember != null)
                            pipe.Owner.Player.GuildBattlePower = pipe.Owner.Player.MyGuild.ShareMemberPotency(pipe.Owner.Player.MyGuildMember.Rank);
                        if (pipe.Owner.Player.VipLevel >= 6)
                            pipe.Owner.Player.UpdateVip(msg);

                        pipe.Owner.Equipment.QueryEquipment(pipe.Owner.Equipment.Alternante);
                        pipe.Owner.Player.CreateHeavenBlessPacket(msg, true);
                    }
                    
                    pipe.NewUserID = 0;
                    pipe.Disconnect();
                    pipe.Owner.PipeClient = null;
                    pipe.Owner.Teleport(pipe.Owner.Player.X, pipe.Owner.Player.Y, pipe.Owner.Player.Map);
                    pipe.Owner.Player.SetPkMode(Role.Flags.PKMode.Capture);


                    pipe.Owner.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                    pipe.Owner.Player.UpdateFlagOffset();


                    if (!pipe.Owner.Player.CheckTransfer)
                    {
                        if (pipe.Owner.Player.MyGuild != null)
                            pipe.Owner.Player.MyGuild.SendThat(pipe.Owner.Player);

                        pipe.Owner.Player.SendUpdateHP();

                    }

                   
                }
            }
        }
    }
}
