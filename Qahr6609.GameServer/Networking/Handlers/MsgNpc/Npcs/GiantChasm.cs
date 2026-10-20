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


        [NpcAttribute(NpcID.CobbWind2)]
        public static void CobbWind2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime Now64 = DateTime.Now;
            var map = Database.Server.ServerMaps[10166];
            string note = null;
            #region Note
            if (map.ContainMobID(29360))//chillingspook
            {
                note = "[Chilling Spook] has appeared around (214,096).* ";
            }
            else if (map.ContainMobID(29363))//dragonwraith
            {
                note = "[Dragon Wraith] has appeared around (204,138).* ";
            }
            else if (map.ContainMobID(29300))//nemsis
            {
                note = "[Nether Tyrant] has appeared around (257,136).* ";
            }
            else if (map.ContainMobID(29370))//bloodybanshee
            {
                note = "[Bloody Banshee] has appeared around (238,165) .* ";
            }
           
            if (String.IsNullOrEmpty(note))
            {
                if (Now64.Minute > 00 && Now64.Minute < 10)
                {
                    if (!map.ContainMobID(29370))
                    {
                        note = "[Bloody Banshee] has appeared around (238,165) on the Giant Chasm.* ";
                    }
                }
                if (Now64.Minute > 20 && Now64.Minute < 40)
                {
                    if (!map.ContainMobID(29360))
                    {
                        note = "[Chilling Spook] has appeared around (658, 718) on the Giant Chasm.* ";
                    }
                }
                if (Now64.Minute > 40 && Now64.Minute < 45)
                {
                    if (!map.ContainMobID(29300))
                    {
                        note = "[Nether Tyrant] has appeared around (257,136) on the Giant Chasm.* ";
                    }
                }
                if (Now64.Minute > 50 && Now64.Minute < 59)
                {
                    if (!map.ContainMobID(29363))
                    {
                        note = "[Dragon Wraith] has appeared around (204,138) on the Giant Chasm.* ";
                    }
                }
            }
            #endregion
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("<tip desc=Cobb#Wind,#formerly#a#pirate,#was#save#by#Sky#Shan#from#a#murder.#He#has#turned#to#the#side#of#justice#since#then,#and#made#lots#of#contributions.>Cobb Wind</tip>:")
                        .AddText("The <tip color=0xfff05050 desc=Killing#the#Queen#of#Evil#brings#+2#Stone,#Blazing#CP#Fragment,#etc.>Queen of Evil</tip> has gathered~")
                        .AddText("<tip color=0xfff05050 desc=Killing#Nether#Tyrant#brings#Savage#Bone,#P7#Dragon#Soul#Fragment,#Meteor#Scroll,#etc.>Nether Tyrant</tip>,")
                        .AddText("~<tip color=0xfff05050 desc=Killing#Bloody#Banshee#brings#Dragon#Ball,#+2#Stone,#Mount#Armor,#Weapon#Accessory,#etc.>Bloody Banshee</tip>,")
                        .AddText("~<tip color=0xfff05050 desc=Killing#Dragon#Wraith#brings#Superior#Refinery#Pack,#Blazing#CP#Fragment,#etc.>Dragon Wraith</tip> and~")
                        .AddText("<tip color=0xfff05050 desc=Killing#Chilling#Spook#brings#P6#Dragon#Soul,#Dragon#Ball,#Blazing#CP#Fragment,#etc.>Chilling Spook</tip>~")
                        .AddText("in the Giant Chasm. They must have known the ancient treasures in the chasm. Hurry up! We have to stop them from getting stronger.")
                        .AddOption("Learn~more~about~these~beasts.", 1)
                       .AddOption("I`ll~keep~an~eye~on~them.", 255).AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (map.ContainMobID(20160) || map.ContainMobID(20300) || map.ContainMobID(20070))
                        {
                            data.AddText("<tip desc=Cobb#Wind,#formerly#a#pirate,#was#save#by#Sky#Shan#from#a#murder.#He#has#turned#to#the#side#of#justice#since#then,#and#made#lots#of#contributions.>Cobb Wind</tip>: Urgent report from the front line!")
                                .AddText(note)
                                .AddText("Many heroes are heading to fight against the beasts. Hurry and go support them!");
                            if (map.ContainMobID(29370))
                            {
                                data.AddOption("Go~kill~the~[Bloody~Banshee].", 255);
                            }
                            else if (map.ContainMobID(29360))
                            {
                                data.AddOption("Go~kill~the~[Chilling~Spook].", 255);
                            }
                            else if (map.ContainMobID(29300))
                            {
                                data.AddOption("Go~kill~the~[Nether~Tyrant].", 255);
                            }
                            else if (map.ContainMobID(29363))
                            {
                                data.AddOption("Go~kill~the~[Dragon~Wraith].", 255);
                            }
                        }
                        else
                        {
                            data.AddText("------------------------------------------------------------------------------------------------ \n")
                                .AddText("     [Beast]                    [Location]       		[Appear Time] \n")
                                //.AddText("Queen of Evil	   Giant Chasm(286,158)  19:10 and 21:10,every day \n")
                                .AddText("Bloody Banshee Giant Chasm(238,165)   At 10 min past each hour,every day \n")
                                .AddText("Chilling Spook	Giant Chasm(214,096)   At 20 min past each hour,every day \n")
                                .AddText("Nether Tyrant	   Giant Chasm(257,136)  At 40 min past each two hour,every day \n")
                                .AddText("Dragon Wraith	 Giant Chasm(204,138)  At 50 min past each hour,every day \n")
                                .AddText("------------------------------------------------------------------------------------------------")
                                .AddText("All the beasts above have left their lair for the Dragon Island.");
                        }
                        data.AddOption("I~see.", 255).AddAvatar(53).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HauKan)]
        public static void HauKan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("<tip desc=Hau Kan has served in the Legion of Heroes for dozens of years. Even though he is very old now,he still wants to make contribution to the hunting evil, and acts as the Logistics Officer>Hau Kan</tip>:")
                            .AddText("I`m old, but I`m vigorous. If you need any supplies, find me.The devils in the Giant Chasm are digging for the <tip color=0xfff05050 desc=Collect#Chaos#Beads#by#killing#devils#in#the#Giant#Chasm.>Chaos Bead</tip>~which can enhance their power. If you`ve collected some Chaos Beads, just exchange them for supplies with me.")
                            .AddOption("Exchange~for~supplies.", 1).AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(3303483, 1) || client.Inventory.Contain(3303485, 1) || client.Inventory.Contain(3303486, 1))
                        {
                            MsgExchangeShop.ExchangeShop obj = new MsgExchangeShop.ExchangeShop();
                            obj.DwParam1 = 21875;
                            obj.DwParam2 = 23;
                            obj.DwParam3 = client.Player.UID;

                            DateTime now64 = DateTime.Now;
                            int hours = now64.Hour;
                            int minutes = now64.Minute;
                            int secounds = now64.Second;

                            int lefthours = 23 - hours;
                            int leftminutes = 60 - minutes;
                            int leftsecounds = 60 - secounds;

                            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
                            TimeSpan nextday = new TimeSpan(DateTime.Now.AddHours(lefthours).AddMinutes(leftminutes).AddSeconds(leftsecounds).Ticks);

                            obj.DwParam4 = (uint)(nextday.TotalSeconds - now.TotalSeconds);
                            client.Send(stream.CreateExchangeShop(obj));

                            client.MyExchangeShop.ShowBoughtItems(stream);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.VictoryChest)]
        public static void VictoryChest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("									*<tip desc=Heroes#who#have#killed#100#devils#in#the#Giant#Chasm#can#unlock#this#chest#and#get#wonderful#rewards.>Victory Chest</tip>* \n")
                            .AddText("--------------------------------------------------------------------------------\n")
                            .AddText("Requirement: Kill 100 devils in the Giant Chasm, once a day")
                            .AddText("Rewards: 1 Bright Star Stone for sure, and a shot to win +2 Stone, \n")
                            .AddText("	      		  Blazing CP Fragment, 150 Chi, etc. \n")
                            .AddText("--------------------------------------------------------------------------------")
                            .AddOption("I~see.", 255).AddAvatar(53).FinalizeDialog();
                        if (!client.Player.GiantGhasomKillClaimed && client.Player.GiantGhasomKill >= 300)
                        {
                            data.AddOption("Unlock~this~chest.", 1);
                        }
                        break;
                    }
                case 1:
                    {
                        // Claim once per day, only with the required kills, and only with room for both rewards.
                        if (client.Player.GiantGhasomKillClaimed || client.Player.GiantGhasomKill < 300)
                            break;
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                .AddOption("Okay.", 255).AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        client.Player.GiantGhasomKillClaimed = true;
                        client.Inventory.Add(stream, 3009001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 0, 1);
                        int rand = new Random().Next(0, 3);
                        switch (rand)
                        {
                            case 0:
                                {
                                    client.Inventory.Add(stream, 3008188, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 0, 1);
                                    break;
                                }
                            case 1:
                                {
                                    client.Inventory.Add(stream, 3301224, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 0, 1);
                                    break;
                                }
                            case 2:
                                {
                                    client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                    break;
                                }
                        }
                        client.Player.GiantGhasomKillClaimed = true;
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.YuKoon2)]
        public static void YuKoon2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("<tip desc=To#rescue#more#people#from#misery,#Yu#Koon#decided#to#join#the#Legion#of#Heroes#as#an#adviser.#His#careful#and#tactical#planning#has#helped#the#legion#won#many#battles#against#the#devil#army.>Yo Koon</tip>:")
                            .AddText("The Giant Chasm looks quite weird now. There may be an ambush at any corner. Be careful!")
                        .AddOption("Return~to~the~Dragon~Island.", 1)
                       .AddOption("Any~other~tips~for~me?", 2).AddAvatar(102).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        //client.Teleport((ushort)(455 - Program.GetRandom.Next(0, 5)), (ushort)(479 - Program.GetRandom.Next(0, 5)), 10137);
                        MsgTournaments.MsgSchedules.DragonIsland.Join(client, stream, true);
                        break;
                    }
                case 2:
                    {
                        data.AddText("							 		*Chasm of Chaos* \n")
                            .AddText("------------------------------------------------------------------------------------ \n")
                            .AddText("1. Killing devils in the Giant Chasm will bring you common treasures.")
                            .AddText("   If you have <tip color=0xfff05050 desc=Luck#refreshes#at#00:00#every#day.#Every#time#you#get#a#rare#treasure,#you#lose#some#Luck#Points.>Luck</tip> while killing, you`ll have a chance of getting rare treasures.")
                            .AddText("2. PK is allowed. You`ll be granted 1 stack of <tip color=0xfff05050 desc=Each#Bliss#lasts#for#5#minutes.#When#your#stack#of#Bliss#increases,#your#Bliss#will#be#reset#to#5#minutes.>Bliss</tip> per opponent you defeat, up to 9 stacks.")
                            .AddText("    The more stacks of your Bliss, the higher drop rate of treasures.")
                        .AddText("3. Defeating robber Sprite will definitely bring you treasures, and also 90 seconds of a random <tip color=0xfff05050 desc=1.#P-Attack#10000,#M-Attack#300002.#HP#1000003.#Break#200%4.#Immunity#200%5.#Storm#effect>buff</tip>.")
                        .AddText("4. Defeating the Queen of Evil within 8 minutes will bring you extra rewards.")
                        .AddText("5. Defeating each of other beasts within 3 minutes will bring you extra rewards. \n")
                        .AddText("------------------------------------------------------------------------------------")
                        .AddOption("I~see.", 0)
                        .AddAvatar(102).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GiantChasmPortal1)]
        public static void GiantChasmPortal1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("This portal will send you to the supply station. Do you want to use the portal now?")
                            .AddOption("Yes.", 1)
                           .AddOption("No.", 255).AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport((ushort)(67 - Program.GetRandom.Next(0, 5)), (ushort)(131 - Program.GetRandom.Next(0, 5)), 10166);
                        break;
                    }
                default:
                    break;
            }
        }
        [NpcAttribute(NpcID.GiantChasmPortal2)]
        public static void GiantChasmPortal2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("This portal will send you to the supply station. Do you want to use the portal now?")
                            .AddOption("Yes.", 1)
                           .AddOption("No.", 255).AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport((ushort)(67 - Program.GetRandom.Next(0, 5)), (ushort)(131 - Program.GetRandom.Next(0, 5)), 10166);
                        break;
                    }
                default:
                    break;
            }
        }
        [NpcAttribute(NpcID.GiantChasmPortal3)]
        public static void GiantChasmPortal3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("This portal will send you to the supply station. Do you want to use the portal now?")
                            .AddOption("Yes.", 1)
                           .AddOption("No.", 255).AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport((ushort)(67 - Program.GetRandom.Next(0, 5)), (ushort)(131 - Program.GetRandom.Next(0, 5)), 10166);
                        break;
                    }
                default:
                    break;
            }
        }
        [NpcAttribute(NpcID.GiantChasmPortal4)]
        public static void GiantChasmPortal4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("This portal will send you to the supply station. Do you want to use the portal now?")
                            .AddOption("Yes.", 1)
                           .AddOption("No.", 255).AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport((ushort)(67 - Program.GetRandom.Next(0, 5)), (ushort)(131 - Program.GetRandom.Next(0, 5)), 10166);
                        break;
                    }
                default:
                    break;
            }
        }
        [NpcAttribute(NpcID.GiantChasmPortal5)]
        public static void GiantChasmPortal5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("This portal will send you to the supply station. Do you want to use the portal now?")
                            .AddOption("Yes.", 1)
                           .AddOption("No.", 255).AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport((ushort)(67 - Program.GetRandom.Next(0, 5)), (ushort)(131 - Program.GetRandom.Next(0, 5)), 10166);
                        break;
                    }
                default:
                    break;
            }
        }
        [NpcAttribute(NpcID.RareChest)]
        public static void RareChest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("									*<tip desc=This#delicate#chest#with#vivid#dragon#pattern#contains#valuable#items.#However,#it`s#locked.>Rare Chasm Chest</tip>* \n")
                         .AddText("-------------------------------------------------------------------------------- \n")
                         .AddText("Requirement: A <tip color=0xfff05050 desc=The#devils#in#the#Giant#Chasm#may#carry#the#Rare#Chest#Key.>Rare Chest Key</tip> \n")
                         .AddText("Rewards: A chance to get Chasm Star Stone Pack, Favored Training Pill, \n")
                         .AddText("	    		  +1 Stone, Soul Scroll, etc. \n")
                         .AddText("-------------------------------------------------------------------------------- \n")
                           .AddOption("I~see.", 255).AddAvatar(67).FinalizeDialog();
                        if (client.Inventory.Contain(3303973, 1))
                        {
                            data.AddOption("Unlock~this~chest.", 1);
                        }
                        break;
                    }
                case 1:
                    {
                        //Claim Prize
                        var map = Database.Server.ServerMaps[10166];
                        ushort x = 0, y = 0;
                        map.GetRandCoord(ref x, ref y);
                        var npc = client.OnRemoveNpc = map.View.GetMapObject<Game.MsgNpc.Npc>(Conquer6609.Role.MapObjectType.Npc, id);
                        npc.X = x;
                        npc.Y = y;
                        map.RemoveNpc(client.OnRemoveNpc, stream);
                        map.AddNpc(npc);
                        switch (Program.GetRandom.Next(1, 6))
                        {
                            case 1:
                                {
                                    client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 2);
                                    client.SendSysMesage("You~received~a~+1~Bound~Stone!");
                                    break;
                                }
                            case 2:
                                {
                                    client.Inventory.AddItemWitchStack(3003124, 0, 1, stream, true);
                                    client.SendSysMesage("You~received~a~FavoredTrainingPill!");
                                    break;
                                }
                            case 3:
                                {
                                    client.Inventory.AddItemWitchStack(3007106, 0, 1, stream, true);
                                    client.SendSysMesage("You~received~a~WeaponAccessoryFragment!");
                                    break;
                                }
                            case 4:
                                {
                                    client.Inventory.AddItemWitchStack(3303483, 0, 1, stream, true);
                                    client.SendSysMesage("You~received~a~ChaosBead!");
                                    break;
                                }
                            case 5:
                                {
                                    client.Inventory.AddItemWitchStack(3004260, 0, 1, stream, true);
                                    client.SendSysMesage("You~received~a~SoulScroll!");
                                    break;
                                }
                        }
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        break;
                    }
                default:
                    break;
            }
        }
        [NpcAttribute(NpcID.EpicChest)]
        public static void EpicChest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("									*<tip desc=This#divine#chest#contains#fabulous#treasures#of#the#Holy#Dragon.#However,#it`s#locked.>Epic Chasm Chest</tip>* \n")
                         .AddText("-------------------------------------------------------------------------------- \n")
                         .AddText("Requirement: An <tip color=0xfff05050 desc=The#devils#in#the#Giant#Chasm#may#carry#the#Epic#Chest#Key.>Epic Chest Key</tip> \n")
                         .AddText("Rewards:  A chance to get 3,000 CPs, Frozen Chi Pill Scrap, \n")
                         .AddText("	    		  +3 Stone, P7 Dragon Soul, etc. \n")
                         .AddText("-------------------------------------------------------------------------------- \n")
                           .AddOption("I~see.", 255).AddAvatar(67).FinalizeDialog();
                        if (client.Inventory.Contain(3303974, 1))
                        {
                            data.AddOption("Unlock~this~chest.", 1);
                        }
                        break;
                    }
                case 1:
                    {
                        //Claim Prize
                        var map = Database.Server.ServerMaps[10166];
                        ushort x = 0, y = 0;
                        map.GetRandCoord(ref x, ref y);
                        var npc = client.OnRemoveNpc = map.View.GetMapObject<Game.MsgNpc.Npc>(Conquer6609.Role.MapObjectType.Npc, id);
                        npc.X = x;
                        npc.Y = y;
                        map.RemoveNpc(client.OnRemoveNpc, stream);
                        map.AddNpc(npc);
                        switch (Program.GetRandom.Next(1, 6))
                        {
                            case 1:
                                {
                                    client.Inventory.Add(stream, 730003, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 2);
                                    client.SendSysMesage("You~received~a~+3~Bound~Stone!");
                                    break;
                                }
                            case 2:
                                {
                                    client.Inventory.AddItemWitchStack(3007269, 0, 1, stream, true);
                                    client.SendSysMesage("You~received~a~FrozenChiPillScrap!");
                                    break;
                                }
                            case 3:
                                {
                                    client.Inventory.AddItemWitchStack(3007106, 0, 1, stream, true);
                                    client.SendSysMesage("You~received~a~WeaponAccessoryFragment!");
                                    break;
                                }
                            case 4:
                                {
                                    client.Inventory.AddItemWitchStack(3303483, 0, 1, stream, true);
                                    client.SendSysMesage("You~received~a~ChaosBead!");
                                    break;
                                }
                            case 5:
                                {
                                    client.Player.ConquerPoints += 3000;
                                    client.SendSysMesage("Obtained~3,000~Cps.");
                                    break;
                                }
                        }
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        break;
                    }
                default:
                    break;
            }
        }

        [NpcAttribute(NpcID.YuKoon)]
        public static void YuKoon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("<tip desc=To#rescue#more#people#from#misery,#Yu#Koon#decided#to#join#the#Legion#of#Heroes#as#an#adviser.#His#careful#and#tactical#planning#has#helped#the#legion#won#many#battles#against#the#devil#army.>Yo Koon</tip>:")
                            .AddText("The Queen of Evil has found the hidden treasures in the Giant Chasm. If we hurry, there`s still a~chance we could stop them from moving all the treasures out.")
                        .AddText("Rare Treasures: 3,000 CPs, P7 Dragon Soul, Knowledge Pill, etc.")
                        .AddText("Common Treasures: Meteor, Favored Training Pill (B), etc.")
                        .AddOption("Send~me~to~the~Giant~Chasm.", 1)
                        .AddOption("Return~to~the~Glowtown.", 2)
                       .AddOption("Any~other~tips~for~me?", 3).AddAvatar(102).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(65, 128, 10166);
                        break;
                    }
                case 2:
                    {
                        client.Teleport((ushort)(100 - Program.GetRandom.Next(0, 5)), (ushort)(408 - Program.GetRandom.Next(0, 5)), 10137);
                        break;
                    }
                case 3:
                    {
                        data.AddText("							 		*Chasm of Chaos*")
                            .AddText("------------------------------------------------------------------------------------")
                            .AddText("1. Killing devils in the Giant Chasm will bring you common treasures.")
                            .AddText("   If you have <tip color=0xfff05050 desc=Luck#refreshes#at#00:00#every#day.#Every#time#you#get#a#rare#treasure,#you#lose#some#Luck#Points.>Luck</tip> while killing, you`ll have a chance of getting rare treasures.")
                            .AddText("2. PK is allowed. You`ll be granted 1 stack of <tip color=0xfff05050 desc=Each#Bliss#lasts#for#5#minutes.#When#your#stack#of#Bliss#increases,#your#Bliss#will#be#reset#to#5#minutes.>Bliss</tip> per opponent you defeat, up to 9 stacks.")
                            .AddText("    The more stacks of your Bliss, the higher drop rate of treasures.")
                        .AddText("3. Defeating robber Sprite will definitely bring you treasures, and also 90 seconds of a random <tip color=0xfff05050 desc=1.#P-Attack#10000,#M-Attack#300002.#HP#1000003.#Break#200%4.#Immunity#200%5.#Storm#effect>buff</tip>.")
                        .AddText("4. Defeating the Queen of Evil within 8 minutes will bring you extra rewards.")
                        .AddText("5. Defeating each of other beasts within 3 minutes will bring you extra rewards.")
                        .AddText("------------------------------------------------------------------------------------")
                        .AddOption("I~see.", 0)
                        .AddAvatar(102).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CobbWind)]
        public static void CobbWind(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime Now64 = DateTime.Now;
            var map = Database.Server.ServerMaps[10137];
            string note = "";
            #region Note
            if (map.ContainMobID(20160))//spook
            {
                note = "[Thrilling Spook] has appeared around (349, 635) on the Dragon Island.* ";
            }
            else if (map.ContainMobID(20300))//nemsis
            {
                note = "[Nemesis Tyrant] has appeared around (568, 372) on the Dragon Island.* ";
            }
            else if (map.ContainMobID(20070))//banshee
            {
                note = "[Snow Banshee] has appeared around (658, 718) on the Dragon Island.* ";
            }

            #endregion
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("<tip desc=Cobb#Wind,#formerly#a#pirate,#was#save#by#Sky#Shan#from#a#murder.#He#has#turned#to#the#side#of#justice#since#then,#and#made#lots#of#contributions.>Cobb Wind</tip>: You came at the right time! The devils have occupied the Twisting Hell Array, and are trying to summon ")
                        .AddText("<tip color=0xfff05050 desc=Killing#Nemesis#Tyrant#brings#Savage#Bone,#P7#Weapon/Armor/Accessory#Soul#Pack#Fragment,#Senior#EXP#Nut,#etc.>Nemesis Tyrant</tip>,")
                        .AddText("~<tip color=0xfff05050 desc=Killing#Snow#Banshee#brings#P5-6#Dragon#Soul,#Dragon#Ball,#+2#Stone,#Senior#EXP#Nut,#etc.>Snow Banshee</tip> and")
                        .AddText("~<tip color=0xfff05050 desc=Killing#Thrilling#Spook#brings#P6#Dragon#Soul,#Dragon#Ball,#Senior#EXP#Nut,#etc.>Thrilling Spook</tip>")
                        .AddText("~to the Dragon Island. Commander Shan has given order to kill them at any cost. Dragon Island is not allowed to sink! \n")
                        .AddText(note)
                        .AddOption("Learn~more~about~these~beasts.", 1)
                       .AddOption("I`ll~keep~an~eye~on~them.", 255).AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (map.ContainMobID(20160) || map.ContainMobID(20300) || map.ContainMobID(20070))
                        {
                            data.AddText("<tip desc=Cobb#Wind,#formerly#a#pirate,#was#save#by#Sky#Shan#from#a#murder.#He#has#turned#to#the#side#of#justice#since#then,#and#made#lots#of#contributions.>Cobb Wind</tip>: Urgent report from the front line!")
                                .AddText(note)
                                .AddText("Many heroes are heading to fight against the beasts. Hurry and go support them!");
                            if (map.ContainMobID(20160))
                            {
                                data.AddOption("Go~kill~the~[Thrilling~Spook].", 2);
                            }
                             if (map.ContainMobID(20300))
                            {
                                data.AddOption("Go~kill~the~[Nemesis~Tyrant].", 3);
                            }
                             if (map.ContainMobID(20070))
                            {
                                data.AddOption("Go~kill~the~[Snow~Banshee].", 4);
                            }
                             if (map.ContainMobID(20060))
                            {
                                data.AddOption("Go~kill~the~[TeratoDragon].", 5);
                            }
                        }
                        else
                        {
                            data.AddText("¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª \n")
                                .AddText("     [Beast]                    [Location]       		[Appear Time] \n")
                                .AddText("Nemesis Tyrant    (DragonIsland 568,372)	   the 15th minute of each two hour, every day \n")
                                .AddText("Thrilling Spook	 (DragonIsland 349,635)   the 00th minute of each hour, every day \n")
                                .AddText("Snow Banshee	 (DragonIsland 658,718)   the 30th minute of each hour, every day \n")
                                  .AddText("Dragon	 (DragonIsland 450,180)   the 30th minute of each hour, every day \n")
                                .AddText("¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª \n")
                                .AddText("All the beasts above have left their lair for the Dragon Island.");
                        }
                        data.AddOption("I~see.", 255).AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 2://Thrilling Spook	 (DragonIsland 349,635)
                    {
                        client.Teleport(349, 635, 10137);
                        //client.Send(stream.PathFinding(client.Player.UID, 10137, 349, 635, 2010));
                        break;
                    }
                case 3://Nemesis Tyrant    (DragonIsland 568,372)
                    {
                        client.Teleport(568, 372, 10137);
                        //client.Send(stream.PathFinding(client.Player.UID, 10137, 568, 372, 20300));
                        break;
                    }
                case 4://Snow Banshee	 (DragonIsland 658,718)
                    {
                        client.Teleport(658, 718, 10137);
                        //client.Send(stream.PathFinding(client.Player.UID, 10137, 658, 718, 20070));
                        break;
                    }
                case 5://dRagon (DragonIsland 658,718)
                    {
                        client.Teleport(450, 480, 10137);
                        //client.Send(stream.PathFinding(client.Player.UID, 10137, 450,480, 20060));
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DragonIslandConductress)]
        public static void DragonIslandConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("<tip color=0xffffff00 desc=The#Conductress#inherited#the#station#from#her#father.#She#prospered#the#business#by#adopting#advance#management #methods,#and#is#praised#for#providing#excellent#teleport#services.>Conductress</tip>: ")
                        .AddText("May I help you? I only charge 100 Silver for teleport service per time. It`s free if you want to go to the Market.")
                        .AddOption("Twin~City.", 1)
                        .AddOption("Market.", 2)
                        .AddOption("Giant~Chasm.~(hunting~area)", 3)
                       .AddOption("I`m~just~passing~by.", 255).AddAvatar(1).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Player.PKPoints < 29)
                        {
                            if (client.Player.Money >= 100)
                            {
                                client.Player.Money -= 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                switch (Option)
                                {
                                    case 1: client.Teleport(314, 350, 1002); break; //TC
                                    case 2: client.Teleport(220, 220, 1036); break; //market
                                    case 3: client.Teleport(455, 481, 10137); break; //GiantChasm

                                }
                            }
                            else
                            {
                                data.AddText("Hey! You don't have 100 gold do you?\nDon't you dare try to cheat me or I'll call the guards!")
                             .AddOption("Crazy!", 255)
                             .AddAvatar(10)
                             .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I will not help MURDERS!")
                           .AddOption("Okay...", 255)
                            .AddAvatar(10)
                           .FinalizeDialog();

                        }



                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.IronTong)]
        public static void IronTong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            data.AddText("Dragon Island is deemed so dangerous that heroes below 2nd rebirth are not allowed to visit it.")
                       .AddOption("Alright.", 255).AddAvatar(55).FinalizeDialog();
                        }
                        else if (client.Player.Level < 120)
                        {
                            data.AddText("Dragon Island is deemed so dangerous that heroes below 120 Level are not allowed to visit it.")
                      .AddOption("Alright.", 255).AddAvatar(55).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("In the battle several hundred years ago, the Five Great Masters defeated the King of Evil and destroyed his Twisting Hell Array on the Dragon Island. After that,11 heroes volunteered to station on the island. Iron Tong has inherited this mission more than ten years.However,the hell devils broke in again. To get out of this crisis, he left Dragon Island for the first time.: ")
                            .AddText("The Queen of Evil and her troop suddenly attacked the Dragon Island. Obviously, she came for the Twisting Hell Array.")
                            .AddText("~I`m afraid she has figured out how to reactivate that array. We`d better not to let it happen. For the sake of humans,~please join us to defend the Dragon Island!")
                            .AddOption("Send~me~to~Dragon~Island.", 1)
                           .AddOption("I`ll~talk~to~you~later.", 255).AddAvatar(55).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        //client.Teleport((ushort)(95 - Program.GetRandom.Next(0, 5)), (ushort)(412 - Program.GetRandom.Next(0, 5)), 10137);
                        MsgTournaments.MsgSchedules.DragonIsland.Join(client, stream);
                        break;
                    }
                default:
                    break;
            }
        }
        [NpcAttribute(NpcID.GlowTownPortal1)]
        public static void GlowTownPortal1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Are you sure you want to return to Glowtown?")
                            .AddOption("Yes.", 1)
                           .AddOption("No.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport((ushort)(95 - Program.GetRandom.Next(0, 5)), (ushort)(412 - Program.GetRandom.Next(0, 5)), 10137);
                        break;
                    }
                default:
                    break;
            }
        }
        [NpcAttribute(NpcID.GlowTownPortal2)]
        public static void GlowTownPortal2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Are you sure you want to return to Glowtown?")
                            .AddOption("Yes.", 1)
                           .AddOption("No.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport((ushort)(95 - Program.GetRandom.Next(0, 5)), (ushort)(412 - Program.GetRandom.Next(0, 5)), 10137);
                        break;
                    }
                default:
                    break;
            }
        }
        [NpcAttribute(NpcID.GlowTownPortal3)]
        public static void GlowTownPortal3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Are you sure you want to return to Glowtown?")
                            .AddOption("Yes.", 1)
                           .AddOption("No.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport((ushort)(95 - Program.GetRandom.Next(0, 5)), (ushort)(412 - Program.GetRandom.Next(0, 5)), 10137);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
