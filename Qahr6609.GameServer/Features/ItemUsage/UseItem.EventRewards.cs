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
        // UseItem handlers: Elite PK / Skill Team / Team PK / Tower of Mistery rewards, lottery, demon boxes, boss summons, fireworks.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        // HorseRacingPointsPack (720874-720881): consume one pack, add the points and refresh the Race Point shop balance.
        static void OpenRacePointsPack(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, uint points)
        {
            if (!client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                return;
            client.Player.RacePoints += points;
            client.Player.SendUpdate(stream, client.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);
            client.CreateBoxDialog("You received " + points.ToString("N0") + " Horse Racing Points.");
        }

        private static void UseItem_EventRewards(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
                case 720842://SwordSoul
                    {
                        if (client.Player.Level >= 99)
                        {
                            if (client.Player.SwordMaster > 0)
                            {
                                client.SendSysMesage("sorry, but you really spawned Sword Master today.");
                                return;
                            }
                            if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                            {
                                if (!client.Map.ContainMobID(6643, client.Player.UID))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    Database.Server.AddMapMonster(stream, client.Map, 6643, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                                    client.Player.SwordMaster++;
                                }
                                else
                                {
                                    client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");

                                }
                            }
                            else
                            {
                                client.CreateDialog(stream, "You have to be in your own house to be able to display it", "I~see.");

                            }
                        }
                        else
                        {
                            client.CreateDialog(stream, "Sorry,~you~cannot~open~this~monster~before~your~Level~is~99.~Please~train~harder.", "I~see.");

                        }
                        break;
                    }
                case 720030://firework
                    {
                        if (client.Player.Stamina >= 100)
                        {
                            //client.Player.SendString(MsgStringPacket.StringID.Fireworks, true, new string[1] { "1123" });

                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Player.Stamina -= 100;
                            client.Player.SendUpdate(stream, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                        }
                        break;
                    }
                case 720031://endleslove
                    {
                        if (client.Player.Stamina >= 100)
                        {
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-1love");
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Player.Stamina -= 100;
                            client.Player.SendUpdate(stream, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                        }
                        break;
                    }
                case 720032://mywish
                    {
                        if (client.Player.Stamina >= 100)
                        {
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-2love");
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Player.Stamina -= 100;
                            client.Player.SendUpdate(stream, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                        }
                        break;
                    }
                case 721261://bomb
                    {
                        if (client.Player.Map == 1038)
                        {
                            if (client.Player.View.Contains(MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate]))
                            {
                                // the bomb is used up only when it really hit the gate
                                if (MsgTournaments.MsgSchedules.GuildWar.Bomb(stream, client, Role.SobNpc.StaticMesh.LeftGate))
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                            if (client.Player.View.Contains(MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate]))
                            {
                                // the bomb is used up only when it really hit the gate
                                if (MsgTournaments.MsgSchedules.GuildWar.Bomb(stream, client, Role.SobNpc.StaticMesh.RightGate))
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                break;
                            }
                        }
                        else
                        {
                            client.SendSysMesage("Use this on gate in Guild Arena");

                        }
                        break;
                    }
                case 724002://LotteryTickets
                    {
                        if (client.Inventory.HaveSpace(0))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(711504, 0, 3, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "lottery");
                            client.SendSysMesage("You opened the Small Lottery Ticket Packet and received 3 Small Lottery Tickets!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }
                case 720880: //HorseRacingPointsPack(3K)
                    {
                        OpenRacePointsPack(item, client, stream, 3000);
                        break;
                    }
                case 729304://chitoken
                    {

                        if (client.Inventory.Contain(729304, 7))
                        {
                            if (client.Player.UseChiToken == 0)
                            {
                                client.Player.UseChiToken = 1;

                                client.Inventory.Remove(729304, 7, stream);
                                client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.AddChampionPoints(100);
                                client.Player.MyChi.ChiPoints += 200;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                client.SendSysMesage("You received 180 min exp, 100 Champion Pts and 200 ChiPoints.", MsgMessage.ChatMode.System);
                            }
                            else
                                client.SendSysMesage("You`ve~swapped~for~the~reward~for~twice,~today.~Please~try~again~tomorrow.");
                        }
                        else
                        {
                            client.SendSysMesage("Sorry, but you'll need to collect 7 Chi Tokens, first.", MsgMessage.ChatMode.System);

                        }
                        break;
                    }
                case 3100011:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please make 1 more space in inventory.");

                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        byte rand = (byte)Program.GetRandom.Next(0, 7);
                        switch (rand)
                        {
                            case 1:
                            case 0:
                                {
                                    if (client.Player.SubClass != null)
                                    {
                                        client.Player.SubClass.AddStudyPoints(client, 300, stream);
                                        client.CreateBoxDialog("You received 300 StudyPoints.");

                                    }
                                    break;
                                }
                            case 2:
                                {
                                    client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 200;
                                    Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                    client.CreateBoxDialog("You received 200 ChiPoints.");

                                    break;
                                }
                            default:
                                {
                                    client.Inventory.Add(stream, 3600023, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.CreateBoxDialog("You received EminentExploitPack.");

                                    break;
                                }
                        }
                        break;
                    }
                case 3100014:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.SendSysMesage("Please make 2 more space in inventory.");

                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                        client.Inventory.Add(stream, 3600023, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 3100012, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                        client.CreateBoxDialog("You received EminentExploitPack and SeniorSign-inBox.");
                        break;
                    }
                case 3100015:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.SendSysMesage("Please make 2 more space in inventory.");

                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 730003, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 3100012, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 800;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received EminentExploitPack, +3Stone(B) and 800ChiPoints.");

                        break;
                    }
                case 3100016:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.SendSysMesage("Please make 2 more space in inventory.");

                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3100012, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, Database.ItemType.DragonBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 1000;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received EminentExploitPack, DragonBall(B) and 1000ChiPoints.");

                        break;
                    }
                #region Elite Pk
                #region Required items
                case 720836: //WeaponAccessoryPack
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        var RndAccessory = Database.ItemType.Accessorys;
                        var Position = Program.GetRandom.Next(0, RndAccessory.Values.Count);
                        var ReceiveItem = RndAccessory.Values.ToArray()[Position];
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, ReceiveItem.ID);
                        client.CreateBoxDialog("You`ve received 1 random accessory.");

                        break;
                    }
                case 720713: //ElitePKEXPPack
                    {

                        if (client.Player.Level == 140)
                        {
                            client.CreateBoxDialog("You already achieved the highest level possible.");
                            return;
                        }

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.CreateBoxDialog("You`ve~~received~EXP~worth~an~EXP~Ball.");
                        client.GainExpBall(600, true, Role.Flags.ExperienceEffect.bless);
                        break;
                    }

                case 720730: //PowerEXPSplinter
                    {

                        if (client.Player.Level == 140)
                        {
                            client.CreateBoxDialog("You already achieved the highest level possible.");
                            return;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.CreateBoxDialog("You`ve~received~1%~of~your~current~level`s~EXP.");
                        client.GainExpBall(6000, true, Role.Flags.ExperienceEffect.bless);
                        break;
                    }


                case 728596: //EXPBallFragment
                    {

                        if (client.Player.Level == 140)
                        {
                            client.CreateBoxDialog("You already achieved the highest level possible.");
                            return;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.CreateBoxDialog("You`ve~received~1%~of~your~current~level`s~EXP.");
                        client.GainExpBall(6000, true, Role.Flags.ExperienceEffect.bless);
                        break;
                    }


                case 720894: //EnduranceBookPack(5)
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 723341, 5, 1);
                            client.CreateBoxDialog("You have received 5 Endurance Books.");

                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");
                        }
                        break;
                    }

                case 728920: //EnduranceBookPack(4)
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 723341, 4, 1);
                            client.CreateBoxDialog("You have received 4 Endurance Books.");

                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 4 more spaces in your inventory.");
                        }
                        break;
                    }



                #endregion



                #region Level 130+
                case 720717: //SuperElitePKChampionPack
                    {
                        if (client.Inventory.HaveSpace(17))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 728527, 1, 1); // Steed + 6
                            client.Inventory.AddItemWitchStack(720730, 0, 3, stream, true); //Power Exp Splinter
                            client.Inventory.Add(stream, 720836, 5, 1); //weapon accessory packs
                            client.Inventory.Add(stream, 3004249, 5, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 3 Dragon Balls
                            client.Inventory.AddItemWitchStack(3004181, 0, 5, stream); //Savage Bone 
                            client.CreateBoxDialog("You've received +6 Random Steed Pack - 3 Power EXP Splinters(B) - 5 Weapon Accessory Packs - 5 Super Refinery Packs - 3 Dragon Balls - 5 SavageBones &  Wings of Infernal Beast.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 17 more spaces in your inventory.");

                        }

                        break;
                    }

                case 720721: //SuperElitePKChampionPack
                    {
                        if (client.Inventory.HaveSpace(10))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.AddItemWitchStack(728596, 0, 1, stream, true); //EXP Ball Fragment(B)
                            client.Inventory.Add(stream, 720836, 3, 1); //weapon accessory packs
                            client.Inventory.Add(stream, 3004249, 3, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // Dragon Balls
                            client.Inventory.AddItemWitchStack(3004181, 0, 2, stream); //Savage Bone 
                            client.Inventory.Add(stream, 723342, 1, 1); //ModestyBook
                            client.CreateBoxDialog("1 EXP Ball Fragment(B) - 3 Weapon Accessory Packs  - 3 Super Refinery Packs - 1 Dragon Ball - 2 SavageBones - 1 Modesty Book.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 10 more spaces in your inventory.");

                        }

                        break;
                    }

                case 720725: //SuperElitePK3rdPlacePack
                    {
                        if (client.Inventory.HaveSpace(7))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 5, stream, true); //5 EXP Balls(B)
                            client.Inventory.Add(stream, 720836, 2, 1); //weapon accessory packs
                            client.Inventory.Add(stream, 3004249, 2, 1); //Super Refinery Packs
                            client.Inventory.AddItemWitchStack(3004181, 0, 1, stream); //Savage Bone 
                            client.Inventory.Add(stream, 723342, 1, 1); //ModestyBook
                            client.CreateBoxDialog("5 EXP Balls(B) - 2 Weapon Accessory Packs - 2 Super Refinery Packs - 1 Savage Bone - 1 Modesty Book.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 7 more spaces in your inventory.");

                        }

                        break;
                    }

                case 720729: //SuperElitePKTop8Pack
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 3, stream, true); //3 EXP Balls(B)
                            client.Inventory.Add(stream, 720836, 1, 1); //weapon accessory packs
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.AddItemWitchStack(3004181, 0, 1, stream); //Savage Bone 
                            client.Inventory.AddItemWitchStack(723341, 0, 5, stream); //Endurance Books
                            client.CreateBoxDialog("3 EXP Balls(B) - 1 Weapon Accessory Pack - 1 Super Refinery Pack - 1 Savage Bone - 5 Endurance Books.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");

                        }

                        break;
                    }


                #endregion
                #region Level 120 - 129
                case 720716: // EliteElitePKChampionPack
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                            client.Inventory.AddItemWitchStack(723341, 0, 5, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 720836, 3, 1); //weapon accessory packs
                            client.CreateBoxDialog("1 Dragon Ball - 5 Endurance Books - 1 Super Refinery pack - 3 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 6 more spaces in your inventory.");

                        }

                        break;
                    }

                case 720720: // EliteElitePK2ndPlacePack
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.AddItemWitchStack(723341, 0, 3, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 720836, 2, 1); //weapon accessory packs
                            client.CreateBoxDialog("3 Endurance Books - 1 Super Refinery Pack - 2 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");

                        }

                        break;
                    }

                case 720724: // EliteElitePK3rdPlacePack
                    {
                        if (client.Inventory.HaveSpace(3))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.AddItemWitchStack(723341, 0, 2, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 720836, 1, 1); //weapon accessory packs
                            client.CreateBoxDialog("2 Endurance Books - 1 Super Refinery Pack - 1 Weapon Accessory Pack.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 3 more spaces in your inventory.");

                        }

                        break;
                    }

                case 720728: // EliteElitePKTop8Pack
                    {
                        if (client.Inventory.HaveSpace(2))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(723341, 0, 1, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.CreateBoxDialog("1 Endurance Book - 1 Super Refinery Pack.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 2 more spaces in your inventory.");

                        }

                        break;
                    }


                #endregion
                #region Level 100 - 119
                case 720715: //UniqueElitePKChampionPack
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 1 Dragon Balls
                            client.Inventory.AddItemWitchStack(723341, 0, 5, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 720836, 3, 1); //weapon accessory packs
                            client.CreateBoxDialog("1 Dragon Ball - 5 Endurance Books - 1 Super Refinery Pack - 3 Weapon Accessory Packs.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 4 more spaces in your inventory.");

                        }

                        break;
                    }

                case 720719: //UniqueElitePK2ndPlacePack
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);


                            client.Inventory.AddItemWitchStack(723341, 0, 3, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 720836, 2, 1); //weapon accessory packs
                            client.CreateBoxDialog("3 Endurance Books - 1 Super Refinery Pack - 2 Weapon Accessory Packs.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 4 more spaces in your inventory.");

                        }

                        break;
                    }


                case 720723: //UniqueElitePK3rdPlacePack
                    {
                        if (client.Inventory.HaveSpace(3))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);


                            client.Inventory.AddItemWitchStack(723341, 0, 2, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 720836, 1, 1); //weapon accessory packs
                            client.CreateBoxDialog("2 Endurance Books - 1 Super Refinery Pack - 1 Weapon Acessory Pack.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 3 more spaces in your inventory.");

                        }

                        break;
                    }

                case 720727: //UniqueElitePKTop8Pack
                    {
                        if (client.Inventory.HaveSpace(2))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);


                            client.Inventory.AddItemWitchStack(723341, 0, 1, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.CreateBoxDialog("1 Endurance Book - 1 Super Refinery Pack.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 2 more spaces in your inventory.");

                        }

                        break;
                    }


                #endregion
                #region Level ~100
                case 720714: //RefinedElitePKChampionPack
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 1 Dragon Balls
                            client.Inventory.AddItemWitchStack(723341, 0, 5, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 720836, 3, 1); //weapon accessory packs

                            client.CreateBoxDialog("You've received 1 Dragon Ball - 5 Endurance Books - 1 Super Refinery Pack - 3 Weapon Accessory Packs.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 4 more spaces in your inventory.");

                        }

                        break;
                    }

                case 720718: //RefinedElitePK2ndPlacePack
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(723341, 0, 3, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 720836, 2, 1); //weapon accessory packs

                            client.CreateBoxDialog("You've received 3 Endurance Books - 1 Super Refinery Pack - 2 Weapon Acessory Packs.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 4 more spaces in your inventory.");

                        }

                        break;
                    }


                case 720722: //RefinedElitePK3rdPlacePack
                    {
                        if (client.Inventory.HaveSpace(3))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(723341, 0, 2, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 720836, 1, 1); //weapon accessory packs

                            client.CreateBoxDialog("You've received 2 Endurance Books - 1 Super Refinery Pack - 1 Weapon Acessory Pack.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 3 more spaces in your inventory.");

                        }

                        break;
                    }

                case 720726: //RefinedElitePKTop8Pack
                    {
                        if (client.Inventory.HaveSpace(2))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(723341, 0, 1, stream); //Endurance Books
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs

                            client.CreateBoxDialog("You've received 1 Endurance book - 1 Super Refinery Pack.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 2 more spaces in your inventory.");

                        }

                        break;
                    }

                #endregion


                #endregion
                #region Skill Team
                #region Required
                case 720981:
                    {

                        if (client.Player.Level == 140)
                        {
                            client.CreateBoxDialog("You already achieved the highest level possible.");
                            return;
                        }

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.CreateBoxDialog("You`ve~received~EXP~worth~an~EXP~Ball.");
                        client.GainExpBall(600, true, Role.Flags.ExperienceEffect.bless);
                        break;
                    }
                case 720890: //ModestyBookPack(5)
                    {

                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 723342, 5, 1); //ModestyBook
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");
                        }
                        break;
                    }



                case 720882: //LotteryTicketPack(Big)
                    {

                        if (client.Inventory.HaveSpace(5))
                        {

                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 724002, 5, 1); //SmallLotteryTicketPack
                            client.CreateBoxDialog("You have received 5 SmallLotteryTicketPack. ");
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");
                        }
                        break;
                    }


                case 720889: //LotteryTicketPack(3)
                    {

                        if (client.Inventory.HaveSpace(3))
                        {

                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 724002, 3, 1); //SmallLotteryTicketPack
                            client.CreateBoxDialog("You have received 3 SmallLotteryTicketPack. ");
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 3 more spaces in your inventory.");
                        }
                        break;
                    }


                case 721318:
                    {

                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 727464, 1, 1); //EXPBallGiftBox
                            client.CreateBoxDialog("You have received 1 ExpBallGift Box.");
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                        }
                        break;
                    }


                case 3003628: //SkillProfiencyPack(1)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        byte randskillbook = (byte)Program.GetRandom.Next(1, 34);
                        uint itemz = 0;
                        switch (randskillbook)
                        {
                            case 1: itemz = 721157; break;
                            case 2: itemz = 721158; break;
                            case 3: itemz = 725005; break;
                            case 4: itemz = 725010; break;
                            case 5: itemz = 725029; break;
                            case 6: itemz = 725042; break;
                            case 7: itemz = 725030; break;
                            case 8: itemz = 725040; break;
                            case 9: itemz = 725041; break;
                            case 10: itemz = 725043; break;
                            case 11: itemz = 725044; break;
                            case 12: itemz = 725013; break;
                            case 13: itemz = 725026; break;
                            case 14: itemz = 725012; break;
                            case 15: itemz = 725011; break;
                            case 16: itemz = 725027; break;
                            case 17: itemz = 725014; break;
                            case 18: itemz = 725031; break;
                            case 19: itemz = 725000; break;
                            case 20: itemz = 725001; break;
                            case 21: itemz = 725002; break;
                            case 22: itemz = 725003; break;
                            case 23: itemz = 725015; break;
                            case 24: itemz = 725004; break;
                            case 25: itemz = 725028; break;
                            case 26: itemz = 725025; break;
                            case 27: itemz = 725016; break;
                            case 28: itemz = 725018; break;
                            case 29: itemz = 725019; break;
                            case 30: itemz = 725020; break;
                            case 31: itemz = 725021; break;
                            case 32: itemz = 725022; break;
                            case 33: itemz = 725023; break;
                            default: itemz = 725024; break;
                        }
                        uint randomitem = itemz;
                        client.CreateBoxDialog("You've received 1 Random Skill Book.");
                        client.Inventory.Add(stream, randomitem, 1, 1); // 


                        break;
                    }
                #endregion
                #region Level 130+
                case 721303: // rank 1
                    {
                        if (client.Inventory.HaveSpace(15))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 5, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 5 Dragon Balls
                            client.Inventory.Add(stream, 720890, 1, 1); //ModestyBookPack(5)
                            client.Inventory.Add(stream, 700072, 5, 1); // 5 Refined Tortoise Gems
                            client.Inventory.Add(stream, 721318, 1, 1); //EXPLuxuriousPack
                            client.Inventory.Add(stream, 720882, 1, 1); //1 LotteryTicket Pack (Big)
                            client.Inventory.Add(stream, 3004249, 5, 1); //Super Refinery Packs


                            client.CreateBoxDialog("You've received 5 Dragon Balls - 1 ModestyBook Pack(5) - 5 Refined Tortoise Gems - 1 EXP Luxurious Pack - 1 Lottery Ticket Pack(BIG) - 5 Refinery Packs.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 15 more spaces in your inventory.");

                        }
                        break;
                    }

                case 721307: // rank 2
                    {
                        if (client.Inventory.HaveSpace(14))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 3 Dragon Balls
                            client.Inventory.Add(stream, 727834, 1, 1); //ModestyBookPack(4)
                            client.Inventory.Add(stream, 700072, 5, 1); // 5 Refined Tortoise Gems
                            client.Inventory.Add(stream, 721318, 1, 1); //EXPLuxuriousPack
                            client.Inventory.Add(stream, 720882, 1, 1); //1 LotteryTicket Pack (Big)
                            client.Inventory.Add(stream, 3004249, 3, 1); //Super Refinery Packs


                            client.CreateBoxDialog("3 Dragon Balls - 4 Modesty Books - 5 Refined Tortoise Gems - 1 EXP Luxurious Pack - 1 LottertTicket Pack(BIG) - 3 Refinery Packs.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 14 more spaces in your inventory.");

                        }
                        break;
                    }

                case 721311: // rank 3
                    {
                        if (client.Inventory.HaveSpace(8))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 
                            client.Inventory.AddItemWitchStack(723342, 0, 3, stream); // Modesty Books
                            client.Inventory.Add(stream, 721318, 1, 1); //EXPLuxuriousPack
                            client.Inventory.Add(stream, 720882, 1, 1); //1 LotteryTicket Pack (Big)
                            client.Inventory.Add(stream, 3004249, 3, 1); //Super Refinery Packs


                            client.CreateBoxDialog("1 Dragon Ball - 3 Modesty Books - 1 EXP Luxurious Pack - 1 Lottert Ticket Pack (BIG) - 3 Refinery Packs.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 8 more spaces in your inventory.");

                        }
                        break;
                    }


                case 721315: //~top~8~
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.AddItemWitchStack(723342, 0, 2, stream); // Modesty Books
                            client.Inventory.Add(stream, 721318, 1, 1); //EXPLuxuriousPack
                            client.Inventory.Add(stream, 720882, 1, 1); //1 LotteryTicket Pack (Big)
                            client.Inventory.Add(stream, 3004249, 3, 1); //Super Refinery Packs


                            client.CreateBoxDialog("2 Modesty books - 1 EXP Luxourious Pack - 1 Lottery Ticket Pack(BIG) - 3 Refinery Packs.");

                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 6 more spaces in your inventory.");

                        }
                        break;
                    }

                #endregion

                #region Level 120 - 129
                case 721302: // rank 1
                    {
                        if (client.Inventory.HaveSpace(7))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720894, 1, 1);  //Endurance Book Pack(5)
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3004249, 3, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 1 Endurance Book Pack(5) - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) - 3 Refinery Packs & 1 Skill Profiency Pack(1).");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 7 more spaces in your inventory.");

                        }
                        break;
                    }

                case 721306: // rank 2
                    {
                        if (client.Inventory.HaveSpace(7))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 728920, 1, 1);  //Endurance Book Pack(4)
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3004249, 2, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 1 Endurance Book Pack(4) - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) - 2 Refinery Packs and 1 Skill Profiency Pack(1)  .");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 7 more spaces in your inventory.");

                        }
                        break;
                    }


                case 721310: // rank 3
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 723341, 1, 1);  //Endurance Book
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 2 Endurance Books - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) - 1 Refinery Pack and 1 Skill Profiency Pack(1).");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");

                        }
                        break;
                    }

                case 721314: // top 8
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 723341, 3, 1);  //Endurance Book
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 3 EnduranceBooks - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) and 1 Skill Profiency Pack(1).");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 6 more spaces in your inventory.");

                        }
                        break;
                    }


                #endregion

                #region Level 100 - 119
                case 721301: // rank 1
                    {
                        if (client.Inventory.HaveSpace(7))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720894, 1, 1);  //Endurance Book Pack(5)
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3004249, 3, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 1 Endurance Book Pack(5) - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) - 3 Refinery Packs & 1 Skill Profiency Pack(1).");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 7 more spaces in your inventory.");

                        }
                        break;
                    }

                case 721305: // rank 2
                    {
                        if (client.Inventory.HaveSpace(7))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 728920, 1, 1);  //Endurance Book Pack(4)
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3004249, 2, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 1 Endurance Book Pack(4) - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) - 2 Refinery Packs and 1 Skill Profiency Pack(1)  .");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 7 more spaces in your inventory.");

                        }
                        break;
                    }


                case 721309: // rank 3
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 723341, 1, 1);  //Endurance Book
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 2 Endurance Books - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) - 1 Refinery Pack and 1 Skill Profiency Pack(1).");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");

                        }
                        break;
                    }

                case 721313: // top 8
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 723341, 3, 1);  //Endurance Book
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 3 EnduranceBooks - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) and 1 Skill Profiency Pack(1).");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 6 more spaces in your inventory.");

                        }
                        break;
                    }


                #endregion

                #region Level 100~
                case 721300: // rank 1
                    {
                        if (client.Inventory.HaveSpace(7))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720894, 1, 1);  //Endurance Book Pack(5)
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3004249, 3, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 1 Endurance Book Pack(5) - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) - 3 Refinery Packs & 1 Skill Profiency Pack(1).");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 7 more spaces in your inventory.");

                        }
                        break;
                    }

                case 721304: // rank 2
                    {
                        if (client.Inventory.HaveSpace(7))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 728920, 1, 1);  //Endurance Book Pack(4)
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3004249, 2, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 1 Endurance Book Pack(4) - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) - 2 Refinery Packs and 1 Skill Profiency Pack(1)  .");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 7 more spaces in your inventory.");

                        }
                        break;
                    }


                case 721308: // rank 3
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 723341, 1, 1);  //Endurance Book
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3004249, 1, 1); //Super Refinery Packs
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 2 Endurance Books - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) - 1 Refinery Pack and 1 Skill Profiency Pack(1).");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");

                        }
                        break;
                    }

                case 721312: // top 8
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 723341, 3, 1);  //Endurance Book
                            client.Inventory.Add(stream, 721317, 1, 1);  //EXPPartyPack
                            client.Inventory.Add(stream, 720889, 1, 1); //LotteryTicketPack(3)
                            client.Inventory.Add(stream, 3003628, 1, 1); //SkillProfiencyPack(1)
                            client.CreateBoxDialog("You've received 3 EnduranceBooks - 1 EXP Party Pack - 1 Lottery Ticket Pack(3) and 1 Skill Profiency Pack(1).");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 6 more spaces in your inventory.");

                        }
                        break;
                    }


                #endregion

                #endregion
                #region Team Pk
                #region Participation pack
                case 720793: //TeamPKPack
                    {
                        if (client.Player.Level == 140)
                        {
                            client.CreateBoxDialog("You already achieved  the highest level possible.");
                            return;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                        client.CreateBoxDialog("You received~1~hour`s~EXP.");
                        break;
                    }

                #endregion
                #region Requirment
                case 720873: //LotteryTicketPack(5)
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(711504, 0, 5, stream);
                            client.CreateBoxDialog("You received 5~Small~Lottery~Tickets.");

                        }
                        else
                        {

                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }

                        break;
                    }

                // HorseRacingPointsPack: adds Horse Racing Points (the Race Point shop currency); amount = the item name.
                case 720874: OpenRacePointsPack(item, client, stream, 10000); break; //HorseRacingPointsPack(10K)
                case 720875: OpenRacePointsPack(item, client, stream, 8000); break; //HorseRacingPointsPack(8K)
                case 720876: OpenRacePointsPack(item, client, stream, 7000); break; //HorseRacingPointsPack(7K)
                case 720877: OpenRacePointsPack(item, client, stream, 6000); break; //HorseRacingPointsPack(6K)
                case 720878: OpenRacePointsPack(item, client, stream, 5000); break; //HorseRacingPointsPack(5K)
                case 720881: OpenRacePointsPack(item, client, stream, 2000); break; //HorseRacingPointsPack(2K)

                #endregion

                #region Level 130+
                case 720797:
                    {
                        if (client.Inventory.HaveSpace(37))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723342, 10, 1); //ModestyBook
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 5, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 5 Dragon Balls
                            client.Inventory.Add(stream, 700071, 5, 1); // 4 Tortoise Gems
                            client.Inventory.Add(stream, 3003628, 5, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720874, 5, 1); ///HorseRacingPointsPack(10K)
                            client.Inventory.Add(stream, 723717, 5, 1); //Class5MoneyBag
                            client.Inventory.Add(stream, 720836, 5, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 10 Modesty Books - 5 Dragon Balls - 4 Tortoise Gems - 5 Skill Profiency Packs - 5 Horse Racing Points Pack(10K) - 5 Class-5-Money Bags - 5 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 37 more spaces in your inventory.");

                        }
                        break;
                    }
                case 720801:
                    {
                        if (client.Inventory.HaveSpace(30))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723342, 5, 1); //ModestyBook
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 3 Dragon Balls
                            client.Inventory.Add(stream, 700071, 3, 1); // 3 Tortoise Gems
                            client.Inventory.Add(stream, 3003628, 5, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720875, 5, 1); ///HorseRacingPointsPack(8K)
                            client.Inventory.Add(stream, 723717, 3, 1); //Class5MoneyBag
                            client.Inventory.Add(stream, 720836, 5, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 5 Modesty Books - 3 Dragon Balls - 3 Tortoise Gems - 5 Skill Profiency Packs - 5 Horse Racing Points Pack(8K) - 3 Class-5 MoneyBags - 5 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 30 more spaces in your inventory.");

                        }
                        break;
                    }

                case 720805:
                    {
                        if (client.Inventory.HaveSpace(28))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723342, 5, 1); //ModestyBook
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 2 Dragon Balls
                            client.Inventory.Add(stream, 700071, 2, 1); // 2 Tortoise Gems
                            client.Inventory.Add(stream, 3003628, 5, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720876, 5, 1); ///HorseRacingPointsPack(7K)
                            client.Inventory.Add(stream, 723717, 2, 1); //Class5MoneyBag
                            client.Inventory.Add(stream, 720836, 5, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 5 Modesty Books - 2 Dragon Balls - 2 Tortoise Gems - 5 Skill Profiency Packs - 5 Horse Racing Points Pack(7K) - 2 Class-5 MoneyBags - 5 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 28 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720809:
                    {
                        if (client.Inventory.HaveSpace(28))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723342, 5, 1); //ModestyBook
                            client.Inventory.Add(stream, 700071, 1, 1); // 1 Tortoise Gems
                            client.Inventory.Add(stream, 3003628, 5, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720877, 5, 1); ///HorseRacingPointsPack(6K)
                            client.Inventory.Add(stream, 723717, 1, 1); //Class5MoneyBag
                            client.Inventory.Add(stream, 720836, 5, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 5 Modesty Books - 1 Tortoise Gems - 5 Skill Profiency Packs - 5 Horse Racing Points Pack(7K) - 1 Class-5 MoneyBags - 5 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 28 more spaces in your inventory.");

                        }
                        break;
                    }


                #endregion
                #region Level 120-129
                case 720796:
                    {
                        if (client.Inventory.HaveSpace(24))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 5, 1);  //Endurance Book
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 3 Dragon Balls
                            client.Inventory.Add(stream, 3003628, 5, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720877, 5, 1); ///HorseRacingPointsPack(6K)
                            client.Inventory.Add(stream, 720836, 5, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 5 Endurance Books - 3 Dragon Balls - 5 Skill Profiency Packs - 5 Horse Racing Points Pack(6K) - 5 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 24 more spaces in your inventory.");

                        }
                        break;
                    }
                case 720800:
                    {
                        if (client.Inventory.HaveSpace(20))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 4, 1);  //Endurance Book
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 2 Dragon Balls
                            client.Inventory.Add(stream, 3003628, 4, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720878, 5, 1); ///HorseRacingPointsPack(5K)
                            client.Inventory.Add(stream, 720836, 3, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 4 Endurance Books - 2 Dragon Balls - 4 Skill Profiency Packs - 5 Horse Racing Points Pack(5K) - 3 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 20 more spaces in your inventory.");

                        }
                        break;
                    }

                case 720804:
                    {
                        if (client.Inventory.HaveSpace(16))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 3, 1);  //Endurance Book
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 1 Dragon Balls
                            client.Inventory.Add(stream, 3003628, 3, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720880, 5, 1); ///HorseRacingPointsPack(3K)
                            client.Inventory.Add(stream, 720836, 2, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 3 Endurance Books - 1 Dragon Balls - 3 Skill Profiency Packs - 5 Horse Racing Points Pack(3K) - 2 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 16 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720808:
                    {
                        if (client.Inventory.HaveSpace(12))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 2, 1);  //Endurance Book
                            client.Inventory.Add(stream, 3003628, 2, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720881, 5, 1); ///HorseRacingPointsPack(2K)
                            client.Inventory.Add(stream, 720836, 1, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 2 Endurance Books - 2 Skill Profiency Packs - 5 Horse Racing Points Pack(2K) - 1 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 12 more spaces in your inventory.");

                        }
                        break;
                    }


                #endregion
                #region Level 100-119
                case 720795:
                    {
                        if (client.Inventory.HaveSpace(24))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 5, 1);  //Endurance Book
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 3 Dragon Balls
                            client.Inventory.Add(stream, 3003628, 5, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720877, 5, 1); ///HorseRacingPointsPack(6K)
                            client.Inventory.Add(stream, 720836, 5, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 5 Endurance Books - 3 Dragon Balls - 5 Skill Profiency Packs - 5 Horse Racing Points Pack(6K) - 5 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 24 more spaces in your inventory.");

                        }
                        break;
                    }
                case 720799:
                    {
                        if (client.Inventory.HaveSpace(20))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 4, 1);  //Endurance Book
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 2 Dragon Balls
                            client.Inventory.Add(stream, 3003628, 4, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720878, 5, 1); ///HorseRacingPointsPack(5K)
                            client.Inventory.Add(stream, 720836, 3, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 4 Endurance Books - 2 Dragon Balls - 4 Skill Profiency Packs - 5 Horse Racing Points Pack(5K) - 3 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 20 more spaces in your inventory.");

                        }
                        break;
                    }

                case 720803:
                    {
                        if (client.Inventory.HaveSpace(16))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 3, 1);  //Endurance Book
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 1 Dragon Balls
                            client.Inventory.Add(stream, 3003628, 3, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720880, 5, 1); ///HorseRacingPointsPack(3K)
                            client.Inventory.Add(stream, 720836, 2, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 3 Endurance Books - 1 Dragon Balls - 3 Skill Profiency Packs - 5 Horse Racing Points Pack(3K) - 2 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 16 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720807:
                    {
                        if (client.Inventory.HaveSpace(12))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 2, 1);  //Endurance Book
                            client.Inventory.Add(stream, 3003628, 2, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720881, 5, 1); ///HorseRacingPointsPack(2K)
                            client.Inventory.Add(stream, 720836, 1, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 2 Endurance Books - 2 Skill Profiency Packs - 5 Horse Racing Points Pack(2K) - 1 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 12 more spaces in your inventory.");

                        }
                        break;
                    }


                #endregion
                #region Level 100~
                case 720794:
                    {
                        if (client.Inventory.HaveSpace(24))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 5, 1);  //Endurance Book
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 3 Dragon Balls
                            client.Inventory.Add(stream, 3003628, 5, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720877, 5, 1); ///HorseRacingPointsPack(6K)
                            client.Inventory.Add(stream, 720836, 5, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 5 Endurance Books - 3 Dragon Balls - 5 Skill Profiency Packs - 5 Horse Racing Points Pack(6K) - 5 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 24 more spaces in your inventory.");

                        }
                        break;
                    }
                case 720798:
                    {
                        if (client.Inventory.HaveSpace(20))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 4, 1);  //Endurance Book
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 2 Dragon Balls
                            client.Inventory.Add(stream, 3003628, 4, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720878, 5, 1); ///HorseRacingPointsPack(5K)
                            client.Inventory.Add(stream, 720836, 3, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 4 Endurance Books - 2 Dragon Balls - 4 Skill Profiency Packs - 5 Horse Racing Points Pack(5K) - 3 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 20 more spaces in your inventory.");

                        }
                        break;
                    }

                case 720802:
                    {
                        if (client.Inventory.HaveSpace(16))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 3, 1);  //Endurance Book
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // 1 Dragon Balls
                            client.Inventory.Add(stream, 3003628, 3, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720880, 5, 1); ///HorseRacingPointsPack(3K)
                            client.Inventory.Add(stream, 720836, 2, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 3 Endurance Books - 1 Dragon Balls - 3 Skill Profiency Packs - 5 Horse Racing Points Pack(3K) - 2 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 16 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720806:
                    {
                        if (client.Inventory.HaveSpace(12))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 720873, 1, 1); //LotteryTicketPack(5)
                            client.Inventory.Add(stream, 723341, 2, 1);  //Endurance Book
                            client.Inventory.Add(stream, 3003628, 2, 1); //SkillProfiencyPack(1)
                            client.Inventory.Add(stream, 720881, 5, 1); ///HorseRacingPointsPack(2K)
                            client.Inventory.Add(stream, 720836, 1, 1); //weapon accessory packs



                            client.CreateBoxDialog("Check your inventory.");
                            client.SendSysMesage("You've received 5 Lottery Ticket Packs - 2 Endurance Books - 2 Skill Profiency Packs - 5 Horse Racing Points Pack(2K) - 1 Weapon Accessory Packs.");
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 12 more spaces in your inventory.");

                        }
                        break;
                    }


                #endregion
                #endregion

                ////////////////////

                case 3005641:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(711504, 0, 99, stream);
                            client.CreateBoxDialog("You received 99~Small~Lottery~Tickets.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720675: //250 GhostCPPack
                    {
                      
                        
                        string logs = "[DemonBox]" + client.Player.Name + " get 250 he have " + client.Player.ConquerPoints + "";
                        Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 250;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Ghost CP Pack and received 250 CPs!");
                        break;
                    }

                case 720676: //500 SoulCPPack
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 500 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 500;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Soul CP Pack and received 500 CPs!");
                        break;
                    }


                case 720677: //1000 BloodCPPack
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 1000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 1000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Blood CP Pack and received 1000 CPs!");
                        break;
                    }

                case 720678: //13500 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 13500 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 13500;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Life CP Pack and received 13500 CPs!");
                        break;
                    }


                case 720679: //69000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 69000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 69000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Frost CP Pack and received 69000 CPs!");
                        break;
                    }


                case 720681: //69000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 500 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 500;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Deity CP Pack and received 500 CPs!");
                        break;
                    }



                case 720682: //1000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 1000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 1000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Flower CP Pack and received 1000 CPs!");
                        break;
                    }




                case 720683: //2000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 2000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 2000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Heart CP Pack and received 2000 CPs!");
                        break;
                    }



                case 720684: //27000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 27000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 27000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Butterfly CP Pack and received 27000 CPs!");
                        break;
                    }




                case 720685: //138000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 138000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 138000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Nimbus CP Pack and received 138000 CPs!");
                        break;
                    }


                case 720687: //1000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 1000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 1000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Cloud CP Pack and received 1000 CPs!");
                        break;
                    }


                case 720688: //2000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 2000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 2000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Jewel CP Pack and received 2000 CPs!");
                        break;
                    }

                case 720689: //4000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 4000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 4000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Shadow CP Pack and received 4000 CPs!");
                        break;
                    }

                case 720690: //54000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 54000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 54000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Rainbow CP Pack and received 54000 CPs!");
                        break;
                    }


                case 720691: //276000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 276000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 276000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Kylin CP Pack and received 276000 CPs!");
                        break;
                    }
                case 720693: //2500 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 2500 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 2500;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Fog CP Pack and received 2500 CPs!");
                        break;
                    }


                case 720694: //5000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 5000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 5000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Moon CP Pack and received 5000 CPs!");
                        break;
                    }


                case 720695: //10000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 10000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 10000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Earth CP Pack and received 10000 CPs!");
                        break;
                    }



                case 720696: //135000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 135000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        //client.Player.ConquerPoints += 135000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Zephyr CP Pack and received 135000 CPs!");
                        break;
                    }


                case 720697: //690000 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 690000 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        //client.Player.ConquerPoints += 690000;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Pilgrim CP Pack and received 690000 CPs!");
                        break;
                    }


                case 720653: //270 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 270 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 270;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Dream CP Pack and received 270 CPs from Demon Box.");
                        break;
                    }



                case 720654: //1380 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 1380 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 1380;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Joy CP Pack and received 1380 CPs from Demon Box.");
                        break;
                    }


                case 720655: //20 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 20 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 20;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Mammon CP Pack and received 20 CPs from Demon Box.");
                        break;
                    }

                case 720656: //10 
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 10 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 10;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Mascot CP Pack and received 10 CPs from Demon Box.");
                        break;
                    }

                case 720657: //5
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 5 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 5;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Hope CP Pack and received 5 CPs from Demon Box.");
                        break;
                    }

                case 720658: //25
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 25 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 25;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Festival CP Pack and received 25 CPs.");
                        break;
                    }

                case 720659: //50
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 50 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 50;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Sweet CP Pack and received 50 CPs.");
                        break;
                    }


                case 720660: //100
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 100 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 100;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Legend CP Pack and received 100 CPs.");
                        break;
                    }

                case 720661: //1350
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 1350 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 1350;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Pure CP Pack and received 1350 CPs!");
                        break;
                    }


                case 720662: //6900
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 6900 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 6900;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Mystic CP Pack and received 6900 CPs!");
                        break;
                    }

                case 720663: //50
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 50 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 50;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Violet CP Pack and received 50 CPs.");
                        break;
                    }

                case 720664: //100
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 100 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 100;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Flare CP Pack and received 100 CPs.");
                        break;
                    }


                case 720666: //2700
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 2700 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 2700;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Star CP Pack and received 2700 CPs!");
                        break;
                    }

                case 720667: //13800
                    {
                        string logs = "[DemonBox]" + client.Player.Name + " get 13800 he have " + client.Player.ConquerPoints + "";
                       Conquer6609.Game.Conquer6609_Work.Logs.OnDequeue(logs);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 13800;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Fantasy CP Pack and received 13800 CPs!");
                        break;
                    }


                case 3000123:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(711504, 0, 7, stream);
                            client.CreateBoxDialog("You received 7~Small~Lottery~Tickets.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }

                case 3000122:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(711504, 0, 5, stream);
                            client.CreateBoxDialog("You received 5~Small~Lottery~Tickets.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }


                case 3000121:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(711504, 0, 3, stream);
                            client.CreateBoxDialog("You received 3~Small~Lottery~Tickets.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }

                case 3000120:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(711504, 0, 2, stream);
                            client.CreateBoxDialog("You received 2~Small~Lottery~Tickets.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }
                case 3008992:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        client.Inventory.Remove(3008992, 1, stream);
                        byte rand = (byte)Program.GetRandom.Next(0, 5);
                        switch (rand)
                        {
                            case 0:
                                {
                                    client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                    client.SendSysMesage("The Reasure of Dragon turned into a flash of light, and you received 1 Blazing CP Fragment!", MsgMessage.ChatMode.System);
                                    break;
                                }
                            case 1:
                                {
                                    client.Inventory.Add(stream, 3009000);
                                    client.SendSysMesage("The Reasure of Dragon turned into a flash of light, and you received 2 Twilight Star Stones!", MsgMessage.ChatMode.System);
                                    break;
                                }
                            case 2:
                                {
                                    client.Player.AddChampionPoints(100);
                                    client.SendSysMesage("The Reasure of Dragon turned into a flash of light, and you received 100 ChampionPoints!", MsgMessage.ChatMode.System);
                                    break;
                                }
                            case 3:
                                {
                                    client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 2000;
                                    Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                    client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                    client.SendSysMesage("The Reasure of Dragon turned into a flash of light, you received 100 StudyPoints and 2000 ChipPoints!", MsgMessage.ChatMode.System);
                                    break;
                                }
                            case 4:
                                {
                                    client.Inventory.AddItemWitchStack(3006284, 0, 1, stream);
                                    client.SendSysMesage("The Reasure of Dragon turned into a flash of light, and you received 1 Mystery Dew Scrap!", MsgMessage.ChatMode.System);
                                    break;
                                }
                        }
                        break;
                    }
                case 3200005:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!client.Inventory.HaveSpace(3))
                        {
                            client.CreateBoxDialog("Please make 3 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Add(stream, 3600023);
                        client.Inventory.AddItemWitchStack(3003124, 0, 20, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 4000;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.Player.KingDomDeeds += 50;


                        client.Player.UpdateKingdomTreasury(50);

                        client.Player.InnerPower.AddPotency(stream, client, 100);

                        client.SendSysMesage("You received 100 Potency Points 50 points for Union Fund, 50 KingDomDeads, 4000 ChiPoints, (1)EminentExploitPack and (20)FavoredTrainingPill!", MsgMessage.ChatMode.System);

                        break;
                    }

                case 3007108:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.InnerPower.AddPotency(stream, client, 400);
                        client.Inventory.Add(stream, 3008735);

                        client.Player.KingDomExploits += 50;
                        client.Player.UpdateKingdomTreasury(50);

                        client.SendSysMesage("You received 400 Potency Points, 50 War Exploits and 50 points for Union Fund!", MsgMessage.ChatMode.System);
                        client.SendSysMesage("You received a FierceSpirit!", MsgMessage.ChatMode.System);
                        break;
                    }

                case 3007109:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.InnerPower.AddPotency(stream, client, 600);

                        client.Player.KingDomExploits += 100;
                        client.Player.UpdateKingdomTreasury(160);
                        client.Player.AddChampionPoints(50);
                        client.SendSysMesage("You received 50 Champion Points,600 Potency Points, 100 War Exploits and 160 points for Union Fund!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 3007110:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.InnerPower.AddPotency(stream, client, 25000);

                        client.Player.KingDomExploits += 150;
                        client.Player.UpdateKingdomTreasury(360);
                        client.Player.AddChampionPoints(100);
                        client.SendSysMesage("You received 100 Champion Points,25000 Potency Points, 150 War Exploits and 360 points for Union Fund!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 721912:
                    {
                        client.CreateDialog(stream, "You may use the Gallbladders at (450,720), (296,290), (255,228), (243,193), (229,142) to find Bitter Ginseng.", "");
                        break;
                    }
                #region Tower of Mistery

                case 3008956: //30 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 10;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 10 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(300);
                            client.SendSysMesage("You received 30-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }

                case 3008957: //60 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 20;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 20 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(600);
                            client.SendSysMesage("You received 60-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }



                case 3008958: //90 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 30;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 30 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(900);
                            client.SendSysMesage("You received 90-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }


                case 3008959: //120 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 40;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 40 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(1200);
                            client.SendSysMesage("You received 120-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }



                case 3008960: //150 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 50 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(1500);
                            client.SendSysMesage("You received 150-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }


                case 3008961: //160 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 50 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(1600);
                            client.SendSysMesage("You received 160-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }

                case 3008962: //180 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 60;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 60 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(1800);
                            client.SendSysMesage("You received 180-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }


                case 3008963: //200 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 60;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 60 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(2000);
                            client.SendSysMesage("You received 200-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }


                case 3008964: //240 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 70;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 70 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(2000);
                            client.SendSysMesage("You received 240-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }

                case 3008965: //300 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 90;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 90 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(3000);
                            client.SendSysMesage("You received 300-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }

                case 3008966: //320 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 100;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 100 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(3200);
                            client.SendSysMesage("You received 320-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }

                case 3008967: //360 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 100;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 100 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(3600);
                            client.SendSysMesage("You received 360-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }

                case 3008968: //400 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 120;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 120 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(4000);
                            client.SendSysMesage("You received 400-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }


                case 3008969: //480 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 150 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(4800);
                            client.SendSysMesage("You received 480-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }

                case 3008970: //600 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 180;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 180 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(6000);
                            client.SendSysMesage("You received 600-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }



                case 3008971: //720 - minEXPPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 200;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.SendSysMesage("You received 200 ChiPoints.", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            client.GainExpBall(7200);
                            client.SendSysMesage("You received 720-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }



                #endregion
                case 3004259://banshe
                    {
                        if (client.Player.Level >= 99)
                        {
                            if (client.Player.PurbleBanshee > 0)
                            {
                                client.SendSysMesage("sorry, but you really spawned Banshee today.");
                                return;
                            }
                            if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                            {
                                if (!client.Map.ContainMobID(20090, client.Player.UID))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    Database.Server.AddMapMonster(stream, client.Map, 20090, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                                    client.Player.PurbleBanshee++;
                                }
                                else
                                {
                                    client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");

                                }
                            }
                            else
                            {
                                client.CreateDialog(stream, "This item can only be used in your house.", "Got~it.");


                            }
                        }
                        else
                        {
                            client.CreateDialog(stream, "Sorry,~you~cannot~open~this~monster~before~your~Level~is~99.~Please~train~harder.", "I~see.");

                        }
                        break;
                    }
                case 3100012:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        byte rand = (byte)Program.GetRandom.Next(1, 7);
                        switch (rand)
                        {
                            case 1:
                                {
                                    client.Inventory.Add(stream, 3600023, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.CreateBoxDialog("You received EminentExploitPack.");

                                    break;
                                }
                            case 2:
                                {
                                    client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 800;
                                    Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                    client.CreateBoxDialog("You received 800 ChiPoints.");

                                    break;
                                }
                            case 3:
                                {
                                    if (client.Player.SubClass != null)
                                    {
                                        client.Player.SubClass.AddStudyPoints(client, 800, stream);
                                        client.CreateBoxDialog("You received 800 StudyPoints.");

                                    }
                                    break;
                                }
                            case 4:
                                {
                                    Database.ItemType.DBItem _item;
                                    while (true)
                                    {
                                        var array = Database.ItemType.Garments.Values.ToArray();
                                        _item = array[Program.GetRandom.Next(0, array.Length)];
                                        Database.CoatStorage.StorageItem DbGarment;
                                        if (Database.CoatStorage.Garments.TryGetValue(_item.ID, out DbGarment))
                                        {
                                            if (DbGarment.Stars <= 4)
                                            {
                                                client.Inventory.Add(stream, _item.ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.CreateBoxDialog("You received " + _item.Name + ".");

                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    Database.ItemType.DBItem _item;
                                    while (true)
                                    {
                                        var array = Database.ItemType.SteedMounts.Values.ToArray();
                                        _item = array[Program.GetRandom.Next(0, array.Length)];
                                        Database.CoatStorage.StorageItem DBMount;
                                        if (Database.CoatStorage.Mounts.TryGetValue(_item.ID, out DBMount))
                                        {
                                            if (DBMount.Stars <= 4)
                                            {
                                                client.Inventory.Add(stream, _item.ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.CreateBoxDialog("You received " + _item.Name + ".");

                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }

                        }
                        break;
                    }
                default:
                    handled = false;
                    break;
            }
        }
    }
}
