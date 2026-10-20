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
     
        [NpcAttribute(NpcID.BankCps)]
        public static void BankCps(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                       
                            dialog.AddText("Hello <tip color=0xfff05050>" + client.Player.Name + "</tip>.\n");
                            dialog.AddText("All You Have [" + client.ConquerPointsSave + "] ConquerPoints IN Bank \n ");
                            dialog.AddText("Please Choose - What Do You Want To Make It ?\n");
                            dialog.AddOption("Save ConquerPoints In My Bank", 1);
                            dialog.AddOption("Take ConquerPoints From My Bank", 5);
                            dialog.AddOption("Thinks", 255);
                        dialog.AddAvatar(183).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                       
                            dialog.AddText("Hello <tip color=0xfff05050>" + client.Player.Name + "</tip>.\n");
                            dialog.AddText("All You Have [" + client.ConquerPointsSave + "] ConquerPoints IN Bank \n ");
                            dialog.AddText("Please Choose - What Do You Want To Make It ?\n");
                            dialog.AddInput("Save ConquerPoints In My Bank", 10);
                            dialog.AddOption("GoodBye", 255);
                     
                        dialog.AddAvatar(183).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        uint input;
                        if (uint.TryParse(Input, out input))
                        {
                            if (input < 0 && input > 2000000000)
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
                                dialog.AddText("Are you sure you want to Save " + input + " CPs In Your Bank ?");
                                dialog.AddOption("Yes", 2);
                                dialog.AddOption("No thank you.", 255);
                                dialog.AddAvatar(48).FinalizeDialog();
                            }
                            else
                            {

                                dialog.AddText("You don't have that much CPs. Try again");
                                dialog.AddInput("Insert amount:", 1);
                                dialog.AddOption("No thank you.", 255);

                                dialog.AddAvatar(48).FinalizeDialog();
                            }
                        }
                        else
                        {

                            dialog.AddText("Try again");
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
                            client.ConquerPointsSave += client.NpcCpsInput;
                            client.NpcCpsInput = 0;
                        }
                        break;
                    }
                case 5:
                    {
                       
                            dialog.AddText("Hello <tip color=0xfff05050>" + client.Player.Name + "</tip>.\n");
                            dialog.AddText("All You Have [" + client.ConquerPointsSave + "] ConquerPoints IN Bank \n ");
                            dialog.AddText("Please Choose - What Do You Want To Make It ?\n");
                            dialog.AddInput("Take ConquerPoints From My Bank", 20);
                            dialog.AddOption("bye", 255);
                       
                        dialog.AddAvatar(183).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        uint input;
                        if (uint.TryParse(Input, out input))
                        {
                            if (input < 0 && input > 2000000000)
                            {

                                dialog.AddText("You need to input more or atleast 60 CPs..");
                                dialog.AddInput("Insert amount:", 1);
                                dialog.AddOption("No thank you.", 255);

                                dialog.AddAvatar(48).FinalizeDialog();
                                return;
                            }
                            if (client.ConquerPointsSave >= input)
                            {

                                client.NpcCpsInput = input;
                                dialog.AddText("Are you sure you want to Take " + input + " CPs From Your Bank ?");
                                dialog.AddOption("Yes", 3);
                                dialog.AddOption("No thank you.", 255);

                                dialog.AddAvatar(48).FinalizeDialog();
                            }
                            else
                            {


                                dialog.AddText("You don't have that much CPs. Try again");
                                dialog.AddInput("Insert amount:", 1);
                                dialog.AddOption("No thank you.", 255);

                                dialog.AddAvatar(48).FinalizeDialog();

                            }
                        }
                        else
                        {


                            dialog.AddText("You don't have that much CPs. Try again");
                            dialog.AddInput("Insert amount:", 1);
                            dialog.AddOption("No thank you.", 255);

                            dialog.AddAvatar(48).FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.ConquerPointsSave >= client.NpcCpsInput && client.NpcCpsInput != 0)
                        {
                            client.ConquerPointsSave -= client.NpcCpsInput;
                            client.Player.ConquerPoints += client.NpcCpsInput;
                            client.NpcCpsInput = 0;
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Huint)]
        public static void Huint(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Welcome IN Huint map You can get good rewords and cps ")
                       .AddOption(" Join me .", 2)
                       .AddOption("Just~passing~by.", 255).AddAvatar(110).FinalizeDialog();

                        break;
                    }
                case 2:
                    {

                        client.Teleport(325, 320, 3030);

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.MissConquer)]
        public static void MissConquer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        
                            data.AddText("Hello there! The MissConquer Pk will take place during 17:00-17:15  on Evryday,")
                             .AddOption("Sign~me~up", 1)
                              .AddOption("I will come later. ", 255)
                              .AddAvatar(63).FinalizeDialog();
                       

                        break;
                    }
                case 1:
                    {
                        if (client.Player.IsGirl())
                        {
                            if ((DateTime.Now.Hour == 17 && DateTime.Now.Minute < 15))
                            {
                                client.Teleport(50, 50, 8841);
                            }
                            else
                            {

                                data.AddText("Please sign up between 17:00 and 17:15 every Day.")
                                .AddOption("I see. ", 255)
                                .AddAvatar(63).FinalizeDialog();

                            }
                        }
                        else
                        {

                            data.AddText("Sorry you Not Gril.").AddOption("I see. ", 255)
                            .AddAvatar(63).FinalizeDialog();

                            data.AddAvatar(63).FinalizeDialog();

                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MRConquer)]
        public static void MRConquer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Hello there! The MRConquer Pk will take place during 17:00-17:15  on Evryday,")
                         .AddOption("Sign~me~up", 1)
                          .AddOption("I will come later. ", 255)
                          .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.IsBoy())
                        {
                            if ((DateTime.Now.Hour == 17 && DateTime.Now.Minute < 15))
                            {
                                client.Teleport(50, 50, 9800);
                            }
                            else
                            {

                                data.AddText("Please sign up between 17:00 and 17:15 every Day.")
                                .AddOption("I see. ", 255)
                                .AddAvatar(63).FinalizeDialog();

                            }
                        }
                        else
                        {

                            data.AddText("Sorry you Not boy.").AddOption("I see. ", 255)
                            .AddAvatar(63).FinalizeDialog();

                            data.AddAvatar(63).FinalizeDialog();

                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.ReallotPoints)]
        public static void ReallotPoints(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                       
                            data.AddText("Hello friend. I can help you set your Atributes points fast .. its hard work for you.")
                                .AddOption("Vitality Points.", 5)
                                .AddOption("Agility Points.", 6)
                                .AddOption("Strength Points.", 7)
                                .AddOption("Spirit Points.", 8)
                                .AddOption("I~will~reallot~my~points.", 9)
                                .AddOption("Just~passing~by.", 255)
                                .AddAvatar(6)
                                .FinalizeDialog();
                        
                      
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Level >= 70)
                        {
                            if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                            {
                                if (!client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream)) break;

                                client.Player.Agility = 0;
                                client.Player.Strength = 0;
                                client.Player.Vitality = 1;
                                client.Player.Spirit = 0;
                              
                                    client.CreateBoxDialog("You have successfully reloaded your attribute points.");

                                if (client.Player.Reborn == 0)
                                {
                                    client.Player.Atributes = 0;
                                    Database.DataCore.AtributeStatus.ResetStatsNonReborn(client.Player);
                                    if (Database.AtributesStatus.IsWater(client.Player.Class))
                                    {
                                        if (client.Player.Level > 110)
                                            client.Player.Atributes = (ushort)((client.Player.Level - 110) * 3 + client.Player.ExtraAtributes);
                                    }
                                    else
                                    {
                                        if (client.Player.Level > 120)
                                            client.Player.Atributes = (ushort)((client.Player.Level - 120) * 3 + client.Player.ExtraAtributes);
                                    }
                                }
                                else if (client.Player.Reborn == 1)
                                {
                                    client.Player.Atributes = (ushort)(Database.Server.RebornInfo.ExtraAtributePoints(client.Player.FirstRebornLevel, client.Player.FirstClass)
                                        + 52 + 3 * (client.Player.Level - 15) + client.Player.ExtraAtributes);
                                }
                                else
                                {
                                    if (client.Player.SecoundeRebornLevel == 0)
                                        client.Player.SecoundeRebornLevel = 130;
                                    client.Player.Atributes = (ushort)(Database.Server.RebornInfo.ExtraAtributePoints(client.Player.FirstRebornLevel, client.Player.FirstClass) +
                                        Database.Server.RebornInfo.ExtraAtributePoints(client.Player.SecoundeRebornLevel, client.Player.SecoundeClass) + 52 + 3 * (client.Player.Level - 15) + client.Player.ExtraAtributes);
                                }
                                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);

                            }
                            else
                            {
                             
                                    data.AddText("Sorry, you don't have one DragonBall.")
                       .AddOption("I~see.", 255)
                       .AddAvatar(67).FinalizeDialog();
                               
                            }
                        }
                        else
                        {
                          
                                data.AddText("You~cannot~reallot~your~ability~points~unless~you~are~reborn~and~above~level~70.")
                      .AddOption("I~see", 255)
                      .AddAvatar(67).FinalizeDialog();
                          
                        }


                        break;
                    }
                case 1:
                    {
                        ushort points;
                        if (ushort.TryParse(Input, out points))
                        {
                            if (client.Player.Atributes >= points)
                            {
                                client.Player.Vitality += points;
                                client.Player.Atributes -= points;

                                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);

                            }
                            else  client.CreateBoxDialog("You dont have: " + Input + " Atribute points."); 
                        }
                        break;
                    }
                case 2:
                    {
                        ushort points;
                        if (ushort.TryParse(Input, out points))
                        {
                            if (client.Player.Atributes >= points)
                            {
                                client.Player.Agility += points;
                                client.Player.Atributes -= points;

                                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);

                            }
                            else  client.CreateBoxDialog("You dont have: " + Input + " Atribute points.");
                        }
                        break;
                    }
                case 3:
                    {
                        ushort points;
                        if (ushort.TryParse(Input, out points))
                        {
                            if (client.Player.Atributes >= points)
                            {
                                client.Player.Strength += points;
                                client.Player.Atributes -= points;

                                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);

                            }
                            else  client.CreateBoxDialog("You dont have: " + Input + " Atribute points."); 
                        }
                        break;
                    }
                case 4:
                    {
                        ushort points;
                        if (ushort.TryParse(Input, out points))
                        {
                            if (client.Player.Atributes >= points)
                            {
                                client.Player.Spirit += points;
                                client.Player.Atributes -= points;

                                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);

                            }
                            else  client.CreateBoxDialog("You dont have: " + Input + " Atribute points."); 
                        }
                        break;
                    }
                case 5:
                    {
                        
                            data.AddText("Enter the Vitality points.").AddInput("Vitality Points.", 1).AddOption("Just~passing~by.", 255).AddAvatar(6).FinalizeDialog();
                       
                        break;
                    }
                case 6:
                    {
                       
                      data.AddText("Enter the Agility points.").AddInput("Agility Points.", 2).AddOption("Just~passing~by.", 255).AddAvatar(6).FinalizeDialog();
                      break;
                    }
                case 7:
                    {

                        data.AddText("Enter the Strength points.").AddInput("Strength Points.", 3).AddOption("Just~passing~by.", 255).AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        
                            data.AddText("Enter the Spirit points.").AddInput("Spirit Points.", 4).AddOption("Just~passing~by.", 255).AddAvatar(6).FinalizeDialog();
                       break;
                    }
            }
        }
        [NpcAttribute(NpcID.JiangMaker)]
        public static void JiangMaker(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.MyJiangHu == null)
                        {

                            data.AddText("Sorry But You Don't Have Jiang Yet.");
                            data.AddOption("I'll think about it!", 255);
                            data.FinalizeDialog();

                            return;
                        }

                        data.AddText("Hello I'M Jiang Seller.");
                        for (int i = 0; i < client.Player.MyJiangHu.ArrayStages.Length; i++)
                        {
                            var stage = client.Player.MyJiangHu.ArrayStages[i];
                            if (stage.Activate)
                                data.AddOption("Stage [" + (byte)(i + 1) + "]", (byte)(i + 1));

                        }
                        data.AddOption("I'll think about it!", 255);
                        data.AddAvatar(258).FinalizeDialog();

                        break;
                    }
                default:
                    {
                        int mStage = Option;
                        if (mStage >= 1 && mStage <= 9)
                        {
                            foreach (var item in Database.JianHuTable.CultivateStatus[(byte)mStage])
                            {
                                data.AddOption(((Role.Instance.JiangHu.Stage.AtributesType)item).ToString(), (byte)((item * 10) + mStage));
                            }
                            data.FinalizeDialog();
                        }
                        else
                        {
                            var typex = mStage / 10;
                            mStage = mStage % 10;
                            byte stageno = (byte)Math.Min(9, mStage);
                            byte level = 5;
                            if (client.Player.ConquerPoints < 10000)
                            {
                              
                                    client.CreateBoxDialog("Sorry you Not Have 10 k Cps .");
                               
                                return;
                            }
                            var type = (Role.Instance.JiangHu.Stage.AtributesType)Math.Min(15, typex);
                            if (client.Player.MyJiangHu.ArrayStages.Length >= stageno)
                            {
                                var stage = client.Player.MyJiangHu.ArrayStages[(stageno - 1)];
                                client.Player.ConquerPoints -= 10000;
                                for (byte i = 1; i < stage.ArrayStars.Length + 1; i++)
                                {
                                    client.Player.MyJiangHu.Buckupstar = new Role.Instance.JiangHu.BackUpStar();
                                    client.Player.MyJiangHu.Buckupstar.PositionStar = i;
                                    client.Player.MyJiangHu.Buckupstar.Stage = stageno;
                                    client.Player.MyJiangHu.Buckupstar.Star = new Role.Instance.JiangHu.Stage.Star(stageno);
                                    client.Player.MyJiangHu.Buckupstar.Star.Activate = true;
                                    client.Player.MyJiangHu.Buckupstar.Star.Level = level;
                                    client.Player.MyJiangHu.Buckupstar.Star.Typ = type;
                                    client.Player.MyJiangHu.Buckupstar.Star.UID = client.Player.MyJiangHu.ValueToRoll(client.Player.MyJiangHu.Buckupstar.Star.Typ, client.Player.MyJiangHu.Buckupstar.Star.Level);
                                    client.Send(stream.JiangHuUpdatesCreate(client.Player.MyJiangHu.FreeCourse, MsgJiangHuUpdates.Action.CreateStar, i, stageno,
                                        client.Player.MyJiangHu.Buckupstar.Star.UID, (byte)client.Player.MyJiangHu.FreeTimeToday,
                                        client.Player.MyJiangHu.RoundBuyPoints));
                                    client.Player.MyJiangHu.ApplayNewStar(client);
                                }
                                if (client.Player.MyJiangHu != null)
                                    client.Player.MyJiangHu.SendStatus(stream, client, client);
                            }

                        }
                        break;
                    }
            }
        }
       
        [NpcAttribute(NpcID.EliteGuildOfficer)]
        public static void EliteGuildOfficer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.EliteGuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            data.AddText("This~is~the~elite~guild~war~envoy.This~event~is~held~twice~a day~and you can gain a good amount of~CPs~income~every~day.~Only~GL~can~claim~this~prize.")
                                .AddText(" The event is held from 15:00 to 16:00.")
                                .AddOption("I~would~like~to~claim~it", 1)
                           .AddOption("Just~passing~by.", 255).AddAvatar(110).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("EliteGW is being held now. Afterwards the leader of winning guild can gain the prize.")
                                .AddOption("Let me in.", 2).AddOption("Ok,~I~see.", 255).AddAvatar(110).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Teleport(47, 131, 2071);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.EliteGuildWar.Winner.GuildID && client.Player.MyGuild != null && MsgTournaments.MsgSchedules.EliteGuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {

                            if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.LeaderSpouse) && MsgTournaments.MsgSchedules.EliteGuildWar.Winner.LeaderReward > 0
                                 && !MsgTournaments.MsgSchedules.EliteGuildWar.RewardLeader.Contains(client.Player.UID))
                            {

                                MsgTournaments.MsgSchedules.EliteGuildWar.RewardLeader.Add(client.Player.UID);
                                MsgTournaments.MsgSchedules.EliteGuildWar.Winner.LeaderReward -= 1;
                                client.Player.ConquerPoints += ServerRate.EliteGuildWar;
                                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.ConuqerSuperBlue, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("" + client.Player.Name + " , GuildLeader from " + client.Player.MyGuild.GuildName + " was rewarded with " + ServerRate.EliteGuildWar.ToString() + " ConquerPoints for the domination the MsgEliteGuildWar.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                            }

                            else
                            {
                                data.AddText("Just the GuildLeader/LeaderSpouse can claim the reward!")
                        .AddOption("Ok~thank~you.", 255).AddAvatar(110).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Your~guild~has~not~dominated~guild~area..")
                        .AddOption("Ok~thank~you.", 255).AddAvatar(110).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.UnionWar)]
        public static void UnionWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(36);
            switch (Option)
            {
                case 0:
                    {

                        dialog.AddText("Hi " + client.Player.Name + ", I'm the UnionWar manager .. How can i help? it start at 15:00 evreyday to 15:30 - at 02:00 evreyday to 02:30");
                            if (MsgSchedules.UnionWar.Proces == ProcesType.Alive)
                                dialog.AddOption("Enter war !", 1);

                            else
                                dialog.AddOption("Claim~reward.", 2);

                            dialog.AddOption("Okay.~I~see..", 255);
                            dialog.FinalizeDialog();
                  
                      
                        break;
                    }
                case 1:
                    {
                        if (MsgSchedules.UnionWar.Proces == ProcesType.Alive)
                        {
                            client.Teleport(135, 135, 20061);
                        }
                        else
                                dialog.AddText("The UnionWar is not started yet.").AddOption("Alright.").FinalizeDialog();
                        break;
                    }

                case 2:
                    {
                        if (MsgSchedules.UnionWar.Proces != ProcesType.Alive)
                        {
                            if (MsgSchedules.UnionWar.Winner != null && Role.Instance.Union.UnionPoll.ContainsKey(MsgSchedules.UnionWar.Winner.UnionID))
                            {
                                if (client.Player.UnionMemeber.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                                {
                                    if (client.Player.MyUnion.UID == MsgSchedules.UnionWar.Winner.UnionID)
                                    {
                                       
                                        client.Player.SubClass.AddStudyPoints(client, 10000, stream);
                                        client.Player.InnerPower.AddPotency(stream, client, 10000);
                                        client.Player.ConquerPoints += ServerRate.UnionWar;
                                        MsgSchedules.UnionWar.Winner = null;
                                        client.Player.AddFlag(MsgServer.MsgUpdate.Flags.ConuqerSuperUnderBlue, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);
                                        MsgSchedules.SendSysMesage(client.Player.Name + " has receved " + ServerRate.UnionWar + " CPs for winning UnionWar.", MsgMessage.ChatMode.Center);
                                       
                                    }
                                    else
                                        
                                            dialog.AddText("You dont have any rewrd to claim.").AddOption("Alright.").FinalizeDialog();
                                       
                                }
                               
                                    dialog.AddText("Only the Union leader can claim the reward.").AddOption("Alright.").FinalizeDialog();
                             
                            }
                          
                                dialog.AddText("There is no any rewards now..").AddOption("Alright.").FinalizeDialog();
                          
                        }
                   
                            dialog.AddText("Please come back when UnionWar is finished.").AddOption("Alright.").FinalizeDialog();
                    
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CouplesPK)]
        public static void CouplesPK(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello lovers! I'm the CouplePk manager,this event is held on Saturday. You'll have 5 minutes to sign up with your loved one in a team. You may sign up from at 16:00 to 16:30 and the event will take place at 14:50.")
                   .AddOption("Sign~me~up", 1)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {

                        if (!Game.MsgTournaments.MsgSchedules.CouplesPKWar.Join(client, stream))
                        {
                            data.AddText("The signup starts at 16:00 to 16:30 .")
        .AddOption("I see. ", 255)
        .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GuildWarScore)]
        //[NpcAttribute((NpcID)6525)]
        public static void GuildWarScore(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Hi, I'm the GuildWarScore .. How can i help you? it start at 18:00 ");
                        data.AddOption("Enter", 1);
                        data.AddOption("Reward.", 2);
                        data.AddOption("Just passing by..");
                        data.AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (MsgSchedules.GuildWarScore.Proces == ProcesType.Alive)
                        {

                            client.Teleport(51, 44, 8800);
                        }
                        else

                            data.AddText("The GuildWarScore is not started yet.").AddOption("Alright.").FinalizeDialog();

                        break;
                    }


                case 2:
                    {
                        if (MsgSchedules.GuildWarScore.Proces != ProcesType.Alive)
                        {
                            var topScore = MsgSchedules.GuildWarScore.GuildWarScor.Values.OrderByDescending(i => i.Score).FirstOrDefault();
                            if (topScore != null && Role.Instance.Guild.GuildPoll.ContainsKey(topScore.GuildID))
                            {
                                if (client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader)
                                {
                                    if (client.Player.MyGuild.Info.GuildID == topScore.GuildID)
                                    {

                                        client.Player.ConquerPoints += ServerRate.GuildScoreWar;
                                        client.Player.AddFlag(MsgServer.MsgUpdate.Flags.rygh_hglx, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);
                                        MsgSchedules.GuildWarScore.GuildWarScor.Clear();
                                        MsgSchedules.SendSysMesage(client.Player.Name + " has receved " + ServerRate.GuildScoreWar.ToString() + " CPs for winning GuildWarScore.", MsgMessage.ChatMode.Center);
                                    }
                                    else

                                        data.AddText("You dont have any rewrd to claim.").AddOption("Alright.").FinalizeDialog();

                                }
                                else
                                    data.AddText("Only the guild leader can claim the reward.").AddOption("Alright.").FinalizeDialog();
                            }
                        }
                        else
                            data.AddText("Please come back when GuildWarScore is finished.").AddOption("Alright.").FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ClassPoleWar)]
        public static void ClassPoleWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hi, I'm the leader of ClassPole im work evrday at 19:00 to 19:30");
                        data.AddOption("Enter", 1);
                        data.AddOption("ClassPole[Claim] .", 2);
                        data.AddOption("#Thnx .");
                        data.AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgArcherClass.Proces == ProcesType.Alive)
                        {

                            client.Teleport(180, 93, 2071);
                        }
                        else
                            client.SendSysMesage("ClassPole PK  has been finished .. Please come back later at  19:00 or 09:00 ");
                        break;
                    }
                case 2:
                    {
                        if (Game.MsgTournaments.MsgArcherClass.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgArcherClass.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgArcherClass.RoundOwner.UID == client.Player.UID)
                                {
                                    client.Player.ConquerPoints += ServerRate.ClassPolePK;
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopArcher, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);
                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from Class Pole Pk .. 4.000 CPs  .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgArcherClass.RoundOwner = null;
                                }
                            }
                        }

                        if (Game.MsgTournaments.MsgFireClass.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgFireClass.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgFireClass.RoundOwner.UID == client.Player.UID)
                                {
                                    client.Player.ConquerPoints += ServerRate.ClassPolePK;
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopFireTaoist, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from Class Pole Pk .. " + ServerRate.ClassPolePK.ToString() + " CPs  .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgFireClass.RoundOwner = null;
                                }
                            }
                        }
                        if (Game.MsgTournaments.MsgKungfuKing.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgKungfuKing.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgKungfuKing.RoundOwner.UID == client.Player.UID)
                                {
                                    client.Player.ConquerPoints += ServerRate.ClassPolePK;
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopDragonLee, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from Class Pole Pk .. " + ServerRate.ClassPolePK.ToString() + " CPs  .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgKungfuKing.RoundOwner = null;
                                }
                            }
                        }
                        if (Game.MsgTournaments.MsgWindWalker.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgWindWalker.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgWindWalker.RoundOwner.UID == client.Player.UID)
                                {
                                    client.Player.ConquerPoints += ServerRate.ClassPolePK;
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopWindWalker, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from Class Pole Pk .. " + ServerRate.ClassPolePK.ToString() + " CPs  .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgWindWalker.RoundOwner = null;
                                }
                            }
                        }
                        if (Game.MsgTournaments.MsgMonkClass.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgMonkClass.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgMonkClass.RoundOwner.UID == client.Player.UID)
                                {
                                    client.Player.ConquerPoints += ServerRate.ClassPolePK;
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopMonk, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from Class Pole Pk .. " + ServerRate.ClassPolePK.ToString() + " CPs  .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgMonkClass.RoundOwner = null;
                                }
                            }
                        }
                        if (Game.MsgTournaments.MsgNinjaClass.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgNinjaClass.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgNinjaClass.RoundOwner.UID == client.Player.UID)
                                {
                                    client.Player.ConquerPoints += ServerRate.ClassPolePK;
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopNinja, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from Class Pole Pk .. " + ServerRate.ClassPolePK.ToString() + " CPs  .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgNinjaClass.RoundOwner = null;
                                }
                            }
                        }

                        if (Game.MsgTournaments.MsgTrojanClass.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgTrojanClass.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgTrojanClass.RoundOwner.UID == client.Player.UID)
                                {
                                    client.Player.ConquerPoints += ServerRate.ClassPolePK;
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopTrojan, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from Class Pole Pk .. " + ServerRate.ClassPolePK.ToString() + " CPs .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgTrojanClass.RoundOwner = null;
                                }
                            }
                        }
                        if (Game.MsgTournaments.MsgWarriorClass.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgWarriorClass.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgWarriorClass.RoundOwner.UID == client.Player.UID)
                                {
                                    client.Player.ConquerPoints += ServerRate.ClassPolePK;
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopWarrior, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from Class Pole Pk .. " + ServerRate.ClassPolePK.ToString() + " CPs  .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgWarriorClass.RoundOwner = null;
                                }
                            }
                        }
                        if (Game.MsgTournaments.MsgWaterClass.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgWaterClass.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgWaterClass.RoundOwner.UID == client.Player.UID)
                                {
                                    client.Player.ConquerPoints += ServerRate.ClassPolePK;
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopWaterTaoist, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from Class Pole Pk .. " + ServerRate.ClassPolePK.ToString() + " CPs .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgWaterClass.RoundOwner = null;
                                }
                            }
                        }
                        if (Game.MsgTournaments.MsgPariteClass.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgPariteClass.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgPariteClass.RoundOwner.UID == client.Player.UID)
                                {
                                    client.Player.ConquerPoints += ServerRate.ClassPolePK;
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopPirate, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from Class Pole Pk .. " + ServerRate.ClassPolePK.ToString() + " CPs .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgPariteClass.RoundOwner = null;
                                }
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.NobilityWar)]
        public static void NobilityWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hi, I'm the leader of NobilityPoleWar start at 21:00. or 10:00.");
                        data.AddOption("Enter", 1);
                        data.AddOption("NobilityPoleWar[Claim] .", 2);
                        data.AddOption("#Thnx .");
                        data.AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgNobilityPole.Proces == ProcesType.Alive)
                        {

                            client.Teleport(50, 50, 8843);
                        }
                        else
                            client.SendSysMesage("NobilityPoleWar has been finished .. Please come back later at hour 21:00 or 10:00");
                        break;
                    }
                case 2:
                    {
                        if (Game.MsgTournaments.MsgNobilityPole.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgNobilityPole.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgNobilityPole.RoundOwner.UID == client.Player.UID)
                                {
                                    client.Player.ConquerPoints += ServerRate.NobilityPoleKing;
                                    //client.Player.AddFlag(MsgServer.MsgUpdate.Flags.King, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);


                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from NobilityPoleWar[King] .. " + ServerRate.NobilityPoleKing.ToString() + " CPs  .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgNobilityPole.RoundOwner = null;
                                }
                            }
                        }
                        if (Game.MsgTournaments.MsgNobilityPole1.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgNobilityPole1.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgNobilityPole1.RoundOwner.UID == client.Player.UID)
                                {
                                    //client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Prince, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                                    client.Player.ConquerPoints += ServerRate.NobilityPolePrince;

                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from NobilityPoleWar[Prince] .. " + ServerRate.NobilityPolePrince.ToString() + " CPs.", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgNobilityPole1.RoundOwner = null;
                                }
                            }
                        }
                        if (Game.MsgTournaments.MsgNobilityPole2.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgNobilityPole2.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgNobilityPole2.RoundOwner.UID == client.Player.UID)
                                {

                                    client.Player.ConquerPoints += ServerRate.NobilityPoleDuck;
                                    //client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Duck, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from NobilityPoleWar[Duke] .. " + ServerRate.NobilityPoleDuck.ToString() + " CPs .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgNobilityPole2.RoundOwner = null;
                                }
                            }
                        }
                        if (Game.MsgTournaments.MsgNobilityPole3.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgNobilityPole3.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgNobilityPole3.RoundOwner.UID == client.Player.UID)
                                {

                                    client.Player.ConquerPoints += ServerRate.NobilityPoleEril;
                                    ////client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Duck, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);
                                    MsgSchedules.SendSysMesage("Congratulations, " + client.Player.Name + " has claim his reward from NobilityPoleWar[Earl] .. " + ServerRate.NobilityPoleEril.ToString() + " CPs  .", MsgMessage.ChatMode.Center);
                                    Game.MsgTournaments.MsgNobilityPole3.RoundOwner = null;
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
