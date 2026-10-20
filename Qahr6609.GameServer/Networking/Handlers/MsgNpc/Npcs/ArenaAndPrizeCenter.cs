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
        [NpcAttribute(NpcID.ArenaManagerWang)]
        public static void ArenaManagerWang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        //action = 5003620
                        if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            data.AddText("Would you like to test your power and skill in the Power Arena? The challenge is under way. Jump in now!");
                            data.AddOption("Enter~Arena.", 30);
                            data.AddOption("What`s~are~the~rules?", 20);
                            data.AddOption("Exchange~for~refinery~packs.", 10);
                            data.AddOption("Not~interested!", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Want to test your power and skill? Power Arena has now provided you such a chance. Come to Arena Manager Wang between 13:00 and 14:00 or between 20:00 and 21:00 to accept the challenge!");
                            data.AddOption("I~will~be~there!", 255);
                            data.AddOption("Exchange~for~refinery~packs.", 10);
                            data.AddOption("Rules?", 20);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        //if (MsgTournaments.MsgSchedules.PowerArena.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (DateTime.Now > client.Player.JoinPowerArenaStamp.AddSeconds(600))
                            {
                                client.SendSysMesage("You have entered the Power Arena. Go defeat the guardians now and test your power!", MsgMessage.ChatMode.System);
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
                                client.Player.JoinPowerArenaStamp = DateTime.Now;
                            }
                            else
                            {
                                data.AddText("My hero! As you just left the Power Arena, you need to wait for 10 minutes to enter the Power Arena again!");
                                data.AddOption("Okay!", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }

                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("The Power Arena has 7 stages. The guardians in higher stages of the Power Arena will be more powerful. If you are knocked down in a stage, you will be sent to a lower stage to restart your challenge. However, if you are knocked down in stage 7, you");
                        data.AddText("~will be knocked out of the Power Arena. You have to wait for 5 minutes to enter the Arena again. You will find an Agate in each stage of the Arena. If you flattened enough guardians in that stage, you can click the Agate to claim your reward, or to");
                        data.AddText("~proceed to an advanced stage. Of course, you can choose to withdraw to a lower stage or to quit the Arena.");
                        data.AddOption("I~see!", 255);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Well, you can exchange Refinery Badges you get in the Arena for refinery packs here! There are 3 kinds of refinery packs: primary, medium");
                        data.AddText("~and superior refinery packs. The items in different kinds of packs are different. Obviously, the items in the superior refinery packs are of the best quality!");
                        data.AddText("~Now which kind of refinery pack do you want to exchange your Refinery Badges for? (The required number of Refinery Badges is marked behind the choice.)");
                        data.AddOption("Primary~refinery~pack.~(2)", 11);
                        data.AddOption("Medium~refinery~pack.~(6)", 14);
                        data.AddOption("Superior~refinery~pack.~(12)", 17);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        //9113120
                        data.AddText("You can exchange 12 Refinery Badges for a superior refinery pack here! Which kind of superior refinery pack do you want?");
                        data.AddOption("Veneer-Critical~Strike", 140);
                        data.AddOption("Bowstring-Critical~Strike", 141);
                        data.AddOption("Red~Tassel-Critical~Strike", 142);
                        data.AddOption("LongTassel-SkillCriticalStrike", 143);
                        data.AddOption("Plate-Immunity", 144);
                        data.AddOption("Visor-Intensification", 145);
                        data.AddOption("Cutter-Breakthrough", 146);
                        data.AddOption("Next~page.", 18);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        //9113140
                        data.AddText(" You can exchange 12 Refinery Badges for a superior refinery pack here! Which kind of superior refinery pack do you want?");
                        data.AddOption("Bow~Limb-Breakthrough", 147);
                        data.AddOption("Cauterant-Breakthrough", 148);
                        data.AddOption("Breastplate-Counteraction", 149);
                        data.AddOption("Tri~Plume-Detoxication", 150);
                        data.AddOption("Belt-Detoxication", 151);
                        data.AddOption("Hat~Rope-Block", 152);
                        data.AddOption("Back~page.", 17);
                        data.AddOption("Next~page.", 19);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 19:
                    {
                        //9113160
                        data.AddText("You can exchange 12 Refinery Badges for a superior refinery pack here! Which kind of superior refinery pack do you want?");
                        data.AddOption("Pin-Block", 153);
                        data.AddOption("Hemstitch-Penetration", 154);
                        data.AddOption("Bell-Penetration", 155);
                        data.AddOption("Back~page.", 18);
                        data.AddOption("I`ve~got~other~things~to~do.", 255);
                        data.AddAvatar(31);
                        data.FinalizeDialog();

                        break;
                    }
                case 155:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724132);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Bell Pack (Bracelet, Penetration)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 154:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724186);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Hemstitch Pack (Headgear, Penetration)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 153:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724171);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Pin Pack (Shield, Block)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 152:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724147);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Hat Rope Pack (Headgear, Block)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 151:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724177);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Belt Pack (Armor, Detoxication)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 150:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724174);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Tri Plume Pack (Headgear, Detoxication)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 149:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724144);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Breastplate Pack (Armor, Counteraction)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 148:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724165);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Cauterant Pack (2-handed, Breakthrough)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 147:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724168);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Bow Limb Pack (Bow, Breakthrough)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 146:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724162);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Cutter Pack (1-handed, Breakthrough)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 145:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724141);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Visor Pack (Headgear, Intensification)!", MsgMessage.ChatMode.System);

                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 144:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724138);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Plate Pack (Armor, Immunity)!", MsgMessage.ChatMode.System);

                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 143:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724135);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Long Tassel Pack (Backsword, Skill Critical Strike)!", MsgMessage.ChatMode.System);

                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 142:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724159);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Red Tassel Pack (2-handed, Critical Strike)!", MsgMessage.ChatMode.System);

                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 141:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724153);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Bowstring Pack (Bow, Critical Strike)!", MsgMessage.ChatMode.System);

                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 140:
                    {
                        if (client.Inventory.Contain(720148, 12))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 12, stream))
                                {
                                    client.Inventory.Add(stream, 724156);
                                    client.SendSysMesage("You exchanged 12 Refinery Badges for a Superior Veneer Pack (1-handed, Critical Strike)!", MsgMessage.ChatMode.System);

                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 12 Refinery Badges if you want to get a superior refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 14:
                    {
                        //9112120
                        data.AddText("6 Refinery Badges for a medium refinery pack! Which kind of medium refinery pack do you want?");
                        data.AddOption("Veneer-Critical~Strike", 120);
                        data.AddOption("Bowstring-Critical~Strike", 121);
                        data.AddOption("Red~Tassel-Critical~Strike", 122);
                        data.AddOption("LongTassel-SkillCriticalStrike", 123);
                        data.AddOption("Plate-Immunity", 124);
                        data.AddOption("Visor-Intensification", 125);
                        data.AddOption("Cutter-Breakthrough", 126);
                        data.AddOption("Next~page.", 15);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        //9112140
                        data.AddText("6 Refinery Badges for a medium refinery pack! Which kind of medium refinery pack do you want?");
                        data.AddOption("Bow~Limb-Breakthrough", 127);
                        data.AddOption("Cauterant-Breakthrough", 128);
                        data.AddOption("Breastplate-Counteraction", 129);
                        data.AddOption("Tri~Plume-Detoxication", 130);
                        data.AddOption("Belt-Detoxication", 131);
                        data.AddOption("Hat~Rope-Block", 132);
                        data.AddOption("Back~page.", 14);
                        data.AddOption("Next~page.", 16);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        //9112160
                        data.AddText("6 Refinery Badges for a medium refinery pack! Which kind of medium refinery pack do you want?");
                        data.AddOption("Pin-Block", 133);
                        data.AddOption("Hemstitch-Penetration", 134);
                        data.AddOption("Bell-Penetration", 135);
                        data.AddOption("Back~page.", 15);
                        data.AddOption("I`ve~got~other~things~to~do.", 255);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 135:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724131);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Bell Pack (Bracelet, Penetration)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 134:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724185);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Hemstitch Pack (Headgear, Penetration)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 133:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724170);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Pin Pack (Shield, Block)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 132:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724146);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Hat Rope Pack (Headgear, Block)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 131:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724176);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Belt Pack (Armor, Detoxication)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 130:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724173);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Tri Plume Pack (Headgear, Detoxication)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 129:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724143);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Breastplate Pack (Armor, Counteraction)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 128:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724164);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Cauterant Pack (2-handed, Breakthrough)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 127:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724167);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Bow Limb Pack (Bow, Breakthrough)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 126:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724161);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Cutter Pack (1-handed, Breakthrough)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 125:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724140);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Visor Pack (Headgear, Intensification)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 124:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724137);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Plate Pack (Armor, Immunity)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 123:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724134);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Long Tassel Pack (Backsword, Skill Critical Strike)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 122:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724158);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Red Tassel Pack (2-handed, Critical Strike)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 121:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724152);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Bowstring Pack (Bow, Critical Strike)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 120:
                    {
                        if (client.Inventory.Contain(720148, 6))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 6, stream))
                                {
                                    client.Inventory.Add(stream, 724155);
                                    client.SendSysMesage("You exchanged 6 Refinery Badges for a Medium Veneer Pack (1-handed, Critical Strike)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 6 Refinery Badges if you want to get a medium refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        //9111120
                        data.AddText("You can exchange just 2 Refinery Badges for a primary refinery pack! Which kind of primary refinery pack do you want?");
                        data.AddOption("Veneer-Critical~Strike", 102);
                        data.AddOption("Bowstring-Critical~Strike", 103);
                        data.AddOption("Red~Tassel-Critical~Strike", 104);
                        data.AddOption("LongTassel-SkillCriticalStrike", 105);
                        data.AddOption("Plate-Immunity", 106);
                        data.AddOption("Visor-Intensification", 107);
                        data.AddOption("Cutter-Breakthrough", 108);
                        data.AddOption("Next~page.", 12);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        data.AddText("You can exchange just 2 Refinery Badges for a primary refinery pack! Which kind of primary refinery pack do you want?");
                        data.AddOption("Bow~Limb-Breakthrough", 109);
                        data.AddOption("Cauterant-Breakthrough", 110);
                        data.AddOption("Breastplate-Counteraction", 111);
                        data.AddOption("Tri~Plume-Detoxication", 112);
                        data.AddOption("Belt-Detoxication", 113);
                        data.AddOption("Hat~Rope-Block", 114);
                        data.AddOption("Back~page.", 11);
                        data.AddOption("Next~page.", 13);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        //9111160
                        data.AddText("You can exchange just 2 Refinery Badges for a primary refinery pack! Which kind of primary refinery pack do you want?");
                        data.AddOption("Pin-Block", 115);
                        data.AddOption("Hemstitch-Penetration", 116);
                        data.AddOption("Bell-Penetration", 117);
                        data.AddOption("Back~page.", 12);
                        data.AddOption("I`ve~got~other~things~to~do.", 255);
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 117:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724130);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Bell Pack (Bracelet, Penetration)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 116:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724184);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Hemstitch Pack (Headgear, Penetration)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 115:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724169);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Pin Pack (Shield, Block)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 114:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724145);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Hat Rope Pack (Headgear, Block)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 113:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724175);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Belt Pack (Armor, Detoxication)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 112:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724172);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Tri Plume Pack (Headgear, Detoxication)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 111:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724142);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Breastplate Pack (Armor, Counteraction)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 110:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724163);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Cauterant Pack (2-handed, Breakthrough)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 109:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724166);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Bow Limb Pack (Bow, Breakthrough)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 108:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724160);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Cutter Pack (1-handed, Breakthrough)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 107:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724139);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Visor Pack (Headgear, Intensification)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 106:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724136);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Plate Pack (Armor, Immunity)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 105:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724133);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Long Tassel Pack (Backsword, Skill Critical Strike)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 104:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724157);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Red Tassel Pack (2-handed, Critical Strike)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 103:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724151);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Bowstring Pack (Bow, Critical Strike)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 102:
                    {
                        if (client.Inventory.Contain(720148, 2))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Remove(720148, 2, stream))
                                {
                                    client.Inventory.Add(stream, 724154);
                                    client.SendSysMesage("You exchanged 2 Refinery Badges for a Primary Veneer Pack (1-handed, Critical Strike)!", MsgMessage.ChatMode.System);
                                }
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Please make 1 cell before you claim the pack!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(31);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough Refinery Badges! You should prepare 2 Refinery Badges if you want to get a primary refinery pack!");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MammonEnvoy)]
        public static void MammonEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome~to~the~Prize~Center!~Did~you~bring~the~treasure~we~want?~So,~which~kind~of~coins~you~want~to~exchange,~the~Gold,~Silver~or~the~Copper?");
                        data.AddOption("Gold.~(Remain:" + MsgTournaments.MsgSchedules.SquidwardOctopus.GoldRemain + ")", 1);
                        data.AddOption("Silver.(Remain:" + MsgTournaments.MsgSchedules.SquidwardOctopus.SilverRemain + ")", 20);
                        data.AddOption("Copper.(Remain:" + MsgTournaments.MsgSchedules.SquidwardOctopus.CopperRemain + ")", 30);
                        data.AddOption("Send~me~to~Twin~City.", 40);
                        data.AddOption("I`ll~think~it~over.", 255);
                        data.AddAvatar(60);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Really?~You~got~a~Gold~Coin?~That`s~great!~So,~are~you~sure~you~want~to~exchange~now?~~You`ll~receive~a~Small~Lottery~Ticket~and~have~a~chance~to~get~a~rare~mount~armor.");
                        data.AddOption("Yeah,~please!", 2);
                        data.AddOption("Sorry,~wait!~Wait!", 255);
                        data.AddAvatar(60);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (MsgTournaments.MsgSchedules.SquidwardOctopus.GoldRemain > 0)
                        {

                            if (client.Inventory.Contain(711609, 1))
                            {
                                if (client.Inventory.HaveSpace(3))
                                {
                                    MsgTournaments.MsgSchedules.SquidwardOctopus.GoldRemain -= 1;
                                    if (!client.Inventory.Remove(711609, 1, stream)) break;
                                    client.Inventory.AddItemWitchStack(711504, 0, 1, stream);
                                    if (Role.Core.Rate(1, 3000))
                                    {
                                        if (Role.Core.Rate(1, 3))
                                        {
                                            client.Inventory.Add(stream, 200407);
                                            client.CreateBoxDialog("You~received~a~Jade~Hare!");
                                            Program.SendGlobalPackets.Enqueue(new MsgMessage("" + client.Player.Name + "~received~a~Jade~Hare~from~the~`Treasure~In~The~Blue`~event~by~swapping~the~Gold~Coin!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e290");
                                        }
                                        else
                                        {

                                            if (Role.Core.Rate(1, 2))
                                            {
                                                client.Inventory.Add(stream, 200420);
                                                client.Inventory.Add(stream, 780001);
                                                client.CreateBoxDialog("You~received~a~Celestial~Bird~and~1~VIP6~Token!");
                                                Program.SendGlobalPackets.Enqueue(new MsgMessage("" + client.Player.Name + "~received~a~Celestial~Bird~and~VIP6~Token~from~the~`Treasure~In~The~Blue`~event~by~swapping~the~Gold~Coin!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                                            }
                                            else
                                            {
                                                client.Inventory.Add(stream, 200413);
                                                client.CreateBoxDialog("You~received~a~Small~Lottery~Ticket~and~a~Green~Eyed~Beast!");
                                                Program.SendGlobalPackets.Enqueue(new MsgMessage("" + client.Player.Name + "~received~a~Green~Eyed~Beast~Bird~from~the~`Treasure~In~The~Blue`~event~by~swapping~the~Gold~Coin!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));

                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(5))
                                        {
                                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll);
                                            client.CreateBoxDialog("You~received~a~Small~Lottery~Ticket~and~a~DragonBallScroll!");
                                        }
                                        else if (Role.Core.Rate(10))
                                        {
                                            client.Inventory.Add(stream, Database.ItemType.DragonBall);
                                            client.CreateBoxDialog("You~received~a~Small~Lottery~Ticket~and~a~DragonBall!");
                                        }
                                        else
                                            client.CreateBoxDialog("You~received~a~Small~Lottery~Ticket!");
                                    }

                                    if (MsgTournaments.MsgSchedules.SquidwardOctopus.GoldRemain == 20)
                                        Program.SendGlobalPackets.Enqueue(new MsgMessage("Mammon~Envoy~only~have~20~pieces~of~rewards~left~for~the~Gold~Coins,~now.~First~come~first~served!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                    else if (MsgTournaments.MsgSchedules.SquidwardOctopus.GoldRemain == 10)
                                        Program.SendGlobalPackets.Enqueue(new MsgMessage("Mammon~Envoy~only~have~10~pieces~of~rewards~left~for~the~Gold~Coins,~now.~First~come~first~served!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                    else if (MsgTournaments.MsgSchedules.SquidwardOctopus.GoldRemain == 5)
                                        Program.SendGlobalPackets.Enqueue(new MsgMessage("Mammon~Envoy~only~have~5~pieces~of~rewards~left~for~the~Gold~Coins,~now.~First~come~first~served!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                    else if (MsgTournaments.MsgSchedules.SquidwardOctopus.GoldRemain <= 0)
                                        Program.SendGlobalPackets.Enqueue(new MsgMessage("~All~the~rewards~for~the~Gold~Coins~have~been~taken.~Hurry~to~exchange~your~other~kinds~of~coins!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));



                                }
                                else
                                {
                                    data.AddText("Your~inventory~is~full~now.~Come~back~when~you~have~some~room.~I`ll~be~here~waiting.");
                                    data.AddOption("I`ll~be~back~soon!", 255);
                                    data.AddAvatar(60);

                                }
                            }
                            else
                            {
                                data.AddText("Are~you~kidding?~You~don`t~have~the~Gold~Coin~we~want.~It~isn`t~a~right~place~to~play~jokes,~okay?");
                                data.AddOption("Sorry,~bye!", 255);
                                data.AddAvatar(60);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sigh...~Sorry,~but~you~are~too~late.~All~the~rewards~I~prepared~for~the~Gold~Coins~have~been~taken.~Come~to~exchange~the~Gold~Coin~earlier,~next~time.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(60);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("Are~you~sure~you~want~to~exchange~the~Silver~Coin,~now?~You`ll~receive~a~gift~randomly~picked~from~a~Tiny~Class2~Money~Pouch,~a~Tiny~Horse~Racing~Points~Bag,~3~Praying~Stone~");
                        data.AddText("~Scraps(S)~and~3~Special~EXP~Ball~Scraps.~Not~only~that~gift,~you~will~also~get~an~Endurance~Book.");
                        data.AddOption("Yeah,~please!", 21);
                        data.AddOption("No,~not~now!", 255);
                        data.AddAvatar(60);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (MsgTournaments.MsgSchedules.SquidwardOctopus.SilverRemain > 0)
                        {
                            if (client.Inventory.Contain(711610, 1))
                            {

                                if (client.Inventory.HaveSpace(2))
                                {
                                    if (!client.Inventory.Remove(711610, 1, stream)) break;
                                    client.Inventory.Add(stream, 723341);
                                    MsgTournaments.MsgSchedules.SquidwardOctopus.SilverRemain -= 1;
                                    if (Role.Core.Rate(1, 4))
                                    {
                                        client.Inventory.AddItemWitchStack(728897, 0, 3, stream);
                                        client.CreateBoxDialog("You~received~3~Special~EXP~Ball~Scraps!");
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e290");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 3))
                                        {
                                            client.Inventory.AddItemWitchStack(728896, 0, 3, stream);
                                            client.CreateBoxDialog("You~received~an~Endurance~Book~and~3~Praying~Stone~Scraps(S)!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 2))
                                            {
                                                client.Inventory.Add(stream, 728898);
                                                client.CreateBoxDialog("You~received~an~Endurance~Book~and~a~Tiny~Class2~Money~Pouch!");

                                            }
                                            else
                                            {
                                                client.Inventory.Add(stream, 728899);
                                                client.CreateBoxDialog("You~received~an~Endurance~Book~and~a~Tiny~Horse~Racing~Points~Bag!");

                                            }
                                        }
                                    }

                                    if (MsgTournaments.MsgSchedules.SquidwardOctopus.SilverRemain == 50)
                                        Program.SendGlobalPackets.Enqueue(new MsgMessage("Mammon~Envoy~only~have~50~pieces~of~rewards~left~for~the~Silver~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                    else if (MsgTournaments.MsgSchedules.SquidwardOctopus.SilverRemain == 30)
                                        Program.SendGlobalPackets.Enqueue(new MsgMessage("Mammon~Envoy~only~have~30~pieces~of~rewards~left~for~the~Silver~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                    else if (MsgTournaments.MsgSchedules.SquidwardOctopus.SilverRemain == 10)
                                        Program.SendGlobalPackets.Enqueue(new MsgMessage("Mammon~Envoy~only~have~10~pieces~of~rewards~left~for~the~Silver~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                    else if (MsgTournaments.MsgSchedules.SquidwardOctopus.SilverRemain <= 0)
                                        Program.SendGlobalPackets.Enqueue(new MsgMessage("Pay~attention!~All~the~rewards~for~the~Silver~Coins~have~been~taken.~Hurry~to~exchange~your~other~kinds~of~coins!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));

                                }
                                else
                                {
                                    data.AddText("Your~inventory~is~full~now.~Why~not~tidy~it~up~first,~then~come~back~for~the~reward?");
                                    data.AddOption("I`1l~be~back~soon!", 255);
                                    data.AddAvatar(60);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Sorry,~but~I~can`t~find~it~in~your~inventory.~Did~you~lose~the~Silver~Coin~somewhere?~");
                                data.AddOption("Oh,~no!", 255);
                                data.AddAvatar(60);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sigh...~Sorry,~but~you~are~too~late.~All~the~rewards~I~prepared~for~the~Silver~Coins~have~been~taken.~Come~to~exchange~the~Silver~Coins~earlier,~next~time.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(60);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("You~have~a~Copper~Coin,~right?~Let~me~see~what~you~can~get.~Oh,~here.~You`ll~receive~a~gift~randomly~picked~from~a~Meteor,~a~Tiny~Class1~Money~Pouch,~a~Praying~Stone~Scrap(S)~and~a");
                        data.AddText("~Special~EXP~Ball~Scrap.");
                        data.AddOption("Yeah,~please!", 31);
                        data.AddOption("Can~I~say~`NO`?", 255);
                        data.AddAvatar(60);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (MsgTournaments.MsgSchedules.SquidwardOctopus.CopperRemain > 0)
                        {
                            if (client.Inventory.Contain(711611, 1))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Your~inventory~is~full~now.~Why~not~tidy~it~up~first,~then~come~back~for~the~reward?");
                                    data.AddOption("I`1l~be~back~soon!", 255);
                                    data.AddAvatar(60);
                                    data.FinalizeDialog();
                                    break;
                                }
                                MsgTournaments.MsgSchedules.SquidwardOctopus.CopperRemain -= 1;
                                if (!client.Inventory.Remove(711611, 1, stream)) break;
                                if (Role.Core.Rate(1, 4))
                                {
                                    client.Inventory.AddItemWitchStack(728896, 0, 3, stream);
                                    client.CreateBoxDialog("You~received~a~Praying~Stone~Scrap(S)!");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e290");
                                }
                                else
                                {
                                    if (Role.Core.Rate(1, 3))
                                    {
                                        client.Inventory.AddItemWitchStack(728897, 0, 1, stream);
                                        client.CreateBoxDialog("You~received~a~Special~EXP~Ball~Scrap!");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 2))
                                        {
                                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                                            client.CreateBoxDialog("You~received~a~Meteor!");
                                        }
                                        else
                                        {
                                            client.Inventory.Add(stream, 728900);
                                            client.CreateBoxDialog("You~received~a~Tiny~Class1~Money~Pouch!");
                                        }
                                    }
                                }

                                if (MsgTournaments.MsgSchedules.SquidwardOctopus.CopperRemain == 100)
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage("Mammon~Envoy~only~have~100~pieces~of~rewards~left~for~the~Copper~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                else if (MsgTournaments.MsgSchedules.SquidwardOctopus.CopperRemain == 50)
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage("Mammon~Envoy~only~have~50~pieces~of~rewards~left~for~the~Copper~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                else if (MsgTournaments.MsgSchedules.SquidwardOctopus.CopperRemain == 10)
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage("Mammon~Envoy~only~have~10~pieces~of~rewards~left~for~the~Copper~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                else if (MsgTournaments.MsgSchedules.SquidwardOctopus.CopperRemain <= 0)
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage("Pay~attention!~All~the~rewards~for~the~Copper~Coins~have~been~taken.~Hurry~to~exchange~your~other~kinds~of~coins!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                            }
                            else
                            {
                                data.AddText("Stop~kidding~me,~okay?~You~don`t~have~any~Copper~Coins~with~you.~A~hero~wouldn`t~try~to~cheat~someone!");
                                data.AddOption("Sorry,~bye!", 255);
                                data.AddAvatar(60);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("Sigh...~Sorry,~but~you~are~too~late.~All~the~rewards~I~prepared~for~the~Copper~Coins~have~been~taken.~Come~to~exchange~the~Copper~Coins~earlier,~next~time.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(60);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 40:
                    {
                        client.Teleport(300, 280, 1002);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.SquidwardOctopus)]
        public static void SquidwardOctopus(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.SquidwardOctopus.Process == MsgTournaments.ProcesType.Alive)
                        {
                            data.AddText("Ladies~and~gentlemen,~the~`Treasure~In~The~Blue`~has~started!~No~matter~you~are~male~or~female,~old~or~young,~if~you~have~interested~in~treasure~hunting~or~pirates~and~have~reached,");
                            data.AddText("~Level~80,~sign~up~with~me~for~a~sea~adventure!~Rewards?~Of~course!");
                            data.AddOption("Send~me~to~the~Proud~Sea!", 30);
                            data.AddOption("Treasure~hunting?", 1);
                            data.AddOption("What~are~the~rewards?", 20);
                            data.AddOption("I`ll~think~it~over.", 255).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Hey! Have you ever heard the Proud Sea? Yeah, I mean the place where lots of pirates` ships sank and treasures lost. Well, our ship will be at sea at 12:30-13:29 and 20:30-21:29, Monday - Saturday.")
                                               .AddText("~Want to join us to hunt treasures? I promise that will be an unforgettable adventure. Moreover, you can win great rewards, if you find us the treasures. I await your decision!")
                                                          .AddOption("Treasure~hunting?", 1)
                                                          .AddOption("What~are~the~rewards?", 20)
                                                 .AddOption("I`ll~think~it~over.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("It~seems~you~have~made~up~the~mind.~Wait,~have~you~put~away~all~the~valuables?~You~know,~the~Proud~Sea~is~so~dangerous~that~you~wouldn`t~know~what~strong~enemies~you~may~meet.~So,~I~suggest~you~recheck~your~inventory.");
                        data.AddOption("Don`t~worry.~I`m~ready.", 31);
                        data.AddOption("I`ll~be~back~soon.", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (client.Player.Level < 80)
                        {
                            data.AddText("Um...~You~are~still~too~young.~Keep~practicing,~and~come~back~when~you~reach~Level~80.");
                            data.AddOption("Alright.", 255).FinalizeDialog();
                            break;
                        }
                        if (MsgTournaments.MsgSchedules.SquidwardOctopus.Process == MsgTournaments.ProcesType.Alive)
                        {
                            client.Activeness.IncreaseTask(10);// Treasure in the Blue, tier 0
                            client.Activeness.IncreaseTask(22);
                            client.Activeness.IncreaseTask(34);
                            byte rand = (byte)Program.GetRandom.Next(0, 4);
                            switch (rand)
                            {
                                case 0:
                                    client.Teleport(251, 392, 3071);
                                    break;
                                case 1:
                                    client.Teleport(72, 131, 3071);
                                    break;
                                case 2:
                                    client.Teleport(197, 64, 3071);
                                    break;
                                case 3:
                                    client.Teleport(348, 208, 3071);
                                    break;

                            }

                        }
                        else
                        {

                            data.AddText("Wow,~I~didn`t~expect~you~would~make~a~decision~so~fast,~but~the~ship~has~been~at~sea.~Why~not~come~earlier~next~time?~Remember~the~ship~will~put~to~sea~at~12:30-13:29~and~20:30-21:29,~Monday~-~Saturday.");
                            data.AddText("~Hope~to~see~you~next~time.");
                            data.AddOption("See~you.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 20:
                    {
                        data.AddText("Trust me, you will be more than satisfied with the rewards. I can take some examples. When you exchange the Gold Coin with Mammon Envoy in the Prize Center, you`ll receive a Small")
                      .AddText("~Lottery Ticket and have a chance to get a rare mount armor. For the Silver Coin, you can get an Endurance Book and a small gift. When you exchange the Copper Coin, you`ll receive a")
                      .AddText("~mysterious gift. Interested? You`d better be quick, since the rewards are limited.")
                            .AddOption("What~do~you~mean~limited?", 21).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        data.AddText("Eh... You know we are a small team with limited human resources, so we accept Gold Coins exchange 100 times at most; Silver Coins exchange 200 times at most and Copper Coins exchange ")
                      .AddText("~400 times at most. When all the rewards are taken or it is 1 hour after the event time, we won`t accept any exchange. Hope you get what you like!")
                            .AddOption("I~know,~time~is~money.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Our ship will send treasure hunters to the Proud Sea at 12:30-13:29 and 20:30-21:29, Monday - Saturday. If you have reached Level 80, I`d be glad to count you in. Join our `Treasure In The Blue` adventure!")
                          .AddOption("What?~`Treasure~In~The~Blue`?", 2).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Alas... That`s what I want, but you came too late and all the gifts have been claimed. So, come earlier in the next hour!")
                          .AddOption("What~are~the~treasures?", 3).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Stop kidding me! You don`t bring any Gold Coins with you. Do I look like a fool?")
                          .AddOption("Special~sea~monsters?", 4).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("Really? You have the Silver Coin? If you give it to me, you`ll receive an Endurance Book. Not enough? How about adding a Weapon Accessory Pack or a Mount Armor?")
                          .AddOption("~So, what do you say? Deal?", 5)
                          .AddOption("I`ll~think~it~over.", 255).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("Mammon Envoy is in the Prize Center and he is my partner. You can find any one of the 8 PC Teleporters in the Proud Sea to send you there.")
                        .AddText("~If you just returned from the Prize Center, you need to wait 3 minutes to go there again. Did I miss anything?")
                            .AddOption("Any~tips?", 6).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("Let me see... Yeah, I mentioned that you can get coins from other treasure hunters. Whereas, you may be the target of others, too. Though no PK point will be counted in the Proud Sea,")
                    .AddText("~the items in your inventory may drop, including the coins, if you were killed. Take care!")
                        .AddOption("Sure,~I~will.", 255).FinalizeDialog();
                        data.AddText("Let me see... Yeah, I mentioned that you can get coins from other treasure hunters. Whereas, you may be the target of others, too. Though no PK point will be counted in the Proud Sea,")
                             .AddText("~the items in your inventory may drop, including the coins, if you were killed. Take care!")
                                 .AddOption("Sure,~I~will.", 255).FinalizeDialog();

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PrizeCenterTeleporter1)]
        public static void PrizeCenterTeleporter1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
                        data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
                        data.AddOption("The~Prize~Center.", 1);
                        data.AddOption("Twin~City.", 2);
                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(14).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(300, 280, 1002);

                        break;
                    }
                case 1:
                    {
                        if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
                        {

                            client.Teleport(33, 33, 1068);
                            client.Player.JoinPrizeNpcOctopus = DateTime.Now;
                        }
                        else
                        {
                            data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(14).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PrizeCenterTeleporter2)]
        public static void PrizeCenterTeleporter2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
                        data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
                        data.AddOption("The~Prize~Center.", 1);
                        data.AddOption("Twin~City.", 2);
                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(14).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(300, 280, 1002);

                        break;
                    }
                case 1:
                    {
                        if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
                        {

                            client.Teleport(33, 33, 1068);
                            client.Player.JoinPrizeNpcOctopus = DateTime.Now;
                        }
                        else
                        {
                            data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(14).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PrizeCenterTeleporter3)]
        public static void PrizeCenterTeleporter3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
                        data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
                        data.AddOption("The~Prize~Center.", 1);
                        data.AddOption("Twin~City.", 2);
                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(14).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(300, 280, 1002);

                        break;
                    }
                case 1:
                    {
                        if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
                        {

                            client.Teleport(33, 33, 1068);
                            client.Player.JoinPrizeNpcOctopus = DateTime.Now;
                        }
                        else
                        {
                            data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(14).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PrizeCenterTeleporter4)]
        public static void PrizeCenterTeleporter4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
                        data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
                        data.AddOption("The~Prize~Center.", 1);
                        data.AddOption("Twin~City.", 2);
                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(14).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(300, 280, 1002);

                        break;
                    }
                case 1:
                    {
                        if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
                        {

                            client.Teleport(33, 33, 1068);
                            client.Player.JoinPrizeNpcOctopus = DateTime.Now;
                        }
                        else
                        {
                            data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(14).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PrizeCenterTeleporter5)]
        public static void PrizeCenterTeleporter5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
                        data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
                        data.AddOption("The~Prize~Center.", 1);
                        data.AddOption("Twin~City.", 2);
                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(14).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(300, 280, 1002);

                        break;
                    }
                case 1:
                    {
                        if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
                        {

                            client.Teleport(33, 33, 1068);
                            client.Player.JoinPrizeNpcOctopus = DateTime.Now;
                        }
                        else
                        {
                            data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(14).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PrizeCenterTeleporter6)]
        public static void PrizeCenterTeleporter6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
                        data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
                        data.AddOption("The~Prize~Center.", 1);
                        data.AddOption("Twin~City.", 2);
                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(14).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(300, 280, 1002);

                        break;
                    }
                case 1:
                    {
                        if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
                        {

                            client.Teleport(33, 33, 1068);
                            client.Player.JoinPrizeNpcOctopus = DateTime.Now;
                        }
                        else
                        {
                            data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(14).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PrizeCenterTeleporter7)]
        public static void PrizeCenterTeleporter7(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
                        data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
                        data.AddOption("The~Prize~Center.", 1);
                        data.AddOption("Twin~City.", 2);
                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(14).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(300, 280, 1002);

                        break;
                    }
                case 1:
                    {
                        if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
                        {

                            client.Teleport(33, 33, 1068);
                            client.Player.JoinPrizeNpcOctopus = DateTime.Now;
                        }
                        else
                        {
                            data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(14).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PrizeCenterTeleporter8)]
        public static void PrizeCenterTeleporter8(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
                        data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
                        data.AddOption("The~Prize~Center.", 1);
                        data.AddOption("Twin~City.", 2);
                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(14).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(300, 280, 1002);

                        break;
                    }
                case 1:
                    {
                        if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
                        {

                            client.Teleport(33, 33, 1068);
                            client.Player.JoinPrizeNpcOctopus = DateTime.Now;
                        }
                        else
                        {
                            data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(14).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CloudSweeper)]
        public static void CloudSweeper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Every day, when you team up with at least one friend, and conquer the Tower of Mystery at any challenge mode,")
                            .AddText("~you can claim a reward from me.")
                .AddOption("Claim~my~team~reward.", 1).AddAvatar(6)
         .FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        if (client.Player.TOMClaimTeamReward == 0)
                        {
                            if (client.Player.TowerOfMysterychallenge == 0)
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
                                        .AddOption("Let me check.", 255).AddAvatar(6)
                                        .FinalizeDialog();
                                    break;
                                }
                                client.Inventory.Add(stream, 3008733);
                                client.Player.TOMClaimTeamReward = 1;
                                data.AddText("Ah, you`ve completed the challenge nicely. Here is a reward for you.")
               .AddOption("Thanks!", 255).AddAvatar(6)
        .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("To claim a reward from me, you need to team up with your friends and conquere the Tower of Mystery.")
                 .AddOption("I~see.", 255).AddAvatar(6)
          .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Look, you`ve already claimed a reward from me. No more for today.")
             .AddOption("Sorry,~I~forgot.", 255).AddAvatar(6)
      .FinalizeDialog();
                        }
                        break;
                    }
            }


        }
    }
}
