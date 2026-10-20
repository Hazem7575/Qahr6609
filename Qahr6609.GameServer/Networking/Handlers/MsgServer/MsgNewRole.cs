using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgNewRole
    {

        public static object SynName = new object();


        public static void GetNewRoleInfo(this ServerSockets.Packet msg, out string name, out ushort Body, out byte Class)
        {
            msg.ReadBytes(20);
            name = msg.ReadCString(16);
            msg.ReadBytes(32);
            Body = msg.ReadUInt16();
            Class = msg.ReadUInt8();

        }

        [PacketAttribute(Game.GamePackets.NewClient)]
        public unsafe static void CreateCharacter(Client.GameClient client, ServerSockets.Packet stream)
        {
            if ((client.ClientFlag & Client.ServerFlag.CreateCharacter) == Client.ServerFlag.CreateCharacter)
            {
                Client.GameClient INGAME;
                if (Database.Server.GamePoll.TryGetValue(client.ConnectionUID, out INGAME))
                {
                    INGAME.Socket.Disconnect();
                    client.Socket.Disconnect();
                    return;
                }
                client.ClientFlag &= ~Client.ServerFlag.AcceptLogin;
                string CharacterName; ushort Body; byte Class;

                stream.GetNewRoleInfo(out CharacterName, out Body, out Class);

                //last update
                byte attackType = 0;
                switch (Class)
                {
                    case 0:
                    case 1: Class = 100; break;
                    case 2:
                    case 3: Class = 10; break;
                    case 4:
                    case 5: Class = 40; break;
                    case 6:
                    case 7: Class = 20; break;
                    case 8:
                    case 9: Class = 50; break;
                    case 10:
                    case 11: Class = 60; break;
                    case 12:
                    case 13: Class = 70; break;
                    case 14:
                    case 15: Class = 80; break;

                    case 16:
                    case 17:
                        {
                            attackType = 0;
                            Class = 160;
                            break;
                        }
                    case 18:
                    case 19:
                        {
                            attackType = 1;
                            Class = 160;
                            break;
                        }
                }
                

                if (!ExitBody(Body))
                {
                    client.Send(new MsgServer.MsgMessage("AHAHAH! WRONG Body, NICE TRY", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));

                    return;
                }
                if (!ExitClass(Class))
                {
                    client.Send(new MsgServer.MsgMessage("AHAHAH! WRONG Class, NICE TRY", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));

                    return;
                }

                CharacterName = CharacterName.Replace("\0", "\0");
                if (Program.NameStrCheck(CharacterName))
                {
                    // check and reserve the name in one step so two creations can't take the same name
                    bool nameReserved = false;
                    lock (Database.Server.NameUsed)
                    {
                        if (!Database.Server.NameUsed.Contains(CharacterName.GetHashCode()))
                        {
                            Database.Server.NameUsed.Add(CharacterName.GetHashCode());
                            nameReserved = true;
                        }
                    }
                    if (nameReserved)
                    {
                        client.ClientFlag &= ~Client.ServerFlag.CreateCharacter;
                    
                        client.Player.Name = CharacterName;
                        client.Player.Class = Class;
                        client.Player.Body = Body;
                        client.Player.Level = 1;
                        client.Player.Map = 1002;
                        client.Player.X = 248;
                        client.Player.Y = 238;
                        Database.DataCore.LoadClient(client.Player);

                        client.Player.UID = client.ConnectionUID;
                        if (attackType == 1)
                            client.Player.MainFlag |= Role.Player.MainFlagType.OnMeleeAttack;

                        Database.DataCore.AtributeStatus.GetStatus(client.Player);

                    
                        Database.DataCore.SetCharacterSides(client.Player);
                        byte Color = (byte)Program.GetRandom.Next(4, 8);
                        client.Player.Hair = (ushort)(Color * 100 + 10 + (byte)Program.GetRandom.Next(4, 9));



                        if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                        {
                            client.Equipment.Add(stream, 152005, Role.Flags.ConquerItem.Ring);
                            client.Equipment.Add(stream, 421301, Role.Flags.ConquerItem.RightWeapon);
                        }

                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            client.Equipment.Add(stream, 150003, Role.Flags.ConquerItem.Ring);
                            client.Equipment.Add(stream, 500006, Role.Flags.ConquerItem.RightWeapon);
                        }
                        else
                        {
                            client.Equipment.Add(stream, 150003, Role.Flags.ConquerItem.Ring);
                            if (Database.AtributesStatus.IsPirate(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 611301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 612301, Role.Flags.ConquerItem.LeftWeapon);
                            }
                            else if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 420301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 420301, Role.Flags.ConquerItem.LeftWeapon);
                            }
                            
                            else if (Database.AtributesStatus.IsLee(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 617301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 617301, Role.Flags.ConquerItem.LeftWeapon);
                                client.Equipment.Add(stream, 138313, Role.Flags.ConquerItem.Armor);
                            }
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 610301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 610301, Role.Flags.ConquerItem.LeftWeapon);
                            }
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 601301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 601301, Role.Flags.ConquerItem.LeftWeapon);
                            }
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                            {
                                
                                client.Equipment.Add(stream, 101319, Role.Flags.ConquerItem.Armor);
                                client.Equipment.Add(stream, 626301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 626301, Role.Flags.ConquerItem.LeftWeapon);
                            }
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 561301, Role.Flags.ConquerItem.RightWeapon);
                            }
                            else
                                client.Equipment.Add(stream, 410301, Role.Flags.ConquerItem.RightWeapon);
                        }
                        if (!Database.AtributesStatus.IsLee(client.Player.Class) && !Database.AtributesStatus.IsWindWalker(client.Player.Class))
                            client.Equipment.Add(stream, 132009, Role.Flags.ConquerItem.Armor);

                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);

                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);

                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
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

                        client.Player.Money += 1000;

                        client.Player.BoundConquerPoints += 1075;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, MsgServer.MsgUpdate.DataType.BoundConquerPoints);
                        client.Inventory.Add(stream, 727266, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 117089, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//earring
                        client.Inventory.Add(stream, 201009, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 202009, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 203009, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 727267, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        #region Stuff
                        /*if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 480439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Clup
                            client.Inventory.Add(stream, 480439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Clup
                            client.Inventory.Add(stream, 614439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//ShadowCrossSaber
                            client.Inventory.Add(stream, 614439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//ShadowCrossSaber
                            client.Inventory.Add(stream, 490439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Dagger
                            client.Inventory.Add(stream, 410439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Blade
                            client.Inventory.Add(stream, 420439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Sword
                            client.Inventory.Add(stream, 130309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//ArmorTrojan
                            client.Inventory.Add(stream, 118309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//CapTrojan
                            client.Inventory.Add(stream, 120269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Necklace
                            client.Inventory.Add(stream, 150269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Ring
                            client.Inventory.Add(stream, 160249, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Boot
                            client.Inventory.Add(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Steed
                            client.Inventory.Add(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Crop
                            client.Inventory.Add(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true);//Tower
                            client.Inventory.Add(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true);//Fan
                            client.Inventory.Add(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true);//Wing
                        }
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 624439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Fist
                            client.Inventory.Add(stream, 624439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Fist
                            client.Inventory.Add(stream, 560439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Spear
                            client.Inventory.Add(stream, 900309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Shield
                            client.Inventory.Add(stream, 131309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//ArmorWarrior
                            client.Inventory.Add(stream, 111309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//CapWarrior
                            client.Inventory.Add(stream, 120269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Necklace
                            client.Inventory.Add(stream, 150269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Ring
                            client.Inventory.Add(stream, 160249, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Boot
                            client.Inventory.Add(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Steed
                            client.Inventory.Add(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Crop
                            client.Inventory.Add(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true);//Tower
                            client.Inventory.Add(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true);//Fan
                            client.Inventory.Add(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true);//Wing
                        }
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 613429, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Knife
                            client.Inventory.Add(stream, 613429, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Knife
                            client.Inventory.Add(stream, 500429, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Bow
                            client.Inventory.Add(stream, 133309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//ArmorArcher
                            client.Inventory.Add(stream, 113309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//CapArcher
                            client.Inventory.Add(stream, 120269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Necklace
                            client.Inventory.Add(stream, 150269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Ring
                            client.Inventory.Add(stream, 160249, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Boot
                            client.Inventory.Add(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Steed
                            client.Inventory.Add(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Crop
                            client.Inventory.Add(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true);//Tower
                            client.Inventory.Add(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true);//Fan
                            client.Inventory.Add(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true);//Wing
                        }
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 511439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Scythe
                            client.Inventory.Add(stream, 616439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Katana
                            client.Inventory.Add(stream, 616439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Katana
                            client.Inventory.Add(stream, 601439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Epic
                            client.Inventory.Add(stream, 601439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Epic
                            client.Inventory.Add(stream, 135309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//ArmorNinja
                            client.Inventory.Add(stream, 123309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//NinjaHood
                            client.Inventory.Add(stream, 120269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Necklace
                            client.Inventory.Add(stream, 150269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Ring
                            client.Inventory.Add(stream, 160249, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Boot
                            client.Inventory.Add(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Steed
                            client.Inventory.Add(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Crop
                            client.Inventory.Add(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true);//Tower
                            client.Inventory.Add(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true);//Fan
                            client.Inventory.Add(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true);//Wing
                        }
                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 622439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//AlmightyEmperor
                            client.Inventory.Add(stream, 622439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//AlmightyEmperor
                            client.Inventory.Add(stream, 610439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Beads
                            client.Inventory.Add(stream, 610439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Beads
                            client.Inventory.Add(stream, 136309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//ArmorMonk
                            client.Inventory.Add(stream, 143309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//CapMonk
                            client.Inventory.Add(stream, 120269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Necklace
                            client.Inventory.Add(stream, 150269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Ring
                            client.Inventory.Add(stream, 160249, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Boot
                            client.Inventory.Add(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Steed
                            client.Inventory.Add(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Crop
                            client.Inventory.Add(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true);//Tower
                            client.Inventory.Add(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true);//Fan
                            client.Inventory.Add(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true);//Wing
                        }
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 611439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Rapier
                            client.Inventory.Add(stream, 612439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Pistol
                            client.Inventory.Add(stream, 139309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//ArmorPirate
                            client.Inventory.Add(stream, 144309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//CapPirate
                            client.Inventory.Add(stream, 120269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Necklace
                            client.Inventory.Add(stream, 150269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Ring
                            client.Inventory.Add(stream, 160249, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Boot
                            client.Inventory.Add(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Steed
                            client.Inventory.Add(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Crop
                            client.Inventory.Add(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true);//Tower
                            client.Inventory.Add(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true);//Fan
                            client.Inventory.Add(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true);//Wing
                        }
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 619439, 1, 12, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Housu
                            client.Inventory.Add(stream, 620439, 1, 12, 7, 200, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, true);//BackSword
                            client.Inventory.Add(stream, 134309, 1, 12, 7, 200, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, true);//ArmorFire
                            client.Inventory.Add(stream, 114309, 1, 12, 7, 200, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, true);//CapFire
                            client.Inventory.Add(stream, 152279, 1, 12, 7, 200, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, true);//Bracelet
                            client.Inventory.Add(stream, 121269, 1, 12, 7, 200, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, true);//Bag
                            client.Inventory.Add(stream, 160249, 1, 12, 7, 200, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, true);//Boot
                            client.Inventory.Add(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Steed
                            client.Inventory.Add(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Crop
                            client.Inventory.Add(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true);//Tower
                            client.Inventory.Add(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true);//Fan
                            client.Inventory.Add(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true);//Wing
                        }
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 617439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//SkyNunchaku
                            client.Inventory.Add(stream, 617439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//SkyNunchaku
                            client.Inventory.Add(stream, 138309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Bruce_LeeArmor
                            client.Inventory.Add(stream, 148309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Bruce_LeeHood
                            client.Inventory.Add(stream, 120269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Necklace
                            client.Inventory.Add(stream, 150269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Ring
                            client.Inventory.Add(stream, 160249, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Boot
                            client.Inventory.Add(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Steed
                            client.Inventory.Add(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Crop
                            client.Inventory.Add(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true);//Tower
                            client.Inventory.Add(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true);//Fan
                            client.Inventory.Add(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true);//Wing
                        }
                        else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 626439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Fan
                            client.Inventory.Add(stream, 626439, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Fan
                            client.Inventory.Add(stream, 101309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Armoer
                            client.Inventory.Add(stream, 170309, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Headgear
                            client.Inventory.Add(stream, 120269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Necklace
                            client.Inventory.Add(stream, 150269, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Ring
                            client.Inventory.Add(stream, 160249, 1, 12, 7, 200, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Boot
                            client.Inventory.Add(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Steed
                            client.Inventory.Add(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//Crop
                            client.Inventory.Add(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true);//Tower
                            client.Inventory.Add(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true);//Fan
                            client.Inventory.Add(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true);//Wing
                        }*/
                        #endregion
                        client.Inventory.Add(stream, 3000550, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None);
                        client.Inventory.Add(stream, 3001027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Send(new MsgServer.MsgMessage("ANSWER_OK", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));

                        client.Status.MaxHitpoints = client.CalculateHitPoint();
                        client.Player.HitPoints = (int)client.Status.MaxHitpoints;


                        client.ClientFlag |= Client.ServerFlag.CreateCharacterSucces;
                    }
                    else
                    {
                          client.Send(new MsgServer.MsgMessage("The name is in use! try other name", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
                      
                    }
                }
                else
                {
                    client.Send(new MsgServer.MsgMessage("Invalid characters name!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
                   
                }
            }
        }

        public static bool ExitBody(ushort _body)
        {
            return (_body == 1003 || _body == 1004 || _body == 2001 || _body == 2002);
        }

        public static bool ExitClass(byte cls)
        {
            return (cls == 10 || cls == 20 || cls == 40 
                || cls == 50 || cls == 60 || cls == 70 || cls == 100 || cls == 80 || cls == 160);
        }
    }
}
