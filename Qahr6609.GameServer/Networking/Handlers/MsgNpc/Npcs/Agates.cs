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

        public static void Agate7(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can claim experience from the Agate for every 90 guardians you have defeated. Also, you can use the Agate to proceed to an advanced stage, to withdraw to a lower stage or to return to Twin City.");
                        data.AddOption("Claim~Experience~and~proceed.", 10);
                        data.AddOption("Claim~Experience.", 1);
                        data.AddOption("Quit~Arena.", 30);
                        data.AddAvatar(31);
                        data.FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 90)
                            {
                                data.AddText("You`ve defeated 90 Arena Guardians! Which reward would you like, EXP or 1 Refinery Badge?");
                                data.AddOption("EXP~worth~1~EXP~Ball.", 12);
                                data.AddOption("1~Refinery~Badges.", 13);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 90 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.HaveSpace(1) && client.Player.DefeatedArenaGuardians >= 90)
                        {
                            client.Player.DefeatedArenaGuardians -= 90;
                            client.Inventory.AddItemWitchStack(720148, 0, 1, stream);
                            client.SendSysMesage("You`ve received 1 Refinery Badge!", MsgMessage.ChatMode.System);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(89, 60, 1775);
                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(78, 73, 1775);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(63, 85, 1775);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(58, 29, 1775);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(27, 53, 1775);
                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(34, 5, 1775);
                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(20, 20, 1775);
                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(6, 35, 1775);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 90)
                        {
                            client.Player.DefeatedArenaGuardians -= 90;
                            client.GainExpBall(600);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth an Exp Ball!", MsgMessage.ChatMode.System);
                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(89, 60, 1775);
                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(78, 73, 1775);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(63, 85, 1775);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(58, 29, 1775);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(27, 53, 1775);
                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(34, 5, 1775);
                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(20, 20, 1775);
                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(6, 35, 1775);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You need to defeat 90 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 90)
                            {
                                data.AddText("You`ve defeated 90 Arena Guardians! Which reward would you like, EXP or 1 Refinery Badge?");
                                data.AddOption("EXP~worth~1~EXP~Ball.", 2);
                                data.AddOption("1~Refinery~Badges.", 3);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 90 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1) && client.Player.DefeatedArenaGuardians >= 90)
                        {
                            client.Player.DefeatedArenaGuardians -= 90;
                            client.Inventory.AddItemWitchStack(720148, 0, 1, stream);
                            client.SendSysMesage("You`ve received 1 Refinery Badge!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 90)
                        {
                            client.Player.DefeatedArenaGuardians -= 90;
                            client.GainExpBall(600);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth an Exp Ball!", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            data.AddText("You need to defeat 90 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("If you quite the Power Arena now, you need to wait for 10 minutes to enter the arena again. Are you sure you want to quit now?");
                        data.AddOption("Yes,~I~am~sure.", 31);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.Teleport(300, 280, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Agate6)]
        public static void Agate6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can claim experience from the Agate for every 60 guardians you have defeated. Also, you can use the Agate to proceed to an advanced stage, to withdraw to a lower stage or to return to Twin City.");
                        data.AddOption("Claim~Experience~and~proceed.", 10);
                        data.AddOption("Claim~Experience.", 1);
                        data.AddOption("Withdraw~to~Stage~7.", 20);
                        data.AddOption("Quit~Arena.", 30);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 60)
                            {
                                data.AddText("You`ve defeated 60 Arena Guardians! Which reward would you like, EXP or 1 Refinery Badge?");
                                data.AddOption("EXP~worth~1~EXP~Ball.", 2);
                                data.AddOption("1~Refinery~Badges.", 3);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 60 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1) && client.Player.DefeatedArenaGuardians >= 60)
                        {
                            client.Player.DefeatedArenaGuardians -= 60;
                            client.Inventory.AddItemWitchStack(720148, 0, 1, stream);
                            client.SendSysMesage("You`ve received 1 Refinery Badge!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 60)
                        {
                            client.Player.DefeatedArenaGuardians -= 60;
                            client.GainExpBall(600);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth an Exp Ball!", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            data.AddText("You need to defeat 60 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 60)
                            {
                                data.AddText("You`ve defeated 60 Arena Guardians! Which reward would you like, EXP or 1 Refinery Badge?");
                                data.AddOption("EXP~worth~1~EXP~Ball.", 12);
                                data.AddOption("1~Refinery~Badges.", 13);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 60 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.HaveSpace(1) && client.Player.DefeatedArenaGuardians >= 60)
                        {
                            client.Player.DefeatedArenaGuardians -= 60;
                            client.Inventory.AddItemWitchStack(720148, 0, 1, stream);
                            client.SendSysMesage("You`ve received 1 Refinery Badge!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(90, 61, 1774);
                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(73, 79, 1774);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(62, 89, 1774);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(44, 16, 1774);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(20, 48, 1774);
                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(34, 8, 1774);
                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(24, 25, 1774);
                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(21, 37, 1774);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 60)
                        {
                            client.Player.DefeatedArenaGuardians -= 60;
                            client.GainExpBall(600);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth an Exp Ball!", MsgMessage.ChatMode.System);
                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(90, 61, 1774);
                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(73, 79, 1774);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(62, 89, 1774);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(44, 16, 1774);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(20, 48, 1774);
                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(34, 8, 1774);
                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(24, 25, 1774);
                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(21, 37, 1774);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You need to defeat 60 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("After you enter a lower stage, there will be some requirements for you to meet before advancing a stage. Are you sure you want to enter a lower stage?");
                        data.AddOption("Enter~a~lower~stage.", 21);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        switch (Program.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    client.Teleport(29, 28, 1777);
                                    break;
                                }
                            case 1:
                                {
                                    client.Teleport(53, 34, 1777);
                                    break;
                                }
                            case 2:
                                {
                                    client.Teleport(67, 54, 1777);
                                    break;
                                }
                            case 3:
                                {
                                    client.Teleport(63, 78, 1777);
                                    break;
                                }
                            case 4:
                                {
                                    client.Teleport(38, 59, 1777);
                                    break;
                                }
                            case 5:
                                {
                                    client.Teleport(25, 37, 1777);
                                    break;
                                }
                            case 6:
                                {
                                    client.Teleport(48, 34, 1777);
                                    break;
                                }
                            case 7:
                                {
                                    client.Teleport(34, 44, 1777);
                                    break;
                                }
                        }
                        break;
                    }

                case 30:
                    {
                        data.AddText("If you quite the Power Arena now, you need to wait for 10 minutes to enter the arena again. Are you sure you want to quit now?");
                        data.AddOption("Yes,~I~am~sure.", 31);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.Teleport(300, 280, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Agate5)]
        public static void Agate5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can claim experience from the Agate for every 45 guardians you have defeated. Also, you can use the Agate to proceed to an advanced stage, to withdraw to a lower stage or to return to Twin City.");
                        data.AddOption("Claim~Experience~and~proceed.", 10);
                        data.AddOption("Claim~Experience.", 1);
                        data.AddOption("Withdraw~to~Stage~6.", 20);
                        data.AddOption("Quit~Arena.", 30);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 45)
                            {
                                data.AddText("You`ve defeated 45 Arena Guardians! Which reward would you like, EXP or 1 Refinery Badge?");
                                data.AddOption("EXP~worth~1~EXP~Ball.", 12);
                                data.AddOption("1~Refinery~Badges.", 13);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 45 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.HaveSpace(1) && client.Player.DefeatedArenaGuardians >= 45)
                        {
                            client.Player.DefeatedArenaGuardians -= 45;
                            client.Inventory.AddItemWitchStack(720148, 0, 1, stream);
                            client.SendSysMesage("You`ve received 1 Refinery Badge!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(39, 66, 1773);
                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(65, 37, 1773);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(61, 88, 1773);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(74, 78, 1773);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(89, 60, 1773);
                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(35, 6, 1773);
                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(9, 36, 1773);
                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(22, 20, 1773);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 45)
                        {
                            client.Player.DefeatedArenaGuardians -= 45;
                            client.GainExpBall(600);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth an Exp Ball!", MsgMessage.ChatMode.System);
                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(39, 66, 1773);
                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(65, 37, 1773);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(61, 88, 1773);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(74, 78, 1773);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(89, 60, 1773);
                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(35, 6, 1773);
                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(9, 36, 1773);
                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(22, 20, 1773);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You need to defeat 45 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 45)
                            {
                                data.AddText("You`ve defeated 45 Arena Guardians! Which reward would you like, EXP or 1 Refinery Badge?");
                                data.AddOption("EXP~worth~1~EXP~Ball.", 2);
                                data.AddOption("1~Refinery~Badges.", 3);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 45 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1) && client.Player.DefeatedArenaGuardians >= 45)
                        {
                            client.Player.DefeatedArenaGuardians -= 45;
                            client.Inventory.AddItemWitchStack(720148, 0, 1, stream);
                            client.SendSysMesage("You`ve received 1 Refinery Badge!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");

                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 45)
                        {
                            client.Player.DefeatedArenaGuardians -= 45;
                            client.GainExpBall(600);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth an Exp Ball!", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            data.AddText("You need to defeat 45 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("After you enter a lower stage, there will be some requirements for you to meet before advancing a stage. Are you sure you want to enter a lower stage?");
                        data.AddOption("Enter~a~lower~stage.", 21);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        switch (Program.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    client.Teleport(89, 60, 1775);
                                    break;
                                }
                            case 1:
                                {
                                    client.Teleport(78, 73, 1775);
                                    break;
                                }
                            case 2:
                                {
                                    client.Teleport(63, 85, 1775);
                                    break;
                                }
                            case 3:
                                {
                                    client.Teleport(58, 29, 1775);
                                    break;
                                }
                            case 4:
                                {
                                    client.Teleport(27, 53, 1775);
                                    break;
                                }
                            case 5:
                                {
                                    client.Teleport(34, 5, 1775);
                                    break;
                                }
                            case 6:
                                {
                                    client.Teleport(20, 20, 1775);
                                    break;
                                }
                            case 7:
                                {
                                    client.Teleport(6, 35, 1775);
                                    break;
                                }
                        }
                        break;
                    }

                case 30:
                    {
                        data.AddText("If you quite the Power Arena now, you need to wait for 10 minutes to enter the arena again. Are you sure you want to quit now?");
                        data.AddOption("Yes,~I~am~sure.", 31);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.Teleport(300, 280, 1002);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Agate4)]
        public static void Agate4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {//5004100
                        data.AddText("You can claim experience from the Agate for every 35 guardians you have defeated. Also, you can use the Agate to proceed to an advanced stage, to withdraw to a lower stage or to return to Twin City.");
                        data.AddOption("Claim~Experience~and~proceed.", 10);
                        data.AddOption("Claim~Experience.", 1);
                        data.AddOption("Withdraw~to~Stage~5.", 20);
                        data.AddOption("Quit~Arena.", 30);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 35)
                            {
                                data.AddText("You`ve defeated 35 Arena Guardians! Which reward would you like, EXP or 1 Refinery Badge?");
                                data.AddOption("Half~of~an~EXP~Ball.", 2);
                                data.AddOption("1~Refinery~Badges.", 3);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 35 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1) && client.Player.DefeatedArenaGuardians >= 35)
                        {
                            client.Player.DefeatedArenaGuardians -= 35;
                            client.Inventory.AddItemWitchStack(720148, 0, 1, stream);
                            client.SendSysMesage("You`ve received 1 Refinery Badge!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");

                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 35)
                        {
                            client.Player.DefeatedArenaGuardians -= 35;
                            client.GainExpBall(300);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth half of an Exp Ball!", MsgMessage.ChatMode.System);

                        }
                        else
                        {
                            data.AddText("You need to defeat 35 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 35)
                            {
                                data.AddText("You`ve defeated 35 Arena Guardians! Which reward would you like, EXP or 1 Refinery Badge?");
                                data.AddOption("Half~of~an~EXP~Ball.", 12);
                                data.AddOption("1~Refinery~Badges.", 13);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 35 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.HaveSpace(1) && client.Player.DefeatedArenaGuardians >= 35)
                        {
                            client.Player.DefeatedArenaGuardians -= 35;
                            client.Inventory.AddItemWitchStack(720148, 0, 1, stream);
                            client.SendSysMesage("You`ve received 1 Refinery Badge!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");

                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(86, 58, 1772);
                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(28, 10, 1772);

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(44, 14, 1772);

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(17, 40, 1772);

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(61, 89, 1772);

                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(81, 70, 1772);

                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(17, 40, 1772);

                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(61, 89, 1772);

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 35)
                        {
                            client.Player.DefeatedArenaGuardians -= 35;
                            client.GainExpBall(300);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth half of an Exp Ball!", MsgMessage.ChatMode.System);

                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(86, 58, 1772);
                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(28, 10, 1772);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(44, 14, 1772);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(17, 40, 1772);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(61, 89, 1772);
                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(81, 70, 1772);
                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(17, 40, 1772);
                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(61, 89, 1772);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You need to defeat 35 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("After you enter a lower stage, there will be some requirements for you to meet before advancing a stage. Are you sure you want to enter a lower stage?");
                        data.AddOption("Enter~a~lower~stage.", 21);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        switch (Program.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    client.Teleport(90, 61, 1774);
                                    break;
                                }
                            case 1:
                                {
                                    client.Teleport(73, 79, 1774);
                                    break;
                                }
                            case 2:
                                {
                                    client.Teleport(62, 89, 1774);
                                    break;
                                }
                            case 3:
                                {
                                    client.Teleport(44, 16, 1774);
                                    break;
                                }
                            case 4:
                                {
                                    client.Teleport(20, 48, 1774);
                                    break;
                                }
                            case 5:
                                {
                                    client.Teleport(34, 8, 1774);
                                    break;
                                }
                            case 6:
                                {
                                    client.Teleport(24, 25, 1774);
                                    break;
                                }
                            case 7:
                                {
                                    client.Teleport(21, 37, 1774);
                                    break;
                                }
                        }
                        break;
                    }

                case 30:
                    {
                        data.AddText("If you quite the Power Arena now, you need to wait for 10 minutes to enter the arena again. Are you sure you want to quit now?");
                        data.AddOption("Yes,~I~am~sure.", 31);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.Teleport(300, 280, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Agate3)]
        public static void Agate3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {//5004000
                        data.AddText("You can claim experience from the Agate for every 60 guardians you have defeated. Also, you can use the Agate to proceed to an advanced stage, to withdraw to a lower stage or to return to Twin City.");
                        data.AddOption("Claim~Experience~and~proceed.", 10);
                        data.AddOption("Claim~Experience.", 1);
                        data.AddOption("Withdraw~to~Stage~4.", 20);
                        data.AddOption("Quit~Arena.", 30);
                        data.AddAvatar(31);
                        data.FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 60)
                            {
                                data.AddText("You`ve defeated 60 Arena Guardians! Which reward would you like, EXP or 2 Refinery Badges?");
                                data.AddOption("EXP~worth~1~EXP~Ball.", 2);
                                data.AddOption("2~Refinery~Badges.", 3);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 60 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(2) && client.Player.DefeatedArenaGuardians >= 60)
                        {
                            client.Player.DefeatedArenaGuardians -= 60;
                            client.Inventory.AddItemWitchStack(720148, 0, 2, stream);
                            client.SendSysMesage("You`ve received 2 Refinery Badges!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 60)
                        {
                            client.Player.DefeatedArenaGuardians -= 60;
                            client.GainExpBall(600);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth an Exp Ball!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("You need to defeat 60 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 60)
                            {
                                data.AddText("You`ve defeated 60 Arena Guardians! Which reward would you like, EXP or 2 Refinery Badges?");
                                data.AddOption("EXP~worth~1~EXP~Ball.", 12);
                                data.AddOption("2~Refinery~Badges.", 13);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 60 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.HaveSpace(2) && client.Player.DefeatedArenaGuardians >= 60)
                        {
                            client.Player.DefeatedArenaGuardians -= 60;
                            client.Inventory.AddItemWitchStack(720148, 0, 2, stream);
                            client.SendSysMesage("You`ve received 2 Refinery Badges!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");

                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(32, 7, 1771);

                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(34, 54, 1771);

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(53, 24, 1771);

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(64, 80, 1771);

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(23, 19, 1771);

                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(10, 34, 1771);

                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(81, 70, 1771);

                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(86, 58, 1771);

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 60)
                        {
                            client.Player.DefeatedArenaGuardians -= 60;
                            client.GainExpBall(600);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth an Exp Ball!", MsgMessage.ChatMode.System);

                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(32, 7, 1771);

                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(34, 54, 1771);

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(53, 24, 1771);

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(64, 80, 1771);

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(23, 19, 1771);

                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(10, 34, 1771);

                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(81, 70, 1771);

                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(86, 58, 1771);

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You need to defeat 60 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("After you enter a lower stage, there will be some requirements for you to meet before advancing a stage. Are you sure you want to enter a lower stage?");
                        data.AddOption("Enter~a~lower~stage.", 21);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        switch (Program.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    client.Teleport(39, 66, 1773);
                                    break;
                                }
                            case 1:
                                {
                                    client.Teleport(65, 37, 1773);
                                    break;
                                }
                            case 2:
                                {
                                    client.Teleport(61, 88, 1773);
                                    break;
                                }
                            case 3:
                                {
                                    client.Teleport(74, 78, 1773);
                                    break;
                                }
                            case 4:
                                {
                                    client.Teleport(89, 60, 1773);
                                    break;
                                }
                            case 5:
                                {
                                    client.Teleport(35, 6, 1773);
                                    break;
                                }
                            case 6:
                                {
                                    client.Teleport(9, 36, 1773);
                                    break;
                                }
                            case 7:
                                {
                                    client.Teleport(22, 20, 1773);
                                    break;
                                }
                        }
                        break;
                    }

                case 30:
                    {
                        data.AddText("If you quite the Power Arena now, you need to wait for 10 minutes to enter the arena again. Are you sure you want to quit now?");
                        data.AddOption("Yes,~I~am~sure.", 31);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.Teleport(300, 280, 1002);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Agate2)]
        public static void Agate2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {   //5003900
                        data.AddText("You can claim experience from the Agate for every 45 guardians you have defeated. Also, you can use the Agate to proceed to an advanced stage, to withdraw to a lower stage or to return to Twin City.");
                        data.AddOption("Claim~Experience~and~proceed.", 10);
                        data.AddOption("Claim~Experience.", 1);
                        data.AddOption("Withdraw~to~Stage~3.", 20);
                        data.AddOption("Quit~Arena.", 30);
                        data.AddAvatar(31);
                        data.FinalizeDialog();

                        break;
                    }
                case 30:
                    {
                        data.AddText("If you quite the Power Arena now, you need to wait for 10 minutes to enter the arena again. Are you sure you want to quit now?");
                        data.AddOption("Yes,~I~am~sure.", 31);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.Teleport(300, 280, 1002);
                        break;
                    }
                case 20:
                    {
                        data.AddText("After you enter a lower stage, there will be some requirements for you to meet before advancing a stage. Are you sure you want to enter a lower stage?");
                        data.AddOption("Enter~a~lower~stage.", 21);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        switch (Program.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    client.Teleport(86, 58, 1772);
                                    break;
                                }
                            case 1:
                                {
                                    client.Teleport(28, 10, 1772);
                                    break;
                                }
                            case 2:
                                {
                                    client.Teleport(44, 14, 1772);
                                    break;
                                }
                            case 3:
                                {
                                    client.Teleport(17, 40, 1772);
                                    break;
                                }
                            case 4:
                                {
                                    client.Teleport(61, 89, 1772);
                                    break;
                                }
                            case 5:
                                {
                                    client.Teleport(81, 70, 1772);
                                    break;
                                }
                            case 6:
                                {
                                    client.Teleport(17, 40, 1772);
                                    break;
                                }
                            case 7:
                                {
                                    client.Teleport(61, 89, 1772);
                                    break;
                                }
                        }
                        break;
                    }
                case 10:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 45)
                            {
                                data.AddText("You`ve defeated 45 Arena Guardians! Which reward would you like, EXP or 2 Refinery Badges?");
                                data.AddOption("EXP~worth~1~EXP~Ball.", 12);
                                data.AddOption("2~Refinery~Badges.", 13);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 45 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.HaveSpace(2) && client.Player.DefeatedArenaGuardians >= 45)
                        {
                            client.Player.DefeatedArenaGuardians -= 45;
                            client.Inventory.AddItemWitchStack(720148, 0, 2, stream);
                            client.SendSysMesage("You`ve received 2 Refinery Badges!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");

                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(9, 37, 1770);

                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(37, 8, 1770);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(21, 21, 1770);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(31, 58, 1770);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(55, 33, 1770);

                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(75, 76, 1770);
                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(80, 70, 1770);
                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(88, 59, 1770);

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 45)
                        {
                            client.Player.DefeatedArenaGuardians -= 45;
                            client.GainExpBall(600);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth an Exp Ball!", MsgMessage.ChatMode.System);

                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Teleport(9, 37, 1770);

                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(37, 8, 1770);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(21, 21, 1770);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(31, 58, 1770);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(55, 33, 1770);

                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(75, 76, 1770);
                                        break;
                                    }
                                case 6:
                                    {
                                        client.Teleport(80, 70, 1770);
                                        break;
                                    }
                                case 7:
                                    {
                                        client.Teleport(88, 59, 1770);

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You need to defeat 46 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 45)
                            {
                                data.AddText("You`ve defeated 45 Arena Guardians! Which reward would you like, EXP or 2 Refinery Badges?");
                                data.AddOption("EXP~worth~1~EXP~Ball.", 2);
                                data.AddOption("2~Refinery~Badges.", 3);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 45 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(2) && client.Player.DefeatedArenaGuardians >= 45)
                        {
                            client.Player.DefeatedArenaGuardians -= 45;
                            client.Inventory.AddItemWitchStack(720148, 0, 2, stream);
                            client.SendSysMesage("You`ve received 2 Refinery Badges!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 45)
                        {
                            client.Player.DefeatedArenaGuardians -= 45;
                            client.GainExpBall(600);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth an Exp Ball!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("You need to defeat 46 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Agate1)]
        public static void Agate1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        //5003800
                        data.AddText("You can claim experience from the Agate for every 30 guardians you have defeated. You can also use the Agate to enter a lower stage or to return to Twin City.");
                        data.AddOption("Claim~Experience.", 1);
                        data.AddOption("Withdraw~to~Stage~2.", 10);
                        data.AddOption("Quit~Arena.", 20);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("If you quite the Power Arena now, you need to wait for 10 minutes to enter the arena again. Are you sure you want to quit now?");
                        data.AddOption("Yes,~I~am~sure.", 21);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.Teleport(300, 280, 1002);
                        break;
                    }
                case 10:
                    {
                        data.AddText("After you enter a lower stage, there will be some requirements for you to meet before advancing a stage. Are you sure you want to enter a lower stage?");
                        data.AddOption("Yes,~enter~a~lower~stage.", 11);
                        data.AddOption("Let~me~think~about~it!");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        switch (Program.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    client.Teleport(32, 7, 1771);
                                    break;
                                }
                            case 1:
                                {
                                    client.Teleport(34, 54, 1771);

                                    break;
                                }
                            case 2:
                                {
                                    client.Teleport(53, 24, 1771);

                                    break;
                                }
                            case 3:
                                {
                                    client.Teleport(64, 80, 1771);
                                    break;
                                }
                            case 4:
                                {
                                    client.Teleport(23, 19, 1771);

                                    break;
                                }
                            case 5:
                                {
                                    client.Teleport(10, 34, 1771);
                                    break;
                                }
                            case 6:
                                {
                                    client.Teleport(81, 70, 1771);
                                    break;
                                }
                            case 7:
                                {
                                    client.Teleport(86, 58, 1771);
                                    break;
                                }
                        }
                        break;
                    }
                case 1:
                    {
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (client.Player.DefeatedArenaGuardians >= 30)
                            {
                                data.AddText("You`ve defeated 30 Arena Guardians! Which reward would you like, EXP or 2 Refinery Badges?");
                                data.AddOption("EXP~worth~1~EXP~Ball.", 2);
                                data.AddOption("2~Refinery~Badges.", 3);
                                data.AddOption("Let~me~think...", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to defeat 30 guardians first to claim experience from the Agate.");
                                data.AddOption("Okay.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Power Arena is closed, please come tomorrow!");
                            data.AddOption("Okay!", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(2) && client.Player.DefeatedArenaGuardians >= 30)
                        {
                            client.Player.DefeatedArenaGuardians -= 30;
                            client.Inventory.AddItemWitchStack(720148, 0, 2, stream);
                            client.SendSysMesage("You`ve received 2 Refinery Badges!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Please make 2 cells first before you claim the reward!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.DefeatedArenaGuardians >= 30)
                        {
                            client.Player.DefeatedArenaGuardians -= 30;
                            client.GainExpBall(600);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You have claimed experience worth an Exp Ball!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("You need to defeat 30 guardians first to claim experience from the Agate.");
                            data.AddOption("Okay.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}
