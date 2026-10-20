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


        [NpcAttribute(NpcID.MarialDealer)]
        public static void MarialDealer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (client.Player.MyJiangHu == null)
            {
                data.AddText("Listen, you can`t acquire any Skill Souls if you have`t increased you Jiang Hu Score to Level 9, Keep practicing!")
             .AddOption("Alright.", 255).FinalizeDialog();
                return;
            }
            //if (client.Player.MyJiangHu.GetGrade() < 14)
            //{
            //    data.AddText("Listen, you can`t acquire any Skill Souls if you have`t increased you Jiang Hu Score to Level 9, Keep practicing!")
            //                        .AddOption("Alright.", 255).FinalizeDialog();
            //    return;
            //}


            switch (Option)
            {
                case 0:
                    {
                        data.AddText("what you want from me?")
                            .AddOption("UpdateSpells", 70)
                       .AddOption("Nothing.", 255).FinalizeDialog();

                        break;
                    }
                case 70:
                    {
                        data.AddText("Can you pay me to update some spell to soul? you need 25.000 Conquer Points.")
                        .AddOption("ViperFang.", 1)
                        .AddOption("FireofHell.", 2)
                        .AddOption("ToxicFog.", 3)
                       .AddOption("TwofoldBlades.", 4)
                       .AddOption("BlisteringWave.", 5)
                        .AddOption("Tornado.", 6)
                       .AddOption("WhirlwindKick.", 7)
                       .AddOption("Next Page.", 8).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        data.AddText("Can you pay me to update some spell to soul? you need 25.000 Conquer Points.")
                        .AddOption("BladeTempest.", 9)
                        .AddOption("DaggerStorm.", 10)
                        .AddOption("RadiantPalm.", 11)
                        .AddOption("ScentSword.", 12)
                       .AddOption("ChargingVortex.", 13)
                       .AddOption("DragonTail.", 14)
                        .AddOption("Back Page.", 70)
                       .AddOption("Next Page.", 16).FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        data.AddText("Can you pay me to update some spell to soul? you need 25.000 Conquer Points.")
                       .AddOption("Snow.", 15)
                       .AddOption("BloodyScythe.", 17)
                       .AddOption("FastBlade.", 18)
                       .AddOption("Phoenix.", 19)
                        .AddOption("GaleBomb.", 20)
                       .AddOption("Hercules.", 21)
                       .AddOption("SuperTwofoldBlads", 22)
                       .AddOption("Back Page.", 8).FinalizeDialog();


                        break;
                    }
                case 22:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.SuperTwofoldBlade;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 21:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.Hercules;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 20:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9)
                            GetMaxLevel = 1;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.GaleBomb;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 19:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.Phoenix;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 18:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.FastBlader;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 17:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.BloodyScythe;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 15:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.Snow;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 14:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.DragonTail;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 13:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.ChargingVortex;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade == 14)
                            GetMaxLevel = 2;
                        if (mygrade == 15)
                            GetMaxLevel = 3;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.ScrenSword;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 11:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 12)
                            GetMaxLevel = 1;
                        if (mygrade == 13)
                            GetMaxLevel = 2;
                        if (mygrade == 14) GetMaxLevel = 3;
                        if (mygrade == 15) GetMaxLevel = 4;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.RadiantPalm;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.DaggerStorm;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 9:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.BladeTempest;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 7:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.WhirlwindKick;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade == 14)
                            GetMaxLevel = 2;
                        if (mygrade == 15)
                            GetMaxLevel = 3;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;

                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.Tornado;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.BlisteringWave;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 4:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.TwofoldBlades;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.ToxicFog;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.ViperFang;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 1500;
                        else if (GetMaxLevel > 1)
                            cost = 3999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.FireofHell;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= cost;

                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                                   .AddOption("Alright.", 255).FinalizeDialog();
                        }


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.SarckSoldier)]
        public static void SarckSoldier(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Tell you what, there`s a monster called Nemesis Tyrant hiding at the bottom of the Bloodshed Sea! No one has come back alive after seeing it!")
                            .AddText("~It`s said that anyone who kills this goddamned monster will receive a Savage Bone, which can be used to explored the P7 Sacred Treasures!")
                            .AddOption("Send~me~to~the~Bloodshed~Sea!", 1)
                       .AddOption("Holy...So~scary!", 255).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        client.CreateBoxDialog("You~arrived~at~the~Bloodshed~Sea.");
                        client.Teleport(381, 271, 3846);


                        break;
                    }
            }
        }
        public static void InvokeSarckSoldier(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This is where the Nemesis Tyrant is hiding. I can`t recall the number of the people who came here. I just remember how they got killed. If you wanna")
                            .AddText("~talk about it, tell me any time.")
                            .AddOption("Tell~me~about~the~sea.", 1)
                       .AddOption("I~wanna~buy~some~Sea~Potions.", 2)
                       .AddOption("God!~Send~me~back!", 3)
                    .AddOption("Alright.", 255).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("This is where the Nemesis Tyrant is hiding. I can`t recall the number of the people who came here. I just remember how they got killed. If you wanna")
                           .AddText("~talk about it, tell me any time.")
                           .AddOption("Tell~me~about~the~sea.", 1)
                      .AddOption("I~wanna~buy~some~Sea~Potions.", 2)
                      .AddOption("God!~Send~me~back!", 3)
                   .AddOption("Alright.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Well, this sea is kinda special. You won`t get PK points and can`t use City Gates. The Nemesis Tyrant will show up at 15 and 45 of each hour. Look at you...")
                        .AddText("~Before it shows up, you`ll find some Gold Boxes! Open the boxes, you`ll receive some Sea Potions and learn the position of the monster in advance!")
                        .AddText("~Oh, you can also get the potions if you defeat the Nightmare Captain!")
                        .AddOption("Tell~me~something~else.", 4)
                   .AddOption("Got~you.", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("You~arrived~at~the~Gale~Shallow.");
                        client.Teleport(62, 89, 3055);

                        break;
                    }
                case 2:
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

        [NpcAttribute(NpcID.SarckSoldierEnctrance)]
        public static void SarckSoldierEnctrance(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        [NpcAttribute(NpcID.SarckSoldierSuppluPoint1)]
        public static void SarckSoldierSuppluPoint1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        [NpcAttribute(NpcID.SarckSoldierSuppluPoint2)]
        public static void SarckSoldierSuppluPoint2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        [NpcAttribute(NpcID.SarckSoldierSuppluPoint3)]
        public static void SarckSoldierSuppluPoint3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        [NpcAttribute(NpcID.SarckSoldierSuppluPoint4)]
        public static void SarckSoldierSuppluPoint4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        [NpcAttribute(NpcID.SarckSoldierSuppluPoint5)]
        public static void SarckSoldierSuppluPoint5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        [NpcAttribute(NpcID.SelectP7EquipmentSoulPack)]
        public static void SelectP7EquipmentSoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3004248, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a P7WeaponSoulPack.");
                return;
            }


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 40:
                    {
                        data.AddText("Select the p7 weapon you need.")
                            .AddOption("MoonHeadgear", 1)
                       .AddOption("SunHeadgear", 2)
                       .AddOption("StarHeadgear", 3)
                       .AddOption("IceHeadgear", 4)
                       .AddOption("HeavenNecklace", 5)
                       .AddOption("FervorBag", 6)
                       .AddOption("NetherArmor", 7)
                    .AddOption("Next~Page", 8).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        data.AddText("Select the p7 weapon you need.")
                            .AddOption("EclipseArmor", 9)
                       .AddOption("CraneRing", 10)
                       .AddOption("DragonRing", 11)
                       .AddOption("RainbowBracelet", 12)
                       .AddOption("LionHeavyRing", 13)
                       .AddOption("TigerHeavyRing", 14)
                       .AddOption("FoxBoots", 15)
                       .AddOption("DragonBoots", 16)
                       .AddOption("CraneBoots", 17)
                    .AddOption("Back~Page", 40).FinalizeDialog();



                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 820073, 1);
                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 820074, 1);
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 820075, 1);
                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 820076, 1);
                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 821033, 1);
                        break;
                    }
                case 6:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 821034, 1);
                        break;
                    }
                case 7:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 822071, 1);
                        break;
                    }
                case 9:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 822072, 1);
                        break;
                    }
                case 10:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 823058, 1);
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 823059, 1);
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 823060, 1);
                        break;
                    }
                case 13:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 823061, 1);
                        break;
                    }
                case 14:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 823062, 1);
                        break;
                    }
                case 15:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 824018, 1);
                        break;
                    }
                case 16:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 824019, 1);
                        break;
                    }
                case 17:
                    {
                        if (!client.Inventory.Remove(3004248, 1, stream)) break;
                        client.Inventory.Add(stream, 824020, 1);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SelectP7WeaponSoulPack)]
        public static void SelectP7WeaponSoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3004247, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a P7WeaponSoulPack.");
                return;
            }


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 20:
                    {
                        data.AddText("Select the p7 weapon you need.")
                            .AddOption("MonsterSaber", 1)
                       .AddOption("SkyHammer", 2)
                       .AddOption("ShadowKatana", 3)
                       .AddOption("SkyHalberd", 4)
                       .AddOption("DemonScythe", 5)
                       .AddOption("SpiritShield", 6)
                       .AddOption("TimeBacksword", 7)
                    .AddOption("Next~Page", 8).FinalizeDialog();


                        break;
                    }
                case 0:
                    {
                        data.AddText("Select the p7 weapon you need.")
                            .AddOption("MonsterSaber", 1)
                       .AddOption("SkyHammer", 2)
                       .AddOption("ShadowKatana", 3)
                       .AddOption("SkyHalberd", 4)
                       .AddOption("DemonScythe", 5)
                       .AddOption("SpiritShield", 6)
                       .AddOption("TimeBacksword", 7)
                    .AddOption("Next~Page", 8).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        data.AddText("Selecte the p7 weapon you need.")
                      .AddOption("SunBow", 9)
                 .AddOption("BuddaBeads", 10)
                 .AddOption("DeathPistol", 11)
                 .AddOption("RepentRapier", 12)
                 .AddOption("GhostKnife", 13)
                  .AddOption("WarCraze(Nunchaku)", 14)
                   .AddOption("WonderHossu", 15)
                  .AddOption("Back~Page", 20)
                  .AddOption("Next~Page", 21).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        data.AddText("Selecte the p7 weapon you need.")
                      .AddOption("FistofDeity(EpicWarrior)", 16)
                 .AddOption("FistofDemon(EpicWarrior)", 17)
                  .AddOption("DivineFan(WindWalker)", 18)
                  .AddOption("Back~Page", 8).FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 801308, 1);
                        break;
                    }
                case 16:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 801216, 1);
                        break;
                    }
                case 17:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 801218, 1);
                        break;
                    }

                case 14:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 801004, 1);
                        break;
                    }
                case 15:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 801104, 1);
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800020, 1);
                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800111, 1);
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800142, 1);
                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800215, 1);
                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800255, 1);
                        break;
                    }
                case 6:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800422, 1);
                        break;
                    }
                case 7:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800522, 1);
                        break;
                    }
                case 9:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800618, 1);
                        break;
                    }
                case 10:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800725, 1);
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800810, 1);
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800811, 1);
                        break;
                    }
                case 13:
                    {
                        if (!client.Inventory.Remove(3004247, 1, stream)) break;
                        client.Inventory.Add(stream, 800917, 1);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.P7SacredTreasuresEnvoy)]
        public static void P7SacredTreasuresEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("There`s a mysterious treasure sealed since the ancient times. With a Savage Bone, you can explore and have a chance to get Sacred refinery materials, rare")
                            .AddText("~weapon accessories, and even the legendary P7 Dragon Soul.")
                            .AddOption("I~wanna~explore.", 1)
                       .AddOption("Special~exploration.", 2)
                    .AddOption("I~see.", 255)
                     .AddAvatar(200)
                    .FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("The Nemesis Tyrant in the Bloodshed Sea has the Savage Bones. You can use one to explore each time.")
                           .AddOption("Use~a~Savage~Bone.", 10)
                      .AddOption("Send~me~there.", 11)
                   .AddOption("I~see.", 255)
                    .AddAvatar(200)
                   .FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("You~arrived~at~the~Gale~Shallow.");
                        client.Teleport(93, 410, 10137);


                        break;
                    }
                case 10:
                    {
                       
                        
                        if (client.Inventory.Contain(3004181, 1))
                        {
                            
                            if (!client.Inventory.Remove(3004181, 1, stream)) break;
                            uint ItemID = 0;
                            ushort Rand = (ushort)Program.GetRandom.Next(1, 2000);
                            if (Rand < 200)
                            {
                                client.CreateBoxDialog("You received a P7 WeaponSoulPackScrap.");
                                ItemID = Database.ItemType.P7WeaponSoulScrap;
                            }
                            else if (Rand > 300 && Rand < 400)
                            {
                                client.CreateBoxDialog("You received a P7 EquipmentSoulPackScrap.");
                                ItemID = Database.ItemType.P7EquipmentSoulScrap;
                            }
                            else
                            {
                                client.CreateBoxDialog("You received a SacredRefineryPackScrap.");
                                ItemID = Database.ItemType.SacredRefineryScrap;
                            }
                            client.Inventory.Add(stream, ItemID);

                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have a Savage Bone.")
         .AddOption("My~bad.", 255)
          .AddAvatar(200)
         .FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {//3004181 SavageBone
                        data.AddText("If you don`t have a Savage Bone, you can also pay 3700 or 17.000 CPs to explore. Of course, the more you pay, the better you get.")
                           .AddText("~How much would you like to pay?")
                           .AddOption("3700~CPs.", 3)
                      //.AddOption("17.000~CPs.", 5)
                   .AddOption("I~see.", 255)
                    .AddAvatar(200)
                   .FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to use 3700 CPs to explored the P7 Sacred Treasures?")
                          .AddOption("Yes.", 4)
                  .AddOption("No.", 255).AddAvatar(200).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("Are you sure you want to use 17.000 CPs to explored the P7 Sacred Treasures?")
                          .AddOption("Yes.", 6)
                  .AddOption("No.", 255).AddAvatar(200).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (client.Player.ConquerPoints >= 17000)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.ConquerPoints -= 17000;


                                uint ItemID = 3001322;
                                if (Role.Core.Rate(20))
                                {
                                    ItemID = 3004181;
                                }
                                else
                                    client.CreateBoxDialog("You~received~a~Money~Bag.");
                                client.Inventory.Add(stream, ItemID);
                            }
                            else
                            {
                                data.AddText("Please make 1 space in your inventory.")
                                .AddOption("Ah~yes.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                        .AddOption("Alright.", 255).AddAvatar(200).FinalizeDialog();
                        }



                        break;
                    }
                case 4:
                    {
                        if (client.Player.ConquerPoints >= 3700)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.ConquerPoints -= 3700;


                                uint ItemID = 3001318;
                                if (Role.Core.Rate(4))
                                {
                                    ItemID = 3004181;
                                }
                                else
                                    client.CreateBoxDialog("You~received~a~Money~Bag.");
                                client.Inventory.Add(stream, ItemID);
                            }
                            else
                            {
                                data.AddText("Please make 1 space in your inventory.")
                                .AddOption("Ah~yes.", 255).AddAvatar(200).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                        .AddOption("Alright.", 255).AddAvatar(200).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ElementalPool)]
        public static void ElementalPool(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("My optione its: 1: I open the  NightmareCaptain for one YinYangSeal and you'll have 20% chance to obtain an Elemental Jade.")
                            .AddText("2: I open the  PurpleBanshee for one Elemental Jade and you'll have 40% chance to obtain an Life`sEye.")
                            .AddText("3: I change you Stardust for Shadow Scroll , TwilightScroll or FatalScroll!")
                            .AddOption("Change my Stardust", 1)
                       .AddOption("Open~NightmareCaptain", 3)
                    .AddOption("Open~PurpleBanshee", 2)
                    .AddOption("Not~interested.", 255)
                    .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(3004462, 1))
                        {
                            if (client.Player.Map == 3851)
                            {
                                if (!client.Map.ContainMobID(20100))
                                {
                                    if (!client.Inventory.Remove(3004462, 1, stream)) break;
                                    Database.Server.AddMapMonster(stream, client.Map, 20100, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                }
                                else
                                    client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");

                            }
                            else
                                client.CreateDialog(stream, "Use that item in Infinity Abyss,", "I~see.");
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the YinYangSeal in your inventory.")
                .AddOption("Okay.", 255)
                .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(3004463, 1))
                        {
                            if (client.Player.Map == 3851)
                            {
                                if (!client.Map.ContainMobID(20101))
                                {
                                    if (!client.Inventory.Remove(3004463, 1, stream)) break;
                                    Database.Server.AddMapMonster(stream, client.Map, 20101, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                }
                                else
                                    client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");

                            }
                            else
                                client.CreateDialog(stream, "Use that item in Infinity Abyss,", "I~see.");
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the ElementalJade in your inventory.")
                .AddOption("Okay.", 255)
                .AddAvatar(203).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("I change you Stardust for Shadow Scroll , TwilightScroll or FatalScroll. Please select the item.")
                      .AddOption("ShadowScroll", 10)
                      .AddOption("TwilightScroll", 11)
                      .AddOption("FatalScroll", 12)

                      .AddOption("Not~interested.", 255)
                      .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3004465, 1))
                        {
                            if (!client.Inventory.Remove(3004465, 1, stream)) break;
                            client.Inventory.Add(stream, 3004459, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the Stardust in your inventory.")
                .AddOption("Okay.", 255)
                .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Contain(3004465, 1))
                        {
                            if (!client.Inventory.Remove(3004465, 1, stream)) break;
                            client.Inventory.Add(stream, 3004460, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the Stardust in your inventory.")
                .AddOption("Okay.", 255)
                .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(3004465, 1))
                        {
                            if (!client.Inventory.Remove(3004465, 1, stream)) break;
                            client.Inventory.Add(stream, 3004461, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the Stardust in your inventory.")
                .AddOption("Okay.", 255)
                .AddAvatar(203).FinalizeDialog();
                        }

                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.SwapperStarry)]
        public static void SwapperStarry(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You'll need 4 Red Flowers and 4 White Flowers to begin the quest. You can obtain Red Flowers and White Flowers in the Evil Abyss .")
                            .AddText("There are 3 stages for you to complete the Epic Quest:")
                            .AddText("Stage 1: Go to feel Yin Yang Stele, and you'll have 30% chance to obtain a Yin Yang Seal.")
                            .AddText("Stage 2: Use a Yin Yang Seal to try to awaken the Elemental Guard, and you'll have 20% chance to obtain an Elemental Jade.")
                            .AddText("Stage 3: Use an Elemental Jade to try Wheel of Life, and you'll have 40% chance to obtain a Life's Eye.")
                            .AddOption("Tell~me~more", 1)
                              .AddOption("I~have~4 Red Flowers and 4 White Flowers.", 3)
                              .AddOption("Buy YinYangSeal 1000 CPs.", 12)
                    .AddOption("Teleport~to~Evil~Abyss", 2)
                    .AddOption("Teleport~to~TwinCity", 10)
                    .AddOption("Not~interested.", 255)
                    .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("If you fail to obtain key quest items during Stage 2 and Stage 3, instead you'll receive Stardust. With Stardust, you can exchange with Swapper Starry at the Infinite Abyss (27,40) for Shadow Scroll, Twilight Scroll, Fatal Scroll.")
                            .AddText("Note: When using a Life's Eye with a Katana equipped, you'll be able to transform the Katana into an Epic Weapon, Nobunaga's Claw. If you already have a Nobunaga's Claw in your left hand, and a Katana in the right, when you use a Life's Eye, your Katana will automatically be transformed into a Nobunaga's Claw, vice versa.")
                      .AddOption("Sound~Good", 255)
                      .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        if (client.Player.ConquerPoints >= 1000)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.ConquerPoints -= 1000;
                                client.Inventory.Add(stream, 3004462, 1);
                            }
                            else
                            {
                                data.AddText("Please make 1 more space in your inventory")
                                                                    .AddOption("Let me check.", 255)
                                                                    .AddAvatar(203).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                        .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {

                        client.Teleport(612, 634, 1700, 0);
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(722732, 4) && client.Inventory.Contain(722736, 4))
                        {
                            if (!client.Inventory.Remove(722732, 4, stream)) break;
                            if (!client.Inventory.Remove(722736, 4, stream)) break;
                            if (Role.Core.Rate(30))
                            {
                                client.Inventory.Add(stream, 3004462);
                            }
                            else
                                data.AddText("You wasn`t very lucky today .. I hope you will have a better luck next time ^^.")
                     .AddOption("Ah~OK", 255)
                     .AddAvatar(203).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Please come back when you will have at inventory 4 Red Flowers and 4 White Flowers.")
                     .AddOption("Ah~OK", 255)
                     .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 10:
                    {
                        client.Teleport(300, 278, 1002);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.GameManager)]
        public static void GameManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.VipLevel >= 4)
                        {
                            data.AddText("Hi, I can teleport you to the poker room. Where you can enjoy yourself in the Texas Hold`em Poker. Wish you a good luck and good health.")
                       .AddOption("Enter~Poker~Room.", 1)
                       .AddOption("Enter~VIP~Hall.", 2)
.AddOption("Enter~Roulette~Room.", 3)
.AddOption("Not~interested.", 255)
                       .AddAvatar(175).FinalizeDialog();

                        }
                        else
                        {
                            data.AddText("Hi, I can teleport you to the poker room. Where you can enjoy yourself in the Texas Hold`em Poker. Wish you a good luck and good health.")
                       .AddOption("Enter~Poker~Room.", 1)
                                // .AddOption("Enter~VIP~Hall.", 2)
.AddOption("Enter~Roulette~Room.", 3)
.AddOption("Not~interested.", 255)
                       .AddAvatar(175).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {

                        client.Teleport(55, 62, 3852);
                        client.SendSysMesage("You arrived on the Roulette Room.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);


                        break;
                    }
                case 1:
                    {
                        client.SendSysMesage("You are teleported to the Poker Room.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(157, 119, 1858);


                        break;
                    }
                case 2:
                    {
                        client.SendSysMesage("You arrived on the VIP Hall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(55, 62, 1860);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CasinoHostess)]
        public static void CasinoHostess(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Greeting, I`m glad to be at your service. Win or lose is not that important. To win is not the ultimate aim of the game, happiness is what we have been pursuing. Here, I even more like to wish you a good health than a good luck!")
                    .AddOption("I~have~no~interest~in~it.", 1)
                    .AddOption("I~still~want~to~play.", 255)
                    .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        client.Teleport(298, 283, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Stanley)]
        public static void Stanley(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                   .AddOption("TwinCity.", 2)
                   .AddOption("Let~me~think...", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Teleport(300, 278, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Lab4)]
        public static void Lab4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~NorthGeneral.~This~is~the~fourth~floor.~If~you~want~to~go~back~to~Twin~City.~I~can~help~you.");
                        data.AddOption("To~Twin~City.", 2);
                        data.AddOption("I~will~stay~here.");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        client.Teleport(300, 278, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Lab3)]
        public static void Lab3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~SouthGeneral.~This~is~the~third~floor~of~Labyrinth.~If~you~want~to~enter~the~next~floor,~you~should~give~me~a");
                        data.AddText("~SoulToken.~Or~would~you~like~to~go~back~to~Twin~City?");
                        data.AddOption("To~the~next~floor.", 1);
                        data.AddOption("To~Twin~City.", 3);
                        data.AddOption("I~will~stay~here.");
                        data.AddAvatar(31);
                        data.FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721539, 1))
                        {
                            data.AddText("The~next~floor~is~more~dangerous.~You~should~be~more~careful.");
                            data.AddOption("Thank~you~very~much.", 2);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~don`t~have~a~SoulToken,~I~cannot~teleport~you~there.");
                            data.AddOption("I~will~stay~here.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(721539, 1))
                        {
                            if (!client.Inventory.Remove(721539, 1, stream)) break;
                            client.Teleport(5, 290, 1354);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~don`t~have~1~SoulToken.")
                        .AddOption("Sigh...", 255)
                        .AddAvatar(63).FinalizeDialog();

                        }


                        break;
                    }
                case 3:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 4)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        client.Teleport(300, 278, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Lab2)]
        public static void Lab2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~WestGeneral.~This~is~the~second~floor~of~Labyrinth.~If~you~want~to~enter~the~next~floor,~you~should~give~me~an");
                        data.AddText("~EarthToken,~or~would~you~like~to~go~back~to~Twin~City?");
                        data.AddOption("To~the~next~floor.", 1);
                        data.AddOption("To~Twin~City.", 3);
                        data.AddOption("I~will~stay~here.");
                        data.AddAvatar(31);
                        data.FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721538, 1))
                        {
                            data.AddText("The~next~floor~is~more~dangerous.~You~should~be~more~careful.");
                            data.AddOption("Thank~you~very~much.", 2);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~don`t~have~an~EarthToken.~I~cannot~teleport~you~to~the~next~floor.");
                            data.AddOption("I~will~stay~here.");
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(721538, 1))
                        {
                            if (!client.Inventory.Remove(721538, 1, stream)) break;
                            client.Teleport(25, 268, 1353);
                        }
                        else
                        {
                            data.AddText("You~don`t~have~an~EarthToken.~I~cannot~teleport~you~to~the~next~floor.");
                            data.AddOption("I~will~stay~here.");
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 4)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Teleport(300, 278, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Lab1)]
        public static void Lab1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~EastGeneral.~This~is~the~first~floor~of~Labyrinth.~If~you~want~to~enter~the~next~floor,~you~should~give~me~a");
                        data.AddText("~Sky~Token,~which~is~carried~by~the~Gold~Gosts.");
                        data.AddOption("To~the~next~floor.", 1);
                        data.AddOption("To~Twin~City.", 3);
                        data.AddOption("I~will~stay~here.");
                        data.AddAvatar(31);
                        data.FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721537, 1))
                        {
                            data.AddText("The~next~floor~is~more~dangerous.~You~should~be~more~careful.");
                            data.AddOption("Thank~you~very~much.", 2);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~have~no~Sky~Token.~Hurry~to~loot~one~from~the~Gold~Gosts.");
                            data.AddOption("I~will~stay~here.");
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {

                        if (client.Inventory.Contain(721537, 1))
                        {
                            if (!client.Inventory.Remove(721537, 1, stream)) break;
                            client.Teleport(27, 227, 1352);
                        }
                        else
                        {
                            data.AddText("You~have~no~Sky~Token.~Hurry~to~loot~one~from~the~Gold~Gosts.");
                            data.AddOption("I~will~stay~here.");
                            data.AddAvatar(31);
                            data.FinalizeDialog();

                        }


                        break;
                    }
                case 3:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 4)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        client.Teleport(300, 278, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Simon)]
        public static void Simon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Great~rewards~will~attract~many~brave~people.~I~am~looking~for~brave~people~to~help~me~take~my~patrimony~back.")
                   .AddOption("Please~tell~me~more.", 1)
                   .AddOption("What~rewards?", 10)
                   .AddOption("I~got~SunDiamonds.", 12)
                   .AddOption("I~got~MoonDiamonds.", 13)
                   .AddOption("I~got~StarDiamonds.", 14)
                   .AddOption("I~got~CloudDiamonds.", 15)
                   .AddOption("Let~me~think...", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Contain(721536, 17))
                        {
                            if (!client.Inventory.Remove(721536, 17, stream)) break;
                            client.Inventory.Add(stream, 721540, 1);
                            data.AddText("Well~done.~Here~is~the~rewards~I~promised~to~you.");
                            data.AddOption("Thanks.");
                            data.AddAvatar(63);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~are~kidding~on~me.~How~dare~you~come~here~to~claim~prize~with~so~few~CloudDiamonds?")
             .AddOption("Wait.~I~will~get~more.", 255)
             .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.Contain(721535, 17))
                        {
                            if (!client.Inventory.Remove(721535, 17, stream)) break;
                            Role.Flags.Gem[] Gems = new Role.Flags.Gem[] { Role.Flags.Gem.NormalDragonGem, Role.Flags.Gem.NormalFuryGem, Role.Flags.Gem.NormalGloryGem
                           , Role.Flags.Gem.NormalKylinGem, Role.Flags.Gem.NormalMoonGem, Role.Flags.Gem.NormalPhoenixGem, Role.Flags.Gem.NormalRainbowGem, Role.Flags.Gem.NormalThunderGem
                           , Role.Flags.Gem.NormalTortoiseGem, Role.Flags.Gem.NormalVioletGem};

                            Role.Flags.Gem GemType = (Role.Flags.Gem)Gems[Program.GetRandom.Next(0, Gems.Length)];
                            client.Inventory.Add(stream, (uint)(700000 + (int)GemType), 1);
                            data.AddText("Well~done.~Here~is~the~rewards~I~promised~to~you.");
                            data.AddOption("Thanks.");
                            data.AddAvatar(63);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~are~kidding~on~me.~How~dare~you~come~here~to~claim~prize~with~so~few~StarDiamonds?")
             .AddOption("Wait.~I~will~get~more.", 255)
             .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Contain(721534, 17))
                        {
                            if (!client.Inventory.Remove(721534, 17, stream)) break;
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 4);
                            data.AddText("Well~done.~Here~is~the~rewards~I~promised~to~you.");
                            data.AddOption("Thanks.");
                            data.AddAvatar(63);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~are~kidding~on~me.~How~dare~you~come~here~to~claim~prize~with~so~few~MoonDiamonds?")
             .AddOption("Wait.~I~will~get~more.", 255)
             .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(721533, 17))
                        {
                            if (!client.Inventory.Remove(721533, 17, stream)) break;
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            data.AddText("Well~done.~Here~is~the~rewards~I~promised~to~you.");
                            data.AddOption("Thanks.");
                            data.AddAvatar(63);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~are~kidding~on~me.~How~dare~you~come~here~to~claim~prize~with~so~few~SunDiamonds?")
             .AddOption("Wait.~I~will~get~more.", 255)
             .AddAvatar(63).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("I~need~Sun`Diamond,~Moon~Diamond,~Star~Diamond~and~Cloud~Diamond.")
                          .AddText("If~You~get~them~for~me,~I~will~give~you~some~rewards.")
                .AddOption("What~rewards?", 11)
                .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        data.AddText("2~meteors~for~17~SunDiamonds,~4~meteors~for~17~MoonDiamonds,~a~normal~gem~for~17~StarDiamonds~and~a~AncestorBox")
                         .AddText("for~17~CloudDiamonds.~If~you~are~lucky~enough,~you~will~get~a~big~surprise~from~the~box.")
               .AddOption("I~see.~Thank~you.", 255)
               .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("My~ancestors~built~a~Labyrinth~long~before.~Many~treasures~were~stored~there~like~SunDiamonds,~MoonDiamonds,~")
                                  .AddText("StarDiamond~and~so~on.~But~it~was~occupied~by~fiece~monsters~soon.~They~expelled~our~clansmen~and~kept~the~treasure.")
                        .AddOption("Wow,~miserable.", 2)
                        .AddOption("I~have~no~interest.", 255)
                        .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("The~Labyrinth~contains~4~floors~guarded~by~different~monsters.~If~you~can~find~the~treasures~and~give~them~to~me,~I~will~reward")
              .AddOption("Please~continue.", 3)
              .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("There~are~3~kinds~of~monsters~on~each~floor.~The~more~monsters,~the~weaker~they~are.~The~ones~in~lesser")
                              .AddText("quantities~are~strong.~The~fiercest~monsters~are~always~moving~around.~And~they~drop~different~tokens.")
                           .AddOption("What~do~they~drop?", 4)
                           .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        data.AddText("SkyToken,~EarthToken~and~SoulToken~you~need~to~enter~the~next~floor.~While~the~weak~monsters~usually~drop~treasure.")
                        .AddText("After~You~get~a~token,~find~a~general~who~will~send~you~to~the~next~floor.~Some~boss~monsters~drop~rare~items.")
              .AddOption("I~see.~Thanks.", 5)
              .AddOption("I~must~leave~now.", 255)
              .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("There are 3 ways to enter the Labyrinth: 1. Pay 2,000 Virtue Points; 2. Submit 5 meteors;")
                      .AddText("~3. VIP service.")
            .AddOption("Give~you~2000~Virtue~Points.", 6)
            .AddOption("I~have~5~Meteors.", 7)
              .AddOption("VIP~service.", 8)
            .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Player.VipLevel >= 1)
                        {
                            client.Teleport(18, 127, 1351);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~don`t~have~VIP.")
                           .AddOption("Sigh...", 255)
                           .AddAvatar(63).FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        if (client.Player.VirtutePoints > 2000)
                        {
                            client.Player.VirtutePoints -= 2000;
                            client.Player.SendUpdate(stream, client.Player.VirtutePoints, MsgServer.MsgUpdate.DataType.VirtutePoints);
                            client.Teleport(18, 127, 1351);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~don`t~have~enough~virtue~points.")
                           .AddOption("Sigh...", 255)
                           .AddAvatar(63).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(Database.ItemType.Meteor, 5))
                        {
                            if (!client.Inventory.Remove(Database.ItemType.Meteor, 5, stream)) break;
                            client.Teleport(18, 127, 1351);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~don`t~have~5~Meteors.")
                            .AddOption("Sigh...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        }

                        break;
                    }

            }
        }
    }
}
