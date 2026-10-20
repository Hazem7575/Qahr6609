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
        [NpcAttribute(NpcID.SkillEraser)]
        public static void SkillEraser(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);


            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You have to give up something in roder to get something. I can help you to forget the skills you don't need anymore.")
                       .AddOption("Forget skills in this life.", 1)
                       .AddOption("I'll think about it.", 255)
                       .AddAvatar(100)
                       .FinalizeDialog();
                        break;
                    }
                #region I want to ask about something else
                case 99:
                    {
                        data.AddText("You have to give up something in roder to get something. I can help you to forget the skills you don't need anymore.")
                       .AddOption("Forget skills in this life.", 1)
                       .AddOption("I'll think about it.", 255)
                        .AddAvatar(100)
                      .FinalizeDialog();
                        break;
                    }
                #endregion

                case 1:
                    {


                        data.AddText("If you don't need some skills anymore, i can help you forget them. So what kind of skill would you like to forget?")
                       .AddOption("Summon skills.", 2)
                       .AddOption("General weapon skills.", 3)  //done
                       .AddOption("Transform skills.", 4)  //done
                       .AddOption("Class-based skills.", 5) //done
                       .AddOption("I want to ask about something else.", 99)
                        .AddAvatar(100)
                       .FinalizeDialog();
                        break;
                    }

                #region Class-based skills.
                case 5: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Spirit Healing.", 6)
                       .AddOption("Serenity.", 7)
                       .AddOption("Flying Moon.", 8)
                       .AddOption("Next.", 9) //ok
                       .AddOption("Cruel Shade.", 10)
                       .AddOption("Compassion.", 11)
                       .AddOption("Thunder.", 12)
                       .AddOption("I want to ask something else..", 1)
                         .AddAvatar(100)
                     .FinalizeDialog();
                        break;

                    }



                #region SpiritHealing
                case 6:
                    {

                        {
                            var SpellID = 1190;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill SpiritHealing. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }

                #endregion

                #region Serenity
                case 7:
                    {

                        {
                            var SpellID = 10400;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Serenity. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion



                #region FlyingMoon
                case 8:
                    {

                        {
                            var SpellID = 1320;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill FlyingMoon. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region CruelShade
                case 10:
                    {

                        {
                            var SpellID = 3050;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill CruelShade. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Compassion
                case 11:
                    {

                        {
                            var SpellID = 10430;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Compassion. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Thunder
                case 12:
                    {

                        {
                            var SpellID = 1000;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Thunder. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion



                #region Invisibility
                case 13:
                    {

                        {
                            var SpellID = 1075;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Invisibility. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Fire
                case 14:
                    {

                        {
                            var SpellID = 1001;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Fire. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Kraken'sRevenge
                case 15:
                    {

                        {
                            var SpellID = 11100;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Kraken'sRevenge. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Pervade
                case 16:
                    {

                        {
                            var SpellID = 3306;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Pervade. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion



                #region FireCircle
                case 17:
                    {

                        {
                            var SpellID = 1120;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill FireCircle. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #endregion

                #region Transform skills.
                case 4: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Night Devil.", 18)
                       .AddOption("Water Elf.", 19)
                       .AddOption("Golem.", 20)
                       .AddOption("Divine Hare.", 22)
                       .AddOption("I want to ask something else..", 1)
                      .AddAvatar(100)
                    .FinalizeDialog();
                        break;

                    }


                #region DivineHare
                case 22:
                    {

                        {
                            var SpellID = 10522;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill DivineHare. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Golem
                case 20:
                    {

                        {
                            var SpellID = 1270;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Golem. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region WaterElf
                case 19:
                    {

                        {
                            var SpellID = 1280;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill WaterElf. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region NightDevil
                case 18:
                    {

                        {
                            var SpellID = 1360;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill NightDevil. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #endregion

                #region General weapon skills.
                case 3: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget");
                        data.AddOption("FastBlade.", 23);
                        data.AddOption("Phoenix.", 24);
                        data.AddOption("Wide Strike.", 25);
                        data.AddOption("Next.", 26); //done
                        data.AddOption("Scent Sword.", 27);
                        data.AddOption("Speed Gun.", 28);
                        data.AddOption("Penetration.", 29);
                        data.AddOption("I want to ask something else..", 1)
                        .AddAvatar(100)
                        .FinalizeDialog();
                        break;

                    }


                case 26: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Boreas.", 30)
                       .AddOption("Snow.", 31) //aici am ramas
                       .AddOption("Boom.", 32)
                       .AddOption("Next.", 33) //done
                       .AddOption("Stranded Monster.", 34)
                       .AddOption("Halt.", 35)
                       .AddOption("Previous.", 3)
                       .AddOption("I want to ask something else..", 1)
                        .AddAvatar(100)
                     .FinalizeDialog();
                        break;

                    }
                case 33: //       
                    {
                        data.AddText("After you forget the skills you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Seizer.", 36)
                       .AddOption("Rage.", 37)
                       .AddOption("Roamer.", 38)
                       .AddOption("Next.", 39) //done
                       .AddOption("EarthQuake.", 40)
                       .AddOption("Celestial.", 41)
                       .AddOption("Previous.", 26) //done
                       .AddOption("I want to ask something else..", 1)
                       .AddAvatar(100)
                     .FinalizeDialog();
                        break;

                    }


                case 39: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Dragon Tail.", 42)
                       .AddOption("Viper Fang.", 69)
                       .AddOption("Previous.", 33) //done
                       .AddOption("I want to ask something else..", 1)
                          .AddAvatar(100)
                     .FinalizeDialog();
                        break;

                    }
                #region DragonTail
                case 42:
                    {

                        {
                            var SpellID = 11000;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill DragonTail. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region ViperFang
                case 69:
                    {

                        {
                            var SpellID = 11005;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill ViperFang. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Seizer
                case 36:
                    {

                        {
                            var SpellID = 7109;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Seizer. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Rage
                case 37:
                    {

                        {
                            var SpellID = 7020;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Rage. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Roamer
                case 38:
                    {

                        {
                            var SpellID = 7040;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Roamer. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion



                #region EartQuake
                case 40:
                    {

                        {
                            var SpellID = 7010;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill EartQuake. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Celestial
                case 41:
                    {

                        {
                            var SpellID = 7030;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Celestial. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Boreas
                case 30:
                    {

                        {
                            var SpellID = 5050;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Boreas. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Snow
                case 31:
                    {

                        {
                            var SpellID = 5010;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Snow. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Boom
                case 32:
                    {

                        {
                            var SpellID = 5040;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Boom. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region StrandedMonster
                case 34:
                    {

                        {
                            var SpellID = 5020;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill StrandedMonster. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Halt
                case 35:
                    {

                        {
                            var SpellID = 1300;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Halt. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region FastBlade
                case 23:
                    {

                        {
                            var SpellID = 1045;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill FastBlade. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Phoenix
                case 24:
                    {

                        {
                            var SpellID = 1420;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Phoenix. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region WideStrike
                case 25:
                    {

                        {
                            var SpellID = 1250;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill WideStrike. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region ScentSword
                case 27:
                    {

                        {
                            var SpellID = 1046;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill ScentSword. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region SpeedGun
                case 28:
                    {

                        {
                            var SpellID = 1260;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill SpeedGun. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Penetration
                case 29:
                    {

                        {
                            var SpellID = 1290;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Penetration. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion
                #endregion

                #region Summon Skills

                #region Summon skills.
                case 2: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Bloody Bat.", 49)
                       .AddOption("Fire Evil.", 44)
                       .AddOption("Summon Bat Boss.", 45)
                       .AddOption("Summon Guard.", 46)
                       .AddOption("Skeleton.", 47)
                       .AddOption("Summon Bat.", 43)
                       .AddOption("I want to ask something else..", 1)
                         .AddAvatar(100)
                        .FinalizeDialog();
                        break;

                    }
                #endregion
                #region Summon Bat.
                case 43:
                    {

                        {
                            var SpellID = 4010;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Summon Bat. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Skeleton.
                case 47:
                    {

                        {
                            var SpellID = 4070;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Skeleton. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Guard.
                case 46:
                    {

                        {
                            var SpellID = 4000;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Guard. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Bat.
                case 45:
                    {

                        {
                            var SpellID = 4000;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill Guard. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region FireEvil.
                case 44:
                    {

                        {
                            var SpellID = 4060;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill FireEvil. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region BloodyBat.
                case 49:
                    {

                        {
                            var SpellID = 8262;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill BloodyBat. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #endregion








                case 100:
                    {



                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.FrozenGrottoGeneral)]
        public static void FrozenGrottoGeneral(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level < 28)
                        {
                            data.AddText("You can get the monster hunter quest from the TC Captain now! It`s a great place for you to level up. Here is a steed to");
                            data.AddText("~take you there!");
                            data.AddOption("Okay,~thanks.", 1);
                            data.AddOption("I~have~no~interest.");
                            data.AddAvatar(57);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Level < 48)
                            {
                                data.AddText("You are able to take the quest from the PC Captain now! You can level up more quickly there. Here is a steed to take you there!");
                                data.AddOption("Okay,~thanks.", 2);
                                data.AddOption("I~have~no~interest.");
                                data.AddAvatar(57);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Level < 68)
                                {
                                    data.AddText("You`d better take the quests from the AC Captain. It`s the right place for you to level up. Here is a steed to take you");
                                    data.AddText("~there!");
                                    data.AddOption("Okay,~thanks.", 3);
                                    data.AddOption("I~have~no~interest.");
                                    data.AddAvatar(57);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Level < 88)
                                    {
                                        data.AddText("You are powerful enough to take the quests from the DC Captain now! Here is a steed to take you there!");
                                        data.AddOption("Okay,~thanks.", 4);
                                        data.AddOption("I~have~no~interest.");
                                        data.AddAvatar(57);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Level < 103)
                                        {
                                            data.AddText("You are powerful now, and you are able to take the quests from the BI Captain! He is in need of your help! Here is a steed to");
                                            data.AddText("~take you there!");
                                            data.AddOption("Okay,~thanks.", 5);
                                            data.AddOption("I~have~no~interest.");
                                            data.AddAvatar(57);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Level < 120)
                                            {
                                                data.AddText("You are strong enough to kill the monsters in Mystic Castle. Why not take a quest from MC Captain to level up quickly? Here");
                                                data.AddText("~is a steed to take you there.");
                                                data.AddOption("Okay,~thanks.", 6);
                                                data.AddOption("I~have~no~interest.");
                                                data.AddAvatar(57);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                data.AddText("Cloud Saint in Market (291,224) is looking for the heroes to help him exterminate the monsters. If you can finish the quest,");
                                                data.AddText("~you will receive 5 EXP Balls as rewards!");
                                                data.AddOption("I~see.");
                                                data.AddAvatar(57);
                                                data.FinalizeDialog();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        client.Teleport(085, 321, 1000);
                        break;
                    }
                case 5:
                    {
                        client.Teleport(792, 571, 1015);
                        break;
                    }
                case 4:
                    {
                        client.Teleport(483, 634, 1000);
                        break;
                    }
                case 3:
                    {
                        client.Teleport(571, 621, 1020);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(233, 260, 1011);
                        break;
                    }
                case 1:
                    {
                        client.Teleport(311, 346, 1002);
                        break;
                    }

            }
        }


        //gata da
        [NpcAttribute(NpcID.KingOfTheHill)]
        public static void KingOfTheHill(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("King of the Hill is a gear based free for all event. Players gain points by")
                          .AddText("standing in the middle of the room while alive. A scoreboard will be listed in ")
               .AddText("the top right corner of the screen and when someone reaches 120 points they win.")
               .AddText("All players received ConquerPoints based on the total points they end the match.")
                          .AddOption("Sign~me~up", 1)
                 .AddOption("I`ll~think~it~over.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {

                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.KingOfTheHill)
                        {
                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {
                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
         .AddOption("I see. ", 255)
         .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
    .AddOption("I see. ", 255)
    .AddAvatar(63).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SkillTournament)]
        public static void SkillTournament(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Fastblade is a skill based for all event. Every player starts")
                          .AddText(" with 3 points. When hit by SS/FB/Viper fang your score is reduced by 1 point.")
               .AddText("When a player.s score reaches 0(they have been hit 3 times) they are eliminated.")
               .AddText("The last persoan standing wins 4k ConquerPoints.")
                          .AddOption("Sign~me~up", 1)
                 .AddOption("I`ll~think~it~over.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {

                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.SkillTournament)
                        {
                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {

                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
                                        .AddOption("I see. ", 255)
                                        .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
.AddOption("I see. ", 255)
.AddAvatar(63).FinalizeDialog();
                        }
                    }
                    break;
            }
        }

        [NpcAttribute(NpcID.Football)]
        public static void Football(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {


                        data.AddText("It`s a new Tournament that takes place every day.!")
                            .AddText("Footbal tournament is based on linear spells like (ss/fb,viper fang,etc).Basically there are ")
                 .AddText("2 teams,both of the teams starts from the middle of the map and they have to fight each other ")
                 .AddText("in order to get the ball.Player who have the ball needs to go in square of the opponents team ")
                 .AddText("to score.If one of the opponent players sucessfully hits you he will get the ball and will try ")
                 .AddText("to go to the square of your map to score.")
                 .AddText("First team who gets 5 goals will win or the team with the most goals scored after 10 minutes.")
                            .AddOption("Sign~me~up", 1)
                            .AddOption("Special~rewards?", 2)
                   .AddOption("I`ll~think~it~over.", 255).FinalizeDialog();


                        break;
                    }
                case 2:
                    {

                        data.AddText("Players who gain points in Footbal Tournament can exchange them into Talismans.You get points only when you score.At the moment you have " + client.Player.MyFootBallPoints + " Points.")
                             .AddOption("EuroSilverTrophy(30 Points)", 3)
                                    .AddOption("EuroGoldTrophy(60 Points)", 4)
                             .AddOption("I`ll~think~it~over.", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check. ", 255)
                                .FinalizeDialog();
                            break;
                        }
                        if (client.Player.MyFootBallPoints >= 30)
                        {
                            client.Player.MyFootBallPoints -= 30;
                            client.Inventory.Add(stream, 2100165, 1);
                            Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("" + client.Player.Name + " has claimed EuroSilveTrophy from Football Tournament npc", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System);
                            Program.SendGlobalPackets.Enqueue(msg.GetArray(stream));
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough FootBallPoints.")
                             .AddOption("I~see. ", 255)
                             .FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check. ", 255)
                                .FinalizeDialog();
                            break;
                        }

                        if (client.Player.MyFootBallPoints >= 60)
                        {
                            client.Player.MyFootBallPoints -= 60;
                            client.Inventory.Add(stream, 2100175, 1);
                            Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("" + client.Player.Name + " has claimed EuroGoldTrophy from Football Tournament npc", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System);
                            Program.SendGlobalPackets.Enqueue(msg.GetArray(stream));
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough FootBallPoints.")
                               .AddOption("I~see. ", 255)
                               .FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {

                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Idle
                            && Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FootBall)
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

        [NpcAttribute(NpcID.PkReset)]
        public static void PkReset(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("In exchange of 10 CPs per PK Point i will decrease your PK, do you think you are strong enough for this exchange?")
                           .AddOption("Clear 10 pkPoints.", 1)
                            .AddOption("Clear 100 pkPoints.", 2)
                             .AddOption("Clear 500 pkPoints.", 3)
                            .AddOption("Nevermind.", 255)
                              .AddAvatar(264)
                            .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 100)
                        {
                            if (client.Player.PKPoints >= 10)
                            {
                                client.Player.PKPoints -= 10;
                                client.Player.ConquerPoints -= 100;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                client.CreateBoxDialog("You've removed 10 Pk Points.");
                            }
                            else
                            {
                                data.AddText("You can not use this option for now since your Pk points are less than 10.")
                                    .AddOption("I see.", 255)
                                      .AddAvatar(264)
                                    .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                                     .AddOption("I see.", 255)
                                       .AddAvatar(264)
                                     .FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 1000)
                        {
                            if (client.Player.PKPoints >= 100)
                            {
                                client.Player.PKPoints -= 100;
                                client.Player.ConquerPoints -= 1000;
                                client.CreateBoxDialog("You've removed 100 Pk Points.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            }
                            else
                            {
                                data.AddText("You can not use this option for now since your Pk points are less than 100.")
                                    .AddOption("I see.", 255)
                                      .AddAvatar(264)
                                    .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                                     .AddOption("I see.", 255)
                                       .AddAvatar(264)
                                     .FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 5000)
                        {
                            if (client.Player.PKPoints >= 500)
                            {
                                client.Player.PKPoints -= 500;
                                client.Player.ConquerPoints -= 5000;
                                client.CreateBoxDialog("You've removed 500 Pk Points.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            }
                            else
                            {
                                data.AddText("You can not use this option for now since your Pk points are less than 500.")
                                    .AddOption("I see.", 255)
                                      .AddAvatar(264)
                                    .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                                     .AddOption("I see.", 255)
                                       .AddAvatar(264)
                                     .FinalizeDialog();
                        }

                        break;
                    }
            }
        }




        [NpcAttribute(NpcID.VipQuest)]
        public static void VipQuest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I'm pretty sure you are looking for a way to get VIP 6, good thing there is a way but it wont be that easy.")
                   .AddOption("No problem.", 1)
                    .AddOption("I`ll~think~it~over.", 255)
                    .AddAvatar(105)
                    .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        client.Teleport(608, 632, 1700);
                        break;
                    }
            }
        }
    }
}
