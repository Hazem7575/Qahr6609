using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe struct MsgRouletteCheck
    {
        public class Item
        {
            public byte Number;
            public uint BetPrice;
        }

        const int MaxBetsPerRound = 64;

        /// <summary>Outside bets the table pays out on (colour, odd/even, small/big, dozens, lines).</summary>
        static bool IsExtraBet(byte number)
        {
            switch (number)
            {
                case 111: case 112:
                case 121: case 122: case 123:
                case 131: case 132: case 133:
                case 141: case 142:
                case 151: case 152:
                    return true;
            }
            return false;
        }

        [PacketAttribute(GamePackets.MsgRouletteCheck)]
        private static void Poroces(Client.GameClient user, ServerSockets.Packet stream)
        {
            byte Count = stream.ReadUInt8();


            if (user.PlayRouletteUID != 0)
            {
                Database.Roulettes.RouletteTable Table;
                if (Database.Roulettes.RoulettesPoll.TryGetValue(user.PlayRouletteUID, out Table))
                {
                    Database.Roulettes.RouletteTable.Member player;
                    if (Table.RegistredPlayers.TryGetValue(user.Player.UID, out player))
                    {
                        if (player.MyLuckNumber.Count != 0)
                            return;
                        if (player.MyLuckExtra.Count != 0)
                            return;
                        if (Count == 0 || Count > MaxBetsPerRound)
                            return;
                        // No bets while the wheel spins / pays out (they would be wiped by the next round's reset).
                        if (!Table.BettingOpen)
                        {
                            user.SendSysMesage("Bet overtime.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            return;
                        }

                        bool moneyTable = Table.SpawnPacket.MoneyType == MsgRouletteTable.TableType.Money;
                        bool cpsTable = Table.SpawnPacket.MoneyType == MsgRouletteTable.TableType.ConquerPoints;
                        if (!moneyTable && !cpsTable)
                            return;
                        // Each bet must be positive and below the per-bet cap of the table currency.
                        long maxBet = moneyTable ? Conquer6609.Security.Guard.MaxMoneyAmount / 100 : Conquer6609.Security.Guard.MaxCpsAmount / 100;
                        long maxTotal = moneyTable ? Conquer6609.Security.Guard.MaxMoneyAmount : Conquer6609.Security.Guard.MaxCpsAmount;

                        // All bets are read and validated before anything is charged.
                        var numbers = new Dictionary<byte, Item>();
                        var extras = new Dictionary<byte, Item>();
                        long total = 0;
                        for (int x = 0; x < Count; x++)
                        {
                            var element = new Item();
                            element.Number = stream.ReadUInt8();
                            element.BetPrice = stream.ReadUInt32();
                            if (element.BetPrice == 0 || element.BetPrice > maxBet)
                                return;
                            if (element.Number <= 37)
                            {
                                if (numbers.ContainsKey(element.Number))
                                    return;
                                numbers.Add(element.Number, element);
                            }
                            else
                            {
                                if (!IsExtraBet(element.Number) || extras.ContainsKey(element.Number))
                                    return;
                                extras.Add(element.Number, element);
                            }
                            total += element.BetPrice;
                        }
                        if (total <= 0 || total > maxTotal)
                            return;

                        // The total bet must be covered by the current balance; it is charged once.
                        if (moneyTable)
                        {
                            if (player.Owner.Player.Money < total)
                                return;
                            player.Owner.Player.Money -= total;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream2 = rec.GetStream();
                                player.Owner.Player.SendUpdate(stream2, player.Owner.Player.Money, MsgUpdate.DataType.Money, false);
                            }
                        }
                        else
                        {
                            if ((long)player.Owner.Player.ConquerPoints < total)
                                return;
                            player.Owner.Player.ConquerPoints -= (uint)total;
                        }
                        foreach (var element in numbers.Values)
                            player.MyLuckNumber.TryAdd(element.Number, element);
                        foreach (var element in extras.Values)
                            player.MyLuckExtra.TryAdd(element.Number, element);

                        player.ShareBetting(Table);
                    }
                }
            }
        }
    }
}
