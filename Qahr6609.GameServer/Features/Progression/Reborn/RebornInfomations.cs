using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Database
{
    public class RebornInfomations : List<Tuple<byte, byte, byte, RebornInfomations.Action, List<ushort>>>
    {
        public List<ushort> StaticSpells = new List<ushort>()
        {
            1360,1260,1290,5050,5040,5030,5020,5010,1045,1046,11000 ,11005,7000,7010,7020,7030,7040,11190,7001,7002,7003,1300
        };
        public enum Action : byte
        {
            Delete = 0,
            Downgrade = 2,
            AllClassSpells = 4,//???
            PureSpell = 5,
            Add = 6,
            AddRebornSpells = 7
        }

        public void Load()
        {
            string[] baseplusText = File.ReadAllLines(Program.ServerConfig.DbLocation + "magictypeop.txt");
            foreach (string line in baseplusText)
            {
                string[] data = line.Split(',');
                byte Reborn = byte.Parse(data[1]);
                byte MyClass = byte.Parse(data[2]);
                byte RebornClass = byte.Parse(data[3]);
                Action Info = (Action)byte.Parse(data[4]);
                List<ushort> Spells = new List<ushort>();
                for (int x = 5; x < data.Length; x++)
                {
                    ushort ID = ushort.Parse(data[x]);
                    if (ID != 0)
                        Spells.Add(ID);
                }
                this.Add(new Tuple<byte, byte, byte, Action, List<ushort>>(Reborn, MyClass, RebornClass, Info, Spells));
            }
        }
        public byte ExtraAtributePoints(byte level, byte mClass)
        {
            if (mClass == 135)
            {
                if (level <= 110)
                    return 0;
                switch (level)
                {
                    case 112: return 1;
                    case 114: return 3;
                    case 116: return 6;
                    case 118: return 10;
                    case 120: return 15;
                    case 121: return 15;
                    case 122: return 21;
                    case 123: return 21;
                    case 124: return 28;
                    case 125: return 28;
                    case 126: return 36;
                    case 127: return 36;
                    case 128: return 45;
                    case 129: return 45;
                    default:
                        return 55;
                }
            }
            else
            {
                if (level <= 120)
                    return 0;
                switch (level)
                {
                    case 121: return 1;
                    case 122: return 3;
                    case 123: return 6;
                    case 124: return 10;
                    case 125: return 15;
                    case 126: return 21;
                    case 127: return 28;
                    case 128: return 36;
                    case 129: return 45;
                    default:
                        return 55;
                }
            }
        }
        public unsafe void Reborn(Role.Player player, byte RebornClass , ServerSockets.Packet stream)
        {


                if (RebornClass % 10 == 1 || RebornClass == 132 || RebornClass == 142)
                {
                    switch (player.Reborn)
                    {
                        case 0:
                            {


                                foreach (var info in this)
                                {
                                    if (info.Item1 == 1 && info.Item2 == player.Class && info.Item3 == RebornClass)
                                    {
                                        switch (info.Item4)
                                        {
                                            case Action.AddRebornSpells:
                                            case Action.Add:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                        player.Owner.MySpells.Add(stream,spellid);
                                                    break;
                                                }
                                            case Action.Delete:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                    {
                                                        player.Owner.MySpells.Remove(spellid,stream);
                                                    }
                                                    break;
                                                }
                                            case Action.Downgrade:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                        player.Owner.MySpells.RebornSpell(stream,spellid);
                                                    break;
                                                }

                                        }
                                    }
                                }
                                player.FirstRebornLevel = (byte)player.Level;
                                player.FirstClass = player.Class;
                                player.Class = RebornClass;
                                player.Reborn = 1;
                                player.SendUpdate(stream,player.Reborn, Game.MsgServer.MsgUpdate.DataType.Reborn);
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + player.Name + " has got " + player.Reborn + " reborns.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                             
                               
                                break;
                            }
                        case 1:
                            {

                                foreach (var info in this)
                                {
                                    if (info.Item1 == 2 && info.Item2 == player.Class && info.Item3 == RebornClass)
                                    {
                                        switch (info.Item4)
                                        {
                                            case Action.AddRebornSpells:
                                            case Action.Add:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                        player.Owner.MySpells.Add(stream,spellid);
                                                    break;
                                                }
                                            case Action.Delete:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                    {
                                                        player.Owner.MySpells.Remove(spellid, stream);
                                                    }
                                                    break;
                                                }
                                            case Action.Downgrade:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                    {
                                                        player.Owner.MySpells.RebornSpell(stream,spellid);
                                                    }
                                                    break;
                                                }

                                        }
                                    }
                                }
                                player.SecoundeRebornLevel = (byte)player.Level;
                                player.SecoundeClass = player.Class;
                                player.Class = RebornClass;
                                player.Reborn = 2;
                                player.SendUpdate(stream,player.Reborn, Game.MsgServer.MsgUpdate.DataType.Reborn);
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + player.Name + " has got " + player.Reborn + " nd reborn.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                
                               
                                break;
                            }
                        case 2:
                            {
                                player.Owner.MySpells.ClearSpells(StaticSpells, stream);

                                byte RClass = 0;
                                if (player.SecoundeClass == 135)
                                    RClass = 132;
                                else if (player.SecoundeClass == 145)
                                    RClass = 142;
                                else
                                    RClass = (byte)(player.SecoundeClass - 4);

                                byte Twoclass = 0;
                                if (player.Class == 135)
                                    Twoclass = 132;
                                else if (player.Class == 145)
                                    Twoclass = 142;
                                else
                                    Twoclass = (byte)(player.Class - 4);

                                foreach (var info in this)
                                {
                                    if (info.Item1 == 0 && info.Item2 == 0 && info.Item3 == RClass)
                                    {
                                        switch (info.Item4)
                                        {
                                            case Action.AllClassSpells:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                    {
                                                        if (!StaticSpells.Contains(spellid))
                                                            player.Owner.MySpells.Add(stream,spellid);
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                }
                                foreach (var info in this)
                                {
                                    if (info.Item1 == 1 && info.Item2 == player.SecoundeClass && info.Item3 == Twoclass)
                                    {
                                        switch (info.Item4)
                                        {
                                            case Action.AddRebornSpells:
                                            case Action.Add:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                        player.Owner.MySpells.Add(stream,spellid);
                                                    break;
                                                }
                                            case Action.Delete:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                    {
                                                        player.Owner.MySpells.Remove(spellid, stream);
                                                    }
                                                    break;
                                                }
                                            case Action.Downgrade:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                    {
                                                        player.Owner.MySpells.RebornSpell(stream,spellid);
                                                    }
                                                    break;
                                                }

                                        }
                                    }
                                }
                                foreach (var info in this)
                                {
                                    if (info.Item1 == 0 && info.Item2 == 0 && info.Item3 == Twoclass)
                                    {
                                        switch (info.Item4)
                                        {
                                            case Action.AllClassSpells:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                    {
                                                        if (!StaticSpells.Contains(spellid))
                                                            player.Owner.MySpells.Add(stream,spellid);
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                }
                                foreach (var info in this)
                                {
                                    if (info.Item1 == 2 && info.Item2 == player.Class && info.Item3 == RebornClass)
                                    {
                                        switch (info.Item4)
                                        {
                                            case Action.AddRebornSpells:
                                            case Action.Add:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                        player.Owner.MySpells.Add(stream,spellid);
                                                    break;
                                                }
                                            case Action.Delete:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                    {
                                                        player.Owner.MySpells.Remove(spellid, stream);
                                                    }
                                                    break;
                                                }
                                            case Action.Downgrade:
                                                {
                                                    foreach (var spellid in info.Item5)
                                                    {
                                                        player.Owner.MySpells.RebornSpell(stream,spellid);
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                }
                                player.FirstRebornLevel = player.SecoundeRebornLevel;
                                player.SecoundeRebornLevel = (byte)player.Level;
                                player.FirstClass = player.SecoundeClass;
                                player.SecoundeClass = player.Class;
                                player.Class = RebornClass;
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + player.Name + " has got reincanation.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                                
                               
                                player.Reincarnation = true;
                                break;
                            }
                    }

                    try
                    {
                        foreach (var item in player.Owner.Equipment.ClientItems.Values)
                        {
                            if (item != null)
                            {
                                try
                                {
                                    if (item.Position != (ushort)Role.Flags.ConquerItem.AleternanteBottle
                                        && item.Position != (ushort)Role.Flags.ConquerItem.AleternanteGarment
                                        && item.Position != (ushort)Role.Flags.ConquerItem.Bottle
                                        && item.Position != (ushort)Role.Flags.ConquerItem.Fan
                                        && item.Position != (ushort)Role.Flags.ConquerItem.Garment
                                        && item.Position != (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                                        && item.Position != (ushort)Role.Flags.ConquerItem.RidingCrop
                                        && item.Position != (ushort)Role.Flags.ConquerItem.RightWeaponAccessory
                                        && item.Position != (ushort)Role.Flags.ConquerItem.Steed
                                        && item.Position != (ushort)Role.Flags.ConquerItem.SteedMount
                                        && item.Position != (ushort)Role.Flags.ConquerItem.Tower)
                                    {
                                        item.ITEM_ID = Database.Server.ItemsBase.DowngradeItem(item.ITEM_ID);
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(player.Owner, stream);
                                    }
                                }
                                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                            }
                        }
                    }
                    catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                    player.Owner.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                    player.Owner.Equipment.Remove(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);


                    player.Level = 15;
                    player.Experience = Database.Server.LevelInfo[Database.DBLevExp.Sort.User][(byte)(player.Level - 1)].Experience;
                    player.SendUpdate(stream,(long)player.Experience, Game.MsgServer.MsgUpdate.DataType.Experience);


                    ActionQuery action = new ActionQuery()
                    {
                        ObjId = player.UID,
                        Type = ActionType.Leveled,
                       dwParam = player.Level
                    };
                    player.Owner.Send(stream.ActionCreate(&action));

                
                    player.SendUpdate(stream,player.Level, Game.MsgServer.MsgUpdate.DataType.Level);
                    // Database.DataCore.AtributeStatus.GetStatus(player, true);

                    player.Strength = player.Agility = player.Spirit = 0;
                    player.Vitality = 1;
                    if (player.Reborn == 1)
                    {
                        player.Atributes = (ushort)(Database.Server.RebornInfo.ExtraAtributePoints(player.FirstRebornLevel, player.FirstClass)
                            + 52 + 3 * (player.Level - 15) + player.ExtraAtributes);
                    }
                    else
                    {
                        player.Atributes = (ushort)(Database.Server.RebornInfo.ExtraAtributePoints(player.FirstRebornLevel, player.FirstClass) +
                            Database.Server.RebornInfo.ExtraAtributePoints(player.SecoundeRebornLevel, player.SecoundeClass) + 52 + 3 * (player.Level - 15) + player.ExtraAtributes);
                    }
                    player.SendUpdate(stream,player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                    player.SendUpdate(stream,player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                    player.SendUpdate(stream,player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                    player.SendUpdate(stream,player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                    player.SendUpdate(stream,player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                    player.SendUpdate(stream,player.FirstClass, Game.MsgServer.MsgUpdate.DataType.FirsRebornClass);
                    player.SendUpdate(stream,player.SecoundeClass, Game.MsgServer.MsgUpdate.DataType.SecondRebornClass);
                    player.Owner.Equipment.QueryEquipment(player.Owner.Equipment.Alternante, true);
                    Database.PrestigeRanking.CheckReborn(player.Owner);

                    var client = player.Owner;

                    client.Player.RemoveFlag(MsgUpdate.Flags.ChillingSnow);
                    client.Player.RemoveFlag(MsgUpdate.Flags.HealingSnow);


                    if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);

                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);

                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
                    }
                    else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AngerofStomper))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AngerofStomper);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.JusticeChant))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.JusticeChant);

                     
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Circle))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Circle);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Rectangle))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Rectangle);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Sector))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Sector);
                    }
                    else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Superman))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Superman);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Shield))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roar))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Roar);
                    }
                    else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                    {
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.XpFly);
                    }
                    else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalStrike))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FatalStrike);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ToxicFog);
                    }
                    else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WhirlwindKick))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WhirlwindKick);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleAttack))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TripleAttack);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Oblivion))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Oblivion);
                    }
                    else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CannonBarrage))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CannonBarrage);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeTempest))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BladeTempest);
                    }
                    else if (Database.AtributesStatus.IsLee(client.Player.Class))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPunch))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonPunch);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonCyclone))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonCyclone);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFlow))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonFlow);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirRaid))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirRaid);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirSweep))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirSweep);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirKick))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirKick);

                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirStrike))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirStrike);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthSweep))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.EarthSweep);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Kick))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Kick);
                    }
                    else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ChainBolt);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Lightning);

                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);

                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);
                    }
                
                }
            }
        
    }
}
