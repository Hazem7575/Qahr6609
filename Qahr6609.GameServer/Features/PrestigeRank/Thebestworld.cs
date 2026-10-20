using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgBestOfTheWorld
    {
        [Flags]
        public enum Mode : uint
        {
            Show = 0,
            Viewer = 1
        }
        [ProtoContract]
        public class BestOfTheWorld
        {
            [ProtoMember(1, IsRequired = true)]
            public Mode Type;
            [ProtoMember(2, IsRequired = true)]
            public Item[] Items;

            [ProtoContract]
            public class Item
            {
                [ProtoMember(1, IsRequired = true)]
                public uint Type;//1
                [ProtoMember(2, IsRequired = true)]
                public uint Rank;//1
                [ProtoMember(3, IsRequired = true)]
                public uint EntityUID;
                [ProtoMember(4, IsRequired = true)]
                public string Name = "";
                [ProtoMember(5, IsRequired = true)]
                public string GuildName = "";
                [ProtoMember(6, IsRequired = true)]
                public uint Mesh;
                [ProtoMember(7, IsRequired = true)]
                public uint HairStyle;
                [ProtoMember(8, IsRequired = true)]
                public uint Head;
                [ProtoMember(9, IsRequired = true)]
                public uint Garment;
                [ProtoMember(10, IsRequired = true)]
                public uint LeftWeapon;
                [ProtoMember(11, IsRequired = true)]
                public uint LefttWeaponAccessory;
                [ProtoMember(12, IsRequired = true)]
                public uint RightWeapon;
                [ProtoMember(13, IsRequired = true)]
                public uint RightWeaponAccessory;
                [ProtoMember(14, IsRequired = true)]
                public uint MountArmor;
                [ProtoMember(15, IsRequired = true)]
                public uint Armor;//???
                [ProtoMember(16, IsRequired = true)]
                public uint Wing;
                [ProtoMember(17, IsRequired = true)]
                public uint WingPlus;
                [ProtoMember(18, IsRequired = true)]
                public uint Title;
                [ProtoMember(19, IsRequired = true)]
                public uint Flag;
            }
        }
        public static unsafe ServerSockets.Packet CreateBestOfTheWorld(this ServerSockets.Packet stream, BestOfTheWorld obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgBestOfTheWorld);

            return stream;
        }
        public static unsafe void GetBestOfTheWorld(this ServerSockets.Packet stream, out BestOfTheWorld pQuery)
        {
            pQuery = new BestOfTheWorld();
            pQuery = stream.ProtoBufferDeserialize<BestOfTheWorld>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgBestOfTheWorld)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {

            BestOfTheWorld pQuery;
            stream.GetBestOfTheWorld(out pQuery);
            switch (pQuery.Type)
            {
                case Mode.Viewer:
                    {
                        Client.GameClient Target;
                        if (Database.Server.GamePoll.TryGetValue(pQuery.Items[0].EntityUID, out Target))
                        {
                            if (Target.Equipment == null || Target.Equipment.CurentEquip == null) return;

                            client.Send(Target.Player.GetArray(stream, true));
                            foreach (var item in Target.Equipment.CurentEquip)
                            {
                                if (item != null)
                                {
                                    client.Send(stream.ItemViewCreate(Target.Player.UID, 0, item, MsgItemView.ActionMode.ViewEquip));
                                    item.SendItemExtra(client, stream);
                                }
                            }
                            MsgUserAbilityScore.UserAbilityScore info = new MsgUserAbilityScore.UserAbilityScore();
                            info.type = 1;
                            info.Level = Target.Player.Level;
                            info.UID = Target.Player.UID;
                            info.Items = new MsgUserAbilityScore.AbilityScore[19];
                            for (int x = 0; x < 19; x++)
                            {
                                info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                                info.Items[x].Position = (uint)(x + 1);
                                info.Items[x].Points = Target.PrestigePoints[x];
                            }
                            client.Send(stream.UserAbilityScoreCreate(info));
                        }
                        else
                        {

                            client.Send(stream.CreateBestOfTheWorld(new BestOfTheWorld()
                            {
                                Type = Mode.Viewer,
                                Items = new BestOfTheWorld.Item[1]
                            }));

                        }
                        break;
                    }
                case Mode.Show:
                    {
                        var BestOf = Database.PrestigeRanking.BestOfTheWorld;
                        if (BestOf != null)
                        {
                            uint Garment = ((int)BestOf.Garment == 0 ? BestOf.Armor : BestOf.Garment);
                            client.Send(stream.CreateBestOfTheWorld(new BestOfTheWorld()
                            {
                                Type = Mode.Show,

                                Items = new BestOfTheWorld.Item[1]
                             {
                                  new BestOfTheWorld.Item()
                                  {
                                        EntityUID = BestOf.UID,
                                        Flag = BestOf.Flag,
                                        GuildName = BestOf.GuildName,
                                        HairStyle = BestOf.HairStyle,
                                        Head = BestOf.Head,
                                        LefttWeaponAccessory =BestOf.LefttWeaponAccessory,
                                        LeftWeapon =BestOf.LeftWeapon,
                                        Mesh =BestOf.Mesh,
                                        MountArmor =BestOf.MountArmor,
                                        Name = BestOf.Name,
                                        Rank = BestOf.Rank,
                                        RightWeapon =BestOf.RightWeapon,
                                        RightWeaponAccessory =BestOf.RightWeaponAccessory,
                                        Title = BestOf.Title,
                                        Type = 1,
                                        Garment = Garment,
                                        Wing = BestOf.Wing,
                                        WingPlus = BestOf.WingPlus
                                        
                                  }
                             }
                            }));
                        }
                        break;
                    }
            }
        }
    }
}
