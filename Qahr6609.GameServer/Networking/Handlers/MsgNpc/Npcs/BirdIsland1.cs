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
        [NpcAttribute(NpcID.GreenSnake)]
        public static void GreenSnake(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("What beautiful scenery! I feel so lucky to meet you here, so my problem of starvation can be solved. Come here, baby!")
.AddOption("Be~off,~or~I`ll~kill~you!", 1)
.AddOption("I`m~gone,~bye!", 255)
.AddAvatar(110).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Wait, I`m only joking! If you spare my life, I`ll lead you to a marvellous place with treasures everywhere! You can take what you want if you are able to break through the Snake Array.")
.AddOption("What~is~snake~array?", 2)
.AddAvatar(110).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Don`t kill me, and I`ll explain. It`s a kind of battle array formed by 17 islands. Our boss watches the last one. Beat the boss down, and the treasure will be yours!")
.AddOption("Show~me~the~way.", 3)
.AddAvatar(110).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Um, I don`t mean to question your ability. However, you may pay for your greed with your life. I`ve seen that a lot, before.")
.AddOption("I~fear~nothing.", 4)
.AddOption("I~changed~my~mind.", 255)
.AddAvatar(110).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.Teleport(444, 355, 1063);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Mulan)]
        public static void Mulan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harriet, client.Player.Class, 1644);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Mulan, client.Player.Class, 1645);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Mulan, client.Player.Class, 1646);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1648);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Mulan, client.Player.Class, 1649);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Do you believe I can also do what any man can do? I`ll join the army and to be a general, someday. Then I`ll realize my father`s wish of serving the country. Yeah, that`s")
                            .AddText("~also my dream!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Are~you~Mulan?", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("How~are~things~going,~now?", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Tell~me~all~you~know.", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("General~Armand~sent~me~here.", 30);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Is~anything~wrong?", 40);

                        data.AddOption("You~did~have~a~great~mind!", 255);
                        data.AddAvatar(1).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Did you see my father? No? Come on, I should keep my mind on combat. Please go to the Fishing Village and tell Harriet `I`m fine`, okay? Just that.")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(1).FinalizeDialog();

                            break;
                        }
                        data.AddText("No, I`m fine. I`m glad to hear that, and I really want to join the army, but I know I`m not strong enough. So, I decided to stay here and keep practicing.")
                            .AddText("~The only thing I worry about is my father. It`s just too early to return home. Could you do me a favor and go tell my father about my situation?")
                            .AddOption("No~problem.", 41)
                            .AddOption("I~have~more~important~things.", 255)
                            .AddAvatar(1).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        client.CreateBoxDialog("You~need~to~go~tell~Harriet~about~Mulan`s~situation~and~comfort~him.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(1).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Add(stream, 150197, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~20~minutes~of~EXP,~2,000~Silver~and~a~Diamond~Ring(B)!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            data.AddText("Really? General Armand asked me to return to the army? That`s wonderful! Wait... Um... I think I need more time to consider it.")
.AddOption("Alright.", 40)
.AddAvatar(1).FinalizeDialog();
                        }

                        break;
                    }

                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Why are you still here? You haven`t informed General Armand of the Banditti`s plan, have you? It`s an emergency! Please be quick!")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(1).FinalizeDialog();

                            break;
                        }
                        data.AddText("Hush! When I was looking for the Royal Sword, I found the Banditti plotted to ally with the Bandits L98 and attack the army from the back. If their plan succeeds, you")
                                        .AddText("~know what it means to the whole island. It will be a disaster! Could you go inform General Armand? Hurry up!")
        .AddOption("I`ll~do~it,~right~now.", 21)
        .AddOption("Why~not~tell~him~by~yourself?", 255)
        .AddAvatar(1).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("You~need~to~tell~General~Armand~about~the~Banditti`s~plan~within~2~minutes.~Hurry!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711475, 1))
                            {
                                if (HasItems(client, 711475, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    if (!client.Inventory.Remove(711475, 1, stream)) break;
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                    data.AddText("Yeah, that`s my Royal Sword! Without your help, I don`t even know how to face my father. I`ll be more cautious with those cunning Banditti. However, I feel I`m not")
                                        .AddText("~their real target.")
        .AddOption("You`d~better~return~home,~now.", 20)
        .AddAvatar(1).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Did you find the Royal Sword? No? My father will be angry with me, and he won`t forgive me. He treats that sword as his life. What can I do now?")
.AddOption("I`ll~go~look~for~it,~again.", 255)
.AddAvatar(1).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Um... Not very well. General Armand found out I was a girl soon after I arrived the camp. Although he didn`t reveal my true identity, he forced me to leave...")
.AddOption("Then~you~should~return~home.", 11)
.AddAvatar(1).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("I`ve thought of that, but I can`t return now, since the Royal Sword was taken by the Banditti. If they weren`t hiding in the dark, I wouldn`t be caught so easily.")
                            .AddText("~Anyway, I should take back the sword. You know, that was given by the Emperor to commemorate my father`s military exploits. It`s very important for my family.")

.AddOption("Don`t~worry.~I`ll~go~find~it.", 12)
.AddOption("That`s~terrible.", 255)
.AddAvatar(1).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("You~need~to~kill~the~Banditti~to~retrieve~the~Royal~Sword.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(1).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(723340, 0, 1, stream);
                            client.CreateBoxDialog("You~received~30~minutes~of~EXP,~3,000~Silver~and~a~Diligence~Book!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            data.AddText("How do you know my name? Did my father send you here? Alright. You don`t need to try and persuade me, since I won`t return home before realizing my father`s dream.")
.AddOption("You`re~the~dream~of~Harriet.", 10)
.AddAvatar(1).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Harriet)]
        public static void Harriet(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harriet, client.Player.Class, 1643);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harriet, client.Player.Class, 1644);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Mulan, client.Player.Class, 1649);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harriet, client.Player.Class, 1652);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Alas, you can`t understand how sad I was when I was forced to leave the army, due to injuries. You know, the battlefield was where my dream started.")
                            .AddText("~But now, I can only play with my hoe on the farm. Son? I don`t have a son, but a daughter. Haha, you must be kidding! How could a girl join the army? It`s illegal.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Where~is~Mulan?", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Mulan?", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~found~Mulan.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Flower~Wine.", 30);
                        data.AddOption("Take~it~easy.", 255);
                        data.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711477, 1))
                            {
                                if (HasItems(client, 711477, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {
                                    if (!client.Inventory.Remove(711477, 1, stream)) break;

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
                                    client.Inventory.Add(stream, ItemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP,~4,000~Silver~and~a~Refined~Gem!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 2000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Is that Flower Wine in your hands? It really smells nice! Owing to your help, I can have a taste of the Flower Wine, again. Why not drink with me?")
        .AddOption("No,~thanks.", 255)
        .AddAvatar(10).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You haven`t found the Flower Wine? What a pity! You know I haven`t had a good night since I smelt that bouquet. Could you understand?")
                          .AddOption("I`ll~go~check,~again.", 255)
                          .AddAvatar(10).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Hey! Do you smell something nice? It must be the Flower Wine! I bet this good bouquet is from the Banditti in the west. Eh... When did I drink Flower Wine last?")
                                  .AddText("~Perhaps, 2 or 3 years ago... That`s really an unforgettable taste. If you are able to bring me some Flower Wine, I`d be very grateful.")
                            .AddOption("No~problem!~Haha...", 31)
                            .AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("You~need~to~kill~the~Banditti~to~collect~the~Flower~Wine.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.Inventory.Add(stream, 160197, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~20~minutes~of~EXP,~2,000~Silver~and~a~pair~of~Unique~Kylin~Boots(B)!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            data.AddText("Really? Where is she now? Why didn`t you bring her home? Wait, wait... You mentioned that General Armand appreciated her effort and wanted to recruit her in the army?")
                                .AddText("~Is that true? Oh, my dear daughter! In that case, I don`t need to force her to return home. I just hope Mulan will be safe and happy.")
.AddOption("I~hope~so,~too.", 255)
.AddAvatar(10).FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You can`t find Mulan? Alas! What should I do? She is my only child... If she was caught by the bandits on the Island, I wouldn`t dare to continue living alone.")
                                .AddText("~Please, go look for her, again.")
     .AddOption("Okay.~I~will.", 255)
     .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        data.AddText("Yeah, that`s my daughter. Wait, that`s Mulan who bought me the wine... and she mentioned my dream, last night. Could this silly girl be thinking of joining the army for")
                                       .AddText("~me? Oh, no! If the general finds out, she will be put to death! I can`t move that fast, so I wonder whether you can go find her? Please, bring")
                                       .AddText("~her home! I won`t mention my military dreams, anymore.")
   .AddOption("I`ll~go~look~for~her.", 11)
   .AddOption("I~have~more~important~things.", 255)
   .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("You~need~to~look~for~Mulan~and~advise~her~to~return~home.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(1002030, 1))
                            {
                                if (HasItems(client, 1002030, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    if (!client.Inventory.Remove(1002030, 1, stream)) break;
                                    client.Inventory.AddItemWitchStack(1002010, 0, 5, stream);
                                    client.CreateBoxDialog("You~received~30~minutes~of~EXP,~3,000~Silver~and~5~Ginsengs!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                    data.AddText("Yeah, this is what I need. Don`t worry, I feel better now. Without you, I should have died at home. By the way, did you see Mulan?")
                                        .AddText("~How can she leave me at home, alone!")
    .AddOption("Anyway,~you~are~fine~now.", 10)
    .AddAvatar(10).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Where is the Refreshing Pill? You know where the Storekeeper is, right? Go buy a Refreshing Pill from her. I`m in urgent need of one to cure my headache.")
      .AddOption("I`m~on~my~way.", 255)
      .AddAvatar(10).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Yeah, where is my daughter? Mulan? Mulan? Gods, this headache is killing me. I must have drank too much, last night. Could you do me a favor and buy me a Refreshing Pill?")
                         .AddText("~I`m sure you can get one from the Storekeeper. Be quick!")
    .AddOption("Okay.~I`ll~be~back~soon!", 2)
    .AddOption("Sorry,but~I~don`t~carry~money.", 255)
    .AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("You~need~to~buy~a~Refreshing~Pill~from~the~Storekeeper~for~Harriet.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.VoteManager)]
        public static void VoteManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.Level >= 100 && client.Player.Reborn >= 1)
                            data.AddText("Welcome to VoteManger seems like you are eligible for vote please do remember that you can vote only once at 12 hours in exchange of your Vote you will get 1 Vote Point that can be exchanged in our shop at the moment you have " + client.Player.VotePoints + " VotePoints.")
                             .AddOption("Vote.", 40)
                             .AddOption("Claim CPS", 41)
                             .AddOption("I`ll think about it.", 255);
                        else
                            data.AddText("I'm sorry,but it appears you don't meet the requirements to be eligible to vote. Please return when you reach level 100 after your first rebirth.");
                        data.AddOption("I~see.", 255)
             .AddAvatar(3)
             .FinalizeDialog();
                        break;
                    }
                case 40:
                    {//bro just translate the file npc and send me tell me after translayte how u add 2 npc arabic and english in 1 npc how 
                        // return;
                        if (Database.VoteSystem.CanVote(client))
                        {
                            client.SendSysMesage(Program.ServerConfig.XtremeTopLink, MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.white);
                            client.Player.StartVote = true;
                            client.Player.StartVoteStamp = Extensions.Time32.Now.AddSeconds(30);
                            client.SendSysMesage("Please wait for system to check your vote");
                        }
                        else
                        {
                            data.AddText("You have already claimed your reward for voting. You may only vote once every 12 hour per IP address.")
 .AddOption("I~see.", 255)
 .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 41:
                    {//bro just translate the file npc and send me tell me after translayte how u add 2 npc arabic and english in 1 npc how 
                        // return;
                        if (client.Player.VotePoints == 0)
                        {

                            client.SendSysMesage("Sorry you don't have enough Points make vote now");
                        }
                        else
                        {
                            data.AddText("well, Check our Prizes which you can got from our server as a reward for voting for us , KeepUp Voting will get more Players [ TrueConquer ]")
                           .AddOption("[ " + client.Player.VotePoints + " Point ] [ " + client.Player.VotePoints * 250 + " CPS ].", 42)
                           .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 42:
                    {
                        uint amount = client.Player.VotePoints * 250;
                        if (client.Player.VotePoints == 0)
                        {

                            client.SendSysMesage("Sorry you don't have enough Points make vote now");
                        }
                        else
                        {
                            client.Player.ConquerPoints += (uint)amount;
                            client.Player.VotePoints = 0;
                            client.SendSysMesage("You got " + amount + " CPS for voting 4 us thx pro");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TempestSecretLetter)]
        public static void TempestSecretLetter(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            if (!client.Inventory.Contain(3006393, 1))
            {
                //Sun~Diamond,~Moon~Diamond,~Star~Diamond~and~Cloud~Diamond
                client.CreateBoxDialog("Sorry, you don`t have a TempestSecretLetter.");
                return;
            }



            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Taskmaster Chang (JobCenter 51,49) has news about the legendary talisman, Tempest Wings.")
                             .AddText("~If you`re interested, pay a visit to him.")
                          .AddOption("Visit~Taskmaster~Chang.", 1)
                          .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.Remove(3006393, 1, stream)) break;
                        client.SendSysMesage("You received 30 minutes of EXP!", MsgMessage.ChatMode.System);
                        client.GainExpBall(300, false);
                        client.Player.QuestGUI.SendAutoPatcher(1004, 51, 49, (uint)NpcID.TaskmasterChang);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralArmand)]
        public static void GeneralArmand(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Alvis, client.Player.Class, 1636);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Mulan, client.Player.Class, 1646);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1647);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1648);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1639);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1642);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("The most regretful thing of all, that man devoted all he had, but died without accomplishing what he wanted. We should hurry to seize the day!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Be~careful~of~the~Banditti!", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Is~there~anything~I~can~help?", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Is~there~anything~I~can~help?", 30);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Excuse~me,~do~you~know~Alvis?", 1);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Task:~Return~Favors.", 40);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Task:~Bandit`s~Saber.", 50);

                        data.AddOption("Take~it~easy.", 255);
                        data.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711474, 10))
                            {
                                if (HasItems(client, 711474, 10) && client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                                {
                                    if (!client.Inventory.Remove(711474, 10, stream)) break;
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP,~4,000~Silver~and~a~bottle~of~Exp~Potion!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Inventory.Add(stream, Database.ItemType.DoubleExp);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Wow, these Short Sabers are all of high quality. I knew you wouldn`t let me down. Thanks for your help!")
              .AddOption("You`re~welcome.", 255).AddAvatar(10).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Have you collected 10 Short Sabers? It seems not. You`re not going to sneak away, are you? The Bandits L98 are no match for you. Take it easy, okay?")
    .AddOption("Alright.~I`m~on~my~way.", 255).AddAvatar(10).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Sigh... It`s all my fault. If I didn`t underestimate the enemy, my troops wouldn`t suffer such heavy casualties in the battle. Anyway, the most")
                                       .AddText("~important thing is to collect some weapons. I noticed the Short Sabers those Bandits L98 use weren`t so bad. If you are able to bring me 10 Short Sabers, I`d")
                                       .AddText("~be very grateful.")
             .AddOption("You~can~count~on~me.", 51)
             .AddOption("Bandits?~I`ll~be~killed!", 255).AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("You~need~to~kill~the~Bandits~L98~and~collect~10~Short~Sabers~from~them.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Did you see the Village Head? No? Come on, don`t tell me you are lost. Hurry to the Fishing Village and send my gift to the Village Head, okay?")
     .AddOption("I`m~on~my~way.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        data.AddText("Hey! We have smashed the den and eliminated hundreds of bandits! See, people are all cheering our crushing victory. Without help from the Village Head, we couldn`t")
                                 .AddText("~achieve all this so easy. Are you going to return to the Fishing Village? Great! I wonder whether you can send this Jewelry Bag to the Village Head for me and")
                                 .AddText("~deliver my sincere thanks to him?")
       .AddOption("No~problem.~I`ll~tell~him.", 41)
       .AddOption("I~have~more~important~things.", 255).AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711471);
                        client.CreateBoxDialog("You~need~to~send~the~Jewelry~Bag~to~the~Village~Head.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText(" From Mulan`s issue, I think I should never underestimate anyone. Will Mulan return?")
           .AddOption("I~hope~so.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        data.AddText("Mulan? Yeah, I totally forgot that it was Mulan who discovered the Banditti`s plan. Without her report, we may have been defeated. I`m so sorry to think so little of that girl.")
                                        .AddText("~Tell you what... Could you ask her to return to the army? I`ll keep the secret for her, as long as she fights like a man!")
              .AddOption("I`d~like~to~have~a~try.", 31).AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("General~Armand~asked~you~to~find~Mulan~and~invite~her~back~to~the~army.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 3, 2))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Aha, these Banditti are hastily retreating from our land. It`s good enough. We don`t need to press an enemy in retreat. Thanks for your help!")
                                        .AddText("~Why not celebrate for the victory with us, tonight?")
              .AddOption("We~will~get~a~chance.~Bye!", 30).AddAvatar(10).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("I didn`t expect there would be so many enemies! Are you okay? Try your best to kill as many as possible!")
    .AddOption("I~can`t~be~better.~Let`s~go!", 255).AddAvatar(10).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I`m really looking forward to it. Actually, some Banditti have snuck into the camp! I hope you can join the fight with us. Will you?")
    .AddOption("Sure,~you~have~me~now.", 21).AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        Database.Server.AddMapMonster(stream, client.Map, 55, client.Player.X, client.Player.Y, 8, 8, 3, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        Database.Server.AddMapMonster(stream, client.Map, 79, client.Player.X, client.Player.Y, 8, 8, 3, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        break;
                    }
                case 10:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("What? The Banditti want to ally with the Bandits L98, then attack my troops? Were their brains eaten by zombies? I`ll go intensify the defenses, right away.")
                                .AddText("~Thanks for the news!")
      .AddOption("My~pleasure.", 20).AddAvatar(10).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 711468, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711468, 1, stream)) break;
                            client.Inventory.Add(stream, 117087, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~40~minutes~of~EXP,~4,000~Silver~and~a~Unique~Jade~Earring(B)!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 4000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Alvis)]
        public static void Alvis(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BiVillageHead, client.Player.Class, 1634);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Alvis, client.Player.Class, 1635);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Alvis, client.Player.Class, 1636);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Heeelp! The food supply was...was...taken. I can`t die like this! Help...me.....");


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Hey!~Are~you~Alvis?", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Food~Delivery~II.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Food~Delivery~III.", 20);

                        data.AddOption("It`s~not~me.~Let~me~go!", 255);
                        data.AddAvatar(9).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("What? You lost the food supply? Fortunately, I just gave you part of it. Anyway, here are the remains. Keep them well, and send them to General Armand!")
    .AddOption("Don`t~worry.~I~will.", 255).AddAvatar(9).FinalizeDialog();
                            break;
                        }
                        data.AddText("I`m afraid! Could you...could you go deliver this box of food to General Armand, on the Central Island? I hope you can finish this task for me.")
                            .AddText("~Remember, to General Armand. I won`t, won`t forget your help...")
       .AddOption("I`ll~take~care~of~this.", 21)
       .AddOption("I~have~more~important~things.", 255)
       .AddAvatar(9).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(9).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711468);
                        client.CreateBoxDialog("You~need~to~send~this~box~of~food~to~General~Armand.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711468, 1))
                            {
                                if (HasItems(client, 711468, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    if (!client.Inventory.Remove(711468, 1, stream)) break;
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Yeah, this is exactly what I lost. For...Fortunately, this box of food isn`t in the wrong hands, now.")
        .AddOption("Hey,~are~you~okay?", 20)
        .AddAvatar(9).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Did you find the food supply? No? Please, I can`t die with this great shame. I can`t...")
                              .AddOption("Come~on,~I`ll~go~find,~again.", 255)
                              .AddAvatar(9).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("The Bandits L97 looted the food supply. Could you... could you help me to retrieve them? You wouldn`t deny a dying request, would you?")
.AddOption("I`ll~go~find~the~food~supply.", 11)
.AddOption("Bandits?~My~goodness!", 255)
.AddAvatar(9).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("You~need~to~retrieve~the~food~supply~from~the~Bandits~L97.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {

                            client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Did th... the Village Head sent you here? I`m soooo sorry to make him disappointed. I don`t want to die like this...")
.AddOption("Hold~on,~you~will~be~fine.", 10)
.AddAvatar(9).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BiVillageHead)]
        public static void BiVillageHead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BiVillageHead, client.Player.Class, 1633);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BiVillageHead, client.Player.Class, 1634);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BiVillageHead, client.Player.Class, 1641);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BiVillageHead, client.Player.Class, 1650);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1639);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Here, our way is simple and people are hardworking. But since the Bandits L97 came, peace is no longer with us. Monsters from other islands continuously")
                            .AddText("threaten to destroy our village. As the head of this village, I should take full responsibility to protect the people, here. However, all of the strong villagers")
                            .AddText("~have been drafted into the army. How could I lead the weak that were left to fight against the cruel bandits?");



                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hey!~You~have~me,~now!", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("~Food~Delivery~!", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hunting~Bandits~L98.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hidden~Banditti.", 30);


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Return~Favors.", 40);

                        data.AddOption("It`s~terrible...", 255);
                        data.AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        if (HasItems(client, 711471, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            if (!client.Inventory.Remove(711471, 1, stream)) break;
                            client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 5719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Really? Those bandits are still enjoying beer and women? Good. I`ll make this disorderly mob collapse at the first blow!")
                                .AddText("~Thanks for telling me!")
.AddOption("It`s~my~pleasure!", 255)
.AddAvatar(56).FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 30))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
               .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                                    break;
                                }
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {
                                    client.Inventory.AddItemWitchStack(1002010, 0, 5, stream);
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP,~4,000~Silver~and~5~Ginsengs!");
                                    client.IncreaseExperience(stream, 5719000);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Finally, we are rid of those Banditti! My hero, please wait! We have prepared a feast to thank you. Would you mind staying here and celebrating with us?")
        .AddOption("Thanks,~but~maybe~next~time.", 255)
        .AddAvatar(56).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("It seems you haven`t killed 30 Banditti, have you? Make haste, please! I hope you can restore peace to our village.")
                           .AddOption("I`m~on~my~way.", 255)
                           .AddAvatar(56).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Oh, no! Someone told me 30 Banditti from the west and the northwest gathered outside the Fishing Village, again. It`s really annoying. Could you please go kill them?")
                            .AddOption("Don`t~worry.~It`s~on~me.", 31)
                            .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("You~need~to~kill~30~Banditti.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 30))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {

                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 5719000);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("You`ve killed 30 Bandits L98? Great! It`s the SECOND time you saved our village! I can never thank you enough.")
        .AddOption("It`s~my~pleasure.", 255)
        .AddAvatar(56).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("It couldn`t be true. I`m sure those Bandits L98 are still watching our village, nearby. Please go check again, okay?")
                  .AddOption("Alright.", 255)
                  .AddAvatar(56).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("It`s so good to see you! Those Bandits L97 haven`t give up their evil designs, and summoned lots of Bandits L98 from the desert. I can`t wait for them to rush")
                            .AddText("~our village. The best way to defend is to attack! One told me a group of Bandits L98 are lingering around. Could you go kill 30 Bandits L98 for us?")
                 .AddOption("Don`t~worry.~I~won`t~sit~back.", 21)
                 .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("You~need~to~kill~30~Bandits~L98.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("What? You didn`t find Alvis? It`s really worrying. According to his route, Alvis should be on the Central Island. Could you go there and check again?")
                    .AddOption("Alright.", 255)
                    .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        data.AddText("Um... It has been more than 15 days since Alvis went to deliver the food supply to General Armand on the Central Island, and I received no reply back. It shouldn`t be more")
                            .AddText("~than 10 days, unless Alvis had some kind of accident. Could you do me a favor and go have a look?")
                        .AddOption("Yeah,~I`m~on~it.", 11)
                        .AddOption("I~have~more~important~things.", 255)
                        .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("You~need~to~go~to~the~Central~Island~and~find~Alvis~around~(324,192).");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 50))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
               .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                                    break;
                                }
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {

                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP,~4,000~Silver~and~an~EXP~Ball~Scrap!");
                                    client.Inventory.AddItemWitchStack(720172, 0, 1, stream);
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Great job! I really don`t know how to thank you. You saved the whole village!")
        .AddOption("It`s~my~pleasure.", 255)
        .AddAvatar(56).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Have you killed 50 Bandits L97? It seems the answer is no... Look, if you just kill a few of them, you will find that you`re only provoking them.")
                                    .AddText("~Then, these bullies will take more revenge on us. I hope you can keep your promise to protect us from start to finish. You know, the bandits are on the")
                                    .AddText("~Central Island.")
                                    .AddOption("Okay.~I`m~on~my~way.", 255)
                                    .AddAvatar(56).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("You? I believe you are an able body, but there are many bandits, and all our young men have been drafted into the army. Are you sure you can deal with those brutes alone?")
                          .AddOption("Right.~You~can~count~on~me!", 2)
                          .AddOption("I~guess~I~was~too~impulsive...", 255)
                          .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 2:
                    {

                        data.AddText("It`s very kind of you! So, can you go kill 50 Bandits L97 on the Central Island, now? I suppose that is the best way to scare the other bandits away.")
                         .AddOption("Great!~I`m~going...", 3)
                         .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("You~need~to~go~kill~50~Bandits~L97~on~the~Central~Island.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.AdjutantLi)]
        public static void AdjutantLi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1640);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AdjutantLi, client.Player.Class, 6131);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Hey! You seem like an able body. Why not join the army? I`ll recommend you to the general!");



                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~came~to~deliver~gifts.", 1);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Deadly~Trap.", 10);

                        data.AddOption("You~must~be~kidding.~Bye!", 255);
                        data.AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720995, 1) && client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                if (HasItems(client, 720995, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    if (!client.Inventory.Remove(720995, 1, stream)) break;
                                    client.CreateBoxDialog("You~received~20~minutes~of~EXP~and~2,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 2000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("How are things going, now? Really? Those stupid Bandits L98 ate up all the Salted Fish? Aha, now we just need to wait and clear their bodies away from our land!")
                                        .AddText("~Did I say thank you? You`re incredible useful!")
        .AddOption("It`s~my~pleasure.", 255)
        .AddAvatar(10).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Are you ready to carry out the `Deadly Plan`? Go poison the Bandits L98!")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(10).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Buddy, I got a great idea to eliminate the Bandits L98! As you know, people here love the Salted Fish very much, including the Bandits L98. If they eat poisonous Salted Fish, things will be")
                                .AddText("~easy for us. You agree with me, right? Then, I`ll count you in the plan. Look, this is a bottle of Poison Liquid. I want you to take a pack of Salted Fish from Old Zhang,")
                                .AddText("~then use the Poison Liquid on the Salted Fish. When the liquid diffuses into the Salted Fish completely, you can entice the Bandits L98 to eat the Poison Fish. They`ll never see it coming!")
                                .AddText("~So?")
