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
        [NpcAttribute(NpcID.TowerOfMysteryLayerChange)]
        public static void TowerOfMysteryLayerChange(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, 0, 6126);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can enter the Tower of Mystery " + Math.Min(9, (int)client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "F through this passage. If you`ve made up your mind")
                                  .AddText("~to challenge this floor, I can help. While if you want to directly complete the challenge of this floor and get the reward,")
                                  .AddText("~you need to enable `Sweep` function by donating CPs or Meteor Scroll. So, what do you say?");
                        data.AddOption("Challenge~Floor~" + Math.Min(9, (int)client.Player.JoinTowerOfMysteryLayer + 1) + ".", 1);
                        if (client.Player.TOMChallengeToday == 1)
                            data.AddOption("Enable~`Sweep`~function.", 2);
                        else
                            data.AddOption("Enable~`Sweep`~function.", 3);
                        data.AddOption("Leave~the~tower.", 6)
                        .AddOption("I~need~to~think~about~it.", 255)
                        .AddAvatar(23)
                      .FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("Are you sure you want to leave the Tower of Mystery, now?")
                          .AddOption("Yes.", 7)
                          .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                   .FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        client.Teleport(85, 75, 4020);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 1:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(23)
                     .FinalizeDialog();
                            break;
                        }
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            if ((client.Player.TowerOfMysteryChallengeFlag & 1u << (byte)Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer)) != 1u << (byte)Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer))
                            {
                                Game.MsgTournaments.MsgSchedules.TowerOfMystery.JoinLayer(client, 60, 60, (byte)Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer), stream);
                            }
                            else
                            {
                                switch (Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer))
                                {
                                    case 0:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 1`s guardian devil GreatPheasant(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 1:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 2`s guardian devil WingedLord(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 2:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 3`s guardian devil WaterTerror(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 3:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 4`s guardian devil RuthlessAsura(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 4:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 5`s guardian devil SoulStrangler(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 5:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 6`s guardian devil DarkGlutton(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 6:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 7`s guardian devil AlienDragon(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 7:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 8`s guardian devil ShadowSpider(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    default:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 9`s guardian devil FlameGiant(Elite).No need to sweep again.")
                                 .AddOption("Okay.", 255).AddAvatar(23)
                          .FinalizeDialog();
                                            break;
                                        }
                                }
                            }
                        }
                        else
                        {
                            if ((client.Player.TowerOfMysteryChallengeFlag & 1u << (byte)Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer)) != 1u << (byte)Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer))
                            {
                                Game.MsgTournaments.MsgSchedules.TowerOfMystery.JoinLayer(client, 60, 60, client.Player.JoinTowerOfMysteryLayer, stream);
                            }
                            else
                            {
                                switch (Math.Min(8, (uint)client.Player.JoinTowerOfMysteryLayer))
                                {
                                    case 0:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 1`s guardian devil GreatPheasant(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 1:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 2`s guardian devil WingedLord(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 2:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 3`s guardian devil WaterTerror(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 3:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 4`s guardian devil RuthlessAsura(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 4:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 5`s guardian devil SoulStrangler(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 5:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 6`s guardian devil DarkGlutton(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 6:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 7`s guardian devil AlienDragon(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 7:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 8`s guardian devil ShadowSpider(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    default:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 9`s guardian devil FlameGiant(common).No need to sweep again.")
                                 .AddOption("Okay.", 255).AddAvatar(23)
                          .FinalizeDialog();
                                            break;
                                        }
                                }
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.CanSweapTowerOfMystery)
                        {

                            data.AddText("                       Bright Tribe`s Reward\n")
                           .AddText("                    **************************************\n");
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                            data.AddText("                    **************************************\n");
                            data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                            if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                            {
                                data.AddOption("Refresh~reward.~(20~CP)", 70);
                                data.AddOption("Enable~`Sweep`.~(430~CPs)", 76);//todo
                                data.AddOption("Refresh~reward.~(50~Study~Pts)", 72);
                                data.AddOption("`Sweep`.~(2~DragonBalls)", 77);//todo
                                if (client.Inventory.Contain(3008735, 1))
                                    data.AddOption("Refresh~reward.~(FierceSpirit)", 74);
                            }
                            else
                            {
                                data.AddOption("Enable~`Sweep`.~(430~CPs)", 76);
                                data.AddOption("`Sweep`.~(2~DragonBalls)", 77);
                            }

                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(23)
                            .FinalizeDialog();
                        }
                        break;
                    }
                case 77:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(23)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(23)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 2) == false)
                        {
                            data.AddText("It seems you don`t have enough DragonBalls to directly clear this floor.Please check it again.")
                           .AddOption("Alright.", 255).AddAvatar(23)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            if (!client.Inventory.Remove(Database.ItemType.DragonBall, 2, stream)) break;
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008956);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.Add(stream, 3008957);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008958);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Inventory.Add(stream, 3008959);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Inventory.Add(stream, 3008960);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(23)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 1`s guardian devil GreatPheasant(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 2`s guardian devil WingedLord(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 3`s guardian devil WaterTerror(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 4`s guardian devil RuthlessAsura(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 5`s guardian devil SoulStrangler(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 6`s guardian devil DarkGlutton(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 7`s guardian devil AlienDragon(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 8`s guardian devil ShadowSpider(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 9`s guardian devil FlameGiant(common).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(23)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 76:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(23)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(23)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Player.ConquerPoints < 430)
                        {
                            data.AddText("It seems you don`t have enough ConquerPoints to `Sweep` the  reward. Please make sure you bring enough ConquerPoints, next time.")
                           .AddOption("Alright.", 255).AddAvatar(23)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            client.Player.ConquerPoints -= 430;
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008956);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.Add(stream, 3008957);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008958);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Inventory.Add(stream, 3008959);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Inventory.Add(stream, 3008960);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(23)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 1`s guardian devil GreatPheasant(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 2`s guardian devil WingedLord(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 3`s guardian devil WaterTerror(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 4`s guardian devil RuthlessAsura(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 5`s guardian devil SoulStrangler(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 6`s guardian devil DarkGlutton(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 7`s guardian devil AlienDragon(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 8`s guardian devil ShadowSpider(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 9`s guardian devil FlameGiant(common).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(23)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 74:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 75)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 75;
                        }
                        break;
                    }
                case 75:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (!client.Inventory.Remove(3008735, 1, stream)) break;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 3;
                        }
                        break;
                    }
                case 72:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 73)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 73;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 73:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 3;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 70:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 71)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 71;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 71:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 3;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.CanSweapTowerOfMystery)
                        {
                            data.AddText("                        Bright Tribe`s Reward         ")
                           .AddText("                     **************************************");
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                            data.AddText("                    **************************************");
                            data.AddText("                              Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                            if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                            {
                                data.AddOption("Refresh~reward.~(20~CP)", 170);
                                data.AddOption("Enable~`Sweep`.~(860~CPs)", 176);//todo
                                data.AddOption("Refresh~reward.~(50~Study~Pts)", 172);
                                data.AddOption("`Sweep`.~(4~DragonBalls)", 177);//todo
                                if (client.Inventory.Contain(3008735, 1))
                                    data.AddOption("Refresh~reward.~(FierceSpirit)", 174);
                            }
                            else
                            {
                                data.AddOption("Enable~`Sweep`.~(860~CPs)", 176);
                                data.AddOption("`Sweep`.~(4~DragonBalls)", 177);
                            }

                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(23)
                            .FinalizeDialog();

                        }
                        break;
                    }
                case 177:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(23)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(23)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 4) == false)
                        {
                            data.AddText("It seems you don`t have enough DragonBalls to directly clear this floor.Please check it again.")
                           .AddOption("Alright.", 255).AddAvatar(23)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            if (!client.Inventory.Remove(Database.ItemType.DragonBall, 4, stream)) break;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008963);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008964);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 2, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008965);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 3, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                        client.Inventory.Add(stream, 3008966);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 5, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.Inventory.Add(stream, 3008967);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(23)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 1`s guardian devil GreatPheasant(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 2`s guardian devil WingedLord(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 3`s guardian devil WaterTerror(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 4`s guardian devil RuthlessAsura(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 5`s guardian devil SoulStrangler(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 6`s guardian devil DarkGlutton(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 7`s guardian devil AlienDragon(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 8`s guardian devil ShadowSpider(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 9`s guardian devil FlameGiant(Elite).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(23)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 176:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(23)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(23)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Player.ConquerPoints < 860)
                        {
                            data.AddText("It seems you don`t have enough ConquerPoints to `Sweep` the  reward. Please make sure you bring enough ConquerPoints, next time.")
                           .AddOption("Alright.", 255).AddAvatar(23)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            client.Player.ConquerPoints -= 860;
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008963);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.Add(stream, 3008964);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008965);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                        client.Inventory.Add(stream, 3008966);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.Inventory.Add(stream, 3008967);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(23)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 1`s guardian devil GreatPheasant(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 2`s guardian devil WingedLord(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 3`s guardian devil WaterTerror(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 4`s guardian devil RuthlessAsura(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 5`s guardian devil SoulStrangler(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 6`s guardian devil DarkGlutton(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 7`s guardian devil AlienDragon(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 8`s guardian devil ShadowSpider(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 9`s guardian devil FlameGiant(Elite).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(23)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 174:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 175)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 175;
                        }
                        break;
                    }
                case 175:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (!client.Inventory.Remove(3008735, 1, stream)) break;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 2;
                        }
                        break;
                    }
                case 172:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 173)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 173;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 173:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 2;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 170:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 171)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 171;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 171:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 2;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TowerofMysteryChallenge4)]
        public static void TowerofMysteryChallenge4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, 0, 6126);
            Dialog data = new Dialog(client, stream);
            var Monster = Game.MsgTournaments.MsgSchedules.TowerOfMystery.GetMonster(client);
            if (client.Player.TOM_StartChallenge == true && client.Player.TOM_FinishChallenge == false)
                return;

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();



                        break;
                    }
                case 21:
                    {
                        client.Teleport(85, 75, 4020);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 20:
                    {
                        client.Player.JoinTowerOfMysteryLayer = (byte)Math.Min(client.Player.JoinTowerOfMysteryLayer + 1, 8);
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 22:
                    {
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 1:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) != 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.JoinTowerOfMysteryLayer;

                            if (client.Player.TOMChallengeToday == 1)
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008963);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008964);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 2, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008965);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 3, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                            client.Inventory.Add(stream, 3008966);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 5, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                            client.Inventory.Add(stream, 3008967);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008956);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.Add(stream, 3008957);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008958);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.Add(stream, 3008959);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.Add(stream, 3008960);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 1).AddAvatar(14)
                                .FinalizeDialog();
                        }
                        break;

                    }
                case 10:
                    {
                        client.SendSysMesage("The Seal on[" + Monster.Name + "] has been removed, and " + Monster.Name + " appeared at the center of this floor. Raise your weapon and knock down [" + Monster.Name + "]!", MsgMessage.ChatMode.System);

                        Database.Server.AddMapMonster(stream, client.Map, Monster.ID, client.Player.X, client.Player.Y, 8, 8, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        if (client.Player.JoinTowerOfMysteryLayer == 1)
                            Database.Server.AddMapMonster(stream, client.Map, (uint)(40932 + client.Player.TOMChallengeToday * 10), client.Player.X, client.Player.Y, 8, 8, 3, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                        client.Player.TOM_StartChallenge = true;

                        foreach (var npc in client.Player.View.Roles(Role.MapObjectType.Npc))
                        {
                            ActionQuery action;

                            action = new ActionQuery()
                            {
                                ObjId = npc.UID,
                                Type = ActionType.RemoveEntity
                            };
                            client.Player.View.SendView(stream.ActionCreate(&action), true);
                        }
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "new_gamebegin");
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 7)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 7;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (!client.Inventory.Remove(3008735, 1, stream)) break;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 5)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 5;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 3)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 3;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TowerofMysteryChallenge3)]
        public static void TowerofMysteryChallenge3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, 0, 6126);
            Dialog data = new Dialog(client, stream);
            var Monster = Game.MsgTournaments.MsgSchedules.TowerOfMystery.GetMonster(client);
            if (client.Player.TOM_StartChallenge == true && client.Player.TOM_FinishChallenge == false)
                return;

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();



                        break;
                    }
                case 21:
                    {
                        client.Teleport(85, 75, 4020);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 20:
                    {
                        client.Player.JoinTowerOfMysteryLayer = (byte)Math.Min(client.Player.JoinTowerOfMysteryLayer + 1, 8);
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 22:
                    {
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 1:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) != 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.JoinTowerOfMysteryLayer;

                            if (client.Player.TOMChallengeToday == 1)
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008963);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008964);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 2, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008965);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 3, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                            client.Inventory.Add(stream, 3008966);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 5, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                            client.Inventory.Add(stream, 3008967);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008956);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.Add(stream, 3008957);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008958);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.Add(stream, 3008959);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.Add(stream, 3008960);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 1).AddAvatar(14)
                                .FinalizeDialog();
                        }
                        break;

                    }
                case 10:
                    {
                        client.SendSysMesage("The Seal on[" + Monster.Name + "] has been removed, and " + Monster.Name + " appeared at the center of this floor. Raise your weapon and knock down [" + Monster.Name + "]!", MsgMessage.ChatMode.System);

                        Database.Server.AddMapMonster(stream, client.Map, Monster.ID, client.Player.X, client.Player.Y, 8, 8, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        if (client.Player.JoinTowerOfMysteryLayer == 1)
                            Database.Server.AddMapMonster(stream, client.Map, (uint)(40932 + client.Player.TOMChallengeToday * 10), client.Player.X, client.Player.Y, 8, 8, 3, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                        client.Player.TOM_StartChallenge = true;

                        foreach (var npc in client.Player.View.Roles(Role.MapObjectType.Npc))
                        {
                            ActionQuery action;

                            action = new ActionQuery()
                            {
                                ObjId = npc.UID,
                                Type = ActionType.RemoveEntity
                            };
                            client.Player.View.SendView(stream.ActionCreate(&action), true);
                        }
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "new_gamebegin");
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 7)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 7;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (!client.Inventory.Remove(3008735, 1, stream)) break;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 5)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 5;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 3)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 3;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TowerofMysteryChallenge2)]
        public static void TowerofMysteryChallenge2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, 0, 6126);
            Dialog data = new Dialog(client, stream);
            var Monster = Game.MsgTournaments.MsgSchedules.TowerOfMystery.GetMonster(client);
            if (client.Player.TOM_StartChallenge == true && client.Player.TOM_FinishChallenge == false)
                return;

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();



                        break;
                    }
                case 21:
                    {
                        client.Teleport(85, 75, 4020);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 20:
                    {
                        client.Player.JoinTowerOfMysteryLayer = (byte)Math.Min(client.Player.JoinTowerOfMysteryLayer + 1, 8);
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 22:
                    {
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 1:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) != 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.JoinTowerOfMysteryLayer;

                            if (client.Player.TOMChallengeToday == 1)
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008963);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008964);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 2, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008965);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 3, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                            client.Inventory.Add(stream, 3008966);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 5, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                            client.Inventory.Add(stream, 3008967);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008956);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.Add(stream, 3008957);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008958);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.Add(stream, 3008959);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.Add(stream, 3008960);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 1).AddAvatar(14)
                                .FinalizeDialog();
                        }
                        break;

                    }
                case 10:
                    {
                        client.SendSysMesage("The Seal on[" + Monster.Name + "] has been removed, and " + Monster.Name + " appeared at the center of this floor. Raise your weapon and knock down [" + Monster.Name + "]!", MsgMessage.ChatMode.System);

                        Database.Server.AddMapMonster(stream, client.Map, Monster.ID, client.Player.X, client.Player.Y, 8, 8, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        if (client.Player.JoinTowerOfMysteryLayer == 1)
                            Database.Server.AddMapMonster(stream, client.Map, (uint)(40932 + client.Player.TOMChallengeToday * 10), client.Player.X, client.Player.Y, 8, 8, 3, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                        client.Player.TOM_StartChallenge = true;

                        foreach (var npc in client.Player.View.Roles(Role.MapObjectType.Npc))
                        {
                            ActionQuery action;

                            action = new ActionQuery()
                            {
                                ObjId = npc.UID,
                                Type = ActionType.RemoveEntity
                            };
                            client.Player.View.SendView(stream.ActionCreate(&action), true);
                        }
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "new_gamebegin");
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 7)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 7;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (!client.Inventory.Remove(3008735, 1, stream)) break;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 5)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 5;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 3)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 3;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }

            }
        }
    }
}
