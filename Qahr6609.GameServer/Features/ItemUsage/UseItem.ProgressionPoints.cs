using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgItemUsuagePacket
    {
        // UseItem handlers: VIP, Chi, Jiang Hu, study points, epic quests, books.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_ProgressionPoints(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
              
                case 780001:
                    {
                        if (DateTime.Now > client.Player.ExpireVip)
                        {
                            client.Player.ExpireVip = DateTime.Now;
                            client.Player.ExpireVip = client.Player.ExpireVip.AddDays(1);
                        }
                        else
                            client.Player.ExpireVip = client.Player.ExpireVip.AddDays(1);

                        client.Player.VipLevel = 6;

                        client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                        client.Player.UpdateVip(stream);

                        client.CreateBoxDialog("You`ve received VIP6 (1 day).");

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 780010://2HourVIP
                    {
                        if (DateTime.Now > client.Player.ExpireVip)
                            client.Player.ExpireVip = DateTime.Now.AddHours(2);
                        else
                            client.Player.ExpireVip = client.Player.ExpireVip.AddHours(2);

                        client.Player.VipLevel = 6;

                        client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                        client.Player.UpdateVip(stream);
                        client.CreateBoxDialog("You`ve received vip6 (2 Hours).");


                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3309320:
                    {

                        if (DateTime.Now > client.Player.ExpireVip)
                            client.Player.ExpireVip = DateTime.Now.AddDays(30);
                        else
                            client.Player.ExpireVip = client.Player.ExpireVip.AddDays(30);

                        client.Player.VipLevel = 7;

                        client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                        client.Player.UpdateVip(stream);
                        client.CreateBoxDialog("You`ve received vip 7 (30 days).");


                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3303690:
                    {

                        client.CreateBoxDialog("You`ve received Full Chi Epic.");
                        client.Player.ChargeChi = 1;
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3303900:
                    {

                        client.CreateBoxDialog("You`ve received Full Jaing Epic.");
                        client.Player.ChargeJaing = 1;
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 720775:
                    {
                        if (client.Player.SubClass != null)
                        {
                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.SendSysMesage("You received 50 StudyPoints.");

                        }
                        break;
                    }
                case 723340:
                    {
                        if (client.Player.SubClass != null)
                        {
                            client.Player.SubClass.AddStudyPoints(client, 5, stream);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.SendSysMesage("You received 5 StudyPoints.");

                        }
                        break;
                    }
                case 723341:
                    {
                        if (client.Player.SubClass != null)
                        {
                            client.Player.SubClass.AddStudyPoints(client, 20, stream);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.SendSysMesage("You received 20 StudyPoints.");

                        }
                        break;
                    }
                case 723342:
                    {
                        if (client.Player.SubClass != null)
                        {
                            client.Player.SubClass.AddStudyPoints(client, 500, stream);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.SendSysMesage("You received 500 StudyPoints.");

                        }
                        break;
                    }
                case 3008188:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 200;
                        client.CreateBoxDialog("You~received~200~Chi~Points.");
                        break;
                    }
                case 729252://Vigor~Pill
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints += 10;
                        client.CreateBoxDialog("You~received~10~Chi~Points.");
                        break;
                    }
                case 3303519://KnowledgePillScrap
                    {
                        if (client.Inventory.Contain(3303519, 10))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                client.CreateBoxDialog("Please make 1 more space in your inventory");
                                break;
                            }
                            client.Inventory.Remove(3303519, 10, stream);
                            client.Inventory.Add(stream, 3303373, 1);
                            client.CreateBoxDialog("You~received~a~Knowledge~Pill~after~you~combined~10~Knowledge~Pill~Scraps!");
                        }
                        else
                        {
                            client.CreateBoxDialog("You~need~to~have~10~Knowledge~Pill~Scraps~to~combine~and~get~a~Knowledge~Pill.");
                        }
                        break;
                    }
                case 3006534:
                    {
                        client.Player.SubClass.AddStudyPoints(client, 10, stream);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.SendSysMesage("You~can~gain~10~study~points.");
                        break;
                    }
                case 3006535:
                    {
                        client.Player.SubClass.AddStudyPoints(client, 30, stream);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.SendSysMesage("You~can~gain~30~study~points.");
                        break;
                    }
                case 3006536:
                    {
                        client.Player.SubClass.AddStudyPoints(client, 50, stream);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.SendSysMesage("You~can~gain~50~study~points.");
                        break;
                    }
                case 728883:
                    {
                        client.Player.MessageBox("You~can~gain~50~study~points~from~this~Cordiality~Book.~Do~you~want~to~read~it,~now?"
                          , new Action<Client.GameClient>(p =>
                          {

                              using (var rec = new ServerSockets.RecycledPacket())
                              {
                                  var pac = rec.GetStream();
                                  if (p.Inventory.Remove(728883, 1, pac))
                                  {
                                      p.Player.SubClass.AddStudyPoints(client, 50, pac);
                                      p.Player.SendString(pac, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                      p.SendSysMesage("You read a Cordiality Book and received 50 study points!");
                                  }
                              }


                          }), null, 60);
                        break;
                    }
                case 3006537:
                    {
                        client.Player.MyChi.ChiPoints += 10;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.SendSysMesage("You~can~gain~10~chi~points.");
                        break;
                    }
                case 3006538:
                    {
                        client.Player.MyChi.ChiPoints += 30;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.SendSysMesage("You~can~gain~30~chi~points.");
                        break;
                    }
                case 3006539:
                    {
                        client.Player.MyChi.ChiPoints += 50;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.SendSysMesage("You~can~gain~50~chi~points.");
                        break;
                    }
                case 3001036:
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 1000;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        break;
                    }
                case 729242:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 500;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received 500 ChiPoints.");

                        break;
                    }




                case 3004580: //1000
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 1000;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received 1000 ChiPoints.");

                        break;
                    }





                case 3005722: //150
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received 150 ChiPoints.");

                        break;
                    }


                case 3005723: //250
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 250;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received 250 ChiPoints.");

                        break;
                    }

                case 3005724: //500
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 500;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received 500 ChiPoints.");

                        break;
                    }

                case 3000062: //60
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 60;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received 60 ChiPoints.");

                        break;
                    }


                case 3200706: //10000
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 10000;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received 10000 ChiPoints.");

                        break;
                    }


                case 3001034: //10000
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 10000;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received 10000 ChiPoints.");

                        break;
                    }


                case 3001035: //5000
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 5000;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received 5000 ChiPoints.");

                        break;
                    }


                case 3001037: //20
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 20;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        client.CreateBoxDialog("You received 202 ChiPoints.");

                        break;
                    }



                case 3200322:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~1250~minutes~of~EXP");

                            client.GainExpBall(12500, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~500~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 500, stream);

                        }


                        break;
                    }

                case 3200323:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~1125~minutes~of~EXP");

                            client.GainExpBall(11250, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~450~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 450, stream);

                        }


                        break;
                    }


                case 3200324:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~875~minutes~of~EXP");

                            client.GainExpBall(8750, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~350~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 350, stream);

                        }


                        break;
                    }



                case 3200325:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~500~minutes~of~EXP");

                            client.GainExpBall(5000, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~200~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 200, stream);

                        }


                        break;
                    }




                case 3200326:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~3000~minutes~of~EXP");

                            client.GainExpBall(30000, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~1500~Chi~Points.");
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 1500;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);

                        }

                        break;
                    }



                case 3200327:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~2700~minutes~of~EXP");

                            client.GainExpBall(27000, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~1350~Chi~Points.");
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 1350;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);

                        }


                        break;
                    }

                case 3200328:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~2100~minutes~of~EXP");

                            client.GainExpBall(21000, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~1050~Chi~Points.");
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 1050;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);

                        }


                        break;
                    }


                case 3200329:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~1200~minutes~of~EXP");

                            client.GainExpBall(12000, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~600~Chi~Points.");
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 600;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);

                        }


                        break;
                    }



                case 3200330:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~125~minutes~of~EXP");

                            client.GainExpBall(1250, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~50~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 50, stream);

                        }


                        break;
                    }


                case 3200331:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~250~minutes~of~EXP");

                            client.GainExpBall(2050, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~100~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 100, stream);

                        }


                        break;
                    }


                case 3200332:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~375~minutes~of~EXP");

                            client.GainExpBall(3750, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~150~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 150, stream);

                        }


                        break;
                    }


                case 3200333:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~150~minutes~of~EXP");

                            client.GainExpBall(1500, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~75~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 75, stream);

                        }


                        break;
                    }


                case 3200334:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~300~minutes~of~EXP");

                            client.GainExpBall(3000, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~150~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 150, stream);

                        }


                        break;
                    }

                case 3200335:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~450~minutes~of~EXP");

                            client.GainExpBall(4500, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~225~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 225, stream);

                        }


                        break;
                    }


                case 3200336:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~600~minutes~of~EXP");

                            client.GainExpBall(6000, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~300~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 300, stream);

                        }


                        break;
                    }

                case 3200337:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~300~minutes~of~EXP");

                            client.GainExpBall(3000, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~150~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 150, stream);

                        }


                        break;
                    }



                case 3200338:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~600~minutes~of~EXP");

                            client.GainExpBall(6000, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~300~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 300, stream);

                        }


                        break;
                    }




                case 3200339:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~900~minutes~of~EXP");

                            client.GainExpBall(9000, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~450~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 450, stream);

                        }


                        break;
                    }


                case 3200340:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~180~minutes~of~EXP");

                            client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~90~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 90, stream);

                        }


                        break;
                    }





                case 3200341:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~240~minutes~of~EXP");

                            client.GainExpBall(2400, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~120~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 120, stream);

                        }


                        break;
                    }

                case 3200344:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~30~minutes~of~EXP");

                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~15~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 15, stream);

                        }


                        break;
                    }

                case 3200345:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~90~minutes~of~EXP");

                            client.GainExpBall(900, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~45~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 45, stream);

                        }


                        break;
                    }

                case 3200346:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                        {
                            client.CreateBoxDialog("You received~120~minutes~of~EXP");

                            client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            client.CreateBoxDialog("You received~60~Study~Points.");
                            client.Player.SubClass.AddStudyPoints(client, 60, stream);

                        }


                        break;
                    }








                case 27832:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(723342, 0, 8, stream);
                            client.CreateBoxDialog("You received 8~Modesty~Books!");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }


                case 727832:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(723342, 0, 8, stream);
                            client.CreateBoxDialog("You received 8~Modesty~Books!");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }


                case 727833:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(723342, 0, 6, stream);
                            client.CreateBoxDialog("You received 6~Modesty~Books!");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }

                case 727834:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(723342, 0, 4, stream);
                            client.CreateBoxDialog("You received 4~Modesty~Books!");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }


                case 727831:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(723342, 0, 10, stream);
                            client.CreateBoxDialog("You received 10~Modesty~Books!");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }





                case 3001038:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.CreateBoxDialog("You received~10,000~Study~Points.");
                        client.Player.SubClass.AddStudyPoints(client, 10000, stream);
                        break;
                    }
                case 3600025:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level == 140)
                        {
                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                            client.SendSysMesage("You received 50 StudyPoints.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.GainExpBall(1000);
                            client.SendSysMesage("You received 100-min EXP!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 3600029:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.KingDomDeeds += 30;
                        client.Player.UpdateKingdomTreasury(30);
                        if (Role.Core.Rate(5))
                        {
                            client.Inventory.Add(stream, 3004181);
                        }
                        else
                        {
                            byte rand = (byte)Program.GetRandom.Next(1, 100);
                            if (rand < 30)
                            {
                                client.Inventory.Add(stream, 3008994);
                            }
                            else if (rand >= 30 && rand <= 60)
                            {
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 4000;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                client.CreateBoxDialog("You received 4000 ChiPoints.");
                            }
                            else if (rand > 60 && rand < 90)
                            {
                                client.Player.SubClass.AddStudyPoints(client, 300, stream);
                                client.CreateBoxDialog("You received 200 StudyPoints.");
                            }
                            else
                            {
                                client.Inventory.AddItemWitchStack(3003124, 0, 20, stream);
                                client.SendSysMesage("You received 20FavoredTrainingPils", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3007386:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(3006542, 0, 10, stream);
                        client.SendSysMesage("You received 10 Training Talent Packs!", MsgMessage.ChatMode.System);

                        break;
                    }
                case 3007284:
                    {
                        if (client.Player.MyJiangHu != null)
                        {

                            if (client.Player.MyJiangHu.FreeCourse <= 900000)
                            {
                                client.Player.MyJiangHu.FreeCourse += 100000;
                                client.SendSysMesage("You received 10 Free Courses!", MsgMessage.ChatMode.System);
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else
                            {
                                client.SendSysMesage("Your Free Courses have reached the upper limit of 90. You need to consume some, first!", MsgMessage.ChatMode.System);
                            }

                        }
                        else
                            client.SendSysMesage("First activate your JiangHu.", MsgMessage.ChatMode.System);
                        break;
                    }
                case 3006542:
                    {
                        if (client.Player.MyJiangHu != null)
                        {
                            if (client.Player.MyJiangHu.Talent < 4)
                            {
                                client.Inventory.Remove(3006542, 1, stream);
                                client.Player.MyJiangHu.Talent += 1;
                                client.Player.MyJiangHu.SendInfo(client, MsgJiangHuInfo.JiangMode.UpdateTalent, true, client.Player.UID.ToString(), client.Player.MyJiangHu.Talent.ToString());
                                client.SendSysMesage("You received 1 Talent for Jiang Hu Training!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.SendSysMesage("Failed to use this pack. Your Talent of Jiang Hu is full.", MsgMessage.ChatMode.System);
                            }
                        }
                        else
                        {
                            client.SendSysMesage("First activate your JiangHu.", MsgMessage.ChatMode.System);
                        }

                        break;
                    }
                case 3001046:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(10))
                        {
                            client.Inventory.Add(stream, 3001044, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, ".", 1);
                            client.SendSysMesage("You received a MysteryFruit", MsgMessage.ChatMode.System);
                            break;
                        }
                        byte rrand = (byte)Program.GetRandom.Next(1, 3);
                        switch (rrand)
                        {
                            case 1:
                                {
                                    client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 4000;
                                    Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                    client.CreateBoxDialog("You received 4000 ChiPoints.");
                                    break;
                                }
                            default:
                                {
                                    client.Player.SubClass.AddStudyPoints(client, 1000, stream);

                                    client.CreateBoxDialog("You received 1000 StudyPoints.");
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    handled = false;
                    break;
            }
        }
    }
}
