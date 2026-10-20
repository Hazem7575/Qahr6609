using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetGuildFastArsenal(this ServerSockets.Packet stream, out List<uint> Items)
        {
            Items = new List<uint>();
            int size = stream.ReadUInt8();
            for (int x = 0; x < size; x++)
            {
                Items.Add(stream.ReadUInt32());
            }
        }
    }
    public struct MsgGuildFastArsenal
    {


        [PacketAttribute(GamePackets.FastArsenal)]
        public unsafe static void FastArsenal(Client.GameClient user, ServerSockets.Packet stream)
        {

            List<uint> Items;

            stream.GetGuildFastArsenal(out Items);

            byte ItemsCount = stream.ReadUInt8();

            foreach (uint ItemUID in Items)
            {

                if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                    break;


                MsgGameItem item = null;
                if (user.Inventory.TryGetItem(ItemUID, out item))
                {
                    byte ArsenalTyp = Role.Instance.Guild.Arsenal.GetArsenalPosition(item.ITEM_ID);
                    if (ArsenalTyp == 0)
                        ArsenalTyp = 8;

                    if (item.Inscribed != 0)
                        break;
                    if (user.Player.MyGuild.MyArsenal.Add(ArsenalTyp, new Role.Instance.Guild.Arsenal.InscribeItem() { BaseItem = item, Name = user.Player.Name, UID = user.Player.UID }))
                    {
                        item.Inscribed = 1;
                        item.Mode = Role.Flags.ItemMode.Update;
                        item.Send(user, stream);
                        user.Player.MyGuildMember.ArsenalDonation += GetItemDonation(item);
                        user.Player.GuildBattlePower = user.Player.MyGuild.ShareMemberPotency(user.Player.MyGuildMember.Rank);
                    }
                }
                else if (user.Equipment.TryGetValue(ItemUID, out item))
                {
                    byte ArsenalTyp = Role.Instance.Guild.Arsenal.GetArsenalPosition(item.ITEM_ID);
                    if (ArsenalTyp == 0)
                        ArsenalTyp = 8;

                    if (item.Inscribed != 0)
                        break;
                    if (user.Player.MyGuild.MyArsenal.Add(ArsenalTyp, new Role.Instance.Guild.Arsenal.InscribeItem() { BaseItem = item, Name = user.Player.Name, UID = user.Player.UID }))
                    {
                        item.Inscribed = 1;
                        item.Mode = Role.Flags.ItemMode.Update;
                        item.Send(user, stream);
                        user.Player.MyGuildMember.ArsenalDonation += GetItemDonation(item);
                        user.Player.GuildBattlePower = user.Player.MyGuild.ShareMemberPotency(user.Player.MyGuildMember.Rank);
                    }
                }
            }
        }
        private static uint GetItemDonation(MsgGameItem GameItem)
        {
            uint Return = 0;
            int id = (int)(GameItem.ITEM_ID % 10);
            switch (id)
            {
                case 8: Return = 1000; break;
                case 9: Return = 16660; break;
            }
            if (GameItem.SocketOne > 0 && GameItem.SocketTwo == 0)
                Return += 33330;
            if (GameItem.SocketOne > 0 && GameItem.SocketTwo > 0)
                Return += 133330;

            switch (GameItem.Plus)
            {
                case 1: Return += 90; break;
                case 2: Return += 490; break;
                case 3: Return += 1350; break;
                case 4: Return += 4070; break;
                case 5: Return += 12340; break;
                case 6: Return += 37030; break;
                case 7: Return += 111110; break;
                case 8: Return += 333330; break;
                case 9: Return += 1000000; break;
                case 10: Return += 1033330; break;
                case 11: Return += 1101230; break;
                case 12: Return += 1212340; break;
                default: break;
            }

            return Return;
        }
    }
}
