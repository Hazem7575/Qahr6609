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


        public static uint[] GetItemMr_MirrorEpicTrojanQuest()
        {
            switch (Program.GetRandom.Next(0, 17))
            {
                case 0: return new uint[2] { 729304, 2 };
                case 1: return new uint[2] { 711211, 2 };
                case 2: return new uint[2] { 721258, 2 };
                case 3: return new uint[2] { 729094, 100 };
                case 4: return new uint[2] { 729095, 100 };
                case 5: return new uint[2] { 729096, 100 };
                case 6: return new uint[2] { 723903, 20 };
                case 7: return new uint[2] { Database.ItemType.MoonBox, 1 };
                case 8: return new uint[2] { 723087, 3 };
                case 9: return new uint[2] { 721261, 5 };
                case 10: return new uint[2] { 3003125, 3 };
                case 11: return new uint[2] { Database.ItemType.MeteorScroll, 2 };
                case 12: return new uint[2] { 720598, 3 };
                case 13: return new uint[2] { 727061, 3 };
                case 14: return new uint[2] { 727060, 3 };
                case 15: return new uint[2] { 1200001, 2 };
                default:
                    return new uint[2] { 721259, 1 };
            }
        }
        public static uint[] GetItemArrayEpicTrojanQuest()
        {
            switch (Program.GetRandom.Next(0, 20))
            {
                case 0: return new uint[2] { 700012, 5 };
                case 1: return new uint[2] { 700022, 5 };
                case 2: return new uint[2] { 700032, 5 };
                case 3: return new uint[2] { 700042, 5 };
                case 4: return new uint[2] { 700052, 5 };
                case 5: return new uint[2] { 700062, 5 };
                case 6: return new uint[2] { 700072, 2 };
                case 7: return new uint[2] { 752003, 3 };
                case 8: return new uint[2] { Database.ItemType.MoonBox, 1 };
                case 9: return new uint[2] { 723903, 10 };
                case 10: return new uint[2] { Database.ItemType.MeteorScroll, 1 };
                case 11: return new uint[2] { 730001, 3 };
                case 12: return new uint[2] { 720842, 3 };
                case 13: return new uint[2] { 729090, 100 };
                case 14: return new uint[2] { 729091, 100 };
                case 15: return new uint[2] { 729092, 100 };
                case 16: return new uint[2] { 729093, 100 };
                case 17: return new uint[2] { 3003124, 1 };
                case 18: return new uint[2] { 723727, 2 };
                default: return new uint[2] { 720828, 3 };
            }
        }
        public static string GetArrayEpicTrojanItemStr(uint[] item)
        {
            string str = "";
            Database.ItemType.DBItem DBItem;
            if (Enum.IsDefined(typeof(Role.Flags.Gem), (byte)(item[0] % 1000)))
            {
                str += ((Role.Flags.Gem)(item[0] % 1000)).ToString() + "*" + item[1];
            }
            else if (Database.Server.ItemsBase.TryGetValue(item[0], out DBItem))
            {

                str += DBItem.Name + "*" + item[1];

            }
            return str;
        }
        public static uint[] GetItemEpicTrojanQuest()
        {
            switch (Program.GetRandom.Next(0, 15))
            {
                case 0:
                    return new uint[2] { Database.ItemType.Meteor, 5 };
                case 1:
                    return new uint[2] { Database.ItemType.DoubleExp, 10 };
                case 2:
                    return new uint[2] { Database.ItemType.ExpBall, 3 };
                case 3: return new uint[2] { 723341, 3 };
                case 4: return new uint[2] { 1200000, 3 };
                case 5: return new uint[2] { 727066, 3 };
                case 6: return new uint[2] { 727065, 3 };
                case 7: return new uint[2] { 727064, 3 };
                case 8: return new uint[2] { 727063, 3 };
                case 9: return new uint[2] { 727062, 3 };
                case 10: return new uint[2] { 727067, 3 };
                case 11: return new uint[2] { 727068, 3 };
                case 12: return new uint[2] { Database.ItemType.MeteorScroll, 1 };
                case 13: return new uint[2] { 1080001, 5 };
                default:
                    {
                        return new uint[2] { Database.ItemType.DragonBall, 1 };
                    }
                //to do: apeskin, heard snake birdclaw.
            }
        }
        public static string GetEpicTrojanItemStr(uint[] item)
        {
            string str = "";
            Database.ItemType.DBItem DBItem;
            if (Database.Server.ItemsBase.TryGetValue(item[0], out DBItem))
                str += DBItem.Name + "*" + item[1];
            return str;
        }


        [NpcAttribute(NpcID.MrMirrorMap2)]
        public static void MrMirrorMap2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The~sky~begins~to~brighten.~It`s~an~omen~of~good~fortune.");
                        data.AddOption("Great!");
                        data.AddAvatar(247);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MonkMisery1)]
        public static void MonkMisery1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You`re~in~the~Evil~Array.~The~magic~is~quite~weak,~now.~Listen,~I`ll~uncover~the~Array`s~Eye,~first.~So,~you~can~enchant~it.");
                        data.AddOption("Uncover~the~Array`s~Eye.", 1);
                        data.AddAvatar(248);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.EpicTrojanAbysalStage = 1;
                        client.Teleport(client.Player.X, client.Player.Y, 3826);
                        client.Player.AddMapEffect(stream, 39, 45, "zf2-e280");
                        client.Player.AddMapEffect(stream, 45, 45, "tj");
                        client.CreateBoxDialog("Monk~Misery~has~uncovered~the~Evil~Array`s~Eye.");

                        break;
                    }
            }
        }



        [NpcAttribute(NpcID.Epic2MonkMisery)]
        public static void Epic2MonkMisery(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3270);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3276);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.StageEpicTrojanQuest >= 20)
                        {

                            data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                            data.AddOption("Report~back~to~General~Pak.", 1);
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;

                            }
                            else
                            {
                                if (client.Player.EpicTrojanArrayItemChange[0] == 0)
                                    client.Player.EpicTrojanArrayItemChange = GetItemArrayEpicTrojanQuest();
                                if (client.Player.CanChangeArrayEpicMaterial == 1)
                                {
                                    data.AddText("Currently,~the~Evil~Array~is~holding~%stc(121,87)~points~of~magic.~It~needs~10,000~points~to~recover~the~seal.~I~can~enchant~it~once~a~day~with~450~points~of~magic.~It`ll~take~23~days~in~total.\n\n");
                                    data.AddText("Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~Exorcism~Token*1\n\n");
                                    data.AddOption("Enhance~it!", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                                    data.AddOption("Change~the~material.~(1~CP)", 10);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                            }


                            break;
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.ConquerPoints < 1)
                        {
                            data.AddText("You~don`t~have~enough~CPs~to~change~the~enchanting~materials.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 1;
                            client.Player.ChangeArrayEpicTrojan += 1;
                            client.Player.EpicTrojanArrayItemChange = GetItemArrayEpicTrojanQuest();
                            data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                            data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                            data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                            data.AddOption("Change~the~material.~(1~CP)", 10);
                            data.AddOption("Leave.", 100);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 100:
                    {
                        client.Teleport(88, 170, 3830);
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(client.Player.EpicTrojanArrayItemChange[0], client.Player.EpicTrojanArrayItemChange[1]) && client.Player.CanChangeArrayEpicMaterial == 1)
                        {
                            client.Player.CanChangeArrayEpicMaterial = 0;
                            if (!client.Inventory.Remove(client.Player.EpicTrojanArrayItemChange[0], client.Player.EpicTrojanArrayItemChange[1], stream)) break;
                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~450~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += 450;
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }

                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 045, 051, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }

                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");

                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");


                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");



                            }
                            else
                            {

                                if (client.Player.CanChangeArrayEpicMaterial == 1)
                                {
                                    data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~to~recover~the~seal.~I~can~enchant~it~once~a~day~with~450~points~of~magic.~It`ll~take~23~days~in~total.\n\n");
                                    data.AddText("Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~Exorcism~Token*1\n\n");
                                    data.AddOption("Enhance~it!", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                                    data.AddOption("Change~the~material.~(1~CP)", 10);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("You~don`t~have~enough~materials~to~enchant~the~Evil~Array.\n\n~~~~Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n");
                            data.AddOption("Alright.");
                            data.AddOption("Change~the~material.", 3);
                            data.AddOption("Entrust~the~work.", 4);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Are~you~sure~you~want~to~pay~18~CPs~to~entrust~me~with~the~whole~enchanting~work~of~the~Evil~Array?");
                        data.AddOption("Yes!~(18~CPs)", 4);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(248);
                        data.FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (client.Player.CanChangeArrayEpicMaterial == 0)
                            break;
                        if (client.Player.ConquerPoints < 18)
                        {
                            data.AddText("Sorry,~I~can`t~help~you~if~you~don`t~have~18~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {

                            client.Player.ConquerPoints -= 18;
                            client.Player.CanChangeArrayEpicMaterial = 0;
                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~450~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += 450;
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 045, 051, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else
                            {

                                if (client.Player.CanChangeArrayEpicMaterial == 1)
                                {
                                    data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~to~recover~the~seal.~I~can~enchant~it~once~a~day~with~450~points~of~magic.~It`ll~take~23~days~in~total.\n\n");
                                    data.AddText("Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~Exorcism~Token*1\n\n");
                                    data.AddOption("Enhance~it!", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                                    data.AddOption("Change~the~material.~(1~CP)", 10);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                            }


                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~of~magic~to~recover~the~seal.\n\n");
                        data.AddText("You~can~enchant~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic,~per~time.~If~you~enchant~the~array~10~times,~at~once,~you`ll~uncover~an~extra~Array`s~Eye.\n");
                        data.AddText("~The~added~Array`s~Eye~can~be~enchanted~with~" + Math.Min(300, GetMapEpicTrojanPoints(client.Player.Map) + 50) + "~points~of~magic,~per~time.\n\n");
                        data.AddOption("Enchant~it~1~time.~(160~CPs)", 21);
                        if (client.Player.EpicTrojanEvilArrayPoints < 8650)
                            data.AddOption("Enchant~it~10~time.~(1600~CPs)", 23);
                        data.AddOption("Leave", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~Do~you~want~to~pay~1600~CPs~to~enchant~the~array~with~" + GetMapEpicTrojanPoints(client.Player.Map) * 10 + "~points~of~magic?");
                        data.AddOption("Yes!~(1600~CPs)", 24);
                        data.AddOption("I~haven`t~decided,~yet!");
                        data.FinalizeDialog();
                        break;
                    }
                case 24:
                    {
                        if (client.Player.ConquerPoints < 1600)
                        {
                            data.AddText("Sorry,~I~can`t~help~you~if~you~don`t~have~1600~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {

                            client.Player.ConquerPoints -= 1600;

                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~" + GetMapEpicTrojanPoints(client.Player.Map) * 10 + "~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += (uint)GetMapEpicTrojanPoints(client.Player.Map) * 10;
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 045, 051, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else
                            {

                                if (client.Player.CanChangeArrayEpicMaterial == 1)
                                {
                                    data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~to~recover~the~seal.~I~can~enchant~it~once~a~day~with~450~points~of~magic.~It`ll~take~23~days~in~total.\n\n");
                                    data.AddText("Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~Exorcism~Token*1\n\n");
                                    data.AddOption("Enhance~it!", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                                    data.AddOption("Change~the~material.~(1~CP)", 10);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~Do~you~want~to~pay~160~CPs~to~enchant~the~array~with~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic?");
                        data.AddOption("Yes!~(160~CPs)", 22);
                        data.AddOption("I~haven`t~decided,~yet!");
                        data.FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (client.Player.ConquerPoints < 160)
                        {
                            data.AddText("Sorry,~I~can`t~help~you~if~you~don`t~have~160~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {

                            client.Player.ConquerPoints -= 160;

                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += (uint)GetMapEpicTrojanPoints(client.Player.Map);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 45, 51, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else
                            {

                                if (client.Player.CanChangeArrayEpicMaterial == 1)
                                {
                                    data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~to~recover~the~seal.~I~can~enchant~it~once~a~day~with~450~points~of~magic.~It`ll~take~23~days~in~total.\n\n");
                                    data.AddText("Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~Exorcism~Token*1\n\n");
                                    data.AddOption("Enhance~it!", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                                    data.AddOption("Change~the~material.~(1~CP)", 10);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        client.Teleport(88, 170, 3830);
                        break;
                    }
            }
        }
        public static int GetMapEpicTrojanPoints(uint MapID)
        {
            if (MapID == 3826)
                return 150;
            if (MapID == 3827)
                return 200;
            if (MapID == 3828)
                return 250;
            if (MapID == 3829)
                return 300;
            return 0;
        }


        public static void ArraysEye(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3270);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3276);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~of~magic~to~recover~the~seal.\n\n");
                        data.AddText("You~can~enchant~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic,~per~time.~If~you~enchant~the~array~10~times,~at~once,~you`ll~uncover~an~extra~Array`s~Eye.\n");
                        data.AddText("~The~added~Array`s~Eye~can~be~enchanted~with~" + Math.Min(300, GetMapEpicTrojanPoints(client.Player.Map) + 50) + "~points~of~magic,~per~time.\n\n");
                        data.AddOption("Enchant~it~1~time.~(160~CPs)", 21);
                        if (client.Player.EpicTrojanEvilArrayPoints < 8650)
                            data.AddOption("Enchant~it~10~time.~(1600~CPs)", 23);
                        data.AddOption("Leave", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(88, 170, 3830);
                        break;
                    }
                case 23:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~Do~you~want~to~pay~1600~CPs~to~enchant~the~array~with~" + GetMapEpicTrojanPoints(client.Player.Map) * 10 + "~points~of~magic?");
                        data.AddOption("Yes!~(1600~CPs)", 24);
                        data.AddOption("I~haven`t~decided,~yet!");
                        data.FinalizeDialog();
                        break;
                    }
                case 24:
                    {
                        if (client.Player.ConquerPoints < 1600)
                        {
                            data.AddText("Sorry,~I~can`t~help~you~if~you~don`t~have~1600~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {

                            client.Player.ConquerPoints -= 1600;

                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~" + GetMapEpicTrojanPoints(client.Player.Map) * 10 + "~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += (uint)GetMapEpicTrojanPoints(client.Player.Map) * 10;
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 045, 051, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else
                            {

                                data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~of~magic~to~recover~the~seal.\n\n");
                                data.AddText("You~can~enchant~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic,~per~time.~If~you~enchant~the~array~10~times,~at~once,~you`ll~uncover~an~extra~Array`s~Eye.\n");
                                data.AddText("~The~added~Array`s~Eye~can~be~enchanted~with~" + Math.Min(300, GetMapEpicTrojanPoints(client.Player.Map) + 50) + "~points~of~magic,~per~time.\n\n");
                                data.AddOption("Enchant~it~1~time.~(160~CPs)", 21);
                                if (client.Player.EpicTrojanEvilArrayPoints < 8650)
                                    data.AddOption("Enchant~it~10~time.~(1600~CPs)", 23);
                                data.AddOption("Leave", 255);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~Do~you~want~to~pay~160~CPs~to~enchant~the~array~with~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic?");
                        data.AddOption("Yes!~(160~CPs)", 22);
                        data.AddOption("I~haven`t~decided,~yet!");
                        data.FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (client.Player.ConquerPoints < 160)
                        {
                            data.AddText("Sorry,~I~can`t~help~you~if~you~don`t~have~160~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {

                            client.Player.ConquerPoints -= 160;

                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += (uint)GetMapEpicTrojanPoints(client.Player.Map);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 45, 51, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else
                            {
                                data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~of~magic~to~recover~the~seal.\n\n");
                                data.AddText("You~can~enchant~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic,~per~time.~If~you~enchant~the~array~10~times,~at~once,~you`ll~uncover~an~extra~Array`s~Eye.\n");
                                data.AddText("~The~added~Array`s~Eye~can~be~enchanted~with~" + Math.Min(300, GetMapEpicTrojanPoints(client.Player.Map) + 50) + "~points~of~magic,~per~time.\n\n");
                                data.AddOption("Enchant~it~1~time.~(160~CPs)", 21);
                                if (client.Player.EpicTrojanEvilArrayPoints < 8650)
                                    data.AddOption("Enchant~it~10~time.~(1600~CPs)", 23);
                                data.AddOption("Leave", 255);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MonkMisery)]
        public static void MonkMisery(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3270);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3276);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.StageEpicTrojanQuest >= 11)
                        {
                            if (client.Player.StageEpicTrojanQuest >= 20)
                            {
                                if (client.Player.StageEpicTrojanQuest >= 31)
                                {

                                    data.AddText("Ah,~how~dare~you~come~back~for~death!~I`ve~made~a~perfect~plan,~but~you~destroyed~it.");
                                    data.AddText("~I~won`t~let~you~go.");
                                    data.AddOption("Why~you~betrayed?", 3);
                                    data.AddAvatar(248);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("The~Evil~Array~is~getting~weak.~We~have~no~time~to~waste,~or~the~Flame~Devastator~will~break~through~the~array~and~return.");
                                    data.AddOption("I`ll~come~back~later.");
                                    data.AddAvatar(248);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                    client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                    client.Player.StageEpicTrojanQuest = 20;
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                    data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                    data.AddOption("Report~back~to~General~Pak.", 2);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("The~magic~of~the~Evil~Array~is~getting~weak.~We~need~to~enhance~its~power~before~it`s~too~late.~You~know,~the~Flame~Devastator~is~just~behind~the~seal.");
                                    data.AddOption("Enter~the~Evil~Array.", 1);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("The~Evil~Array~is~getting~weak.~We~have~no~time~to~waste,~or~the~Flame~Devastator~will~break~through~the~array~and~return.");
                            data.AddOption("I`ll~come~back~later.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Why?~For~the~power.~On~the~first~day~I~was~here,~the~Flame~Devastator~and~I~came~to~an~agreement.~I~nourished~him~with~creatures`~souls,");
                        data.AddText("~while~he~would~help~me~take~the~throne,~ruling~Twin~City,~even~the~whole~world.");
                        data.AddOption("Is~that~so~important?", 4);
                        data.AddAvatar(248);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Yeah,~I~need~power.~You~know,~it~was~me~who~taught~those~Ghost~Reavers~to~be~invisible.");
                        data.AddText("~I`m~the~best~spell~caster,~but~people~envied~me,~and~tried~to~kill~me~here.~I~have~to~disguise~myself~to~survive.");
                        data.AddText("~The~time~to~revenge~has~come!");
                        data.AddOption("You...", 5);
                        data.AddAvatar(248);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        client.Player.AddPick(stream, "MonkMisery(Transforming)", 1);
                        client.Player.MonkMiseryTransforming = 1;
                        client.SendSysMesage("With~a~chilling~roaring,~Monk~Misery~transformed~into~a~disgusting~monster.~Be~careful!");
                        foreach (var npc in client.Player.View.Roles(Role.MapObjectType.Npc))
                            if (npc.UID == id)
                            {
                                Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                                effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeLeftRight;
                                effect.DropType = MsgDropID.Earth;
                                effect.m_X = 82;
                                effect.m_Y = 166;
                                client.Send(stream.ItemPacketCreate(effect));
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, npc.UID, true, "ssch_wlhd_hit");
                            }
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
                case 1:
                    {
                        switch (client.Player.EpicTrojanAbysalStage)
                        {
                            case 0: client.Teleport(42, 51, 3825); break;
                            case 1: client.Teleport(42, 51, 3826); break;
                            case 2: client.Teleport(42, 51, 3827); break;
                            case 3: client.Teleport(42, 51, 3828); break;
                            case 4: client.Teleport(42, 51, 3829); break;
                        }
                        client.SendSysMesage("You`ve~entered~in~the~Evil~Array.", MsgMessage.ChatMode.System);
                        break;
                    }
            }

        }


        [NpcAttribute(NpcID.MrMirrorMap3)]
        public static void MrMirrorMap3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("It`s~the~chance~for~miracle.");
                        data.AddOption("Great!");
                        data.AddAvatar(247);
                        data.FinalizeDialog();
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.DivineJadeMap3)]
        public static void DivineJadeMap3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("This~clear~jade~glows~with~a~wonderful~light.~Infusing~the~Solar~Essences~into~this~jade~will~considerably~weaken~the~Flame~Devastator.");
                        if (client.Inventory.Contain(3003340, 1) && client.Player.StageEpicTrojanQuest >= 50)
                        {
                            data.AddOption("Enter~the~array.", 1);
                        }
                        else
                        {
                            data.AddOption("Leave.");
                        }
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.MessageBox("After~you~kill~the~Flame~Devastator,~your~universal~1-handed~weapon~will~turn~into~an~Epic~Weapon!", new Action<Client.GameClient>(p =>
                        {
                            var map = Database.Server.ServerMaps[3833];
                            p.Teleport(40, 40, 3833);
                            if (p.Player.StageEpicTrojanQuest == 50)
                            {
                                if (!map.ContainMobID(7485))
                                {
                                    Database.Server.AddMapMonster(stream, p.Map, 7485, p.Player.X, p.Player.Y, 3, 3, 1, p.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                                }
                                p.SendSysMesage("The~roaring~of~the~Flame~Devastator~make~the~earth~shaking.~Go~and~test~its~power!");
                            }
                            else if (p.Player.StageEpicTrojanQuest == 51)
                            {
                                if (!map.ContainMobID(7486))
                                {
                                    Database.Server.AddMapMonster(stream, p.Map, 7486, p.Player.X, p.Player.Y, 3, 3, 1, p.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                                }
                                p.SendSysMesage("The~roaring~of~the~Flame~Devastator~make~the~earth~shaking.~Go~and~test~its~power!");
                            }
                            else if (p.Player.StageEpicTrojanQuest == 52)
                            {
                                if (!map.ContainMobID(7487))
                                {
                                    Database.Server.AddMapMonster(stream, p.Map, 7487, p.Player.X, p.Player.Y, 3, 3, 1, p.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                                }
                            }

                        }), null);

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.GeneralPakMap3)]
        public static void GeneralPakMap3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap3, 0, 3280);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap3, 0, 3274);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.StageEpicTrojanQuest >= 50)
                        {
                            if (client.Inventory.Contain(3003340, 1))
                            {
                                data.AddText("It`s~the~end.~Though~the~Flame~Devastator~was~weakened,~it`ll~still~fierce~enough~to~smash~the~world.");
                                data.AddText("~Look,~you~need~awaken~the~Solar~Blade`s~spirit,~and~then~combine~the~spirit~with~your~main~weapon~(must~be~universal~one-handed)~to~create~an~Epic~Weapon.");
                                data.AddText("~With~the~power~of~the~Epic~Weapon,~we~may~behead~the~Flame~Devastator.");
                                data.AddOption("I`ll~take~care.", 2);
                                data.AddAvatar(245);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The~Flame~Devastator~has~been~weakened,~but~I`m~afraid~normal~weapons~can`t~wound~this~demon.");
                                data.AddText("~Take~my~Solar~Blade!~If~you`re~able~to~awaken~its~spirit,~and~combine~the~spirit~with~your~main~weapon~(must~be~universal~one-handed)");
                                data.AddText("~to~create~an~Epic~Weapon,~you`ll~achieve~a~divine~power.");
                                data.AddOption("I~see.", 3);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("What~a~miracle!~The~Solar~signed~a~blood~contract~with~you!~But~for~this~reason,~the~Flame~Devastator~can~be~eliminated.");
                            data.AddText("~Now,~only~you~can~summon~the~spirit~of~Solar~Blade.~From~now~on,~you`re~the~owner~of~this~Epic~Weapon!");
                            data.AddOption("Go~back~to~300~years~later.", 1);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId);
                            client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                            client.Inventory.Add(stream, 3003340);
                            client.Player.QuestGUI.SendAutoPatcher("The~Solar~Blade~is~already~in~your~inventory.~Make~good~use~of~it~to~eliminate~the~Flame~Devastator!", 3832, 101, 175, 10605);
                        }
                        else
                        {
                            data.AddText("You~inventory~is~full.~Make~some~room~for~the~Solar~Blade,~first.");
                            data.AddOption("I`ll~do~it~now.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3832, 101, 176, 10605);
                        break;
                    }
                case 1:
                    {
                        client.Teleport(273, 194, 1002);
                        client.CreateBoxDialog("You`re~in~Twin~City,~300~years~ago.");
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.DivineJadeMap2)]
        public static void DivineJadeMap2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap2, 0, 3279);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap3, 0, 3280);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This~clear~jade~glows~with~a~wonderful~light.~Infusing~the~Solar~Essences~into~this~jade~will~considerably~weaken~the~Flame~Devastator.");
                        data.AddOption("Use~the~Solar~Essences.", 1);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(3003338, 99))
                        {
                            client.Player.StageEpicTrojanQuest = 50;
                            client.Inventory.RemoveStackItem(3003338, 99, stream);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                            client.Teleport(client.Player.X, client.Player.Y, 3832);
                            client.Player.AddMapEffect(stream, 101, 175, "fire3");
                            client.Player.QuestGUI.SendAutoPatcher("You`ve~successfully~weakened~the~Flame~Devastator!~Hurry~and~report~back~to~General~Pack!", 3832, 154, 130, 10603);
                        }
                        else
                        {
                            data.AddText("You~should~have~99~Solar~Essences~before~you~can~weaken~the~Flame~Devastator.");
                            data.AddOption("I~see.", 2);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3831, 154, 130, 10600);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralPakMap2)]
        public static void GeneralPakMap2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap2, 0, 3273);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap2, 0, 3279);


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.StageEpicTrojanQuest >= 40)
                        {
                            if (client.Player.StageEpicTrojanQuest >= 41)
                            {
                                if (client.Inventory.Contain(3003338, 99))
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                    client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                    data.AddText("You`ve~extracted~99~Solar~Essences.~Hurry~and~go~weaken~the~Flame~Devastator~through~the~Divine~Jade!");
                                    data.AddOption("Head~to~the~Divine~Jade.", 40);
                                    data.FinalizeDialog();
                                    break;
                                }
                                if (client.Player.CanChangGeneralPakMaterial == 1)
                                {
                                    if (client.Player.EpicTrojanGeneralPakItemChange[0] == 0)
                                        client.Player.EpicTrojanGeneralPakItemChange = GetItemArrayEpicTrojanQuest();

                                    data.AddText("Trojan`s~blood~is~the~purest~solar~medium.~Combined~with~some~other~materials,~we~can~extract~the~Solar~Essences~from~the~blood.");
                                    data.AddOption("Extract~the~Solar~Essences.", 5);
                                    data.AddOption("Go~back~to~300~years~later.", 1);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                    data.AddOption("Extract~with~CPs.", 30);
                                    data.AddOption("Change~the~material.~(3~CPs)", 10);
                                    data.AddOption("Go~back~to~300~years~later.", 1);
                                    data.AddOption("I`ll~think~about~it.");
                                    data.FinalizeDialog();
                                }

                            }
                            else
                            {
                                data.AddText("Actually,~Monk~Misery~has~spent~too~much~energy~in~sacrificing~creature`s~souls~to~the~Flame~Devastator,~so~you~defeated~him~easily.");
                                data.AddText("~However,~he~sacrificed~his~own~soul~to~the~Devastator,~and~enabled~that~demon~to~break~through~the~array.");
                                data.AddOption("It`s~too~bad.", 3);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Hurry~and~go~defeat~Monk~Misery~at~the~Evil~Array!");
                            data.AddOption("I`m~on~the~way.", 1);
                            data.AddOption("Go~back~to~300~years~later.", 2);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.CanChangGeneralPakMaterial == 1)
                        {
                            data.AddText("Give~me~a~drop~of~Trojan`s~blood,~and~some~other~materials,~so~I~can~extract~4~Solar~Essences.~It`ll~take~25~days~to~extract~enough~essences.\n\n");
                            data.AddText("~~~~Extracting~Material:~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~200~Study~Points\n\n");
                            data.AddOption("Extract~Solar~Essences.", 6);
                            data.AddOption("Change~the~material.", 7);
                            data.AddOption("Leave.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(client.Player.EpicTrojanGeneralPakItemChange[0], client.Player.EpicTrojanGeneralPakItemChange[1]))
                        {
                            if (client.Inventory.HaveSpace(1) && client.Player.CanChangGeneralPakMaterial == 1)
                            {
                                client.Player.CanChangGeneralPakMaterial = 0;
                                if (!client.Inventory.Remove(client.Player.EpicTrojanGeneralPakItemChange[0], client.Player.EpicTrojanGeneralPakItemChange[1], stream)) break;
                                client.Inventory.AddItemWitchStack(3003338, 0, 4, stream);
                                client.CreateBoxDialog("You~successfully~extracted~4~Solar~Essences~from~Trojan`s~blood.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "meteor1");
                                if (client.Inventory.Contain(3003338, 99))
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                    client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                    data.AddText("You`ve~extracted~99~Solar~Essences.~Hurry~and~go~weaken~the~Flame~Devastator~through~the~Divine~Jade!");
                                    data.AddOption("Head~to~the~Divine~Jade.", 40);
                                    data.FinalizeDialog();
                                    break;
                                }
                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                data.AddOption("Extract~with~CPs.", 30);
                                data.AddOption("Change~the~material.~(3~CPs)", 10);
                                data.AddOption("Go~back~to~300~years~later.", 1);
                                data.AddOption("I`ll~think~about~it.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You`re~carrying~a~full~inventory.~Make~some~room,~first.");
                                data.AddOption("Okay.");
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("You~don`t~have~enough~materials~to~extract~the~Solar~Essences.\n\n~~~~Extracting~Material:~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + ",~200~Study~Points.");
                            data.AddOption("Alright.");
                            data.AddOption("Change~the~material.", 7);
                            data.AddOption("Entrust~the~work.", 8);
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        data.AddText("~~~~~~~~~~~~~~~~~~Extracting~the~Solar~Essences\n~~��������������������������������������������������\n");
                        data.AddText("~~~[Extracting~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~��������������������������������������������������\n\n");
                        data.AddOption("Extract~the~Solar~Essences.", 5);
                        data.AddOption("Change~the~material.~(3~CPs)", 10);
                        data.AddOption("Entrust~the~work.~(36~CPs)", 8);
                        data.AddOption("Leave.");
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.ConquerPoints < 3)
                        {
                            data.AddText("Buddy,~I~can`t~help~you~if~you~don`t~have~enough~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.ChangeGeneralPakEpicTrojan += 1;
                            client.Player.ConquerPoints -= 3;
                            client.Player.EpicTrojanGeneralPakItemChange = GetItemArrayEpicTrojanQuest();
                            if (client.Player.CanChangGeneralPakMaterial == 1)
                            {
                                data.AddText("Give~me~a~drop~of~Trojan`s~blood,~and~some~other~materials,~so~I~can~extract~4~Solar~Essences.~It`ll~take~25~days~to~extract~enough~essences.\n\n");
                                data.AddText("~~~~Extracting~Material:~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~200~Study~Points\n\n");
                                data.AddOption("Extract~Solar~Essences.", 6);
                                data.AddOption("Change~the~material.", 7);
                                data.AddOption("Leave.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                data.AddOption("Extract~with~CPs.", 30);
                                data.AddOption("Change~the~material.~(3~CPs)", 10);
                                data.AddOption("Go~back~to~300~years~later.", 1);
                                data.AddOption("I`ll~think~about~it.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        data.AddText("I`ll~charge~36~CPs~for~collecting~the~extracting~materials,~except~Trojan`s~blood.~So,~what~you~say?");
                        data.AddOption("Do~the~job~for~me!~(36~CPs)", 9);
                        data.AddOption("Wait,~I~haven`t~decided.");
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        if (client.Player.CanChangGeneralPakMaterial == 1)
                        {
                            if (client.Player.ConquerPoints < 36)
                            {
                                data.AddText("Buddy,~I~can`t~help~you~if~you~don`t~have~enough~CPs.");
                                data.AddOption("Alright.");
                                data.AddAvatar(245);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    client.Player.CanChangGeneralPakMaterial = 0;
                                    client.Player.ConquerPoints -= 36;
                                    client.Inventory.AddItemWitchStack(3003338, 0, 4, stream);
                                    client.CreateBoxDialog("You~successfully~extracted~4~Solar~Essences~from~Trojan`s~blood.");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "meteor1");
                                    if (client.Inventory.Contain(3003338, 99))
                                    {
                                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                        data.AddText("You`ve~extracted~99~Solar~Essences.~Hurry~and~go~weaken~the~Flame~Devastator~through~the~Divine~Jade!");
                                        data.AddOption("Head~to~the~Divine~Jade.", 40);
                                        data.FinalizeDialog();
                                        break;
                                    }
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                    data.AddOption("Extract~with~CPs.", 30);
                                    data.AddOption("Change~the~material.~(3~CPs)", 10);
                                    data.AddOption("Go~back~to~300~years~later.", 1);
                                    data.AddOption("I`ll~think~about~it.");
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You`re~carrying~a~full~inventory.~Make~some~room,~first.");
                                    data.AddOption("Okay.");
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("Currently,~you`ve~totally~extracted~" + client.Inventory.GetCountItem(3003338) + "~Solar~Essence(s).~You~need~to~collect~99~Solar~Essences~to~weaken~the~Flame~Devastator.");
                        data.AddOption("Extract~1~essence.~(90CPs)", 31);
                        if (client.Inventory.GetCountItem(3003338) < 82)
                            data.AddOption("Extract~18~essences.~(1600CPs)", 33);
                        else
                        {
                            data.AddOption("Leave.");
                            data.AddAvatar(245);
                        }
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("It`ll~cost~you~90~CPs~to~extract~a~Solar~Essence~directly~with~CPs.~Continue?");
                        data.AddOption("Yes.~Here~are~90~CPs.", 32);
                        data.AddOption("I~need~to~think~about~it.");
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        data.AddText("It`ll~cost~you~1,600~CPs~to~extract~18~Solar~Essences.~Are~you~sure~you~want~to~start~extracting~by~that~way?");
                        data.AddOption("Yes!~Here`re~1,600~CPs.", 34);
                        data.AddOption("Wait,~I~haven`t~decided.");
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 34:
                    {
                        if (client.Player.ConquerPoints < 1600)
                        {
                            data.AddText("Come~on,~make~sure~you~bring~enough~CPs~with~you!");
                            data.AddOption("Alright.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.ConquerPoints -= 1600;
                                client.CreateBoxDialog("You~successfully~extracted~18~Solar~Essence!");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "meteor1");


                                client.Inventory.AddItemWitchStack(3003338, 0, 18, stream);

                                if (client.Inventory.Contain(3003338, 99))
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                    client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                    data.AddText("You`ve~extracted~99~Solar~Essences.~Hurry~and~go~weaken~the~Flame~Devastator~through~the~Divine~Jade!");
                                    data.AddOption("Head~to~the~Divine~Jade.", 40);
                                    data.FinalizeDialog();
                                    break;
                                }

                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                data.AddOption("Extract~with~CPs.", 30);
                                data.AddOption("Change~the~material.~(3~CPs)", 10);
                                data.AddOption("Go~back~to~300~years~later.", 1);
                                data.AddOption("I`ll~think~about~it.");
                                data.FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Your~inventory~is~full.~Why~not~make~some~room,~and~then~come~back~to~extract~the~Solar~Essences?");
                                data.AddOption("Alright.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 32:
                    {
                        if (client.Player.ConquerPoints < 90)
                        {
                            data.AddText("Come~on,~make~sure~you~bring~enough~CPs~with~you!");
                            data.AddOption("Alright.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.ConquerPoints -= 90;
                                client.CreateBoxDialog("You~successfully~extracted~a~Solar~Essence!");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "meteor1");


                                client.Inventory.AddItemWitchStack(3003338, 0, 1, stream);

                                if (client.Inventory.Contain(3003338, 99))
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                    client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                    data.AddText("You`ve~extracted~99~Solar~Essences.~Hurry~and~go~weaken~the~Flame~Devastator~through~the~Divine~Jade!");
                                    data.AddOption("Head~to~the~Divine~Jade.", 40);
                                    data.FinalizeDialog();
                                    break;
                                }

                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                data.AddOption("Extract~with~CPs.", 30);
                                data.AddOption("Change~the~material.~(3~CPs)", 10);
                                data.AddOption("Go~back~to~300~years~later.", 1);
                                data.AddOption("I`ll~think~about~it.");
                                data.FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Your~inventory~is~full.~Why~not~make~some~room,~and~then~come~back~to~extract~the~Solar~Essences?");
                                data.AddOption("Alright.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 40:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3831, 101, 176, 10602);

                        break;
                    }
                case 3:
                    {
                        data.AddText("Look,~the~Flame~Devastator~hasn`t~completely~recovered.~I~guess~we~can~only~use~the~Solar~Essences~to~weaken~his~power.");
                        data.AddOption("Let`s~start!", 4);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.Player.StageEpicTrojanQuest = 41;
                        goto case 0;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(client.Player.Map, 082, 166, 0);
                        break;
                    }
                case 1:
                    {
                        client.Teleport(273, 194, 1002);
                        client.CreateBoxDialog("You`re~in~Twin~City,~300~years~ago.");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralPak)]
        public static void GeneralPak(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PaksGhost, 0, 3275);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3270);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3276);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3271);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3277);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MonkMisery, 0, 3272);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.StageEpicTrojanQuest >= 20)
                        {
                            if (client.Player.StageEpicTrojanQuest >= 21)
                            {
                                if (client.Player.StageEpicTrojanQuest >= 30)
                                {
                                    if (client.Player.StageEpicTrojanQuest >= 31)
                                    {

                                        data.AddText("It~seems~Monk~Misery~has~united~with~the~Flame~Devastator~for~a~long~time.~They~are~getting~fiery.");
                                        data.AddText("~I`m~afraid~they`ll~carry~out~the~plan~earlier.~See,~it`s~not~wise~to~deal~with~an~unknown~thing.");
                                        data.AddText("~Go~collect~possible~information~from~Monk~Misery,~first.");
                                        data.AddOption("I`ll~go~find~Monk~Misery.", 7);
                                        data.AddAvatar(245);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddText("How~could~those~Ghost~Reavers~escape~from~the~Evil~Array~when~you~were~enhancing~its~power?");
                                        data.AddText("~The~Monk~Misery~looked~quite~suspicious~when~you~were~killing~the~Ghost~Reavers.");
                                        data.AddText("~He~was~reciting~some~strange~spells~and~talking~to~the~Flame~Devastator.");
                                        data.AddOption("Did~him?", 5);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Though~the~Evil~Array~has~been~enhanced,~there~are~18~Ghost~Reavers~from~the~Flame~Devastator`s~army~sneaking~into~the~world.");
                                    data.AddText("~Be~careful,~those~Ghost~Reavers~are~trying~to~destroy~the~array.~They~should~die.");
                                    data.AddOption("How~to~find~them?", 3);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Though~the~Evil~Array~has~been~enhanced,~there~are~18~Ghost~Reavers~from~the~Flame~Devastator`s~army~sneaking~into~the~world.");
                                data.AddText("~Be~careful,~those~Ghost~Reavers~are~trying~to~destroy~the~array.~They~should~die.");
                                data.AddOption("How~to~find~them?", 3);
                                data.AddAvatar(245);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Years~ago,~the~Flame~Devastator,~a~vicious~demon,~terrorized~Twin~City.~At~that~time,~a~prominent~taoist~spent~out~his~power~and~built~the~Evil~Array");
                            data.AddText("~to~seal~the~gate~of~evils.~However,~with~time~goes~by,~the~Evil~Array~becomes~weak~and~the~sealed~Flame~Devastator~is~getting~unrest.");
                            data.AddText("~I`m~afraid~the~returning~of~the~Flame~Devastator~will~push~Twin~City~into~hell,~again.");
                            data.AddOption("What~can~we~do?", 1);
                            data.FinalizeDialog();
                        }
                        break;
                    }

                case 5:
                    {
                        data.AddText("What~a~shame!~I`m~the~General,~but~I~were~quite~unaware~of~that.~Anyway,~we~have~to~stop~them~no~matter~what~they~were~plotting.");
                        data.AddOption("Count~on~me!", 6);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("It~seems~Monk~Misery~has~united~with~the~Flame~Devastator~for~a~long~time.~They~are~getting~fiery.");
                        data.AddText("~I`m~afraid~they`ll~carry~out~the~plan~earlier.~See,~it`s~not~wise~to~deal~with~an~unknown~thing.");
                        data.AddText("~Go~collect~possible~information~from~Monk~Misery,~first.");
                        data.AddOption("I`ll~go~find~Monk~Misery.", 7);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        client.Player.StageEpicTrojanQuest = 31;
                        client.Player.QuestGUI.SendAutoPatcher(3830, 082, 166, 10583);
                        break;
                    }
                case 1:
                    {
                        data.AddText("Re-enchant~the~Evil~Array.~It`s~the~only~way.~Would~you~like~to~take~this~critical~task~of~enchanting~the~array?");
                        data.AddText("~Find~Monk~Misery~at~the~Evil~Array,~and~he`ll~tell~you~what~to~do.");
                        data.AddOption("Count~on~me!", 2);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(3830, 082, 166, (uint)NpcID.MonkMisery);
                        client.Player.StageEpicTrojanQuest = 11;
                        break;
                    }
                case 3:
                    {
                        data.AddText("Those~Ghost~Reaver~are~invisible~in~the~evil~gas.~I~heard~that~Mr.~Mirror~has~a~skill~to~reveal~those~demons.~Maybe,~you~can~ask~him~for~help.");
                        data.AddOption("Head~to~see~Mr.~Mirror.", 4);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.Player.StageEpicTrojanQuest = 21;
                        client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher(3830, 162, 217, (uint)NpcID.MrMirror);
                        break;
                    }

            }

        }

        [NpcAttribute(NpcID.MrMirror2)]
        public static void MrMirror2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            // This dialog is not bound to an NPC location; its teleports are refused on restricted maps.
            if (Program.BlockTeleportMap.Contains(client.Player.Map))
                return;
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3271);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3277);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.EpicTrojanMrMirrorPrograss >= 18)
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.Player.MessageBox("All~the~18~Ghost~Reavers~in~the~Flame~Devastator`s~army~have~been~eliminated.~Report~back~to~General~Pak!", new Action<Client.GameClient>(p =>
                            {
                                client.Player.StageEpicTrojanQuest = 30;
                                client.Teleport(162, 218, 3830);
                                client.Player.QuestGUI.SendAutoPatcher(3830, 154, 130, 10581);

                            }), null);
                            break;
                        }
                        if (client.Player.StageEpicTrojanQuest >= 30)
                        {
                            data.AddText("Good~job!~You`ve~eliminated~18~Ghost~Reaver~in~the~army~of~the~Flame~Devastator.~Hurry~and~report~back~to~General~Pak!");
                            data.AddOption("Head~to~see~General~Pak.", 1);
                            data.AddAvatar(247);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.View.ContainMobInScreen("GhostReaver"))
                            {
                                data.AddText("The~Ghost~Reaver~has~already~appeared~in~the~Unreal~Land~of~Twin~City.~Go~eliminate~it,~first.");
                                data.AddOption("Okay.");
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.EpicTrijanKillGhostReaver == 0)
                            {

                                if (client.Player.EpicTrojanMr_MirrorItemChange[0] == 0)
                                    client.Player.EpicTrojanMr_MirrorItemChange = GetItemMr_MirrorEpicTrojanQuest();


                                data.AddText("So~far,~there~are~" + (18 - client.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reavers~hiding~in~the~evil~gas.~You~need~to~provide~me~the~materials~below~to~reveal~the~Ghost~Reaver.\n\n");
                                data.AddText("~~~~Material~Required:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~Chi~Token*1\n\n");
                                if (client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                                    data.AddOption("Reveal~the~Reaver.", 2);
                                data.AddOption("Change~the~material.", 3);
                                data.FinalizeDialog();

                            }
                            else
                            {
                                if (client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                                {
                                    data.AddText("The~Ghost~Reaver~hid~into~the~evil~gas,~again.~I`ll~reveal~another~Ghost~Reaver.~Seize~the~chance~and~beat~it~down!");
                                    data.AddOption("Just~do~it!", 20);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~~You~can~also~use~320~CPs~to~call~down~a~thunder~to~kill~a~Ghost~Reaver~directly.");
                                    data.AddText("~So~far,~there~are~still~" + (18 - client.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reavers~alive.\n\n~~------------------------------------------------------\n~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~~Chi~Token*1\n~~~[Change~Times]~" + client.Player.ChangeMr_MirrorEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Call~down~a~thunder.~(320~CPs)", 30);
                                    data.AddOption("Change~the~material.~(2~CPs)", 40);
                                    data.AddOption("I`ll~wait~till~tomorrow.", 50);
                                    data.FinalizeDialog();
                                }
                            }

                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1]) && client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                        {
                            client.Player.CanChangeMr_MirrorEpicMaterial = 0;
                            if (!client.Inventory.Remove(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1], stream)) break;
                            client.CreateBoxDialog("With~a~roaring~of~thunder,~the~Ghost~Reaver~fell~upon~the~unreal~space~of~Twin~City.~Go~kill~the~demon!");
                            Database.Server.AddMapMonster(stream, client.Map, 7483, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                        }
                        else
                        {
                            data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~~You~can~also~use~320~CPs~to~call~down~a~thunder~to~kill~a~Ghost~Reaver~directly.");
                            data.AddText("~So~far,~there~are~still~" + (18 - client.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reavers~alive.\n\n~~------------------------------------------------------\n~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~~Chi~Token*1\n~~~[Change~Times]~" + client.Player.ChangeMr_MirrorEpicTrojan + "\n~~------------------------------------------------------\n\n");
                            data.AddOption("Call~down~a~thunder.~(320~CPs)", 30);
                            data.AddOption("Change~the~material.~(2~CPs)", 40);
                            data.AddOption("I`ll~wait~till~tomorrow.", 50);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 40:
                    {
                        if (client.Player.ConquerPoints < 2)
                        {
                            data.AddText("You~don`t~have~enough~CPs~to~change~the~materials.");
                            data.AddOption("Alright.");
                            data.AddAvatar(247);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.ChangeMr_MirrorEpicTrojan++;

                            client.Player.ConquerPoints -= 2;
                            client.Player.EpicTrojanMr_MirrorItemChange = GetItemMr_MirrorEpicTrojanQuest();
                            if (client.Inventory.Contain(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1]) && client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                            {
                                client.Player.CanChangeMr_MirrorEpicMaterial = 0;
                                if (!client.Inventory.Remove(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1], stream)) break;
                                client.CreateBoxDialog("With~a~roaring~of~thunder,~the~Ghost~Reaver~fell~upon~the~unreal~space~of~Twin~City.~Go~kill~the~demon!");
                                Database.Server.AddMapMonster(stream, client.Map, 7483, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");

                            }
                            else
                            {
                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~~You~can~also~use~320~CPs~to~call~down~a~thunder~to~kill~a~Ghost~Reaver~directly.");
                                data.AddText("~So~far,~there~are~still~" + (18 - client.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reavers~alive.\n\n~~------------------------------------------------------\n~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~~Chi~Token*1\n~~~[Change~Times]~" + client.Player.ChangeMr_MirrorEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                data.AddOption("Call~down~a~thunder.~(320~CPs)", 30);
                                data.AddOption("Change~the~material.~(2~CPs)", 40);
                                data.AddOption("I`ll~wait~till~tomorrow.", 50);
                                data.FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 30:
                    {
                        data.AddText("Are~you~sure~you~want~to~pay~320~CPs~to~call~down~a~thunder~to~kill~a~Ghost~Reaver?");
                        data.AddOption("Yes,~I`ve~decided.~(320~CPs)", 31);
                        data.AddOption("Wait,~I~haven`t~decided,~yet.");
                        data.AddAvatar(247);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (client.Player.ConquerPoints < 320)
                        {
                            data.AddText("I`m~afraid~I~can`t~help~you~if~you~don`t~have~27~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(247);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 320;
                            client.Player.EpicTrojanMrMirrorPrograss += 1;

                            MsgServer.MsgGameItem item = new MsgServer.MsgGameItem();
                            item.Color = (Role.Flags.Color)2;
                            item.ITEM_ID = 1033;
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, 163, 228, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                   , 0, false, client.Map, 6);

                            if (client.Map.EnqueueItem(DropItem))
                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);

                            if (client.Player.EpicTrojanMrMirrorPrograss >= 18)
                            {
                                client.Player.StageEpicTrojanQuest = 30;
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.MessageBox("All~the~18~Ghost~Reavers~in~the~Flame~Devastator`s~army~have~been~eliminated.~Report~back~to~General~Pak!", new Action<Client.GameClient>(p =>
                                {


                                    client.Teleport(162, 218, 3830);
                                    client.Player.QuestGUI.SendAutoPatcher(3830, 154, 130, 10581);

                                }), null);
                            }
                            else
                            {
                                client.Player.MessageBox("A~Ghost~Reaver~fell~down~and~vanished.~So~far,~you~still~have~" + (18 - client.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reaver(s)~to~deal~with.", new Action<Client.GameClient>(p =>
                                {

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var pstream = rec.GetStream();
                                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.MrMirror2;
                                        Game.MsgNpc.NpcHandler.MrMirror2(client, pstream, 0, "", 0);
                                    }
                                }), null);
                            }


                        }
                        break;
                    }
                case 50:
                    {
                        client.Teleport(162, 218, 3830);
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1]) && client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                        {
                            client.Player.CanChangeMr_MirrorEpicMaterial = 0;
                            if (!client.Inventory.Remove(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1], stream)) break;
                            client.CreateBoxDialog("With~a~roaring~of~thunder,~the~Ghost~Reaver~fell~upon~the~unreal~space~of~Twin~City.~Go~kill~the~demon!");
                            Database.Server.AddMapMonster(stream, client.Map, 7483, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            foreach (var mob in client.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                client.Player.AddMapEffect(stream, mob.X, mob.Y, "SuperXp - 4");
                            }
                        }
                        else
                        {
                            data.AddText("You~don`t~have~enough~materials~to~reveal~the~Ghost~Reaver.\n\n~~~~Material~Required:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + ",~Chi~Token*1\n");
                            data.AddOption("Alright.");
                            data.AddOption("Change~the~material.", 3);
                            data.AddOption("Reveal~the~Reaver~immediately.", 4);
                            data.AddAvatar(247);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("~~~~~~~~~~~~~~~~~~~~Reaver~Elimination\n~~����������������������������������������������������\n");
                        data.AddText("~~~[Spell~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~~Chi~Token*1\n~~~[Change~Times]~" + client.Player.ChangeMr_MirrorEpicTrojan + "\n~~����������������������������������������������������\n\n");
                        if (client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                            data.AddOption("Reveal~the~Reaver.", 2);
                        data.AddOption("Change~the~material.~(2~CPs)", 10);
                        data.AddOption("Immediately~reveal.~(27~CPs)", 4);
                        data.AddOption("Leave.");
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.ConquerPoints < 2)
                        {
                            data.AddText("You~don`t~have~enough~CPs~to~change~the~materials.");
                            data.AddOption("Alright.");
                            data.AddAvatar(247);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 2;
                            client.Player.ChangeMr_MirrorEpicTrojan++;
                            client.Player.EpicTrojanMr_MirrorItemChange = GetItemMr_MirrorEpicTrojanQuest();
                            data.AddText("~~~~~~~~~~~~~~~~~~~~Reaver~Elimination\n~~����������������������������������������������������\n");
                            data.AddText("~~~[Spell~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~~Chi~Token*1\n~~~[Change~Times]~" + client.Player.ChangeMr_MirrorEpicTrojan + "\n~~����������������������������������������������������\n\n");
                            if (client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                                data.AddOption("Reveal~the~Reaver.", 2);
                            data.AddOption("Change~the~material.~(2~CPs)", 10);

                            data.AddOption("Immediately~reveal.~(27~CPs)", 4);
                            data.AddOption("Leave.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("Are~you~sure~you~want~to~pay~27~CPs~to~immediately~reveal~the~Ghost~Reaver?");
                        data.AddOption("Yes,~I`ve~decided.~(27~CPs)", 5);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(247);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                        {
                            if (client.Player.ConquerPoints < 27)
                            {
                                data.AddText("I`m~afraid~I~can`t~help~you~if~you~don`t~have~27~CPs.");
                                data.AddOption("Alright.");
                                data.AddAvatar(247);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.CanChangeMr_MirrorEpicMaterial = 0;
                                client.Player.ConquerPoints -= 27;
                                client.CreateBoxDialog("With~a~roaring~of~thunder,~the~Ghost~Reaver~fell~upon~the~unreal~space~of~Twin~City.~Go~kill~the~demon!");
                                Database.Server.AddMapMonster(stream, client.Map, 7483, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");

                            }
                        }
                        break;
                    }
                case 1:
                    {
                        client.Teleport(162, 218, 3830);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MrMirror)]
        public static void MrMirror(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.StageEpicTrojanQuest >= 21)
                        {
                            if (client.Player.StageEpicTrojanQuest >= 30)
                            {
                                data.AddText("You`ve~eliminated~all~the~18~Ghost~Reavers.~Hurry~and~report~back~to~General~Pak!");
                                data.AddOption("Head~to~see~General~Pak.", 2);
                                data.AddAvatar(247);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The~evil~gas~is~going~to~swallow~Twin~City.~You~can~probably~find~those~Ghost~Reaver~in~the~Unreal~Land~that~I~created.");
                                data.AddOption("Enter~the~Unreal~Land.", 1);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("9~stars~are~going~to~be~on~one~line.~Is~it~an~omen~of~blessing~or~misery?");
                            data.AddOption("Who~knows.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        client.Teleport(163, 223, 3834);
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3830, 154, 130, 10581);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SuperMok)]
        public static void SuperMok(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PaksGhost, 0, 3269);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SuperMok, 0, 3278);
            Dialog data = new Dialog(client, stream);
            if (client.Player.StageEpicTrojanQuest != 1)
            {
                data.AddText("Alas...~Father~always~forces~me~to~recite~formulas~of~forging.~They`re~useless.~You~don`t~know~what~I`m~talking~about,~right?");
                data.AddOption("No~idea.");
                data.AddAvatar(246);
                data.FinalizeDialog();
                return;
            }
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class >= 20)
                        {
                            data.AddText("Alas...~Father~always~forces~me~to~recite~formulas~of~forging.~They`re~useless.~You~don`t~know~what~I`m~talking~about,~right?");
                            data.AddOption("No~idea.");
                            data.AddAvatar(246);
                            data.FinalizeDialog();
                        }
                        else
                        {

                            if (client.Inventory.Contain(3003336, 27))
                            {
                                data.AddText("Wow,~you`ve~collected~27~Solar~Scraps!~Hurry~and~help~General~Pak~reforge~the~Solar~Blade!");
                                data.AddOption("Head~to~Pak`s~Ghost.", 3);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.EpicTrojanItemChange[0] == 0)
                                    client.Player.EpicTrojanItemChange = GetItemEpicTrojanQuest();
                                if (client.Player.CanChangeEpicMaterial == 1)
                                {
                                    data.AddText("Yeah,~I~know~the~secret~of~Solar~power.~It`s~quite~complicated,~so~I~only~forge~1~Solar~Scrap~in~a~day.~It`ll~take~27~days~in~total~to~forge~enough~scraps.\n\n");
                                    data.AddText("~The~material:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n\n");
                                    data.AddOption("Forge~a~Solar~Scrap.", 2);
                                    data.AddOption("Change~the~materials.", 4);
                                    data.AddOption("I~see.");
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.");
                                    data.AddText("~Would~you~like~to~buy~some~Energies?~\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n~~~[Change~Times]~" + client.Player.ChangeEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Yes,~please.", 10);
                                    data.AddOption("Change~the~material.~(1~CP)", 20);
                                    data.AddOption("Leave.");
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1002, 267, 194, (uint)NpcID.PaksGhost);
                        break;
                    }

                case 2:
                    {
                        if (client.Inventory.Contain(client.Player.EpicTrojanItemChange[0], client.Player.EpicTrojanItemChange[1]) && client.Player.CanChangeEpicMaterial == 1)
                        {
                            client.Player.CanChangeEpicMaterial = 0;
                            client.Player.EpicTrojanItemChange = GetItemEpicTrojanQuest();
                            if (!client.Inventory.Remove(client.Player.EpicTrojanItemChange[0], client.Player.EpicTrojanItemChange[1], stream)) break;
                            client.Inventory.AddItemWitchStack(3003336, 0, 1, stream);
                            client.CreateBoxDialog("You~received~a~Solar~Scrap!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e280");
                            if (client.Inventory.Contain(3003336, 27))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                                data.AddText("Wow,~you`ve~collected~27~Solar~Scraps!~Hurry~and~help~General~Pak~reforge~the~Solar~Blade!");
                                data.AddOption("Head~to~Pak`s~Ghost.", 3);
                                data.AddAvatar(246);
                                data.FinalizeDialog();

                            }
                            else
                            {
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                                {
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperCyclone);
                                    client.Player.XPCount = 100;
                                    client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Super~Cyclone,~from~the~Solar~Scrap!");

                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                                {
                                    if (client.Inventory.Contain(3003336, 7))
                                    {
                                        client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Breath~Focus,~from~7~Solar~Scraps!");
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BreathFocus);
                                    }
                                }

                                if (client.Player.CanChangeEpicMaterial == 1)
                                {
                                    data.AddText("Yeah,~I~know~the~secret~of~Solar~power.~It`s~quite~complicated,~so~I~only~forge~1~Solar~Scrap~in~a~day.~It`ll~take~27~days~in~total~to~forge~enough~scraps.\n\n");
                                    data.AddText("~The~material:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n\n");
                                    data.AddOption("Forge~a~Solar~Scrap.", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("I~see.");
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.");
                                    data.AddText("~Would~you~like~to~buy~some~Energies?~\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n~~~[Change~Times]~" + client.Player.ChangeEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Yes,~please.", 10);
                                    data.AddOption("Change~the~material.~(1~CP)", 20);
                                    data.AddOption("Leave.");
                                    data.FinalizeDialog();
                                }

                            }


                        }
                        else
                        {
                            data.AddText("You~don`t~have~enough~materials~to~forge~the~Solar~Scrap!\n\n~~~~Forging~Material:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n");
                            data.AddOption("Alright.");
                            data.AddOption("Change~the~material.", 4);
                            data.AddOption("Entrust~forging~work.", 5);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("~~~~~~~~~~~~~~~~~~~Forging~Solar~Scraps\n~~������������������������������������������������������\n");
                        data.AddText("~~~[Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n~~~[Change~Times]~" + client.Player.ChangeEpicTrojan + "\n~~������������������������������������������������������\n\n");
                        data.AddOption("Forge~a~Solar~Scrap.", 2);
                        data.AddOption("Change~the~materials.~(1~CP)", 20);
                        data.AddOption("Entrust~with~the~forging.~(12~CPs).", 5);
                        data.AddOption("I~see.");
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("It`ll~cost~you~12~CPs~to~entrust~me~with~the~whole~forging~work~of~Solar~Scrap.~So,~start~forging~the~Solar~Scrap?");
                        data.AddOption("Yes.~(12~CPs)", 6);
                        data.AddOption("Wait,~I~haven`t~decided.");
                        data.AddAvatar(246);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.ConquerPoints < 12)
                        {
                            data.AddText("It~really~costs~a~lot~in~forging~the~Solar~Scrap.~Make~sure~you~have~enough~CPs~with~you.");
                            data.AddOption("Okay.");
                            data.AddAvatar(246);
                            data.FinalizeDialog();
                            break;
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1) && client.Player.CanChangeEpicMaterial == 1)
                            {
                                client.Player.EpicTrojanItemChange = GetItemEpicTrojanQuest();
                                client.Player.CanChangeEpicMaterial = 0;
                                client.Player.ConquerPoints -= 12;
                                client.Inventory.AddItemWitchStack(3003336, 0, 1, stream);
                                client.CreateBoxDialog("You~received~a~Solar~Scrap!");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e280");

                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                                {
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperCyclone);
                                    client.Player.XPCount = 100;
                                    client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Super~Cyclone,~from~the~Solar~Scrap!");

                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                                {
                                    if (client.Inventory.Contain(3003336, 7))
                                    {
                                        client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Breath~Focus,~from~7~Solar~Scraps!");
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BreathFocus);
                                    }
                                }

                                if (client.Player.CanChangeEpicMaterial == 1)
                                {
                                    data.AddText("Yeah,~I~know~the~secret~of~Solar~power.~It`s~quite~complicated,~so~I~only~forge~1~Solar~Scrap~in~a~day.~It`ll~take~27~days~in~total~to~forge~enough~scraps.\n\n");
                                    data.AddText("~The~material:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n\n");
                                    data.AddOption("Forge~a~Solar~Scrap.", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("I~see.");
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.");
                                    data.AddText("~Would~you~like~to~buy~some~Energies?~\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n~~~[Change~Times]~" + client.Player.ChangeEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Yes,~please.", 10);
                                    data.AddOption("Change~the~material.~(1~CP)", 20);
                                    data.AddOption("Leave.");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("I~can`t~put~anything~into~a~full~bag.~Make~some~room~in~your~inventory~for~the~Solar~Scrap.");
                                data.AddOption("I`ll~do~it~now.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Player.ConquerPoints < 1)
                        {
                            data.AddText("You~don`t~have~enough~CPs~to~refresh~the~materials.");
                            data.AddOption("Alright.");
                            data.AddAvatar(246);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 1;
                            client.Player.ChangeEpicTrojan += 1;
                            client.Player.EpicTrojanItemChange = GetItemEpicTrojanQuest();
                            if (client.Player.CanChangeEpicMaterial == 1)
                            {
                                data.AddText("Yeah,~I~know~the~secret~of~Solar~power.~It`s~quite~complicated,~so~I~only~forge~1~Solar~Scrap~in~a~day.~It`ll~take~27~days~in~total~to~forge~enough~scraps.\n\n");
                                data.AddText("~The~material:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n\n");
                                data.AddOption("Forge~a~Solar~Scrap.", 2);
                                data.AddOption("Change~the~materials.", 3);
                                data.AddOption("I~see.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.");
                                data.AddText("~Would~you~like~to~buy~some~Energies?~\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n~~~[Change~Times]~" + client.Player.ChangeEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                data.AddOption("Yes,~please.", 10);
                                data.AddOption("Change~the~material.~(1~CP)", 20);
                                data.AddOption("Leave.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3003335, 1))
                        {
                            if (client.Inventory.Contain(3003336, 27))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                                data.AddText("Wow,~you`ve~collected~27~Solar~Scraps!~Hurry~and~help~General~Pak~reforge~the~Solar~Blade!");
                                data.AddOption("Head~to~Pak`s~Ghost.", 3);
                                data.AddAvatar(246);
                                data.FinalizeDialog();

                            }
                            else
                            {
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                                {
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperCyclone);
                                    client.Player.XPCount = 100;
                                    client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Super~Cyclone,~from~the~Solar~Scrap!");

                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                                {
                                    if (client.Inventory.Contain(3003336, 7))
                                    {
                                        client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Breath~Focus,~from~7~Solar~Scraps!");
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BreathFocus);
                                    }
                                }

                                data.AddText("So~far,~you`ve~forged~and~absorbed~" + client.Inventory.GetCountItem(3003336) + "~Solar~Scrap(s).~You~need~to~collect~27~scraps~to~reforge~the~Solar~Blade.");
                                data.AddText("~Each~time,~you~can~only~forge~1~Solar~Scrap.~So,~how~many~Energies~would~you~like~to~buy?");
                                data.AddOption("1~Energy~and~complete~the~quest.(300~CPs)", 11);
                                if (client.Inventory.GetCountItem(3003336) < 22)
                                    data.AddOption("6~Energies~and~complete~the~quest.(1800~CPs)", 13);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            client.CreateBoxDialog("You~don`t~have~the~Solar~Blade`s~Remain.~Go~claim~that~from~General~Pak!");
                        }
                        break;
                    }
                case 13:
                    {
                        data.AddText("Are~you~sure~you~want~to~pay~1,800~CPs~to~absorb~6~Solar~Scraps?");
                        data.AddOption("6~Energies~and~complete~the~quest.(1800~CPs)", 14);
                        data.AddOption("Wait,~I~haven`t~decided.");
                        data.AddAvatar(246);
                        data.FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.Contain(3003335, 1))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.ConquerPoints < 1800)
                                {
                                    client.CreateBoxDialog("You~don`t~have~enough~CPs~to~activate~the~power~of~the~Solar~Blade`s~Remain.");
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 1800;
                                    client.Inventory.AddItemWitchStack(3003336, 0, 6, stream);
                                    client.SendSysMesage("You~successfully~activated~the~power~of~the~Solar~Blade`s~Remain,~and~obtained~6~Solar~Scrap!", MsgMessage.ChatMode.System);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "Attack35r");

                                    if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                                    {
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperCyclone);
                                        client.Player.XPCount = 100;
                                        client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Super~Cyclone,~from~the~Solar~Scrap!");

                                    }
                                    if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                                    {
                                        if (client.Inventory.Contain(3003336, 7))
                                        {
                                            client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Breath~Focus,~from~7~Solar~Scraps!");
                                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BreathFocus);
                                        }
                                    }

                                    if (client.Inventory.Contain(3003336, 27))
                                    {
                                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                                        data.AddText("Wow,~you`ve~collected~27~Solar~Scraps!~Hurry~and~help~General~Pak~reforge~the~Solar~Blade!");
                                        data.AddOption("Head~to~Pak`s~Ghost.", 3);
                                        data.AddAvatar(246);
                                        data.FinalizeDialog();

                                    }
                                    else
                                    {
                                        data.AddText("So~far,~you`ve~forged~and~absorbed~" + client.Inventory.GetCountItem(3003336) + "~Solar~Scrap(s).~You~need~to~collect~27~scraps~to~reforge~the~Solar~Blade.");
                                        data.AddText("~Each~time,~you~can~only~forge~1~Solar~Scrap.~So,~how~many~Energies~would~you~like~to~buy?");
                                        data.AddOption("1~Energy~and~complete~the~quest.(300~CPs)", 11);
                                        if (client.Inventory.GetCountItem(3003336) < 22)
                                            data.AddOption("6~Energies~and~complete~the~quest.(1800~CPs)", 13);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                                client.CreateBoxDialog("Your~inventory~is~full.~You~need~to~make~some~room,~first.");
                        }
                        else
                            client.CreateBoxDialog("You~don`t~have~the~Solar~Blade`s~Remain.~Go~claim~that~from~General~Pak!");
                        break;
                    }
                case 11:
                    {
                        data.AddText("Are~you~sure~you~want~to~pay~300~CPs~to~absorb~1~Solar~Scrap?");
                        data.AddOption("1~Energy~and~complete~the~quest.(300~CPs)", 12);
                        data.AddOption("Wait,~I~haven`t~decided.");
                        data.AddAvatar(246);
                        data.FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(3003335, 1))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.ConquerPoints < 300)
                                {
                                    client.CreateBoxDialog("You~don`t~have~enough~CPs~to~activate~the~power~of~the~Solar~Blade`s~Remain.");
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 300;
                                    client.Inventory.AddItemWitchStack(3003336, 0, 1, stream);
                                    client.SendSysMesage("You~successfully~activated~the~power~of~the~Solar~Blade`s~Remain,~and~obtained~1~Solar~Scrap!", MsgMessage.ChatMode.System);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "Attack35r");

                                    if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                                    {
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperCyclone);
                                        client.Player.XPCount = 100;
                                        client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Super~Cyclone,~from~the~Solar~Scrap!");

                                    }
                                    if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                                    {
                                        if (client.Inventory.Contain(3003336, 7))
                                        {
                                            client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Breath~Focus,~from~7~Solar~Scraps!");
                                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BreathFocus);
                                        }
                                    }

                                    if (client.Inventory.Contain(3003336, 27))
                                    {
                                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                                        data.AddText("Wow,~you`ve~collected~27~Solar~Scraps!~Hurry~and~help~General~Pak~reforge~the~Solar~Blade!");
                                        data.AddOption("Head~to~Pak`s~Ghost.", 3);
                                        data.AddAvatar(246);
                                        data.FinalizeDialog();

                                    }
                                    else
                                    {
                                        data.AddText("So~far,~you`ve~forged~and~absorbed~" + client.Inventory.GetCountItem(3003336) + "~Solar~Scrap(s).~You~need~to~collect~27~scraps~to~reforge~the~Solar~Blade.");
                                        data.AddText("~Each~time,~you~can~only~forge~1~Solar~Scrap.~So,~how~many~Energies~would~you~like~to~buy?");
                                        data.AddOption("1~Energy~and~complete~the~quest.(300~CPs)", 11);
                                        if (client.Inventory.GetCountItem(3003336) < 22)
                                            data.AddOption("6~Energies~and~complete~the~quest.(1800~CPs)", 13);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                                client.CreateBoxDialog("Your~inventory~is~full.~You~need~to~make~some~room,~first.");
                        }
                        else
                            client.CreateBoxDialog("You~don`t~have~the~Solar~Blade`s~Remain.~Go~claim~that~from~General~Pak!");
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.PaksGhost)]
        public static void PaksGhost(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PaksGhost, 0, 3269);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SuperMok, 0, 3278);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PaksGhost, 0, 3275);


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class >= 20)
                        {
                            data.AddText("No...~No...~You`re~not~the~Trojan~I`m~looking~for.~It`s~been~300~years,~however,~I~still~can`t~find~that~chosen~man.");
                            data.AddText("\n\n~~~~(Level~120+~or~reborn~Trojans~will~receive~a~Trojan~Epic~Weapon~for~free~by~completing~a~specified~Trojan~Epic~Quest~every~day,~and~a~series~of~Epic~Quests~in~90~days.)");
                            data.AddOption("Um...");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.StageEpicTrojanQuest == 0)
                            {
                                data.AddText("Hey,~YOU!~You`re~the~chosen~man.~You~know,~I`ve~been~here~300~years,~waiting~for~you!");
                                data.AddOption("What`re~talking?", 2);
                                data.FinalizeDialog();
                            }
                            else if (client.Player.StageEpicTrojanQuest == 1)
                            {
                                data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                if (client.Inventory.Contain(3003335, 1))
                                {
                                    if (client.Inventory.Contain(3003336, 27))
                                    {
                                        data.AddOption("Reforge~the~Solar~Blade.", 5);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddOption("How~to~collect~those~scraps?", 10);
                                        if (client.Inventory.Contain(3003336, 27))
                                        {
                                            data.AddOption("Reforge~the~Solar~Blade.", 5);
                                            data.AddAvatar(245);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                                else
                                {
                                    data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Finally...~the~Solar~Blade~is~back!~With~its~power,~you~can~travel~back~in~time~for~300~years.~Remember~to~find~me~at~that~time!");
                                data.AddText("~See~you~in~Twin~City.");
                                data.AddText("\n\n~~~~(Level~120+~or~reborn~Trojans~will~receive~a~Trojan~Epic~Weapon~for~free~by~completing~a~specified~Trojan~Epic~Quest~every~day,~and~a~series~of~Epic~Quests~in~90~days.)");
                                data.AddOption("Go~back~to~300~years~ago.", 6);
                                data.AddOption("I`m~not~ready,~yet.");
                                data.FinalizeDialog();
                            }

                        }
                        break;

                    }
                case 2:
                    {
                        data.AddText("It`s~a~long~story,~but~I`ll~make~it~short.~300~years~ago,~I~was~ordered~to~defend~against~the~invasion~of~the~Flame~Devastator.");
                        data.AddText("~Before~the~battle,~I~made~a~secret~plan,~however,~the~Flame~Devastator~has~already~known~it~and~trapped~my~army.");
                        data.AddText("~No~one~survived.~Over~a~night,~Twin~City~was~ruined~and~thousands~of~civilians~were~killed.");
                        data.AddOption("What~a~sad~thing.", 3);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("There~must~be~a~traitor~in~my~army!~How~can~I~rest~in~peace~without~digging~him~out?~So,~my~soul~lived~on~my~Solar~Blade`s~Remain,");
                        data.AddText("~waiting~for~a~chance.~Look,~you`re~the~chance!~Are~you~willing~to~travel~back~in~time~for~300~years,~and~root~out~this~cowardly~traitor?");
                        data.AddText("~So,~we~can~stop~the~Flame~Devastator~and~avoid~the~massacre~of~Twin~City.");
                        data.AddText("\n\n~~~~(Level~120+~or~reborn~Trojans~will~receive~a~Trojan~Epic~Weapon~for~free~by~completing~a~specified~Trojan~Epic~Quest~every~day,~and~a~series~of~Epic~Quests~in~90~days.)");
                        data.AddOption("I`ll~try~my~best!", 4);
                        data.AddOption("I`m~not~ready,~yet.");
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {

                        if (client.Inventory.Contain(3003335, 1))
                        {
                            if (client.Inventory.Contain(3003336, 27))
                            {
                                if (!client.Inventory.Remove(3003335, 1, stream)) break;
                                if (client.Inventory.Remove(3003336, 27, stream))
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);

                                    client.Player.StageEpicTrojanQuest = 2;
                                    client.Player.QuestGUI.Accept(ActiveQuest3, 0);

                                    client.SendSysMesage("General~Pak~recited~some~words~to~the~Solar~Scraps.~With~a~flash~of~bright~light,~the~Solar~Blade~is~reshaped!~Hurry~and~awaken~this~blade~to~go~back~to~300~years~ago!", MsgMessage.ChatMode.System);
                                    client.Player.AddMapEffect(stream, 267, 194, "role-select1");
                                    //97199128 312                     1002 267 194 role-select1 on npc think
                                    data.AddText("Finally...~the~Solar~Blade~is~back!~With~its~power,~you~can~travel~back~in~time~for~300~years.~Remember~to~find~me~at~that~time!");
                                    data.AddText("~See~you~in~Twin~City.");
                                    data.AddText("\n\n~~~~(Level~120+~or~reborn~Trojans~will~receive~a~Trojan~Epic~Weapon~for~free~by~completing~a~specified~Trojan~Epic~Quest~every~day,~and~a~series~of~Epic~Quests~in~90~days.)");
                                    data.AddOption("Go~back~to~300~years~ago.", 6);
                                    data.AddOption("I`m~not~ready,~yet.");
                                    data.AddAvatar(245);
                                    data.FinalizeDialog();


                                }
                            }
                            else
                            {
                                data.AddText("You~need~to~collect~27~Solar~Scraps,~first.~With~them,~I~can`t~reforge~the~Solar~Blade.");
                                data.AddOption("I~see.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You~need~to~collect~27~Solar~Scraps,~first.~With~them,~I~can`t~reforge~the~Solar~Blade.");
                            data.AddOption("I~see.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Player.StageEpicTrojanQuest >= 40)
                        {
                            if (client.Player.StageEpicTrojanQuest >= 50)
                            {
                                client.Teleport(150, 144, 3832);
                                client.Player.QuestGUI.SendAutoPatcher("You~felt~dizzy,~and~found~yourself~in~Twin~City,~300~years~ago.", 3832, 154, 130, 10581);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession5");
                            }
                            else
                            {
                                client.Teleport(150, 144, 3831);
                                client.Player.QuestGUI.SendAutoPatcher("You~felt~dizzy,~and~found~yourself~in~Twin~City,~300~years~ago.", 3831, 154, 130, 10581);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession5");
                            }
                        }
                        else
                        {
                            client.Teleport(150, 144, 3830);
                            client.Player.QuestGUI.SendAutoPatcher("You~felt~dizzy,~and~found~yourself~in~Twin~City,~300~years~ago.", 3832, 154, 130, 10581);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession5");
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("You~can~probably~get~the~Solar~Scraps~from~Super~Mok.~She~holds~the~secret~of~solar~power.~Well,~you~need~to~provide~the~materials~for~making~the~Solar~Scraps.");
                        data.AddText("~Since~the~job~is~quite~exhausted,~she~can~only~make~1~Solar~Scrap~in~a~day.");
                        data.AddText("\n\n~~~~(Level~120+~or~reborn~Trojans~will~receive~a~Trojan~Epic~Weapon~for~free~by~completing~a~specified~Trojan~Epic~Quest~every~day,~and~a~series~of~Epic~Quests~in~90~days.)");
                        data.AddOption("I`ll~go~find~Super~Mok.", 11);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1002, 238, 216, (uint)NpcID.SuperMok);
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(3003335, 1))
                        {
                            data.AddText("The~Solar~Blade`s~Remain~is~already~in~your~inventory.");
                            data.AddOption("Okay.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 3003335);
                                client.Player.StageEpicTrojanQuest = 1;
                                data.AddText("This~is~the~Solar~Blade`s~Remain.~Keep~it~well!");
                                data.AddOption("I~will.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Your~inventory~is~full.~Make~some~room,~first.");
                                data.AddOption("I`ll~do~it~now.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 4:
                    {


                        if (client.Player.Level < 120)
                        {
                            data.AddText("Sorry,~the~Flame~Devastator~is~much~fiercer~than~you~imagined.~You~should~be~at~least~Level~120~to~challenge.");
                            data.AddOption("Alright.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {

                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    if (client.Inventory.Contain(3003335, 1))
                                    {
                                        client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                        data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                        data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                        if (client.Inventory.Contain(3003335, 1))
                                        {
                                            if (client.Inventory.Contain(3003336, 27))
                                            {
                                                data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                data.AddOption("How~to~collect~those~scraps?", 10);
                                                data.FinalizeDialog();
                                            }
                                        }
                                        else
                                        {
                                            data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        client.Inventory.Add(stream, 3003335);
                                        client.Player.StageEpicTrojanQuest = 1;
                                        client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                        data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                        data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                        if (client.Inventory.Contain(3003335, 1))
                                        {
                                            if (client.Inventory.Contain(3003336, 27))
                                            {
                                                data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                data.AddOption("How~to~collect~those~scraps?", 10);
                                                data.FinalizeDialog();
                                            }
                                        }
                                        else
                                        {
                                            data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                                else
                                {
                                    client.Player.QuestGUI.Accept(ActiveQuest, 0);
                                    if (client.Inventory.Contain(3003335, 1))
                                    {
                                        client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                        data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                        data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                        if (client.Inventory.Contain(3003335, 1))
                                        {
                                            client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                            data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                            data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                            if (client.Inventory.Contain(3003335, 1))
                                            {
                                                if (client.Inventory.Contain(3003336, 27))
                                                {
                                                    data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("How~to~collect~those~scraps?", 10);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                            else
                                            {
                                                data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                                data.FinalizeDialog();
                                            }
                                        }
                                        else
                                        {
                                            client.Inventory.Add(stream, 3003335);
                                            client.Player.StageEpicTrojanQuest = 1;
                                            client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                            data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                            data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                            if (client.Inventory.Contain(3003335, 1))
                                            {
                                                if (client.Inventory.Contain(3003336, 27))
                                                {
                                                    data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("How~to~collect~those~scraps?", 10);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                            else
                                            {
                                                data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                                data.FinalizeDialog();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        client.Inventory.Add(stream, 3003335);
                                        client.Player.StageEpicTrojanQuest = 1;
                                        client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                        data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                        data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                        if (client.Inventory.Contain(3003335, 1))
                                        {
                                            client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                            data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                            data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                            if (client.Inventory.Contain(3003335, 1))
                                            {
                                                if (client.Inventory.Contain(3003336, 27))
                                                {
                                                    data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("How~to~collect~those~scraps?", 10);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                            else
                                            {
                                                data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                                data.FinalizeDialog();
                                            }
                                        }
                                        else
                                        {
                                            client.Inventory.Add(stream, 3003335);
                                            client.Player.StageEpicTrojanQuest = 1;
                                            client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                            data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                            data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                            if (client.Inventory.Contain(3003335, 1))
                                            {
                                                if (client.Inventory.Contain(3003336, 27))
                                                {
                                                    data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("How~to~collect~those~scraps?", 10);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                            else
                                            {
                                                data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                                data.FinalizeDialog();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Make~some~room~in~your~inventory,~first!~I`ll~give~you~my~Solar~Blade`s~Remain.~With~its~power,~you~may~go~back~to~300~years~ago.");
                                data.AddOption("I~see.");
                                data.FinalizeDialog();
                            }
                        }

                        break;
                    }
            }
        }
    }
}
