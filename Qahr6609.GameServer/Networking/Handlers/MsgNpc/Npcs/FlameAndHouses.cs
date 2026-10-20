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
        [NpcAttribute(NpcID.Flame1)]
        public static void Flame1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("It`s not the right time. You can`t light up the Flame Stone.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            break;
                        }
                        if (client.Inventory.Contain(729960, 1))
                        {
                            if (!client.Inventory.Remove(729960, 1, stream)) break;
                            client.Inventory.Add(stream, 729961);
                            data.AddText("Congratulation! light~up~the~Flame~Stone~(317,270)~on~the~right~of~the~Guild~Arena.").FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You cannot flame up this stone without the proper rune. Light~up~the~Flame~Stone~(317,270)~on~the~right~of~the~Guild~Arena.").FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Flame2)]
        public static void Flame2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("It`s not the right time. You can`t light up the Flame Stone.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            break;
                        }
                        if (client.Inventory.Contain(729961, 1))
                        {
                            if (!client.Inventory.Remove(729961, 1, stream)) break;
                            client.Inventory.Add(stream, 729962);
                            data.AddText("Congratulation! light~up~the~Flame~Stone~(236,291)~near~the~bridge~in~the~Guild~Arena.").FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You cannot flame up this stone without the proper rune. Light~up~the~Flame~Stone~(236,291)~near~the~bridge~in~the~Guild~Arena.").FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Flame3)]
        public static void Flame3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("It`s not the right time. You can`t light up the Flame Stone.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            break;
                        }
                        if (client.Inventory.Contain(729962, 1))
                        {
                            if (!client.Inventory.Remove(729962, 1, stream)) break;
                            client.Inventory.Add(stream, 729963);
                            data.AddText("Congratulation! light~up~the~Flame~Stone~(194,168)~near~the~right~gate~of~the~Guild~City.").FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You cannot flame up this stone without the proper rune. Light~up~the~Flame~Stone~(194,168)~near~the~right~gate~of~the~Guild~City.").FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Flame4)]
        public static void Flame4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("It`s not the right time. You can`t light up the Flame Stone.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            break;
                        }
                        if (client.Inventory.Contain(729963, 1))
                        {
                            if (!client.Inventory.Remove(729963, 1, stream)) break;
                            client.Inventory.Add(stream, 729964);
                            data.AddText("Congratulation! light~up~the~Flame~Stone~(115,53)~on~the~right~of~the~Guild~City.").FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You cannot flame up this stone without the proper rune. Light~up~the~Flame~Stone~(115,53)~on~the~right~of~the~Guild~City.").FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Flame5)]
        public static void Flame5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("It`s not the right time. You can`t light up the Flame Stone.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            break;
                        }
                        if (client.Inventory.Contain(729964, 1))
                        {
                            if (!client.Inventory.Remove(729964, 1, stream)) break;
                            client.Inventory.Add(stream, 729965);
                            data.AddText("Congratulation! light~up~the~Flame~Stone~(316,378)~on~the~left~of~the~Guild~Arena.").FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You cannot flame up this stone without the proper rune. Light~up~the~Flame~Stone~(316,378)~on~the~left~of~the~Guild~Arena.").FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Flame6)]
        public static void Flame6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("It`s not the right time. You can`t light up the Flame Stone.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            break;
                        }
                        if (client.Inventory.Contain(729965, 1))
                        {
                            if (!client.Inventory.Remove(729965, 1, stream)) break;
                            client.Inventory.Add(stream, 729966);
                            data.AddText("Congratulation! light~up~the~Flame~Stone~(136,182)~near~the~left~gate~of~the~Guild~City.").FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You cannot flame up this stone without the proper rune. Light~up~the~Flame~Stone~(136,182)~near~the~left~gate~of~the~Guild~City.").FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Flame7)]
        public static void Flame7(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("It`s not the right time. You can`t light up the Flame Stone.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            break;
                        }
                        if (client.Inventory.Contain(729966, 1))
                        {
                            if (!client.Inventory.Remove(729966, 1, stream)) break;
                            client.Inventory.Add(stream, 729967);
                            data.AddText("Congratulation! light~up~the~Flame~Stone~(38,94)~on~the~left~of~the~Guild~City.").FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You cannot flame up this stone without the proper rune. Light~up~the~Flame~Stone~(38,94)~on~the~left~of~the~Guild~City.").FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Flame8)]
        public static void Flame8(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("It`s not the right time. You can`t light up the Flame Stone.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            break;
                        }
                        if (client.Inventory.Contain(729967, 1))
                        {
                            if (!client.Inventory.Remove(729967, 1, stream)) break;
                            client.Inventory.Add(stream, 729968);
                            data.AddText("Congratulation! light~up~the~Flame~Stone~(350,321)~near~the~Flame~Taoist.").FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You cannot flame up this stone without the proper rune. Light~up~the~Flame~Stone~(350,321)~near~the~Flame~Taoist.").FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Flame9)]
        public static void Flame9(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("It`s not the right time. You can`t light up the Flame Stone.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            break;
                        }
                        if (client.Inventory.Contain(729968, 1))
                        {
                            if (!client.Inventory.Remove(729968, 1, stream)) break;
                            client.Inventory.Add(stream, 729969);
                            data.AddText("Congratulation! light~up~the~Flame~Stone~(350,321)~near~the~Flame~Taoist.").FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You cannot flame up this stone without the proper rune. Light~up~the~Flame~Stone~(350,321)~near~the~Flame~Taoist.").FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Flame10)]
        public static void Flame10(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("You totally missed the right time.the 10th Flame Stone can`t be lit up now.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            break;
                        }
                        if (MsgTournaments.MsgSchedules.GuildWar.FlamesQuest.ActiveFlame10)
                        {
                            if (client.Inventory.Contain(729969, 1))
                            {
                                if (!client.Inventory.Remove(729969, 1, stream)) break;
                                client.Inventory.Add(stream, 729970);
                                data.AddText("Congratulation! light~up~the~Flame~Stone~(62,59)~near~the~Stone~Pole~in~the~Guild~City.").FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You cannot flame up this stone without the proper rune. light~up~the~Flame~Stone~(62,59)~near~the~Stone~Pole~in~the~Guild~City.").FinalizeDialog();
                            }
                        }
                        else
                        {
                            client.SendSysMesage("You totally missed the right time.the 10th Flame Stone can`t be lit up now.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FlameTaoist)]
        public static void FlameTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You must have heard that the Olympic Games. I`ve been waiting for it for my entire life. There are 10 Flame Stones")
                                                      .AddText("~that I would like you to light up...You will get some rewards, of course. It really makes sense to us, ah, at least to me.")
                                                  .AddOption("Rewards?~Tell~me~more!~Come~on!", 1)
                                                  .AddOption("Can you give me a rune?", 4)
                                                  .AddOption("I~have~a~rune.~Gimme~the~reward!", 5)
                                                  .AddOption("Not~interested.", 255)
                                                  .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(729970, 1))
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                            {
                                if (!client.Inventory.Remove(729970, 1, stream)) break;
                                client.GainExpBall(600 * 12, true, Role.Flags.ExperienceEffect.angelwing);
                            }
                            else
                            {
                                data.AddText("You cannot claim experience while guild war.")
                   .AddOption("Ahh", 255)
                   .AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You cannot claim experience if you don't have a rune.")
                   .AddOption("Ahh", 255)
                   .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        // client Activity.ini "Flame Lit": Saturday 21:00-22:59 (the last two hours of the Guild War), level 110+, once a week
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces != MsgTournaments.ProcesType.Dead && !MsgTournaments.MsgSchedules.GuildWar.GmStarted && DateTime.Now.Hour < 21)
                        {
                            data.AddText("It`s~not~the~right~time.~The~Flame~Lit~ceremony~is~held~in~the~last~two~hours~of~the~Guild~War~(21:00-23:00).~Come~back~then.")
                                .AddOption("I~see.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces != MsgTournaments.ProcesType.Dead && client.Player.Level < 110)
                        {
                            data.AddText("You~must~be~at~least~level~110~to~light~up~the~Flame~Stones.")
                                .AddOption("I~see.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces != MsgTournaments.ProcesType.Dead)
                        {
                            if (!MsgTournaments.MsgSchedules.GuildWar.FlamesQuest.Registred.Contains(client.Player.UID))
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    // the rune was handed out again on every click (nobody was ever registered)
                                    MsgTournaments.MsgSchedules.GuildWar.FlamesQuest.Registred.Add(client.Player.UID);
                                    client.Inventory.Add(stream, 729960);
                                }
                                else
                                {
                                    data.AddText("There is not enough room in your inventory.")
                    .AddOption("Ah, hold on.", 255)
                    .AddAvatar(67).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You already have a rune.")
                .AddOption("Thank you.", 255)
                .AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You? Wait a sec... Oh, yeah, no wonder you looked familir. Each player can carry this quest ONCE a week")
                                .AddText("only, and if you have more runes, you may as well accept the quest with Flame Taoist next week and claim your reward directly when the quest ends.")
               .AddOption("Once~a~week?!", 255)
               .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Ah, you seem very interested! Good. You are eligible to light up the flame! But, it`s not the right time. You see, the")
                                                     .AddText("~ceremony will be held till the last two hours of the Guild War. Come and sign up then.")
                                                 .AddOption("Would~you~please~give~me~more~information?", 2)
                                                 .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Yeah. I`ve prepare 10 Flame Stones in the Wind Plain and Guild Arena. When the time comes, I will give you a Jade Rune. You")
                                                      .AddText("~just take it to light up all the Flame Stones in sequence.")
                                                  .AddOption("Sure.", 255)
                                                  .AddAvatar(67).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Class6House)]
        public static void Class6House(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome, this is the Class 6 model house with many new features. When you`re walking around the house,")
                            .AddText("~you can talk to our guide in each area to learn more. If you want to upgrade your house to Class 6,")
                            .AddText("~I`m the right person to serve you./n")
                            .AddText("You will need 99,000 Conquer Points in order to update your House to the next level.");
                        data.AddOption("Upgrade~my~house~with~CPs.", 1);
                        data.AddOption("Use~materials~to~upgrade.", 3);
                        data.AddOption("How~to~upgrade~my~house?", 6);
                        data.AddOption("Where~is~the~exit?", 2);
                        data.AddOption("Just passing by.", 255).AddAvatar(111).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        data.AddText("There are two ways to upgrade to Class 6: pay 8,999 CPs or submit 500 Class 6 House Packs. You can collect the Class 6 House Packs")
                            .AddText("~by participating in Dis City, Evil Labyrinth, cross-server competitions and other quests, or buy them with Honor Points,")
                            .AddText("~Horse Racing Points, or Champion Points.")
                              .AddOption("I~see", 255).AddAvatar(111).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("We need totally 500 Class 6 House Packs to upgrade your house to Class 6. So, how many packs would you like to submit this time?")//asta?
                                .AddOption("100.", 4)
                                .AddOption("500.", 5)
                                .AddOption("Let~me~see.", 255).AddAvatar(111).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(3008100, 100))
                        {
                            client.Inventory.RemoveStackItem(3008100, 100, stream);
                            if (Role.Core.Rate(30))
                            {
                                if (client.MyHouse == null)
                                    break;

                                ushort House_ID = Database.HouseTable.GetHouseId(client.MyHouse.Level);
                                if (House_ID != 0)
                                {
                                    //remove players house
                                    foreach (var pClient in Database.Server.GamePoll.Values)
                                        if (pClient.Player.Map == House_ID && pClient.Player.DynamicID == client.Player.UID)
                                            pClient.Teleport(200, 200, 1036);
                                }
                                if (client.MyHouse.Furnitures.Count != 0)
                                {
                                    data.AddText("Please remove all your furnitures from your house.")//asta?
                                       .AddOption("I~see.", 255).AddAvatar(111).FinalizeDialog();
                                    break;
                                }
                                if (client.MyHouse.Level == 5)
                                {

                                    client.MyHouse.Level++;
                                    data.AddText("You have successfully updated your House.")
                                 .AddOption("Thank you.", 255).AddAvatar(111).FinalizeDialog();
                                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Hurray ! " + client.Player.Name + " has successfully updated his house to the level 6.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                                    client.Teleport(200, 95, 1036);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                                }
                            }
                            else
                            {
                                data.AddText("You have failed to upgrade the house to level 6 , give a try again later maybe you will have more luck .")
                              .AddOption("I~see.", 255).AddAvatar(111).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I`m afraid I can`t help you if you don`t have enough materials. Please go collect more.")
                               .AddOption("Okay.", 255).AddAvatar(111).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3008100, 500))
                        {

                            if (client.MyHouse == null)
                                break;

                            ushort House_ID = Database.HouseTable.GetHouseId(client.MyHouse.Level);
                            if (House_ID != 0)
                            {
                                //remove players house
                                foreach (var pClient in Database.Server.GamePoll.Values)
                                    if (pClient.Player.Map == House_ID && pClient.Player.DynamicID == client.Player.UID)
                                        pClient.Teleport(200, 200, 1036);
                            }
                            if (client.MyHouse.Furnitures.Count != 0)
                            {
                                data.AddText("Please remove all your furnitures from your house.")//asta?
                                   .AddOption("I~see.", 255).AddAvatar(111).FinalizeDialog();
                                break;
                            }
                            if (client.MyHouse.Level == 5)
                            {
                                client.Inventory.RemoveStackItem(3008100, 500, stream);
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(111).FinalizeDialog();
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Hurray ! " + client.Player.Name + " has successfully updated his house to the level 6.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                                client.Teleport(200, 95, 1036);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                            }
                        }
                        else
                        {
                            data.AddText("I`m afraid I can`t help you if you don`t have enough materials. Please go collect more.")
                               .AddOption("Okay.", 255).AddAvatar(111).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Teleport(200, 95, 1036);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 99000)
                        {
                            if (client.MyHouse == null)
                                break;

                            ushort House_ID = Database.HouseTable.GetHouseId(client.MyHouse.Level);
                            if (House_ID != 0)
                            {
                                //remove players house
                                foreach (var pClient in Database.Server.GamePoll.Values)
                                    if (pClient.Player.Map == House_ID && pClient.Player.DynamicID == client.Player.UID)
                                        pClient.Teleport(200, 200, 1036);
                            }
                            if (client.MyHouse.Furnitures.Count != 0)
                            {
                                data.AddText("Please remove all your furnitures from your house.")//asta?
                                   .AddOption("I~see.", 255).AddAvatar(111).FinalizeDialog();
                                break;
                            }
                            if (client.MyHouse.Level == 5)
                            {
                                client.Player.ConquerPoints -= 99000;
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(111).FinalizeDialog();
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Hurray ! " + client.Player.Name + " has successfully updated his house to the level 6.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                                client.Teleport(200, 95, 1036);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                            }


                        }
                        else
                        {
                            data.AddText("Sorry, I can`t help you in this matter since you don`t have enough CPs. Please bring the money, next time.")
                            .AddOption("Okay.", 255).AddAvatar(111).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.HouseAdmin)]
        public static void HouseAdmin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.MyHouse == null)
                        {
                            data.AddText("What can i do for you?");
                            data.AddOption("buy a house.", 1);
                            data.AddOption("buy house certificate.", 2);
                            data.AddOption("Visit my wife`s House.", 3);
                            data.AddOption("Just passing by.", 255).AddAvatar(7).FinalizeDialog();

                        }
                        else if (client.MyHouse.Level == 0)
                        {
                            data.AddText("What can i do for you?");
                            data.AddOption("buy a house.", 1);
                            data.AddOption("buy house certificate.", 2);
                            data.AddOption("Visit my wife`s House.", 3);
                            data.AddOption("Just passing by.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        else if (client.MyHouse.Level == 1)
                        {
                            data.AddText("What can i do for you?");
                            data.AddOption("Visit house.", 4);
                            data.AddOption("Upgrade house.", 5);
                            data.AddOption("buy house certificate.", 2);
                            data.AddOption("Visit my wife`s House.", 3);
                            data.AddOption("Just passing by.", 255).AddAvatar(7).FinalizeDialog();

                        }
                        else if (client.MyHouse.Level == 5)
                        {
                            data.AddText("As the owner of a Class 5 house, you`ll be able to hold a Grand Wedding. Additionally, you can upgrade it to Class 6")
                                .AddText("~to get a more luxurious living space and more services.");
                            data.AddOption("I~wanna~go~home.", 4);
                            if (client.MyHouse.Level < 6)
                            {
                                data.AddOption("Upgrade~to~Class~6.", 20);
                                data.AddOption("Remove All furnitures.", 6);
                            }
                            data.AddOption("Visit my wife`s House.", 3);
                            data.AddOption("Just passing by.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("What can i do for you?");
                            data.AddOption("Visit house.", 4);
                            data.AddOption("Upgrade house.", 5);
                            data.AddOption("Remove All furnitures.", 6);
                            if (client.MyHouse.Level < 5)
                                data.AddOption("buy house certificate.", 2);
                            data.AddOption("Visit my wife`s House.", 3);
                            data.AddOption("Just passing by.", 255).AddAvatar(7).FinalizeDialog();

                        }

                        break;
                    }
                case 20:
                    {
                        data.AddText("To upgrade your house to Class 6, you need to talk to the Class 6 House Admin in the Class 6 model house. I cannot help you in this matter.");
                        data.AddOption("Okay,~send~me~there.", 21);
                        data.AddOption("Just~passing~by.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 21:
                    {

                        client.Teleport(127, 138, 3995, 0);
                        break;
                    }
                case 6:
                    {
                        if (client.MyHouse != null)
                        {
                            client.MyHouse.Furnitures.Clear();

                            data.AddText("You furnitures remove Successful")
                                .AddOption("Thank~you", 255)
                            .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse == null)
                        {
                            if (client.Inventory.Remove(721170, 1, stream))
                            {
                                client.HeroRewards.AddGoal(704);
                                client.MyHouse = new Role.Instance.House(client.Player.UID);
                                data.AddText("You're create a house Successful. ")
                               .AddOption("Thank~you", 255).AddAvatar(7).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Sorry you not have house Permit")
                                .AddOption("Ok", 255).AddAvatar(7).FinalizeDialog();

                            }

                        }


                        break;
                    }
                case 5:
                    {
                        if (client.MyHouse == null) break;

                        if (client.MyHouse.Level == 0)
                        {
                            data.AddText("You don`t have house.")
                              .AddOption("Ok", 255).AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        ushort House_ID = Database.HouseTable.GetHouseId(client.MyHouse.Level);
                        if (House_ID != 0)
                        {
                            //remove players house
                            foreach (var pClient in Database.Server.GamePoll.Values)
                                if (pClient.Player.Map == House_ID && pClient.Player.DynamicID == client.Player.UID)
                                    pClient.Teleport(200, 200, 1036);
                        }
                        if (client.MyHouse.Furnitures.Count != 0)
                        {
                            data.AddText("Please remove all your furnitures from your house.")
                               .AddOption("Ok", 255).AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.MyHouse.Level == 1)
                        {
                            if (client.Inventory.Remove(721174, 1, stream))
                            {
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you not have Upgrade Cert for update.")
                           .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else if (client.MyHouse.Level == 2)
                        {
                            if (client.Inventory.Remove(711198, 1, stream))
                            {
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you not have Upgrade Cert(class 3) for update.")
                                     .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else if (client.MyHouse.Level == 3)
                        {
                            if (client.Inventory.Remove(711200, 1, stream))
                            {
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you not have Upgrade Cert(class 4) for update.")
                                       .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else if (client.MyHouse.Level == 4)
                        {
                            if (client.Inventory.Remove(711553, 1, stream))
                            {
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you not have Upgrade Cert(class 5) for update.")
                                     .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You house is the best level")
                                      .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 4:
                    {
                        if (client.MyHouse == null)
                            break;
                        if (client.MyHouse.Level == 0)
                        {
                            data.AddText("You don`t have house.")
                              .AddOption("Ok", 255).AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        ushort HouseID = 0;
                        ushort house_x = 0;
                        ushort house_y = 0;
                        if (client.MyHouse.Level == 1)
                        {
                            HouseID = 1098;
                            house_x = 32;//3
                            house_y = 39;//40
                        }
                        else if (client.MyHouse.Level == 2)
                        {
                            HouseID = 1099;
                            house_x = 53;
                            house_y = 83;
                        }
                        else if (client.MyHouse.Level == 3)
                        {
                            HouseID = 2080;
                            house_x = 78;
                            house_y = 57;
                        }
                        else if (client.MyHouse.Level == 4)
                        {
                            HouseID = 601;
                            house_x = 80;
                            house_y = 58;
                        }
                        else if (client.MyHouse.Level == 5)
                        {
                            HouseID = 3024;
                            house_x = 105;
                            house_y = 139;
                        }
                        else if (client.MyHouse.Level == 6)
                        {
                            HouseID = 3995;
                            house_x = 127;
                            house_y = 138;
                        }
                        if (HouseID != 0)
                        {

                            if (client.Team != null)
                            {
                                foreach (var members in client.Team.Temates)
                                {
                                    if (members.client != null)
                                    {
                                        if (members.client.Player.UID != client.Player.UID)
                                        {
                                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, members.client.Player.X, members.client.Player.Y) < 18)
                                            {
                                                members.client.Teleport(house_x, house_y, HouseID, client.Player.UID);
                                            }
                                        }
                                    }
                                }
                            }
                            client.Teleport(house_x, house_y, HouseID, client.Player.UID);
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.SpouseUID != 0)
                        {
                            if (Database.Server.GamePoll.ContainsKey(client.Player.SpouseUID))
                            {
                                var MySpouse = Database.Server.GamePoll[client.Player.SpouseUID];
                                if (MySpouse.MyHouse != null)
                                {
                                    ushort HouseID = 0;
                                    ushort house_x = 0;
                                    ushort house_y = 0;
                                    if (MySpouse.MyHouse.Level == 1)
                                    {
                                        HouseID = 1098;
                                        house_x = 33;
                                        house_y = 40;
                                    }
                                    else if (MySpouse.MyHouse.Level == 2)
                                    {
                                        HouseID = 1099;
                                        house_x = 53;
                                        house_y = 83;
                                    }
                                    else if (MySpouse.MyHouse.Level == 3)
                                    {
                                        HouseID = 2080;
                                        house_x = 78;
                                        house_y = 57;
                                    }
                                    else if (MySpouse.MyHouse.Level == 4)
                                    {
                                        HouseID = 601;
                                        house_x = 80;
                                        house_y = 58;
                                    }
                                    else if (MySpouse.MyHouse.Level == 5)
                                    {
                                        HouseID = 3024;
                                        house_x = 105;
                                        house_y = 139;
                                    }
                                    else if (client.MyHouse.Level == 6)
                                    {
                                        HouseID = 3995;
                                        house_x = 127;
                                        house_y = 138;
                                    }
                                    client.Teleport(house_x, house_y, HouseID, MySpouse.Player.UID);
                                }
                                else
                                {
                                    data.AddText("Sorry, but you vife not have House")
                                                              .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Sorry, but you vife is not online.")
                                                              .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you vife not have House")
                                   .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        data.AddText("Do you want a memorable wedding ceremony? it`s not easy, you know However,");
                        data.AddText("I can help you with that. First of all, we need a professional emcee.");
                        data.AddOption("House Permit: 99 CPs.", 10);
                        data.AddOption("Upgrade Cert: 199 Cps", 11);
                        data.AddOption("UpgradeCert(Class 3): 259 CPs", 12);
                        data.AddOption("UpgradeCert(Class 4): 299 CPs", 13);
                        data.AddOption("UpgradeCert(Class 5): 9999 CPs", 14);
                        data.AddOption("Let me think about it.", 255).AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        ushort cost = 99;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.Inventory.Add(stream, 721170))
                            {
                                client.Player.ConquerPoints -= cost;

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                        .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 11:
                    {
                        ushort cost = 199;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.Inventory.Add(stream, 721174))
                            {
                                client.Player.ConquerPoints -= cost;

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                     .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 12:
                    {
                        ushort cost = 259;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.Inventory.Add(stream, 711198))
                            {
                                client.Player.ConquerPoints -= cost;

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                   .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 13:
                    {
                        ushort cost = 299;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.Inventory.Add(stream, 711200))
                            {
                                client.Player.ConquerPoints -= cost;

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                   .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 14:
                    {
                        ushort cost = 9999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.Inventory.Add(stream, 711553))
                            {
                                client.Player.ConquerPoints -= cost;

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                   .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
    }
}
