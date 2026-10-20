using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;
using Conquer6609.MsgInterServer.Packets;
using Conquer6609.Game.MsgFloorItem;
using Project2D.Database;
using Conquer6609.Game.MsgTournaments;

namespace Conquer6609.Game.MsgNpc
{
    public unsafe partial class NpcHandler
    {
       
        const uint MaxArenaCpsBet = 10000000;
        [NpcAttribute(NpcID.UnlimitedArena)]
        public static void UnlimitedArena(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("This is the Unlimited Stamina envoy.");
                        dialog.AddOption("Enter Unlimited:", 3);
                        dialog.AddOption("Enter Arena cps:", 200);
                        dialog.AddOption("Leave", 255);
                        dialog.AddAvatar(102).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        dialog.AddInput("Enter the Room No:", 1);
                        dialog.AddOption("Leave", 255);
                        dialog.AddAvatar(102).FinalizeDialog();
                        break;
                    }
                //case 4:
                //    {
                //        dialog.AddInput("Arena cps:", 200);
                //        dialog.AddOption("Leave", 255);
                //        dialog.AddAvatar(102).FinalizeDialog();
                //        break;
                //    }
                case 200:
                    {
                        if (client.Player.Level < 100)
                        {
                            dialog.AddText("sorry, you need to be at keast level 100.")
                        .AddOption("Holy~Shit.", 255).AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {
                         dialog.AddText("Welcome, please insert the amount of cps u want to bet on.")
                       .AddInput("minimum 500 CPS", 201).AddOption("Passing~by.", 255).AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }

              
                case 201:
                    {
                        // CP-bet announcement: level 100+, a valid bet between 500 and MaxArenaCpsBet that the announcer can pay,
                        // at most one announcement per player per minute and one server-wide every 15 seconds.
                        if (client.Player.Level < 100)
                            break;
                        uint bet;
                        if (!uint.TryParse(Input, out bet) || bet < 500)
                        {
                            client.Player.MessageBox("Minimum 500 CPs.", null, null);
                            break;
                        }
                        if (bet > MaxArenaCpsBet)
                        {
                            client.Player.MessageBox("Maximum " + MaxArenaCpsBet + " CPs.", null, null);
                            break;
                        }
                        if (client.Player.ConquerPoints < bet)
                        {
                            client.Player.MessageBox("You don't have enough CPs.", null, null);
                            break;
                        }
                        if (Game.MsgTournaments.MsgSchedules.ArenaCpsBattle.Working)
                        {
                            client.Player.MessageBox("Minimum 500 CPs.", null, null);
                            break;
                        }
                        // The player's 60 s slot is only consumed when the global 15 s slot is free too.
                        if (!global::Conquer6609.Security.Guard.ThrottleBoth("cpsbet:" + client.Player.UID, 60000, "cpsbet:all", 15000))
                        {
                            client.Player.MessageBox("Please wait before announcing another bet.", null, null);
                            break;
                        }
                        foreach (var user in Database.Server.GamePoll.Values)
                        {
                            user.Player.MessageBox("Cps Tournment War Started Bet " + bet + " Cps", p =>
                            {
                                if (Game.MsgTournaments.MsgSchedules.ArenaCpsBattle.SignUp(p, bet) == false)
                                {
                                    p.SendSysMesage("don't have requierments.", MsgMessage.ChatMode.Service, MsgMessage.MsgColor.yellow);
                                }
                            }, null, 15);
                        }
                        break;
                    }
                case 1:
                    {
                        uint result;
                        if (uint.TryParse(Input, out result))
                        {
                            uint roomDynamicId;
                            if (UnlimitedArenaRooms.TryGetRoom(result, out roomDynamicId))
                            {
                                var map = Database.Server.ServerMaps[700];
                                var mapId = roomDynamicId;
                                ushort x = 0;
                                ushort y = 0;
                                map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 700, mapId);
                                client.Player.Hits = 0;
                                client.Player.TotalHits = 0;
                            }
                            else
                            {
                                var map = Database.Server.ServerMaps[700];
                                var mapId = map.GenerateDynamicID();
                                UnlimitedArenaRooms.Maps.Add(result, mapId);
                                ushort x = 0;
                                ushort y = 0;
                                map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 700, mapId);
                                client.Player.Hits = 0;
                                client.Player.TotalHits = 0;
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BootManager)]
        public static void BootManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {//
                        dialog.AddText("Hello, would you like to do PK, with a bot to train? He will have the same appearance and strenght.");
                        dialog.AddOption("Sure.!", 1);
                        dialog.AddAvatar(10);
                        dialog.AddOption("I understand.", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        dialog.AddText("Choose bellow witch type of linear skill your want to use. It is necessary to have the skill and its respective weapon.");
                        dialog.AddOption("Fastblade", 2);
                        dialog.AddOption("ScentSword", 3);
                        dialog.AddOption("ViperFang", 4);
                        dialog.AddOption("DragonTail", 5);
                        dialog.AddAvatar(10);
                        dialog.AddOption("I understand.", 255);
                        dialog.FinalizeDialog();

                        break;
                    }
                case 2://Fastblade
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                        {
                            dialog.AddText("Chosse the type of duel you want.");
                            dialog.AddOption("100 hits.", 10);//Fastblade 
                            dialog.AddOption("Unlimited hits.", 11);
                            dialog.AddOption("Leave", 255);
                            dialog.AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {
                            dialog.AddText("Sorry, but your haven't learned any skills suitable.");
                            dialog.AddOption("What are the requirements?", 6);
                            dialog.AddOption("I understand.", 255);
                            dialog.AddAvatar(10);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
                case 3://ScrenSword
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                        {
                            dialog.AddText("Chosse the type of duel you want.");
                            dialog.AddOption("100 hits.", 12);//ScrenSword
                            dialog.AddOption("Unlimited hits.", 13);
                            dialog.AddOption("Leave", 255);
                            dialog.AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {
                            dialog.AddText("Sorry, but your haven't learned any skills suitable.");
                            dialog.AddOption("What are the requirements?", 6);
                            dialog.AddOption("I understand.", 255);
                            dialog.AddAvatar(10);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
                case 4://ViperFang
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ViperFang))
                        {
                            dialog.AddText("Chosse the type of duel you want.");
                            dialog.AddOption("100 hits.", 14);//ViperFang
                            dialog.AddOption("Unlimited hits.", 15);
                            dialog.AddOption("Leave", 255);
                            dialog.AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {
                            dialog.AddText("Sorry, but your haven't learned any skills suitable.");
                            dialog.AddOption("What are the requirements?", 6);
                            dialog.AddOption("I understand.", 255);
                            dialog.AddAvatar(10);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
                case 5://DragonTail
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonTail))
                        {
                            dialog.AddText("Chosse the type of duel you want.");
                            dialog.AddOption("100 hits.", 16);//DragonTail
                            dialog.AddOption("Unlimited hits.", 17);
                            dialog.AddOption("Leave", 255);
                            dialog.AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {
                            dialog.AddText("Sorry, but your haven't learned any skills suitable.");
                            dialog.AddOption("What are the requirements?", 6);
                            dialog.AddOption("I understand.", 255);
                            dialog.AddAvatar(10);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
                case 10://100hit fastblade
                    {
                        dialog.AddText("Choose your bot level.");
                        dialog.AddOption("Easy .", 30 + 1);
                        dialog.AddOption("Normal .", 30 + 2);
                        dialog.AddOption("Medium .", 30 + 3);
                        dialog.AddOption("Hard .", 30 + 4);
                        dialog.AddOption("Pro Player", 30 + 5);
                        dialog.AddOption("Forget it!", 255);
                        dialog.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 11://Unlimited fastblade
                    {
                        dialog.AddText("Choose your bot level.");
                        dialog.AddOption("Easy .", 20 + 1);
                        dialog.AddOption("Normal .", 20 + 2);
                        dialog.AddOption("Medium .", 20 + 3);
                        dialog.AddOption("Hard .", 20 + 4);
                        dialog.AddOption("Pro Player", 20 + 5);
                        dialog.AddOption("Forget it!", 255);
                        dialog.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 12://100hit ScrenSword
                    {
                        dialog.AddText("Choose your bot level.");
                        dialog.AddOption("Easy .", 40 + 1);
                        dialog.AddOption("Normal .", 40 + 2);
                        dialog.AddOption("Medium .", 40 + 3);
                        dialog.AddOption("Hard .", 40 + 4);
                        dialog.AddOption("Pro Player", 40 + 5);
                        dialog.AddOption("Forget it!", 255);
                        dialog.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 13://Unlimited ScrenSword
                    {
                        dialog.AddText("Choose your bot level.");
                        dialog.AddOption("Easy .", 50 + 1);
                        dialog.AddOption("Normal .", 50 + 2);
                        dialog.AddOption("Medium .", 50 + 3);
                        dialog.AddOption("Hard .", 50 + 4);
                        dialog.AddOption("Pro Player", 50 + 5);
                        dialog.AddOption("Forget it!", 255);
                        dialog.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 14://100hit ViperFang
                    {
                        dialog.AddText("Choose your bot level.");
                        dialog.AddOption("Easy .", 60 + 1);
                        dialog.AddOption("Normal .", 60 + 2);
                        dialog.AddOption("Medium .", 60 + 3);
                        dialog.AddOption("Hard .", 60 + 4);
                        dialog.AddOption("Pro Player", 60 + 5);
                        dialog.AddOption("Forget it!", 255);
                        dialog.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 15://Unlimited ViperFang
                    {
                        dialog.AddText("Choose your bot level.");
                        dialog.AddOption("Easy .", 70 + 1);
                        dialog.AddOption("Normal .", 70 + 2);
                        dialog.AddOption("Medium .", 70 + 3);
                        dialog.AddOption("Hard .", 70 + 4);
                        dialog.AddOption("Pro Player", 70 + 5);
                        dialog.AddOption("Forget it!", 255);
                        dialog.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 16://100hit DragonTail
                    {
                        dialog.AddText("Choose your bot level.");
                        dialog.AddOption("Easy .", 80 + 1);
                        dialog.AddOption("Normal .", 80 + 2);
                        dialog.AddOption("Medium .", 80 + 3);
                        dialog.AddOption("Hard .", 80 + 4);
                        dialog.AddOption("Pro Player", 80 + 5);
                        dialog.AddOption("Forget it!", 255);
                        dialog.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 17://Unlimited DragonTail
                    {
                        dialog.AddText("Choose your bot level.");
                        dialog.AddOption("Easy .", 90 + 1);
                        dialog.AddOption("Normal .", 90 + 2);
                        dialog.AddOption("Medium .", 90 + 3);
                        dialog.AddOption("Hard .", 90 + 4);
                        dialog.AddOption("Pro Player", 90 + 5);
                        dialog.AddOption("Forget it!", 255);
                        dialog.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        dialog.AddText("The followwing weapon and skills are allowed. \n");
                        dialog.AddText("Blade/Sword/BackSword: ScentSword or Fasblde.\n");
                        dialog.AddText("Spear: ViperFang.\n");
                        dialog.AddText("Wand : DragonTail.\n");
                        dialog.AddAvatar(10);
                        dialog.AddOption("I understand.", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                default:
                    {
                        #region Fastblade
                        if ((Option >= 20 + 0) && (Option <= 20 + 6))
                        {
                            var map = Database.Server.ServerMaps[700];
                            var mapId = map.GenerateDynamicID();
                            if (!UnlimitedArenaRooms.Maps.ContainsKey(mapId))
                            {
                                UnlimitedArenaRooms.Maps.Add(mapId, mapId);
                                ushort x = 0;
                                ushort y = 0;
                                map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 700, mapId);
                                client.Player.Hits = 0;
                                client.Player.TotalHits = 0;
                                client.Player.MaxChains = 0;
                                BotManager.ConquerAIHandle.CreateBots(stream, client, (BotManager.BotLevel)(Option - 20), BotManager.BotSkills.Fastblade, BotManager.BotType.MyAi);
                            }
                            break;
                        }
                        else if ((Option >= 30 + 0) && (Option <= 30 + 6))//100Hit Fastblade
                        {
                            var map = Database.Server.ServerMaps[700];
                            var mapId = map.GenerateDynamicID();
                            if (!UnlimitedArenaRooms.Maps.ContainsKey(mapId))
                            {
                                UnlimitedArenaRooms.Maps.Add(mapId, mapId);
                                ushort x = 0;
                                ushort y = 0;
                                map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 700, mapId);
                                client.Player.Hits = 0;
                                client.Player.TotalHits = 0;
                                client.Player.MaxChains = 0;
                                BotManager.ConquerAIHandle.CreateBots(stream, client, (BotManager.BotLevel)(Option - 30), BotManager.BotSkills.Fastblade, BotManager.BotType.AI);

                            }
                            break;
                        }
                        #endregion
                        #region ScrenSword
                        else if ((Option >= 40 + 0) && (Option <= 40 + 6))
                        {
                            var map = Database.Server.ServerMaps[700];
                            var mapId = map.GenerateDynamicID();
                            if (!UnlimitedArenaRooms.Maps.ContainsKey(mapId))
                            {
                                UnlimitedArenaRooms.Maps.Add(mapId, mapId);
                                ushort x = 0;
                                ushort y = 0;
                                map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 700, mapId);
                                client.Player.Hits = 0;
                                client.Player.TotalHits = 0;
                                client.Player.MaxChains = 0;
                                BotManager.ConquerAIHandle.CreateBots(stream, client, (BotManager.BotLevel)(Option - 40), BotManager.BotSkills.ScrenSword, BotManager.BotType.AI);
                            }
                            break;
                        }
                        else if ((Option >= 50 + 0) && (Option <= 50 + 6))
                        {
                            var map = Database.Server.ServerMaps[700];
                            var mapId = map.GenerateDynamicID();

                            if (!UnlimitedArenaRooms.Maps.ContainsKey(mapId))
                            {
                                UnlimitedArenaRooms.Maps.Add(mapId, mapId);
                                ushort x = 0;
                                ushort y = 0;
                                map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 700, mapId);
                                client.Player.Hits = 0;
                                client.Player.TotalHits = 0;
                                client.Player.MaxChains = 0;
                                BotManager.ConquerAIHandle.CreateBots(stream, client, (BotManager.BotLevel)(Option - 50), BotManager.BotSkills.ScrenSword, BotManager.BotType.MyAi);
                            }
                            break;
                        }
                        #endregion
                        #region ViperFang
                        else if ((Option >= 60 + 0) && (Option <= 60 + 6))
                        {
                            var map = Database.Server.ServerMaps[700];
                            var mapId = map.GenerateDynamicID();
                            if (!UnlimitedArenaRooms.Maps.ContainsKey(mapId))
                            {
                                UnlimitedArenaRooms.Maps.Add(mapId, mapId);
                                ushort x = 0;
                                ushort y = 0;
                                map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 700, mapId);
                                client.Player.Hits = 0;
                                client.Player.TotalHits = 0;
                                client.Player.MaxChains = 0;
                                BotManager.ConquerAIHandle.CreateBots(stream, client, (BotManager.BotLevel)(Option - 60), BotManager.BotSkills.ViperFang, BotManager.BotType.AI);
                            }
                            break;
                        }
                        else if ((Option >= 70 + 0) && (Option <= 70 + 6))
                        {
                            var map = Database.Server.ServerMaps[700];
                            var mapId = map.GenerateDynamicID();

                            if (!UnlimitedArenaRooms.Maps.ContainsKey(mapId))
                            {
                                UnlimitedArenaRooms.Maps.Add(mapId, mapId);
                                ushort x = 0;
                                ushort y = 0;
                                map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 700, mapId);
                                client.Player.Hits = 0;
                                client.Player.TotalHits = 0;
                                client.Player.MaxChains = 0;
                                BotManager.ConquerAIHandle.CreateBots(stream, client, (BotManager.BotLevel)(Option - 70), BotManager.BotSkills.ViperFang, BotManager.BotType.MyAi);
                            }
                            break;
                        }
                        #endregion
                        #region DragonTail
                        else if ((Option >= 80 + 0) && (Option <= 80 + 6))
                        {
                            var map = Database.Server.ServerMaps[700];
                            var mapId = map.GenerateDynamicID();
                            if (!UnlimitedArenaRooms.Maps.ContainsKey(mapId))
                            {
                                UnlimitedArenaRooms.Maps.Add(mapId, mapId);
                                ushort x = 0;
                                ushort y = 0;
                                map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 700, mapId);
                                client.Player.Hits = 0;
                                client.Player.TotalHits = 0;
                                client.Player.MaxChains = 0;
                                BotManager.ConquerAIHandle.CreateBots(stream, client, (BotManager.BotLevel)(Option - 80), BotManager.BotSkills.DragonTail, BotManager.BotType.AI);
                            }
                            break;
                        }
                        else if ((Option >= 90 + 0) && (Option <= 90 + 6))
                        {
                            var map = Database.Server.ServerMaps[700];
                            var mapId = map.GenerateDynamicID();

                            if (!UnlimitedArenaRooms.Maps.ContainsKey(mapId))
                            {
                                UnlimitedArenaRooms.Maps.Add(mapId, mapId);
                                ushort x = 0;
                                ushort y = 0;
                                map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 700, mapId);
                                client.Player.Hits = 0;
                                client.Player.TotalHits = 0;
                                client.Player.MaxChains = 0;
                                BotManager.ConquerAIHandle.CreateBots(stream, client, (BotManager.BotLevel)(Option - 90), BotManager.BotSkills.DragonTail, BotManager.BotType.MyAi);
                            }
                            break;
                        }
                        #endregion
                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.NewMonster)]
        public static void NewMonster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("This portal will send you to new Monster");
                        data.AddText("SnowBanshe xx:01");
                        data.AddText("BloodyBanshee xx:08");
                        data.AddText("Delhab xx:12");
                        data.AddText("Nemesis xx:13");
                        data.AddText("SnowBansheeSoul xx:17");
                        data.AddText("ChillingSpook xx:23");
                        data.AddText("ThrillingSpook xx:32");
                        data.AddText("DragonWraith xx:38");
                        data.AddText("DarkWizard xx:42");
                        data.AddText("TeratoDragon xx:48");
                        data.AddText("QueenOfEvil xx:52");
                        data.AddText("NetherTyrant xx:57");

                        // map 32 (Reborn2nd) is not in the official 6609 client (GameMapInfo (32) not found);
                        // its bosses were moved to Dragon Island 10137
                        data.AddOption("Dragon Islend Boss Map.", 2)
                         .AddOption("No.", 255).AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                case 2:
                    {
                        client.Teleport(95, 412, 10137);
                        break;
                    }
                default:
                    break;
            }
        }
     
        [NpcAttribute(NpcID.SellItemsInnerPower)]
        public static void SellItemsInnerPower(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        { //852102,SellItemsInnerPower's,2,54176,1002,317,283,0,0,1,1
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey " + client.Player.Name + " I'm Sell All Of Items One Of 5K And 50K CP[S] .")
                       .AddOption("UniversalConcept(A)&(B)", 1)//614439
                       .AddOption("SecretofBreath(A)&(B)", 2)
                       .AddOption("DragonTactics(A)&(B)", 3)
                       .AddOption("DoctrineofDeity(A)(B)(C)", 4)
                       .AddOption("DemonRider(A)(B)", 5) //-> 3007113& 3007114
                       .AddOption("BoundlessHeart(A)(B)(C)", 6) //EpicPower New 3D -> 671439
                       .AddOption("PuzzleofLife(A)(B)(C)", 7)
                       .AddOption("PackPointsInner", 8)
                       .AddOption("Let~me~think.", 255)
                       .AddAvatar(63).FinalizeDialog();
                        //}
                        break;
                    }

                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            return;
                        }
                        else if (client.Player.ConquerPoints < 10000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 10k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Inventory.Add(stream, 3005365);
                            client.Inventory.Add(stream, 3005366);
                            client.SendSysMesage("Congratz you got a UniversalConcept(A)(B) in your inventory.");
                        }
                        break;
                    }


                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            return;
                        }
                        else if (client.Player.ConquerPoints < 10000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 10k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Inventory.Add(stream, 3005395);
                            client.Inventory.Add(stream, 3005396);
                            client.SendSysMesage("Congratz you got a SecretofBreath(A)(B) in your inventory.");
                        }
                        break;
                    }

                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            return;
                        }
                        else if (client.Player.ConquerPoints < 10000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 10k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Inventory.Add(stream, 3005397);
                            client.Inventory.Add(stream, 3005398);
                            client.SendSysMesage("Congratz you got a DragonTactics(A)(B) in your inventory.");
                        }
                        break;
                    }

                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            return;
                        }
                        else if (client.Player.ConquerPoints < 15000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 15k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 15000;
                            client.Inventory.Add(stream, 3005399);
                            client.Inventory.Add(stream, 3005400);
                            client.Inventory.Add(stream, 3005401);
                            client.SendSysMesage("Congratz you got a DoctrineofDeity(A)(B)(C) in your inventory.");
                        }
                        break;
                    }

                case 5:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            return;
                        }
                        else if (client.Player.ConquerPoints < 10000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 10k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Inventory.Add(stream, 3007113);
                            client.Inventory.Add(stream, 3007114);
                            client.SendSysMesage("Congratz you got a DemonRider(A)(B) in your inventory.");
                        }
                        break;
                    }

                case 6:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            return;
                        }
                        else if (client.Player.ConquerPoints < 15000)
                        {
                            // client.SendSysMesage("Hey. " + client.Player.Name + " Not Now For You Get it's Epic.. I'm Sorry, This Is Coming Soon In The Server If Your Ready .");
                            client.SendSysMesage("Congratz you got a DemonRider(A)(B) in your inventory.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 15000;
                            client.Inventory.Add(stream, 3007115);
                            client.Inventory.Add(stream, 3007116);
                            client.Inventory.Add(stream, 3007117);
                            client.SendSysMesage("Congratz you got a BoundlessHeart(A)(B)(C) in your inventory.");
                        }
                        break;
                    }
                case 7:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            return;
                        }
                        else if (client.Player.ConquerPoints < 15000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 15k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 15000;
                            client.Inventory.Add(stream, 3007230);
                            client.Inventory.Add(stream, 3007231);
                            client.Inventory.Add(stream, 3007232);
                            client.SendSysMesage("Congratz you got a PuzzleofLife(A)(B)(C) in your inventory.");
                        }
                        break;
                    }
                case 8:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            return;
                        }
                        else if (client.Player.ConquerPoints < 50000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 50k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 50000;
                            client.Inventory.Add(stream, 3007110);
                            client.SendSysMesage("Congratz you got a PackPointsInner in your inventory.");
                        }
                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.NPCDONATe)]
        public static void NPCDONATe(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:// quest item 711533
                    {



                        dialog.AddText("Hello <tip color=0xfff05050>");
                        dialog.AddText(client.Player.Name + "</tip>,");
                        // dialog.AddText("you can give me your cps and I will automatically do the hard work for you. The exchange ratio is 1:50000. That means you give me 60 CPs, I donate for you 50000 gold. The Maxmum amount is 6,000,000 CPs.");
                        dialog.AddText("i'm the new stream npc that can help you get on the top faster ! \n");
                        dialog.AddText("instead of donating with gold or by your hand ! \n");
                        dialog.AddText("you can give me <tip color=‎#f0e050> ");
                        dialog.AddText(" ConquerCPS </tip>, and i will automatically do the hard wark for you. The exchange ratio is 1:50000. That means you give me 60 CPs, I donate for you 50000 gold. The Maxmum amount is 6,000,000 CPs. \n");
                        dialog.AddText("So what about your opinion ?!");
                        dialog.AddInput("Insert amount:", 1);
                        dialog.AddOption("No thank you.", 255);
                        dialog.AddAvatar(329).FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        uint input;
                        //  if (int.TryParse(Input, out Value))
                        if (uint.TryParse(Input, out input))
                        {
                            if (input < 60)
                            {
                                dialog.AddText("You need to input more or atleast 60 CPs..");
                                dialog.AddInput("Insert amount:", 1);
                                dialog.AddOption("No thank you.", 255);
                                dialog.AddAvatar(48).FinalizeDialog();
                                return;
                            }
                            if (client.Player.ConquerPoints >= input)
                            {
                                client.NpcCpsInput = input;
                                dialog.AddText("Are you sure you want to donate " + input + " CPs? That means I will donate for you " + (((ulong)input) * 50000ul) + " gold.");
                                dialog.AddOption("Yes", 2);
                                dialog.AddOption("No thank you.", 255);
                                dialog.AddAvatar(48).FinalizeDialog();
                            }
                            else
                            {
                                dialog.AddText("You don't have that much CPs. Try again. \nThe exchange ratio is 1:50000. That means you give me 1 CPs, I donate for you 50000 gold. The Maxmum amount is 6,000,000 CPs.");
                                dialog.AddInput("Insert amount:", 1);
                                dialog.AddOption("No thank you.", 255);
                                dialog.AddAvatar(48).FinalizeDialog();
                            }
                        }
                        else
                        {
                            dialog.AddText("Try again. \nThe exchange ratio is 1:50000. That means you give me 1 CPs, I donate for you 50000 gold. The minimum amount is 60 CPs.");
                            dialog.AddInput("Insert amount:", 1);
                            dialog.AddOption("No thank you.", 255);
                            dialog.AddAvatar(48).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= client.NpcCpsInput && client.NpcCpsInput != 0)
                        {
                            client.Player.ConquerPoints -= client.NpcCpsInput;
                            ////client.Player.ConquerPointsFake -= client.NpcCpsInput;
                            // user.Player.ConquerPoints -= (uint)(UID / 50000);
                            client.Player.Nobility.Donation += client.NpcCpsInput * 50000ul;
                            client.Send(stream.NobilityIconCreate(client.Player.Nobility));
                            Program.NobilityRanking.UpdateRank(client.Player.Nobility);
                            // Game.ConquerStructures.Nobility.Donate((ulong)client.NpcCpsInput * 50000ul, client);
                            client.NpcCpsInput = 0;
                        }
                        break;
                    }



            }
        }
        [NpcAttribute(NpcID.FBSSPortal)]
        public static void FBSSPortal(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("This portal will send you to the TwinCity station. Do you want to use the portal now?")
                            .AddOption("Yes.", 1)
                           .AddOption("No.", 255).AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(300, 300, 1002);
                        break;
                    }
                default:
                    break;
            }
        }
        [NpcAttribute(NpcID.PowerFight)]
        public static void PowerFight(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:// quest item 711533
                    {


                        if (Game.MsgTournaments.MsgCheckLine.CheckLineSpells(client) && client.Player.ConquerPoints > 500)
                        {
                            dialog.AddText("<tip color=0xfff05050>");
                            dialog.AddText("FB/SS Room </tip>,");
                            dialog.AddText("That's a new Magic Arena you can use only ss/fb skills. \n in the arena every loser will lose some cps and the winner will get it be careful");
                            dialog.AddOption("yeah , i wanna know More.", 10);
                            dialog.AddOption("Jois SS/FB Arena .", 11);
                            dialog.AddOption("go hell.", 255);
                            dialog.AddAvatar(255).FinalizeDialog();

                        }
                        else
                        {

                            dialog.AddText("Please Learn FastBlade or ScreenSword or Cps is low than 500 CPS.")
                                     .AddOption("Okay.", 255)
                                    .AddAvatar(63).FinalizeDialog();
                            return;

                        }
                        break;
                    }

                case 11:
                    {
                        var rand = new Random().Next(1, 6);
                        switch (rand)
                        {
                            case 1:
                                {
                                    client.Teleport(83, 83, 5051);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "role-select1");
                                    break;
                                }
                            case 2:
                                {
                                    client.Teleport(47, 64, 5051);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                                    break;
                                }
                            case 3:
                                {
                                    client.Teleport(66, 66, 5051);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "glebesword");
                                    break;
                                }
                            case 4:
                                {
                                    client.Teleport(47, 47, 5051);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "dispel5");
                                    break;
                                }
                            case 5:
                                {
                                    client.Teleport(45, 81, 5051);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                                    break;
                                }

                            default:
                                client.Teleport(55, 55, 5051);
                                break;
                        }
                        break;
                    }
                #region KnowMore
                case 10:
                    {
                        dialog.AddText("Well, We have a new Room to fight in it");

                        dialog.AddText("Every Kill you will got CPS form dead target");
                        dialog.AddText("if you are the dead target you will lose CPS even other killer will got CPS too.");
                        dialog.AddText("The CPS is the reward ! what about the option allowd there ?! /n");
                        dialog.AddText("Ok, Only [FB/SS] Skills are allowd there /n");
                        dialog.AddOption("Join Me arena", 11);
                        dialog.AddOption("Thx i will be back later.", 255);
                        dialog.AddAvatar(255).FinalizeDialog();
                        break;
                    }
                #endregion


            }
        }
        [NpcAttribute(NpcID.LostLady)]
        public static void LostLady(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:// quest item 711533
                    {


                        if (client.Inventory.Contain(720957, 1))
                        {
                            dialog.AddText("Gratz, Mr " + client.Player.Name + " you killed the monster for me and got back my item sounds good /n So you will give it to me i'm at my word there's a great prize awaiting you ?!");
                            dialog.AddOption("well , lady here is your item .", 8);
                            dialog.AddOption("go hell.", 255);
                            dialog.AddAvatar(255).FinalizeDialog();
                            return;
                        }
                        else if (client.Player.helpladytime == 1)
                        {

                            dialog.AddText("Well done , you already helped me i thanx u for helping you could help me but tommorrow /n");

                            dialog.AddOption("NP.", 255);
                            dialog.AddAvatar(255).FinalizeDialog();
                            return;
                        }
                        dialog.AddText("Hello <tip color=0xfff05050>");
                        dialog.AddText(client.Player.Name + "</tip>,");
                        dialog.AddText("Welcome, please help me I need his help I have been here for a while and no one helped me /n");
                        dialog.AddText("You want to help me and know my story, I will reward you /n");
                        dialog.AddOption("yeah , i wanna know about you.", 10);
                        dialog.AddOption("go hell.", 255);
                        dialog.AddAvatar(255).FinalizeDialog();
                        break;
                    }

                #region knowmore
                case 10:
                    {
                        dialog.AddText("I have been here for a long time before some fatal monsters appear to claim immunity /n");

                        dialog.AddText("They killed my husband and stole some money from me but the most important money stolen by something special from me /n");
                        dialog.AddText("And here I can not move before I get it back, do you want to know more or go now? /n");

                        dialog.AddOption("Well. then what?.", 11);
                        dialog.AddOption("No more Words I'll go to kill this monster.", 9);
                        dialog.AddOption("Thx for information.", 255);
                        dialog.AddAvatar(255).FinalizeDialog();
                        break;
                    }
                #endregion
                #region well then
                case 11:
                    {
                        dialog.AddText("Well, one day the monsters entered our house was a happy day until these monsters appeared /n");

                        dialog.AddText("They killed my children and my husband and they robbed me but I pretended that I had been seen and had seen something that had been stolen from me a long time ago. /n");
                        dialog.AddText("My mother has done it to me and she died only a few hours later, and now I want this thing, something dear to me, I can /n");
                        dialog.AddText("I will give you some of the CPS as a bonus if you get my body back, so what is your opinion? /n");
                        dialog.AddOption("Ummm , No Porblem with killing The Monster.", 9);
                        dialog.AddOption("Thx i will be back later.", 255);
                        dialog.AddAvatar(255).FinalizeDialog();
                        break;
                    }
                #endregion
                #region Gokill
                case 9:
                    {

                        client.SendSysMesage("Note!. you have been into the map if failed to kill the monster no chance again to do that", MsgMessage.ChatMode.Service);
                        client.Player.helpladytime = 1;
                        client.Teleport(669, 663, 1001, Database.Server.ServerMaps[1001].GenerateDynamicID());
                        Database.Server.AddMapMonster(stream, client.Map, 40937, 669, 663, 2, 2, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        break;
                    }
                #endregion

            }
        }
  
        /// <summary>VIP can be shared only by a VIP6 whose VIP expires more than 2 days from now.</summary>
        static bool CanShareVip(Client.GameClient sharer)
        {
            return sharer != null && sharer.Player != null && sharer.Player.VipLevel >= 6
                && sharer.Player.ExpireVip > DateTime.Now.AddDays(2);
        }
        [NpcAttribute(NpcID.GM)]
        public static void GM(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        dialog.AddText("Hello <tip color=0xfff05050>");
                        dialog.AddText(client.Player.Name + "</tip>,");
                        dialog.AddText("Welcome, player, we care about you for your happiness with us in the server ");
                        dialog.AddText("The [G.M] put me here to help you Learn more about our server \n");
                        dialog.AddText("Just ask what you want and I will answer, can you also know our own websites? \n");
                        dialog.AddText("If you have any problem about the game you can contact us via the website /n or communicate with the gm directly \n");

                        dialog.AddOption("WebSite~Game.", 61);
                        dialog.AddOption("Facebook+twitter~Game.", 64);
                        dialog.AddOption("ShareVIP.", 10);
                        //dialog.AddOption("Register Account.", 20);
                        //dialog.AddOption("Change Password.", 30);
                        dialog.AddOption("Later.", 255);
                        dialog.AddAvatar(255).FinalizeDialog();
                        break;
                    }
                #region Facebook
                case 64:
                    {

                        dialog.AddText("If you would like to access our social networking pages click here");
                        dialog.AddOption("groups~Facebook~Game.", 65);
                        dialog.AddOption("Page~Facebook~Game.", 66);
                        dialog.AddOption("twitter~Game.", 88);
                        //  dialog.AddOption("Cliam vote prize.", 71);
                        dialog.AddAvatar(62).FinalizeDialog();
                        break;
                    }
                case 65:
                    {
                        client.SendSysMesage("", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);
                        break;
                    }
                case 66:
                    {
                        client.SendSysMesage("", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);
                        break;
                    }
                case 88:
                    {
                        client.SendSysMesage("", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);
                        break;
                    }
                case 71:
                    {
                        using (var cmd = new MySqlCommand(MySqlCommandType.SELECT))
                        {
                            cmd.Select("votes_accounts").Where("Username", client.Player.UID);
                            using (MySqlReader rdr = new MySqlReader(cmd))
                            {
                                if (rdr.Read())
                                {
                                    var count = rdr.ReadUInt16("Claims");
                                    try
                                    {
                                        if (client.Player.UID == uint.Parse(rdr.ReadString("Username")))
                                        {
                                            if (count > 0)
                                            {
                                                var com = new MySqlCommand(MySqlCommandType.UPDATE);
                                                com.Update("votes_accounts").Set("Claims", (int)0)
                                                    .Where("Username", client.Player.UID);
                                                if (com.Execute() > 0)
                                                {
                                                    client.Inventory.AddItemWitchStack(3001044, 0, 1, stream, true);
                                                    client.Player.ConquerPoints += 20000;
                                                    client.Player.VipLevel = 4;
                                                    client.Player.ExpireVip = DateTime.Now.AddHours(count * 5);
                                                    client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                                                    client.CreateBoxDialog("You`ve received vip4 for " + (count * 5) + " hours.");
                                                }
                                            }
                                            else
                                            {
                                                client.CreateBoxDialog("there's no prize for you");
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        var com = new MySqlCommand(MySqlCommandType.DELETE);
                                        com.Delete("votes_accounts", "Username", client.Player.UID).Execute();
                                        client.CreateBoxDialog("Somthing Wrong try agine later");
                                    }
                                }
                                else
                                {
                                    client.CreateBoxDialog("there's no prize for you");
                                }
                            }
                        }
                        break;
                    }
                #endregion
                #region WebSite
                case 61:
                    {

                        dialog.AddText("If you want to access the game site click here WebSite Game");
                        dialog.AddOption("WebSite~Game.", 62);
                        //  dialog.AddOption("Cliam vote prize.", 69);
                        dialog.AddAvatar(62).FinalizeDialog();
                        break;
                    }
                case 62:
                    {
                        client.SendSysMesage("10.0.9.252", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);
                        break;
                    }
                case 69:
                    {
                        using (var cmd = new MySqlCommand(MySqlCommandType.SELECT))
                        {
                            cmd.Select("votes_accounts").Where("Username", client.Player.UID);
                            using (MySqlReader rdr = new MySqlReader(cmd))
                            {
                                if (rdr.Read())
                                {
                                    var count = rdr.ReadUInt16("Claims");
                                    try
                                    {
                                        if (client.Player.UID == uint.Parse(rdr.ReadString("Username")))
                                        {
                                            if (count > 0)
                                            {
                                                var com = new MySqlCommand(MySqlCommandType.UPDATE);
                                                com.Update("votes_accounts").Set("Claims", (int)0)
                                                    .Where("Username", client.Player.UID);
                                                if (com.Execute() > 0)
                                                {
                                                    client.Inventory.AddItemWitchStack(3001044, 0, 1, stream, true);
                                                    client.Player.ConquerPoints += 20000;
                                                    client.Player.VipLevel = 4;
                                                    client.Player.ExpireVip = DateTime.Now.AddHours(count * 5);
                                                    client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                                                    client.CreateBoxDialog("You`ve received vip4 for " + (count * 5) + " hours.");
                                                }
                                            }
                                            else
                                            {
                                                client.CreateBoxDialog("there's no prize for you");
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        var com = new MySqlCommand(MySqlCommandType.DELETE);
                                        com.Delete("votes_accounts", "Username", client.Player.UID).Execute();
                                        client.CreateBoxDialog("Somthing Wrong try agine later");
                                    }
                                }
                                else
                                {
                                    client.CreateBoxDialog("there's no prize for you");
                                }
                            }
                        }
                        break;
                    }
                #endregion
                #region VIP Share
                case 10:
                    {
                        if (client.Player.VipLevel >= 6 && client.Player.ExpireVip > DateTime.Now.AddDays(2))
                        {
                            dialog.AddText("do you wish to share your VIPLevel?.");
                            dialog.AddInput("Name", 11);
                            dialog.AddOption("Later.", 255);
                        }
                        else
                        {
                            dialog.AddText("Only vip6 can be shared.");
                            dialog.AddOption("i see.", 255);
                        }
                        dialog.AddAvatar(255).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        // VIP share: the sharer must still hold VIP6 with more than 2 days left (re-checked at every step),
                        // cannot share with themselves, and the share never outlives the sharer's own VIP.
                        if (!CanShareVip(client))
                            break;
                        Client.GameClient shared = Client.GameClient.CharacterFromName(Input);
                        if (shared != null && shared.Player != null && shared.Player.UID != client.Player.UID)
                        {
                            client.Player.MessageBox("You are about to share VIP4 for until " + client.Player.ExpireVip.ToShortDateString() + " to " + shared.Player.Name + " are you sure?",
                            new Action<Client.GameClient>(p =>
                            {
                                if (!CanShareVip(p) || shared.Player == null || !shared.Socket.Alive)
                                    return;
                                shared.Player.MessageBox(p.Player.Name + " Want to share VIP4 that will ends on " + p.Player.ExpireVip.ToShortDateString() + " do you agree?",
                                     new Action<Client.GameClient>(s =>
                                     {
                                         if (!CanShareVip(p) || s.Player.UID == p.Player.UID || s.Player.UID != shared.Player.UID)
                                         {
                                             s.Player.MessageBox("VIP Sharing process has been failed", null, null);
                                             return;
                                         }
                                         if (Database.ShareVIP.Add(new Database.ShareVIP.Client { UID = p.Player.UID, ShareUID = s.Player.UID, ShareLevel = 4, ShareEnds = p.Player.ExpireVip, ShareName = s.Player.Name }))
                                         {
                                             using (var rec = new ServerSockets.RecycledPacket())
                                             {
                                                 var pstream = rec.GetStream();
                                                 s.Player.SendUpdate(pstream, p.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                                             }
                                             p.Player.MessageBox("VIP Sharing process has been successfully completed", null, null);
                                             s.Player.MessageBox("VIP Sharing process has been successfully completed", null, null);
                                         }
                                         else
                                         {
                                             p.Player.MessageBox("VIP Sharing process has been failed", null, null);
                                             s.Player.MessageBox("VIP Sharing process has been failed", null, null);
                                         }
                                     }), null);

                            }), null);
                        }
                        break;
                    }
                #endregion
            }
        }
        [NpcAttribute(NpcID.SantaStuff)]
        public static void SantaStuff(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Hello <tip color=0xfff05050> i have some stuff for you");
                        dialog.AddText(client.Player.Name + "</tip> ,, i Have Created By Edit. ->  <- The Best,");
                        dialog.AddText(" how can i help you?");
                        dialog.AddOption("Stuff Trojan.", 1);
                        dialog.AddOption("Stuff Warrior.", 2);
                        dialog.AddOption("Stuff Archer.", 3);
                        dialog.AddOption("Stuff NInja.", 4);
                        dialog.AddOption("Stuff Monk.", 5);
                        dialog.AddOption("Stuff Pirate.", 6);
                        dialog.AddOption("Stuff BrucLee.", 7);
                        dialog.AddOption("Stuff Taoist.", 8);
                        dialog.AddOption("Stuff WindWalker.", 9);
                        dialog.AddOption("Later.", 255);
                        dialog.AddAvatar(255).FinalizeDialog();
                        break;
                    }
                #region Stuff trojan
                case 1:
                    {
                      
                        if (client.Player.ConquerPoints < 200000)
                        {
                            client.SendSysMesage("You don't have Enough Cps i need 200k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 200000;
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);

                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.AddSoul(120269, 821033, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Necklace
                            client.Inventory.AddSoul(150269, 823059, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Ring
                            client.Inventory.AddSoul(160249, 824018, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Boot
                            client.Inventory.AddSoul(201009, 0, 0, 0, 12, 103, 103, 0, 0, 1, stream, false);//Fan
                            client.Inventory.AddSoul(202009, 0, 0, 0, 12, 123, 123, 0, 0, 1, stream, false);//Tower
                            client.Inventory.AddSoul(300000, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Steed
                            client.Inventory.AddSoul(203009, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Crop
                            client.Inventory.AddSoul(410439, 800111, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//SkyBlade
                            client.Inventory.AddSoul(410439, 800111, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//SquallSword
                            client.Inventory.AddSoul(480439, 800111, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//NirvanaClub
                            client.Inventory.AddSoul(480439, 800111, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//NirvanaClub
                            client.Inventory.AddSoul(130309, 822071, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//ObsidianArmor
                            client.Inventory.AddSoul(118309, 820075, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//PeerlessCoronet
                            client.Inventory.AddSoul(204009, 0, 0, 0, 12, 103, 123, 0, 1, 1, stream, false);

                            client.SendSysMesage("Congratz you got a stuff in your inventory.");
                        }
                        break;
                    }
                #endregion
                /////////////////
                #region Stuff Warrior
                case 2:
                    {
                    
                        if (client.Player.ConquerPoints < 200000)
                        {
                            client.SendSysMesage("You don't have Enough Cps i need 200k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 200000;
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);

                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.AddSoul(120269, 821033, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Necklace
                            client.Inventory.AddSoul(150269, 823059, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Ring
                            client.Inventory.AddSoul(160249, 824018, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Boot
                            client.Inventory.AddSoul(201009, 0, 0, 0, 12, 103, 103, 0, 0, 1, stream, false);//Fan
                            client.Inventory.AddSoul(202009, 0, 0, 0, 12, 123, 123, 0, 0, 1, stream, false);//Tower
                            client.Inventory.AddSoul(300000, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Steed
                            client.Inventory.AddSoul(203009, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Crop
                            client.Inventory.AddSoul(560439, 800215, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//SpearOfWrath
                            client.Inventory.AddSoul(561439, 800215, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//OccultWand
                            client.Inventory.AddSoul(510439, 800215, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//VigorGlaive
                            client.Inventory.AddSoul(530439, 800215, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//HawkPoleaxe
                            client.Inventory.AddSoul(580439, 800215, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//FlameHalbert
                            client.Inventory.AddSoul(131309, 822071, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//ImperiousArmor
                            client.Inventory.AddSoul(148309, 800422, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//CelestialShield
                            client.Inventory.AddSoul(111309, 820075, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//SteelHelmet
                            client.Inventory.AddSoul(204009, 0, 0, 0, 12, 103, 123, 0, 1, 1, stream, false);

                            client.SendSysMesage("Congratz you got a stuff in your inventory.");
                        }
                        break;
                    }
                #endregion #region Stuff trojan
                ////////////////////////
                #region Stuff Archer
                case 3:
                    {
                        if (client.Player.ConquerPoints < 200000)
                        {
                            client.SendSysMesage("You don't have Enough Cps i need 200k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 200000;
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);

                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.AddSoul(120269, 821033, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Necklace
                            client.Inventory.AddSoul(150269, 823059, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Ring
                            client.Inventory.AddSoul(160249, 824018, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Boot
                            client.Inventory.AddSoul(201009, 0, 0, 0, 12, 103, 103, 0, 0, 1, stream, false);//Fan
                            client.Inventory.AddSoul(202009, 0, 0, 0, 12, 123, 123, 0, 0, 1, stream, false);//Tower
                            client.Inventory.AddSoul(300000, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Steed
                            client.Inventory.AddSoul(203009, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Crop
                            client.Inventory.AddSoul(613429, 800917, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//Knife
                            client.Inventory.AddSoul(613429, 800917, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//Knife
                            client.Inventory.AddSoul(500429, 800618, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Bow
                            client.Inventory.AddSoul(133309, 822071, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//ArmorArcher
                            client.Inventory.AddSoul(113309, 820075, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//CapArcher
                            client.Inventory.AddSoul(204009, 0, 0, 0, 12, 103, 123, 0, 1, 1, stream, false);
                            client.SendSysMesage("Congratz you got a stuff in your inventory.");
                        }
                        break;
                    }
                #endregion #region Stuff trojan

                ///////////
                #region Stuff Ninja
                case 4:
                    {
                       if (client.Player.ConquerPoints < 200000)
                        {
                            client.SendSysMesage("You don't have Enough Cps i need 200k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 200000;

                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);

                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.AddSoul(120269, 821033, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Necklace
                            client.Inventory.AddSoul(150269, 823059, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Ring
                            client.Inventory.AddSoul(160249, 824018, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Boot
                            client.Inventory.AddSoul(201009, 0, 0, 0, 12, 103, 103, 0, 0, 1, stream, false);//Fan
                            client.Inventory.AddSoul(202009, 0, 0, 0, 12, 123, 123, 0, 0, 1, stream, false);//Tower
                            client.Inventory.AddSoul(300000, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Steed
                            client.Inventory.AddSoul(203009, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Crop
                            client.Inventory.AddSoul(601439, 800142, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//HanzoKatana
                            client.Inventory.AddSoul(601439, 800142, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//HanzoKatana
                            client.Inventory.AddSoul(511439, 800255, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//SilenceScythe
                            client.Inventory.AddSoul(135309, 822071, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//NightmareVest
                            client.Inventory.AddSoul(112309, 820075, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//RambleVeil
                            client.Inventory.AddSoul(204009, 0, 0, 0, 12, 103, 123, 0, 1, 1, stream, false);
                            client.SendSysMesage("Congratz you got a stuff in your inventory.");
                        }
                        break;
                    }
                #endregion #region Stuff trojan

                ////////
                #region Stuff Monk
                case 5:
                    {
                        if (client.Player.ConquerPoints < 200000)
                        {
                            client.SendSysMesage("You don't have Enough Cps i need 200k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 200000;

                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);

                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.AddSoul(120269, 821033, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Necklace
                            client.Inventory.AddSoul(150269, 823059, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Ring
                            client.Inventory.AddSoul(160249, 824018, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Boot
                            client.Inventory.AddSoul(201009, 0, 0, 0, 12, 103, 103, 0, 0, 1, stream, false);//Fan
                            client.Inventory.AddSoul(202009, 0, 0, 0, 12, 123, 123, 0, 0, 1, stream, false);//Tower
                            client.Inventory.AddSoul(300000, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Steed
                            client.Inventory.AddSoul(203009, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Crop
                            client.Inventory.AddSoul(610439, 800725, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//LazuritePrayerBeads
                            client.Inventory.AddSoul(610439, 800725, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//LazuritePrayerBeads
                            client.Inventory.AddSoul(136309, 822071, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//WhiteLotusFrock
                            client.Inventory.AddSoul(143309, 820075, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//XumiCap
                            client.Inventory.AddSoul(204009, 0, 0, 0, 12, 103, 123, 0, 1, 1, stream, false);
                            client.SendSysMesage("Congratz you got a stuff in your inventory.");
                        }
                        break;
                    }
                #endregion #region Stuff trojan
                //////
                #region Stuff Pirate
                case 6:
                    {
                         if (client.Player.ConquerPoints < 200000)
                        {
                            client.SendSysMesage("You don't have Enough Cps i need 200k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 200000;

                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);

                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.AddSoul(120269, 821033, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Necklace
                            client.Inventory.AddSoul(150269, 823059, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Ring
                            client.Inventory.AddSoul(160249, 824018, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Boot
                            client.Inventory.AddSoul(201009, 0, 0, 0, 12, 103, 103, 0, 0, 1, stream, false);//Fan
                            client.Inventory.AddSoul(202009, 0, 0, 0, 12, 123, 123, 0, 0, 1, stream, false);//Tower
                            client.Inventory.AddSoul(300000, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Steed
                            client.Inventory.AddSoul(203009, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Crop
                            client.Inventory.AddSoul(611439, 800811, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//CaptainRapier
                            client.Inventory.AddSoul(612439, 800810, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//LordPistol
                            client.Inventory.AddSoul(139309, 822071, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//DarkDragonCoat
                            client.Inventory.AddSoul(144309, 820075, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//DominatorHat
                            client.Inventory.AddSoul(204009, 0, 0, 0, 12, 103, 123, 0, 1, 1, stream, false);
                            client.SendSysMesage("Congratz you got a stuff in your inventory.");
                        }
                        break;
                    }
                #endregion #region Stuff trojan
                //////
                #region Stuff Bruclee
                case 7:
                    {
                        if (client.Player.ConquerPoints < 200000)
                        {
                            client.SendSysMesage("You don't have Enough Cps i need 200k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 200000;

                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);

                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.AddSoul(120269, 821033, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Necklace
                            client.Inventory.AddSoul(150269, 823059, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Ring
                            client.Inventory.AddSoul(160249, 824018, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Boot
                            client.Inventory.AddSoul(201009, 0, 0, 0, 12, 103, 103, 0, 0, 1, stream, false);//Fan
                            client.Inventory.AddSoul(202009, 0, 0, 0, 12, 123, 123, 0, 0, 1, stream, false);//Tower
                            client.Inventory.AddSoul(300000, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Steed
                            client.Inventory.AddSoul(203009, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Crop
                            client.Inventory.AddSoul(617439, 801004, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//SkyNunchaku
                            client.Inventory.AddSoul(617439, 801004, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//SkyNunchaku
                            client.Inventory.AddSoul(138309, 822071, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//CombatSuit
                            client.Inventory.AddSoul(148309, 820075, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//LegendHood
                            client.Inventory.AddSoul(204009, 0, 0, 0, 12, 103, 123, 0, 1, 1, stream, false);
                            client.SendSysMesage("Congratz you got a stuff in your inventory.");
                        }
                        break;
                    }
                #endregion #region Stuff trojan
                //
                #region Stuff Taoist
                case 8:
                    {
                        if (client.Player.ConquerPoints < 200000)
                        {
                            client.SendSysMesage("You don't have Enough Cps i need 200k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 200000;

                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);

                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.AddSoul(201009, 0, 0, 0, 12, 103, 103, 0, 0, 1, stream, false);//Fan
                            client.Inventory.AddSoul(202009, 0, 0, 0, 12, 123, 123, 0, 0, 1, stream, false);//Tower
                            client.Inventory.AddSoul(300000, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Steed
                            client.Inventory.AddSoul(203009, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Crop
                            client.Inventory.AddSoul(620439, 800522, 6, 12, 12, 3, 3, 255, 7, 1, stream, false);//ImperialBacksword
                            client.Inventory.AddSoul(619439, 801104, 6, 12, 12, 0, 0, 0, 0, 1, stream, false);//UniverseHossu
                            client.Inventory.AddSoul(421439, 800522, 6, 12, 12, 3, 3, 255, 7, 1, stream, false);//BackSword
                            client.Inventory.AddSoul(134309, 822072, 6, 12, 12, 3, 3, 255, 7, 1, stream, false);//ArmorFire
                            client.Inventory.AddSoul(114309, 820074, 6, 12, 12, 3, 3, 255, 7, 1, stream, false);//CapFire
                            client.Inventory.AddSoul(152279, 823060, 6, 12, 12, 3, 3, 255, 7, 1, stream, false);//Bracelet
                            client.Inventory.AddSoul(121269, 821034, 6, 12, 12, 3, 3, 255, 7, 1, stream, false);//Bag
                            client.Inventory.AddSoul(160249, 824018, 6, 12, 12, 3, 3, 255, 7, 1, stream, false);//Boot
                            client.Inventory.AddSoul(204009, 0, 0, 0, 12, 103, 123, 0, 1, 1, stream, false);
                            client.SendSysMesage("Congratz you got a stuff in your inventory.");
                        }
                        break;
                    }
                #endregion #region Stuff trojan
                ////
                #region Stuff WindWalker
                case 9:
                    {
                        if (client.Player.ConquerPoints < 200000)
                        {
                            client.SendSysMesage("You don't have Enough Cps i need 200k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 200000;

                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004247);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);
                            client.Inventory.Add(stream, 3004248);

                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.Add(stream, 3004249);
                            client.Inventory.AddSoul(120269, 821033, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Necklace
                            client.Inventory.AddSoul(150269, 823059, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Ring
                            client.Inventory.AddSoul(160249, 824018, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//Boot
                            client.Inventory.AddSoul(201009, 0, 0, 0, 12, 103, 103, 0, 0, 1, stream, false);//Fan
                            client.Inventory.AddSoul(202009, 0, 0, 0, 12, 123, 123, 0, 0, 1, stream, false);//Tower
                            client.Inventory.AddSoul(300000, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Steed
                            client.Inventory.AddSoul(203009, 0, 0, 0, 12, 0, 0, 0, 0, 1, stream, false);//Crop
                            client.Inventory.AddSoul(626439, 0, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//SkyNunchaku
                            client.Inventory.AddSoul(626439, 0, 6, 12, 12, 13, 13, 255, 7, 2, stream, false);//SkyNunchaku
                            client.Inventory.AddSoul(101309, 0, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//CombatSuit
                            client.Inventory.AddSoul(170309, 0, 6, 12, 12, 13, 13, 255, 7, 1, stream, false);//LegendHood
                            client.Inventory.AddSoul(204009, 0, 0, 0, 12, 103, 123, 0, 1, 1, stream, false);
                            client.SendSysMesage("Congratz you got a stuff in your inventory.");
                        }
                        break;
                    }
                #endregion #region Stuff trojan

            }
        }
    }
}
