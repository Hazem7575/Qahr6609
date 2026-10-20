using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetOfflineTG(this ServerSockets.Packet stream, out MsgOfflineTG.Mode Mode)
        {
            Mode = (MsgOfflineTG.Mode)stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet OfflineTGCreate(this ServerSockets.Packet stream, MsgOfflineTG.Mode Mode)
        {
            stream.InitWriter();

            stream.Write((uint)Mode);
            stream.Write(0);


            stream.Finalize(GamePackets.OfflineTG);

            return stream;
        }


    }
    public struct MsgOfflineTG
    {
        public enum Mode : uint
        {
            OnConfirmation = 0,
            Disconnect = 1,
            UnKnow = 2,//if send that receive -> ReplyToConfirmation
            ReplyToConfirmation = 3,
            ClaimExperience = 4
        }

        [PacketAttribute(Game.GamePackets.OfflineTG)]
        public unsafe static void OfflineTG(Client.GameClient client, ServerSockets.Packet packet)
        {
            MsgOfflineTG.Mode Action;

            packet.GetOfflineTG(out Action);

            switch (Action)
            {
                case Mode.OnConfirmation:
                    {
                        if (client.Player.OnMyOwnServer == false)
                            return;
                        if (!client.Player.Alive)
                        {
                            client.SendSysMesage("Please revive your character.");

                            break;
                        }
                        if (client.Player.Map == 1017 || client.Player.Map == 1081 || client.Player.Map == 2060 || client.Player.Map == 9972
                      || client.Player.Map == 1080 || client.Player.Map == 3820 || client.Player.Map == 3954
                  || client.Player.Map == 1806
                      || Game.MsgTournaments.MsgSchedules.DisCity.IsInDisCity(client.Player.Map) || client.Player.Map == 1508
                      || client.Player.Map == Game.MsgTournaments.MsgTeamDeathMatch.MapID || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(client.Player.Map)
              || client.Player.Map == 1768
              || client.Player.Map == Game.MsgTournaments.MsgFootball.MapID
              || client.Player.Map == 1505 || client.Player.Map == 1506 || client.Player.Map == 1509 || client.Player.Map == 1508 || client.Player.Map == 1507)
                        {
                            client.SendSysMesage("The Offline TG is not allowed on this map.");

                            break;
                        }

                        if (client.Player.Map == 1038 || client.Player.Map == MsgTournaments.MsgClassPKWar.MapID || client.Player.DynamicID != 0
                            || client.Player.Map == 6001)
                        {
                            client.SendSysMesage("The Offline TG is not allowed on this map.");

                            break;
                        }
                        if (client.Player.Map != 601)
                        {
                            if (client.Player.Map != 1036)
                            {
                                client.Player.PMap = client.Player.Map;
                                client.Player.PMapX = client.Player.X;
                                client.Player.PMapY = client.Player.Y;
                            }
                            client.Map.View.LeaveMap<Role.IMapObj>(client.Player);
                            client.Player.Map = 601;
                            client.Player.X = 64;
                            client.Player.Y = 56;
                            client.Player.JoinOnflineTG = DateTime.Now;
                        }
                        client.Send(packet.OfflineTGCreate(Mode.Disconnect));
                        break;
                    }
                case Mode.ClaimExperience:
                    {
                        if (client.Player.Map == 601)
                        {
                            var T1 = new TimeSpan(DateTime.Now.Ticks);
                            var T2 = new TimeSpan(client.Player.JoinOnflineTG.Ticks);
                            ushort minutes = (ushort)(T1.TotalMinutes - T2.TotalMinutes);
                            minutes = (ushort)Math.Min((ushort)900, minutes);
                            client.Player.JoinOnflineTG = DateTime.Now;
                            client.GainExpBall(minutes, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Teleport(client.Player.PMapX, client.Player.PMapY, client.Player.PMap);
                        }
                        break;
                    }

            }
        }
    }
}
