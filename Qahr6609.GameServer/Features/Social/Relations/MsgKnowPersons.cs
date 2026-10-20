using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgKnowPersons
    {
        public enum Action : byte
        {
            RequestFriendship = 10,
            AcceptRequest = 11,
            AddOnline = 12,
            AddOffline = 13,
            RemovePerson = 14,
            AddFriend = 15,

            RemoveEnemy = 18,
            AddEnemy = 19
        }

        public static unsafe void GetKnowPersons(this ServerSockets.Packet stream, out uint UID,out Action mode, out bool online)
        {
            UID = stream.ReadUInt32();
            mode = (Action)stream.ReadInt8();
            online = stream.ReadInt8() == 1;
          
        }
        public static unsafe ServerSockets.Packet KnowPersonsCreate(this ServerSockets.Packet stream, Action Typ, uint UID, bool online, string Name, uint NobilityRank, uint body)
        {
            stream.InitWriter();

            stream.Write(UID);//4
            stream.Write((byte)Typ);//8
            stream.Write((byte)(online == true ? 1 : 0));//9
            stream.Write((ushort)0);
            stream.Write((uint)NobilityRank);

            if (body % 10 < 3)
                stream.Write((uint)2);
            else
                stream.Write((uint)1);

            stream.Write(Name, 16);
            stream.ZeroFill(36);

            stream.Finalize(GamePackets.KnowPersons);
            return stream;
        }

        [PacketAttribute(GamePackets.KnowPersons)]
        public unsafe static void HandlerKnowPersons(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (!user.Player.OnMyOwnServer)
                return;
            uint UID;
            Action Mode;
            bool Online;
            stream.GetKnowPersons(out UID, out Mode, out Online);

            switch (Mode)
            {
                case Action.RequestFriendship:
                    {
                        if (user.Player.Associate.AllowAdd(Role.Instance.Associate.Friends, UID, 50))
                        {
                            Client.GameClient target;
                            if (Database.Server.GamePoll.TryGetValue(UID, out target))
                            {
                                if (Game.MsgServer.BlackList.Blocks(target.Player.UID, user.Player.Name))
                                    break;//on the target's black list
                                user.Player.TargetFriend = target.Player.UID;

                                target.Send(stream.PopupInfoCreate(user.Player.UID, target.Player.UID, user.Player.Level, user.Player.BattlePower));

                                target.Send(stream.KnowPersonsCreate(Mode, user.Player.UID, true, user.Player.Name,(uint)user.Player.NobilityRank, user.Player.Body));
                            }
                        }
                        break;
                    }
                case Action.AcceptRequest:
                    {
                        if (user.Player.Associate.AllowAdd(Role.Instance.Associate.Friends, UID, 50))
                        {
                            Client.GameClient target;
                            if (Database.Server.GamePoll.TryGetValue(UID, out target))
                            {
                                if (user.Player.UID != target.Player.TargetFriend)
                                    break;

                                target.Send(stream.KnowPersonsCreate(Action.AddFriend, user.Player.UID, true, user.Player.Name,(uint)user.Player.NobilityRank, user.Player.Body));

                                user.Send(stream.KnowPersonsCreate(Action.AddFriend, target.Player.UID, true, target.Player.Name,(uint)target.Player.NobilityRank, target.Player.Body));

                                user.Player.Associate.AddFriends(target, target.Player);
                                target.Player.Associate.AddFriends(target, user.Player);

                                user.SendSysMesage("" + user.Player.Name + " and " + target.Player.Name + " are friends from now on!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red, true);
                            }
                        }
                        break;
                    }
                case Action.RemovePerson:
                    {
                        if (user.Player.Associate.Remove(Role.Instance.Associate.Friends, UID))
                        {
                            user.Send(stream.KnowPersonsCreate(Action.RemovePerson, UID, Online, "",0,0));

                            Client.GameClient target;
                            if (Database.Server.GamePoll.TryGetValue(UID, out target))
                            {
                                if (target.Player.Associate.Remove(Role.Instance.Associate.Friends, user.Player.UID))
                                {
                                    target.Send(stream.KnowPersonsCreate(Action.RemovePerson, user.Player.UID, Online, "", (uint)target.Player.NobilityRank, target.Player.Body));
                                }
                            }
                            else
                                Role.Instance.Associate.RemoveOffline(Role.Instance.Associate.Friends, UID, user.Player.UID);

                        }
                        break;
                    }
                case Action.RemoveEnemy:
                    {
                        if (user.Player.Associate.Remove(Role.Instance.Associate.Enemy, UID))
                        {
                            user.Send(stream.KnowPersonsCreate(Action.RemovePerson, UID, Online, "",0,0));
                        }
                        break;
                    }
            }
        }
    }
}
