using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;
using Conquer6609.Database;

namespace Conquer6609.Role
{
    public unsafe partial class Player
    {
        public unsafe void MessageBoxx(string text, Action<Client.GameClient> msg_ok, Action<Client.GameClient> msg_cancel, int Seconds = 0, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None, bool automsg = false)
        {//ayman
            if (!OnMyOwnServer)
                return;

            if (Program.BlockTeleportMap.Contains(Owner.Player.Map))
            {
                if (Owner != null && Owner.Map != null)
                    Owner.SendSysMesage("you can't use it in " + Owner.Map.Name + " ");
                return;
            }
            if (this.Map == 6000)//jail
            {
                return;
            }
            if (this.Map == 6003)//jail
            {
                return;
            }
            if (Owner.AutoHunting.Enable && !automsg)
            {
                Owner.SendSysMesage("[MSG AUTOHUNT] " + text);
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                MessageOK = msg_ok;
                MessageCancel = msg_cancel;
                Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(Owner, stream);
                dialog.CreateMessageBox(text).FinalizeDialog(true);
                StartMessageBox = Extensions.Time32.Now.AddHours(24);
                if (Seconds != 0)
                {
                    StartMessageBox = Extensions.Time32.Now.AddSeconds(Seconds);
                    if (messaj != Game.MsgServer.MsgStaticMessage.Messages.None)
                    {
                        Owner.Send(stream.StaticMessageCreate(messaj, MsgStaticMessage.Action.Append, (uint)Seconds));
                    }
                }
            }
        }
        public void UpdateInventorySash(ServerSockets.Packet stream)
        {
            SendUpdate(stream, MaxInventorySashCount, MsgUpdate.DataType.InventorySashMax);
            SendUpdate(stream, InventorySashCount, MsgUpdate.DataType.InventorySash);
        }
        public void AddPick(ServerSockets.Packet stream, string Name, ushort timer)
        {
            PickStamp = Extensions.Time32.Now.AddSeconds(timer);
            Owner.Send(stream.ActionPick(UID, 1, timer, Name));
            ActivePick = true;
            ActionQuery action = new ActionQuery()
            {
                ObjId = UID,
                Type = (ActionType)1165,
                wParam1 = 277,
                wParam2 = 2050
            };
            Owner.Send(stream.ActionCreate(&action));
        }
        public void RemovePick(ServerSockets.Packet stream)
        {
            ActivePick = false;
            Owner.Send(stream.ActionPick(UID, 3, 0, Name));
        }
     

        public unsafe void memcpy(void* dest, void* src, Int32 size)
        {
            Int32 count = size / sizeof(long);
            for (Int32 i = 0; i < count; i++)
                *(((long*)dest) + i) = *(((long*)src) + i);

            Int32 pos = size - (size % sizeof(long));
            for (Int32 i = 0; i < size % sizeof(long); i++)
                *(((Byte*)dest) + pos + i) = *(((Byte*)src) + pos + i);
        }
        public unsafe byte[] GetBytes(byte* packet)
        {
            int size = *(ushort*)(packet);
            size += 8;
            byte[] buff = new byte[size];
            fixed (byte* ptr = buff)
                memcpy(ptr, packet, size);
            return buff;
        }
        public unsafe void MessageBox(string text, Action<Client.GameClient> msg_ok, Action<Client.GameClient> msg_cancel, int secounds = 0, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None)
        {
            if (!OnMyOwnServer)
                return;
          
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                MessageOK = msg_ok;
                MessageCancel = msg_cancel;
                Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(Owner, stream);
                dialog.CreateMessageBox(text).FinalizeDialog(true);
                StartMessageBox = Extensions.Time32.Now.AddHours(24);
                if (secounds != 0)
                {
                    StartMessageBox = Extensions.Time32.Now.AddSeconds(secounds);
                    if (messaj != Game.MsgServer.MsgStaticMessage.Messages.None)
                    {
                        Owner.Send(stream.StaticMessageCreate(messaj, MsgStaticMessage.Action.Append, (uint)secounds));
                    }
                  
                }
            }
        }
        public unsafe void Send(ServerSockets.Packet msg)
        {
            Owner.Send(msg);
        }

        public unsafe void SendUpdate(ServerSockets.Packet stream, long Value, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, Value);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
            {
                View.SendView(stream, false);
            }
        }

        public unsafe void SendUpdate(ServerSockets.Packet stream, Game.MsgServer.MsgUpdate.Flags Flag, uint Time, uint Dmg, uint Level, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, (byte)Flag, Time, Dmg, Level);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }
        public unsafe void SendUpdate(ServerSockets.Packet stream, Game.MsgServer.MsgUpdate.DataType datatype, uint Time, uint Dmg, uint Level, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, (byte)0, Time, Dmg, Level);
            stream = packet.GetArray(stream);

            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }
        public unsafe void SendUpdate(uint[] Value, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = packet.Append(stream, datatype, Value);
                stream = packet.GetArray(stream);
                Owner.Send(stream);
                if (scren)
                    View.SendView(stream, false);
            }
        }
        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, bool SendScreen, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = UID;
            packet.Strings = args;

            if (SendScreen)
                View.SendView(stream.StringPacketCreate(packet), true);
            else
                Owner.Send(stream.StringPacketCreate(packet));
        }
        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, uint _uid, bool SendScreen, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = _uid;
            packet.Strings = args;

            if (SendScreen)
                View.SendView(stream.StringPacketCreate(packet), true);
            else
                Owner.Send(stream.StringPacketCreate(packet));
        }



        public void AddMapEffect(ServerSockets.Packet stream, ushort x, ushort y, params string[] effect)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = MsgStringPacket.StringID.LocationEffect;
            packet.X = x;
            packet.Y = y;
            packet.Strings = effect;
            View.SendView(stream.StringPacketCreate(packet), true);

         
        }
    }

}
