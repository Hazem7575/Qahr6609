using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{


    public unsafe static class MsgTradePartner
    {
        public enum Action : byte
        {
            RequestPartnership = 0,
            RejectRequest = 1,
            AddOnline = 2,
            AddOffline = 3,
            BreakPartnership = 4,
            AddPartner = 5
        }

        public static unsafe void GetTradePartner(this ServerSockets.Packet stream, out uint UID, out Action mode, out bool online, out int HoursLeft)
        {
            UID = stream.ReadUInt32();
            mode = (Action)stream.ReadInt8();
            online = stream.ReadInt8() == 1;
            HoursLeft = stream.ReadInt32();
        }

        public static unsafe ServerSockets.Packet TradePartnerCreate(this ServerSockets.Packet stream, uint UID, Action Typ, bool online, int HoursLeft, string Name)
        {
            stream.InitWriter();

            stream.Write(UID);//4
            stream.Write((byte)Typ);//8
            stream.Write((byte)(online == true ? 1 : 0));//9
            stream.Write((ushort)HoursLeft);
            stream.Write((uint)0);
            stream.Write(Name, 16);
            stream.Write((uint)0);

            stream.Finalize(GamePackets.TradePartner);
            return stream;
        }

        [PacketAttribute(GamePackets.TradePartner)]
        public unsafe static void HandlerTradePartner(Client.GameClient user, ServerSockets.Packet stream)
        {
              uint UID;
         Action Mode;
         bool Online;
         int HoursLeft;
         stream.GetTradePartner(out UID, out Mode, out Online, out HoursLeft);


            switch (Mode)
            {
                case Action.RequestPartnership:
                    {
                        if (user.Player.Associate.AllowAdd(Role.Instance.Associate.Partener,UID, 10))
                        {
                            Role.IMapObj obj;
                            if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                            {
                                Role.Player player = obj as Role.Player;

                                user.Player.TradePartner = UID;
                                if (player.TradePartner == user.Player.UID)
                                {
                                    if (user.Player.Associate.AllowAdd(Role.Instance.Associate.Partener, player.UID, 10))
                                    {
                                        if (player.Associate.AllowAdd(Role.Instance.Associate.Partener, user.Player.UID, 10))
                                        {
                                            user.Player.Associate.AddPartener(user, player);
                                            player.Associate.AddPartener(player.Owner, user.Player);

                                            HoursLeft = (int)(new TimeSpan(DateTime.Now.AddDays(3).Ticks).TotalMinutes - new TimeSpan(DateTime.Now.Ticks).TotalMinutes);

                                            player.Owner.Send(stream.TradePartnerCreate(user.Player.UID, Action.AddPartner, true, HoursLeft, user.Player.Name));
                                            user.Send(stream.TradePartnerCreate(player.UID, Action.AddPartner, true, HoursLeft, player.Name));
                                        }
                                    }

                                }
                                else
                                {
                                    player.Owner.Send(stream.PopupInfoCreate(user.Player.UID, player.UID, user.Player.Level, user.Player.BattlePower));

                                    player.Owner.Send(stream.TradePartnerCreate(user.Player.UID, Action.RequestPartnership, true, HoursLeft, user.Player.Name));
                                }
                            }
                        }

                        break;
                    }
                case Action.RejectRequest:
                    {
                        user.Player.TradePartner = 0;
                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            Role.Player player = obj as Role.Player;

                            player.Owner.Send(stream.TradePartnerCreate(user.Player.UID, Action.RejectRequest, true, HoursLeft, user.Player.Name));
                        }
                        break;
                    }
                case Action.BreakPartnership:
                    {
                        if (user.Player.Associate.Remove(Role.Instance.Associate.Partener, UID))
                        {
                            user.Send(stream.TradePartnerCreate(UID, Action.BreakPartnership, Online, HoursLeft, ""));


                            Client.GameClient target;
                            if (Database.Server.GamePoll.TryGetValue(UID, out target))
                            {
                                if (target.Player.Associate.Remove(Role.Instance.Associate.Partener, user.Player.UID))
                                {
                                    target.Send(stream.TradePartnerCreate(user.Player.UID, Action.BreakPartnership, Online, HoursLeft, ""));
                                }
                            }
                            else
                                Role.Instance.Associate.RemoveOffline(Role.Instance.Associate.Partener, UID, user.Player.UID);
                        }
                        break;
                    }

            }

        }
    }
}
