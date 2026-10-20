using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet RouletteSignUpCreate(this ServerSockets.Packet stream, MsgRouletteSignUp.ActionJoin Action,uint UID)
        {
            stream.InitWriter();

            stream.Write((byte)Action);
            stream.Write(UID);

            stream.Finalize(GamePackets.MsgRouletteSignUp);
            return stream;
        }

        public static void GetRouletteSignUp(this ServerSockets.Packet stream, out MsgRouletteSignUp.ActionJoin Action, out uint UID)
        {
            Action = (MsgRouletteSignUp.ActionJoin)stream.ReadUInt8();
            UID = stream.ReadUInt32();

        }
    }

    public unsafe struct MsgRouletteSignUp
    {
        public enum ActionJoin : byte
        {
            Join = 0,
            Watch = 4,
            InfoTable = 5,//?????
            Quit = 7
        }

        [PacketAttribute(GamePackets.MsgRouletteSignUp)]
        private static void Poroces(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgRouletteSignUp.ActionJoin Typ; 
            uint UID;

            stream.GetRouletteSignUp(out Typ, out UID);

            // Joining/watching needs the player to stand in the Roulette Room next to the table.
            if (Typ == ActionJoin.Join || Typ == ActionJoin.Watch)
            {
                Database.Roulettes.RouletteTable near;
                if (!Database.Roulettes.RoulettesPoll.TryGetValue(UID, out near))
                    return;
                if (user.Player.Map != near.TableMap || user.Player.GetMyDistance(near.SpawnPacket.X, near.SpawnPacket.Y) > 18)
                    return;
            }

            switch (Typ)
            {
                case ActionJoin.InfoTable:
                    {
                         Database.Roulettes.RouletteTable Table;
                         if (Database.Roulettes.RoulettesPoll.TryGetValue(UID, out Table))
                         {
                             Table.SpawnPacket.PlayersCount = (byte)Table.RegistredPlayers.Count;
                             user.Send(stream.RouletteTableCreate(Table.SpawnPacket.UID, Table.SpawnPacket.TableNumber, Table.SpawnPacket.MoneyType
                                 , Table.SpawnPacket.X, Table.SpawnPacket.Y, Table.SpawnPacket.Mesh, Table.SpawnPacket.PlayersCount));
                         }
                        break;
                    }
                case ActionJoin.Join:
                    {
                        Database.Roulettes.RouletteTable Table;
                        if (Database.Roulettes.RoulettesPoll.TryGetValue(UID, out Table))
                        {
                            switch (Table.SpawnPacket.MoneyType)
                            {
                                case MsgRouletteTable.TableType.ConquerPoints:
                                    {
                                        if (user.Player.ConquerPoints >= 5)
                                        {
                                            Table.AddPlayer(stream, user);
                                        }
                                        else
                                        {
                                            user.SendSysMesage("Sorry, you should have at least 5 CPs to join in the Roulette.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                          
                                        }
                                        break;
                                    }
                                case MsgRouletteTable.TableType.Money:
                                    {
                                        if (user.Player.Money >= 5000)
                                        {
                                            Table.AddPlayer(stream, user);
                                        }
                                        else
                                        {
                                            user.SendSysMesage("Sorry, you should have at least 5,000 Silvers to join in the Roulette.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                           
                                        }
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case ActionJoin.Watch:
                    {
                        Database.Roulettes.RouletteTable Table;
                        if (Database.Roulettes.RoulettesPoll.TryGetValue(UID, out Table))
                        {
                            Table.AddWatch(stream,user);
                        }
                        break;
                    }
            }
        }
    }
}
