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
        [NpcAttribute(NpcID.Performer)]
        public static void Performer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello~there!~Please~allow~me~to~introduce~the~sub-class~of~Performers~to~you.~Performers~are~gifted~in~the~harmonizing~of~song~and~swordplay~into~beautiful,~unique~dances.~These~dances~will~surely~make~you~the~center~of~people`s~attention!");
                        data.AddText("~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~learn~more~stylish~dances.");
                        data.AddText("Performers~are~gifted~in~the~harmonizing~of~song~and~swordplay~into~beautiful,~unique~dances.~Besides~that,~their~physical~and~magical~attacks~improve~when~their~Sub-Class~level~improves.");
                        data.AddOption("Yes,~I`d~like~to~join!", 20);
                        data.AddOption("I~want~to~get~promoted.", 40);
                        data.AddOption("I~want~to~learn~new~dances.", 3);
                        data.AddOption("How~can~I~get~promoted?", 1);
                        data.AddOption("Nice.~I`ll~think~about~it.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 1)
                        {
                            data.AddText("Well,~you~need~to~join~us~before~you~can~be~promoted.~Would~you~like~to~become~a~Performer~now?");
                            data.AddOption("Sure.", 20);
                            data.AddOption("Oh,~I`m~not~ready~yet.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~learn~more~dances,~you~should~have~a~character~level~of~90~and~become~a~level~2~Performer.~Have~you~met~all~these~requirements?");
                                data.AddOption("Positive.", 41);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~learn~more~dances,~you~should~have~a~character~level~of~110~and~become~a~level~3~Performer.~Have~you~met~all~these~requirements?");
                                    data.AddOption("Positive.", 42);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~learn~more~dances,~you~should~have~a~character~level~of~120~and~become~a~level~4~Performer.~Have~you~met~all~these~requirements?");
                                        data.AddOption("Positive.", 43);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~learn~more~dances,~you~should~have~a~character~level~of~90,~become~a~level~5~Performer,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                            data.AddOption("Positive.", 44);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(110);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~learn~more~dances,~you~should~have~a~character~level~of~110,~become~a~level~6~Performer,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddOption("Positive.", 45);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(110);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~learn~more~dances,~you~should~have~a~character~level~of~120,~become~a~level~7~Performer,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Positive.", 46);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(110);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~learn~more~dances,~you~should~have~a~character~level~of~120,~become~a~level~8~Performer,~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Positive.", 47);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(110);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~learn~more~dances,~you~should~have~a~character~level~of~130,~become~a~level~9~Performer,~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Positive.", 48);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(110);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Oh.~Thanks~anyway.");
                                                            data.AddAvatar(110);
                                                            data.FinalizeDialog();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 48:
                    {
                        if (client.Player.Level < 130)
                        {
                            data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 9)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~9~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~9~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 47:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 8)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~8~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~8~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 46:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 7)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~7~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~have~been~promoted~to~a~Phase~7~Performer!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 7)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~7~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~7~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }

                        break;
                    }
                case 45:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 6)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~6~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~have~been~promoted~to~a~Phase~6~Performer!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 6)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~6~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~6~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 44:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 5)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~5~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~have~been~promoted~to~a~Phase~5~Performer!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 5)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~5~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~5~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 43:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 4)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~4~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~4~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 4)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~4~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~have~been~promoted~to~a~Phase~4~Performer!");
                                data.AddOption("Great.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 42:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 3)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~3~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~3~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 3)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~3~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~have~been~promoted~to~a~Phase~3~Performer!");
                                data.AddOption("Great.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 41:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 2)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~2~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~2~Performer!");
                                    data.AddOption("Great!");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 2)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~2~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~have~been~promoted~to~a~Phase~2~Performer!");
                                data.AddOption("Great!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 20:
                    {

                        data.AddText("Performers~always~attract~public~attention~by~unique~Dances.~If~you`ve~reached~Level~70~and~you~want~to~join~us,~bring~me~15~Orchids~or~800~Study~Points.");
                        data.AddText("~it~requires~a~Performer~Card~which~can~be~obtained~by~handing~in~15~Orchids.~After~joining~the~Performer~sub-class,~click~open~your~character~sheet~and~you`ll~find~the~sub-class~button~on~the~left-hand~corner~where~you~may~level~up~your~sub-class.");
                        data.AddOption("Pay~15~Orchids~to~join.", 23);
                        data.AddOption("Pay~800~Study~Points~to~join.", 21);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();

                        break;
                    }
                case 23:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Now~please~leave~me~alone,~I`ve~got~some~dancing~classes~to~prepare~for.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(711679, 1))
                                    {
                                        data.AddText("Hmmm,~I`ve~accepted~the~15~Orchids~you~just~handed~in~and~here~is~your~Performer~Card.~Well,~if~you`ve~reached~level~70,~you~will~become~a~Performer~immediately.~Are~you~sure~you~want~to~join~us?");
                                        data.AddOption("Sure.", 25);
                                        data.AddOption("I`ve~changed~my~mind.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddText("How~many~orchids~would~you~like~to~hand~in~this~time?~It~takes~15~orchids~to~get~a~Performer~Card~which~is~required~to~join~the~Performers.");
                                        data.AddOption("1~Orchid.", 26);
                                        data.AddOption("3~Orchids.", 28);
                                        data.AddOption("9~Orchids.", 29);
                                        data.AddOption("How~many~have~I~given~you?", 30);
                                        data.AddOption("Just~passing~by.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(711679, 1))
                                {
                                    data.AddText("Hmmm,~I`ve~accepted~the~15~Orchids~you~just~handed~in~and~here~is~your~Performer~Card.~Well,~if~you`ve~reached~level~70,~you~will~become~a~Performer~immediately.~Are~you~sure~you~want~to~join~us?");
                                    data.AddOption("Sure.", 25);
                                    data.AddOption("I`ve~changed~my~mind.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("How~many~orchids~would~you~like~to~hand~in~this~time?~It~takes~15~orchids~to~get~a~Performer~Card~which~is~required~to~join~the~Performers.");
                                    data.AddOption("1~Orchid.", 26);
                                    data.AddOption("3~Orchids.", 28);
                                    data.AddOption("9~Orchids.", 29);
                                    data.AddOption("How~many~have~I~given~you?", 30);
                                    data.AddOption("Just~passing~by.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }

                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Performer,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        if (client.Player.GiveFlowersToPerformer >= 1)
                        {
                            data.AddText("You`ve~handed~in~" + client.Player.GiveFlowersToPerformer + "~Orchids.~As~you~meet~the~requirement,~you`ll~get~a~Performer~Card.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~haven`t~handed~in~any~Orchids~yet.~It~charges~15~Orchids~for~a~Performer~Card.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 29:
                    {
                        if (client.Inventory.Contain(753009, 1))
                        {
                            if (!client.Inventory.Remove(753009, 1, stream)) break;
                            client.Player.GiveFlowersToPerformer += 9;
                            if (client.Player.GiveFlowersToPerformer >= 15)
                            {
                                client.Player.GiveFlowersToPerformer -= 15;
                                client.Inventory.Add(stream, 711679, 1);
                                data.AddText("Hmmm,~I`ve~accepted~the~15~Orchids~you~just~handed~in~and~here~is~your~Performer~Card.~Well,~if~you`ve~reached~level~70,~you~will~become~a~Performer~immediately.~Are~you~sure~you~want~to~join~us?");
                                data.AddOption("Sure.", 27);
                                data.AddOption("I`ve~changed~my~mind.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You`ve~handed~in~" + client.Player.GiveFlowersToPerformer + "~Orchids.~As~there~are~15~Orchids,~you~can~get~a~Performer~Card.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You~don`t~have~9~Orchids~to~give.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Contain(753003, 1))
                        {
                            if (!client.Inventory.Remove(753003, 1, stream)) break;
                            client.Player.GiveFlowersToPerformer += 3;
                            if (client.Player.GiveFlowersToPerformer >= 15)
                            {
                                client.Player.GiveFlowersToPerformer -= 15;
                                client.Inventory.Add(stream, 711679, 1);

                                data.AddText("Hmmm,~I`ve~accepted~the~15~Orchids~you~just~handed~in~and~here~is~your~Performer~Card.~Well,~if~you`ve~reached~level~70,~you~will~become~a~Performer~immediately.~Are~you~sure~you~want~to~join~us?");
                                data.AddOption("Sure.", 27);
                                data.AddOption("I`ve~changed~my~mind.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {

                                data.AddText("You`ve~handed~in~" + client.Player.GiveFlowersToPerformer + "~Orchids.~As~there~are~15~Orchids,~you~can~get~a~Performer~Card.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("~You~don`t~have~3~Orchids~to~give.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 26:
                    {
                        if (client.Inventory.Contain(753001, 1))
                        {
                            if (!client.Inventory.Remove(753001, 1, stream)) break;
                            client.Player.GiveFlowersToPerformer += 1;
                            if (client.Player.GiveFlowersToPerformer >= 15)
                            {
                                client.Player.GiveFlowersToPerformer -= 15;
                                client.Inventory.Add(stream, 711679, 1);
                                data.AddText("Hmmm,~I`ve~accepted~the~15~Orchids~you~just~handed~in~and~here~is~your~Performer~Card.~Well,~if~you`ve~reached~level~70,~you~will~become~a~Performer~immediately.~Are~you~sure~you~want~to~join~us?");
                                data.AddOption("Sure.", 27);
                                data.AddOption("I`ve~changed~my~mind.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You`ve~handed~in~" + client.Player.GiveFlowersToPerformer + "~Orchids.~As~there~are~15~Orchids,~you~can~get~a~Performer~Card.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("You~don`t~have~an~Orchid~to~give~me.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Contain(711679, 1))
                        {
                            if (!client.Inventory.Remove(711679, 1, stream)) break;
                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Performer, client, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                            Role.Core.SendGlobalMessage(stream, "Excellent!~" + client.Player.Name + "~is~now~a~beautiful~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                            data.AddText("Welcome~to~the~dancing~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~performers.~Work~hard~in~the~future,~and~when~you`re~ready~to~get~promoted,~I`m~always~here~for~you.");
                            data.AddOption("Thank~you.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 25:
                    {
                        if (client.Inventory.Contain(711679, 1))
                        {
                            if (!client.Inventory.Remove(711679, 1, stream)) break;
                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Performer, client, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                            Role.Core.SendGlobalMessage(stream, "Excellent!~" + client.Player.Name + "~is~now~a~beautiful~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                            data.AddText("Welcome~to~the~dancing~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~performers.~Work~hard~in~the~future,~and~when~you`re~ready~to~get~promoted,~I`m~always~here~for~you.");
                            data.AddOption("Thank~you.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("How~many~orchids~would~you~like~to~hand~in~this~time?~It~takes~15~orchids~to~get~a~Performer~Card~which~is~required~to~join~the~Performers.");
                            data.AddOption("1~Orchid.", 26);
                            data.AddOption("3~Orchids.", 28);
                            data.AddOption("9~Orchids.", 29);
                            data.AddOption("How~many~have~I~given~you?", 30);
                            data.AddOption("Just~passing~by.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 22);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Now~please~leave~me~alone,~I`ve~got~some~dancing~classes~to~prepare~for.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Performer, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "Excellent!~" + client.Player.Name + "~is~now~a~beautiful~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~dancing~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~performers.~Work~hard~in~the~future,~and~when~you`re~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thank~you.");
                                            data.AddAvatar(110);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (client.Player.SubClass.StudyPoints < 800)
                                {
                                    data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Performer, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "Excellent!~" + client.Player.Name + "~is~now~a~beautiful~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~dancing~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~performers.~Work~hard~in~the~future,~and~when~you`re~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thank~you.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Performer,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("It~takes~higher~phases~to~learn~more~advanced~dances.~Learn~the~Battle~Dance~at~Phase~1,~learn~the~Triumph~at~Phase~3,~learn~the~Step~Stomp~at~Phase~5,~learn~Moonlight~at~Phase~7,~and~Snow~Wind~at~Phase~9.");
                        data.AddText("~Which~of~these~dances~do~you~want~to~learn?");
                        data.AddOption("Learn~Partner~Dances.", 9);
                        data.AddOption("The~Battle~Dance.", 8);
                        data.AddOption("Triumph.", 7);
                        data.AddOption("The~Step~Stomp.", 6);
                        data.AddOption("Moonlight.", 5);
                        data.AddOption("Snow~Wind.", 4);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        data.AddText("The~dance~skill~you`re~able~to~learn~depends~on~your~Performer~Phase.~The~higher~your~phase~is,~the~more~dances~you~can~learn.~So,~what~kind~of~dance~would~you~like~to~learn?");
                        data.AddOption("Learn~Phoenix.~(P3~Performer)", 12);
                        data.AddOption("Melody~(Phase~6).", 11);
                        data.AddOption("Tipsy~(Phase~9).", 10);
                        data.AddOption("Previous.", 3);
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 3)
                        {
                            data.AddText("I`m~sorry,~but~only~Phase~3~Performers~can~learn~this~dance.~I~can`t~teach~you~this~dance~now.");
                            data.AddOption("Alright.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1420))
                            {
                                data.AddText("But~you`ve~already~learned~this~dance,~remember?");
                                data.AddOption("Oh,~alright.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1420);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Partner~Dance,~Phoenix!");
                                data.AddText("You`ve~learned~the~Partner~Dance,~Phoenix!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 6)
                        {
                            data.AddText("I`m~sorry,~but~only~Phase~6~Performers~can~learn~this~dance.~I~can`t~teach~you~this~dance~now.");
                            data.AddOption("Alright.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1421))
                            {
                                data.AddText("But~you`ve~already~learned~this~dance,~remember?");
                                data.AddOption("Oh,~alright.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1421);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Partner~Dance,~Melody!");
                                data.AddText("You`ve~learned~the~Partner~Dance,~Melody!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 9)
                        {
                            data.AddText("I`m~sorry,~but~only~Phase~9~Performers~can~learn~this~dance.~I~can`t~teach~you~this~dance~now.");
                            data.AddOption("Alright.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1422))
                            {
                                data.AddText("But~you`ve~already~learned~this~dance,~remember?");
                                data.AddOption("Oh,~alright.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1422);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Partner~Dance,~Tipsy!");
                                data.AddText("You`ve~learned~the~Partner~Dance,~Tipsy!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 1)
                        {
                            data.AddText("You~can~only~learn~it~at~Phase~1.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1415))
                            {
                                data.AddText("You`ve~already~learned~this~dance.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1415);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Battle~Dance.");
                                data.AddText("You`ve~learned~the~Battle~Dance.~Hope~you`ll~work~on~it~and~learn~more~dances~in~the~future.~See~you!");
                                data.AddOption("See~you!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 3)
                        {
                            data.AddText("You~can~only~learn~it~at~Phase~3.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1416))
                            {
                                data.AddText("You`ve~already~learned~this~dance.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1416);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Triumph.");
                                data.AddText("You`ve~learned~the~Triumph.~Hope~you`ll~work~on~it~and~learn~more~dances~in~the~future.~See~you!");
                                data.AddOption("See~you!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 5)
                        {
                            data.AddText("You~can~only~learn~it~at~Phase~5.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1417))
                            {
                                data.AddText("You`ve~already~learned~this~dance.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1417);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Step~Stomp.");
                                data.AddText("You`ve~learned~the~Step~Stomp.~Hope~you`ll~work~on~it~and~learn~more~dances~in~the~future.~See~you!");
                                data.AddOption("See~you!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 7)
                        {
                            data.AddText("You~can~only~learn~it~at~Phase~7.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1418))
                            {
                                data.AddText("You`ve~already~learned~this~dance.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1418);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Moonlight.");
                                data.AddText("You`ve~learned~the~Moonlight.~Hope~you`ll~work~on~it~and~learn~more~dances~in~the~future.~See~you!");
                                data.AddOption("See~you!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 9)
                        {
                            data.AddText("You~can~only~learn~it~at~Phase~9.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1419))
                            {
                                data.AddText("You`ve~already~learned~this~dance.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1419);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Snow~Wind.");
                                data.AddText("You`ve~learned~the~Snow~Wind.~You~are~the~master~of~dances~now!");
                                data.AddOption("Yay!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Good~day!~Please~spare~me~a~few~minutes~so~that~I~can~tell~you~about~the~sub-classes~in~Twin~City.~Each~Sub-Class~has~9~phases,~and~the~higher~the~phase~is,~the~more~bonus~effects~the~sub-class~will~offer.~But~keep~in~mind~that~these");
                        data.AddText("~sub-classes~also~have~a~level~system~and~you~need~to~level~them~up~before~you~can~get~promoted!~You~can~level~up~your~sub-class~by~gaining~study~points.");
                        data.AddOption("How~can~I~get~study~points?", 2);
                        data.AddOption("Oh.~Thanks~for~telling~me.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~your~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("I~see.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ApothecarySubClass)]
        public static void ApothecarySubClass(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello~there!~Please~allow~me~to~introduce~to~you~the~sub-class~of~the~Apothecary.~An~Apothecary~is~a~master~of~various~oriental~herbs~and~can~use~their~healing~powers~to~protect~themselves~from~poison~damage.");
                        data.AddText("~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~gain~more~protection.");
                        data.AddOption("Yes,~I`d~like~to~join!", 3);
                        data.AddOption("I~want~to~get~promoted.", 7);
                        data.AddOption("How~can~I~get~promoted?", 1);
                        data.AddOption("Nice.~I`ll~think~about~it.");
                        data.AddAvatar(113);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) < 1)
                        {
                            data.AddText("You~are~not~an~Apothecary~yet~and~can`t~get~promoted.~Do~you~want~to~join~the~Apothecary~sub-class~now?");
                            data.AddOption("Yes,~I`d~like~to~join.", 3);
                            data.AddOption("Oh.~Not~now.");
                            data.AddAvatar(113);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~increase~more~poison~damage~reduced,~you~should~have~a~character~level~of~90~and~become~a~level~2~Apothecary.~Have~you~met~all~these~requirements?");
                                data.AddOption("Positive.", 8);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~110~and~become~a~level~3~Apothecary.~Have~you~met~all~these~requirements?");
                                    data.AddOption("Positive.", 9);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~120~and~become~a~level~4~Apothecary.~Have~you~met~all~these~requirements?");
                                        data.AddOption("Positive.", 10);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~90~and~become~a~level~5~Apothecary.~Have~you~met~all~these~requirements?");
                                            data.AddOption("Positive.", 11);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(113);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~110,~~become~a~level~6~Apothecary~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddOption("Positive.", 12);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(113);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~120,~~become~a~level~7~Apothecary~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Positive.", 13);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(113);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~120,~~become~a~level~8~Apothecary~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Positive.", 14);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(113);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~reduce~poison~damage~fully,~you~should~have~a~character~level~of~130,~~become~a~level~9~Apothecary~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Positive.", 15);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(113);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Oh,~thanks~anyway.");
                                                            data.AddAvatar(113);
                                                            data.FinalizeDialog();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 15:
                    {
                        if (client.Player.Level < 130)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(113);
                            data.AddAvatar(113);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 9)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~9~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~9~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(113);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 8)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~8~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~8~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 7)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~7~Apothecary~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 7)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~7~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 12:
                    {

                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 6)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~6~Apothecary~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 6)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~6~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }

                case 11:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 5)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~5~Apothecary~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 5)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~5~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 4)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~4~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 4)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~4~Apothecary~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 3)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 3)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Apothecary~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 2)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Apothecary~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("The~Apothecaries~are~quite~good~at~protecting~themselves~from~poison~damage.~If~you~want~to~join~us,~you~should~have~reached~Level~70~and~pay~me~10~Meteors~or~800~Study~Points.");
                        data.AddText("~After~that,~you~can~start~your~sub-class~training~on~the~`Status`~sheet.~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~10~Meteors~to~join.", 6);
                        data.AddOption("Pay~800~Study~Points~to~join.", 4);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(113);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(1088001, 10))
                                    {
                                        if (client.Inventory.Remove(1088001, 10, stream))
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Apothecary, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~an~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Apothecary~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Apothecaries.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thank~you.");
                                            data.AddAvatar(113);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("You~don`t~have~10~Meteor.~There~is~nothing~I~can~do~for~you.");
                                        data.AddOption("Oh.~What~a~pity.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(1088001, 10))
                                {
                                    if (client.Inventory.Remove(1088001, 10, stream))
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Apothecary, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~an~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Apothecary~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Apothecaries.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thank~you.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~10~Meteor.~There~is~nothing~I~can~do~for~you.");
                                    data.AddOption("Oh.~What~a~pity.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~an~Apothecary,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(113);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 5);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(113);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Apothecary, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~an~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Apothecary~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Apothecaries.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thank~you.");
                                            data.AddAvatar(113);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (client.Player.SubClass.StudyPoints < 800)
                                {
                                    data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Apothecary, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~an~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Apothecary~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Apothecaries.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thank~you.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~an~Apothecary,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(113);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Good~day!~Please~spare~me~a~few~minutes~so~that~I~can~tell~you~about~the~sub-classes~in~Twin~City.~Each~Sub-Class~has~9~phases,~and~the~higher~the~phase~is,~the~more~bonus~effects~the~sub-class~will~offer.~But~keep~in~mind~that~these");
                        data.AddText("~sub-classes~also~have~a~level~system~and~you~need~to~level~them~up~before~you~can~get~promoted.~You~can~level~up~your~sub-class~by~gaining~study~points.");
                        data.AddOption("How~can~I~get~study~points?", 2);
                        data.AddOption("Oh.~Thanks~for~telling~me.");
                        data.AddAvatar(113);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~the~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("I~see.");
                        data.AddAvatar(113);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MartialArtist)]
        public static void MartialArtist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello~there!~You~look~quite~capable.~Perhaps~you`ll~make~a~great~Martial~Artist.~Please~allow~me~to~explain~the~Martial~Artist~sub-class.~The~Martial~Artist`s~awesome~displays~of~combat~prowess~are~a~wonder~to~behold,~but~a~nightmare~to~face.");
                        data.AddText("~When~you~are~a~Martial~Artist,~you~will~be~able~to~increase~the~chances~of~hitting~with~1.5x~Attack~Power!~");
                        data.AddText("~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~increase~your~chances~of~hitting~your~enemies~with~more~power!");
                        data.AddOption("Yes,~I`d~like~to~join.", 3);
                        data.AddOption("I~want~to~get~promoted.", 7);
                        data.AddOption("How~can~I~get~promoted?", 1);
                        data.AddOption("Nice.~I`ll~think~about~it.");
                        data.AddAvatar(32);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) < 1)
                        {
                            data.AddText("You~are~not~a~Martial~Artist~yet~and~can`t~get~promoted.~Do~you~want~to~join~the~Martial~Artist~sub-class~now?");
                            data.AddOption("Yes,~I`d~like~to~join.", 3);
                            data.AddOption("Oh.~Not~now.");
                            data.AddAvatar(32);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~increase~the~chance~of~making~1.5x~Attack,~you~should~have~a~character~level~of~90~and~become~a~level~2~Martial~Artist.~Have~you~met~all~these~requirements?");
                                data.AddOption("Positive.", 8);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~110~and~become~a~level~3~Martial~Artist.~Have~you~met~all~these~requirements?");
                                    data.AddOption("Positive.", 9);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~120~and~become~a~level~4~Martial~Artist.~Have~you~met~all~these~requirements?");
                                        data.AddOption("Positive.", 10);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~90,~become~a~level~5~Martial~Artist,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                            data.AddOption("Positive.", 11);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(32);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~110,~become~a~level~6~Martial~Artist,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddOption("Positive.", 12);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(32);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~120,~become~a~level~7~Martial~Artist,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Positive.", 13);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(32);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~120,~become~a~level~8~Martial~Artist,~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Positive.", 14);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(32);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~130,~become~a~level~9~Martial~Artist,~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Positive.", 15);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(32);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Thank~you.");
                                                            data.AddAvatar(32);
                                                            data.FinalizeDialog();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 15:
                    {
                        if (client.Player.Level < 130)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(32);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 9)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~9~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~9~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(32);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 8)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~8~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~8~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 7)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~7~Martial~Artist~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 7)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~7~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 6)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~6~Martial~Artist~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 6)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~6~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 5)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~5~Martial~Artist~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 5)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~5~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 4)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~4~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 4)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~4~Martial~Artist~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 3)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~3~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 3)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~3~Martial~Artist~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 2)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Martial~Artist~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Being~a~Martial~Artist,~you`ll~find~your~Critical~Strike~nicely~enhanced.~Interested?~Make~sure~you`ve~reached~Level~70,~and~pay~5~Celestial~Stones~or~800~Study~Points~to~join~us!");
                        data.AddText("~After~that,~you~can~working~on~your~sub-class~skill~on~the~`Status`~sheet.~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~5~CelestialStones~to~join.", 6);
                        data.AddOption("Pay~800~Study~Points~to~join.", 4);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(32);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(721259, 5) || client.Inventory.Contain(721259, 5, 1))
                                    {
                                        if (client.Inventory.Remove(721259, 5, stream))
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.MartialArtist, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Martial~Artist~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Martial~Artists.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks~in~advance.");
                                            data.AddAvatar(32);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("You~don`t~have~5~Celestial~Stones~to~join~this~sub-class.~Sorry.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(721259, 5) || client.Inventory.Contain(721259, 5, 1))
                                {
                                    if (client.Inventory.Remove(721259, 5, stream))
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.MartialArtist, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Martial~Artist~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Martial~Artists.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks~in~advance.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~5~Celestial~Stones~to~join~this~sub-class.~Sorry.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Martial~Artist,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(32);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 5);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(32);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.MartialArtist, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Martial~Artist~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Martial~Artists.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks~in~advance.");
                                            data.AddAvatar(32);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (client.Player.SubClass.StudyPoints < 800)
                                {
                                    data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.MartialArtist, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Martial~Artist~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Martial~Artists.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks~in~advance.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Martial~Artist,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(32);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Good~day!~Please~spare~me~a~few~minutes~so~that~I~can~tell~you~about~the~sub-classes~in~Twin~City.~Each~Sub-Class~has~9~phases,~and~the~higher~the~phase~is,~the~more~bonus~effects~the~sub-class~will~offer.~But~keep~in~mind~that~these");
                        data.AddText("~sub-classes~also~have~a~level~system~and~you~need~to~level~them~up~before~you~can~get~promoted.~You~can~level~up~your~sub-class~by~gaining~study~points.");
                        data.AddOption("How~can~I~get~study~points?", 2);
                        data.AddOption("Oh.");
                        data.AddAvatar(32);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~the~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("Oh.");
                        data.AddAvatar(32);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
    }
}