.AddOption("Got~it,~sir!", 11)
.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
.AddOption("Let me check.", 255)
.AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720994);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (HasItems(client, 711472, 1) && HasItems(client, 711473, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711472, 1, stream)) break;
                            if (!client.Inventory.Remove(711473, 1, stream)) break;
                            client.CreateBoxDialog("You~received~30~minutes~of~EXP,~3,000~Silver~and~a~Defense~Pot~(20m)!");
                            client.Inventory.AddItemWitchStack(723017, 0, 1, stream);
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Gifts? Wow, the Salted Fish smells so good! It`s very kind of the people from the Fishing Village! We are just doing our jobs.")
                                .AddText("~And what`s that? A Fishing Net? Yeah, we can use this net to make Salted Fish by ourselves! Old Zhang is so considerate.")
.AddOption("I`m~glad~you~like~them.", 255)
.AddAvatar(10).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        public static void BandittiFlowers(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1653);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                client.Player.AddPick(stream, "CollectingRainbowFlower", 2);

            }
        }
        [NpcAttribute(NpcID.OldZhang)]
        public static void OldZhang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1640);//1631);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1653);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Greetings! Have you ever tasted this Salted Fish? You should never miss this special food, when you come to our Fishing Village. Suppose you are eating a dried fish")
                            .AddText("~with the right salty taste, what would you think? You can even feel the warm sunshine and refreshing sea wind while eating! That`s amazing, right?");


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Can~I~have~a~taste?", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Rainbow~Flowers.", 10);

                        data.AddOption("Yes,~indeed.", 255);
                        data.AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711478, 10))
                            {
                                if (HasItems(client, 711478, 10) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    if (!client.Inventory.Remove(711478, 10, stream)) break;
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Yeah, these flowers are exactly what I am looking for. Actually, the Rainbow Flower is my secret! By chance, I found the bait made from the Rainbow Flowers and rice would")
                                        .AddText("~attract more fish. Now, I`ve told you all I know. Promise you won`t tell anyone, okay?")
        .AddOption("I~swear~I~won`t.", 255)
        .AddAvatar(111).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Just as its name indicates, the Rainbow Flower shows bright colors in the sun. So, you can easily recognize them. Remember to bring me 10 Rainbow Flowers!")
                       .AddOption("No~problem.", 255)
                       .AddAvatar(56).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Speaking of fishing skill, no one in the village is better than me. Haha, alright. I`ll tell you my secret, if you are able to bring me 10 Rainbow Flowers from the")
                         .AddText("~West Island and the Northwest Island. So, what do you say?")
                            .AddOption("Okay.~I`ll~be~back~soon!", 11)
                            .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("You~need~to~collect~10~Rainbow~Flowers~from~the~West~Island~and~the~Northwest~Island.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The gift itself may be small, but the goodwill is deep. Remember to express my sincere gratitude to Adjutant Li on the Central Island, too.")
                           .AddOption("They~will~understand.", 255)
                           .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        data.AddText("Of course! Here you are. How is the taste? I knew you would like it! By the way, did you notice the soldiers on the Central Island? They came to eliminate the")
                              .AddText("~bandits. I do appreciate their effort. Could you send this bag of Salted Fish to Adjutant Li of the army?")
                            .AddOption("No~problem.~Anything~else?", 2)
                            .AddOption("I~have~more~important~things..", 255)
                            .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Yeah, and this Fishing Net. They can go fishing in their free time. Hope they like the Salted Fish!")
                             .AddOption("You`re~very~kind.", 3)
                             .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("You~need~to~send~the~Fishing~Net~and~Salted~Fish~to~Adjutant~Li.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FishingNet)]
        public static void FishingNet(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1640);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                client.Player.AddPick(stream, "FishingNet", 3);
            }

        }
        [NpcAttribute(NpcID.SaltedFish)]
        public static void SaltedFish(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1640);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AdjutantLi, client.Player.Class, 6131);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                client.Player.AddPick(stream, "SaltedFish", 3);
            }
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                client.Player.AddPick(stream, "SaltedFish", 3);
            }
        }
        [NpcAttribute(NpcID.SoldiersMother)]
        public static void SoldiersMother(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZhang, client.Player.Class, 1631);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldiersMother, client.Player.Class, 1632);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldiersMother, client.Player.Class, 1651);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("My son has been away from home for almost 2 years. I`m really worried about him. I hope")
                            .AddText("~he can come back to me, soon.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Soldier`s~Letter.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Retrieve~Bacon.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Retrieve~Yarn~Ball.", 20);

                        data.AddOption("He`ll~be~back.", 255);
                        data.AddAvatar(111).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711476, 1))
                            {
                                if (HasItems(client, 711476, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    if (!client.Inventory.Remove(711476, 1, stream)) break;
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("That`s the Yarn Ball! I believe my son will be more handsome if he wears the new clothes.")
                                        .AddText("~I hope he`ll come back, soon.")
        .AddOption("He`ll~be~back,~soon.", 255)
        .AddAvatar(111).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Hurry up! My son is still wearing the old clothes he wore on the day he left home! That just won`t do!")
 .AddOption("Ok.", 255)
 .AddAvatar(111).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Brave hero, please help me! I want to make new clothes for my son, but the Yarn Ball was stole by a Banditti.")
                            .AddText("~Could you help me retrieve it?")
  .AddOption("Ok.", 21)
  .AddOption("Sorry~but~I`m~busy.", 255)
  .AddAvatar(111).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Kill~the~Banditti~and~retrieve~the~Yarn~Ball.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711467, 5))
                            {
                                if (HasItems(client, 711467, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    if (!client.Inventory.Remove(711467, 5, stream)) break;
                                    client.CreateBoxDialog("Congratulations!~You~received~40~minutes~of~Exp,~4,000~silver~and~1~Diligence~Book!");
                                    client.Inventory.AddItemWitchStack(723340, 0, 1, stream);
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Thank you for retrieving the bacon for me. If my son was home, the monsters wouldn`t have stolen my things so easily.")
        .AddOption("Take~care.", 255)
        .AddAvatar(111).FinalizeDialog();
                                }
                            }

                            else
                            {

                            }

                            break;
                        }
                        data.AddText("A few days ago, several Bandits stole my bacon, and I`m running out of food! Could you")
                            .AddText("~please help me retrieve some of my lost bacon?")
.AddOption("Ok.", 11)
.AddOption("Sorry,~but~I`m~busy.", 255)
.AddAvatar(111).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Help~Soldier`s~Mother~retrieve~5~pieces~of~Bacon~from~the~BanditL97.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (HasItems(client, 711466, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711466, 1, stream)) break;
                            client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~1~Small~Joy~Stone!");
                            client.Inventory.AddItemWitchStack(720173, 0, 1, stream);
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("What? A letter from my son? I feel much better now, since he says everything is fine. Thank you!")
.AddOption("You`re~welcome.", 255)
.AddAvatar(111).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierZhang)]
        public static void SoldierZhang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZhang, client.Player.Class, 1623);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZhang, client.Player.Class, 1630);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZhang, client.Player.Class, 1631);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("The HawKings are so terrible...");

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Can~I~help~you?", 1);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Revenge", 10);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Soldier`s~Letter.", 30);
                        if (client.Player.QuestGUI.CheckQuest(10, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("An~accessory", 40);

                        data.AddOption("Be~careful.", 255);
                        data.AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(10, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            client.Player.QuestGUI.Accept(new Database.QuestInfo.DBQuest() { MissionId = 10, Intentions = 1 }, 0);
                        if (client.Player.QuestGUI.FinishQuest(10))
                        {
                            client.CreateBoxDialog("Congratulations!~You~received~20~minutes~of~Exp,~2,000~silver~and~1~Diligence~Book!");
                            client.Inventory.AddItemWitchStack(723340, 0, 1, stream);
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("What? You got it from a soldier`s corpse? No! It must be my brother. I have a same one like this. Thank you, anyway.")
.AddOption("Take~care.", 255)
.AddAvatar(35).FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Please bring this letter to my mother. My home is on north Fishpond Island.")
      .AddOption("Ok.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        data.AddText("I`ve been here for 2 years, and never went back home. My old mother lives alone, on north Fishpond Island.")
       .AddOption("Do~you~miss~her?", 31).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("Of course. But the only thing I can do is to pray that she is healthy. Could you please give this letter")
                            .AddText("~to her? I want my mother to know everything is good, here.")
   .AddOption("Ok.", 32)
   .AddOption("Sorry,~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711466);
                        client.CreateBoxDialog("Send~the~soldier`s~letter~to~his~mother.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 30, 20))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~5~Ginseng!");
                                    client.Inventory.AddItemWitchStack(1002010, 0, 5, stream);
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("You killed the monsters, but my friends still won`t come back... Thanks for your help, anyway.")
        .AddOption("Good~luck.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Kill the Hawks! That`s the only way to avenge my dead friends.")
                                             .AddOption("Ok.", 255).AddAvatar(35).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I`m dreaming of my dead friends, everyday. Just painful... How I wish I could avenge them! But I`m just too")
                            .AddText("~weak to kill the Hawks, myself. Could you please kill 30 HawKings and 20 HawkL93 for me?")
             .AddOption("Ok.", 11)
             .AddOption("Sorry,~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Kill~30~HawKings~and~20~HawkL93.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 5))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("You did it! Thank you very much! Rest in peace, my friends...")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Hurry up! I hope that they will rest in peace!")
                .AddOption("Ok.", 255).AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("This war was so awful. Though we won, many soldiers died during the war. Could you please kill some HawkL93")
                  .AddText("~and collect their heads? I want you to cremate the heads and dead soldiers` bodies as a memorial.")
                  .AddOption("Okay.", 2)
                  .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Collect~5~heads~of~the~HawkL93~and~cremate~them~with~dead~soldiers`~bodies.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        public static void SoldierBird(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZhang, client.Player.Class, 1623);

            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                switch (Option)
                {
                    case 0:
                        {

                            data.AddText("Are you sure you want to burn the Soldier`s Corpse and the Hawk Head?");

                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Yes.", 1);

                            data.AddOption("No.", 255);
                            data.AddAvatar(35).FinalizeDialog();
                            break;
                        }
                    case 1:
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                            packet.ID = MsgStringPacket.StringID.Effect;
                            packet.UID = client.OnRemoveNpc.UID;
                            packet.Strings = new string[1] { "M_Fire1" };
                            client.Player.View.SendView(stream.StringPacketCreate(packet), true);


                            client.OnRemoveNpc.Respawn = Extensions.Time32.Now.AddSeconds(10);
                            client.Map.RemoveNpc(client.OnRemoveNpc, stream);
                            client.Map.soldierRemains.TryAdd(client.OnRemoveNpc.UID, client.OnRemoveNpc);

                            break;
                        }
                }
            }
        }
        [NpcAttribute(NpcID.MysteryTaoist)]
        public static void MysteryTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 1619);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1622);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1624);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1625);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1626);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1627);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1628);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1629);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("So my brother sent you here? The HawKings are really a great trouble to us. I will kill as many as possible.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I`ll~stop~them", 1);

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Claws~and~Fangs.", 10);

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Hawk`s~Treasure.", 20);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Hawk`s~Treasure~2.", 30);

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Elemental~Circle.", 40);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Elemental~Circle~2.", 50);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Elemental~Circle~3.", 60);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Elemental~Circle~4.", 70);

                        data.AddOption("Good~luck.", 255);
                        data.AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest8.MissionId, 1))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {

                                    data.AddText("Please make 1 more space in your inventory.")
                                          .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                                    break;
                                }
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                                {
                                    client.Inventory.Add(stream, 120187, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~a~Level~97~Unique~necklace~(B)!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Without their leader, the Hawks will be easily destroyed. Thanks for your help!")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                if (client.Player.View.ContainMobInScreen("HawkLeader") == false)
                                    Database.Server.AddMapMonster(stream, client.Map, 4431179, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                                data.AddText("Sorry, but I`m busy activating the elemental circle.")
      .AddOption("Good~luck!", 255)
      .AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Once the elemental circle is activated, the leader of the Hawks will appear. He will disappear after 1 minute,")
                     .AddText("~so you`ll need to hurry! Are you ready?")
                     .AddOption("Yes.", 71)
                     .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 71:
                    {
                        if (client.Player.View.ContainMobInScreen("HawkLeader") == false)
                            Database.Server.AddMapMonster(stream, client.Map, 4431179, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                        client.CreateBoxDialog("Kill~the~Leader~of~the~Hawks~within~1~minute,~after~the~elemental~circle~is~activated.~Are~you~ready?");
                        client.Player.QuestGUI.Accept(ActiveQuest8, 100);
                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest7.MissionId, 10))
                            {
                                if (HasItems(client, 711465, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                                {
                                    if (!client.Inventory.Remove(711465, 1, stream)) break;
                                    client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~5~Panacea!");
                                    client.IncreaseExperience(stream, 3719000);

                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Thanks for your help! The smell will lure the HawkL93 into the elemental circle, and then we can kill them all.")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You haven`t collected 10 scents of the HawkL93. Hurry up!")
                                   .AddOption("I`m~on~my~way.", 255).AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The elemental circle is finally complete! Now, I need you to collect 10 scents of the HawkL93.")
                        .AddText("~We can place the scent inside of the circle, to lure them into it. Here is a scent bag. You can use it to")
                        .AddText("~store the scent.")
                        .AddOption("Easy.", 61)
                        .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711465);
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.CreateBoxDialog("Go~and~collect~10~scents~of~the~HawkL93.");
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                                {

                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp~and~3,000~silver!");
                                    client.IncreaseExperience(stream, 3719000);

                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Great! Now I can focus on the elemental circle. Once complete, the HawKings will all be killed!")
        .AddOption("I`m~looking~forward~to~it.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Hurry up! We can`t give the HawKings the chance to disturb the summoning of the elemental circle.")
                          .AddOption("I~see.", 255).AddAvatar(35).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("I`m ready to summon the elemental circle. Would you please go to the southern bridge and ask")
                           .AddText("~the Truth Taoist to prevent the HawKings from disturbing me? He may also need your help.")
                           .AddOption("Alright.", 51)
                           .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("Go~and~talk~with~the~Truth~Taoist.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711461, 1) && client.Inventory.Contain(711462, 1)
                                && client.Inventory.Contain(711463, 1) && client.Inventory.Contain(711464, 1))
                            {
                                if (HasItems(client, 711461, 1) && HasItems(client, 711462, 1) && HasItems(client, 711463, 1) && HasItems(client, 711464, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                                {
                                    if (!client.Inventory.Remove(711461, 1, stream)) break;
                                    if (!client.Inventory.Remove(711462, 1, stream)) break;
                                    if (!client.Inventory.Remove(711463, 1, stream)) break;
                                    if (!client.Inventory.Remove(711464, 1, stream)) break;
                                    client.Inventory.AddItemWitchStack(1060024, 0, 1, stream);

                                    client.CreateBoxDialog("Congratulations!~You~received~40~minutes~of~Exp,~4,000~silver~and~1~Bird~Island~Gate!");
                                    client.IncreaseExperience(stream, 3719000);

                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Now I`ve got all 5 items! I hope the elemental circle will have a great effect. Thanks for your help!")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You haven`t collect all the items I need. Hurry up!")
                          .AddOption("Ok.", 255).AddAvatar(35).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("After a long time of studying, I`ve come up with an elemental circle that can lure out the leader of the")
                            .AddText("~HawKings. If we kill the leader, things will become easier! Could you lend me a hand?")
                            .AddOption("Of~course.", 41)
                            .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        data.AddText("To complete the elemental circle, we need 5 items, each standing for an element. I`ve got the earth item. Could you please")
                           .AddText("~collect the Birdman`s golden beak, Ximen Qing`s wood sword, Cook Yuan`s water bag and Wealthy Wan`s gunpowder?")
                           .AddOption("Ok.", 42)
                           .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        client.CreateBoxDialog("Collect~the~items~which~stand~for~the~gold,~water,~wood~and~fire~elements.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711460, 1))
                            {
                                if (HasItems(client, 711460, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {
                                    if (!client.Inventory.Remove(711460, 1, stream)) break;
                                    client.Inventory.Add(stream, 152187);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~a~Unique~Blue~Bracelet~(B)!");
                                    client.IncreaseExperience(stream, 3719000);

                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("These treasures were actually taken away from the people here. Now, we should return the treasures")
                                        .AddText("~to their owners. Thanks for your help!")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Hurry up! We must take away all the treasures before the Hawks notice it.")
                                .AddOption("Ok.", 255).AddAvatar(35).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Finally, we`ve got the location of the HawkL93`s treasure! It`s buried under the tree (445, 681).")
                                    .AddText("~Could you please go and dig it up? I`ll distribute what we make to the poor of the village.")
                                 .AddOption("Ok.", 31)
                                 .AddOption("Sorry,~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("Find~the~tree~(445,~681)~and~dig~up~the~treasure.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711459, 1))
                            {
                                if (HasItems(client, 711459, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    if (!client.Inventory.Remove(711459, 1, stream)) break;
                                    client.Inventory.AddItemWitchStack(1002010, 0, 5, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~5~Ginseng!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Thanks for helping me reveal the truth, brave hero! Now, I just need some time to find out where the treasures are hidden.")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("This map may help us find some treasures, or even secrets of the HawkL93! Please go and")
                                    .AddText("~collect some blood of the hawks to reveal the secret of the map!")
                                 .AddOption("Okay.", 255).AddAvatar(35).FinalizeDialog();

                            }

                            break;
                        }
                        data.AddText("A few days ago, I accidently stumbled upon the HawkL93`s treasure map, but it is cursed that only the blood of the HawkL93")
   .AddText("~can reveal the true path. Could you help me collect some of their blood?")
                            .AddOption("I`ll~try~my~best.", 21)
                            .AddOption("Leave.", 255)
     .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711458);
                        client.CreateBoxDialog("Collect~some~Hawk~Blood~to~reveal~the~secret~of~the~treasure~map.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711456, 1) && client.Inventory.Contain(711457, 1))
                            {
                                if (HasItems(client, 711456, 1) && HasItems(client, 711457, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    if (!client.Inventory.Remove(711456, 1, stream)) break;
                                    if (!client.Inventory.Remove(711457, 1, stream)) break;
                                    client.Inventory.AddItemWitchStack(720173, 0, 1, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~1~Small~Joy~Stone!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("I will find out the weakness of the HawkL93 with all these things you collected for me. Thanks a lot!")
        .AddOption("You`re~welcome!", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("If we have the claws and fangs of the HawkL93, we`ll know them better, and it will be much")
                                    .AddText("~easier to destroy them. Hurry up!")
    .AddOption("I~will.", 255)
    .AddAvatar(35).FinalizeDialog();

                            }
                            break;
                        }
                        data.AddText("The HawkL93 is strong and powerful. It`s very difficult to defeat them in a group. Could you please collect some of their claws and fangs?")
                            .AddText("~I want to know what makes them so undefeatable.")
                            .AddOption("Ok.", 11)
                            .AddOption("Sorry,~but~I`m~busy.", 255)
                            .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 11:
                    {

                        client.CreateBoxDialog("Collect~the~claws~and~fangs~of~the~HawkL93.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (HasItems(client, 720992, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(720992, 5, stream)) break;
                            client.CreateBoxDialog("Both~bridges~have~been~blocked~and~the~Haw~Kings~won`t~escape!~You`ve~received~EXP~worth~1/2~EXP~Ball~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("So you`re here to warn me? Thanks! Since the bridges have been blocked, the Haw Kings will not be able to escape!")
.AddOption("You`re~welcome.", 255)
.AddAvatar(35).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierGuan)]
        public static void SoldierGuan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CookYuan, client.Player.Class, 1613);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierGuan, client.Player.Class, 1614);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("It`s boring to stay here all the time... How I miss the wine from the city!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                                data.AddOption("Deliver~food.", 1);
                        }
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Soldier`s~Story", 10);


                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Please talk to me! I`ve been here alone for such a long time. Do you want to hear my story?")
                          .AddOption("Tell~me.", 11)
                          .AddOption("Sorry,~but~I`m~busy.", 255)
                          .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("Haha. Actually, I was a general, once. The reason I`m here now is because I loved drinking so much that")
                            .AddText("~I always got drunk, even on duty. My superior sent me here as a punishment.")
                         .AddOption("Don`t~drink~too~much.", 12)
                         .AddAvatar(237).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("Congratulations! You received 10 minutes of Exp and 1,000 Silver!", MsgMessage.ChatMode.System);
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        data.AddText("Great! I was really starving. Thanks a lot!")
                            .AddOption("You`re~welcome.", 255)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierXu)]
        public static void SoldierXu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CookYuan, client.Player.Class, 1613);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierXu, client.Player.Class, 1615);//1614);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I`ve been away from home for such a long time... I miss my family so much...");


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1) == false)
                                data.AddOption("Deliver~food.", 1);
                        }
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Worried~Soldier", 10);


                        data.AddOption("Homesick~soldier.", 255);
                        data.AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("These questions are driving me crazy! Could you please help me?")
                           .AddOption("Ok.", 11)
                           .AddOption("Sorry,~but~I`m~busy.", 255)
                           .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        data.AddText("What country is most popular on Thanksgiving?")
                         .AddOption("China.", 19)
                         .AddOption("Turkey.", 12)
                         .AddOption("Japan.", 19)
                         .AddOption("France.", 19)
                         .AddOption("No~idea.", 255)
                         .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("What tree is always very sad?")
                      .AddOption("Weeping~Willow.", 13)
                      .AddOption("Pine.", 19)
                      .AddOption("Christmas~tree.", 19)
                      .AddOption("Cypress.", 19)
                      .AddOption("No~idea.", 255)
                      .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.AddText("What letter is also a body of water?")
                    .AddOption("W.", 19)
                    .AddOption("A.", 19)
                    .AddOption("E.", 19)
                    .AddOption("C.", 14)
                    .AddOption("No~idea.", 255)
                    .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        data.AddText("Why is a river rich?")
                  .AddOption("It~has~water.", 19)
                  .AddOption("It~has~banks.", 15)
                  .AddOption("It~has~fish.", 19)
                  .AddOption("It~has~money.", 19)
                  .AddOption("No~idea.", 255)
                  .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        data.AddText("What`s too much for two and just right for one?")
                .AddOption("Friend.", 19)
                .AddOption("Time.", 19)
                .AddOption("Secret.", 16)
                .AddOption("Room.", 19)
                .AddOption("No~idea.", 255)
                .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 19:
                    {
                        data.AddText("Sorry, but you`ll need to go study up a little more.")
                              .AddOption("Ok.", 255)
                              .AddAvatar(47).FinalizeDialog();
                        break;
                    }

                case 16:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("Congratulations!~You~received~10~minutes~of~Exp~and~1,000~silver!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("Correct! You`re really smart!")
                          .AddOption("Haha.", 255)
                          .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                        data.AddText("Great! I was really starving. Thanks a lot!")
                            .AddOption("You`re~welcome.", 255)
                            .AddAvatar(30).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierZheng)]
        public static void SoldierZheng(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CookYuan, client.Player.Class, 1613);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZheng, client.Player.Class, 1616);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I`ve been training hard to become a soldier who can protect the island.");


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1) == false)
                                data.AddOption("Deliver~food.", 1);
                        }
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Soldier`s~Challenge", 10);


                        data.AddOption("Good~luck.", 255);
                        data.AddAvatar(55).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~10~minutes~of~Exp,~1,000~silver~and~5~Panacea!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 1000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                }
                                data.AddText("You`re really a brave hero. I lose.")
                                  .AddOption("Haha.", 255)
                                  .AddAvatar(55).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Kill 10 HawKings in 3 minutes. Hurry up!")
 .AddOption("Ok.", 255)
 .AddAvatar(55).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I heard that you`re a brave hero... Let`s play a game! If you can kill 10 HawKings within 3 minutes, you`ll win!")
    .AddOption("It`s~easy.", 11)
    .AddOption("Sorry,~but~I`m~busy.", 255)
    .AddAvatar(55).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 180);
                        client.CreateBoxDialog("Please~kill~10~HawKings~within~3~minutes.~Are~you~ready?");

                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                        data.AddText("Great! I was really starving. Thanks a lot!")
                            .AddOption("You`re~welcome.", 255)
                            .AddAvatar(55).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierFei)]
        public static void SoldierFei(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CookYuan, client.Player.Class, 1613);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierFei, client.Player.Class, 1617);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Before I joined the army, I married my childhood sweetheart. Sigh, I really miss my beautiful wife...");


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 1) == false)
                                data.AddOption("Deliver~food.", 1);
                        }

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Soldier`s~Request", 10);

                        data.AddOption("Sigh...", 255);
                        data.AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {

                                data.AddText("Please make 1 more space in your inventory.")
                                      .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(720991, 1))
                                client.Inventory.Add(stream, 720991);
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.CreateBoxDialog("Congratulations!~You~received~10~minutes~of~Exp~and~1,000~silver!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 1000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                }
                                data.AddText("Thanks for your help! I will never let anyone damage my wife`s portrait.")
                                  .AddOption("You`re~welcome.", 255)
                                  .AddAvatar(55).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please help me bury the portrait under the tree, next to me.")
                        .AddOption("Ok.", 255)
                        .AddAvatar(56).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("I was sent here not long after I got married, so I miss my beautiful wife very much. I have a portrait of her,")
                            .AddText("~but I`m afraid it will be lost in battle. Could you please bury it under the tree, next to us?")
                           .AddOption("Ok.", 11)
                           .AddOption("Sorry,~but~I`m~busy.", 255)
                           .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720991, 1))
                            client.Inventory.Add(stream, 720991);
                        client.CreateBoxDialog("Bury~the~portrait~under~the~tree~next~to~Soldier~Fei.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {

                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 1);
                        data.AddText("Great! I was really starving. Thanks a lot!")
                            .AddOption("You`re~welcome.", 255)
                            .AddAvatar(56).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.CookYuan)]
        public static void CookYuan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CookYuan, client.Player.Class, 1613);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1626);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Hello, hero! I`m the cook for the army on Bird Island, and I`m really busy. I`d be")
                            .AddText("~really happy if you could help me solve a problem I`m having.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && !client.Inventory.Contain(711462, 1))
                            data.AddOption("Elemental~Circle.", 10);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("At~your~service.", 1);




                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("So you want my water bag to complete the elemental circle? I`m happy I can help! Here you are.")
                            .AddOption("Thanks.", 11)
                            .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.CreateBoxDialog("You~received~the~Water~Bag~from~the~Cook~(water~element)!");
                        client.Inventory.Add(stream, 711462);

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Thanks for your help! I can have a little break, now.")
.AddOption("You`re~welcome.", 255)
.AddAvatar(35).FinalizeDialog();
                                }

                            }
                            else
                            {
                                data.AddText("Hurry up! Our soldiers are hungry!")
           .AddOption("Ok.", 255)
           .AddAvatar(35).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("Great! Actually, my assistant has become ill, so I need someone to help me deliver food to the 4 soldiers on the island.")
              .AddOption("I`ll~help~you.", 2)
              .AddOption("Sorry,~but~I`m~busy.", 255)
              .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Deliver~food~to~the~soldiers~in~the~4~corners~of~the~island.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.SouthwestIsland)]
        public static void SouthwestIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
            }
        }
        [NpcAttribute(NpcID.WestIsland)]
        public static void WestIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
            }
        }
        [NpcAttribute(NpcID.NorthwestIsland)]
        public static void NorthwestIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
            }
        }
        [NpcAttribute(NpcID.NorthIsland)]
        public static void NorthIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 1);
            }
        }
        [NpcAttribute(NpcID.NortheastIsland)]
        public static void NortheastIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 0, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 1);
            }
        }
        [NpcAttribute(NpcID.CentralIsland)]
        public static void CentralIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 0, 0, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 0, 1);
            }
        }
    }
}
