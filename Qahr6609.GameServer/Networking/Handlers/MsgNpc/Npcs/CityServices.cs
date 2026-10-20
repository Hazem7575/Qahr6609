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
        [NpcAttribute(NpcID.Barber)]
        public static void Barber(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Pharmacist, client.Player.Class, 815);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("I`m capable of doing more hairstyles now! Tell me the style you like!")
                           .AddOption("You~can~prettify~my~face?", 103)
                           .AddOption("I~want~to~change~hair~style!", 1)
                           .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            data.AddText("Of course I can. I can even help you change your appearance! Come to me if you need my service! I will give you a discount!")
                 .AddOption("Tell~me~more.", 101)
                   .AddOption("I~want~to~change~hair~style!.", 1).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 100;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Of course I can. I can even help you change your appearance! Come to me if you need my service! I will give you a discount!")
                .AddOption("Tell~me~more.", 105)
                  .AddOption("I~want~to~change~hair~style!.", 100)
                .AddAvatar(10).FinalizeDialog();


                        }
                        goto case 100;

                    }
                case 105:
                    {
                        data.AddText("Hm, I guess you will need to store your items in the Warehouse in the future! Pay a visit to the Warehouseman, you may need his help!")
                                    .AddOption("Thanks!.", 255)
                                    .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 103:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.IncreaseExperience(stream, 1400, Role.Flags.ExperienceEffect.angelwing);
                            data.AddText("Of course I can. I can even help you change your appearance! Come to me if you need my service! I will give you a discount!")
              .AddOption("Tell~me~more.", 101)
                .AddOption("I~want~to~change~hair~style!.", 100)
              .AddAvatar(10).FinalizeDialog();
                        }

                        break;
                    }
                case 101:
                    {
                        data.AddText("Hm, I guess you will need to store your items in the Warehouse in the future! Pay a visit to the Warehouseman, you may need his help!")
           .AddOption("Thanks!.", 102)
           .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 102:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The~Barber~asked~you~to~speak~with~the~Warehouseman~in~the~Twin~City!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);


                        break;
                    }
                case 100:
                    {
                        data.AddText("I`m capable of doing more hairstyles now! A New Dynasty Hairstyle is worth 10000 silvers. Previous hairstyles cost 500 silvers.")

                        .AddOption("New~Dynasty~Hairstyles.", 1)
                        .AddOption("New~styles.", 2)
                        .AddOption("Nostalgic~styles.", 3)
                        .AddOption("(Level~4+)~VIP~Hairstyles.", 6)
                        .AddOption("Just~passing~by.", 255)
                        .AddAvatar(111).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Which style would you like to change to?")

                      .AddOption("New~Dynasty~Hairstyle~01", 10)
                      .AddOption("New~Dynasty~Hairstyle~02", 11)
                      .AddOption("New~Dynasty~Hairstyle~03", 12)
                      .AddOption("New~Dynasty~Hairstyle~04", 13)
                      .AddOption("New~Dynasty~Hairstyle~05", 14)
                      .AddOption("Next~Page.", 4)
                      .AddAvatar(111).FinalizeDialog();



                        break;
                    }
                case 16:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 48);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 15:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 47);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 46);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 45);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 44);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 43);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 42);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                default:
                    {
                        if (client.Player.Hair < 300)
                            client.Player.Hair = 300;
                        ushort NewHaire = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + Option);
                        if (client.Player.Money >= 500)
                        {
                            client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                            client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                            client.Player.Money -= 500;
                            client.Player.Hair = NewHaire;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                            client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        }
                        else
                        {
                            data.AddText("You do not have 500 silvers with you.")
                  .AddOption("I~see.", 255)
                  .AddAvatar(111).FinalizeDialog();
                        }

                        break;
                    }
                case 17:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 49);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 21:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 50);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 22:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 51);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 56:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 70);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("Done! What do you think? Do you like it?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 6);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 57:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 71);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("Done! What do you think? Do you like it?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 6);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 70:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 72);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("Done! What do you think? Do you like it?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 6);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 71:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 73);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("Done! What do you think? Do you like it?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 6);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 72:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 74);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("Done! What do you think? Do you like it?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 6);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Player.VipLevel < 4)
                        {
                            data.AddText("Sorry, your VIP Level has to be at least Level 4 to purchase this hairstyle.");
                            data.AddOption("Alright.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You can try out the different hairstyles, until you find your favorite!")
                        .AddOption("(Level~4+)~VIP~Hairstyle~1.", 56)
                        .AddOption("(Level~4+)~VIP~Hairstyle~2.", 57)
                        .AddOption("(Level~4+)~VIP~Hairstyle~3.", 70)
                        .AddOption("(Level~4+)~VIP~Hairstyle~4.", 71)
                        .AddOption("(Level~4+)~VIP~Hairstyle~5.", 72)
                                //  .AddOption("Special~HairStyle06", 73)
                                // .AddOption("Special~HairStyle07", 74)
                       .AddOption("I~won`t~change~my~hairstyle.")
                        .AddAvatar(111).FinalizeDialog();

                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("Which style would you like to change to?")
                      .AddOption("New~Dynasty~Hairstyle~06", 15)
                      .AddOption("New~Dynasty~Hairstyle~07", 16)
                     .AddOption("New~Dynasty~Hairstyle~08", 17)
                     .AddOption("New~Dynasty~Hairstyle~09", 21)
                     .AddOption("New~Dynasty~Hairstyle~10", 22)
                     .AddOption("Previous.", 1)
                     .AddAvatar(111).FinalizeDialog();


                        break;
                    }
                case 30:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 30);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 31:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 33);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 32:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 34);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 33:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 35);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 34:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 36);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 35:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 37);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 36:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 38);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 37:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 39);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 38:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 40);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("Which~style~would~you~like~to~select~from?")
                                                .AddOption("New~HairStyle01", 30)
                                                .AddOption("New~HairStyle02", 31)
                                                .AddOption("New~HairStyle03", 32)
                                                .AddOption("New~HairStyle04", 33)
                                                .AddOption("New~HairStyle05", 34)
                                                .AddOption("New~HairStyle06", 35)
                                                .AddOption("New~HairStyle07", 36)
                                                .AddOption("Next~Page.", 5)
                                                .AddAvatar(111).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        data.AddText("Which~style~would~you~like~to~select~from?")
                         .AddOption("New~HairStyle08", 37)
                         .AddOption("New~HairStyle09", 38)
                            /*  .AddOption("New~HairStyle10", 39)
                              .AddOption("New~HairStyle11", 40)
                              .AddOption("New~HairStyle12", 41)
                              .AddOption("New~HairStyle13", 42)
                              .AddOption("New~HairStyle14", 43)*/
                         .AddOption("Previous.", 2)
                         .AddAvatar(111).FinalizeDialog();

                        break;
                    }
                case 44:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 10);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 45:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 11);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 46:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 12);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 47:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 13);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 48:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 14);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;

                    }
                case 49:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 15);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 50:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(111);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 0);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(111);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Which style would you like to change to?")
                                            .AddOption("Nostalgic01", 44)
                                            .AddOption("Nostalgic02", 45)
                                            .AddOption("Nostalgic03", 46)
                                            .AddOption("Nostalgic04", 47)
                                            .AddOption("Nostalgic05", 48)
                                            .AddOption("Nostalgic06", 49)
                                            .AddOption("Bald~head", 50)
                            //.AddOption("Nostalgic08", 51)
                                           .AddOption("I~chanded~my~mind.")
                                            .AddAvatar(111).FinalizeDialog();


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GuildGateKeeper)]
        public static void GuildGateKeeper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Are~you~going~to~leave~here?")
                       .AddOption("Yes.", 1)
                       .AddOption("No.~Wait~a~moment.", 255)
                       .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        client.Teleport(300, 278, 1002);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.GuildConductor)]
        public static void GuildConductor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("What~can~I~do~for~you? Guild War: every Saturday 20:00-23:00 (Flame Lit 21:00-23:00) - Capture the Flag: every Saturday 20:00-21:00")
                                                .AddOption("Enter~the~guild~area.", 1)

                                                .AddOption("Buy~statue.", 2)
                                                .AddOption("Claim~the~CTF~Reward.", 4)
                                                .AddOption("Sign up Capture The Flag.", 5)
                                                .AddOption("Just~passing~by.", 255)
                                                .AddAvatar(123).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            if (client.Player.MyGuild != null && client.Player.MyGuild.CTF_Rank >= 1 && client.Player.MyGuild.CTF_Rank <= 9)
                            {
                                if (client.Player.MyGuildMember.CTF_Claimed == 1)
                                {
                                    data.AddText("Sorry, you already claimed the reward.")
            .AddOption("Ah~yes.", 255).AddAvatar(110).FinalizeDialog();
                                    break;
                                }
                                if (client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader)
                                {
                                    if (client.Player.MyGuild.ClaimCtfReward == 1)
                                    {
                                        data.AddText("Sorry, you already claimed the reward.")
        .AddOption("Ah~yes.", 255).AddAvatar(110).FinalizeDialog();
                                        break;
                                    }
                                    client.Player.MyGuild.ClaimCtfReward = 1;
                                    client.Player.ConquerPoints += client.Player.MyGuild.GetLeaderReward()[0];
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.ConuqerSuperBlue, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);
                                    client.Player.Money += client.Player.MyGuild.GetLeaderReward()[1];
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.Player.MyGuildMember.CTF_Claimed = 1;
                                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("" + client.Player.Name + " , GuildLider from " + client.Player.MyGuild.GuildName + " was rewarded "
                                        + client.Player.MyGuild.GetLeaderReward()[0].ToString() + " ConquerPoints for the domination the CaptureTheFlag rank No."
                                    + client.Player.MyGuild.CTF_Rank.ToString() + " .", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                }
                                else
                                {
                                    client.Player.ConquerPoints += client.Player.MyGuildMember.RewardConquerPoints;
                                    client.Player.Money += client.Player.MyGuildMember.RewardMoney;
                                    client.Player.MyGuildMember.CTF_Claimed = 1;

                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                }
                            }
                            else
                            {
                                data.AddText("Sorry, your guild is not top 9.")
             .AddOption("Ah~yes.", 255).AddAvatar(110).FinalizeDialog();
                            }

                        }


                        break;
                    }
                case 6:
                    {
                        client.Teleport(232, 385, 3868);
                        break;
                    }
                case 7:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID && client.Player.MyGuild != null && MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {

                            if (client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.LeaderReward > 0
                                 && !MsgTournaments.MsgSchedules.SuperGuildWar.RewardLeader.Contains(client.Player.UID))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make one more space in your inventory.")
                  .AddOption("Let me check.", 255).AddAvatar(110).FinalizeDialog();
                                    break;
                                }

                                MsgTournaments.MsgSchedules.SuperGuildWar.RewardLeader.Add(client.Player.UID);

                                MsgTournaments.MsgSchedules.SuperGuildWar.Winner.LeaderReward -= 1;
                                client.Player.ConquerPoints += MsgTournaments.MsgSuperGuildWar.GuildWarScrore.ConquerPointsReward;

                                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopSuperGuildWar, Role.StatusFlagsBigVector32.PermanentFlag, false);
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("" + client.Player.Name + " , GuildLideru from " + client.Player.MyGuild.GuildName + " was rewarded with " + MsgTournaments.MsgSuperGuildWar.GuildWarScrore.ConquerPointsReward.ToString() + " for the domination the SuperGuildWar.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                            }
                            else if (client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.DeputiLeaderReward > 0
                                && !MsgTournaments.MsgSchedules.SuperGuildWar.RewardDeputiLeader.Contains(client.Player.UID))
                            {
                                MsgTournaments.MsgSchedules.SuperGuildWar.Winner.DeputiLeaderReward -= 1;
                                MsgTournaments.MsgSchedules.SuperGuildWar.RewardDeputiLeader.Add(client.Player.UID);
                                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopSuperGuildWar, Role.StatusFlagsBigVector32.PermanentFlag, false);
                            }
                            else
                            {
                                data.AddText("Just the GuildLeader or DeputiLeader can claim the reward!")
                        .AddOption("Ok~thank~you.", 255).AddAvatar(110).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Your~guild~has~not~dominated~super~guild~area.~Super~Guild~war~will~be~held~on~every~Tuesday,~lasting~2~days.")
                        .AddOption("Ok~thank~you.", 255).AddAvatar(110).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        if (client.Player.MyGuild == null)
                        {
                            data.AddText("Sorry you not have Guild.")
                    .AddAvatar(123).FinalizeDialog();
                            break;
                        }
                        if (!MsgTournaments.MsgSchedules.CaptureTheFlag.Join(client, stream))
                        {
                            data.AddText("Capture the Flag opens every Saturday, 20:00-21:00.")
                     .AddAvatar(123).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Alive)
                        {
                            byte RandTele = (byte)Program.GetRandom.Next(1, 14);
                            switch (RandTele)
                            {
                                case 1: client.Teleport(326, 374, 1038); break;
                                case 2: client.Teleport(299, 371, 1038); break;
                                case 3: client.Teleport(293, 352, 1038); break;
                                case 4: client.Teleport(262, 337, 1038); break;
                                case 5: client.Teleport(285, 324, 1038); break;
                                case 6: client.Teleport(315, 289, 1038); break;
                                case 7: client.Teleport(323, 268, 1038); break;
                                case 8: client.Teleport(351, 282, 1038); break;
                                case 9: client.Teleport(371, 305, 1038); break;
                                case 10: client.Teleport(370, 326, 1038); break;
                                case 11: client.Teleport(357, 335, 1038); break;
                                case 12: client.Teleport(333, 330, 1038); break;
                                default: client.Teleport(322, 321, 1038); break;
                            }
                        }
                        else
                        {
                            client.Teleport(349, 340, 1038);
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("A~statue~costs~50,000~silver.~Are~you~sure~that~you~want~to~buy?")
                        .AddOption("Yeah.", 3)
                        .AddOption("No,~thanks.", 255)
                        .AddAvatar(123).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.Money >= 50000)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("I~see.", 255)
                      .AddAvatar(123).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 720020);
                            client.Player.Money -= 50000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);

                        }
                        else
                        {
                            data.AddText("Sorry,~you~do~not~have~50000~silver.")
                       .AddOption("I~see.", 255)
                       .AddAvatar(123).FinalizeDialog();
                        }

                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.FurnitureStore)]
        public static void FurnitureStore(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome!~This~is~the~Twin~City~Furniture~Store.~Take~a~look~around.~I~have~the~best~furniture~in~Twin~City.")
                        .AddOption("Show~me~your~wares.", 1)
                        .AddOption("Not~my~concern.", 255)
                        .AddAvatar(188).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        client.Teleport(52, 70, 1511);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Tinter)]
        public static void Tinter(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome!~Please~wear~the~armors~first.~What~armors~do~you~want~me~to~dye?")
                        .AddOption("Cap", 1)
                        .AddOption("Coat", 2)
                        .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("There~are~seven~colors~for~you~to~choose.~You~can~try~all~the~colors~for~no~more~charges.~What~color~do~you~like~best?")
                       .AddOption("Color~1", 13)
                       .AddOption("Color~2", 14)
                       .AddOption("Color~3", 15)
                       .AddOption("Color~4", 16)
                       .AddOption("Color~5", 17)
                       .AddOption("Color~6", 18)
                       .AddOption("Color~7", 19)
                       .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                    {
                        MsgServer.MsgGameItem Coat;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Armor, out Coat))
                        {
                            Coat.Color = (Role.Flags.Color)(Option - 10);
                            Coat.Mode = Role.Flags.ItemMode.Update;
                            Coat.Send(client, stream);
                            client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("There~are~seven~colors~for~you~to~choose.~You~can~try~all~the~colors~for~no~more~charges.~What~color~do~you~like~best?")
                        .AddOption("Color~1", 3)
                        .AddOption("Color~2", 4)
                        .AddOption("Color~3", 5)
                        .AddOption("Color~4", 6)
                        .AddOption("Color~5", 7)
                        .AddOption("Color~6", 8)
                        .AddOption("Color~7", 9)
                        .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    {
                        MsgServer.MsgGameItem Cap;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Head, out Cap))
                        {
                            Cap.Color = (Role.Flags.Color)Option;
                            Cap.Mode = Role.Flags.ItemMode.Update;
                            Cap.Send(client, stream);
                            client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Shopboy)]
        public static void Shopboy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Our~shop~is~famous~for~dyeing.~If~you~want~to~have~your~equipment~dyed,~please~wear~them~before~you~enter.~You")
                            .AddText("have~a~wide~choice~of~colors.~One~meteor~will~be~charged~before~you~try~the~colors.~Do~you~want~a~try?")
                        .AddOption("Yes,~here~is~a~meteor.", 1)
                        .AddOption("Can~you~dye~my~armor~black?", 2)
                        .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(Database.ItemType.Meteor, 1))
                        {
                            if (!client.Inventory.Remove(Database.ItemType.Meteor, 1, stream)) break;
                            client.Teleport(22, 26, 1008);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~do~not~have~a~meteor.")
                      .AddOption("Oh.~I~am~sorry.", 255)
                      .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure.~Once~your~armor~is~dyed~black,~it~will~never~fade~no~matter~how~it~is~updated,~unless~I~dye~it~to~other")
                         .AddText("colors.~I~will~charge~1~dragonball~to~do~it.~")
                     .AddOption("Yes.~Here`s~a~Dragonball.", 3)
                     .AddOption("Let~me~think~it~over.", 255)
                     .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                        {
                            if (!client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream)) break;
                            MsgServer.MsgGameItem Armor;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Armor, out Armor))
                            {
                                Armor.Color = Role.Flags.Color.Black;
                                Armor.Mode = Role.Flags.ItemMode.Update;
                                Armor.Send(client, stream);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            }
                        }
                        else
                        {
                            data.AddText("Sorry,~you~do~not~have~a~DragonBall.")
                      .AddOption("Oh.~I~am~sorry.", 255)
                      .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ArenaNpc)]
        public static void ArenaNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The~arena~is~open.~Welcome~to~challenge~other~people.~The~admission~fee~is~only~50~silver.")
                            .AddText("If~you~PK~in~the~arena,~you~will~not~gain~or~lose~any~experience~or~items~equipped,~and~will~get~revived~at~the~place~you~die.")
                            .AddText("The~Kungfu~circle~is~very~dangerous,~I~suggest~you~PK~in~area.")
                        .AddOption("Enter~the~arena.", 1)
                        .AddOption("Just~passing~by.", 255)
                        .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money >= 50)
                        {
                            client.Player.Money -= 50;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.Teleport(51, 69, 1005);
                        }
                        else
                        {
                            data.AddText("You do not have 50 silvers with you.")
                      .AddOption("I~see.", 255)
                      .AddAvatar(7).FinalizeDialog();

                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SurgeonMiracle)]
        public static void SurgeonMiracle(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Do~you~want~to~have~your~size~changed~so~that~you~can~become~much~more~adorable?~Now~here~is~a~precious~chance~for~you.")
                       .AddOption("I~want~to~change~my~size.", 1)
                       .AddOption("I~don`t~want~to~change.", 255)
                       .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("If~you~pay~me~one~dragonball,~I~can~have~your~size~changed.~You~will~become~more~attractive~and~start~a~fresh~life.~")
                           .AddText("By~the~way,~to~avoid~some~unexpected~things,~make~sure~you~are~not~in~any~disguise~form.")
                     .AddOption("Here~is~a~dragonball.", 2)
                     .AddOption("I~have~no~dragonball.", 255)
                     .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                        {
                            switch (client.Player.Body)
                            {
                                case 1003: client.Player.Body = 1004; break;
                                case 1004: client.Player.Body = 1003; break;
                                case 2001: client.Player.Body = 2002; break;
                                case 2002: client.Player.Body = 2001; break;
                            }
                            client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                            data.AddText("You Size was change Successful !")
                   .AddOption("Thank~you~master.", 255)
                   .AddAvatar(67).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry,~there~is~no~dragonball~in~your~inventory.~I~can`t~have~your~size~changed.")
                    .AddOption("I~see.", 255)
                    .AddAvatar(67).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.MasterXuan)]
        public static void MasterXuan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello! May I help you?")
                                                 .AddOption("Yes!~Upgrade~my~sashes!", 2)
                                                 .AddOption("How~to~upgrade~my~sashes?", 1)
                                                 .AddOption("Nothing.", 255)
                                                 .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("You can pay 100 CPs to combine 2 Sashes (S) into a Sash (M), and pay 200 CPs to combine 2 Sashes (M) into a Sash (L).")
                                              .AddOption("I~see.", 255)
                                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("First of all, you should take all items out of the sashes. Otherwise, they will disappear after the upgrade. You will need to pay 100 CPs to combine 2 Sashes (S) into a Sash (M),")
                                                     .AddText("~or pay 200 CPs to combine 2 Sashes (M) into a Sash (L). Are you ready?")
                                                 .AddOption("Yes.~I~want~a~Sash(M)(100CPs).", 3)
                                                 .AddOption("Yes.~I~want~a~Sash(L)(200CPs).", 4)
                                                 .AddOption("Not~yet.", 255)
                                                 .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(1100003, 2) && client.Player.ConquerPoints >= 100)
                        {
                            List<MsgServer.MsgGameItem> Items;
                            if (client.Inventory.SearchItemByID(1100003, 2, out Items))
                            {
                                bool Allow = true;
                                foreach (var item in Items)
                                {
                                    if (item.Deposite.Count != 0)
                                    {
                                        Allow = false;

                                        break;
                                    }
                                }
                                if (Allow)
                                {
                                    foreach (var item in Items)
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                                    client.Inventory.Add(stream, 1100006, 1);
                                    client.Player.ConquerPoints -= 100;

                                    data.AddText("You Sash was update Successful.")
                                        .AddOption("Thnks~you.", 255)
                                        .AddAvatar(7).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Buddy, you don`t have 2 Sashes (S), or your sashes are not empty. Remember, 2 Sashes (S) can be composed into a Sash (M).")
                                   .AddOption("Let~me~see.", 255)
                                   .AddAvatar(7).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Buddy, you don`t have 2 Sashes (S), or your sashes are not empty. Remember, 2 Sashes (S) can be composed into a Sash (M).")
                                     .AddOption("Let~me~see.", 255)
                                     .AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Buddy, you don`t have 2 Sashes (S), or your sashes are not empty. Remember, 2 Sashes (S) can be composed into a Sash (M).")
                                     .AddOption("Let~me~see.", 255)
                                     .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(1100006, 2) && client.Player.ConquerPoints >= 200)
                        {
                            List<MsgServer.MsgGameItem> Items;
                            if (client.Inventory.SearchItemByID(1100006, 2, out Items))
                            {
                                bool Allow = true;
                                foreach (var item in Items)
                                {
                                    if (item.Deposite.Count != 0)
                                    {
                                        Allow = false;

                                        break;
                                    }
                                }
                                if (Allow)
                                {
                                    foreach (var item in Items)
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                                    client.Inventory.Add(stream, 1100009, 1);
                                    client.Player.ConquerPoints -= 200;

                                    data.AddText("You Sash was update Successful.")
                                        .AddOption("Thnks~you.", 255)
                                        .AddAvatar(7).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Buddy, you don`t have 2 Sashes (M), or your sashes are not empty. Remember, 2 Sashes (M) can be composed into a Sash (L).")
                                     .AddOption("Let~me~see.", 255)
                                     .AddAvatar(7).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Buddy, you don`t have 2 Sashes (M), or your sashes are not empty. Remember, 2 Sashes (M) can be composed into a Sash (L).")
                                     .AddOption("Let~me~see.", 255)
                                     .AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Buddy, you don`t have 2 Sashes (M), or your sashes are not empty. Remember, 2 Sashes (M) can be composed into a Sash (L).")
                                     .AddOption("Let~me~see.", 255)
                                     .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }

            }

        }
        [NpcAttribute(NpcID.MCaptain)]
        public static void MCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                if (!client.Inventory.Remove(750000, 1, stream)) break;
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[1001].Where(p => p.Level + 5 > client.Player.Level).ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Database.Server.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BCaptain)]
        public static void BCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                if (!client.Inventory.Remove(750000, 1, stream)) break;
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[client.Player.Map].Where(p => p.Level + 5 > client.Player.Level).ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Database.Server.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DCCaptain)]
        public static void DCCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                if (!client.Inventory.Remove(750000, 1, stream)) break;
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[client.Player.Map].Where(p => p.Level + 5 > client.Player.Level).ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Database.Server.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AMCaptain)]
        public static void AMCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                if (!client.Inventory.Remove(750000, 1, stream)) break;
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();



                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[client.Player.Map].Where(p => p.Level + 5 > client.Player.Level).ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Database.Server.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PCCaptain)]
        public static void PCCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                if (!client.Inventory.Remove(750000, 1, stream)) break;
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[client.Player.Map].Where(p => p.Level + 5 > client.Player.Level).ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Database.Server.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();

                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TCCaptain)]
        public static void TCCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BlackSmith, client.Player.Class, 823);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 824);
            Database.QuestInfo.DBQuest ActiveQuest3 = null;
            if (Database.AtributesStatus.IsLee(client.Player.Class))
                ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BlackSmith, client.Player.Class, 6426);
            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BlackSmith, client.Player.Class, 3795);
            else
                ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class, 0, p => p.MissionId != 824 && p.FinishNpcId.Map == 1004);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 230;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);

                            data.AddText("Good day, my friend, I`m the TC Captain, I`m in charge of the public security around")
                                .AddText(" here. It is indeed an important job, isn`t it? Well, ever talking about contributing")
                                .AddText(" a bit to the public? You'll be rewarded.")
                           .AddOption("What can i do for you?", 240)
                           .AddOption("I want to take a Daily Quest.", 230)
                           .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                                client.GainExpBall(200);
                                data.AddText("Good job! That is what i want! You returned peace to Twin City!")
         .AddOption("Thank you!", 231)
         .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            data.AddText("Defeat 10 Pheasants outside Twin City. You are doing for this for people of Twin City!")
            .AddOption("Okay!", 255)
            .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 231;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            goto case 240;
                        }

                        goto case 230;

                    }
                case 240:
                    {
                        data.AddText("I have some jobs for you to do here! The Pheasants outside Twin City are becoming more")
                                                        .AddText(" and more aggressive! I want you to defeat 10 of them. This is for the safety of the people")
                                                        .AddText(" in Twin City.")
                                                   .AddOption("Okay, right on my way.", 232)
                                                   .AddOption("I want to take a Daily Quest.", 230)
                                                   .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 233:
                    {
                        data.AddText("Good job! That is what i want! You returned peace to Twin City!")
                            .AddOption("Thank you!", 233)
                            .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 231:
                    {
                        data.AddText("There is a long way to go! You need some professional guidance to grow stronger! Find the NPC of your class")
                                                           .AddText(" in the Job center to learn more about job promotion!")
                                                      .AddOption("Job promotion 1.", 234)
                                                      .AddOption("I want to take a Daily Quest.", 230)
                                                      .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 234:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The TC Captain asked you to enter the Job Center and find the NPC of you class to get promoted.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
                case 232:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The TC Captain asked you to slay 10 Pheasants outside Twin City! Remember to wear your equipment first!", 1002, 341, 401, 0);

                        break;
                    }
                case 230:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                if (!client.Inventory.Remove(750000, 1, stream)) break;
                                client.IncreaseExperience(stream, 1400, Role.Flags.ExperienceEffect.angelwing);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[client.Player.Map].Where(p => p.Level + 5 > client.Player.Level).ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Database.Server.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();

                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CloudSaint)]
        public static void CloudSaint(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.DemonExterminator.FinishToday == 1)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                byte stage = client.DemonExterminator.GetDemonExterminatorStage(Jar);
                                if (stage == 3)
                                {
                                    data.AddText("You have finished killing all the monsters for today. Do you want to claim your prize?");
                                    data.AddOption("Yes.", 3);
                                    data.AddOption("Nevermind.", 255);
                                    data.AddAvatar(102).FinalizeDialog();
                                }
                                else if (stage != byte.MaxValue)
                                {
                                    data.AddText("You have killed the amount required for this stage. Do you want to advance?");
                                    data.AddOption("Yes.", 2);
                                    data.AddOption("Nevermind.", 255);
                                    data.AddAvatar(102).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters for your current stage. Are you giving up?");
                                data.AddOption("Yes, I want to give up.", 4);
                                data.AddOption("Nevermind.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                        }

                        else
                        {

                            if (client.DemonExterminator.FinishToday == 1)
                            {
                                data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You are too weak to take quests from me. Go and find the MC Captain.");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                                break;
                            }
                            data.AddText("Stay calm and listem, the Demons are taking over the control of ");
                            data.AddText("the world. My family is the only one that can make a contract with ");
                            data.AddText("the humans to purify the world from the devils. I can give you two ");
                            data.AddText("expballs as reward for doing it. But you can do it once one time per ");
                            data.AddText("day, what will you do?");
                            data.AddOption("Begin quest.", 1);
                            data.AddOption("Wana to buy That Quest", 5);
                            data.AddOption("Just passing by", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                client.DemonExterminator.HuntKills = 0;

                                byte stage = client.DemonExterminator.GetDemonExterminatorStage(Jar);
                                stage++;

                                client.DemonExterminator.CreateDemonExterminator(client, stream, Database.InfoDemonExterminators.StageKills[stage], Database.InfoDemonExterminators.QuestTyp[client.DemonExterminator.QuestTyp][stage]);
                                data.AddText(string.Concat("Now go and kill ", Database.InfoDemonExterminators.StageKills[stage], " ", Database.InfoDemonExterminators.QuestName[client.DemonExterminator.QuestTyp][stage], " then come back."));
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("You do not have the jar.");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.FinishToday = 1;
                            int questtype = Program.GetRandom.Next(0, Database.InfoDemonExterminators.QuestTyp.Length);
                            client.DemonExterminator.QuestTyp = (uint)questtype;
                            client.DemonExterminator.CreateDemonExterminator(client, stream, Database.InfoDemonExterminators.StageKills[0], Database.InfoDemonExterminators.QuestTyp[questtype][0]);
                            data.AddText("Thank you, brave soldier, for accepting this quest. Kill " + Database.InfoDemonExterminators.StageKills[0] + " " + Database.InfoDemonExterminators.QuestTyp[questtype][0] + " and come back.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {

                        if (!client.Inventory.Remove(750000, 1, stream)) break;
                        client.DemonExterminator.HuntKills = 0;
                        client.DemonExterminator.ItemUID = 0;
                        break;
                    }
                case 3:
                    {

                        if (!client.Inventory.Remove(750000, 1, stream)) break;
                        client.GainExpBall(600 * 5, true, Role.Flags.ExperienceEffect.angelwing);
                        client.DemonExterminator.HuntKills = 0;
                        client.DemonExterminator.ItemUID = 0;
                        break;
                    }
                case 5:
                    {
                        data.AddText("You need to buy that quest for 3.000 ConquerPoints?");
                        data.AddOption("Yes. I`m sure,", 6);
                        data.AddOption("Just passing by", 255);
                        data.AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (client.Player.ConquerPoints >= 3000)
                        {
                            if (!client.Inventory.Remove(750000, 1, stream)) break;
                            client.Player.ConquerPoints -= 3000;


                            client.GainExpBall(600 * 5, true, Role.Flags.ExperienceEffect.angelwing);
                            client.DemonExterminator.ItemUID = 0;
                            client.DemonExterminator.HuntKills = 0;
                            client.DemonExterminator.FinishToday = 1;
                        }
                        else
                        {
                            data.AddText("Sorry you not have 98 ConquerPoints");
                            data.AddOption("Ah. Ok", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Confiscator)]
        public static void Confiscator(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The red/black name can redeem their detained equipment within 7 days. The CPs they pay for their guilt will be rewarded to the")
                                                    .AddText("~who killed them. If they do not pay, the equipment detained will be given to the one who killed them as rewards.")
                                            .AddOption("I~want~to~redeem~my~equipment.", 1)
                                            .AddOption("I~want~to~claim~my~rewards.", 3)
                                            .AddOption("What?", 255)
                                            .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("You can pay some CPs to redeem your equipment.")
                                            .AddOption("I~want~my~equipment.", 2)
                                            .AddOption("Let~me~think.", 255)
                                            .AddAvatar(102).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (!client.Confiscator.IsEmptyReadem())
                        {

                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.DetainRedeem,
                                wParam1 = client.Player.X,
                                wParam2 = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(&action));

                        }
                        else
                        {
                            data.AddText("You do not have any equipment detained.")
                 .AddOption("Got~it.", 255)
                 .AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("The red/black name you killed can redeem equipment within 7 days. You will be rewarded the CPs that he/she pay for the guilt.")
                           .AddText("~If not, you will get the detained equipment as rewards.")
                  .AddOption("I~want~my~equipment.", 4)
                  .AddOption("Let~me~think.", 255)
                  .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Confiscator.IsEmptyClaim())
                        {
                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.DetainClaim,
                                wParam1 = client.Player.X,
                                wParam2 = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(&action));

                        }
                        else
                        {
                            data.AddText("Sorry, you do not have anything to claim now.")
                 .AddOption("I~see.", 255)
                 .AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.JailConfiscator)]
        public static void JailConfiscator(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The red/black name can redeem their detained equipment within 7 days. The CPs they pay for their guilt will be rewarded to the")
                            .AddText("~who killed them. If they do not pay, the equipment detained will be given to the one who killed them as rewards.")
                    .AddOption("I~want~to~redeem~my~equipment.", 1)
                    .AddOption("I~want~to~claim~my~rewards.", 3)
                    .AddOption("What?", 255)
                    .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("You can pay some CPs to redeem your equipment.")
                  .AddOption("I~want~my~equipment.", 2)
                  .AddOption("Let~me~think.", 255)
                  .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Confiscator.IsEmptyReadem())
                        {
                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.DetainRedeem,
                                wParam1 = client.Player.X,
                                wParam2 = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(&action));


                        }
                        else
                        {
                            data.AddText("You do not have any equipment detained.")
                 .AddOption("Got~it.", 255)
                 .AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("The red/black name you killed can redeem equipment within 7 days. You will be rewarded the CPs that he/she pay for the guilt.")
                            .AddText("~If not, you will get the detained equipment as rewards.")
                   .AddOption("I~want~my~equipment.", 4)
                   .AddOption("Let~me~think.", 255)
                   .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Confiscator.IsEmptyClaim())
                        {

                            ActionQuery action = new ActionQuery()
                            {
                                ObjId = client.Player.UID,
                                Type = ActionType.OpenDialog,
                                dwParam = MsgServer.DialogCommands.DetainClaim,
                                wParam1 = client.Player.X,
                                wParam2 = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(&action));

                        }
                        else
                        {
                            data.AddText("Sorry, you do not have anything to claim now.")
                 .AddOption("I~see.", 255)
                 .AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.GarmentShopKeeper)]
        public static void GarmentShopKeeper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = MsgServer.DialogCommands.GarmentShop, 
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ShopKeepers)]
        public static void ShopKeepers(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                            ActionQuery action = new ActionQuery()
                            {
                                ObjId = client.Player.UID,
                                dwParam = (uint)Role.Flags.NpcType.Shop,
                                Type = ActionType.OpenGuiNpc,
                                Timestamp = (int)client.ActiveNpc,
                                wParam1 = client.Player.X,
                                wParam2 = client.Player.Y,
                                dwParam3 = client.Player.Map
                            };
                            client.Send(stream.ActionCreate(&action));
                            break;
                        }
            }
        }
        [NpcAttribute(NpcID.ShopType)]
        public static void ShopType(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam = (uint)Role.Flags.NpcType.DialogAndGui,
                            Type = ActionType.OpenGuiNpc,
                            Timestamp = (int)client.ActiveNpc,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y,
                            dwParam3 = client.Player.Map
                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }
            }
        }
    }
}
