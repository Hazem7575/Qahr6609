using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgRacePotion
    {
        public enum RaceItemType : ushort
        {
            Null = 8329,
            ChaosBomb = 8330,
            SpiritPotion = 8331,
            ExcitementPotion = 8332,
            FrozenTrap = 8333,
            ScreamBomb = 8334,
            SluggishPotion = 8335,
            GuardPotion = 8336,
            DizzyHammer = 8337,
            TransformItem = 8338,
            RestorePotion = 8339,
            SuperExcitementPotion = 8340,
        }
        public class RacePotion
        {
            public ushort Amount;
            public RaceItemType PotionType;
            public int Location;
            public uint dwParam;
        }

        public static ServerSockets.Packet CreateRecePotion(this ServerSockets.Packet stream, RacePotion item)
        {
            stream.InitWriter();

            stream.Write(item.Amount);
            stream.Write((ushort)item.PotionType);
            stream.Write(item.Location);
            stream.Write(item.dwParam);

            stream.Finalize(GamePackets.RacePotion);
            return stream;
        }
        public static unsafe void GetRecePotion(this ServerSockets.Packet stream, out RacePotion item)
        {
            item = new RacePotion();
            item.Amount = stream.ReadUInt16();
            item.PotionType = (RaceItemType)stream.ReadUInt16();
            item.Location = stream.ReadInt32();
            item.dwParam = stream.ReadUInt32();

        }
        [PacketAttribute(GamePackets.RacePotion)]
        private static void Poroces(Client.GameClient user, ServerSockets.Packet stream)
        {
            RacePotion pot;
            stream.GetRecePotion(out pot);

            int i = pot.Location - 1;
            if (i < 0 || i > 4)
                return;
            // only a rider who joined the running race may use race items (RacePotions is null for anyone who never joined)
            if (user.Player.RacePotions == null || !user.Player.Alive || !MsgTournaments.MsgSchedules.SteedRace.IsOn
                || user.Player.Map != MsgTournaments.MsgSteedRace.MAPID || !user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                return;
            if (user.Player.RacePotions[i] == null || user.Player.RacePotions[i].Count <= 0)
                return;
            var potion = user.Player.RacePotions[i];
            potion.Count--;

            pot.PotionType = potion.Type;
            pot.Amount = (ushort)potion.Count;
            user.Send(stream.CreateRecePotion(pot));

            user.ApplyRacePotion(potion.Type, pot.dwParam);

            if (potion.Count == 0)
                user.Player.RacePotions[i] = null;
        }
    }
}
