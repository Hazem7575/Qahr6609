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
        [NpcAttribute(NpcID.Arthur)]
        public static void Arthur(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This place can be quite dangerous. If you want to leave, just come talk to me.")
                            .AddText("Well.. i can help you to get vip(Level 6) token. \n")
                            .AddText("There are 4 stages for you to complete the Vip6 Quest: \n")
                        .AddText("Stage 1. Change 3 Moss, 3 SoulAroma and 3 DreamGras for an GhostHorn. \n")
                        .AddText("Stage 2. Change 3xGhostHorn for an EvilTooth. \n")
                        .AddText("Stage 3. Change 4xEvilTooth for an ImpureVigor. \n")
                        .AddText("Stage 4. Change 5xImpureVigor for an 1DayVIP token. \n")
                            .AddOption("I have (Moss,SoulAroma,DreamGras)", 1)
                             .AddOption("I have 3xGhostHorn to trade.", 2)
                             .AddOption("I have 4xEvilTooth to trade.", 3)
                             .AddOption("I have 5xImpureVigor to trade", 4)
                             .AddOption("I have 10xImpureVigor to trade for Hunting Layer Level 15", 20)
                             .AddOption("Teleport~to~Twin~City", 5)
                   .AddOption("I`ll~think~it~over.", 255).FinalizeDialog();

                        break;

                    }
                case 20:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                   .AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(722730, 10, 0))
                        {
                            if (!client.Inventory.Remove(722730, 10, stream)) break;


                            if (DateTime.Now > client.Player.ExpireVip)
                                client.Player.ExpireVip = DateTime.Now.AddDays(1);
                            else
                                client.Player.ExpireVip = client.Player.ExpireVip.AddDays(1);

                            client.Player.VipLevel = 6;

                            client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                            client.Player.UpdateVip(stream);

                            Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Vip6 Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                            data.AddText("You successfully exchange the ImpureVigor for an 1DayVIP!")
                          .AddOption("Thank you.", 255).FinalizeDialog();

                            //client.Player.AddDropLayer(15);

                            //client.CreateBoxDialog("You`ve received Drop Layer Level 15 (7 days).");


                          
                        }
                        else
                        {
                            data.AddText("I'm sorry, you need at least 10 ImpureVigor in order for me to help you.")
                      .AddOption("I see.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                   .AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(722730, 5, 0))
                        {
                            if (!client.Inventory.Remove(722730, 5, stream)) break;
                            if (DateTime.Now > client.Player.ExpireVip)
                                client.Player.ExpireVip = DateTime.Now.AddDays(1);
                            else
                                client.Player.ExpireVip = client.Player.ExpireVip.AddDays(1);

                            client.Player.VipLevel = 6;

                            client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                            client.Player.UpdateVip(stream);

                            client.CreateBoxDialog("You`ve received VIP6 (1 day).");


                            Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Vip6 Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                            //  data.AddText("You successfully exchange the ImpureVigor for an 1DayVIP!")
                            ///.AddOption("Thank you.", 255).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I'm sorry, you need at least 5 ImpureVigor in order for me to help you.")
                      .AddOption("I see.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                   .AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(722721, 4, 0))
                        {
                            if (!client.Inventory.Remove(722721, 4, stream)) break;
                            client.Inventory.Add(stream, 722730);
                            data.AddText("You successfully exchange the EvilTooth for an ImpureVigor!")
                       .AddOption("Thank you.", 255).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I'm sorry, you need at least 5 EvilTooth in order for me to help you.")
                      .AddOption("I see.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                   .AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(722722, 3, 0))
                        {
                            if (!client.Inventory.Remove(722722, 3, stream)) break;
                            client.Inventory.Add(stream, 722721);
                            data.AddText("You successfully exchange the GhostHorn for an EvilTooth!")
                       .AddOption("Thank you.", 255).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I'm sorry, you need at least 5 GhostHorn in order for me to help you.")
                      .AddOption("I see.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                   .AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(722723, 3, 0)
                            && client.Inventory.Contain(722724, 3, 0)
                            && client.Inventory.Contain(722725, 3, 0))
                        {
                            if (!client.Inventory.Remove(722723, 3, stream)) break;
                            if (!client.Inventory.Remove(722724, 3, stream)) break;
                            if (!client.Inventory.Remove(722725, 3, stream)) break;
                            client.Inventory.Add(stream, 722722);
                            data.AddText("You successfully exchange the (Moss,SoulAroma and DreamGras) for an GhostHorn!")
                       .AddOption("Thank you.", 255).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I'm sorry, you need at least 3xMoss, 3xSoulAroma and 3xDreamGras in order for me to help you.")
                      .AddOption("I see.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        client.Teleport(299, 298, 1002);
                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.Norbert)]
        public static void Norbert(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(721262, 5) && client.Inventory.Contain(721263, 1))
                        {
                            data.AddText("Hi, you have got the meterial i want. Do you come for the Bomb?")
                 .AddOption("Yes", 10)
                 .AddOption("Just passing by.", 255)
                 .AddAvatar(67).FinalizeDialog();

                            break;
                        }
                        data.AddText("Five~pieces~of~Saltpeter~and~one~piece~of~Sulphur~are~required~to~make~the~Bomb.~And~I~charge~a~fee~of~30,000~silvers.")
                   .AddOption("What~is~the~Bomb?", 1)
                   .AddOption("I~see.", 255)
                   .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("It`s used to blow down the guild gate! I participated in a guild war and witnessed lots of people lost their lives when attacking the gate.")
                            .AddText("~Since then, I have been working on inventing powerful bombs which can destroy the guild gate. Finally, I make it!")
                   .AddOption("Can~you~make~one~for~me?", 2)
                   .AddOption("Oh.~I~see.", 255)
                   .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("No problem. But you know such bombs are powerful and dangerous. Whoever uses it to destroy a guild gate will be killed by the bomb, too!")
                  .AddOption("Thanks.~I~need~it~badly.", 3)
                  .AddOption("Then~forget~it.", 255)
                  .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Good. You`re going to collect 5 Saltpeter and a Sulphur. Besides, you`ll need to pay 30,000 Silver as fees.")
                   .AddOption("Where~do~I~get~the~materials?", 4)
                   .AddOption("I`ll~give~up.", 255)
                   .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("You~can~get~Saltpeter~from~my~friend~OldQuarrier~in~Ape~City~and~get~Sulphur~from~the~Bandits~nearby.")
                   .AddOption("Okay,~I`ll~be~back~soon.", 255)
                   .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(721262, 5) && client.Inventory.Contain(721263, 1))
                        {
                            if (!client.Inventory.Remove(721262, 5, stream)) break;
                            if (!client.Inventory.Remove(721263, 1, stream)) break;
                            client.Inventory.Add(stream, 721261);
                            data.AddText("You can take the Bomb away. Remember that you`ll be killed once you use it to destroy the guild gate!")
                   .AddOption("I see. Thanks for the remind.", 255)
                   .AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.OldQuarrier)]
        public static void OldQuarrier(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Let me guess. It`s Norbert who asked you to come here. Am I right?")
                  .AddOption("Yeah,~I~need~a~Saltpeter.", 1)
                  .AddOption("Nope.", 2)
                  .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721262);
                            data.AddText("My~pleasure.~Here~you~are.")
             .AddOption("Thank~you.", 255)
             .AddAvatar(91).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory.")
              .AddOption("Let me check.", 255)
              .AddAvatar(91).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        data.AddText("Are~you~coming~for~Saltpeter?~Since~Norbert~invented~Bomb,~I~have~been~kept~busy~with~supplying~Saltpeter~for~him.")
                            .AddOption("What~is~the~Bomb?", 3)
                            .AddOption("Sorry~to~bother~you.", 255)
                            .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("It~is~powerful~and~used~to~bomb~out~City~Gate.~If~you`re~interested~in~it,~you~can~visit~Norbert~ for the details. He lives in~the~small~village~of~the~Maple Forest")
                             .AddOption("I~see.", 255)
                             .AddAvatar(91).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DrYinYang)]
        public static void DrYinYang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you want to change your gender for a new life, I can help you. Before making the")
                            .AddText("~magic of gender reassignment works, you need to pay 1075 CPs, and make sure you`ve understood")
                            .AddText("~the rules and procedures about the reassignment.")
                   .AddOption("Please~change~my~gender,~now.", 1)
                   .AddOption("What~else~should~I~know?", 2)
                   .AddOption("Not~interested.", 255)
                   .AddAvatar(218).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("If you want to change your gender for a new life, I can help you. Before making the")
                            .AddText("~magic of gender reassignment works, you need to pay 1075 CPs, and make sure you`ve understood")
                            .AddText("~the rules and procedures about the reassignment.")
                   .AddOption("Please~change~my~gender,~now.", 1)
                   .AddOption("What~else~should~I~know?", 2)
                   .AddOption("Not~interested.", 255)
                   .AddAvatar(218).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.Spouse != "None")
                        {
                            data.AddText("You`re married. To change your gender, you need to get a divorce, first.")
                .AddOption("Okay.", 255)
                .AddAvatar(218).FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints >= 1075)
                        {
                            if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Garment) == false
                           || !client.Equipment.FreeEquip(Role.Flags.ConquerItem.AleternanteGarment))
                            {
                                data.AddText("Please remove your garment.")
                   .AddOption("Okay.", 255)
                   .AddAvatar(218).FinalizeDialog();
                                break;
                            }
                            client.Player.ConquerPoints -= 1075;
                            if (Role.Core.IsBoy(client.Player.Body))
                            {
                                ushort typ = (ushort)(client.Player.Body % 10);
                                switch (typ)
                                {
                                    case 3: typ = 1; break;
                                    case 4: typ = 2; break;
                                }
                                client.Player.Body = (ushort)(2000 + typ);
                                client.Player.SendUpdate(stream, client.Player.Mesh, MsgUpdate.DataType.Mesh);
                                client.Player.View.ReSendView(stream);
                            }
                            else
                            {
                                ushort typ = (ushort)(client.Player.Body % 10);
                                switch (typ)
                                {
                                    case 1: typ = 3; break;
                                    case 2: typ = 4; break;
                                }
                                client.Player.Body = (ushort)(1000 + typ);
                                client.Player.SendUpdate(stream, client.Player.Mesh, MsgUpdate.DataType.Mesh);
                                client.Player.View.ReSendView(stream);
                            }
                            Database.DataCore.SetCharacterSides(client.Player);
                            if (client.Player.Nobility != null)
                            {
                                client.Player.Nobility.Gender = client.Player.GetGender;
                                client.Player.Nobility.Mesh = client.Player.Mesh;
                            }
                            if (client.Player.MyGuild != null && client.Player.MyGuildMember != null)
                                client.Player.MyGuildMember.Mesh = client.Player.Mesh;
                            if (client.ArenaStatistic != null)
                                client.ArenaStatistic.Mesh = client.Player.Mesh;
                            if (client.TeamArenaStatistic != null)
                                client.TeamArenaStatistic.Mesh = client.Player.Mesh;

                            MsgMessage messaj = new MsgMessage(client.Player.Name + ", Changed He's/Hers mesh into male / female.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System);


                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                        }
                        else
                        {
                            data.AddText("Gender reassignment requires 1,075 CPs. Make sure you have enough money with you.")
                 .AddOption("Okay.", 255)
                 .AddAvatar(218).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        data.AddText("To make gender reassignment as simple as possible, I want you to be single, not married. Make sure you`ve")
                            .AddText("~properly handled with your clan and guild affairs. After the reassignment, your name will be removed")
                            .AddText("~from the Charm ranking, and your garment will be put into your inventory. All in all, you need to carefully")
                   .AddOption("I~want~to~change~my~gender.", 3)
                   .AddOption("I`ll~think~about~it.", 255)
                   .AddAvatar(218).FinalizeDialog();

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.NameRegister)]
        public static void NameRegister(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Good day! If you want to turn a new page in your life, you can pay me 810 CPs to get a new name. Your new name will be registered, immediately.")
                   .AddOption("Okay,~here`s~my~CPs!", 1)
                   .AddOption("Reset edit times", 10)
                   .AddOption("I`ll~think~about~it.", 255)
                   .AddAvatar(31).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        data.AddText("Do you want to reset your name for 10k Conquer Points ?")
                        .AddOption("Yes.", 11).AddOption("No , thanks !", 255)
                        .AddAvatar(31).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Player.NameEditCount = 0;
                            data.AddText("Your name reset was finished succesfully!")
                       .AddOption("thank you", 255)
                      .AddAvatar(31).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                       .AddOption("Okay.", 255)
                      .AddAvatar(31).FinalizeDialog();
                        }
                        break;
                    }

                case 1:
                    {
                        if (client.Player.ConquerPoints >= 810)
                        {
                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.ChangeName,
                                wParam1 = client.Player.X,
                                wParam2 = client.Player.Y,

                            };
                            client.Send(stream.ActionCreate(&action));
                            client.Send(stream.NameChangeCreate(MsgNameChange.ActionID.DialogInfo, client.Player.NameEditCount, 5, client.Player.Name));
                        }
                        else
                        {
                            data.AddText("At your service! Oh, no... I`m sorry. You`d better check your bag, again. You don`t have enough CPs to rename yourself. It takes 810 CPs, and I`m not allowed to offer any discounts.")
                      .AddOption("Rules~are~rules.~I`ll~wait.", 255)
                      .AddAvatar(31).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        //96321,Epic's,1,54176,1002,317,283,0,0,1,1
        [NpcAttribute(NpcID.Revenant1)]
        public static void Revenant1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                data.AddText("Thank you very much! I can rest in peace now!")
                           .AddOption("Close~your~eyes,~soldier!", 255)
                           .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("Charge! I will kill you all!")
                           .AddOption("Oh~MY!", 255)
                           .AddAvatar(30).FinalizeDialog();



                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Revenant2)]
        public static void Revenant2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                                data.AddText("Thank you very much! I can rest in peace now!")
                           .AddOption("Close~your~eyes,~soldier!", 255)
                           .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("Charge! I will kill you all!")
                           .AddOption("Oh~MY!", 255)
                           .AddAvatar(30).FinalizeDialog();


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Revenant3)]
        public static void Revenant3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                                data.AddText("Thank you very much! I can rest in peace now!")
                           .AddOption("Close~your~eyes,~soldier!", 255)
                           .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("Charge! I will kill you all!")
                           .AddOption("Oh~MY!", 255)
                           .AddAvatar(30).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Revenant4)]
        public static void Revenant4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 1);
                                data.AddText("Thank you very much! I can rest in peace now!")
                           .AddOption("Close~your~eyes,~soldier!", 255)
                           .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("Charge! I will kill you all!")
                           .AddOption("Oh~MY!", 255)
                           .AddAvatar(30).FinalizeDialog();


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Revenant5)]
        public static void Revenant5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 0, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 1);
                                data.AddText("Thank you very much! I can rest in peace now!")
                           .AddOption("Close~your~eyes,~soldier!", 255)
                           .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("Charge! I will kill you all!")
                           .AddOption("Oh~MY!", 255)
                           .AddAvatar(30).FinalizeDialog();


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CrystalRiddler)]
        public static void CrystalRiddler(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DisCityMain, client.Player.Class, 1313);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1) == false)
                            {
                                if (!client.Inventory.Remove(711354, 1, stream)) break;
                                client.Inventory.Add(stream, 711355);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest2.MissionId, 1);
                                client.SendSysMesage("The Divine Calix was filled with the fountain water.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneApe)]
        public static void StoneApe(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneApe, client.Player.Class, 1312);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DisCityMain, client.Player.Class, 1313);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneApe, client.Player.Class, 1314);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I come from the stone, and so I am the Stone Ape. The other apes regard me as a freak, but I`m not! Please, go and ask the Solar Saint what I am!")
                               .AddText("~I must know what I truly am, or I fear I shall go bananas!");
                            data.AddOption("Okay!", 1);
                            data.AddOption("Sorry,~not~today!", 255);
                            data.AddAvatar(53).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I was born from the rocks used to restore cracks in the sky? I can`t believe my ears! And what is this? A Divine Calix? Solar Saint told you that I can")
                              .AddText("~become an immortal after I drink the fountain water in the calix? That`s great news! Thank you very much for your help!");
                            data.AddOption("You~are~welcome!", 2);
                            data.AddOption("See~you~then!", 255);
                            data.AddAvatar(53).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 3))
                            {
                                data.AddText("Thank you! I feel much more powerful now! I will work hard to become a worthy immortal!")
                              .AddOption("You~will!", 10)
                              .AddOption("See~you~then!", 255)
                              .AddAvatar(53).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Where is the Divine Calix? I want to drink the water inside!")
                                 .AddOption("I~will~grab~it~back~for~you!", 255)
                                 .AddAvatar(53).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Go and ask the Solar Saint what I am!")
                           .AddOption("Okay!", 255)
                           .AddAvatar(53).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        data.AddText("Go and ask the Solar Saint what I am!")
                                 .AddOption("Okay!", 255)
                                 .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (HasItems(client, 711355, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(711355, 1, stream)) break;
                            client.SendSysMesage("You received 485,000 points of EXP, 1,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 485000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.SendSysMesage("You received 728,000 points of EXP, 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 728000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, Database.ItemType.DoubleExp);
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.JackDaniel)]
        public static void JackDaniel(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CarpenterJack, client.Player.Class, 1315);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.JackDaniel, client.Player.Class, 1316);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("No problem! I will do everything I can to help Carpenter Jack fix the wooden bridge! Why not take a short break? Or better yet, I have something else for you to do!")
                          .AddOption("As~you~wish!", 1)
                          .AddOption("Sorry,~not~today!", 255)
                          .AddAvatar(53).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            goto case 2;
                        }
                        else
                        {
                            data.AddText("Please don`t stand and in the way! The transport column will pass through here!")
                       .AddOption("Sorry!", 255)
                       .AddAvatar(53).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.SendSysMesage("You received 495,000 points of EXP, 1000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 495000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        goto case 2;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Go slay 50 Giant Ape and protect the wood transport column.")
                         .AddOption("Okay!", 255)
                         .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        data.AddText("I can prepare the wood needed by the carpenter, but I`m worried about the Giant Apes. Slay 50 Giant Apes for me in the Love Canyon, in case they attack the wood transports.")
                          .AddOption("No~problem!", 3)
                          .AddOption("Sorry,~not~today!", 255)
                          .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CliffFlower1)]
        public static void CliffFlower1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CarpenterJack, client.Player.Class, 1317);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Player.ActivePick)
                        {
                            if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                            {
                                client.Player.AddPick(stream, "Collect~Cliff~Flower", 5);
                            }
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CliffFlower2)]
        public static void CliffFlower2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CarpenterJack, client.Player.Class, 1317);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Player.ActivePick)
                        {
                            if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                            {
                                client.Player.AddPick(stream, "Collect~Cliff~Flower", 5);
                            }
                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.KeYulunsFollower)]
        public static void KeYulunsFollower(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KeYulun, client.Player.Class, 1463);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                            {
                                data.AddText("The Sand Monsters L68 are too strong for me. Please go kill them and protect me!")
                    .AddOption("I`m~on~my~way.", 255)
                     .AddAvatar(53).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Help, help! It`s so good to see you here. The Sand Monsters L68 blocked my way home. Please, I don`t want to die here. Could you kill them for me?")
                    .AddOption("No~problem!", 1)
                    .AddOption("Monsters?~It`s~terrible.", 255)
                     .AddAvatar(53).FinalizeDialog();
                            }

                            break;
                        }

                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.DCViceGenera)]
        public static void DCViceGenera(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceGenera, client.Player.Class, 1452);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceGenera, client.Player.Class, 1453);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceGenera, client.Player.Class, 1454);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1455);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("There are many dangerous creatures in the desert. Once you step through here, I won`t be able to protect you anymore. Take care.");



                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hidden~Danger~1.", 1);

                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hidden~Danger~2.", 4);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hidden~Danger~3.", 10);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Hidden~Danger~4.", 20);
                        data.AddOption("Thanks.", 255);
                        data.AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(1, 1, 1, 1, 1))
                        {
                            data.AddText("The Sandbag is open! We did it!")
                                .AddOption("Yeah,~yeah!", 21)
                                 .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("You`re going to collect some water from the source in the city and spread the water over the city gates.")
                              .AddText("~After that, the Sand Monsters will be unable to go through the gates or enter the city.")
                                .AddOption("I~see.", 255)
                                 .AddAvatar(51).FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                  .AddOption("Let me check.", 255)
                                   .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 720939, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(720939, 1, stream)) break;
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.Inventory.Add(stream, 720173);
                            client.CreateBoxDialog("ou've~received~some~EXP,~3,000~Silver,~a~Small~Joy~Stone~and~a~+1~Stone~(B)!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.QuestGUI.SendAutoPatcher(1000, 559, 795, (ushort)NpcID.KeYulun);
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1, 1))
                            {

                                data.AddText("A lot of yellow sand? That`s weird. This information is important. Thanks for you help!")
                                    .AddOption("Okay.", 3)
                                    .AddAvatar(51).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Have you killed the Sand Monsters, yet? Go patrol outside the city gates. You`ll find plenty of them roaming around.")
                                    .AddOption("Okay.", 255)
                                    .AddAvatar(51).FinalizeDialog();

                            }

                            break;
                        }
                        data.AddText("Have you killed the Sand Monsters, yet? Go patrol outside the city gates. You`ll find plenty of them roaming around.")

                             .AddOption("I`ll~go~right~away.", 2)
                         .AddOption("Later.", 255)
                         .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Head~out~and~patrol~around~the~city~gates.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~1,500~Silver!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        goto case 4;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711338, 1))
                            {
                                data.AddText("The Sand Monsters want to use the Sandbags to attack the city? Thanks for the information. Here`s your reward!")
                   .AddOption("Thanks.", 6)
                   .AddAvatar(51).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Sand Monsters must have something to do with the weird yellow sand. I need you to turn to the Sand Monsters for some clues.")
                      .AddOption("Alright.", 255)
                      .AddAvatar(51).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Sand Monsters must have something to do with the weird yellow sand. I need you to return to the Sand Monsters to look for clues.")
                                   .AddOption("I~won`t~let~you~down.", 5)
                                   .AddOption("Later.", 255)
                                   .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 6:
                    {

                        if (HasItems(client, 711338, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711338, 1, stream)) break;
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

                            client.Inventory.Add(stream, ItemID);

                            client.CreateBoxDialog("You~received~some~EXP,~3,000~Silver~and~1~Refined~Gem!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        goto case 10;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("These Sandbags can`t be opened somehow. I`ve tried many ways, and even the a blade doesn`t work. Can you deliver it to Taoist Yu? Maybe he can figure that out.")

                                .AddOption("I`m~on~my~way.", 255)
                                .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        data.AddText("These Sandbags can`t be opened, no matter what I do. I`ve tried many ways, and even a blade won`t work. Can you take it to Taoist Yu? Maybe he can figure it out.")
                                   .AddOption("You~can~count~on~me.", 11)
                                   .AddOption("Later.", 255)
                                   .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~deliver~the~Sandbag~to~Taoist~Yu.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FreezeWar)]
        public static void FreezeWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Freeze War is a skill based team event. Hitting the enemy team will lock ")
                            .AddText(" them in a block of ice. Hitting frozen team players will unfreeze them.. When ")
                 .AddText("an entire team is frozen at the same time, they will lose the round. All")
                 .AddText(" players are rewarded with ConquerPoitens.")
                 .AddText("First team who won 5 rounds will win or the team with the most points after 10 minutes.")
                            .AddOption("Sign~me~up", 1)
                   .AddOption("I`ll~think~it~over.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {

                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Idle
                            && Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FreezeWar)
                        {
                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {
                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
.AddOption("I see. ", 255)
.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
.AddOption("I see. ", 255)
.FinalizeDialog();
                        }

                    }
                    break;

            }



        }


        [NpcAttribute(NpcID.SnakemanLeader)]
        public static void SnakemanLeader(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MoonMaiden, client.Player.Class, 1332);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SnakemanLeader, client.Player.Class, 1359);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Why you are here? Speak!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                            data.AddOption("Quest:~Snakeman~Leader(2).", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Race~Hatred(1).", 10);

                        data.AddOption("See~you~then!", 255);

                        data.AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                data.AddText("Good job! Take your reward!")
                          .AddOption("Thanks!", 12)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(10).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Kill 50 Heresy Snakemen for me! They are my enemies!")
                            .AddOption("Okay!", 255)
                            .AddAvatar(10).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("The Heresy Snakemen are enemies to my race! I want you to kill 50 Heresy Snakemen for me!")
                                   .AddOption("Okay!", 11)
                                   .AddOption("Sorry,~not~today!", 255)
                                   .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("Oh? A Catholicon? Give it to me!")
                                .AddOption("Eat~it!", 2)
                                .AddOption("See~you~then!", 255)
                                .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("You received 1,205,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1205000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                        client.SendSysMesage("The Snakeman Leader ate the Catlholicon and become very sock! Go report back to Moon Maiden!", MsgMessage.ChatMode.System);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CaptorCooke)]
        public static void CaptorCooke(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1344);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1336);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1339);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1340);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1342);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1345);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1346);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1347);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1450);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("No criminal can escape from my hands! Here`s a letter for the DC Vice Captain. I need you to head to the Desert and deliver this letter to him.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Back~Home(1).", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Back~Home(2).", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Back~Home(5).", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Pick~a~Fight(1).", 30);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Pick~a~Fight(3).", 40);


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Strife(1).", 50);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Strife(2).", 60);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Strife(3).", 70);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("You~can~count~on~me.", 90);

                        data.AddOption("See~you~then!", 255);

                        data.AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 90:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~deliver~this~letter~to~the~DC~Vice~Captain!", ActiveQuest9.FinishNpcId.Map, ActiveQuest9.FinishNpcId.X, ActiveQuest9.FinishNpcId.Y, ActiveQuest9.FinishNpcId.ID);

                        break;
                    }
                case 70:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest8.MissionId, 1, 1))
                        {
                            data.AddText("Thank you for all you have done. Your name will be remembered by us!")
                 .AddOption("You~are~welcome.", 72)
                 .AddOption("See~you~then!", 255)
                 .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("Listen, this maybe a little complicated. Take these two Corpse Bags and these two Smell Bags of the two snake races. Use them in the appointed place to entice the elders of the 2 races from their lair and kill them. After the elders are killed!")
                 .AddText(" ~The 2 races will be furious at each other! And we can just sit down to watch them fight.")
                            .AddOption("No~problem!", 71)
                      .AddOption("Sorry,~not~today", 255)
                      .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 72:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 720861, 1) && HasItems(client, 720862, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            if (!client.Inventory.Remove(720861, 1, stream)) break;
                            if (!client.Inventory.Remove(720862, 1, stream)) break;
                            if (client.Player.Class > 100)
                                client.Inventory.Add(stream, 121128, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else
                                client.Inventory.Add(stream, 120128, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.SendSysMesage("You received 1,863,000 EXP, an Elite Necklace and 3,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1863000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 71:
                    {

                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720861, 1))
                            client.Inventory.Add(stream, 720861);
                        if (!client.Inventory.Contain(720862, 1))
                            client.Inventory.Add(stream, 720862);
                        client.Player.QuestGUI.Accept(ActiveQuest8, 0);
                        client.SendSysMesage("You can used the Corpse Bag(Heresy Bnake) and the Small Bag(Heresy Snake). You enticed the Heresy Snakeman Eider out from its fair", MsgMessage.ChatMode.System);

                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest7.MissionId, 40, 40))
                        {
                            data.AddText("My scout reported that the two snake races are at war! Thank you for your help! Now we can deliver the final blow!")
               .AddOption("Tell~me~what!", 62)
               .AddOption("See~you~then!", 255)
               .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        data.AddText("Now, carry these 2 Smell Bags and slay 40 Snakemen and 40 Heresy Snakemen. The smell of the other race will be left on the dead body of the Snakemen and Heresy Snakemen. Let`s make their hatred greater!")
                        .AddOption("No~problem!", 61)
                        .AddOption("Sorry,~not~today", 255)
                        .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 62:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                        {
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.SendSysMesage("You received 1,739,000 EXP, and 2,700 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1739000);
                            client.Player.Money += 2700;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }


                        break;
                    }
                case 61:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 20, 20))
                            {
                                data.AddText("It is great to have you help me! Now, listen carefully to my plan!")
                     .AddOption("Okay!", 52)
                     .AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 20 Snakemen and 20 Heresy Snakemen and use these 2 Smell bags to collect their smell.")
                       .AddOption("See~you~then!", 255)
                       .AddAvatar(7).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Snakeman and the Heresy Snakeman are two major snake races in Ape City. I have a secret plan to stir up a fight between them! Take these 2 Smell Bags, and go slay 20 Snakemen and 20 Heresy Snakemen to collect their smell.")
                        .AddOption("No~problem!", 51)
                        .AddOption("Sorry,~not~today", 255)
                        .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 52:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.SendSysMesage("You received 1,553,000 EXP, 2,500 Silver, and 5 Panaceas.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1553000);
                            client.Player.Money += 2500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 51:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        data.AddText("Good job! This plan will not succeed without your help! Here are your rewards!")
                           .AddOption("Thank~you~very~much!", 41)
                           .AddOption("See~you~then!", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B).~You~can~use~Stones~to~compose~equipment~in~the~Wuxing~Oven~(Market~178,204).");
                            client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.SendSysMesage("You received 1,835,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1835000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 30:
                    {
                        data.AddText("The Snakeman and the Heresy Snakeman are the two major creatures, here. They usually stir up trouble for us, but I have a plan to pick up a fight between them. Go speak with Harmony Taoist to learn more about the plan!")
                           .AddOption("No~problem!", 31)
                           .AddOption("Sorry,~not~today", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
                        break;
                    }
                case 20:
                    {
                        data.AddText("Wonderful! With the help of the Cloud Riding, I can bring the criminal back to Ape City and have him punished by law safely! Here are your rewards!")
                          .AddOption("Thank~you!", 21)
                          .AddOption("Sorry,~not~today", 255)
                          .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Add(stream, 160118);
                            client.SendSysMesage("You received 475,000 EXP, a pair of Elite Shoes and 1500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 475000);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("I heard that there is a famous Harmony Taoist here. He must know a shortcut to the Ape City. Go ask him for me! You know, I need to stay here watch this criminal.")
                         .AddOption("No~problem!", 11)
                         .AddOption("Sorry,~not~today", 255)
                         .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                            {
                                data.AddText("It is all cliff around here. I need to return to the Ape City as soon as possible! What can I do?")
                         .AddOption("You~have~my~help!", 3)
                         .AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Can you find a shortcut for me? I need to stay here watch this criminal.")
                            .AddOption("Okay!", 255)
                            .AddAvatar(7).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I just captured a wanted criminal! But I am afraid that his friends will come to rescue him. I need to return to Ape City as fast as possible! Can you find a shortcut for me? I need to stay here and watch this ruffian!")
                             .AddOption("No~problem!", 2)
                             .AddOption("Sorry,~not~today", 255)
                             .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.SendSysMesage("You received 890,000 EXP, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 890000);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HeresySnakemanLeader)]
        public static void HeresySnakemanLeader(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1342);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeresySnakemanLeader, client.Player.Class, 1360);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You are not a normal person! I can tell it from your face!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Pick~a~Fight(3).", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Race~Hatread(2).", 10);
                        data.AddOption("Really?", 255);

                        data.AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 60))
                        {
                            data.AddText("Thank you very much for slaying my enemies! Here are your rewards!")
         .AddOption("Thanks!", 12)
      .AddOption("See~you~then!", 255)
           .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        data.AddText("The Snakemen are my enemy! I will wipe them all out! Slay 60 Snakemen for me!")
               .AddOption("No~problem!", 11)
            .AddOption("Sorry,~I~am~not~interested.", 255)
                 .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("You received 1,205,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1205000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {

                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher(1020, 211, 445, 8298);
                        break;
                    }
            }
        }
    }
}
