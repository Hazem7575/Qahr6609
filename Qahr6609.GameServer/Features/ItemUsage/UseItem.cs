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

       
       
        public static void UseItem(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!client.Player.Alive)
                return;
            
            Database.ItemType.DBItem DBItem;
            if (Database.Server.ItemsBase.TryGetValue(item.ITEM_ID, out DBItem))
            {
                if (client.PipeServer != null)
                {
                    if (item.ITEM_ID >= 1000000 && item.ITEM_ID <= 1000040
                  || item.ITEM_ID >= 1002000 && item.ITEM_ID <= 1002030
                  || item.ITEM_ID == 1002050
                  || item.ITEM_ID == 725065 || item.ITEM_ID == 1003010 || item.ITEM_ID == 1003000 || item.ITEM_ID == 1003020 || item.ITEM_ID == 723993)
                    {
                        if (DateTime.Now > client.Player.MedicineStamp.AddMilliseconds(300))
                        {
                            if (client.Player.ContainFlag(MsgUpdate.Flags.PoisonStar) && item.ITEM_ID != 1060020 && item.ITEM_ID != 1060021 && item.ITEM_ID != 1060022 && item.ITEM_ID != 1060023 && item.ITEM_ID != 1060024)
                                return;
                            if (client.Player.HitPoints == client.Status.MaxHitpoints)
                                return;
                            client.Player.HitPoints = Math.Min(client.Player.HitPoints + DBItem.ItemHP, (int)client.Status.MaxHitpoints);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Player.MedicineStamp = DateTime.Now;
                        }
                    }
                    else if (item.ITEM_ID >= 1001000 && item.ITEM_ID <= 1001040
                        || item.ITEM_ID == 1002030 || item.ITEM_ID == 1002040
                        || item.ITEM_ID == 725066 || item.ITEM_ID == 1004010)
                    {
                        if (client.Player.ContainFlag(MsgUpdate.Flags.PoisonStar) && item.ITEM_ID != 1060020 && item.ITEM_ID != 1060021 && item.ITEM_ID != 1060022 && item.ITEM_ID != 1060023 && item.ITEM_ID != 1060024)
                            return;
                        if (client.Player.Mana == client.Status.MaxMana || !client.ManaPotionReady())
                            return;
                        client.Player.Mana = (ushort)Math.Min(client.Player.Mana + DBItem.ItemMP, (int)client.Status.MaxMana);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.ManaMedicineStamp = DateTime.Now;
                    }
                    return;
                }
                else if (client.IsConnectedInterServer())
                {
                    return;
                }
                if (Conquer6609.Framework.Modules.ModuleManager.ItemUse(client, item)) return;

                
                if (item.ITEM_ID == 720996)
                {
                    client.Player.ConquerPoints += 2;
                    client.Player.MessageBox("You~recieved~2~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 720997)
                {
                    client.Player.ConquerPoints += 5;
                    client.Player.MessageBox("You~recieved~5~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3200881)
                {
                    
                    foreach (var itemm in Database.TitleStorage.Titles.Values)
                        client.Player.AddSpecialTitle((Game.MsgServer.MsgTitleStorage.TitleType)itemm.ID, stream);
                    client.Player.MessageBox("You~Are Now Have All Titel And Wing.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 720998)
                {
                    client.Player.ConquerPoints += 8;
                    client.Player.MessageBox("You~recieved~8~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3300658)
                {
                    client.Player.ConquerPoints += 10;
                    client.Player.MessageBox("You~recieved~10~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3300659)
                {
                    client.Player.ConquerPoints += 20;
                    client.Player.MessageBox("You~recieved~20~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;

                }
                /////////////////////
                if (item.ITEM_ID == 3300663)
                {
                    client.Player.ConquerPoints += 500;
                    client.Player.MessageBox("You~recieved~500~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3300664)
                {
                    client.Player.ConquerPoints += 1000;
                    client.Player.MessageBox("You~recieved~100~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3300665)
                {
                    client.Player.ConquerPoints += 5000;
                    client.Player.MessageBox("You~recieved~5000~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                //////////////////
                if (item.ITEM_ID == 3300671)
                {
                    client.Player.ConquerPoints += 30;
                    client.Player.MessageBox("You~recieved~30~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3300661)
                {
                    client.Player.ConquerPoints += 50;
                    client.Player.MessageBox("You~recieved~50~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3300662)
                {
                    client.Player.ConquerPoints += 100;
                    client.Player.MessageBox("You~recieved~100~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3301183)
                {
                    client.Player.BoundConquerPoints += 3;
                    client.Player.MessageBox("You~recieved~3~cps(B).", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3301183)
                {
                    client.Player.BoundConquerPoints += 3;
                    client.Player.MessageBox("You~recieved~3~cps(B).", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3301184)
                {
                    client.Player.BoundConquerPoints += 10;
                    client.Player.MessageBox("You~recieved~10~cps(B).", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3301185)
                {
                    client.Player.BoundConquerPoints += 15;
                    client.Player.MessageBox("You~recieved~15~cps(B).", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3301186)
                {
                    client.Player.BoundConquerPoints += 20;
                    client.Player.MessageBox("You~recieved~20~cps(B).", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3301187)
                {
                    client.Player.BoundConquerPoints += 30;
                    client.Player.MessageBox("You~recieved~30~cps(B).", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                if (item.ITEM_ID == 3303706)
                {
                    client.Player.ChargCps += 1000000;
                    client.Player.MessageBox("You~recieved~1.000.000~cps.", null, null);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    return;
                }
                /*case Database.ItemType.LoveHatMale:
                        {
                            client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 89);
                            client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                            client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                            client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            break;
                        }*/
                if (item.ITEM_ID == 3005978)
                {
                    if (Role.Core.IsBoy(client.Player.Body))
                    {
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 89);
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        //client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        return;
                    }
                    else
                    {
                        client.SendSysMesage("this~cut~is~legal~only~for~Male.");
                        return;
                    }
                }
                if (item.ITEM_ID == 3005977)
                {
                    if (Role.Core.IsBoy(client.Player.Body))
                    {
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 80);
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        //client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        return;
                    }
                    else
                    {
                        client.SendSysMesage("this~cut~is~legal~only~for~Male.");
                        return;
                    }
                }
                if (item.ITEM_ID == 3005974)
                {
                    if (Role.Core.IsGirl(client.Player.Body))
                    {
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 80);
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        //client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        return;
                    }
                    else
                    {
                        client.SendSysMesage("this~cut~is~legal~only~for~Girl.");
                        return;
                    }
                }
                if (item.ITEM_ID == 3005979)
                {
                    if (Role.Core.IsBoy(client.Player.Body))
                    {
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 90);
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        //client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        return;
                    }
                    else
                    {
                        client.SendSysMesage("this~cut~is~legal~only~for~Male.");
                        return;
                    }
                }
                if (item.ITEM_ID == 3005976)
                {
                    if (Role.Core.IsGirl(client.Player.Body))
                    {
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 82);
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        //client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        return;
                    }
                    else
                    {
                        client.SendSysMesage("this~cut~is~legal~only~for~Girl.");
                        return;
                    }
                }
                if (item.ITEM_ID == 3005975)
                {
                    if (Role.Core.IsGirl(client.Player.Body))
                    {
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 89);
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        //client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        return;
                    }
                    else
                    {
                        client.SendSysMesage("this~cut~is~legal~only~for~Girl.");
                        return;
                    }
                }
                if (item.ITEM_ID == Database.ItemType.P7EquipmentSoulScrap)
                {
                    if (client.Inventory.ContainItemWithStack(Database.ItemType.P7EquipmentSoulScrap, 15))
                    {
                        client.Inventory.RemoveStackItem(Database.ItemType.P7EquipmentSoulScrap, 15, stream);
                        client.Inventory.Add(stream, Database.ItemType.P7EquipmentSoulPack, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound == 1, Role.Flags.ItemEffect.None, true, ".");
                    }
                    else if (client.Inventory.Contain(Database.ItemType.P7EquipmentSoulScrap, 15, item.Bound))
                    {
                        client.Inventory.Remove(Database.ItemType.P7EquipmentSoulScrap, 15, stream);
                        client.Inventory.Add(stream, Database.ItemType.P7EquipmentSoulPack, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound == 1, Role.Flags.ItemEffect.None, true, ".");
                    }
                    else
                    {
                        client.SendSysMesage("don't have 15 Scrap.");
                    }
                    return;
                }
                if (item.ITEM_ID == Database.ItemType.P7WeaponSoulScrap)
                {
                    if (client.Inventory.ContainItemWithStack(Database.ItemType.P7WeaponSoulScrap, 15))
                    {
                        client.Inventory.RemoveStackItem(Database.ItemType.P7WeaponSoulScrap, 15, stream);
                        client.Inventory.Add(stream, Database.ItemType.P7WeaponSoulPack, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound == 1, Role.Flags.ItemEffect.None, true, ".");
                    }
                    else if (client.Inventory.Contain(Database.ItemType.P7WeaponSoulScrap, 15, item.Bound))
                    {
                        client.Inventory.Remove(Database.ItemType.P7WeaponSoulScrap, 15, stream);
                        client.Inventory.Add(stream, Database.ItemType.P7WeaponSoulPack, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound == 1, Role.Flags.ItemEffect.None, true, ".");
                    }
                    else
                    {
                        client.SendSysMesage("don't have 15 Scrap.");
                    }
                    return;
                }
                if (item.ITEM_ID == Database.ItemType.SacredRefineryScrap)
                {
                    if (client.Inventory.ContainItemWithStack(Database.ItemType.SacredRefineryScrap, 15))
                    {
                        client.Inventory.RemoveStackItem(Database.ItemType.SacredRefineryScrap, 15, stream);
                        client.Inventory.Add(stream, Database.ItemType.SacredRefineryPack, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound == 1, Role.Flags.ItemEffect.None, true, ".");
                    }
                    else if (client.Inventory.Contain(Database.ItemType.SacredRefineryScrap, 15, item.Bound))
                    {
                        client.Inventory.Remove(Database.ItemType.SacredRefineryScrap, 15, stream);
                        client.Inventory.Add(stream, Database.ItemType.SacredRefineryPack, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound == 1, Role.Flags.ItemEffect.None, true, ".");
                    }
                    else
                    {
                        client.SendSysMesage("don't have 15 Scrap.");
                    }
                    return;
                }
                if (item.ITEM_ID == 727267)
                {
                    if (client.Player.CPBoundPack >= 30)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        return;
                    }
                    if (client.Player.ClaimedBCPToday)
                    {
                        client.SendSysMesage("Claim (B) CPs tomorrow.");
                        return;
                    }
                    if (client.Player.CPBoundPack == 29)
                    {
                        client.Player.BoundConquerPoints += 20;
                        client.Player.ClaimedBCPToday = true;
                        client.Player.CPBoundPack++;
                        client.SendSysMesage("Successfully claimed 20 (B) cps.");
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        return;
                    }
                    else if (client.Player.CPBoundPack < 29)
                    {
                        client.Player.BoundConquerPoints += 20;
                        client.Player.ClaimedBCPToday = true;
                        client.Player.CPBoundPack++;
                        client.SendSysMesage("Successfully claimed 20 (B) cps.");
                        return;
                    }
                }
                if (item.ITEM_ID == 727266)
                {
                    uint[] IDs = null;
                    if (client.Player.Level >= 100)
                        return;
                    switch (client.Player.Class)
                    {
                        case 10:
                            int rand = new Random().Next(1, 4);
                            uint[] Ids = null;
                            switch (rand)
                            {
                                case 1:
                                    Ids = new uint[2] { 420029/*sword*/, 410029/*blade*/ };
                                    break;
                                case 2:
                                    Ids = new uint[2] { 420029/*sword*/, 480029/*club*/ };
                                    break;
                                case 3:
                                    Ids = new uint[2] { 410029/*blade*/, 480029/*club*/ };
                                    break;
                            }
                            IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, Ids[0], Ids[1], 130009/*Armor*/, 118009/*hood*/ };
                            break;
                        case 20:
                            int rands = new Random().Next(1, 3);
                            uint ID = 0;
                            switch (rands)
                            {
                                case 1:
                                    ID = 561029;
                                    break;
                                case 2:
                                    ID = 560029;
                                    break;
                            }
                            IDs = new uint[6] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, ID/*weap*/, 131009/*Armor*/, 111009/*hood*/ };
                            break;
                        case 40:
                            IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, 613019/*knife*/, 613019/*knife*/, 133009/*Armor*/, 113009/*hood*/ };
                            break;
                        case 50:
                            IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, 601029/*katana*/, 601029/*katana*/, 135009/*Armor*/, 123009/*hood*/ };
                            break;
                        case 60:
                            IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, 610029/*prayer*/, 610029/*prayer*/, 136009/*Armor*/, 143009/*hood*/ };
                            break;
                        case 70:
                            IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, 611029/*raper*/, 612029/*pistol*/, 139009/*Armor*/, 144009/*hood*/ };
                            break;
                        case 80:
                            IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, 617029/*weap*/, 617029/*weap*/, 138009/*Armor*/, 148009/*hood*/ };
                            break;
                        case 100:
                            IDs = new uint[6] { 152019/*necklace*/, 121029/*ring*/, 160039/*boot*/, 421029/*weap*/, 134009/*Armor*/, 114009/*hood*/ };
                            break;
                        case 160:
                            IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, 626029/*weap*/, 626029/*weap*/, 101009/*Armor*/, 170009/*hood*/ };
                            break;
                        default:
                            break;
                    }
                    if (IDs != null)
                    {
                        foreach (uint ID in IDs)
                        {
                            if (client.Player.Class == 100)
                            {
                                client.Inventory.Add(stream, ID, 1, 5, 0, 0, Conquer6609.Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                            {
                                client.Inventory.Add(stream, ID, 1, 5, 0, 0, Conquer6609.Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, true);
                            }
                        }
                        client.SendSysMesage("You claimed full equipment super +5 (S).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    }
                }
                if (item.ITEM_ID == 3003219)//RecruitGearBox
                {
                    if (client.Player.JustCreated)
                    {
                        uint[] IDs = null;
                        switch (client.Player.Class)
                        {
                            case 10:
                                int rand = new Random().Next(1, 4);
                                uint[] Ids = null;
                                switch (rand)
                                {
                                    case 1:
                                        Ids = new uint[2] { 420029/*sword*/, 410029/*blade*/ };
                                        break;
                                    case 2:
                                        Ids = new uint[2] { 420029/*sword*/, 480029/*club*/ };
                                        break;
                                    case 3:
                                        Ids = new uint[2] { 410029/*blade*/, 480029/*club*/ };
                                        break;
                                }
                                IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, Ids[0], Ids[1], 130009/*Armor*/, 118009/*hood*/ };
                                break;
                            case 20:
                                int rands = new Random().Next(1, 3);
                                uint ID = 0;
                                switch (rands)
                                {
                                    case 1:
                                        ID = 561029;
                                        break;
                                    case 2:
                                        ID = 560029;
                                        break;
                                }
                                IDs = new uint[6] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, ID/*weap*/, 131009/*Armor*/, 111009/*hood*/ };
                                break;
                            case 40:
                                IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, 613019/*knife*/, 613019/*knife*/, 133009/*Armor*/, 113009/*hood*/ };
                                break;
                            case 50:
                                IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, 601029/*katana*/, 601029/*katana*/, 135009/*Armor*/, 123009/*hood*/ };
                                break;
                            case 60:
                                IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, 610029/*prayer*/, 610029/*prayer*/, 136009/*Armor*/, 143009/*hood*/ };
                                break;
                            case 70:
                                IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, 611029/*raper*/, 612029/*pistol*/, 139009/*Armor*/, 144009/*hood*/ };
                                break;
                            case 80:
                                IDs = new uint[7] {120029/*necklace*/, 150039/*ring*/, 160039/*boot*/,  617029/*weap*/, 617029/*weap*/, 138009/*Armor*/, 148009/*hood*/ };
                                break;
                            case 100:
                                IDs = new uint[6] { 152019/*necklace*/, 121029/*ring*/, 160039/*boot*/, 421029/*weap*/, 134009/*Armor*/, 114009/*hood*/ };
                                break;
                            case 160:
                                IDs = new uint[7] { 120029/*necklace*/, 150039/*ring*/, 160039/*boot*/, 626029/*weap*/, 626029/*weap*/, 101009/*Armor*/, 170009/*hood*/ };
                                break;
                            default:
                                break;
                        }
                        if (IDs != null)
                        {
                            foreach (uint ID in IDs)
                            {
                                client.Inventory.Add(stream, ID, 1 ,5, 0, 0, Conquer6609.Role.Flags.Gem.EmptySocket, Conquer6609.Role.Flags.Gem.EmptySocket, true);
                            }
                            client.SendSysMesage("You claimed full equipment super .", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        }
                    }
                    else
                    {
                        client.SendSysMesage("This item is only for just created characters.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    }
                }
                switch (item.ITEM_ID)
                {
                    case 720279:
                        {
                            if (client.Player.X == Database.Server.AncientBeadX && client.Player.Y == Database.Server.AncientBeadY)
                            {
                                client.Player.MessageBox("You~found~an~Ancient~Bead!~You~should~show~it~to~the~Fiend~Lair~Guard.", null, null);
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Inventory.Add(stream, 720280);
                                break;
                            }
                            client.Player.MessageBox("You~get~a~strange~feeling~that~an~Ancient~Bead~is~buried~under~the~earth,~somewhere~near~("+Database.Server.AncientBeadX+","+Database.Server.AncientBeadY+").", null, null);
                            break;
                        }
                    case 720282:
                    case 721003:
                    case 721004:
                    case 721005:
                    case 721006:
                    case 721007:
                    case 721008:
                    case 721009:
                        {
                            if (client.Player.Level >= 140)
                            {
                                client.SendSysMesage("You are high level ! ", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                                break;
                            }
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.SendSysMesage("You have gained 1.0 exp.");
                            break;
                        }
                    case 3004895://StrengthToken
                    case 3004896://SpiritToken
                    case 3004897://VitalityToken
                    case 3004898://AgilityToken
                        {
                            if (!client.Inventory.Contain(3004895, 1, item.Bound))
                            {
                                client.SendSysMesage("Failed to combine still need StrengthToken.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }
                            if (!client.Inventory.Contain(3004896, 1, item.Bound))
                            {
                                client.SendSysMesage("Failed to combine still need SpiritToken.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }
                            if (!client.Inventory.Contain(3004897, 1, item.Bound))
                            {
                                client.SendSysMesage("Failed to combine still need VitalityToken.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }
                            if (!client.Inventory.Contain(3004898, 1, item.Bound))
                            {
                                client.SendSysMesage("Failed to combine still need AgilityToken.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }
                            bool bound = item.Bound == 1 ? true : false;
                            client.Inventory.Remove(3004895, 1, stream);
                            client.Inventory.Remove(3004896, 1, stream);
                            client.Inventory.Remove(3004897, 1, stream);
                            client.Inventory.Remove(3004898, 1, stream);
                            client.Inventory.Add(stream, 3001044, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, bound, Role.Flags.ItemEffect.None, true, " from combining 4 sorts of tokens.");
                            break;
                        }
                    case 3301169:
                        {
                            MsgTaskReward.OpenWheel(client, stream, MsgTaskReward.FortuneWheelTicket, 2811);
                            break;
                        }
                    case 3008193:
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Player.MyChi.ChiPoints += 1000;
                            client.SendSysMesage("You~recieved~1000~Chi~Points.");
                            break;
                        }
                    case 3001413:
                        {
                            if (client.Inventory.ContainItemWithStack(3001413, 1))
                            {
                                client.Inventory.RemoveStackItem(3001413, 1, stream);
                                client.Player.MyChi.ChiPoints += 500;
                                client.SendSysMesage("You~recieved~500~Chi~Points.");
                                break;
                            }
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Player.MyChi.ChiPoints += 500;
                            client.SendSysMesage("You~recieved~500~Chi~Points.");
                            break;
                        }
                    case Database.ItemType.MemoryAgate:
                        {
                            if (Database.Server.ClientAgates.ContainsKey(client.Player.UID))
                            {
                                if (!Database.Server.ClientAgates[client.Player.UID].ContainsKey(item.UID))
                                {
                                    Database.Server.ClientAgates[client.Player.UID].Add(item.UID, new Dictionary<uint, Tuple<uint, uint, uint, string>>());
                                }
                            }
                            else
                            {
                                Database.Server.ClientAgates.Add(client.Player.UID, new Dictionary<uint, Dictionary<uint, Tuple<uint, uint, uint, string>>>());
                                Database.Server.ClientAgates[client.Player.UID].Add(item.UID, new Dictionary<uint, Tuple<uint, uint, uint, string>>());

                            }
                            client.Send(stream.CreateSuperFlag(item, Database.Server.ClientAgates[client.Player.UID][item.UID]));
                            break;
                        }
                    case 3303484://ChaosBeadPack
                        {
                            if (client.Inventory.ContainItemWithStack(3303484, 1))
                            {
                                client.Inventory.RemoveStackItem(3303484, 1, stream);
                                client.Inventory.AddItemWitchStack(3303483, 0, 30, stream);
                                client.SendSysMesage("You~recieved~30~Chaos~Beads.");
                            }
                            else if (client.Inventory.Contain(3303484, 1))
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Inventory.AddItemWitchStack(3303483, 0, 30, stream);
                                client.SendSysMesage("You~recieved~30~Chaos~Beads.");
                            }
                            break;
                        }
                    case 3303975:
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                client.SendSysMesage("You~recieved~1~DB.");
                            }
                            else
                            {
                                client.Inventory.AddReturnedItem(stream, 3303975, 1);
                                client.SendSysMesage("You~recieved~1~DB.");
                            }
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            break;
                        }
                    case 3303482://ChasmWishingGate
                        {
                            if (client.Player.Map != 10166)
                            {
                                client.SendSysMesage("Only~usable~inside~Chasm.");
                                return;
                            }
                            ushort x = 0, y = 0;
                            Database.Server.ServerMaps[10166].GetRandCoord(ref x, ref y);
                            client.Teleport(x, y, 10166);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            break;
                        }
                    default:
                        break;
                }
                if (item.ITEM_ID >= 726001 && item.ITEM_ID <= 726108 || item.ITEM_ID == 722559// item box.
                || item.ITEM_ID >= 721177 && item.ITEM_ID <= 721202)
                {
                    if (Database.HouseTable.InHouse(client.Player.Map) && client.Player.DynamicID == client.Player.UID/*that check if is you house*/)
                    {
                        NpcServerQuery Furniture = new NpcServerQuery();
                        if (item.ITEM_ID == 722559)
                            Furniture.NpcType = Role.Flags.NpcType.Talker;
                        else
                            Furniture.NpcType = Role.Flags.NpcType.Furniture;

                        Furniture.Action = MsgNpc.NpcServerReplay.Mode.Cursor;
                        var Npc = Database.NpcServer.GetNpc(item.ITEM_ID);
                        if (Npc != null)
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            Furniture.Mesh = Npc.Mesh;
                            Furniture.ID = (MsgNpc.NpcID)Npc.UID;
                            client.MoveNpcMesh = Furniture.Mesh;
                            client.MoveNpcUID = (uint)Furniture.ID;
                            client.Send(stream.NpcServerCreate(Furniture));
                        }
                    }
                    else
                    {
                        client.SendSysMesage("You have to be in your own house to be able to display it.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                    }
                    return;
                }
                if (item.ITEM_ID >= 1000000 && item.ITEM_ID <= 1000040
                    || item.ITEM_ID >= 1002000 && item.ITEM_ID < 1002030
                    || item.ITEM_ID == 1002050
                    || item.ITEM_ID == 725065 || item.ITEM_ID == 1003010 || item.ITEM_ID == 1003000 || item.ITEM_ID == 1003020 || item.ITEM_ID == 723993)
                {
                    if (DateTime.Now > client.Player.MedicineStamp.AddMilliseconds(300))
                    {
                        if (client.Player.ContainFlag(MsgUpdate.Flags.PoisonStar) && item.ITEM_ID != 1060020 && item.ITEM_ID != 1060021 && item.ITEM_ID != 1060022 && item.ITEM_ID != 1060023 && item.ITEM_ID != 1060024)
                            return;
                        if (client.Player.HitPoints == client.Status.MaxHitpoints)
                            return;
                        client.Player.HitPoints = Math.Min(client.Player.HitPoints + DBItem.ItemHP, (int)client.Status.MaxHitpoints);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MedicineStamp = DateTime.Now;
                    }
                }
                else if (item.ITEM_ID >= 1001000 && item.ITEM_ID <= 1001040
                    || item.ITEM_ID == 1002030 || item.ITEM_ID == 1002040
                    || item.ITEM_ID == 725066 || item.ITEM_ID == 1004010)
                {
                    if (client.Player.ContainFlag(MsgUpdate.Flags.PoisonStar) && item.ITEM_ID != 1060020 && item.ITEM_ID != 1060021 && item.ITEM_ID != 1060022 && item.ITEM_ID != 1060023 && item.ITEM_ID != 1060024)
                        return;
                    if (client.Player.Mana == client.Status.MaxMana || !client.ManaPotionReady())
                        return;
                    client.Player.Mana = (ushort)Math.Min(client.Player.Mana + DBItem.ItemMP, (int)client.Status.MaxMana);
                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                    client.ManaMedicineStamp = DateTime.Now;
                }
                else if (item.ITEM_ID >= 725000 && item.ITEM_ID <= 725044 || item.ITEM_ID == 1060101 || item.ITEM_ID == 3303481
                    || item.ITEM_ID == 721158 || item.ITEM_ID == 721157 || item.ITEM_ID == 3007568 || item.ITEM_ID == 3007567
                    || item.ITEM_ID == 3007566 || item.ITEM_ID >= 3006017 && item.ITEM_ID <= 3006020)//skills books
                {
                    switch (item.ITEM_ID)
                    {
                        case 3303481:
                            {
                                if (client.Player.PKPoints < 30)
                                {
                                    client.SendSysMesage("Only~usable~while~pk~increased~to~30~points.");
                                    return;
                                }
                                if(client.Player.VipLevel < 3)
                                {
                                    client.SendSysMesage("Only~usable~for~+3~vip~level~membership.");
                                    return;
                                }
                                client.Player.PKPoints -= 30;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("Your~pk~points~removed.");
                                break;
                            }
                        case 729252:
                            {
                                client.Player.MyChi.ChiPoints += 10;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~recieved~10~chi~points.");
                                break;
                            }
                        case 1060101://fireofheal
                            {
                                if (client.Player.Level < 84)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~84.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireofHell))
                                {
                                    if (Database.AtributesStatus.IsFire(client.Player.Class))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireofHell);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.FireofHell.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,this spell is just for Fire Taoist`s.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725016://nightdevil
                            {
                                if (client.Player.Level < 90)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~90.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NightDevil))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NightDevil);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.NightDevil.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 721157://dragon tail
                            {
                                if (client.Player.Level < 40)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonTail))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonTail);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.DragonTail.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 721158://ViperFang
                            {
                                if (client.Player.Level < 40)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ViperFang))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ViperFang);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.ViperFang.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725005://fastblader
                            {
                                if (client.Player.Level < 40)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                {
                                    if (client.MyProfs.CheckProf((ushort)Database.MagicType.WeaponsType.Blade, 5))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);

                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.FastBlader.ToString() + ".", "I~see.");
                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~skill~before~you~practice~your~blade~to~level~5.~Please~train~harder.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725010://screensword
                            {
                                if (client.Player.Level < 40)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                                {
                                    if (client.MyProfs.CheckProf((ushort)Database.MagicType.WeaponsType.Sword, 5))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.ScrenSword.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~skill~before~you~practice~your~sword~to~level~5.~Please~train~harder.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725029://pheonix
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Phoenix))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Phoenix);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Phoenix.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725042://rage
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Rage))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Rage);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Rage.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725030://boom
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Boom))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Boom);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Boom.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725040://seizer
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Seizer))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Seizer);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Seizer.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");
                                }
                                break;
                            }
                        case 725041://Earthquake
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Earthquake))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Earthquake);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Earthquake.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725043://celestial
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Celestial))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Celestial);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Celestial.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725044://roamer
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roamer))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Roamer);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Roamer.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725013://penetarion
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Penetration))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Penetration);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Penetration.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725026://snow
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Snow))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Snow);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Snow.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725012://speed Gun
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedGun))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpeedGun);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.SpeedGun.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725011://WideStrike
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WideStrike))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WideStrike);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.WideStrike.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725027:// StrandedMonster
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StrandedMonster))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StrandedMonster);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.StrandedMonster.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725014://halt
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Halt))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Halt);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Halt.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725031://bores
                            {
                                if (client.Player.Level < 30)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Boreas))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Boreas);
                                    client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Boreas.ToString() + ".", "I~see.");

                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }

                                break;
                            }
                        case 725000://thunder.
                            {
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                                {
                                    if (client.Player.Spirit >= 20)
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Thunder.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~20.~Please~train~harder.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725001://fire
                            {
                                if (client.Player.Level < 40)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");
                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fire))
                                {
                                    if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                    {
                                        if (client.MySpells.CheckSpell((ushort)Role.Flags.SpellID.Thunder, 4))
                                        {
                                            if (client.Player.Spirit >= 80)
                                            {
                                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Fire);
                                                client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Fire.ToString() + ".", "I~see.");

                                            }
                                            else
                                            {
                                                client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~80.~Please~train~harder.", "I~see.");

                                            }
                                        }
                                        else
                                        {
                                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~you~practice~Thunder~to~level~4.", "I~see.");

                                        }
                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,the spell is just for Taoists.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725002://tornado.
                            {
                                if (client.Player.Level < 90)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~90.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tornado))
                                {
                                    if (Database.AtributesStatus.IsFire(client.Player.Class))
                                    {
                                        if (client.MySpells.CheckSpell((ushort)Role.Flags.SpellID.Fire, 3))
                                        {
                                            if (client.Player.Spirit >= 160)
                                            {
                                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Tornado);
                                                client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Tornado.ToString() + ".", "I~see.");

                                            }
                                            else
                                            {
                                                client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~160.~Please~train~harder.", "I~see.");

                                            }
                                        }
                                        else
                                        {
                                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~you~practice~Fire~to~level~3.", "I~see.");

                                        }
                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,this spell is just for Fire Taoist.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725003://cure
                            {

                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                                {

                                    if (client.Player.Spirit >= 30)
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Cure.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~30.~Please~train~harder.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725015://divinehare
                            {
                                if (client.Player.Level < 54)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~54.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DivineHare))
                                {

                                    if (Database.AtributesStatus.IsWater(client.Player.Class))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DivineHare);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.DivineHare.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,this spell is just for Water Taoist.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725004://lighiting
                            {
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning))
                                {
                                    if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                    {
                                        if (client.Player.Spirit >= 25)
                                        {
                                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Lightning);
                                            client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.Lightning.ToString() + ".", "I~see.");

                                        }
                                        else
                                        {
                                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~25.~Please~train~harder.", "I~see.");

                                        }
                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,this spell is just for Taoist`s.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                } break;
                            }
                        case 725028://SpeedLightning
                            {
                                if (client.Player.Level < 70)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~70.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedLightning))
                                {
                                    if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                    {
                                        if (client.Player.Spirit >= 25)
                                        {
                                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpeedLightning);
                                            client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.SpeedLightning.ToString() + ".", "I~see.");

                                        }
                                        else
                                        {
                                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~25.~Please~train~harder.", "I~see.");

                                        }
                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,this spell is just for Taoist`s.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 725025://fly moon
                            {
                                if (client.Player.Level < 40)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlyingMoon))
                                {
                                    if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FlyingMoon);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.FlyingMoon.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,this spell is just for Warrior`s.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 3007568://
                            {
                                if (client.Player.Level < 70)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~70.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.InfernalEcho))
                                {
                                    if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.InfernalEcho);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.InfernalEcho.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,this spell is just for Monks`s.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 3007567://
                            {
                                if (client.Player.Level < 70)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~70.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GraceofHeaven))
                                {
                                    if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.GraceofHeaven);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.GraceofHeaven.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,this spell is just for Monks`s.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 3007566://
                            {
                                if (client.Player.Level < 70)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~70.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WrathoftheEmperor))
                                {
                                    if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WrathoftheEmperor);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.WrathoftheEmperor.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry,this spell is just for Monks`s.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }

                        case 3006017://
                            {
                                if (client.Player.Level < 70)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~70.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlameLotus))
                                {
                                    if (Database.AtributesStatus.IsFire(client.Player.Class))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FlameLotus);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.FlameLotus.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry, this spell is just for Fire`s.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 3006019://
                            {
                                if (client.Player.Level < 70)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~70.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SearingTouch))
                                {
                                    if (Database.AtributesStatus.IsFire(client.Player.Class))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SearingTouch);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.SearingTouch.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry, this spell is just for Fire`s.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 3006018://
                            {
                                if (client.Player.Level < 70)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~70.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AuroraLotus))
                                {
                                    if (Database.AtributesStatus.IsWater(client.Player.Class))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AuroraLotus);
                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.AuroraLotus.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry, this spell is just for Water`s.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }
                        case 3006020://
                            {
                                if (client.Player.Level < 70)
                                {
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~70.~Please~train~harder.", "I~see.");

                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlessingTouch))
                                {
                                    if (Database.AtributesStatus.IsWater(client.Player.Class))
                                    {
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BlessingTouch);

                                        client.CreateDialog(stream, "You~have~learned~" + Role.Flags.SpellID.BlessingTouch.ToString() + ".", "I~see.");

                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Sorry, this spell is just for Water`s.", "I~see.");

                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "You already have this spell.", "I~see.");

                                }
                                break;
                            }

                    }
                }
                else if (Database.ItemType.IsMagicSpar(DBItem.ID))
                {
                    #region MagicSpars
                    switch (item.ITEM_ID)
                    {
                        case 3006587:
                            {
                                // client: "Break it to receive 200,000,000 Silver."
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 200000000;
                                client.SendSysMesage("You received 200,000,000 silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }
                        case 3006588:
                            {
                                // client: "Break it to receive 500,000,000 Silver."
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 500000000;
                                client.SendSysMesage("You received 500,000,000 silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }
                        case 3006589:
                            {
                                // client: "Break it to receive 1000,000,000 Silver."
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 1000000000;
                                client.SendSysMesage("You received 1,000,000,000 silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }
                        case 3006594:
                            {
                                client.Player.Money += 200000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~200,000~money.");
                                break;
                            }
                        case 3006595:
                            {
                                client.Player.Money += 500000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~500,000~money.");
                                break;
                            }
                        case 3006596:
                            {
                                client.Player.Money += 1500000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~1,500,000~money.");
                                break;
                            }
                        case 3006597:
                            {
                                client.Player.Money += 2000000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~2,000,000~money.");
                                break;
                            }
                        case 3006602:
                        case 3006598:
                            {
                                client.Player.Money += 2500000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~2,500,000~money.");
                                break;
                            }
                        case 3006603:
                            {
                                client.Player.Money += 7500000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~7,500,000~money.");
                                break;
                            }
                        case 3006599:
                            {
                                client.Player.Money += 5000000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~5,000,000~money.");
                                break;
                            }
                        case 3006600:
                            {
                                client.Player.Money += 8000000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~8,000,000~money.");
                                break;
                            }
                        case 3006604:
                        case 3006601:
                            {
                                client.Player.Money += 10000000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~10,000,000~money.");
                                break;
                            }
                        case 3006609:
                            {
                                client.Player.Money += 15000000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~15,000,000~money.");
                                break;
                            }
                        case 3006610:
                            {
                                client.Player.Money += 20000000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~20,000,000~money.");
                                break;
                            }
                        case 3006605:
                            {
                                client.Player.Money += 12500000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~12,500,000~money.");
                                break;
                            }
                        case 3006611:
                        case 3006606:
                            {
                                client.Player.Money += 25000000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~25,000,000~money.");
                                break;
                            }
                        case 3006612:
                        case 3006607:
                        case 3006615:
                            {
                                // MagicSpar(50Million): "Break it to receive 50,000,000 Silver."
                                if (!client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                                    break;
                                client.Player.Money += 50000000;
                                client.SendSysMesage("You~can~gain~50,000,000~money.");
                                break;
                            }
                        case 3006582:
                        case 3006616:
                            {
                                // MagicSpar(100Million): "Break it to receive 100,000,000 Silver."
                                if (!client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                                    break;
                                client.Player.Money += 100000000;
                                client.SendSysMesage("You~can~gain~100,000,000~money.");
                                break;
                            }
                        case 3006613:
                            {
                                client.Player.Money += 30000000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~30,000,000~money.");
                                break;
                            }
                        case 3006614:
                            {
                                client.Player.Money += 40000000;
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.SendSysMesage("You~can~gain~40,000,000~money.");
                                break;
                            }
                        case 3006608:
                            {
                                // MagicSpar(250Million): "Break it to receive 250,000,000 Silver."
                                if (!client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                                    break;
                                client.Player.Money += 250000000;
                                client.SendSysMesage("You~can~gain~250,000,000~money.");
                                break;
                            }
                        default:
                            break;
                    }
                    #endregion
                }
                else if (Database.ItemType.IsMoneyBag(DBItem.ID))
                {
                    #region MoneyBags
                    switch (item.ITEM_ID)
                    {
                        case 3005945:
                            {
                                // HonorableMoneyBag: "It contains 200,000,000 Silver. Right click to open."
                                if (!client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                                    break;
                                client.Player.Money += 200000000;
                                client.SendSysMesage("You received 200000000 silver for opening the HonorableMoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }


                        case 3008452:
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 1000000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 1000000 silver for opening the MillionSilverBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                break;
                            }
                        case 723713:
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 300000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 300000 silver for opening the MoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                break;
                            }
                        case 723714:
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 800000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 800000 silver for opening the MoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                break;
                            }
                        case 723715:
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 1200000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 1200000 silver for opening the MoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                break;
                            }
                        case 723716:
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 1800000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 1800000 silver for opening the MoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                break;
                            }
                        case 723717:
                            {
                              
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 5000000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 5000000 silver for opening the MoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }
                        case 723718:
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 20000000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 20000000 silver for opening the MoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                break;
                            }
                        case 723719:
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 25000000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 25000000 silver for opening the MoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                break;
                            }
                        case 723720:
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 80000000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 80000000 silver for opening the MoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                break;
                            }
                        case 723721:
                            {
                                // client: "Right click on it to get 100000000 silvers."
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 100000000;
                                client.SendSysMesage("You received 100000000 silver for opening the MoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }
                        case 723722:
                            {
                                // client: "Right click on it to get 300000000 silvers."
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 300000000;
                                client.SendSysMesage("You received 300000000 silver for opening the MoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }
                        case 723723:
                            {
                                // client: "Right click on it to get 500000000 silvers."
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.Money += 500000000;
                                client.SendSysMesage("You received 500000000 silver for opening the MoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                break;
                            }
                    }
                    #endregion
                }
                else if (item.ITEM_ID >= 3005107 && item.ITEM_ID <= 3005135 || item.ITEM_ID == 3001264 || item.ITEM_ID == 3003126
                    || item.ITEM_ID == 728919)
                {

                    switch (item.ITEM_ID)
                    {
                        case 3005107:
                            {
                                client.Player.BoundConquerPoints += 20;

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005108:
                            {
                                client.Player.BoundConquerPoints += 30;

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005109:
                            {
                                client.Player.BoundConquerPoints += 50;

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005110:
                            {
                                client.Player.BoundConquerPoints += 80;

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005111:
                            {
                                client.Player.BoundConquerPoints += 100;

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005112:
                            {
                                client.Player.BoundConquerPoints += 200;

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005113:
                            {
                                client.Player.BoundConquerPoints += 300;

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005114:
                            {
                                client.Player.BoundConquerPoints += 500;

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005115:
                            {
                                client.Player.BoundConquerPoints += 800;

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005116:
                            {
                                client.Player.BoundConquerPoints += 1000;

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005117:
                            {
                                client.Player.Money += 5000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005118:
                            {
                                client.Player.Money += 10000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005119:
                            {
                                client.Player.Money += 20000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005120:
                            {
                                client.Player.Money += 50000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005121:
                            {
                                client.Player.Money += 100000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        case 3005122:
                            {
                                client.Player.Money += 200000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                if (client.Player.SubClass != null)
                                {
                                    client.Player.SubClass.AddStudyPoints(client, 400, stream);
                                    client.SendSysMesage("You received 400 StudyPoints.");

                                }
                                break;
                            }
                        case 3005123:
                            {
                                if (client.Player.SubClass != null)
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                    client.SendSysMesage("You received 200 StudyPoints.");

                                }
                                break;
                            }
                        case 3005124:
                            {
                                if (client.Player.SubClass != null)
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.Player.SubClass.AddStudyPoints(client, 300, stream);
                                    client.SendSysMesage("You received 300 StudyPoints.");

                                }
                                break;
                            }
                        case 3005125:
                            {
                                if (client.Player.SubClass != null)
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.Player.SubClass.AddStudyPoints(client, 500, stream);
                                    client.SendSysMesage("You received 500 StudyPoints.");

                                }
                                break;
                            }
                        case 3005126:
                            {

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 500;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                client.CreateBoxDialog("You received 500 ChiPoints.");


                                break;
                            }

                        case 729481:
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 500;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                client.CreateBoxDialog("You received 500 ChiPoints.");

                                break;
                            }
                        case 3005127:
                            {

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 1000;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                client.CreateBoxDialog("You received 1000 ChiPoints.");


                                break;
                            }
                        case 3005128:
                            {

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 2000;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                client.CreateBoxDialog("You received 2000 ChiPoints.");


                                break;
                            }
                        case 3005129:
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 3000;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                client.CreateBoxDialog("You received 3000 ChiPoints.");

                                break;
                            }
                        case 3005130:
                            {
                                uint[] Items = new uint[]
                                {
                                    (uint)(700000 + (uint)Role.Flags.Gem.RefinedDragonGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedFuryGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedGloryGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedKylinGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedMoonGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedPhoenixGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedRainbowGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedThunderGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedTortoiseGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedVioletGem)
                                };
                                uint ItemID = Items[(uint)Program.GetRandom.Next(0, Items.Length)];
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                break;
                            }
                        case 3005131:
                            {
                                uint[] Items = new uint[]
                                {
                                  726083
                                    ,726078
                                    ,726080
                                    ,721179
                                    ,726076
                                    ,726082
                                    ,721183
                                    ,726108
                                    ,726107
                                    ,726087
                                    ,726096
                                    ,726095
                                    ,726072
                                    ,726064
                                    ,726073
                                    ,726097
                                   ,726098
                                   ,726086
                                   ,726104
                                   ,726100,726103,726086,726099,726106
                                };

                                uint ItemID = Items[(uint)Program.GetRandom.Next(0, Items.Length)];
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                break;
                            }
                        case 3005132:
                            {
                                uint[] Items = new uint[]
                                {
                                    (uint)(700000 + (uint)Role.Flags.Gem.SuperDragonGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.SuperFuryGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.SuperGloryGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.SuperKylinGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.SuperMoonGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.SuperPhoenixGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.SuperRainbowGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.SuperThunderGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.SuperTortoiseGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.SuperVioletGem)
                                };
                                uint ItemID = Items[(uint)Program.GetRandom.Next(0, Items.Length)];
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                break;
                            }
                        case 3005133:
                            {
                                var PurificationArray = Database.ItemType.PurificationItems[4];
                                var Position = Program.GetRandom.Next(0, PurificationArray.Values.Count);
                                var ReceiveItem = PurificationArray.Values.ToArray()[Position];
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Inventory.Add(stream, ReceiveItem.ID);
                                break;
                            }
                        case 3005134:
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.Inventory.Add(stream, 755099, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.Inventory.Add(stream, 751099, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                }
                                else
                                {
                                    client.SendSysMesage("Please make one space in you bag.");

                                }
                                break;
                            }
                        case 3005135:
                            {
                                var PurificationArray = Database.ItemType.PurificationItems[6];
                                var Position = Program.GetRandom.Next(0, PurificationArray.Values.Count);
                                var ReceiveItem = PurificationArray.Values.ToArray()[Position];
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Inventory.Add(stream, ReceiveItem.ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                break;
                            }
                        case 3001264:
                            {
                                uint[] Items = new uint[]
                                {
                                   181305
                                    ,181315
                                    ,181325
                                    ,181335
                                    ,181345
                                    ,181355
                                    ,181365
                                    ,181375
                                    ,181385
                                    ,181395
                                    ,181405
                                    ,181415
                                    ,181425
                                    ,181435
                                    ,181445
                                    ,181455
                                    ,181465
                                };
                                uint ItemID = Items[(uint)Program.GetRandom.Next(0, Items.Length)];
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                break;
                            }
                        case 3003126:
                            {
                                client.Activeness.IncreaseTask(39);

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.JingTryngUltra = 5;
                                client.SendSysMesage("You have just 5 trying to make Ultra Star.");

                                break;
                            }
                        case 728919:
                            {
                                if (Role.Core.Rate(70))
                                {
                                    var RefinaryArray = Database.ItemType.Refinary[4];
                                    var Position = Program.GetRandom.Next(0, RefinaryArray.Values.Count);
                                    var ReceiveItem = RefinaryArray.Values.ToArray()[Position];
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.Inventory.Add(stream, ReceiveItem.ItemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                }
                                else
                                {
                                    var RefinaryArray = Database.ItemType.Refinary[5];
                                    var Position = Program.GetRandom.Next(0, RefinaryArray.Values.Count);
                                    var ReceiveItem = RefinaryArray.Values.ToArray()[Position];
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.Inventory.Add(stream, ReceiveItem.ItemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                }
                                break;
                            }
                    }
                }
                else if (item.ITEM_ID >= 729476 && item.ITEM_ID <= 729479 || item.ITEM_ID >= 3000060 && item.ITEM_ID <= 3000063)
                {
                    switch (item.ITEM_ID)
                    {
                        case 729476:
                            {

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 100;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);



                                break;
                            }
                        case 729477:
                            {

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 200;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);


                                break;
                            }
                        case 729478:
                            {

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 300;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);


                                break;
                            }
                        case 729479:
                            {

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 400;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);


                                break;
                            }
                        case 3000063:
                            {

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);


                                break;
                            }
                        case 3000062:
                            {

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 60;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);


                                break;
                            }
                        case 3000061:
                            {

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 40;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);


                                break;
                            }
                        case 3000060:
                            {

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 20;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);

                                break;
                            }
                    }
                }
                else
                {
                    // Item handlers live in ItemUsage/UseItem.*.cs; each returns handled = false when the id is not one of its cases.
                    bool handled;
                    UseItem_CurrencyPacks(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_ExpAndPotions(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_ProgressionPoints(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_SkillItems(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_SteedsAndAppearance(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_StonesAndScrolls(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_GearPacks(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_RefineryPacks(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_RefineryMaterialPacks(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_SoulAndMaterialPacks(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_EventRewards(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_QuestItems(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_NpcDialogItems(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_MiscItems(item, client, stream, out handled);
                    if (handled)
                        return;
                    UseItem_UnionPacks(item, client, stream, out handled);// Kingdom stipend / allowance packs
                    if (handled)
                        return;
                    UseItem_Packs(item, client, stream, out handled);// data-driven packs (PackContents.ini), always last
                    if (handled)
                        return;
                    // items whose handler reads DBItem:
                    switch (item.ITEM_ID)
                    {
                        

                        case 3005365:
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                                var stage = client.Player.InnerPower.Stages[0];
                                var gong = stage.NeiGongs[0];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3005366:
                            {
                                if (client.Player.InnerPower.Stages[0].NeiGongs[0].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3005365];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[0];
                                var gong = stage.NeiGongs[1];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3005395:
                            {
                                if (client.Player.InnerPower.Stages[0].NeiGongs[1].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3005366];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[1];
                                var gong = stage.NeiGongs[0];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");

                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3005396:
                            {
                                if (client.Player.InnerPower.Stages[1].NeiGongs[0].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3005395];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[1];
                                var gong = stage.NeiGongs[1];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3007113:
                            {
                                if (client.Player.InnerPower.Stages[1].NeiGongs[1].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3005396];

                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[2];
                                var gong = stage.NeiGongs[0];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3007114:
                            {
                                if (client.Player.InnerPower.Stages[2].NeiGongs[0].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3007113];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[2];
                                var gong = stage.NeiGongs[1];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");

                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3005397:
                            {
                                if (client.Player.InnerPower.Stages[2].NeiGongs[1].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3007114];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[3];
                                var gong = stage.NeiGongs[0];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3005398:
                            {
                                if (client.Player.InnerPower.Stages[3].NeiGongs[0].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3005397];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[3];
                                var gong = stage.NeiGongs[1];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3007115:
                            {
                                if (client.Player.InnerPower.Stages[3].NeiGongs[1].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3005398];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[4];
                                var gong = stage.NeiGongs[0];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {

                                    client.SendSysMesage("You have already unlocked this concept.");
                                }

                                break;
                            }
                        case 3007116:
                            {
                                if (client.Player.InnerPower.Stages[4].NeiGongs[0].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3007115];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[4];
                                var gong = stage.NeiGongs[1];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3007117:
                            {
                                if (client.Player.InnerPower.Stages[4].NeiGongs[1].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3007116];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[4];
                                var gong = stage.NeiGongs[2];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3005399:
                            {
                                if (client.Player.InnerPower.Stages[4].NeiGongs[2].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3007117];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[5];
                                var gong = stage.NeiGongs[0];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3005400:
                            {
                                if (client.Player.InnerPower.Stages[5].NeiGongs[0].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3005399];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[5];
                                var gong = stage.NeiGongs[1];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3005401:
                            {
                                if (client.Player.InnerPower.Stages[5].NeiGongs[1].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3005400];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[5];
                                var gong = stage.NeiGongs[2];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3007230:
                            {
                                if (client.Player.InnerPower.Stages[5].NeiGongs[2].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3005401];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[6];
                                var gong = stage.NeiGongs[0];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3007231:
                            {
                                if (client.Player.InnerPower.Stages[6].NeiGongs[0].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3007230];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[6];
                                var gong = stage.NeiGongs[1];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                        case 3007232:
                            {
                                if (client.Player.InnerPower.Stages[6].NeiGongs[1].Complete == false)
                                {
                                    var DbItem2 = Database.Server.ItemsBase[3007231];
                                    client.CreateDialog(stream, "Kid, you need to train the power of " + DbItem2.Name + " to the perfect level before  you can claim the next book. Be patient, okay?", "Okay.");

                                    break;
                                }
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                var stage = client.Player.InnerPower.Stages[6];
                                var gong = stage.NeiGongs[2];
                                if (gong.Unlocked == false)
                                {
                                    stage.UnLocked = gong.Unlocked = true;
                                    client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");

                                    client.SendSysMesage("You successfully acquired " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " successfully acquired [" + DBItem.Name + "], getting closer to be a super hero! [Link Go acquire ###1 692]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.InnerPower).GetArray(stream));

                                }
                                else
                                {
                                    client.SendSysMesage("You have already unlocked this concept.");

                                }

                                break;
                            }
                    }
                }
            }
        }
    }
}
