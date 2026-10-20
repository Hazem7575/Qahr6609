using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgItemUsuagePacket
    {

        public unsafe static void AlternanteEquipItem(Client.GameClient client, ServerSockets.Packet stream, uint id, uint dwParam1, uint timestamp, uint dwParam2, uint dwParam3, uint dwparam4)
        {

            uint Position = dwParam1;
            Position -= 20;
            if (client.Player.ContainFlag(MsgUpdate.Flags.Fly))
                client.Player.RemoveFlag(MsgUpdate.Flags.Fly);
            if (Database.AtributesStatus.IsArcher(client.Player.Class))
            {
                client.OnAutoAttack = false;
            }
            Game.MsgServer.MsgGameItem item = null;
            if (client.Inventory.TryGetItem(id, out item))
            {
                if (Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Garment)
                {
                    if (client.Player.SpecialGarment != 0)
                    {
                        client.SendSysMesage("Item can't be unequiped during the event.");

                        return;
                    }
                }
                if (Database.ItemType.IsPistol(item.ITEM_ID))
                {
                    if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.RightWeapon))
                        return;
                    Game.MsgServer.MsgGameItem rapier;
                    if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out rapier))
                    {
                        if (!Database.ItemType.IsRapier(rapier.ITEM_ID))
                            return;
                    }
                    else
                        return;
                }
                if (Position == (ushort)Role.Flags.ConquerItem.LeftWeapon)
                {
                    if (Database.ItemType.IsBacksword(client.Equipment.RightWeapon))
                    {
                        if (Database.ItemType.IsShield(item.ITEM_ID))
                        {
                            client.SendSysMesage("Remove the right-hand IsBacksword.", MsgMessage.ChatMode.System);
                            return;
                        }
                    }
                }
                if (Position == (ushort)Role.Flags.ConquerItem.LeftWeapon)
                {
                    if (Database.ItemType.IsKnife(client.Equipment.RightWeapon))
                    {
                        if (Database.ItemType.IsKnife(item.ITEM_ID) == false)
                        {
                            client.SendSysMesage("Remove the right-hand Knife.", MsgMessage.ChatMode.System);
                            return;
                        }
                    }
                }
                if (Position == (ushort)Role.Flags.ConquerItem.RightWeapon)
                {
                    if (Database.ItemType.IsKnife(client.Equipment.LeftWeapon))
                    {
                        if (Database.ItemType.IsKnife(item.ITEM_ID) == false)
                        {
                            client.SendSysMesage("Remove the left-hand Knife.", MsgMessage.ChatMode.System);
                            return;
                        }
                    }
                    if (Database.ItemType.IsShield(client.Equipment.LeftWeapon))
                    {
                        if (Database.ItemType.IsKnife(item.ITEM_ID))
                        {
                            client.SendSysMesage("Remove the left-hand Shield.", MsgMessage.ChatMode.System);
                            return;
                        }
                    }
                }
                bool can2hand = false;
                bool can2wpn = false;
                if (client.Player.Class >= 11 && client.Player.Class <= 85)
                    can2hand = true;
                if (client.Player.Class >= 11 && client.Player.Class <= 15 || client.Player.Class >= 20 && client.Player.Class <= 25 || client.Player.Class >= 51 && client.Player.Class <= 85)
                    can2wpn = true;

                if (client.Player.Class >= 40 && client.Player.Class <= 45)
                {
                    if (Database.ItemType.IsTwoHand(item.ITEM_ID) && client.Player.IsAsasin)
                        return;
                    //  if (Database.ItemType.IsKnife(item.ITEM_ID))
                    can2wpn = true;
                }
                if (!Database.ItemType.Equipable(item, client))
                    return;
                ushort po = Database.ItemType.ItemPosition(item.ITEM_ID);
                if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                {
                    if (po == (ushort)Role.Flags.ConquerItem.LeftWeapon)
                    {
                        if (Database.ItemType.IsHossu(item.ITEM_ID) == false)
                            return;
                    }
                }
                if (po == (ushort)Role.Flags.ConquerItem.Garment || po == (ushort)Role.Flags.ConquerItem.SteedMount)
                    return;
                if (po == 5)
                {
                    Position = 5;
                    if (!can2hand && !can2wpn)
                        return;
                    if (!client.Equipment.FreeEquip(Role.Flags.ConquerItem.AleternanteRightWeapon))
                    {
                        Game.MsgServer.MsgGameItem RightWeapon;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.AleternanteRightWeapon, out RightWeapon))
                        {
                            if (RightWeapon.ITEM_ID / 1000 != 500 && Database.ItemType.IsArrow(item.ITEM_ID))
                                return;
                        }
                    }
                    else
                        return;
                }
                else if (po == 0) return;
                if (Database.ItemType.ItemPosition(item.ITEM_ID) == 4)
                {
                    if (Position == 5)
                        if (!can2hand || !can2wpn)
                            return;
                }
                if (!((Position == 4 || Position == 5) && (Database.ItemType.ItemPosition(item.ITEM_ID) == 4 || Database.ItemType.ItemPosition(item.ITEM_ID) == 5)))
                {
                    if (!Database.ItemType.IsAccessory(item.ITEM_ID))
                        Position = Database.ItemType.ItemPosition(item.ITEM_ID);
                }
                bool twohand = Database.ItemType.IsTwoHand(item.ITEM_ID);
                if (!twohand && Position == 4)
                {
                    Game.MsgServer.MsgGameItem LeftWeapon;
                    if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.AleternanteLeftWeapon, out LeftWeapon))
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (Database.ItemType.IsArrow(LeftWeapon.ITEM_ID))
                                client.Equipment.Remove(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                            else
                            {
                                Game.MsgServer.MsgGameItem RightWeapon;
                                if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.AleternanteRightWeapon, out RightWeapon))
                                {
                                    if (Database.ItemType.IsTwoHand(RightWeapon.ITEM_ID))
                                        client.Equipment.Remove(Role.Flags.ConquerItem.AleternanteRightWeapon, stream);
                                }
                            }
                        }
                    }
                }
                Position += 20;
                if (client.Equipment.FreeEquip((Role.Flags.ConquerItem)Position))
                {
                    bool spaceinventory = true;
                    if (twohand)
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                        }
                        else
                        {
                            spaceinventory = false;
                        }
                    }
                    if (spaceinventory)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        item.Position = (ushort)Position;
                        client.Equipment.Add(item, stream);
                        item.Mode = Role.Flags.ItemMode.Update;
                        item.Send(client, stream);
                    }
                }
                else//if is item on character
                {
                    bool spaceinventory = true;
                    if (twohand)
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                        }
                        else
                            spaceinventory = false;
                    }
                    if (spaceinventory)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        item.Position = (ushort)Position;
                        item.Mode = Role.Flags.ItemMode.AddItem;
                        //item.Send(client);
                        client.Equipment.Remove((Role.Flags.ConquerItem)Position, stream);
                        client.Equipment.Add(item, stream);
                    }
                }

                Game.MsgServer.MsgShowEquipment equip = new MsgShowEquipment();
                equip.wParam = item.UID;
                equip.Alternante = item.Position;
                equip.UID = 5;
                client.Send(stream.ShowEquipmentCreate(equip));
                if (client.Equipment.Alternante)
                {
                    client.Equipment.QueryEquipment(true);
                }
                else
                    client.Equipment.SendAlowAlternante(stream);
            }
        }
        public unsafe static void EquipItem(Client.GameClient client, ServerSockets.Packet stream, uint id, uint dwParam1, uint timestamp, uint dwParam2, uint dwParam3, uint dwparam4)
        {
            uint Position = dwParam1;
            if (Database.AtributesStatus.IsArcher(client.Player.Class))
            {
                client.OnAutoAttack = false;
            }
            Game.MsgServer.MsgGameItem item = null;
            if (client.Inventory.TryGetItem(id, out item))
            {
              
                if (client.Player.Mining)
                    client.Player.StopMining();
                switch (item.ITEM_ID)
                {
                    case 1200000:
                    case 1200001:
                    case 1200002:
                        {
                            UseItem(item, client, stream);
                            return;
                        }
                    default:
                        {
                            if (Position == 17 && Database.ItemType.ItemPosition(item.ITEM_ID) == 0)
                            {
                                UseItem(item, client, stream);
                                return;
                            }
                            else if (Position == 0 && Database.ItemType.ItemPosition(item.ITEM_ID) == 0)
                            {
                                UseItem(item, client, stream);
                                return;
                            }
                            break;
                        }
                }

                if (client.Player.ContainFlag(MsgUpdate.Flags.PathOfShadow))
                {
                    client.SendSysMesage("Please remove you PathOfShadow.");

                    return;
                }
                if (Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Garment)
                {
                    if (client.Player.SpecialGarment != 0)
                    {
                        client.SendSysMesage("Item can't be unequiped during the event.");

                        return;
                    }
                }

                if (Database.ItemType.IsPistol(item.ITEM_ID))
                {
                    if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.RightWeapon))
                        return;
                    Game.MsgServer.MsgGameItem rapier;
                    if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out rapier))
                    {
                        if (!Database.ItemType.IsRapier(rapier.ITEM_ID))
                            return;
                    }
                    else
                        return;
                }


                bool can2hand = false;
                bool can2wpn = false;
                if (client.Player.Class >= 11 && client.Player.Class <= 85 || client.Player.Class >= 160 && client.Player.Class <= 165)
                    can2hand = true;
                if (client.Player.Class >= 11 && client.Player.Class <= 15 || client.Player.Class >= 20 && client.Player.Class <= 25 || client.Player.Class >= 51 && client.Player.Class <= 145
                    || client.Player.Class >= 160 && client.Player.Class <= 165)
                    can2wpn = true;

                if (client.Player.Class >= 40 && client.Player.Class <= 45)
                {
                    if (Database.ItemType.IsTwoHand(item.ITEM_ID) && client.Player.IsAsasin)
                        return;
                    // if (Database.ItemType.IsKnife(item.ITEM_ID))
                    can2wpn = true;
                }
                if (!Database.ItemType.Equipable(item, client))
                    return;
                ushort po = Database.ItemType.ItemPosition(item.ITEM_ID);
                if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                {
                    if (po == (ushort)Role.Flags.ConquerItem.LeftWeapon)
                    {
                        if (Database.ItemType.IsHossu(item.ITEM_ID) == false)
                            return;
                    }
                }
                if (Position == 5)
                {
                    if (Database.ItemType.IsTwoHand(client.Equipment.RightWeapon))
                    {
                        if (Database.ItemType.IsShield(item.ITEM_ID) == false)
                            return;
                    }
                }
                if (Position == (ushort)Role.Flags.ConquerItem.LeftWeapon)
                {
                    if (Database.ItemType.IsBacksword(client.Equipment.RightWeapon))
                    {
                        if (Database.ItemType.IsShield(item.ITEM_ID))
                        {
                            client.SendSysMesage("Remove the right-hand IsBacksword.", MsgMessage.ChatMode.System);
                            return;
                        }
                    }
                }
                if (Position == (ushort)Role.Flags.ConquerItem.LeftWeapon)
                {
                    if (Database.ItemType.IsBacksword(client.Equipment.RightWeapon))
                    {
                        if (Database.ItemType.IsShield(item.ITEM_ID))
                        {
                            client.SendSysMesage("Remove the right-hand IsBacksword.", MsgMessage.ChatMode.System);
                            return;
                        }
                    }
                }
                if (Position == (ushort)Role.Flags.ConquerItem.LeftWeapon)
                {
                    if (Database.ItemType.IsKnife(client.Equipment.RightWeapon))
                    {
                        if (Database.ItemType.IsKnife(item.ITEM_ID) == false)
                        {
                            client.SendSysMesage("Remove the right-hand Knife.", MsgMessage.ChatMode.System);
                            return;
                        }
                    }
                }
                if (Position == (ushort)Role.Flags.ConquerItem.RightWeapon)
                {
                    if (Database.ItemType.IsKnife(client.Equipment.LeftWeapon))
                    {
                        if (Database.ItemType.IsKnife(item.ITEM_ID) == false)
                        {
                            client.SendSysMesage("Remove the left-hand Knife.", MsgMessage.ChatMode.System);
                            return;
                        }
                    }
                    if (Database.ItemType.IsShield(client.Equipment.LeftWeapon))
                    {
                        if (Database.ItemType.IsKnife(item.ITEM_ID))
                        {
                            client.SendSysMesage("Remove the left-hand Shield.", MsgMessage.ChatMode.System);
                            return;
                        }
                    }
                }

                if (po == (ushort)Role.Flags.ConquerItem.Garment || po == (ushort)Role.Flags.ConquerItem.SteedMount)
                    return;
                if (po == 5)
                {
                    Position = 5;
                    if (!can2hand && !can2wpn)
                        return;
                    if (!client.Equipment.FreeEquip(Role.Flags.ConquerItem.RightWeapon))
                    {
                        Game.MsgServer.MsgGameItem RightWeapon;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out RightWeapon))
                        {
                            if (RightWeapon.ITEM_ID / 1000 != 500 && Database.ItemType.IsArrow(item.ITEM_ID))
                                return;
                        }
                    }
                    else
                        return;
                }
                else if (po == 0) return;
                if (Database.ItemType.ItemPosition(item.ITEM_ID) == 4)
                {
                    if (Position == 5)
                        if (!can2hand || !can2wpn)
                            return;
                }
                if (!((Position == 4 || Position == 5) && (Database.ItemType.ItemPosition(item.ITEM_ID) == 4 || Database.ItemType.ItemPosition(item.ITEM_ID) == 5)))
                {
                    if (!Database.ItemType.IsAccessory(item.ITEM_ID))
                        Position = Database.ItemType.ItemPosition(item.ITEM_ID);
                }
                bool twohand = Database.ItemType.IsTwoHand(item.ITEM_ID);
                if (!twohand && Position == 4)
                {
                    Game.MsgServer.MsgGameItem LeftWeapon;
                    if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out LeftWeapon))
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (Database.ItemType.IsArrow(LeftWeapon.ITEM_ID))
                                client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                            else
                            {
                                Game.MsgServer.MsgGameItem RightWeapon;
                                if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out RightWeapon))
                                {
                                    if (Database.ItemType.IsTwoHand(RightWeapon.ITEM_ID))
                                        client.Equipment.Remove(Role.Flags.ConquerItem.RightWeapon, stream);
                                }
                            }
                        }
                    }
                }
                if (client.Equipment.FreeEquip((Role.Flags.ConquerItem)Position))
                {
                    bool spaceinventory = true;
                    if (twohand)
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                        }
                        else
                        {
                            spaceinventory = false;
                        }
                    }
                    if (spaceinventory)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        item.Position = (ushort)Position;
                        client.Equipment.Add(item, stream);
                        item.Mode = Role.Flags.ItemMode.Update;
                        item.Send(client, stream);
                    }
                }
                else//if is item on character
                {
                    bool spaceinventory = true;
                    if (twohand)
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                        }
                        else
                            spaceinventory = false;
                    }
                    if (spaceinventory)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        item.Position = (ushort)Position;
                        item.Mode = Role.Flags.ItemMode.AddItem;
                        //item.Send(client);
                        client.Equipment.Remove((Role.Flags.ConquerItem)Position, stream);
                        client.Equipment.Add(item, stream);
                    }
                }
                client.Equipment.QueryEquipment(client.Equipment.Alternante);
            }
        }
    }
}
