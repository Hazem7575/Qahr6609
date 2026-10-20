using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetArsenalList(this ServerSockets.Packet stream, out uint Type
            , out uint BeginAt, out uint EndAt, out byte ArsenalTyp)
        {
            Type = stream.ReadUInt32();
            BeginAt = stream.ReadUInt32();
            EndAt = stream.ReadUInt32();
            ArsenalTyp = stream.ReadUInt8();
        }
    }
    public class MsgGuildArsenalList
    {
        public static byte GetBattlePowerItem(MsgGameItem item)
        {
            byte potBase = 0;
            byte Quality = (byte)(item.ITEM_ID % 10);
            if (Quality >= 5)
                potBase += (byte)(Quality - 5);
            potBase += item.Plus;
            if (item.SocketOne != Role.Flags.Gem.NoSocket) potBase++;
            if (item.SocketTwo != Role.Flags.Gem.NoSocket) potBase++;
            if (((byte)item.SocketOne) % 10 == 3) potBase++;
            if (((byte)item.SocketTwo) % 10 == 3) potBase++;

            return potBase;
        }

        [PacketAttribute(GamePackets.PageArsenal)]
        public unsafe static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                return;
            try
            {
                DateTime Now = new DateTime();
                Now = Now.AddDays(30);
                int ExpireEnchant = Now.Year * 10000 + Now.Month * 100 + Now.Day;
                uint Type;
                uint BeginAt;
                uint EndAt;
                byte ArsenalTyp;

                stream.GetArsenalList(out Type, out BeginAt, out EndAt, out ArsenalTyp);


                if (ArsenalTyp == 0)
                    ArsenalTyp = 8;
                var Arsenal = user.Player.MyGuild.MyArsenal.GetArsenal(ArsenalTyp);
                if (Arsenal == null)
                    return;
                Role.Instance.Guild.Arsenal.InscribeItem[] items = Arsenal.DescreasedItems;

                if (items == null || items.Length == 0)
                    return;
                uint Start_index = (BeginAt - 1);
                const int max = 8;
                uint count = (uint)Math.Min(max, items.Length - Start_index);
                if (items.Length > Start_index)
                {

                    stream.InitWriter();
                    stream.Write(Type);
                    stream.Write(BeginAt);
                    stream.Write((uint)(BeginAt + count));
                    stream.Write((uint)ArsenalTyp);
                    stream.Write((uint)items.Length);
                    stream.Write((uint)Arsenal.GetPotency);
                    stream.Write((uint)Arsenal.Enchant);
                    stream.Write(ExpireEnchant);
                    stream.Write((uint)Arsenal.GetDonation);
                    stream.Write((uint)count);

                    uint i = BeginAt;
                    for (uint x = 0; x < count; x++)
                    {
                        if (x >= items.Length)
                            break;
                        Role.Instance.Guild.Arsenal.InscribeItem client = items[x + Start_index];

                        stream.Write(client.BaseItem.UID);
                        stream.Write(i);
                        stream.Write(client.Name, 16);
                        stream.Write(client.BaseItem.ITEM_ID);
                        stream.Write((byte)(client.BaseItem.ITEM_ID % 10));
                        stream.Write(client.BaseItem.Plus);
                        stream.Write((byte)client.BaseItem.SocketOne);
                        stream.Write((byte)client.BaseItem.SocketTwo);
                        stream.Write((uint)GetBattlePowerItem(client.BaseItem));
                        stream.Write(Arsenal.GetItemDonation(client.BaseItem));
                        i++;
                    }
                    stream.Finalize(GamePackets.PageArsenal);
                    user.Send(stream);
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}
