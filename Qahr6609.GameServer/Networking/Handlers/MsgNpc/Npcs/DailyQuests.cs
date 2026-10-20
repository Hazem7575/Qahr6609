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

        public static string GetStrTask(Client.GameClient user, Database.QuestInfo.DBQuest dbquest)
        {
            if (user.Player.Level < dbquest.Lv_min)
                return "Level~" + dbquest.Lv_min + "";
            if (user.Player.QuestGUI.CheckQuest(dbquest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                return "Underway";
            if (user.Player.QuestGUI.CheckQuest(dbquest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                return "Complete";
            if (user.Player.Level > dbquest.Lv_min)
                return "Acceptable";
            return "Level~" + dbquest.Lv_min;
        }
        [NpcAttribute(NpcID.MysterCheast1)]
        public static void MysterCheast1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6390);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.DailyHeavenChance == 0)
                        {
                            data.AddText("Are you ready to harvest fortune from the Mystery Chest? Waste no time, and go for it!")
                               .AddText("After opening the chest, you`ll receive 60 minutes of EXP, 10 Study Points, 1 Chi Token and 1 Heaven Key Scrap. If you`re a Jiang Hu fighter, you`ll also get 1 Talent point.");
                            data.AddOption("Open~the~chest.", 1).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance > 0)
                        {
                            data.AddText("Are you ready to embrace wealth coming from the Mystery Chests? Waste no time, and go for it!");
                            data.AddOption("Open~the~chest.", 2).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.DailyHeavenChance += 1;
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 10, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Inventory.AddItemWitchStack(3002307, 0, 1, stream);
                            client.CreateBoxDialog("You~received~60~minutes~of~EXP,~10~Study~Points,~1~Chi~Token,~1~Heaven~Key~Scrap,~and~1~Talent~point!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }

                        break;
                    }

                case 2:
                    {
                        if (client.Player.DailyHeavenChance == 1)
                        {
                            data.AddText("Well, it`s the 2nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP and 30 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 2)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 100 Study Points and 60 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 3)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 150 Study Points and 100 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 1 Super Protection Pill and 1 Free Course.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 4)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 200 Study Points and 150 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs to open the Mystery Chest?");
                        data.AddOption("Yes.", 4).AddOption("No.", 255).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3002308, 1))
                        {
                            if (!client.Inventory.Remove(3002308, 1, stream)) break;
                            if (client.Player.MyJiangHu != null)
                            {
                                client.Player.MyJiangHu.Talent = (byte)(Math.Min(client.Player.MyJiangHu.Talent + 1, 4));
                                client.Player.MyJiangHu.SendInfo(client, MsgJiangHuInfo.JiangMode.UpdateTalent, true, client.Player.UID.ToString(), client.Player.MyJiangHu.Talent.ToString());
                            }
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);

                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {

                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(256, 238, 1002);
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }
                        else
                        {
                            data.AddText("You don`t have a Heaven Key to open the Mystery Chest.");
                            data.AddOption("I~see.", 255).FinalizeDialog();

                        }
                        break;
                    }
                case 4:
                    {
                        uint cps = DailiHeavenCpsCost(client.Player.DailyHeavenChance);
                        if (client.Player.ConquerPoints >= cps)
                        {
                            client.Player.ConquerPoints -= cps;

                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(256, 238, 1002);
                            }

                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to open the Mystery Chest. Make sure you bring enough money, next time!");
                            data.AddOption("Alright.", 255).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MysterCheast2)]
        public static void MysterCheast2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6390);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.DailyHeavenChance == 0)
                        {
                            data.AddText("Are you ready to harvest fortune from the Mystery Chest? Waste no time, and go for it!")
                               .AddText("After opening the chest, you`ll receive 60 minutes of EXP, 10 Study Points, 1 Chi Token and 1 Heaven Key Scrap. If you`re a Jiang Hu fighter, you`ll also get 1 Talent point.");
                            data.AddOption("Open~the~chest.", 1).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance > 0)
                        {
                            data.AddText("Are you ready to embrace wealth coming from the Mystery Chests? Waste no time, and go for it!");
                            data.AddOption("Open~the~chest.", 2).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.DailyHeavenChance += 1;
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 10, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Inventory.AddItemWitchStack(3002307, 0, 1, stream);
                            client.CreateBoxDialog("You~received~60~minutes~of~EXP,~10~Study~Points,~1~Chi~Token,~1~Heaven~Key~Scrap,~and~1~Talent~point!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }

                        break;
                    }

                case 2:
                    {
                        if (client.Player.DailyHeavenChance == 1)
                        {
                            data.AddText("Well, it`s the 2nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP and 30 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 2)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 100 Study Points and 60 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 3)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 150 Study Points and 100 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 1 Super Protection Pill and 1 Free Course.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 4)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 200 Study Points and 150 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs to open the Mystery Chest?");
                        data.AddOption("Yes.", 4).AddOption("No.", 255).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3002308, 1))
                        {
                            if (!client.Inventory.Remove(3002308, 1, stream)) break;

                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(256, 238, 1002);
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }
                        else
                        {
                            data.AddText("You don`t have a Heaven Key to open the Mystery Chest.");
                            data.AddOption("I~see.", 255).FinalizeDialog();

                        }

                        break;
                    }
                case 4:
                    {
                        uint cps = DailiHeavenCpsCost(client.Player.DailyHeavenChance);
                        if (client.Player.ConquerPoints >= cps)
                        {
                            client.Player.ConquerPoints -= cps;


                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(256, 238, 1002);
                            }

                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to open the Mystery Chest. Make sure you bring enough money, next time!");
                            data.AddOption("Alright.", 255).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MysterCheast3)]
        public static void MysterCheast3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6390);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.DailyHeavenChance == 0)
                        {
                            data.AddText("Are you ready to harvest fortune from the Mystery Chest? Waste no time, and go for it!")
                               .AddText("After opening the chest, you`ll receive 60 minutes of EXP, 10 Study Points, 1 Chi Token and 1 Heaven Key Scrap. If you`re a Jiang Hu fighter, you`ll also get 1 Talent point.");
                            data.AddOption("Open~the~chest.", 1).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance > 0)
                        {
                            data.AddText("Are you ready to embrace wealth coming from the Mystery Chests? Waste no time, and go for it!");
                            data.AddOption("Open~the~chest.", 2).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.DailyHeavenChance += 1;
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 10, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Inventory.AddItemWitchStack(3002307, 0, 1, stream);
                            client.CreateBoxDialog("You~received~60~minutes~of~EXP,~10~Study~Points,~1~Chi~Token,~1~Heaven~Key~Scrap,~and~1~Talent~point!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }

                        break;
                    }

                case 2:
                    {
                        if (client.Player.DailyHeavenChance == 1)
                        {
                            data.AddText("Well, it`s the 2nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP and 30 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 2)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 100 Study Points and 60 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 3)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 150 Study Points and 100 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 1 Super Protection Pill and 1 Free Course.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 4)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 200 Study Points and 150 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs to open the Mystery Chest?");
                        data.AddOption("Yes.", 4).AddOption("No.", 255).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3002308, 1))
                        {
                            if (!client.Inventory.Remove(3002308, 1, stream)) break;

                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(256, 238, 1002);
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }
                        else
                        {
                            data.AddText("You don`t have a Heaven Key to open the Mystery Chest.");
                            data.AddOption("I~see.", 255).FinalizeDialog();

                        }

                        break;
                    }
                case 4:
                    {
                        uint cps = DailiHeavenCpsCost(client.Player.DailyHeavenChance);
                        if (client.Player.ConquerPoints >= cps)
                        {
                            client.Player.ConquerPoints -= cps;


                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(256, 238, 1002);
                            }

                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to open the Mystery Chest. Make sure you bring enough money, next time!");
                            data.AddOption("Alright.", 255).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MysterCheast4)]
        public static void MysterCheast4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6390);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.DailyHeavenChance == 0)
                        {
                            data.AddText("Are you ready to harvest fortune from the Mystery Chest? Waste no time, and go for it!")
                               .AddText("After opening the chest, you`ll receive 60 minutes of EXP, 10 Study Points, 1 Chi Token and 1 Heaven Key Scrap. If you`re a Jiang Hu fighter, you`ll also get 1 Talent point.");
                            data.AddOption("Open~the~chest.", 1).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance > 0)
                        {
                            data.AddText("Are you ready to embrace wealth coming from the Mystery Chests? Waste no time, and go for it!");
                            data.AddOption("Open~the~chest.", 2).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.DailyHeavenChance += 1;
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 10, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Inventory.AddItemWitchStack(3002307, 0, 1, stream);
                            client.CreateBoxDialog("You~received~60~minutes~of~EXP,~10~Study~Points,~1~Chi~Token,~1~Heaven~Key~Scrap,~and~1~Talent~point!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }

                        break;
                    }

                case 2:
                    {
                        if (client.Player.DailyHeavenChance == 1)
                        {
                            data.AddText("Well, it`s the 2nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP and 30 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 2)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 100 Study Points and 60 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 3)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 150 Study Points and 100 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 1 Super Protection Pill and 1 Free Course.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 4)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 200 Study Points and 150 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs to open the Mystery Chest?");
                        data.AddOption("Yes.", 4).AddOption("No.", 255).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3002308, 1))
                        {
                            if (!client.Inventory.Remove(3002308, 1, stream)) break;

                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(256, 238, 1002);
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }
                        else
                        {
                            data.AddText("You don`t have a Heaven Key to open the Mystery Chest.");
                            data.AddOption("I~see.", 255).FinalizeDialog();

                        }

                        break;
                    }
                case 4:
                    {
                        uint cps = DailiHeavenCpsCost(client.Player.DailyHeavenChance);
                        if (client.Player.ConquerPoints >= cps)
                        {
                            client.Player.ConquerPoints -= cps;


                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(256, 238, 1002);
                            }

                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to open the Mystery Chest. Make sure you bring enough money, next time!");
                            data.AddOption("Alright.", 255).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MysterCheast5)]
        public static void MysterCheast5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6390);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.DailyHeavenChance == 0)
                        {
                            data.AddText("Are you ready to harvest fortune from the Mystery Chest? Waste no time, and go for it!")
                               .AddText("After opening the chest, you`ll receive 60 minutes of EXP, 10 Study Points, 1 Chi Token and 1 Heaven Key Scrap. If you`re a Jiang Hu fighter, you`ll also get 1 Talent point.");
                            data.AddOption("Open~the~chest.", 1).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance > 0)
                        {
                            data.AddText("Are you ready to embrace wealth coming from the Mystery Chests? Waste no time, and go for it!");
                            data.AddOption("Open~the~chest.", 2).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.DailyHeavenChance += 1;
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 10, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Inventory.AddItemWitchStack(3002307, 0, 1, stream);
                            client.CreateBoxDialog("You~received~60~minutes~of~EXP,~10~Study~Points,~1~Chi~Token,~1~Heaven~Key~Scrap,~and~1~Talent~point!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }

                        break;
                    }

                case 2:
                    {
                        if (client.Player.DailyHeavenChance == 1)
                        {
                            data.AddText("Well, it`s the 2nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP and 30 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 2)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 100 Study Points and 60 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 3)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 150 Study Points and 100 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 1 Super Protection Pill and 1 Free Course.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 4)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 200 Study Points and 150 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs to open the Mystery Chest?");
                        data.AddOption("Yes.", 4).AddOption("No.", 255).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3002308, 1))
                        {
                            if (!client.Inventory.Remove(3002308, 1, stream)) break;

                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(256, 238, 1002);
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }
                        else
                        {
                            data.AddText("You don`t have a Heaven Key to open the Mystery Chest.");
                            data.AddOption("I~see.", 255).FinalizeDialog();

                        }

                        break;
                    }
                case 4:
                    {
                        uint cps = DailiHeavenCpsCost(client.Player.DailyHeavenChance);
                        if (client.Player.ConquerPoints >= cps)
                        {
                            client.Player.ConquerPoints -= cps;


                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(256, 238, 1002);
                            }

                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to open the Mystery Chest. Make sure you bring enough money, next time!");
                            data.AddOption("Alright.", 255).FinalizeDialog();

                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.FairyCloud)]
        public static void FairyCloud(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 1:
                case 0:
                    {
                        data.AddText("Wow, you`re so lucky to enter the Heaven Treasury! Let me introduce you the rules here.")
                           .AddText("~You can open one Mystery Chest in the Treasury, free of charge. Once opened, the chest will disappear. For other chests, you need to use CPs or the Heaven Keys to open them.")
                           .AddText("~Of course, the rewards will be better and better. Oh, if you`re a Jiang Hu fighter, you`ll also get a special reward.");
                        data.AddOption("Learn~about~opening~chests.", 2)
                            .AddOption("Send~me~to~Twin~City.", 6)
                             .AddOption("What`re~in~those~chests?", 4)
                              .AddOption("I~see.", 255)
                            .AddAvatar(233).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Teleport(256, 238, 1002);
                        break;
                    }
                case 2:
                    {
                        data.AddText("You can open one Mystery Chest for free, once in a day. For the other chests, you need to use CPs.")
               .AddText("~or the Heaven Keys to open. If you decide to pay CPs, the charge is, in order, 5 CPs, 20 CPs, 35 CPs and 100 CPs. While if you have a Heaven Key, ")
               .AddText("~you can open a chest worth 5 - 100 CPs. However, you can use up to 2 Heaven Keys in a day. Each 3 Heaven Key Scraps that you may obtain from the Mystery Chests")
               .AddText("~can be combined into a Heaven Key.");
                        data.AddOption("Learn~about~other~rules.", 1).AddOption("I~see.", 255).AddAvatar(233).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("1st Time: 60 minutes of EXP, 10 Study Points, 1 Chi Token and 1 Heaven Key Scrap;\n")
                            .AddText("2nd Time: 60 minutes of EXP, 30 Chi Points;\n")
                            .AddText("3rd Time: 60 minutes of EXP, 100 Study Points and 60 Chi Points;\n")
                            .AddText("4th Time: 60 minutes of EXP, 150 Study Points and 100 Chi Points;\n")
                            .AddText("5th Time: 60 minutes of EXP, 200 Study Points and 150 Chi Points.");
                        data.AddOption("What`s~for~Jiang~Hu~fighters.", 5)
                            .AddOption("Learn~about~other~rules.", 1)
                            .AddOption("I~see.", 255).AddAvatar(233).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("1st Time Extra Reward: 1 Talent point;\n")
                         .AddText("2nd Time Extra Reward: 1 Talent point and 1 Free Course;\n")
                         .AddText("3rd Time Extra Reward: 1 Talent point and 1 Free Course;\n")
                         .AddText("4th Time Extra Reward: 1 Talent point, 1 Super Protection Pill and 1 Free Course;\n")
                         .AddText("5th Time Extra Reward: 1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses;\nPlease note that, you won`t receive Talent points, if your Talent is already full.");
                        data.AddOption("Learn~about~other~rules.", 1)
                            .AddOption("I~see.", 255).AddAvatar(233).FinalizeDialog();

                        break;
                    }

            }
        }

        public static void CheckDesertGuardian(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DesertGuardian, client.Player.Class, 6049);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, client.OnRemoveNpc.X, client.OnRemoveNpc.Y) > 5)
                                client.CreateBoxDialog("You~are~too~far~away~from~the~Soldier`s~Remains!");
                            else
                                client.Player.AddPick(stream, "Sure", 4);
                        }

                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.MagnoliaEnvoy)]
        public static void MagnoliaEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6014);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && !client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`m here to deliver seeds, and I can also teleport you back to Twin City.");
                            data.AddOption("Claim~1~seed.", 1);

                        }
                        else
                            data.AddText("I don`t have any Magnolia Seeds. You`ll need to wait until tomorrow!");
                        data.AddOption("Back~to~Twin~City.", 9);
                        data.AddOption("How~to~plant~them.", 10);
                        data.AddOption("I`m~just~passing~by.", 255);

                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        client.Teleport(252, 235, 1002);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.DailyMagnoliaItemId == 0)
                        {
                            client.Player.DailyMagnoliaChance += 1;
                            client.Player.DailyMagnoliaItemId = 729306;//5
                            client.Player.DailyMagnoliaItemId += (byte)Program.GetRandom.Next(0, 4);
                        }
                        var DbItem = Database.Server.ItemsBase[client.Player.DailyMagnoliaItemId];

                        data.AddText("" + DbItem.Name + " \n");
                        data.AddText("You`ve already changed [" + client.Player.DailyMagnoliaChance.ToString() + "] times.\n");
                        data.AddOption("Claim~the~seed.", 2);
                        data.AddOption("Another~seed.~(50~CP)", 3);
                        data.AddOption("Another~seed.~(25~Study~Points)", 4);
                        data.AddOption("Buy~1~Super~seed.~(600~CPs)", 5);
                        data.AddOption("I`ll~think~about~it.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        var DbItem = Database.Server.ItemsBase[client.Player.DailyMagnoliaItemId];
                        client.Inventory.Add(stream, DbItem.ID);
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.CreateBoxDialog("You~received~1~" + DbItem.Name + "!~Go~plant~it~in~the~garden!");

                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 50)
                        {
                            client.Player.DailyMagnoliaItemId = 0;
                            client.Player.ConquerPoints -= 50;
                            goto case 1;
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(113).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.StudyPoints >= 25)
                        {
                            client.Player.DailyMagnoliaItemId = 0;
                            client.Player.SubClass.RemoveStudy(client, 25, stream);
                            goto case 1;
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough StudyPoints.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(113).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints >= 600)
                        {
                            client.Player.ConquerPoints -= 600;
                            client.Inventory.Add(stream, 729310);
                            client.Player.DailyMagnoliaItemId = 729310;
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            client.CreateBoxDialog("You~received~1~Super~Magnolia~Seed!~Go~plant~it~in~the~garden!");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(113).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("I`ll tell you a secret! Did you know that different seeds will bring you different rewards?")
                            .AddText("~Which kind of seed were you interested in?");
                        data.AddOption("Normal~Magnolia~Seed..", 11)
                            .AddOption("Refined~Magnolia~Seed.", 12)
                            .AddOption("Unique~Magnolia~Seed.", 13)
                            .AddOption("Elite~Magnolia~Seed.", 14)
                            .AddOption("Super~Magnolia~Seed.", 15)
                            .AddOption("Bye.", 255)
                        .AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        data.AddText("If your Normal Magnolia Seed becomes a flower, you`ll receive 60 minutes of EXP, 10 Study Points and 1 Chi Token.");
                        data.AddOption("Other~rewards.", 10).AddOption("Leave.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("If your Refined Magnolia Seed becomes a flower, you`ll have the chance to receive 90 minutes of EXP, 20 Study Points, 1 Chi Token, and more!");
                        data.AddOption("Other~rewards.", 10).AddOption("Leave.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        data.AddText("If your Unique Magnolia Seed becomes a flower, you`ll have the chance to receive 120 minutes of EXP, 50 Study Points, 1 Chi Token, and more!");
                        data.AddOption("Other~rewards.", 10).AddOption("Leave.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        data.AddText("If your Elite Magnolia Seed becomes a flower, you`ll have the chance to receive 180 minutes of EXP, 100 Study Points, 1 Chi Token, and more!");
                        data.AddOption("Other~rewards.", 10).AddOption("Leave.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        data.AddText("If your Super Magnolia Seed becomes a flower, you`ll have the chance to receive 300 minutes of EXP, 300 Study Points, 1 Chi Token, and more!");
                        data.AddOption("Other~rewards.", 10).AddOption("Leave.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DesertGuardian)]
        public static void DesertGuardian(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DesertGuardian, client.Player.Class, 6049);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Several days ago, a group of bandits appeared in the Desert, and we failed to defeat them. Many of our soldiers were killed.")
                                .AddText("~I need someone to release the trapped souls of these brave soldiers. Could you please help me? Well, if you`re too busy,")
                                .AddText("~you can give 10 CPs so that I can hire other heroes to help me. Of course, you`ll be handsomely rewarded!");
                            data.AddOption("Let~me~help~you.", 1);
                            data.AddAvatar(101).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You`ve helped us a lot, already! You`d better have a rest. If you want, you can come back tomorrow!");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(101).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 8))
                        {
                            data.AddText("You have released the souls of some dead soldiers, right? Just a few? Well, thanks for your help! I`ve got something to give you,")
                           .AddText("~for all that you`re doing for us. May those soldiers find peace in the afterlife...");
                            data.AddOption("Thanks!", 255)
                            .AddAvatar(101).FinalizeDialog();
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                                client.CreateBoxDialog("You`ve~completed~the~quest:~Release~the~Souls!~You~received~50~Study~Points,~120~minutes~of~EXP~and~1~Chi~Token!");

                            }
                            break;
                        }

                        data.AddText("Eh? Could it be there are still decent men in the world? It`s very kind of you to offer! If you can find 8 Soldier`s Remains and release their souls,")
                           .AddText("~I`ll reward you handsomely for all your effort. You have no idea what this will mean to the families. Take care, out there.");
                        data.AddOption("Okay!~I`ll~go~now!", 4)
                            .AddOption("Pay~700~CPs.", 3)
                            .AddAvatar(101).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 700)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            client.Player.ConquerPoints -= 700;
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                                client.CreateBoxDialog("You`ve~completed~the~quest:~Release~the~Souls!~You~received~50~Study~Points,~120~minutes~of~EXP~and~1~Chi~Token!");

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                              .AddOption("Okay.", 255).AddAvatar(84)
                .FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("You`ve~accepted~the~quest:~Release~the~Souls.~Find~at~least~8~Soldier`s~Remains~in~the~Desert!");
                        data.AddOption("Yes", 5)
                            .AddOption("No", 255)
                            .AddAvatar(101).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1000, 631, 500, 0);
                        break;
                    }
            }
        }
        public static uint GetNextSpitiBead(uint spiritbead)
        {
            if (spiritbead == 0)
                return 729611;
            if (spiritbead == 729611)
                return 729611 + 1;
            if (spiritbead == 729612)
                return spiritbead + 1;
            if (spiritbead == 729613)
                return spiritbead + 1;
            if (spiritbead == 729614)
                return 729703;
            return 729703;
        }
        public static uint GetDailySpiritBeadKills(Client.GameClient client)
        {

            switch (client.Player.DailySpiritBeadItem)
            {
                case 729611:
                    return 2500;
                case 729612:
                    return 2000;
                case 729613:
                    return 1500;
                case 729614:
                    return 1000;
                case 729703:
                    return 500;
            }
            return 0;

        }
        [NpcAttribute(NpcID.DailyNormalSpiritBead)]
        public static void DailyNormalSpiritBead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            Dialog data = new Dialog(client, stream);
            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                client.Player.QuestGUI.Accept(ActiveQuest, 0);
            bool payBeadCps = false;
            switch (Option)
            {
                case 0:
                    {
                        if (GetDailySpiritBeadKills(client) == 0)
                            return;
                        if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            data.AddText("You`ve collect enough spirits. Do you want to claim 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and a Chi Token, now?")
                              .AddOption("Yes,~of~course!", 3)
                              .AddOption("No,~not~now.", 255).FinalizeDialog();
                            break;
                        }
                        data.AddText("This bead needs to absorb " + GetDailySpiritBeadKills(client) + " spirits. Current spirits: " + client.Player.DailySpiritBeadCount + ".")
                               .AddOption("Fill~it~up.~(500CPs)", 1)
                               .AddOption("Okay.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("This is a Normal Spirit Bead. Once fully charged, you`ll receive 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and a Chi Token.")
                              .AddText("~Are you sure you want to pay 500 CPs to get the reward, directly?")
                             .AddOption("Yeah,~please!", 2)
                             .AddOption("Wait,~I~haven`t~decided!", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.DailySpiritBeadItem == 729611 && HasItems(client, client.Player.DailySpiritBeadItem, 1) && (payBeadCps || client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client)))
                        {
                            // reward requires a full bead or the paid fill; the quest is closed only after that check
                            if (payBeadCps && client.Player.ConquerPoints < 500) break;
                            if (!client.Inventory.Remove(client.Player.DailySpiritBeadItem, 1, stream)) break;
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            if (payBeadCps) client.Player.ConquerPoints -= 500;
                            client.Player.DailySpiritBeadCount = 0;
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);

                            client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 30, stream);
                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 7);
                            client.CreateBoxDialog("You received 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and 1 Chi Token.");

                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 500 && HasItems(client, client.Player.DailySpiritBeadItem, 1))
                        {
                            //  if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                payBeadCps = true; // CPs are deducted in case 3, after its checks
                                goto case 3;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DailyRefinedSpiritBead)]
        public static void DailyRefinedSpiritBead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                client.Player.QuestGUI.Accept(ActiveQuest, 0);
            Dialog data = new Dialog(client, stream);
            bool payBeadCps = false;
            switch (Option)
            {
                case 0:
                    {
                        if (GetDailySpiritBeadKills(client) == 0)
                            return;
                        if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            data.AddText("You`ve collect enough spirits. Do you want to claim 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token, now?")
                              .AddOption("Yes,~of~course!", 3)
                              .AddOption("No,~not~now.", 255).FinalizeDialog();
                            break;
                        }
                        data.AddText("This bead needs to absorb " + GetDailySpiritBeadKills(client) + " spirits. Current spirits: " + client.Player.DailySpiritBeadCount + ".")
                               .AddOption("Fill~it~up.~(500CPs)", 1)
                               .AddOption("Okay.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("This is a Normal Spirit Bead. Once fully charged, you`ll receive 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token")
                               .AddText("~Are you sure you want to pay 500 CPs to get the reward, directly?")
                             .AddOption("Yeah,~please!", 2)
                             .AddOption("Wait,~I~haven`t~decided!", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.DailySpiritBeadItem == 729612 && HasItems(client, client.Player.DailySpiritBeadItem, 1) && (payBeadCps || client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client)))
                        {
                            // reward requires a full bead or the paid fill; the quest is closed only after that check
                            if (payBeadCps && client.Player.ConquerPoints < 500) break;
                            if (!client.Inventory.Remove(client.Player.DailySpiritBeadItem, 1, stream)) break;
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            if (payBeadCps) client.Player.ConquerPoints -= 500;
                            client.Player.DailySpiritBeadCount = 0;
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);

                            client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 10);
                            client.CreateBoxDialog("You received 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token.");

                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 500 && HasItems(client, client.Player.DailySpiritBeadItem, 1))
                        {
                            //   if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                payBeadCps = true; // CPs are deducted in case 3, after its checks
                                goto case 3;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DailyUniqueSpiritBead)]
        public static void DailyUniqueSpiritBead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                client.Player.QuestGUI.Accept(ActiveQuest, 0);
            Dialog data = new Dialog(client, stream);
            bool payBeadCps = false;
            switch (Option)
            {
                case 0:
                    {
                        if (GetDailySpiritBeadKills(client) == 0)
                            return;
                        if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            data.AddText("You`ve collect enough spirits. Do you want to claim 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token, now?")
                              .AddOption("Yes,~of~course!", 3)
                              .AddOption("No,~not~now.", 255).FinalizeDialog();
                            break;
                        }
                        data.AddText("This bead needs to absorb " + GetDailySpiritBeadKills(client) + " spirits. Current spirits: " + client.Player.DailySpiritBeadCount + ".")
                               .AddOption("Fill~it~up.~(500CPs)", 1)
                               .AddOption("Okay.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("This is a Normal Spirit Bead. Once fully charged, you`ll receive 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token")
                             .AddText("~Are you sure you want to pay 500 CPs to get the reward, directly?")
                             .AddOption("Yeah,~please!", 2)
                             .AddOption("Wait,~I~haven`t~decided!", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.DailySpiritBeadItem == 729613 && HasItems(client, client.Player.DailySpiritBeadItem, 1) && (payBeadCps || client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client)))
                        {
                            // reward requires a full bead or the paid fill; the quest is closed only after that check
                            if (payBeadCps && client.Player.ConquerPoints < 500) break;
                            if (!client.Inventory.Remove(client.Player.DailySpiritBeadItem, 1, stream)) break;
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            if (payBeadCps) client.Player.ConquerPoints -= 500;
                            client.Player.DailySpiritBeadCount = 0;
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);

                            client.GainExpBall(2300, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 15);
                            client.CreateBoxDialog("You received 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token.");

                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 500 && HasItems(client, client.Player.DailySpiritBeadItem, 1))
                        {
                            //    if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                payBeadCps = true; // CPs are deducted in case 3, after its checks
                                goto case 3;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DailyEliteSpiritBead)]
        public static void DailyEliteSpiritBead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            Dialog data = new Dialog(client, stream);
            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                client.Player.QuestGUI.Accept(ActiveQuest, 0);
            bool payBeadCps = false;
            switch (Option)
            {
                case 0:
                    {
                        if (GetDailySpiritBeadKills(client) == 0)
                            return;
                        if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            data.AddText("You`ve collect enough spirits. Do you want to claim 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token, now?")
                              .AddOption("Yes,~of~course!", 3)
                              .AddOption("No,~not~now.", 255).FinalizeDialog();
                            break;
                        }
                        data.AddText("This bead needs to absorb " + GetDailySpiritBeadKills(client) + " spirits. Current spirits: " + client.Player.DailySpiritBeadCount + ".")
                               .AddOption("Fill~it~up.~(500CPs)", 1)
                               .AddOption("Okay.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("This is a Normal Spirit Bead. Once fully charged, you`ll receive 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token.")
                                .AddText("~Are you sure you want to pay 500 CPs to get the reward, directly?")
                             .AddOption("Yeah,~please!", 2)
                             .AddOption("Wait,~I~haven`t~decided!", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.DailySpiritBeadItem == 729614 && HasItems(client, client.Player.DailySpiritBeadItem, 1) && (payBeadCps || client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client)))
                        {
                            // reward requires a full bead or the paid fill; the quest is closed only after that check
                            if (payBeadCps && client.Player.ConquerPoints < 500) break;
                            if (!client.Inventory.Remove(client.Player.DailySpiritBeadItem, 1, stream)) break;
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            if (payBeadCps) client.Player.ConquerPoints -= 500;
                            client.Player.DailySpiritBeadCount = 0;
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);

                            client.GainExpBall(3000, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 24);
                            client.CreateBoxDialog("You received 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token.");

                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 500 && HasItems(client, client.Player.DailySpiritBeadItem, 1))
                        {
                            //if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                payBeadCps = true; // CPs are deducted in case 3, after its checks
                                goto case 3;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DailySuperSpiritBead)]
        public static void DailySuperSpiritBead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            Dialog data = new Dialog(client, stream);
            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                client.Player.QuestGUI.Accept(ActiveQuest, 0);
            bool payBeadCps = false;
            switch (Option)
            {
                case 0:
                    {
                        if (GetDailySpiritBeadKills(client) == 0)
                            return;
                        if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            data.AddText("You`ve collect enough spirits. Do you want to claim 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token, now?")
                              .AddOption("Yes,~of~course!", 3)
                              .AddOption("No,~not~now.", 255).FinalizeDialog();
                            break;
                        }
                        data.AddText("This bead needs to absorb " + GetDailySpiritBeadKills(client) + " spirits. Current spirits: " + client.Player.DailySpiritBeadCount + ".")
                               .AddOption("Fill~it~up.~(500CPs)", 1)
                               .AddOption("Okay.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("This is a Normal Spirit Bead. Once fully charged, you`ll receive 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token.")
                             .AddText("~Are you sure you want to pay 500 CPs to get the reward, directly?")
                            .AddOption("Yeah,~please!", 2)
                            .AddOption("Wait,~I~haven`t~decided!", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.DailySpiritBeadItem == 729703 && HasItems(client, client.Player.DailySpiritBeadItem, 1) && (payBeadCps || client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client)))
                        {
                            // reward requires a full bead or the paid fill; the quest is closed only after that check
                            if (payBeadCps && client.Player.ConquerPoints < 500) break;
                            if (!client.Inventory.Remove(client.Player.DailySpiritBeadItem, 1, stream)) break;
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            if (payBeadCps) client.Player.ConquerPoints -= 500;
                            client.Player.DailySpiritBeadCount = 0;
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);

                            client.GainExpBall(3600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 100, stream);
                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 48);
                            client.CreateBoxDialog("You received 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token.");

                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 500 && HasItems(client, client.Player.DailySpiritBeadItem, 1))
                        {
                            //   if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                payBeadCps = true; // CPs are deducted in case 3, after its checks
                                goto case 3;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HeavenlyMaster)]
        public static void HeavenlyMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, client.Player.Class, 6126);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Every day when you successfully challenged the Tower of Mystery, you can claim")
                                .AddText("~a Chi Token, 200 Champion Points, 3 Exorcism Tokens and 100 Study Points from me.");
                        data.AddOption("Take~me~to~the~tower.", 2);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Claim~my~rewards.", 30);
                        data.AddOption("Exchange~for~Exorcism~Pack.", 3)
                    .AddOption("Leave.", 255)
                    .AddAvatar(67)
                    .FinalizeDialog();

                        break;
                    }
                case 2:
                    {

                        client.Teleport(85, 75, 4020);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 30:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.");
                            data.AddOption("I~see.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {

                                client.SendSysMesage("Congratulations! You've received a Chi Token, 3 Exorcism Tokens, 100 Study Points and 200 Champion Points!");
                                client.Player.AddChampionPoints(200);
                                client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                client.Inventory.AddItemWitchStack(711211, 0, 3, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            }
                        }
                        else
                        {

                            data.AddText("You haven`t completed the challenge of the Tower of Mystery, and you can`t claim the rewards, otherwise you wold like to pay 1500 CPs or a Mystery Amulet.");
                            data.AddOption("Pay 1500 CPs for reward.", 11)
                                .AddOption("Swap MysteryAmulet for reward.", 10)
                                .AddOption("I haven`t decided, yet!", 255)
                                .AddAvatar(67).FinalizeDialog();
                            //You haven`t completed the challenge of the Tower of Mystery, and you can`t claim the rewards.
                        }

                        break;
                    }
                case 11:
                    {
                        if (client.Player.ConquerPoints >= 1500)
                        {
                            if (!client.Inventory.HaveSpace(2))
                            {
                                data.AddText("Please make 2 more spaces in your inventory.");
                                data.AddOption("I~see.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Player.ConquerPoints -= 1500;
                                client.SendSysMesage("Congratulations! You've received a Chi Token, 3 Exorcism Tokens, 100 Study Points and 200 Champion Points!");
                                client.Player.AddChampionPoints(200);
                                client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                client.Inventory.AddItemWitchStack(711211, 0, 3, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to directly get the rewards.");
                            data.AddOption("Alright.", 255)
                                .AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3600012, 1))
                        {
                            if (!client.Inventory.HaveSpace(2))
                            {
                                data.AddText("Please make 2 more spaces in your inventory.");
                                data.AddOption("I~see.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            if (HasItems(client, 3600012, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                if (!client.Inventory.Remove(3600012, 1, stream)) break;
                                client.SendSysMesage("Congratulations! You've received a Chi Token, 3 Exorcism Tokens, 100 Study Points and 200 Champion Points!");
                                client.Player.AddChampionPoints(200);
                                client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                client.Inventory.AddItemWitchStack(711211, 0, 3, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have Mystery amulet to directly get the rewards.");
                            data.AddOption("Alright.", 255)
                                .AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }

                case 3:
                    {
                        data.AddText("If you have 3 Exorcism Tokens, you can exchange for an Exorcism Pack. And if you have 9 tokens, you can get a")
                            .AddText("~Golden Exorcism Pack! Which one do you want to claim?");
                        data.AddOption("Exorcism~Pack.", 20)
                            .AddOption("Golden~Exorcism~Pack.", 21)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(711211, 3))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.");
                                data.AddOption("I~see.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Remove(711211, 3, stream)) break;
                            client.Inventory.Add(stream, 720948);
                            client.SendSysMesage("You've successfully exchanged 3 Exorcism Tokens for an Exorcism Pack!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have 3 Exorcism Tokens.");
                            data.AddOption("I~see.", 255)
                                .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Contain(711211, 9))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.");
                                data.AddOption("I~see.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Remove(711211, 9, stream)) break;
                            client.Inventory.Add(stream, 729594);
                            client.SendSysMesage("You've successfully exchanged 3 Exorcism Tokens for an Gold Exorcism Pack!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have 9 Exorcism Tokens.");
                            data.AddOption("I~see.", 255)
                                .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.EartMaster)]
        public static void EartMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, client.Player.Class, 6126);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome, my friend. Are you sure you want to release the Thrilling Spook now, or would you rather return to the Market");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false
                            || !client.Map.ContainMobID(20160, client.Player.DynamicID))
                            data.AddOption("Release~a~Thrilling~Spook.", 1);
                        data.AddOption("Send~me~to~the~Market.", 2)
                        .AddOption("Nothing.", 255)
                        .AddAvatar(67)
                        .FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Teleport(251, 153, 1036);
                        break;
                    }
                case 1:
                    {
                        data.AddText("I just want to be sure. Shall I release the Thrilling Spook, now?");
                        data.AddOption("Okay.", 3)
                            .AddOption("Not~now.", 255)
                            .AddAvatar(67)
                            .FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level < 70)
                        {
                            data.AddText("Sorry,~you~cannot~release the Thrilling~Spook ~before~your~Level~is~70.~Please~train~harder.")
                                .AddOption("Oops.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false
                             || !client.Map.ContainMobID(20160, client.Player.DynamicID))
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            client.CreateBoxDialog("You~killed~a~Thrilling~Spook!~Speak~with~the~Heavenly~Master~to~claim~a~reward.");
                            Database.Server.AddMapMonster(stream, client.Map, 20160, client.Player.X, client.Player.Y, 10, 10, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DailyQuestEnvoy)]
        public static void DailyQuestEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6329);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6245);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6390);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DesertGuardian, client.Player.Class, 6049);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6366);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6014);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, client.Player.Class, 6126);


            switch (Option)
            {
                case 0:
                    {
                        data.AddText("All heroes can talk to me to learn about the daily tasks, and go to the required places to accept the work.");
                        data.AddOption("Head~to~the~Dragon~Island.", 170);
                        //data.AddOption("Cross-server~missions~(Lv.110)", 120);
                        data.AddOption("Bright~Fortune.~(" + (string)((client.Player.ClaimTowerAmulets == 0) ? "Lv.110" : "completed") + ")", 121);
                        data.AddOption("Worship~Leaders~(" + GetQuestModes(client, ActiveQuest.MissionId, 50) + ").", 1);
                        data.AddOption("Everything~Has~A~Price~(" + GetQuestModes(client, ActiveQuest2.MissionId, 60) + ").", 3);
                        data.AddOption("Release~the~Souls~(" + GetQuestModes(client, ActiveQuest4.MissionId, 70) + ")", 20);
                        data.AddOption("Rare~Materials.~(" + GetQuestModes(client, ActiveQuest5.MissionId, 75) + ")", 30);
                        data.AddOption("Magnolia Envoy~(" + GetQuestModes(client, ActiveQuest6.MissionId, 80) + ")", 40);
                        data.AddOption("Tower~of~Mystery.~[(" + GetQuestModes(client, ActiveQuest8.MissionId, 90) + ")", 60);
                        data.AddOption("Spirit~Beads~(" + GetQuestModes(client, ActiveQuest7.MissionId, 100) + ")", 50);
                        data.AddOption("Heaven~Treasury.~(" + GetQuestModes(client, ActiveQuest3.MissionId, 110) + ")", 10);
                        data.AddOption("Leave.", 255)
     .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 121:
                    {
                        if (client.Player.ClaimTowerAmulets == 0)
                        {
                            client.Teleport(88, 362, 3998);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        }
                        else
                        {

                            data.AddText("You`ve already made a big harvest in the Fiery Dragon region today. If you want to hunt for more treasures, please come to me tomorrow.");
                            data.AddOption("Learn~more~about~daily~quests.", 122)
                                .AddOption("Enter~the~Fiery~Dragon~region.", 123).AddAvatar(84).FinalizeDialog();
                        }
                        break;
                    }
                case 122:
                    {
                        goto case 0;
                    }
                case 123:
                    {
                        client.Teleport(88, 362, 3998);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 120:
                    {
                        client.SendSysMesage("Every day if you complete the cross-server missions first, you'll win many special tokens which enable you can directly complete corresponding daily quests. Don`t miss the change!", MsgMessage.ChatMode.System);
                        client.Player.QuestGUI.SendAutoPatcher(1002, 281, 291, 17400);
                        break;
                    }
                case 170:
                    {
                        client.SendSysMesage("Every day if you qualified to enter Chasm Of Chaos, you'll claim several prizes.", MsgMessage.ChatMode.System);
                        client.SendSysMesage("Heading to see IronTong at (344, 230).", MsgMessage.ChatMode.System);
                        client.Player.QuestGUI.SendAutoPatcher(1002, 344, 230, 20499);
                        break;
                    }
                case 60:
                    {
                        if (client.Player.Level < 90)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 90 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }

                        data.AddText("In the Wind Plain, there is the Tower of Mystery sealing the most cruel devils. For heroes who defeat those devils to energize the Mystery Pearl,")
       .AddText("~the Bright Tribe promises to give them wonderful rewards which include <tip color=0xfff05050 desc=collect#9#fragments#to#get#270#CPs#(B)>Blazing CP Fragments, </tip>??")
       .AddText("<tip color=0xfff05050 desc=randomly#give#Luxury#Protection#Pack,#Golden#Training#Pack,#Plentiful#Talent#Pack,#etc.>Concerted Effort Pack,</tip>")
       .AddText("~Chi Points and Study Points. If you`re willing to help, please talk to the Heavenly Master.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Head~to~see~Heavenly~Master.", 61);
                        data.AddOption("Leave.", 255)
                        .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 61:
                    {
                        client.Teleport(250, 150, 1036);
                        break;
                    }
                case 50:
                    {
                        //return;

                        if (client.Player.Level < 100)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 100 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                            {
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                switch (client.Player.DailySpiritBeadItem)
                                {
                                    case 729611:
                                        {
                                            client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 30, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 7);
                                            client.CreateBoxDialog("You received 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729612:
                                        {
                                            client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 10);
                                            client.CreateBoxDialog("You received 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729613:
                                        {
                                            client.GainExpBall(2400, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 15);
                                            client.CreateBoxDialog("You received 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729614:
                                        {
                                            client.GainExpBall(3000, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 24);
                                            client.CreateBoxDialog("You received 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729703:
                                        {
                                            client.GainExpBall(3600, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 48);
                                            client.CreateBoxDialog("You received 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                }
                                client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                                break;
                            }

                        }
                        if (client.Player.DailySpiritBeadItem == 0)
                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(0);
                        var DbItem = Database.Server.ItemsBase[client.Player.DailySpiritBeadItem];
                        var DbItem2 = Database.Server.ItemsBase[GetNextSpitiBead(client.Player.DailySpiritBeadItem)];

                        data.AddText("You can claim a " + DbItem.Name + ", today. If you complete the task, I`ll give you a " + DbItem2.Name + ", tomorrow.\nBy the way, you can do this job with other monster elimination tasks, at the same time.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddOption("Claim~a~bead.", 51);
                            data.AddOption("Get~a~" + DbItem2.Name + ".~(100CP)", 52);
                            data.AddOption("Complete~the~task.~(500CPs)", 53);
                        }
                        data.AddOption("What~are~the~beads?", 54);
                        data.AddOption("Leave.", 255)
                          .AddAvatar(84).FinalizeDialog();


                        break;
                    }
                case 54:
                    {
                        //return;

                        data.AddText("The Spirit Beads have 5 different qualities: Normal, Refined, Unique, Elite and Super. When you kill monsters, the bead will absorb the ")
                            .AddText("~monster spirits. When it`s fully charged, you can use the bead to claim a reward. If you complete the task, you`ll be able to claim")
                            .AddText("~a better bead, next time. As for the Super Spirit Bead, the next one will go back to a Normal bead.");
                        data.AddOption("How~many~spirits~are~needed?", 55)
                            .AddOption("What`re~the~rewards?", 56)
                            .AddOption("How~to~calculate~the~spirits?", 57)
                            .AddOption("I~want~to~know~something~else.", 50)
                            .AddOption("I~see.", 255)
                            .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 57:
                    {
                        data.AddText("Killing a monster below Level 70 will add 1 spirit. Level 70 - 99, 2 spirits. Level 100 - 119, 3 spirits. Level 120 - 140, 4 spirits.")
                              .AddText("~A monster leader will provide 10 spirits. As for the Terato Dragon, Snow Banshee, Snow Banshee Soul, Sword Master and Thrilling Spook 3,")
                              .AddText("~you`ll receive 1,000 spirits, if you`re able to kill anyone of them!")
                            .AddOption("I~want~to~know~something~else.", 54)
                            .AddOption("I~see.", 255)
                            .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 56:
                    {
                        data.AddText("Normal Spirit Bead: 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and 1 Chi Token.\n")
                              .AddText("Refined Spirit Bead: 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token.\n")
                              .AddText("Unique Spirit Bead: 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token.\n")
                              .AddText("Elite Spirit Bead: 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token.\n")
                              .AddText("Super bead: 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token.\n")
                            .AddOption("I~want~to~know~something~else.", 54)
                            .AddOption("I~see.", 255)
                            .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 55:
                    {
                        data.AddText("A Normal Spirit Bead can absorb 2,500 monster spirits. A Refined bead needs 2,000 spirits to fill it up,")
                              .AddText("~1,500 spirits for the Unique bead, 1,000 spirits for the Elite bead, and 500 spirits for a Super bead.")
                              .AddText("~a better bead, next time. As for the Super Spirit Bead, the next one will go back to a Normal bead.")
                            .AddOption("I~want~to~know~something~else.", 54)
                            .AddOption("I~see.", 255)
                            .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 51:
                    {
                        //return;
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        if (client.Player.DailySpiritBeadItem != 0)
                        {
                            if (client.Inventory.Contain(client.Player.DailySpiritBeadItem, 1))
                            {
                                data.AddText("You~already~have~a~Spirit~Bead~in~your~inventory.");
                                data.AddOption("Ok", 255).AddAvatar(84).FinalizeDialog();
                                break;
                            }
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        var DbItem = Database.Server.ItemsBase[client.Player.DailySpiritBeadItem];
                        client.Player.DailySpiritBeadItem = DbItem.ID;
                        client.Inventory.Add(stream, DbItem.ID);
                        client.CreateBoxDialog("You received a " + DbItem.Name + "!");

                        break;
                    }
                case 53:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }

                        if (client.Player.ConquerPoints >= 500)
                        {
                            client.Player.ConquerPoints -= 500;
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                            {
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                switch (client.Player.DailySpiritBeadItem)
                                {
                                    case 729611:
                                        {
                                            client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 30, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 7);
                                            client.CreateBoxDialog("You received 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729612:
                                        {
                                            client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 10);
                                            client.CreateBoxDialog("You received 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729613:
                                        {
                                            client.GainExpBall(2400, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 15);
                                            client.CreateBoxDialog("You received 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729614:
                                        {
                                            client.GainExpBall(3000, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 24);
                                            client.CreateBoxDialog("You received 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729703:
                                        {
                                            client.GainExpBall(3600, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 48);
                                            client.CreateBoxDialog("You received 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                }
                            }
                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }
                        else
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Sorry, but you don`t have enough CPs.");
                                data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();

                                break;
                            }
                        }
                        break;
                    }
                case 52:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        if (client.Player.DailySpiritBeadItem != 0)
                        {
                            if (client.Inventory.Contain(client.Player.DailySpiritBeadItem, 1))
                            {
                                data.AddText("You~already~have~a~Spirit~Bead~in~your~inventory.");
                                data.AddOption("Ok", 255).AddAvatar(84).FinalizeDialog();
                                break;
                            }
                        }
                        if (client.Player.ConquerPoints >= 100)
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                            client.Player.ConquerPoints -= 100;
                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                            var DbItem = Database.Server.ItemsBase[client.Player.DailySpiritBeadItem];
                            client.Player.DailySpiritBeadItem = DbItem.ID;
                            client.Inventory.Add(stream, DbItem.ID);
                            client.CreateBoxDialog("You received a " + DbItem.Name + "!");
                        }
                        else
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Sorry, but you don`t have enough CPs.");
                                data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                                break;
                            }
                        }


                        break;
                    }
                case 40:
                    {
                        if (client.Player.Level < 80)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 80 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        data.AddText("If you help the Magnolia Envoy plant a beautiful magnolia in the Garden, you`ll receive some EXP, Study Points, 1 Chi Token, and")
                            .AddText("~even some precious items like a Dragon Ball and Super Gems!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Visit~the~Magnolia~Envoy.", 41);
                        data.AddOption("Leave.", 255)
                          .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        client.Teleport(106, 108, 1806);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.Level < 75)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 75 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        data.AddText("Hey, I`m looking for some able men to collect items for me! You know, I always pay a high price for what I want.");
                        data.AddOption("Count~me~in!", 31).AddOption("Sorry,~I`m~busy~now.", 255).AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve finished today! Please come again tomorrow!");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }

                        data.AddText("     \t\t\t\t\t\t\t\t\tRare Materials\n")
                            .AddText("\t\t\t\t-----------------------\n")
                            .AddText("\t\t\t\tQuest Quality: [" + client.Player.DailyRareChance + 1 + "-Star]\n")
                            .AddText("\t\t\t\tItem: [QuestChanceA]; Amount: 2\n")
                            .AddText("\t\t\t\tChanged Times:  [ " + client.Player.DailyRareChance + " ]\n");
                        if (client.Player.DailyRareChance == 1)
                            data.AddText("\t\t\t\tRewards: 30 Study Points, 120 minutes of EXP and 1 Chi Token.\n");
                        else if (client.Player.DailyRareChance == 2)
                            data.AddText("50 Study Points, 180 minutes of EXP and 1 Chi Token.\n");
                        else if (client.Player.DailyRareChance == 3)
                            data.AddText("70 Study Points, 240 minutes of EXP, 1 Chi Token and 1 Protection Pill.");
                        data.AddText("\t\t\t\t-----------------------");
                        data.AddOption("Hand~in~the~item.", 32)
                            .AddOption("Change~the~required~item.", 33)
                            .AddOption("Directly~complete~it.", 34)
                            .AddOption("Sorry,~I`m~busy~now.", 255).AddAvatar(84).FinalizeDialog();


                        break;
                    }
                case 34:
                    {
                        data.AddText("If you would like to pay 50 CPs, you can directly claim the rewards without collecting the items for me.");
                        data.AddOption("Pay~50~CPs~to~complete~it.", 35)
                            .AddOption("I`ll~think~about~it.", 255).AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 35:
                    {
                        if (client.Player.ConquerPoints >= 50)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(84).FinalizeDialog();
                                break;
                            }
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.Accept(ActiveQuest5, 0);

                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                            {
                                client.Player.ConquerPoints -= 50;
                                client.CreateBoxDialog("You received 70 Study Points, 240 minutes of EXP, 1 Chi Token and 1 Protection Pill.");
                                client.GainExpBall(2400, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.SubClass.AddStudyPoints(client, 70, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("Okay.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
                case 33:
                    {
                        if (client.Player.DailyRareChance == 3)
                        {
                            data.AddText("You`ve finished today! Please come again tomorrow!");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.RacePoints >= 200)
                        {
                            //client.Player.RacePoints -= 200;
                            //client.Player.SendUpdate(stream, client.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);
                            //client.Player.DailyRareChance += 1;
                            //data.AddText("You are good. Now you have the item.");
                            //data.AddOption("Thanks", 255).AddAvatar(84).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I won`t change my mind if you don`t have 200 Horse Racing Points!");
                            data.AddOption("Alright.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
                case 32:
                    {
                        if (client.Player.DailyRareChance > 0)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(84).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                            {
                                if (client.Player.DailyRareChance == 1)
                                {
                                    client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                                    client.CreateBoxDialog("You received 30 Study Points, 120 minutes of EXP and 1 Chi Token.");
                                    client.Player.SubClass.AddStudyPoints(client, 30, stream);
                                    client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                }
                                else if (client.Player.DailyRareChance == 2)
                                {
                                    client.CreateBoxDialog("You received 50 Study Points, 180 minutes of EXP and 1 Chi Token.");
                                    client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);
                                    client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                    client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                }
                                else if (client.Player.DailyRareChance == 3)
                                {
                                    client.CreateBoxDialog("You received 70 Study Points, 240 minutes of EXP, 1 Chi Token and 1 Protection Pill.");
                                    client.GainExpBall(2400, true, Role.Flags.ExperienceEffect.angelwing);
                                    client.Player.SubClass.AddStudyPoints(client, 70, stream);
                                    client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Are you kidding me? You don`t have the items I want!");
                            data.AddOption("I`ll~bring~you~some!", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
                case 20:
                    {
                        if (client.Player.Level < 70)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 70 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        data.AddText("Help the Desert Guardian release the souls of 8 soldiers in the Desert. Once completed, you`ll receive")
                        .AddText("~120 minutes of EXP, 50 Study Points and 1 Chi Token.");
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Go~to~the~Desert.", 21);
                        data.AddOption("Leave.", 255).AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Teleport(539, 646, 1000);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < 110 && client.Player.Reborn == 0)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 110 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }

                        data.AddText("Trust me, I can never over praise the Heaven Treasury. It`s the top place that keeps fabulous treasures.")
                            .AddText("~If you`ve reached Level 110 or been reborn, I`ll give you a chance to enter that amazing land.");
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Take~me~to~Heaven~Treasury!", 11);
                        data.AddOption("I`m~just~passing~by.", 255).AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("You~entered~the~Heaven~Treasury!~Go~pick~a~Mystery~Chest!");
                        client.Teleport(61, 53, 9972);


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Level < 50)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 50 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        data.AddText("If you worship the leaders of the Supreme Guilds in the Ancestral Hall, you`ll receive some rewards and a Chi Token. If the Guild Leaders are touched by your sincerity,");
                        data.AddText("~you`ll receive 200 Study Points and 20 CPs!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Cool!~Send~me~there!", 2)
                                .AddOption("Awesome!", 255);
                        else
                            data.AddOption("Check~other~quests.", 255);
                        data.AddAvatar(84).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        client.Teleport(53, 56, 3081, 0);
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level < 60)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 60 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("When you`re killing monsters, you`ll have the chance to get the items I need. If you hand in the items I need according to")
                                .AddText("~the list, you`ll be handsomely rewarded.");
                            data.AddOption("What`s~the~reward?", 7).AddOption("Leave.", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        data.AddText("\t\t\t\t\t\t\t\t\t\tItem List\n")
                            .AddText("\t\t\t??????????????????????????????????????????????\n")
                            .AddText("\t\t\tItem: [BirdClaw]\n")
                            .AddText("\t\t\tAmount: [25]\n")
                            .AddText("\t\t\tMonster: [Birdman][BirdmanL88]\n")
                            .AddText("\t\t\tLocation: [Island]\n")
                            .AddText("\t\t\tChanged times: [1]\n")
                            .AddText("\t\t\t??????????????????????????????????????????????\n")
                         .AddOption("Hand~in~items.", 4)
                         .AddOption("Pay~50~CPs~to~complete~it.", 5)
                         .AddOption("Directly~complete~the~quest.", 110)
                         .AddOption("Claim~a~list.", 6)
                         .AddOption("What`s~the~reward?", 7)
                         .AddAvatar(84)
                        .FinalizeDialog();

                        break;
                    }
                case 110:
                    {
                        data.AddText("If you think it`s too troublesome to collect these materials, you can give me 50 CPs")
                            .AddText("~or a Collection Order to directly complete the quest and claim the reward.")
  .AddOption("Use~a~[Collection~Order].", 111)
  .AddOption("Directly~complete.~(50~CPs).", 112)
  .AddOption("I`ll~think~about~it.", 255)
  .AddAvatar(226).FinalizeDialog();
                        break;
                    }
                case 112:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            break;
                        if (client.Player.ConquerPoints >= 50)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(226).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {

                                client.Player.ConquerPoints -= 50;
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.bless);
                                client.Inventory.Add(stream, 729304);
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                client.CreateBoxDialog("You~received~600~minutes~of~EXP,~50~Study~Points~and~1~Chi~Token!");
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(226).FinalizeDialog();
                        }
                        break;
                    }
                case 111:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            break;
                        if (client.Inventory.Contain(3008731, 1))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(226).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            if (HasItems(client, 3008731, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (!client.Inventory.Remove(3008731, 1, stream)) break;
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.bless);
                                client.Inventory.Add(stream, 729304);
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                client.CreateBoxDialog("You~received~600~minutes~of~EXP,~50~Study~Points~and~1~Chi~Token!");
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have [Collection~Order] Token.");
                            data.AddOption("I~see.", 255).AddAvatar(226).FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(729094, 25))
                        {
                            if (HasItems(client, 729094, 25) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (!client.Inventory.Remove(729094, 25, stream)) break;

                                data.AddText("Well done! Here is your reward.");
                                data.AddOption("Thanks.", 255).AddAvatar(84).FinalizeDialog();

                                uint MinutesExp = 0;
                                if (client.Player.Level >= 60 && client.Player.Level <= 80)
                                    MinutesExp = 180;
                                else if (client.Player.Level >= 81 && client.Player.Level <= 100)
                                    MinutesExp = 240;
                                if (client.Player.Level >= 101 && client.Player.Level <= 110)
                                    MinutesExp = 300;
                                else if (client.Player.Level >= 111 && client.Player.Level <= 120)
                                    MinutesExp = 360;
                                else if (client.Player.Level >= 121 && client.Player.Level <= 130)
                                    MinutesExp = 480;
                                else if (client.Player.Level >= 131)
                                    MinutesExp = 600;

                                if (!client.Inventory.Remove(729087, 1, stream)) break;
                                client.GainExpBall(MinutesExp / 60 * 600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.CreateBoxDialog("You~received~" + MinutesExp + "~minutes~of~EXP,~50~Study~Points~and~1~Chi~Token!");

                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have 25 BirdClaw. Hurry up!")
    .AddOption("Okay.", 255)
    .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
                        data.AddText("Level 60 - 80: 180 minutes of EXP, 50 Study Points and a Chi Token\n")
                            .AddText("Level 81 - 100: 240 minutes of EXP, 50 Study Points and a Chi Token\n")
                            .AddText("Level 101 - 110: 300 minutes of EXP, 50 Study Points and a Chi Token\n")
                            .AddText("Level 111 - 120: 360 minutes of EXP, 50 Study Points and a Chi Token\n")
                            .AddText("Level 121 - 130: 480 minutes of EXP, 50 Study Points and a Chi Token\n")
                            .AddText("Level 131+: 600 minutes of EXP, 50 Study Points and a Chi Token\n")
                              .AddOption("Leave.", 255).AddAvatar(84)
                .FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(729087, 1) == false)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 729087);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        }
                        else
                        {
                            data.AddText("Sorry, but you already had a list in your inventory.")
    .AddOption("I~forgot.", 255)
    .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        if (client.Player.ConquerPoints >= 50)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                uint MinutesExp = 0;
                                if (client.Player.Level >= 60 && client.Player.Level <= 80)
                                    MinutesExp = 180;
                                else if (client.Player.Level >= 81 && client.Player.Level <= 100)
                                    MinutesExp = 240;
                                if (client.Player.Level >= 101 && client.Player.Level <= 110)
                                    MinutesExp = 300;
                                else if (client.Player.Level >= 111 && client.Player.Level <= 120)
                                    MinutesExp = 360;
                                else if (client.Player.Level >= 121 && client.Player.Level <= 130)
                                    MinutesExp = 480;
                                else if (client.Player.Level >= 131)
                                    MinutesExp = 600;

                                if (!client.Inventory.Remove(729087, 1, stream)) break;
                                client.GainExpBall(MinutesExp / 60 * 600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.CreateBoxDialog("You~received~" + MinutesExp + "~minutes~of~EXP,~50~Study~Points~and~1~Chi~Token!");
                                client.Player.ConquerPoints -= 50;
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                                .AddOption("Okay.", 255).AddAvatar(84)
                  .FinalizeDialog();
                        }

                        break;
                    }
            }
        }
    }
}
