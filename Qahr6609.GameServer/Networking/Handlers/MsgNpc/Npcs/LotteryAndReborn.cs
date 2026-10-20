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
        [NpcAttribute(NpcID.MillionaireLee)]
        public static void MillionaireLee(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hunting~and~mining~for~the~treasure~is~an~exciting~thing.~However,~they~also~pile~up~in~your~inventories.")
                    .AddOption("Pack~+1~Stones.", 1)
                    .AddOption("Pack~+2~Stones.", 3)
                    .AddOption("Pack~+3~Stones.", 5)
                    .AddOption("Pack~Normal~Gems.", 7)
                    .AddOption("Pack~EXP~Balls~for~me!", 32)
                    .AddOption("Pack~Meteors.", 34)
                    .AddOption("Pack~DragonBalls.", 36)
                    .AddOption("Pack~Meteor~Scroll.", 38)
                    .AddOption("Pack~DragonBall~Scrolls.", 40)
                    .AddOption("Just~passing~by.", 255)
                    .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 40:
                    {
                        data.AddText("10 DragonBall Scrolls can be packed into a Super DragonBall Scroll. Would you like me to pack them for you?")
                           .AddOption("Yes,~please.", 41)
                           .AddOption("Bye.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        if (client.Inventory.Remove2(Database.ItemType.DragonBallScroll, 10, stream, 1))
                        {

                            client.Inventory.Add(stream, Database.ItemType.SuperDragonBallScroll_B_, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Remove2(Database.ItemType.DragonBallScroll, 10, stream))
                        {

                            client.Inventory.Add(stream, Database.ItemType.SuperDragonBallScroll, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 10 DragonBall Scrolls.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 38:
                    {
                        data.AddText("10 Meteor Scrolls can be packed into a Super Meteor Scroll. Would you like me to pack them for you?")
                           .AddOption("Yes,~please.", 39)
                           .AddOption("Bye.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 39:
                    {
                        if (client.Inventory.Remove2(Database.ItemType.MeteorScroll, 10, stream, 1))
                        {

                            client.Inventory.Add(stream, Database.ItemType.SuperMeteorScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Remove2(Database.ItemType.MeteorScroll, 10, stream, 0))
                        {

                            client.Inventory.Add(stream, Database.ItemType.SuperMeteorScroll, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 10 meteor scrolls.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 36:
                    {
                        data.AddText("10 DragonBall can be packed into a DragonBall Pack. Would you like me to pack them for you?")
                           .AddOption("Yes,~please.", 37)
                           .AddOption("Bye.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 37:
                    {
                        if (client.Inventory.Remove2(Database.ItemType.DragonBall, 10, stream))
                        {

                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 10 DragonBall.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }

                case 34:
                    {
                        data.AddText("10 Meteors can be packed into a Meteor Pack. Would you like me to pack them for you?")
                            .AddOption("Yes,~please.", 35)
                            .AddOption("Bye.", 255)
                            .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 35:
                    {
                        if (client.Inventory.Remove2(Database.ItemType.Meteor, 10, stream, 1))
                        {

                            client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Remove2(Database.ItemType.Meteor, 10, stream, 0))
                        {

                            client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 10 meteors.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 32:
                    {
                        data.AddText("Pay 100,000 Silver and give me 5 EXP Balls, I can pack them into an EXP Ball Gift Box for you!")
                                                    .AddOption("Pack~EXP~Balls~for~me!", 33)
                                                    .AddOption("Bye.", 255)
                                                    .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 33:
                    {
                        if (client.Inventory.Remove2(Database.ItemType.ExpBall, 5, stream, 1))
                        {
                            if (client.Player.Money >= 100000)
                            {
                                client.Player.Money -= 100000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Inventory.Add(stream, 727464, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                            {
                                data.AddText("You do not have 100,000 silvers with you.")
                 .AddOption("My~bad.", 255)
                 .AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else if (client.Inventory.Remove2(Database.ItemType.ExpBall, 5, stream, 0))
                        {
                            if (client.Player.Money >= 100000)
                            {
                                client.Player.Money -= 100000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Inventory.Add(stream, 727464, 1);
                            }
                            else
                            {
                                data.AddText("You do not have 100,000 silvers with you.")
                 .AddOption("My~bad.", 255)
                 .AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 EXP Balls.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
                        data.AddText("Which kind of Normal Gems do you want to pack up?")
                   .AddOption("Glory~Gems.", 8)
                   .AddOption("Thunder~Gems.", 9)
                   .AddOption("Kylin~Gems.", 10)
                   .AddOption("Rainbow~Gems.", 11)
                   .AddOption("Fury~Gems.", 12)
                   .AddOption("Next~page.", 13)
                   .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        data.AddText("Which kind of Normal Gems do you want to pack up?")
                  .AddOption("Dragon~Gems.", 14)
                  .AddOption("Phoenix~Gems.", 15)
                  .AddOption("Violet~Gems.", 16)
                  .AddOption("Moon~Gems.", 17)
                  .AddOption("Tortoise~Gems.", 18)
                  .AddOption("Back~page.", 7)
                  .AddOption("Just~passing~by.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 18:
                    {
                        data.AddText("5 Normal Tortoise Gems can be packed into a Tortoise Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 29)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 29:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalTortoiseGem), 5))
                        {
                            if (!client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalTortoiseGem), 5, stream)) break;
                            client.Inventory.Add(stream, 727069, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Tortoise Gems.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 17:
                    {
                        data.AddText("5 Normal Moon Gems can be packed into a Moon Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 28)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Remove2((uint)(700000 + (uint)Role.Flags.Gem.NormalMoonGem), 5, stream))
                        {

                            client.Inventory.Add(stream, 727068, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Moon Gems.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 16:
                    {
                        data.AddText("5 Normal Violet Gems can be packed into a Violet Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 27)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Remove2((uint)(700000 + (uint)Role.Flags.Gem.NormalVioletGem), 5, stream))
                        {

                            client.Inventory.Add(stream, 727067, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Violet Gems.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 15:
                    {
                        data.AddText("5 Normal Phoenix Gems can be packed into a Phoenix Gem Pack! Would you like me to pack them for you?")
                .AddOption("Yes,~please.", 26)
                .AddOption("Bye.", 255)
                .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 26:
                    {
                        if (client.Inventory.Remove2((uint)(700000 + (uint)Role.Flags.Gem.NormalPhoenixGem), 5, stream))
                        {

                            client.Inventory.Add(stream, 727066, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Phoenix Gems.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 14:
                    {
                        data.AddText("5 Normal Dragon Gems can be packed into a Dragon Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 25)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 25:
                    {
                        if (client.Inventory.Remove2((uint)(700000 + (uint)Role.Flags.Gem.NormalDragonGem), 5, stream))
                        {

                            client.Inventory.Add(stream, 727065, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Dragon Gems.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        data.AddText("5 Normal Fury Gems can be packed into a Fury Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 24)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 24:
                    {
                        if (client.Inventory.Remove2((uint)(700000 + (uint)Role.Flags.Gem.NormalFuryGem), 5, stream))
                        {

                            client.Inventory.Add(stream, 727064, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Fury Gems.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 11:
                    {
                        data.AddText("5 Normal Rainbow Gems can be packed into a Rainbow Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 23)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.Remove2((uint)(700000 + (uint)Role.Flags.Gem.NormalRainbowGem), 5, stream))
                        {

                            client.Inventory.Add(stream, 727063, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Rainbow Gems.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("5 Normal Kylin Gems can be packed into a Kylin Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 22)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Remove2((uint)(700000 + (uint)Role.Flags.Gem.NormalKylinGem), 5, stream))
                        {

                            client.Inventory.Add(stream, 727062, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Kylin Gems.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 9:
                    {
                        data.AddText("5 Normal Thunder Gems can be packed into a Thunder Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 21)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Remove2((uint)(700000 + (uint)Role.Flags.Gem.NormalThunderGem), 5, stream))
                        {

                            client.Inventory.Add(stream, 727061, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Thunder Gems.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }

                case 8:
                    {
                        data.AddText("5 Normal Glory Gems can be packed into a Glory Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 20)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Remove2((uint)(700000 + (uint)Role.Flags.Gem.NormalGloryGem), 5, stream))
                        {

                            client.Inventory.Add(stream, 727060, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Normal Glory.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("10 +3 Stones can be packed into a +3 Stone Pack. Would you like me to pack them for you?")
                .AddOption("Yes,~please.", 6)
                .AddOption("Bye.", 255)
                .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Remove2(730003, 10, stream))
                        {

                            client.Inventory.Add(stream, 729023, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 10 +3 Stones.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("10 +2 Stones can be packed into a +2 Stone Pack. Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 4)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove2(730002, 10, stream))
                        {

                            client.Inventory.Add(stream, 729022, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 10 +2 Stones.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 1:
                    {
                        data.AddText("5 +1 Stones can be packed into a +1 Stone Pack. Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 2)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 2:
                    {

                        if (client.Inventory.Remove2(730001, 5, stream, 1))
                        {

                            client.Inventory.Add(stream, 723712, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Remove2(730001, 5, stream, 0))
                        {
                            client.Inventory.Add(stream, 723712, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 +1 Stones.")
                  .AddOption("My~bad.", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.UnknowMan)]
        public static void UnknowMan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The Dragon Ball is really a precious item. You can use it to refine equipment. What`s more, if you use it properly,")
                            .AddText("~you can gain a large amount of EXP from it. Well, if you have a Dragon Ball, I can help you gain some EXP.")
                    .AddOption("Please~help~me.", 1)
                    .AddOption("Definitely.", 255)
                    .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("According to your ability, I can help you gain EXP from 1 Dragon Ball, each day.")
                    .AddOption("Please~help~me.", 2)
                    .AddOption("I~see.", 255)
                    .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.Level == 140)
                        {
                            data.AddText("Sorry, but you have a big level.")
              .AddOption("I~see.", 255)
              .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                        {
                            if (!client.Player.DbTry)
                            {
                                client.Player.DbTry = true;
                                if (!client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream)) break;
                                client.GainExpBall(600 * 10, true, Role.Flags.ExperienceEffect.angelwing);
                            }
                            else
                            {
                                data.AddText("Sorry, but you my service today, come back tomorow with one dragonball.")
                 .AddOption("I~see.", 255)
                 .AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough Dragon Balls.")
                   .AddOption("I~see.", 255)
                   .AddAvatar(67).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CollectorZhao)]
        public static void CollectorZhao(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~fond~of~collecting~all~kinds~of~treasure.~If~you~happen~to~have~any~Disguise Amulets,~Life Fruit Basket,~or")
                            .AddText("~PenitenceAmulets,~you~may~sell~them~to~me.~I~will~pay~you~ten~CPs~for~each~one.")
                    .AddOption("Here~are~my~Disguise~Amulets.", 1)
                    .AddOption("Here~are~my~Life~Fruit~Baskets.", 2)
                    .AddOption("Here~are~my~Penitence~Amulets.", 3)
                    .AddOption("Really?~I~don`t~believe~it.", 255)
                    .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(723724, 1))
                        {
                            if (!client.Inventory.Remove(723724, 1, stream)) break;
                            client.Player.ConquerPoints += 15;


                            data.AddText("You receive 15 ConquerPoints for you item.")
                   .AddOption("Ok~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(723725, 1))
                        {
                            if (!client.Inventory.Remove(723725, 1, stream)) break;
                            client.Player.ConquerPoints += 15;


                            data.AddText("You receive 15 ConquerPoints for you item.")
                   .AddOption("Ok~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(723727, 1))
                        {
                            if (!client.Inventory.Remove(723727, 1, stream)) break;
                            client.Player.ConquerPoints += 15;


                            data.AddText("You receive 15 ConquerPoints for you item.")
                   .AddOption("Ok~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LotteryCollectorWong)]
        public static void LotteryCollectorWong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~fond~of~collecting~all~kinds~of~treasure.~If~you~happen~to~have~any~Disguise Amulets,~Life Fruit Basket,~or")
                            .AddText("~PenitenceAmulets,~you~may~sell~them~to~me.~I~will~pay~you~ten~CPs~for~each~one.")
                    .AddOption("Here~are~my~Disguise~Amulets.", 1)
                    .AddOption("Here~are~my~Life~Fruit~Baskets.", 2)
                    .AddOption("Here~are~my~Penitence~Amulets.", 3)
                    .AddOption("Really?~I~don`t~believe~it.", 255)
                    .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(723724, 1))
                        {
                            if (!client.Inventory.Remove(723724, 1, stream)) break;
                            client.Player.ConquerPoints += 15;


                            data.AddText("You receive 15 ConquerPoints for you item.")
                   .AddOption("Ok~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(723725, 1))
                        {
                            if (!client.Inventory.Remove(723725, 1, stream)) break;
                            client.Player.ConquerPoints += 15;


                            data.AddText("You receive 15 ConquerPoints for you item.")
                   .AddOption("Ok~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(723727, 1))
                        {
                            if (!client.Inventory.Remove(723727, 1, stream)) break;
                            client.Player.ConquerPoints += 15;


                            data.AddText("You receive 15 ConquerPoints for you item.")
                   .AddOption("Ok~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LotteryLadyLuck)]
        public static void LotteryLadyLuck(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Look!~Here~is~the~LuckyBox,~a~kind~of~magical~box~which~makes~your~dream~come~true.");
                        if (client.Player.LotteryItem != null)
                            data.AddOption("Show~me~the~prize~I~drew.", 3);
                        data.AddOption("Get~me~out~of~here!", 1)
                                            .AddOption("Tell~me~more~about~it~please.", 2)
                                            .AddOption("Really?~I~don`t~believe~it.", 255)
                                            .AddAvatar(159).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        // a prize drawn but not accepted (e.g. before a logout): open the lottery window on it again
                        MsgServer.MsgLottery.SendShow(client, stream);
                        break;
                    }
                case 1:
                    {
                        client.Teleport(217, 193, 1036);
                        break;
                    }
                case 2:
                    {
                        data.AddText("You~have~chance~to~get~whatever~you~desire~from~a~LuckyBox.~It~May~be~a~Dragonball,~+8~weapon,~or")
                            .AddText("~two-socket~boot,~you~don`t~know~what~you~will~get~from~the~Box,~it~totally~depends~on~your~luck.~Well,")
                            .AddText("~let`s~get~started~now.~Good~luck!")
                   .AddOption("Thanks~a~lot.", 255)
                   .AddAvatar(123).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MarketLadyLuck)]
        public static void MarketLadyLuck(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome to the Lottery Center! There are many Lucky Boxes here, each containing incredible treasures! Pay me 3 Small Lottery Tickets to draw from the Lucky Box in the Lottery Center. You are allowed to enter the Lottery Center and try your luck upto 10")
                            .AddText("~times a day. You will have extra lottery chances if you are a VIP player.")
                    .AddOption("Let`s~try~my~luck!", 1)
                    .AddOption("Exchange~Small~Lottery~Ticket.", 2)
                    .AddOption("May~I~know~the~rules~first?", 3);
                        if (client.Player.LotteryItem != null)
                            data.AddOption("Show~me~the~prize~I~drew.", 5);
                            //.AddOption("How~about~the~VIP~privilege?", 4)
                        data.AddOption("Nice~to~see~you.", 255)
                    .AddAvatar(123).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        // a prize drawn but not accepted (e.g. before a logout): open the lottery window on it again
                        MsgServer.MsgLottery.SendShow(client, stream);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.Level >= 70)
                        {
                            client.Teleport(39, 45, 700);
                        }
                        else
                        {
                            data.AddText("Sorry, you need level 70.")
                 .AddOption("Oops.", 255)
                 .AddAvatar(123).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710212, 1))
                        {
                            // the new tickets need a free slot (a full bag used to eat the Lottery Ticket)
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory first.")
                                    .AddOption("Okay.", 255)
                                    .AddAvatar(123).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Remove(710212, 1, stream)) break;
                            // official itemtype 710212: "Swap it with Lady Luck (Market 212,188) for 3 Small Lottery Tickets."
                            client.Inventory.Add(stream, MsgServer.MsgLottery.SmallLotteryTicket, MsgServer.MsgLottery.SmallTicketsPerLotteryTicket);
                        }
                        else
                        {
                            data.AddText("You don`t have any Lottery Tickets to exchange for the Small Lottery Tickets.")
                   .AddOption("Oops.", 255)
                   .AddAvatar(123).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        data.AddText("There are many Lucky Boxes in the Lottery Center. Pay 3 Small Lottery Tickets, and you can draw from the Lucky Box! If you are not satisfied with the item you get, you can pay 1 more Small Lottery Ticket to change the item you drew. However, you can")
                            .AddText("~only pay and change the item for 2 times. Each player is allowed to draw from the Lottery up to 10 times, each day.")
                   .AddOption("Okay,~I~see.", 255)
                   .AddAvatar(123).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RebirthMaster)]
        public static void RebirthMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I devote all my life to the research of eternity. And finally I know the secret of rebirth! Well, anything I can help you with?")
                       .AddOption("1st~Rebirth.", 1)
                       .AddOption("2nd~Rebirth.", 4)
                       .AddOption("Reincarnation.", 5)
                       .AddOption("Reallot~my~Attribute~Points.", 8)
                       .AddOption("Just~passing~by.", 255)
                       .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        data.AddText("Reborn~level~70+~players~can~redistribute~their~attribute~points~at~the~cost~of~a~dragonball.")
                    .AddOption("I~will~reallot~my~points.", 9).AddOption("Let~me~think~it~over.", 255)
                    .AddAvatar(67).FinalizeDialog();


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
                case 5:
                    {
                        data.AddText("From now on, have total control of your life! If you've been Reborn twice and are at least Level 110, just bring me some Oblivion Dew and I'll help you become Reincarnated! After the Reincarnation, your class skills will revert to level 0,")
                                                    .AddText("~but they`ll return to their previous levels when you reach Level 110, again. If your classes have been the same for all three lives, including your current one, you can learn the Pure Skills from your class trainer,")
                                                    .AddText("~in the job center.")
                                          .AddOption("I~want~to~get~Reincarnated.", 6)
                                          .AddOption("Can~I~get~the~Dew[5k]?", 7)
                                          .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 5000)
                            {
                                client.Player.ConquerPoints -= 5000;

                                client.Inventory.Add(stream, 711083);
                            }
                            else
                                data.AddText("Please make sure u have 5.000 Cps to get this item.")
                       .AddOption("Let~me~think.", 255)
                      .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
                case 6:
                    {
                        if (client.Player.Reborn == 2 && client.Inventory.Contain(711083, 1))
                        {
                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.Reincarnation,
                                wParam1 = client.Player.X,
                                wParam2 = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(&action));


                        }
                        else
                        {
                            data.AddText("You have not been Reborn twice. I`m afraid you will not withstand the power of the Oblivion Dew. Come to me when you get stronger.")
                     .AddOption("Got~it!~Thanks!", 255)
                     .AddAvatar(67).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Reborn == 0 && (client.Player.Level >= 110 && client.Player.Class == 135 || client.Player.Level >= 120 & client.Player.Class % 10 == 5))
                        {
                            if (client.Inventory.Contain(721259, 1) || client.Inventory.Contain(721259, 1, 1))
                            {
                                data.AddText("There are two kinds of reborns. One is the normal one and the second one is blessed.")
                                    .AddText("The normal reborn will give you the chance to get a Super Gem and the blessed reborn will set a -1 into one of your equipment that you wear during the reborn. What do you chose?")
                             .AddOption("Normal reborn.", 2)
                             .AddOption("Blessed reborn.", 3)
                             .AddOption("I`ll~think~about~it.", 255)
                             .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you don't have CelestialStone.")
                      .AddOption("I~see.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You~haven`t~reached~the~highest~rank~yet.~Please~work~harder. You cannot reborn if your level is not 110+ for water saints and 120+ for other masters.")
                        .AddOption("I~see.", 255)
                        .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        if (client.Player.Reborn == 1 && (client.Player.Level >= 110 && client.Player.Class == 135 || client.Player.Level >= 120 & client.Player.Class % 10 == 5))
                        {
                            if (client.Inventory.Contain(723701, 1) || client.Inventory.Contain(723701, 1, 1))
                            {
                                data.AddText("You don't worry, it's not gonna hurt you a bit when getting the third life. You just need an exemption token and it's all going to be fine.")
                             .AddOption("Here is the ExemptionToken.", 3)
                             .AddOption("I`ll~think~about~it.", 255)
                             .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you don't have ExemptionToken.")
                      .AddOption("I~see.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry,~you~are~ineligible~for~second~rebirth.~Please~wait~until~you~are~a~level~120~reborn~person.")
                        .AddOption("I~see.", 255)
                        .AddAvatar(67).FinalizeDialog();
                        }



                        break;
                    }
                case 2:
                    {
                        data.AddText("Select the super gem you desire.")
                      .AddOption("SuperPhoenixGem.", 203)
                      .AddOption("SuperDragonGem.", 213)
                      .AddOption("SuperFuryGem.", 223)
                      .AddOption("SuperRainbowGem.", 233)
                      .AddOption("SuperVioletGem.", 253)
                      .AddOption("SuperMoonGem.", 254)
                      .AddOption("SuperKylinGem", 243)
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 203:
                case 213:
                case 223:
                case 233:
                case 253:
                case 254:
                case 243:
                    {
                        if (client.Player.Reborn == 0)
                        {
                            client.RebornGem = (uint)(Option % 100);
                            if (client.RebornGem == 254)
                                client.RebornGem = 263;
                        }
                        data.AddText("Select the class you want to reborn in.")
                      .AddOption("Trojan.", 11)
                      .AddOption("Warrior.", 21)
                      .AddOption("Archer.", 41)
                      .AddOption("WaterTaoist.", 132)
                      .AddOption("FireTaoist.", 142)
                      .AddOption("Ninja.", 51)
                      .AddOption("Monk", 61)
                        .AddOption("Pirate", 71)
                        .AddOption("Dragon~Warrior", 81)
                        .AddOption("WindWalker", 161)
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Select the class you want to reborn in.")
                    .AddOption("Trojan.", 11)
                    .AddOption("Warrior.", 21)
                    .AddOption("Archer.", 41)
                    .AddOption("WaterTaoist.", 132)
                    .AddOption("FireTaoist.", 142)
                    .AddOption("Ninja.", 51)
                    .AddOption("Monk", 61)
                      .AddOption("Pirate", 71)
                       .AddOption("Dragon~Warrior", 81)
                       .AddOption("WindWalker", 161)
                    .AddOption("I`ll~think~about~it.", 255)
                    .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 11:
                case 21:
                case 41:
                case 132:
                case 142:
                case 51:
                case 161:
                case 61:
                case 81:
                case 71:
                    {

                        if (!client.Inventory.HaveSpace(4))
                        {
                            data.AddText("Sorry, but you need atleast 4 free spaces in your inventory.")
            .AddOption("I~see.", 255)
            .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Reborn == 0 && (client.Player.Level >= 110 && client.Player.Class == 135 || client.Player.Level >= 120 & client.Player.Class % 10 == 5))
                        {
                            if (client.Inventory.Contain(721259, 1) || client.Inventory.Contain(721259, 1, 1))
                            {
                                if (!client.Inventory.Remove(721259, 1, stream)) break;
                                if (client.RebornGem != 0)
                                {
                                    client.Inventory.Add(stream, (uint)(700000 + client.RebornGem), 1);
                                    client.RebornGem = 0;

                                }
                                else
                                {
                                    foreach (var item in client.Equipment.CurentEquip)
                                    {
                                        if (item.Position != (ushort)Role.Flags.ConquerItem.Steed || item.Position != (ushort)Role.Flags.ConquerItem.Wing)
                                        {
                                            if (item.Bless == 0)
                                            {
                                                item.Bless = 1;
                                                item.Mode = Role.Flags.ItemMode.Update;
                                                item.Send(client, stream);
                                                break;
                                            }
                                        }
                                    }
                                }
                                switch (Option)
                                {
                                    case 161:
                                    case 11:
                                    case 21:
                                    case 51:
                                    case 61:
                                    case 71:
                                    case 81:
                                        {
                                            client.Inventory.Add(stream, 410077, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.Poison);
                                            break;
                                        }
                                    case 41:
                                        {
                                            client.Inventory.Add(stream, 410077, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.Poison);
                                            break;
                                        }
                                    case 132:
                                    case 142:
                                        {
                                            client.Inventory.Add(stream, 421077, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.MP);
                                            break;
                                        }
                                }

                                Database.Server.RebornInfo.Reborn(client.Player, Option, stream);
                            }
                            else
                            {
                                data.AddText("Sorry, you don't have CelatialStone.")
                      .AddOption("I~see.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else if (client.Player.Reborn == 1 && (client.Player.Level >= 110 && client.Player.Class == 135 || client.Player.Level >= 120 & client.Player.Class % 10 == 5))
                        {
                            if (client.Inventory.Contain(723701, 1) || client.Inventory.Contain(723701, 1, 1))
                            {
                                if (!client.Inventory.Remove(723701, 1, stream)) break;
                                Database.Server.RebornInfo.Reborn(client.Player, Option, stream);
                            }
                            else
                            {
                                data.AddText("Sorry, you don't have ExemptionToken.")
                      .AddOption("I~see.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            }
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MountRetailer)]
        public static void MountRetailer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MountTrainer, client.Player.Class, 702);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Wow, it seems that I need to get some more steeds for all of you guys to choose! Maybe I`ll be a millionaire. It`s just a")
                               .AddText("~matter of time.")
                              .AddOption("Yeah.~Tell~me~more.", 2)
                              .AddOption("I~want~to~buy~a~Steed.", 1)
                              .AddAvatar(15).FinalizeDialog();
                        }
                        else
                            goto case 1;

                        break;
                    }
                case 1:
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
                case 2:
                    {
                        data.AddText("There are a few kinds of steeds here, with many more that you can get. A good steed is a pretty convenient thing to have.")
                               .AddText("~Able to take you out for relaxing rides around the country, or ready to help you when you are fighting for your life!")
                              .AddOption("Looking~forward~to~it!", 3)
                              .AddAvatar(15).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You can unite 2 mounts with different appearances to breed a new mount. In this way, you can also improve on your steed`s")
                                  .AddText("~lineage. You can talk to the SteedKeeper beside me if you want to learn more about it.")
                                 .AddOption("Sure.", 4)
                                 .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.CreateBoxDialog("You`ve~learned~something~about~steeds~and~received~EXP~worth~1/12~of~an~EXP~Ball!");
                            client.GainExpBall(100, true, Role.Flags.ExperienceEffect.bless);
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MountTrainer)]
        public static void MountTrainer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MountTrainer, client.Player.Class, 702);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level > 25)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("Hey! Guys, look at me! Good Steeds! Nah, not me. I mean there are some good steeds for you to choose. Would you like to take a")
                            .AddText("~look?")
                           .AddOption("Steed?~Why~do~I~need~it?", 20)
                           .AddOption("I~want~to~learn~to~ride.", 10)
                           .AddAvatar(15).FinalizeDialog();
                            }
                            else
                            {
                                goto case 10;
                            }
                        }
                        else
                        {

                            data.AddText("I`m the best Mount Trainer in the Empire. You can learn the Mount Skills from me. It costs 50,000 silver for the Riding skill,")
                                .AddText("~100,000 silver for the Spook skill, and 1,000,000 silver for the War Cry skill. Is there something you needed?")
                                .AddOption("Learn~Riding" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Riding) ? "(Learnt)" : "(50,000~silver)") + "", 1)
                                .AddOption("Learn~Spook" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Spook) ? "(Learnt)" : "(100,000~silver)") + "", 3)
                                .AddOption("Learn~War~Cry" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarCry) ? "(Learnt)" : "(1,000,000~silver)") + "", 5)
                                .AddOption("ChargingVortex" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChargingVortex) ? "(Learnt)" : "(500,000~silver)") + "", 7)
                                .AddOption("Just~passing~by.", 255)
                                .AddAvatar(15).FinalizeDialog();
                        }


                        break;
                    }
                case 20:
                    {
                        data.AddText("A good steed can increase your movement and efficiency. Why not pay a visit to the")
                           .AddText("~Steed Dealer behind me to learn about the different kinds of steeds?")
                          .AddOption("As~you~wish.", 21)
                          .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 21:
                    {
                        data.AddText("Pay~a~visit~to~the~Mount~Retailer.")
                             .AddOption("Yes.", 22)
                             .AddOption("No.", 255)
                             .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 22:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                        break;
                    }
                case 10:
                    {
                        data.AddText("I`m the best Mount Trainer in the Empire. You can learn the Mount Skills from me. It costs 50,000 silver for the Riding skill,")
                                                       .AddText("~100,000 silver for the Spook skill, and 1,000,000 silver for the War Cry skill. Is there something you needed?")
                                                       .AddOption("Learn~Riding" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Riding) ? "(Learnt)" : "(50,000~silver)") + "", 1)
                                                       .AddOption("Learn~Spook" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Spook) ? "(Learnt)" : "(100,000~silver)") + "", 3)
                                                       .AddOption("Learn~War~Cry" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarCry) ? "(Learnt)" : "(1,000,000~silver)") + "", 5)
                                                       .AddOption("ChargingVortex" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChargingVortex) ? "(Learnt)" : "(500,000~silver)") + "", 7)
                                                       .AddOption("Just~passing~by.", 255)
                                                       .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        data.AddText("It`s upgradable. You`ll need to equip a two-handed weapon and be mounted to use this skill. You will attack a target area within 15 paces and cause")
                         .AddText("~70%damage on all targets within 3 paces. It costs 30 Stamina. Are you sure you want to pay 500,000 silver to learn it?")
                        .AddOption("Yes.", 8)
                        .AddOption("I`ll~think~about~it.", 255)
                        .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Player.Level < 30)
                        {
                            data.AddText("Sorry, but you`ll need to reach Level 30, first.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(15).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Money >= 500000)
                        {
                            var SpellID = Role.Flags.SpellID.ChargingVortex;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(15).FinalizeDialog();
                                break;
                            }
                            client.Player.Money -= 500000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(15).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You don`t have 500,000 silver. You are unable to learn the ChargingVortex skill.").AddOption("Oh,~I`ll~fetch~some~money!", 255).AddAvatar(15).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("After you equip and ride your mount, you can right click the War Cry skill to dismount the players in a certain area whose")
                          .AddText("~mounts` Lineage Levels are lower than yours and force the Vigors of their mounts drop to 0. Cool down time: 60 seconds.")
                         .AddOption("Learn~War~Cry.", 6)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(15).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (client.Player.Money >= 1000000)
                        {
                            var SpellID = Role.Flags.SpellID.WarCry;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(15).FinalizeDialog();
                                break;
                            }
                            client.Player.Money -= 1000000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(15).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You don`t have 1,000,000 silver. You are unable to learn the war cry skill.").AddOption("Oh,~I`ll~fetch~some~money!", 255).AddAvatar(15).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("After you equip and ride your mount, you can right click the Spook skill to dismount a player whose mount`s Lineage Level is")
                          .AddText("~lower than yours and force the Vigor of his mount drop to 0. Cool down time: 15 seconds.")
                         .AddOption("Learn~Spook.", 4)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Player.Money >= 100000)
                        {
                            var SpellID = Role.Flags.SpellID.Spook;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(15).FinalizeDialog();
                                break;
                            }
                            client.Player.Money -= 100000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(15).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You don`t have 100,000 silver. You are unable to learn the Spook skill.").AddOption("Oh,~I`ll~fetch~some~money!", 255).AddAvatar(15).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        data.AddText("After the mount is equipped, you can right click the (Riding) button or drag the button into the shortcut column to ride")
                           .AddText("~your mount. The higher the Lineage Level, the higher Vigor and the less limits for your mount!")
                          .AddOption("Learn~Riding~skill.", 2)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.Money >= 50000)
                        {
                            var SpellID = Role.Flags.SpellID.Riding;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(15).FinalizeDialog();
                                break;
                            }
                            client.Player.Money -= 50000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(15).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You don`t have 50,000 silver. You are unable to learn the Riding skill.").AddOption("Oh,~I`ll~fetch~some~money!", 255).AddAvatar(15).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
    }
}
