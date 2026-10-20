using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;
using Conquer6609.Database;

namespace Conquer6609.Role
{
    public unsafe partial class Player
    {

        public void SwitchWingWalkerAttack(ServerSockets.Packet stream)
        {
            if ((MainFlag & MainFlagType.OnMeleeAttack) == MainFlagType.OnMeleeAttack)
            {
                MainFlag &= ~MainFlagType.OnMeleeAttack;


                SendUpdate(stream, (uint)MainFlag, MsgUpdate.DataType.MainFlag);
                Owner.CreateBoxDialog("You have successfully changed your branch to Chaser(Ranged).");


            }
            else
            {
                MainFlag |= MainFlagType.OnMeleeAttack;
                SendUpdate(stream, (uint)MainFlag, MsgUpdate.DataType.MainFlag);
                Owner.CreateBoxDialog("You have successfully changed your branch to Stomper(Melee).");
            }
            this.Owner.Socket.Disconnect();
        }

        public uint GetTitlesScore()
        {
            uint score = 500;
            return score;
        }
        public void AddSpecialTitle(MsgTitleStorage.TitleType type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {
                if (!SpecialTitles.Contains(type))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetTitlesScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;
                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                    SpecialTitles.Add(type);
                }
            }
        }
        public void SendSpecialTitle(ServerSockets.Packet stream)
        {
            foreach (var _title in SpecialTitles)
            {
                Database.TitleStorage dbtitle;
                if (Database.TitleStorage.Titles.TryGetValue((uint)_title, out dbtitle))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetTitlesScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;
                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                    // SpecialTitles.Add(_title);
                }
            }
        }
        public void ActiveSpecialTitles(ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)(SpecialTitleID / 10000), out dbtitle))
            {
                MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                title.ActionID = MsgTitleStorage.Action.UpdateScore;
                title.dwparam1 = GetTitlesScore();
                title.dwparam2 = dbtitle.ID;
                title.dwparam3 = dbtitle.SubID;
                title.Title = new MsgTitleStorage.Title();
                title.Title.ID = dbtitle.ID;
                title.Title.SubId = dbtitle.SubID;
                title.Title.dwparam1 = 100;
                Owner.Send(stream.CreateTitleStorage(title));
            }
            if (Database.TitleStorage.Titles.TryGetValue((uint)(SpecialWingID / 10000), out dbtitle))
            {
                MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                title.ActionID = MsgTitleStorage.Action.UpdateScore;
                title.dwparam1 = GetTitlesScore();
                title.dwparam2 = dbtitle.ID;
                title.dwparam3 = dbtitle.SubID;
                title.Title = new MsgTitleStorage.Title();
                title.Title.ID = dbtitle.ID;
                title.Title.SubId = dbtitle.SubID;
                title.Title.dwparam1 = 100;
                Owner.Send(stream.CreateTitleStorage(title));
            }
        }

        public void RemoveSpecialTitle(MsgTitleStorage.TitleType type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {
                if (SpecialTitles.Contains(type))
                    SpecialTitles.Remove(type);
                MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                title.ActionID = MsgTitleStorage.Action.UnEquip;
                title.dwparam2 = dbtitle.ID;
                title.dwparam3 = dbtitle.SubID;
                Owner.Send(stream.CreateTitleStorage(title));
                title.dwparam1 = 0;
                title.Title = new MsgTitleStorage.Title();
                title.Title.ID = dbtitle.ID;
                title.Title.SubId = dbtitle.SubID;
                title.Title.dwparam1 = 0;
                Owner.Send(stream.CreateTitleStorage(title));
                title.ActionID = MsgTitleStorage.Action.UpdateScore;
                Owner.Send(stream.CreateTitleStorage(title));
                title.ActionID = MsgTitleStorage.Action.RemoveTitle;
                Owner.Send(stream.CreateTitleStorage(title));

                if (SpecialTitleID / 10000 == (uint)type)
                    SpecialTitleID = SpecialTitleScore = 0;
                else if (SpecialWingID / 10000 == (uint)type)
                    SpecialWingID = 0;

            }

        }
        public void RemoveSpecialGarment(ServerSockets.Packet stream)
        {
            SpecialGarment = 0;
            GarmentId = 0;
            //   Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, uint.MaxValue - 1, (ushort)Flags.ConquerItem.Garment, 0, 0, 0, 0));

            MsgGameItem item;
            if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.Garment, out item))
            {
                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, item.UID, (ushort)Flags.ConquerItem.Garment, 0, 0, 0, 0));
                item.Mode = Flags.ItemMode.AddItem;
                item.Send(Owner, stream);
            }
            Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);
        }
        public void AddSpecialGarment(ServerSockets.Packet stream, uint ID)
        {
            /*  if (SpecialGarment == 0)
              {
                  MsgGameItem gitem;
                  if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.Garment, out gitem))
                  {
                      Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, gitem.UID, (ushort)Flags.ConquerItem.Garment, 0, 0, 0, 0));
                  }
              }
              else
              */
            //  Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, uint.MaxValue - 1, (ushort)Flags.ConquerItem.Garment, 0, 0, 0, 0));

            SpecialGarment = ID;
            GarmentId = SpecialGarment;


            Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, uint.MaxValue - 1, (ushort)Flags.ConquerItem.Garment, 0, 0, 0, 0));

            Game.MsgServer.MsgGameItem item = new MsgGameItem();
            item.ITEM_ID = ID;
            item.Mode = Flags.ItemMode.AddItem;
            item.UID = uint.MaxValue - 1;
            item.Color = Flags.Color.Red;
            item.Position = (ushort)Flags.ConquerItem.Garment;
            item.Durability = Database.Server.ItemsBase[ID].Durability;
            item.Send(Owner, stream);
            Owner.Equipment.AppendItems(true, Owner.Equipment.CurentEquip, stream);
            View.SendView(GetArray(stream, false), false);
        }

        public unsafe void AddTitle(byte _title, bool aSwitch)
        {
            if (!Titles.Contains(_title))
            {
                Titles.Add(_title);

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    Owner.Send(stream.TitleCreate(UID, _title, MsgTitle.QueueTitle.Enqueue));

                }

                if (aSwitch)
                    SwitchTitle(_title);
            }
        }
        public unsafe void SwitchTitle(byte ntitle)
        {
            if (Titles.Contains(ntitle) || ntitle == 0)
            {
                MyTitle = ntitle;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    Owner.Send(stream.TitleCreate(UID, ntitle, MsgTitle.QueueTitle.Change));

                }
            }
        }
    }

}
