using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgFamilyOccupy
    {
        public enum FamilyOccupyType : uint
        {
            ShowInformation = 6,
            Join = 8
        }
        public static unsafe ServerSockets.Packet CreateFamilyOccupy(this ServerSockets.Packet stream, FamilyOccupyType type, uint NpcID, Game.MsgTournaments.MsgClanWar.CityWar map, bool Info =false)
        {
            stream.InitWriter();
            stream.Write((uint)type);
            var Winner = map.Winner;
            if (Info)
                Winner = map.BestWinner;
            if (Winner != null)
                stream.Write((uint)Winner.ClainID);
            else
                stream.Write((uint)0);
            stream.Write(NpcID);
            stream.Write((uint)5);//????
            if (Winner != null)
                stream.Write(Winner.Name, 36);
            else
                stream.ZeroFill(36);
            stream.Write(map.Type.ToString(), 36);
            stream.Write((byte)(map.Proces == MsgTournaments.ProcesType.Alive ? 1 : 0));
            stream.Write((byte)1);//??
            stream.Write((ushort)1);//??
            if (Winner != null)
            {
                stream.Write(Winner.OccupationDays);
                stream.Write(Winner.Reward);
                stream.Write(Winner.NextReward);
            }
            else
                stream.ZeroFill(12);
            stream.ZeroFill(sizeof(uint) * 3);
            stream.Write(100);//???

            stream.Finalize(GamePackets.MsgFamilyOccupy);

            return stream;
        }
        public static unsafe void GetFamilyOccupy(this ServerSockets.Packet stream, out FamilyOccupyType Type, out uint NPCUID)
        {
            Type = (FamilyOccupyType)stream.ReadUInt32();
            stream.SeekForward(4);
            NPCUID = stream.ReadUInt32();

        }

        [PacketAttribute(GamePackets.MsgFamilyOccupy)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            
            FamilyOccupyType Type;
            uint NPCID;
            stream.GetFamilyOccupy(out Type, out NPCID);
            switch (Type)
            {
                case FamilyOccupyType.ShowInformation:
                    {
                        client.ActiveNpc = NPCID;
                        var War = Game.MsgTournaments.MsgSchedules.ClanWar.GetNpcTournament(NPCID);
                        if (War == null)
                        {
                            War = Game.MsgTournaments.MsgSchedules.ClanWar.GetNpcInformation(NPCID);
                            client.Send(stream.CreateFamilyOccupy(FamilyOccupyType.ShowInformation, NPCID, War,true));
                            break;
                        }
                        client.Send(stream.CreateFamilyOccupy(FamilyOccupyType.ShowInformation, NPCID, War));
                        break;
                    }
                case FamilyOccupyType.Join:
                    {
                        if (client.Player.MyClan == null)
                        {
                            client.SendSysMesage("Please make Clan, or join in one Clan.");
                            break;
                        }
                        var War = Game.MsgTournaments.MsgSchedules.ClanWar.GetNpcTournament(client.ActiveNpc);
                        if (War != null)
                            War.Join(client);
                        break;
                    }
            }

        }
    }
}
