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
        [NpcAttribute(NpcID.IronMineAssistant)]
        public static void IronMineAssistant(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level < 100)
                        {
                            data.AddText("Sorry~you~need~to~power~up~little~more.")
                            .AddOption("Can~you~teleport~me.", 1)
                            .AddOption("Not~interested.", 255)
                            .AddAvatar(9).FinalizeDialog();
                            break;
                        }
                        data.AddText("Dear~friend,~a~good~beginning~makes~a~good~ending.~Hello,~I~am~the~assistant~of~the~mine~union.")
                            .AddOption("Can~you~teleport~me.", 1)
                            .AddOption("Not~interested.", 255)
                            .AddAvatar(9).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(155, 93, 1028);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FiendLairGuard)]
        public static void FiendLairGuard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(720279, 1) || client.Inventory.Contain(720280, 1))
                        {
                            data.AddText("An Ancient Bead is needed to open the portals.")
                            .AddText("~They are buried under the earth.")
                            .AddText("~Take this Spade and dig for one,")
                            .AddText("~and I will be able to open the portals for you.")
                            .AddOption("Claim~a~Spade.", 12)
                            .AddOption("Hand~in~Ancient~Bead.", 20)
                            .AddOption("OK.", 255)
                            .AddOption("Back~to~the~Market.", 200)
                            .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        if (client.Player.AukKilled < 30)
                        {
                            data.AddText("I`m always at your service. But maybe you can do me a favor, first?")
                            .AddText("~Please kill 30 Thunder Auks for me. They`re getting far too noisy. I can`t stand their infernal racket!")
                            .AddText("~Once you kill 30 of them, I will open the portals for you.")
                            .AddOption("OK.~I`ll~be~back~soon.", 2)
                            .AddOption("I`ve~killed~them.", 3)
                            .AddOption("Back~to~the~Market.", 200)
                            .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        data.AddText("Nice to see a friendly face! This is the 1st floor of the Fiend`s Lair.")
                            .AddText("~The Rocking Evil is the name of the devil held on this floor. Go and kill it!")
                            .AddText("~But I guess you came to go all the way down to the 15th level. You`ll need to walk there floor by floor.")
                            .AddText("~If you like, I can open the Fiend`s Lair Portals. That should help you get to the next floor.")
                            .AddOption("Open~the~portals.", 1)
                            .AddOption("Kill~Rocking~Evil.", 150)
                            .AddOption("Back~to~the~Market.", 200)
                            .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("There, there! I can still hear them buzzing! It`s driving me mad!")
                            .AddText("~Please, kill them for me.")
                            .AddOption("OK.", 255)
                            .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Player.MessageBox("Please~kill~30~Thunder~Auks~for~the~Fiend~Lair~Guard.", null, null);
                        break;
                    }
                case 10:
                    {

                        data.AddText("An Ancient Bead is needed to open the portals.")
                            .AddText("~They are buried under the earth.")
                            .AddText("~Take this Spade and dig for one,")
                            .AddText("~and I will be able to open the portals for you.")
                            .AddOption("Claim~a~Spade.", 12)
                            .AddOption("Hand~in~Ancient~Bead.", 20)
                            .AddOption("OK.", 255)
                            .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(720279, 1))
                        {
                            data.AddText("Didn`t you claim a Spade, yet? Why not go and dig for an Ancient Bead?")
                            .AddOption("Roger.", 255)
                            .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720279);
                        Database.Server.ServerMaps[8822].GetRandCoord(ref Database.Server.AncientBeadX, ref Database.Server.AncientBeadY);
                        break;
                    }
                case 150:
                    {
                        client.Player.MessageBox("The~Rocking~Evil~is~growling~at~you!~It~doesn`t~seem~to~have~any~treasure.~Maybe~you~should~keep~hunting~on~the~next~floor.", null, null);
                        client.Send(stream.PathFinding(client.Player.UID, 8822, 195, 205, 0));
                        break;
                    }
                case 200:
                    {
                        client.Teleport(204, 200, 1036);
                        client.Player.MessageBox("You~were~teleported~to~the~Market.", null, null);
                        break;
                    }
                case 20:
                    {
                        if (!client.Inventory.Contain(720280, 1))
                        {
                            client.Player.MessageBox("You~don't~have~AncientBead.", null, null);
                            break;
                        }
                        data.AddText("Great! You`ve got an Ancient Bead. I have opened the portals for you. You can find them in the 4 rooms on this floor.")
                            .AddText("~Oh by the way, only one of the 4 portals will send you to the next floor. The magic here is too unstable to be sure")
                            .AddText("~I wish you good luck.")
                            .AddOption("Thanks.", 21)
                            .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        Database.Server.GatesOpened = true;
                        client.Player.MessageBox("All~4~portals~are~open~now.~You~can~select~one~of~them~to~try~your~luck.", null, null);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FlamePagoda_1_)]
        public static void FlamePagoda_1_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(720281, 1))
                            if (!client.Inventory.Remove(720281, 1, stream)) break;
                        data.AddText("After darkness, there is always light.")
                            .AddOption("Aye,~aye.", 255)
                            .AddAvatar(52).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FlamePagoda_2_)]
        public static void FlamePagoda_2_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(720281, 1))
                            if (!client.Inventory.Remove(720281, 1, stream)) break;
                        data.AddText("After darkness, there is always light.")
                            .AddOption("Aye,~aye.", 255)
                            .AddAvatar(52).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FlamePagoda_3_)]
        public static void FlamePagoda_3_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(720281, 1))
                            if (!client.Inventory.Remove(720281, 1, stream)) break;
                        data.AddText("After darkness, there is always light.")
                            .AddOption("Aye,~aye.", 255)
                            .AddAvatar(52).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FlamePagoda_4_)]
        public static void FlamePagoda_4_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(720281, 1))
                            if (!client.Inventory.Remove(720281, 1, stream)) break;
                        data.AddText("After darkness, there is always light.")
                            .AddOption("Aye,~aye.", 255)
                            .AddAvatar(52).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FiendLairPortal_1)]
        public static void FiendLairPortal_1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!Database.Server.GatesOpened)
                        {
                            data.AddText("Sorry, but the portals haven`t been opened, yet. Please find the Fiend`s Lair Guard for help.")
                                .AddText("~I`m sure he would be glad to open them for you!")
                            .AddOption("OK.", 255)
                            .AddAvatar(52).FinalizeDialog();
                            break;
                        }
                        data.AddText("This Fiend`s Lair portal is ready. But as unstable as they are, who knows where you could be sent!")
                            .AddText("~Are you sure you want to enter the portal now?")
                            .AddOption("Yes,~I~can`t~wait.", 1)
                            .AddOption("Wait,~wait.", 255)
                            .AddAvatar(52).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var rand = new Random().Next(1, 100);
                        if (rand == 50)
                        {
                            //Next Floor
                            break;
                        }
                        client.Teleport(192, 168, 8822);
                        client.Player.MessageBox("You~entered~the~portal,~and~were~sent~to~a~strange~place!", null, null);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FiendLairPortal_2)]
        public static void FiendLairPortal_2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!Database.Server.GatesOpened)
                        {
                            data.AddText("Sorry, but the portals haven`t been opened, yet. Please find the Fiend`s Lair Guard for help.")
                                .AddText("~I`m sure he would be glad to open them for you!")
                            .AddOption("OK.", 255)
                            .AddAvatar(52).FinalizeDialog();
                            break;
                        }
                        data.AddText("This Fiend`s Lair portal is ready. But as unstable as they are, who knows where you could be sent!")
                            .AddText("~Are you sure you want to enter the portal now?")
                            .AddOption("Yes,~I~can`t~wait.", 1)
                            .AddOption("Wait,~wait.", 255)
                            .AddAvatar(52).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var rand = new Random().Next(1, 100);
                        if (rand == 50)
                        {
                            //Next Floor
                            break;
                        }
                        client.Teleport(192, 168, 8822);
                        client.Player.MessageBox("You~entered~the~portal,~and~were~sent~to~a~strange~place!", null, null);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FiendLairPortal_3)]
        public static void FiendLairPortal_3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!Database.Server.GatesOpened)
                        {
                            data.AddText("Sorry, but the portals haven`t been opened, yet. Please find the Fiend`s Lair Guard for help.")
                                .AddText("~I`m sure he would be glad to open them for you!")
                            .AddOption("OK.", 255)
                            .AddAvatar(52).FinalizeDialog();
                            break;
                        }
                        data.AddText("This Fiend`s Lair portal is ready. But as unstable as they are, who knows where you could be sent!")
                            .AddText("~Are you sure you want to enter the portal now?")
                            .AddOption("Yes,~I~can`t~wait.", 1)
                            .AddOption("Wait,~wait.", 255)
                            .AddAvatar(52).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var rand = new Random().Next(1, 100);
                        if (rand == 50)
                        {
                            //Next Floor
                            break;
                        }
                        client.Teleport(192, 168, 8822);
                        client.Player.MessageBox("You~entered~the~portal,~and~were~sent~to~a~strange~place!", null, null);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FiendLairPortal_4)]
        public static void FiendLairPortal_4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!Database.Server.GatesOpened)
                        {
                            data.AddText("Sorry, but the portals haven`t been opened, yet. Please find the Fiend`s Lair Guard for help.")
                                .AddText("~I`m sure he would be glad to open them for you!")
                            .AddOption("OK.", 255)
                            .AddAvatar(52).FinalizeDialog();
                            break;
                        }
                        data.AddText("This Fiend`s Lair portal is ready. But as unstable as they are, who knows where you could be sent!")
                            .AddText("~Are you sure you want to enter the portal now?")
                            .AddOption("Yes,~I~can`t~wait.", 1)
                            .AddOption("Wait,~wait.", 255)
                            .AddAvatar(52).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var rand = new Random().Next(1, 100);
                        if (rand == 50)
                        {
                            //Next Floor
                            break;
                        }
                        client.Teleport(192, 168, 8822);
                        client.Player.MessageBox("You~entered~the~portal,~and~were~sent~to~a~strange~place!", null, null);
                        break;
                    }
            }
        }
        ///MoonBox
        [NpcAttribute(NpcID.Maggie)]
        public static void Maggie(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey! What are you here for? You`d better not go ahead, or you`ll get involved in a weird dimension. Look, I`m the best example!")
                            .AddText("~Maybe you want to leave? If so, I can help you.")
                            .AddOption("So,~that`s~real?", 1)
                            .AddOption("I`ve~decided~to~try.", 50)
                            .AddOption("Take~me~to~the~Life~Tactic.", 90)
                            .AddOption("I~had~better~leave~here.", 255)
                            .AddAvatar(237)
                            .FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        data.AddText("Alright, I`ll help you to get into that space, but which dimension you will be sent is unknown. Besides, it won`t be the Life or the Death at the first time. Ready?")
                            .AddOption("Okay.", 5)
                            .AddAvatar(237)
                            .FinalizeDialog();
                        break;
                    }
                case 90:
                    {
                        if (!client.Inventory.CollectedMoonBoxTokens() && !client.Inventory.CollectedMoonBoxTokens(1))
                        {
                            data.AddText("To access the Life Dimension, you should have the 6 tokens collected from different dimensions. It seems you don`t have them now. If so, I can`t teleport you there.")
                                .AddOption("Alright.", 255)
                                .AddAvatar(237)
                                .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721011, 1, stream)) break;
                        if (!client.Inventory.Remove(721013, 1, stream)) break;
                        if (!client.Inventory.Remove(721015, 1, stream)) break;
                        if (!client.Inventory.Remove(721012, 1, stream)) break;
                        if (!client.Inventory.Remove(721010, 1, stream)) break;
                        if (!client.Inventory.Remove(721014, 1, stream)) break;
                        client.Teleport(206, 161, 1050);
                        client.Inventory.Add(stream, 721072, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        break;
                    }
                case 1:
                    {
                        data.AddText("These changeful dimensions include the Peace, Chaos, Deserted, Prosperous, Disturbed, Calm, Death and Life. I once challenged one of them, but end up with my death.")
                            .AddOption("How~can~I~break~through?", 2)
                            .AddOption("I~do~not~believe~it.", 255)
                            .AddAvatar(237)
                            .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("I`ve studied it for many years, but failed to work it out. Perhaps, those fierce monsters in the dimensions have the some keys. Do you still want to try?")
                            .AddOption("Yeah.~I~like~challenge.", 3)
                            .AddOption("No.~I~changed~my~mind.", 255)
                            .AddAvatar(237)
                            .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Once you enter the dimensions, it`ll be very difficult to leave. Think it twice! I don`t want you to die for nothing.")
                            .AddOption("I`ve~made~up~my~mind.", 4)
                            .AddOption("I~changed~my~mind.", 255)
                            .AddAvatar(237)
                            .FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Alright, I`ll help you to get into that space, but which dimension you will be sent is unknown. Besides, it won`t be the Life or the Death at the first time. Ready?")
                            .AddOption("Okay.", 5)
                            .AddAvatar(237)
                            .FinalizeDialog();
                        break;
                    }
                case 5://Teleportation
                    {
                        var rand = new Random().Next(1, 7);
                        switch (rand)
                        {
                            case 1:
                                {
                                    client.Teleport(11, 68, 1043);
                                    break;
                                }
                            case 2:
                                {
                                    client.Teleport(11, 68, 1044);
                                    break;
                                }
                            case 3:
                                {
                                    client.Teleport(11, 68, 1045);
                                    break;
                                }
                            case 4:
                                {
                                    client.Teleport(11, 68, 1046);
                                    break;
                                }
                            case 5:
                                {
                                    client.Teleport(11, 68, 1047);
                                    break;
                                }
                            case 6:
                                {
                                    client.Teleport(11, 68, 1048);
                                    break;
                                }
                            default:
                                client.Teleport(11, 68, 1043);
                                break;
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_1046_)]
        public static void Ghost_1046_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Who I am means nothing. If you`re able to break through this dimension, your name will be remembered! Try your best to defeat the monsters here, and collect the vital token.")
                            .AddOption("Take~me~to~the~Life~Tactic.", 1)
                            .AddOption("How~can~I~conquer?", 3)
                            .AddOption("What~nonsense!", 255)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.CollectedMoonBoxTokens() && !client.Inventory.CollectedMoonBoxTokens(1))
                        {
                            data.AddText("To access the Life Dimension, you should have the 6 tokens collected from different dimensions. It seems you don`t have them now. If so, I can`t teleport you there.")
                                .AddOption("Alright.", 255)
                                .AddAvatar(237)
                                .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721011, 1, stream)) break;
                        if (!client.Inventory.Remove(721013, 1, stream)) break;
                        if (!client.Inventory.Remove(721015, 1, stream)) break;
                        if (!client.Inventory.Remove(721012, 1, stream)) break;
                        if (!client.Inventory.Remove(721010, 1, stream)) break;
                        if (!client.Inventory.Remove(721014, 1, stream)) break;
                        client.Teleport(206, 161, 1050);
                        client.Inventory.Add(stream, 721072, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        break;
                    }
                case 3:
                    {


                        if (client.Inventory.CollectedTokens(client.Player.Map))
                        {
                            data.AddText("Great job! You got the token for the Prosperous Dimension! My only wish is to see a hero break through all of these dimensions. Wish you succeed!")
                            .AddOption("Why~not~leave~here?", 4)
                            .AddAvatar(237).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Just do not stop hunting monsters in this dimension until you collect a token that acts as the key to get out. I wait for your return!")
                            .AddOption("I~must~prove~my~ability.", 255)
                            .AddAvatar(237).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("It`s not time to leave. I know you are going to challenge other dimensions. Good luck!")
                            .AddOption("How~can~I~break~through?", 5).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("According to my experience, the key dimension is the Death, but I do not know how it works. You may try the other dimensions first.")
                            .AddOption("How~can~I~leave?", 6).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Though I don`t really know how to break through these dimensions, I can help you leave here. Talk to Maggie again and continue your challenge.")
                            .AddOption("I~must~try~my~best.", 7).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("So, are you ready to leave this dimension? I am going to teleport you out right now.")
                            .AddOption("I~am~ready.", 8).AddOption("I~shall~leave~later.", 255).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        client.Teleport(23, 24, 1042);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_1044_)]
        public static void Ghost_1044_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Who I am means nothing. If you`re able to break through this dimension, your name will be remembered! Try your best to defeat the monsters here, and collect the vital token.")
                            .AddOption("Take~me~to~the~Life~Tactic.", 1)
                            .AddOption("How~can~I~conquer?", 3)
                            .AddOption("What~nonsense!", 255)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.CollectedMoonBoxTokens() && !client.Inventory.CollectedMoonBoxTokens(1))
                        {
                            data.AddText("To access the Life Dimension, you should have the 6 tokens collected from different dimensions. It seems you don`t have them now. If so, I can`t teleport you there.")
                                .AddOption("Alright.", 255)
                                .AddAvatar(237)
                                .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721011, 1, stream)) break;
                        if (!client.Inventory.Remove(721013, 1, stream)) break;
                        if (!client.Inventory.Remove(721015, 1, stream)) break;
                        if (!client.Inventory.Remove(721012, 1, stream)) break;
                        if (!client.Inventory.Remove(721010, 1, stream)) break;
                        if (!client.Inventory.Remove(721014, 1, stream)) break;
                        client.Teleport(206, 161, 1050);
                        client.Inventory.Add(stream, 721072, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        break;
                    }
                case 3:
                    {


                        if (client.Inventory.CollectedTokens(client.Player.Map))
                        {
                            data.AddText("Great job! You got the token for the Chaos Dimension! My only wish is to see a hero break through all of these dimensions. Wish you succeed!")
                            .AddOption("Why~not~leave~here?", 4)
                            .AddAvatar(237).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Just do not stop hunting monsters in this dimension until you collect a token that acts as the key to get out. I wait for your return!")
                            .AddOption("I~must~prove~my~ability.", 255)
                            .AddAvatar(237).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("It`s not time to leave. I know you are going to challenge other dimensions. Good luck!")
                            .AddOption("How~can~I~break~through?", 5).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("According to my experience, the key dimension is the Death, but I do not know how it works. You may try the other dimensions first.")
                            .AddOption("How~can~I~leave?", 6).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Though I don`t really know how to break through these dimensions, I can help you leave here. Talk to Maggie again and continue your challenge.")
                            .AddOption("I~must~try~my~best.", 7).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("So, are you ready to leave this dimension? I am going to teleport you out right now.")
                            .AddOption("I~am~ready.", 8).AddOption("I~shall~leave~later.", 255).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        client.Teleport(23, 24, 1042);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_1048_)]
        public static void Ghost_1048_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Who I am means nothing. If you`re able to break through this dimension, your name will be remembered! Try your best to defeat the monsters here, and collect the vital token.")
                            .AddOption("Take~me~to~the~Life~Tactic.", 1)
                            .AddOption("How~can~I~conquer?", 3)
                            .AddOption("What~nonsense!", 255)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.CollectedMoonBoxTokens() && !client.Inventory.CollectedMoonBoxTokens(1))
                        {
                            data.AddText("To access the Life Dimension, you should have the 6 tokens collected from different dimensions. It seems you don`t have them now. If so, I can`t teleport you there.")
                                .AddOption("Alright.", 255)
                                .AddAvatar(237)
                                .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721011, 1, stream)) break;
                        if (!client.Inventory.Remove(721013, 1, stream)) break;
                        if (!client.Inventory.Remove(721015, 1, stream)) break;
                        if (!client.Inventory.Remove(721012, 1, stream)) break;
                        if (!client.Inventory.Remove(721010, 1, stream)) break;
                        if (!client.Inventory.Remove(721014, 1, stream)) break;
                        client.Teleport(206, 161, 1050);
                        client.Inventory.Add(stream, 721072, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        break;
                    }
                case 3:
                    {


                        if (client.Inventory.CollectedTokens(client.Player.Map))
                        {
                            data.AddText("Great job! You got the token for the Calm Dimension! My only wish is to see a hero break through all of these dimensions. Wish you succeed!")
                            .AddOption("Why~not~leave~here?", 4)
                            .AddAvatar(237).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Just do not stop hunting monsters in this dimension until you collect a token that acts as the key to get out. I wait for your return!")
                            .AddOption("I~must~prove~my~ability.", 255)
                            .AddAvatar(237).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("It`s not time to leave. I know you are going to challenge other dimensions. Good luck!")
                            .AddOption("How~can~I~break~through?", 5).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("According to my experience, the key dimension is the Death, but I do not know how it works. You may try the other dimensions first.")
                            .AddOption("How~can~I~leave?", 6).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Though I don`t really know how to break through these dimensions, I can help you leave here. Talk to Maggie again and continue your challenge.")
                            .AddOption("I~must~try~my~best.", 7).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("So, are you ready to leave this dimension? I am going to teleport you out right now.")
                            .AddOption("I~am~ready.", 8).AddOption("I~shall~leave~later.", 255).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        client.Teleport(23, 24, 1042);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_1045_)]
        public static void Ghost_1045_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Who I am means nothing. If you`re able to break through this dimension, your name will be remembered! Try your best to defeat the monsters here, and collect the vital token.")
                            .AddOption("Take~me~to~the~Life~Tactic.", 1)
                            .AddOption("How~can~I~conquer?", 3)
                            .AddOption("What~nonsense!", 255)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.CollectedMoonBoxTokens() && !client.Inventory.CollectedMoonBoxTokens(1))
                        {
                            data.AddText("To access the Life Dimension, you should have the 6 tokens collected from different dimensions. It seems you don`t have them now. If so, I can`t teleport you there.")
                                .AddOption("Alright.", 255)
                                .AddAvatar(237)
                                .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721011, 1, stream)) break;
                        if (!client.Inventory.Remove(721013, 1, stream)) break;
                        if (!client.Inventory.Remove(721015, 1, stream)) break;
                        if (!client.Inventory.Remove(721012, 1, stream)) break;
                        if (!client.Inventory.Remove(721010, 1, stream)) break;
                        if (!client.Inventory.Remove(721014, 1, stream)) break;
                        client.Teleport(206, 161, 1050);
                        client.Inventory.Add(stream, 721072, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        break;
                    }
                case 3:
                    {


                        if (client.Inventory.CollectedTokens(client.Player.Map))
                        {
                            data.AddText("Great job! You got the token for the Deserted Dimension! My only wish is to see a hero break through all of these dimensions. Wish you succeed!")
                            .AddOption("Why~not~leave~here?", 4)
                            .AddAvatar(237).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Just do not stop hunting monsters in this dimension until you collect a token that acts as the key to get out. I wait for your return!")
                            .AddOption("I~must~prove~my~ability.", 255)
                            .AddAvatar(237).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("It`s not time to leave. I know you are going to challenge other dimensions. Good luck!")
                            .AddOption("How~can~I~break~through?", 5).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("According to my experience, the key dimension is the Death, but I do not know how it works. You may try the other dimensions first.")
                            .AddOption("How~can~I~leave?", 6).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Though I don`t really know how to break through these dimensions, I can help you leave here. Talk to Maggie again and continue your challenge.")
                            .AddOption("I~must~try~my~best.", 7).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("So, are you ready to leave this dimension? I am going to teleport you out right now.")
                            .AddOption("I~am~ready.", 8).AddOption("I~shall~leave~later.", 255).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        client.Teleport(23, 24, 1042);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_1043_)]
        public static void Ghost_1043_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Who I am means nothing. If you`re able to break through this dimension, your name will be remembered! Try your best to defeat the monsters here, and collect the vital token.")
                            .AddOption("Take~me~to~the~Life~Tactic.", 1)
                            .AddOption("How~can~I~conquer?", 3)
                            .AddOption("What~nonsense!", 255)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.CollectedMoonBoxTokens() && !client.Inventory.CollectedMoonBoxTokens(1))
                        {
                            data.AddText("To access the Life Dimension, you should have the 6 tokens collected from different dimensions. It seems you don`t have them now. If so, I can`t teleport you there.")
                                .AddOption("Alright.", 255)
                                .AddAvatar(237)
                                .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721011, 1, stream)) break;
                        if (!client.Inventory.Remove(721013, 1, stream)) break;
                        if (!client.Inventory.Remove(721015, 1, stream)) break;
                        if (!client.Inventory.Remove(721012, 1, stream)) break;
                        if (!client.Inventory.Remove(721010, 1, stream)) break;
                        if (!client.Inventory.Remove(721014, 1, stream)) break;
                        client.Teleport(206, 161, 1050);
                        client.Inventory.Add(stream, 721072, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        break;
                    }
                case 3:
                    {


                        if (client.Inventory.CollectedTokens(client.Player.Map))
                        {
                            data.AddText("Great job! You got the token for the Peace Dimension! My only wish is to see a hero break through all of these dimensions. Wish you succeed!")
                            .AddOption("Why~not~leave~here?", 4)
                            .AddAvatar(237).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Just do not stop hunting monsters in this dimension until you collect a token that acts as the key to get out. I wait for your return!")
                            .AddOption("I~must~prove~my~ability.", 255)
                            .AddAvatar(237).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("It`s not time to leave. I know you are going to challenge other dimensions. Good luck!")
                            .AddOption("How~can~I~break~through?", 5).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("According to my experience, the key dimension is the Death, but I do not know how it works. You may try the other dimensions first.")
                            .AddOption("How~can~I~leave?", 6).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Though I don`t really know how to break through these dimensions, I can help you leave here. Talk to Maggie again and continue your challenge.")
                            .AddOption("I~must~try~my~best.", 7).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("So, are you ready to leave this dimension? I am going to teleport you out right now.")
                            .AddOption("I~am~ready.", 8).AddOption("I~shall~leave~later.", 255).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        client.Teleport(23, 24, 1042);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_1047_)]
        public static void Ghost_1047_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Who I am means nothing. If you`re able to break through this dimension, your name will be remembered! Try your best to defeat the monsters here, and collect the vital token.")
                            .AddOption("Take~me~to~the~Life~Tactic.", 1)
                            .AddOption("How~can~I~conquer?", 3)
                            .AddOption("What~nonsense!", 255)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.CollectedMoonBoxTokens() && !client.Inventory.CollectedMoonBoxTokens(1))
                        {
                            data.AddText("To access the Life Dimension, you should have the 6 tokens collected from different dimensions. It seems you don`t have them now. If so, I can`t teleport you there.")
                                .AddOption("Alright.", 255)
                                .AddAvatar(237)
                                .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721011, 1, stream)) break;
                        if (!client.Inventory.Remove(721013, 1, stream)) break;
                        if (!client.Inventory.Remove(721015, 1, stream)) break;
                        if (!client.Inventory.Remove(721012, 1, stream)) break;
                        if (!client.Inventory.Remove(721010, 1, stream)) break;
                        if (!client.Inventory.Remove(721014, 1, stream)) break;
                        client.Teleport(206, 161, 1050);
                        client.Inventory.Add(stream, 721072, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        break;
                    }
                case 3:
                    {


                        if (client.Inventory.CollectedTokens(client.Player.Map))
                        {
                            data.AddText("Great job! You got the token for the Disturbed Dimension! My only wish is to see a hero break through all of these dimensions. Wish you succeed!")
                            .AddOption("Why~not~leave~here?", 4)
                            .AddAvatar(237).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Just do not stop hunting monsters in this dimension until you collect a token that acts as the key to get out. I wait for your return!")
                            .AddOption("I~must~prove~my~ability.", 255)
                            .AddAvatar(237).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("It`s not time to leave. I know you are going to challenge other dimensions. Good luck!")
                            .AddOption("How~can~I~break~through?", 5).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("According to my experience, the key dimension is the Death, but I do not know how it works. You may try the other dimensions first.")
                            .AddOption("How~can~I~leave?", 6).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Though I don`t really know how to break through these dimensions, I can help you leave here. Talk to Maggie again and continue your challenge.")
                            .AddOption("I~must~try~my~best.", 7).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("So, are you ready to leave this dimension? I am going to teleport you out right now.")
                            .AddOption("I~am~ready.", 8).AddOption("I~shall~leave~later.", 255).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        client.Teleport(23, 24, 1042);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.VagrantGhost_1_)]
        public static void VagrantGhost_1_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("We~have~been~trapped~here~for~many~years.~Few~challengers~have~a~chance~to~arrive~here.~You~are~a~real~conqueror.")
                            .AddOption("Who~are~you?", 1)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Finally, I got a person alive! Do you want to know my story? Yeah, of course! We were from an old country. Our rivals made these mystic dimensions and trapped us to death.")
                            .AddOption("That`s~terrible.", 2)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("A kind-hearted person like you will never leave a lonely ghost like me in distress, right? So, could you give me a hand and help me get reborn?")
                            .AddOption("How~can~I~help~you?", 3)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Contain(721072, 1))
                        {
                            data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. but you don't have one please bring me one.")
                                .AddOption("okay~i~will~come~back~for~you.", 255)
                                .AddAvatar(237).FinalizeDialog();
                            break;
                        }
                        data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. However, each Soul Jade can only be used to save one life at a time. Choose me, please!")
                                .AddOption("No~problem.", 4)
                                .AddOption("Sorry,~I~am~very~busy.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("I`ll never forget your help. Look, here is a Moon Box for you, but what you can get from it depends on your luck. Do you want to claim it?")
                                .AddOption("Yeah,~of~course!", 5)
                                .AddOption("No,~thanks.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(721072, 1))
                        {
                            if (!client.Inventory.Remove(721072, 1, stream)) break;
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721014);
                            }
                            else
                            {
                                client.Inventory.AddReturnedItem(stream, 721014);
                            }
                            client.Teleport(23, 24, 1042);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.VagrantGhost_2_)]
        public static void VagrantGhost_2_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("We~have~been~trapped~here~for~many~years.~Few~challengers~have~a~chance~to~arrive~here.~You~are~a~real~conqueror.")
                            .AddOption("Who~are~you?", 1)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Finally, I got a person alive! Do you want to know my story? Yeah, of course! We were from an old country. Our rivals made these mystic dimensions and trapped us to death.")
                            .AddOption("That`s~terrible.", 2)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("A kind-hearted person like you will never leave a lonely ghost like me in distress, right? So, could you give me a hand and help me get reborn?")
                            .AddOption("How~can~I~help~you?", 3)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Contain(721072, 1))
                        {
                            data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. but you don't have one please bring me one.")
                                .AddOption("okay~i~will~come~back~for~you.", 255)
                                .AddAvatar(237).FinalizeDialog();
                            break;
                        }
                        data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. However, each Soul Jade can only be used to save one life at a time. Choose me, please!")
                                .AddOption("No~problem.", 4)
                                .AddOption("Sorry,~I~am~very~busy.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("I`ll never forget your help. Look, here is a Moon Box for you, but what you can get from it depends on your luck. Do you want to claim it?")
                                .AddOption("Yeah,~of~course!", 5)
                                .AddOption("No,~thanks.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(721072, 1))
                        {
                            if (!client.Inventory.Remove(721072, 1, stream)) break;
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721014);
                            }
                            else
                            {
                                client.Inventory.AddReturnedItem(stream, 721014);
                            }
                            client.Teleport(23, 24, 1042);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.VagrantGhost_3_)]
        public static void VagrantGhost_3_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("We~have~been~trapped~here~for~many~years.~Few~challengers~have~a~chance~to~arrive~here.~You~are~a~real~conqueror.")
                            .AddOption("Who~are~you?", 1)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Finally, I got a person alive! Do you want to know my story? Yeah, of course! We were from an old country. Our rivals made these mystic dimensions and trapped us to death.")
                            .AddOption("That`s~terrible.", 2)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("A kind-hearted person like you will never leave a lonely ghost like me in distress, right? So, could you give me a hand and help me get reborn?")
                            .AddOption("How~can~I~help~you?", 3)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Contain(721072, 1))
                        {
                            data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. but you don't have one please bring me one.")
                                .AddOption("okay~i~will~come~back~for~you.", 255)
                                .AddAvatar(237).FinalizeDialog();
                            break;
                        }
                        data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. However, each Soul Jade can only be used to save one life at a time. Choose me, please!")
                                .AddOption("No~problem.", 4)
                                .AddOption("Sorry,~I~am~very~busy.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("I`ll never forget your help. Look, here is a Moon Box for you, but what you can get from it depends on your luck. Do you want to claim it?")
                                .AddOption("Yeah,~of~course!", 5)
                                .AddOption("No,~thanks.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(721072, 1))
                        {
                            if (!client.Inventory.Remove(721072, 1, stream)) break;
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721014);
                            }
                            else
                            {
                                client.Inventory.AddReturnedItem(stream, 721014);
                            }
                            client.Teleport(23, 24, 1042);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.VagrantGhost_4_)]
        public static void VagrantGhost_4_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("We~have~been~trapped~here~for~many~years.~Few~challengers~have~a~chance~to~arrive~here.~You~are~a~real~conqueror.")
                            .AddOption("Who~are~you?", 1)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Finally, I got a person alive! Do you want to know my story? Yeah, of course! We were from an old country. Our rivals made these mystic dimensions and trapped us to death.")
                            .AddOption("That`s~terrible.", 2)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("A kind-hearted person like you will never leave a lonely ghost like me in distress, right? So, could you give me a hand and help me get reborn?")
                            .AddOption("How~can~I~help~you?", 3)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Contain(721072, 1))
                        {
                            data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. but you don't have one please bring me one.")
                                .AddOption("okay~i~will~come~back~for~you.", 255)
                                .AddAvatar(237).FinalizeDialog();
                            break;
                        }
                        data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. However, each Soul Jade can only be used to save one life at a time. Choose me, please!")
                                .AddOption("No~problem.", 4)
                                .AddOption("Sorry,~I~am~very~busy.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("I`ll never forget your help. Look, here is a Moon Box for you, but what you can get from it depends on your luck. Do you want to claim it?")
                                .AddOption("Yeah,~of~course!", 5)
                                .AddOption("No,~thanks.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(721072, 1))
                        {
                            if (!client.Inventory.Remove(721072, 1, stream)) break;
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721014);
                            }
                            else
                            {
                                client.Inventory.AddReturnedItem(stream, 721014);
                            }
                            client.Teleport(23, 24, 1042);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.VagrantGhost_5_)]
        public static void VagrantGhost_5_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("We~have~been~trapped~here~for~many~years.~Few~challengers~have~a~chance~to~arrive~here.~You~are~a~real~conqueror.")
                            .AddOption("Who~are~you?", 1)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Finally, I got a person alive! Do you want to know my story? Yeah, of course! We were from an old country. Our rivals made these mystic dimensions and trapped us to death.")
                            .AddOption("That`s~terrible.", 2)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("A kind-hearted person like you will never leave a lonely ghost like me in distress, right? So, could you give me a hand and help me get reborn?")
                            .AddOption("How~can~I~help~you?", 3)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Contain(721072, 1))
                        {
                            data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. but you don't have one please bring me one.")
                                .AddOption("okay~i~will~come~back~for~you.", 255)
                                .AddAvatar(237).FinalizeDialog();
                            break;
                        }
                        data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. However, each Soul Jade can only be used to save one life at a time. Choose me, please!")
                                .AddOption("No~problem.", 4)
                                .AddOption("Sorry,~I~am~very~busy.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("I`ll never forget your help. Look, here is a Moon Box for you, but what you can get from it depends on your luck. Do you want to claim it?")
                                .AddOption("Yeah,~of~course!", 5)
                                .AddOption("No,~thanks.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(721072, 1))
                        {
                            if (!client.Inventory.Remove(721072, 1, stream)) break;
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721014);
                            }
                            else
                            {
                                client.Inventory.AddReturnedItem(stream, 721014);
                            }
                            client.Teleport(23, 24, 1042);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.VagrantGhost_6_)]
        public static void VagrantGhost_6_(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("We~have~been~trapped~here~for~many~years.~Few~challengers~have~a~chance~to~arrive~here.~You~are~a~real~conqueror.")
                            .AddOption("Who~are~you?", 1)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Finally, I got a person alive! Do you want to know my story? Yeah, of course! We were from an old country. Our rivals made these mystic dimensions and trapped us to death.")
                            .AddOption("That`s~terrible.", 2)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("A kind-hearted person like you will never leave a lonely ghost like me in distress, right? So, could you give me a hand and help me get reborn?")
                            .AddOption("How~can~I~help~you?", 3)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Contain(721072, 1))
                        {
                            data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. but you don't have one please bring me one.")
                                .AddOption("okay~i~will~come~back~for~you.", 255)
                                .AddAvatar(237).FinalizeDialog();
                            break;
                        }
                        data.AddText("You can, of course! With the power of Soul Jade, I`ll be able to leave and get revived. However, each Soul Jade can only be used to save one life at a time. Choose me, please!")
                                .AddOption("No~problem.", 4)
                                .AddOption("Sorry,~I~am~very~busy.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("I`ll never forget your help. Look, here is a Moon Box for you, but what you can get from it depends on your luck. Do you want to claim it?")
                                .AddOption("Yeah,~of~course!", 5)
                                .AddOption("No,~thanks.", 255)
                                .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(721072, 1))
                        {
                            if (!client.Inventory.Remove(721072, 1, stream)) break;
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721014);
                            }
                            else
                            {
                                client.Inventory.AddReturnedItem(stream, 721014);
                            }
                            client.Teleport(23, 24, 1042);
                        }
                        break;
                    }
            }
        }
        ///

        [NpcAttribute(NpcID.CloudBeast)]
        public static void CloudBeast(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            client.SendSysMesage("You took courge to touch the Cloud Beast, and found this giant creature is quiet and gentle.");
        }
    }
}
