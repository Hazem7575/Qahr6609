using Conquer6609.Client;
using Conquer6609.Game.MsgMonster;
using Conquer6609.Game.MsgServer;
using Conquer6609.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Conquer6609
{

    public class DropMob
    {
        public unsafe static bool Checkup(ServerSockets.Packet stream, GameClient killer, MonsterRole Mob)
        {

            ushort xx = Mob.X;
            ushort yy = Mob.Y;
            #region Items
            if (Role.Core.Rate(1))
            {

                Database.ItemType.DBItem DbItem = null;
                byte ID_Quality;
                bool ID_Special;
                uint ID = GenerateItemId(Mob, Mob.Map, out ID_Quality, out ID_Special, out DbItem);
                if (killer.Map.AddGroundItem(ref xx, ref yy))
                {
                    DropItem(stream, Mob, killer.Player.UID, killer.Map, ID, xx, yy, Game.MsgFloorItem.MsgItem.ItemType.Item, 0, ID_Special, ID_Quality, killer, DbItem);
                }
                return true;
            }
            #endregion

            #region DragonBall
            if (Role.Core.Rate(0.001))
            {
                uint ID = 1088000;
                if (killer.Player.VipLevel == 6 )
                {
                    if (killer.Inventory.HaveSpace(1))
                    {
                        killer.Inventory.AddItemWitchStack(ID, 0, 1, stream);
                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel7");//ssch_wlhd_hit
                        if (killer.Inventory.Contain(1088000, 10) && killer.Player.VipLevel == 6)
                        {
                            killer.Inventory.Remove(1088000, 10, stream);
                            killer.Inventory.Add(stream, 720028, 1);
                            killer.SendSysMesage("[VIP-6] DBScroll got autopacked.", MsgMessage.ChatMode.TopLeft);
                            return true;
                        }
                        killer.SendSysMesage("[VIP-6] You got a DragonBall in your inventory.", MsgMessage.ChatMode.TopLeft);
                        return true;
                    }
                }
                else
                {
                    killer.Player.AddMapEffect(stream, xx, yy, "ssch_wlhd_hit");
                    Mob.DropItemID(killer, ID, stream, 3);
                }
                return true;
            }
            #endregion

            #region Meteros

            if (Role.Core.Rate(0.01))
            {
                uint ID = 1088001;
                if (killer.Player.VipLevel == 6 )
                {
                    if (killer.Inventory.HaveSpace(1))
                    {
                        killer.Inventory.AddItemWitchStack(ID, 0, 1, stream);
                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel7");
                        if (killer.Inventory.Contain(1088001, 10) && killer.Player.VipLevel == 6)
                        {
                            killer.Inventory.Remove(1088001, 10, stream);
                            killer.Inventory.Add(stream, 720027, 1);
                            killer.SendSysMesage("[VIP-6] Meteors Scrol got autopacked.", MsgMessage.ChatMode.TopLeft);
                            return true;
                        }
                        killer.SendSysMesage("[VIP-6] You got a Meteors in your inventory.", MsgMessage.ChatMode.TopLeft);
                        return true;
                    }
                }
                else
                    Mob.DropItemID(killer, ID, stream, 3);
                return true;
            }
            #endregion

            #region Stone

            if (Role.Core.Rate(0.01))
            {
                uint IDD = StoneId(1);
                if ( killer.Player.VipLevel == 6)
                {
                    if (killer.Inventory.HaveSpace(1))
                    {
                        killer.Inventory.Add(stream, IDD, 1);
                        killer.SendSysMesage("[AutoHunt] +1 Stone got autopacked.", MsgMessage.ChatMode.Talk);

                    }

                }

                else if (killer.Map.AddGroundItem(ref xx, ref yy))
                {
                    Mob.DropItem(stream, killer.Player.UID, killer.Map, IDD, xx, yy,
                        Game.MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                    killer.SendSysMesage("A Stone(+1) dropped at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.Talk);
                    return true;
                }
            }
            #endregion

            #region ExpBall
            if (Role.Core.Rate(0.01))
            {
                if (killer.Player.VipLevel >= 6 && killer.Inventory.HaveSpace(1) )
                {
                    killer.Inventory.AddItemWitchStack(722136, 0, 1, stream);
                    if (killer.Inventory.Contain(722136, 30) && killer.Player.VipLevel == 6)
                    {
                        killer.Inventory.Remove(722136, 30, stream);
                        killer.Inventory.Add(stream, 720757, 1);
                        killer.SendSysMesage("[VIP-6] Exp balls got AutoPacked.", MsgMessage.ChatMode.TopLeft);
                    }
                    killer.SendSysMesage("You got an EXP Ball in your inventory.", MsgMessage.ChatMode.TopLeft);
                }
                else
                    Mob.DropItemID(killer, 722136, stream);
            }
            #endregion
            #region MaterialItems
            if (Role.Core.Rate(0.1))
            {

                if (killer.Player.VipLevel >= 6 && killer.Inventory.HaveSpace(1) )
                {
                    killer.Inventory.AddItemWitchStack(720549, 0, 1, stream);
                    killer.SendSysMesage("You got A Refinary pack has been autolooted in your inventory.", MsgMessage.ChatMode.TopLeft);
                }
                else
                    Mob.DropItemID(killer, 720549, stream);
            }
            #endregion

            return false;
        }
        public static uint StoneId(UInt16 Plus)
        {
            switch (Plus)
            {
                case 1: return 730001;
                case 2: return 730002;
                case 3: return 730003;
                case 4: return 730004;
                case 5: return 730005;
                case 6: return 730006;
                case 7: return 730007;
                case 8: return 730008;
            }
            return 0;
        }
        public static void DropItem(ServerSockets.Packet stream, MonsterRole Mob, uint OwnerItem, Role.GameMap map, uint ItemID, ushort XX, ushort YY, Game.MsgFloorItem.MsgItem.ItemType typ
             , uint amount, bool special, byte ID_Quality, Client.GameClient user = null, Database.ItemType.DBItem DBItem = null)
        {
            try
            {
                if (DBItem == null) return;

                Game.MsgServer.MsgGameItem DataItem = new Game.MsgServer.MsgGameItem();

                DataItem.ITEM_ID = ItemID;
                if (DataItem.Durability > 100)
                {
                    DataItem.Durability = (ushort)Program.GetRandom.Next(100, DataItem.Durability / 10);
                    DataItem.MaximDurability = DataItem.Durability;
                }

                else
                {
                    DataItem.Durability = (ushort)Program.GetRandom.Next(1, 10);
                    DataItem.MaximDurability = 10;
                }
                DataItem.Color = Role.Flags.Color.Red;
                if (typ == Game.MsgFloorItem.MsgItem.ItemType.Item)
                {
                    if (DataItem.IsEquip)
                    {
                        if (!special)
                        {
                            if (ID_Quality == 3)
                            {
                                if (Core.Rate(50))
                                {
                                    DataItem.Plus = 2;
                                }
                                else if (Core.Rate(60))
                                {
                                    DataItem.Bless = 1;
                                }
                                else if (Core.Rate(20))
                                {
                                    DataItem.SocketOne = Role.Flags.Gem.EmptySocket;

                                }
                                else if (Core.Rate(10))
                                {
                                    DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                    DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                }
                            }
                            if (DBItem != null)
                            {
                                DataItem.Durability = (ushort)Program.GetRandom.Next(1, DBItem.Durability / 10 + 10);
                                DataItem.MaximDurability = (ushort)Program.GetRandom.Next(DataItem.Durability, DBItem.Durability);
                            }
                        }
                    }
                    else
                    {
                        if (DBItem != null)
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                    }
                    if (user.Player.VipLevel == 6)
                    {
                        if (user.AutoHunting.BlessedItems)
                        {
                            if (DataItem.Bless > 0)
                            {
                                if (user.Inventory.HaveSpace(1))
                                {
                                    user.Inventory.Add(DataItem, DBItem, stream);
                                    user.SendSysMesage("[AutoPacked] " + DBItem.Name + "[-" + DataItem.Bless + " Blessed] got AutoPacked.", MsgMessage.ChatMode.Talk);
                                    return;
                                }
                            }
                        }
                        if (user.AutoHunting.PlusItems)
                        {
                            if (DataItem.Plus > 0)
                            {
                                if (user.Inventory.HaveSpace(1))
                                {
                                    user.Inventory.Add(DataItem, DBItem, stream);
                                    user.SendSysMesage("[AutoPacked] " + DBItem.Name + "[+" + DataItem.Plus + "] got AutoPacked.", MsgMessage.ChatMode.Talk);
                                    return;
                                }
                            }
                        }
                        if (user.AutoHunting.SocketedItems)
                        {
                            if (DataItem.SocketTwo > 0)
                            {
                                if (user.Inventory.HaveSpace(1))
                                {
                                    user.Inventory.Add(DataItem, DBItem, stream);
                                    user.SendSysMesage("[AutoPacked] " + DBItem.Name + "[2soc] got AutoPacked.", MsgMessage.ChatMode.Talk);
                                    return;
                                }
                            }
                        }
                        if (user.AutoHunting.QualityItems)
                        {
                            if (ID_Quality == 9)
                            {
                                if (user.Inventory.HaveSpace(1))
                                {
                                    user.Inventory.Add(DataItem, DBItem, stream);
                                    user.SendSysMesage("[AutoPacked] " + DBItem.Name + "[Super] got AutoPacked.", MsgMessage.ChatMode.Talk);
                                    return;
                                }
                            }
                        }

                    }
                }
                Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(DataItem, XX, YY, typ, amount, Mob.DynamicID, Mob.Map, OwnerItem, true, map);

                if (map.EnqueueItem(DropItem))
                {
                    DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Visible);
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }

        public static uint GenerateItemId(MonsterRole Mob, uint map, out byte dwItemQuality, out bool Special, out Database.ItemType.DBItem DbItem)
        {
            Special = false;
            if (Role.Core.Rate(30))
            {
                dwItemQuality = Mob.Family.ItemGenerator.GenerateQuality();
            }
            else dwItemQuality = 3;//super
            uint dwItemSort = 0;
            uint dwItemLev = 0;
            int nRand = Extensions.BaseFunc.RandGet(1000, false);
            if (nRand >= 0 && nRand < 100) // 0.17%
            {
                dwItemSort = 160;
                dwItemLev = Mob.Family.DropBoots;
            }
            else if (nRand >= 100 && nRand < 200) // 0.25%
            {
                dwItemSort = MobItemGenerator.NecklaceType[Extensions.BaseFunc.RandGet(MobItemGenerator.NecklaceType.Length, false)];
                dwItemLev = Mob.Family.DropNecklace;
            }
            else if (nRand >= 200 && nRand < 300) // 4.17%
            {
                dwItemSort = MobItemGenerator.RingType[Extensions.BaseFunc.RandGet(MobItemGenerator.RingType.Length, false)];
                dwItemLev = Mob.Family.DropRing;
            }
            else if (nRand >= 300 && nRand < 400) // 25%
            {
                dwItemSort = MobItemGenerator.ArmetType[Extensions.BaseFunc.RandGet(MobItemGenerator.ArmetType.Length, false)];
                dwItemLev = Mob.Family.DropArmet;
            }
            else if (nRand >= 400 && nRand < 600) // 25%
            {
                dwItemSort = MobItemGenerator.ArmorType[Extensions.BaseFunc.RandGet(MobItemGenerator.ArmorType.Length, false)];
                dwItemLev = Mob.Family.DropArmor;
            }
            else // 45%
            {
                int nRate = Extensions.BaseFunc.RandGet(100, false);
                if (nRate >= 500 && nRate < 550) // 20% of 45% (= 9%) - Backswords
                {
                    dwItemSort = 421;
                }
                else if (nRate >= 600 && nRate < 700)	// 40% of 45% (= 18%) - One handers
                {
                    dwItemSort = MobItemGenerator.OneHanderType[Extensions.BaseFunc.RandGet(MobItemGenerator.OneHanderType.Length, false)];
                    dwItemLev = Mob.Family.DropWeapon;
                }
                else if (nRand >= 700 && nRand < 800)// 20% of 45% (= 9%) - Two handers (and shield)
                {
                    dwItemSort = MobItemGenerator.TwoHanderType[Extensions.BaseFunc.RandGet(MobItemGenerator.TwoHanderType.Length, false)];
                    dwItemLev = ((dwItemSort == 900) ? Mob.Family.DropShield : Mob.Family.DropWeapon);
                }
            }
            //if (dwItemLev != 99)
            {
                dwItemLev = Mob.Family.ItemGenerator.AlterItemLevel(dwItemLev, dwItemSort);
                uint idItemType = (dwItemSort * 1000) + (dwItemLev * 10) + dwItemQuality;
                // Database.ItemType.DBItem DbItem;
                if (Database.Server.ItemsBase.TryGetValue(idItemType, out DbItem))
                {
                    ushort position = Database.ItemType.ItemPosition(idItemType);
                    byte level = Database.ItemType.ItemMaxLevel((Role.Flags.ConquerItem)position);
                    if (DbItem.Level > level)
                        return 0;
                    return idItemType;
                }
            }
            DbItem = null;
            return 0;
        }

    }
}
