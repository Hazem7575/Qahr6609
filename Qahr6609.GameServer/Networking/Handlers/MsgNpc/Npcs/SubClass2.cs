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

        [NpcAttribute(NpcID.Warlock)]
        public static void Warlock(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Good~day!~Have~you~ever~wondered~what~a~Warlock~is?~Well,~please~allow~me~to~introduce~this~sub-class~to~you.~Warlocks~are~masters~of~the~the~psionic~energies~and~can~increase~their~chances~of~casting~spells~with~2x~Magical~Attack.");
                        data.AddText("~Isn`t~that~wonderful?~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~increase~your~power!");
                        data.AddOption("Yes,~I`d~like~to~join.", 3);
                        data.AddOption("I~want~to~get~promoted.", 7);
                        data.AddOption("How~can~I~get~promoted?", 1);
                        data.AddOption("Not~my~concern.");
                        data.AddAvatar(23);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) < 1)
                        {
                            data.AddText("You~are~not~a~Warlock~and~can`t~get~promoted.~Do~you~want~to~join~the~Warlock~sub-class~now?");
                            data.AddOption("Yes,~I`d~like~to~join~it.", 3);
                            data.AddOption("Oh.~I`ll~think~about~it.");
                            data.AddAvatar(23);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~90~and~become~a~level~2~Warlock.~Have~you~met~all~these~requirements?");
                                data.AddOption("Sure.", 8);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~110~and~become~a~level~3~Warlock.~Have~you~met~all~these~requirements?");
                                    data.AddOption("Sure.", 9);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~120~and~become~a~level~4~Warlock.~Have~you~met~all~these~requirements?");
                                        data.AddOption("Sure.", 10);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~90,~become~a~level~5~Warlock~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                            data.AddOption("Sure.", 11);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(23);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~110,~become~a~level~6~Warlock~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddOption("Sure.", 12);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(23);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~120,~become~a~level~7~Warlock~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Sure.", 13);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(23);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~120,~become~a~level~8~Warlock~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Sure.", 14);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(23);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~130,~become~a~level~9~Warlock~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Sure.", 15);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(23);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Oh,~it~does~not~suit~me.");
                                                            data.AddAvatar(23);
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
                            data.AddAvatar(23);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 9)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~9~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~9~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
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
                            data.AddAvatar(23);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 8)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~8~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~8~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
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
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 7)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~7~Warlock~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 7)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~7~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
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
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 6)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~6~Warlock~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 6)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~6~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
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
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 5)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~5~Warlock~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 5)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~5~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
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
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 4)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~4~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 4)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~4~Warlock~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(23);
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
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 3)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~3~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 3)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~3~Warlock~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(23);
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
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Warlock~now");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 2)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Warlock~now");
                                data.AddOption("Great.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Warlocks~are~masters~of~psionic~energies~who~have~the~chance~to~make~Skill~Critical~Strike.~If~you`ve~reached~Level~70,~just~pay~10~Bombs~or~800~Study~Points,");
                        data.AddText("~and~you`ll~be~one~of~us.~After~that,~you~can~enhance~your~sub-class~skill~on~the~`Status`~sheet.~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~10~Bombs~to~join.", 6);
                        data.AddOption("Pay~800~Study~Points~to~join.", 4);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(23);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(721261, 10))
                                    {
                                        if (client.Inventory.Remove(721261, 10, stream))
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Warlock, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Warlock~now.", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Warlock~community.~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Warlocks.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks.");
                                            data.AddAvatar(23);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("You~don`t~have~the~10~Bombs~to~join~the~Warlock~sub-class.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(721261, 10))
                                {
                                    if (client.Inventory.Remove(721261, 10, stream))
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Warlock, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Warlock~now.", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Warlock~community.~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Warlocks.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~the~10~Bombs~to~join~the~Warlock~sub-class.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Warlock,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(23);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 5);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(23);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Warlock, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Warlock~now.", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Warlock~community.~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Warlocks.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks.");
                                            data.AddAvatar(23);
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
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Warlock, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Warlock~now.", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Warlock~community.~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Warlocks.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Warlock,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(23);
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
                        data.AddAvatar(23);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~the~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("I~see.");
                        data.AddAvatar(23);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Sage)]
        public static void Sage(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello,~my~friend.~Are~you~here~to~join~the~Sage~sub-class?~Before~we~start,~please~allow~me~to~introduce~this~sub-class~to~you.~Sages~have~strong~spiritual~powers~and~have~a~gruesome~ability~to~reduce~a~target`s");
                        data.AddText("~Magical~Defense.~This~is~very~helpful~in~defeating~those~who~might~underestimate~your~power!");
                        data.AddText("~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~reduce~more~of~your~target`s~Magical~Defense.");
                        data.AddOption("Yes,~I`d~like~to~join.", 3);
                        data.AddOption("I~want~to~get~promoted.", 7);
                        data.AddOption("How~can~I~get~promoted?", 1);
                        data.AddOption("Not~my~concern.");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) < 1)
                        {
                            data.AddText("Well,~you~need~to~join~us~before~getting~promoted.~Do~you~want~to~become~a~Sage~now?");
                            data.AddOption("Yes,~I`d~like~to~join~it.", 3);
                            data.AddOption("Oh,~I`m~not~ready~yet.");
                            data.AddAvatar(87);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~90~and~become~a~level~2~Sage.~Have~you~met~all~these~requirements?");
                                data.AddOption("Positive.", 8);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~110~and~become~a~level~3~Sage.~Have~you~met~all~these~requirements?");
                                    data.AddOption("Positive.", 9);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~120~and~become~a~level~4~Sage.~Have~you~met~all~these~requirements?");
                                        data.AddOption("Positive.", 10);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~90,~become~a~level~5~Sage~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                            data.AddOption("Positive.", 11);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(87);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~110,~become~a~level~6~Sage~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddOption("Positive.", 12);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(87);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~120,~become~a~level~7~Sage~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Positive.", 13);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(87);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~120,~become~a~level~8~Sage~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Positive.", 14);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(87);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~130,~become~a~level~9~Sage~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Positive.", 15);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(87);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Oh,~it~does~not~suit~me.");
                                                            data.AddAvatar(87);
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
                            data.AddAvatar(87);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 9)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~9~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~9~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
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
                            data.AddAvatar(87);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 8)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~8~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~8~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
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
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 7)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~7~Sage~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 7)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~7~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
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
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 6)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~6~Sage~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 6)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~6~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
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
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 5)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~5~Sage~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 5)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~5~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
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
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 4)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~4~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 4)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~4~Sage~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(87);
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
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 3)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~3~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 3)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~3~Sage~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(87);
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
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 2)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Sage~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Want~to~be~a~Sage?~We`re~able~to~reduce~a~target`s~Magical~Defense.~Make~sure~you`ve~reached~Level~70,~and~pay~20~Lucky~Amulets~or~800~Study~Points~to~join~us!~");
                        data.AddText("~After~being~one~of~us,~you~can~work~on~your~sub-class~training~on~the~`Status`~sheet.~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~20~Lucky~Amulets~to~join.", 6);
                        data.AddOption("Pay~800~Study~Points~to~join.", 4);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(723087, 20))
                                    {
                                        if (client.Inventory.Remove(723087, 20, stream))
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Sage, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Sage.~Excellent!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Sage~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Sages.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks.");
                                            data.AddAvatar(87);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("Well,~I`m~sorry,~but~you~don`t~have~the~20~Lucky~Amulets~needed~to~join~us.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(723087, 20))
                                {
                                    if (client.Inventory.Remove(723087, 20, stream))
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Sage, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Sage.~Excellent!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Sage~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Sages.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Well,~I`m~sorry,~but~you~don`t~have~the~20~Lucky~Amulets~needed~to~join~us.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Sage,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(87);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 5);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Sage, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Sage.~Excellent!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Sage~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Sages.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks.");
                                            data.AddAvatar(87);
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
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Sage, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Sage.~Excellent!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Sage~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Sages.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Sage,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(87);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Good~day!~Please~spare~me~a~few~minutes~so~that~I~can~tell~you~about~the~sub-classes~in~Twin~City.~Each~Sub-Class~has~9~phases,~and~the~higher~the~phase~is,~the~more~bonus~effects~the~sub-class~will~offer.~But~keep~in~mind~that~these");
                        data.AddText("~sub-classes~also~have~a~level~system~and~you~need~to~level~them~to~a~certain~level~before~getting~promoted~to~some~phase.~You~can~level~up~your~sub-class~by~gaining~study~points.");
                        data.AddOption("How~can~I~get~study~points?", 2);
                        data.AddOption("I~see.");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~the~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("I~see.~Thanks.");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ChiMaster)]
        public static void ChiMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello~there!~Please~allow~me~to~introduce~to~you~the~sub-class~of~the~Chi~Master.~The~Chi~Master~is~a~potent~combatant~so~versed~in~counter~attacks,~that~they~have~a~chance~of~preventing~their~enemy~from~landing~Critical~Strikes!");
                        data.AddText("~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~increase~your~immunity~to~critical~strikes.");
                        data.AddOption("Yes,~I`d~like~to~join~it.", 1);
                        data.AddOption("I~want~to~get~promoted.", 5);
                        data.AddOption("How~can~I~get~promoted?", 14);
                        data.AddOption("Where~to~buy~Yin-Yang~Fruits?", 16);
                        data.AddOption("Not~my~concern.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        data.AddText("You~may~pay~10,000~Horse~Racing~points~to~the~Horse~Race~Manager~to~buy~a~Yin~Yang~Fruit.");
                        data.AddOption("Thanks.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        data.AddText("Good~day!~Please~spare~me~a~few~minutes~so~that~I~can~tell~you~about~the~sub-classes~in~Twin~City.~Each~Sub-Class~has~9~phases,~and~the~higher~the~phase~is,~the~more~bonus~effects~the~sub-class~will~offer.~But~keep~in~mind~that~these");
                        data.AddText("~sub-classes~also~have~a~level~system~and~you~need~to~level~them~up~before~you~can~get~promoted.~You~can~level~up~your~sub-class~by~gaining~study~points.");
                        data.AddOption("How~can~I~get~study~points?", 15);
                        data.AddOption("Oh.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~the~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("Okay.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }

                case 5:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) < 1)
                        {
                            data.AddText("You~are~not~a~Chi~Master~yet~and~can`t~get~promoted.~Do~you~want~to~join~the~Chi~Master~sub-class~now?");
                            data.AddOption("Yes,~I`d~like~to~join.", 1);
                            data.AddOption("Oh.~Not~now.");
                            data.AddAvatar(79);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~90~and~become~a~level~2~Chi~Master.~Have~you~met~all~these~requirements?");
                                data.AddText("~");
                                data.AddOption("Positive.", 6);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~110~and~become~a~level~3~Chi~Master.~Have~you~met~all~these~requirements?");
                                    data.AddText("~");
                                    data.AddOption("Positive.", 7);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~120~and~become~a~level~4~Chi~Master.~Have~you~met~all~these~requirements?");
                                        data.AddText("~");
                                        data.AddOption("Positive.", 8);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~90~,~become~a~level~5~Chi~Master~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                            data.AddText("~");
                                            data.AddOption("Positive.", 9);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(79);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~110~,~become~a~level~6~Chi~Master~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddText("~");
                                                data.AddOption("Positive.", 10);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(79);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~120~,~become~a~level~7~Chi~Master~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddText("~");
                                                    data.AddOption("Positive.", 11);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(79);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~120~,~become~a~level~8~Chi~Master~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddText("~");
                                                        data.AddOption("Positive.", 12);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(79);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~130~,~become~a~level~9~Chi~Master~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddText("~");
                                                            data.AddOption("Positive.", 13);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(79);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Oh,~it~does~not~suit~me.");
                                                            data.AddAvatar(79);
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
                case 13:
                    {
                        if (client.Player.Level < 130)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(79);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 9)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~9~Chi~Master~now.", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~9~Chi~Master~now.");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(79);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 8)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~8~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~8~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
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
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 7)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~7~Chi~Master~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 7)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~7~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 6)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~6~Chi~Master~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 6)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~6~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 5)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~5~Chi~Master~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 5)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~5~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 4)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~4~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 4)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~4~Chi~Master~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 3)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~3~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 3)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~3~Chi~Master~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 2)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Chi~Master~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("The~Chi~Master~focuses~on~immunity~rate~to~critical~strikes~from~enemies.~If~you`ve~reached~Level~70~and~you~want~to~join~us,~give~me~800~Study~Points~or~a~Yin-Yang~Fruit");
                        data.AddText("~that~you~can~buy~from~the~Horse~Race~Manager~at~10,000~Horse~Racing~Points.~After~that,~you~can~start~your~study~of~the~sub-class~on~the~`Status`~sheet.");
                        data.AddText("~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~a~Yin-Yang~Fruit~to~join.", 4);
                        data.AddOption("Pay~800~Study~Points~to~join.", 2);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(711188, 1))
                                    {
                                        if (!client.Inventory.Remove(711188, 1, stream)) break;
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.ChiMaster, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Chi~Master!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Chi~Master~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Chi~Masters.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddText("I`m~sorry,~you~need~to~buy~a~Yin-Yang~Fruit~from~the~Horse~Race~Manager~and~reach~level~70~to~join~us.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(711188, 1))
                                {
                                    if (!client.Inventory.Remove(711188, 1, stream)) break;
                                    client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.ChiMaster, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Chi~Master!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Welcome~to~the~Chi~Master~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Chi~Masters.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("I`m~sorry,~you~need~to~buy~a~Yin-Yang~Fruit~from~the~Horse~Race~Manager~and~reach~level~70~to~join~us.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Chi~Master,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(79);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 3);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.ChiMaster, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Chi~Master!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Chi~Master~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Chi~Masters.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks.");
                                            data.AddAvatar(79);
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
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.ChiMaster, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Chi~Master!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Chi~Master~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Chi~Masters.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Chi~Master,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(79);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LeadWrangler)]
        public static void LeadWrangler(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Hey,~there!~Let~me~tell~you~about~the~joys~of~the~Wrangler!~Wranglers~are~able~to~equip~special~armor~on~their~mounts.~As~their~Sub-Class~level~improves,");
                        data.AddText("~their~Max~HP~increases.~Isn`t~it~great?~Would~you~like~to~join~us?");
                        data.AddOption("Yes,~I`d~like~to~join.", 1);
                        data.AddOption("I`d~like~to~get~promoted.", 7);
                        data.AddOption("How~can~I~get~promoted?", 5);
                        data.AddOption("Not~my~concern.");
                        data.AddAvatar(233);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) < 1)
                        {
                            data.AddText("You`re~not~a~Wrangler~yet,~and~can`t~get~promoted.~Would~you~like~to~become~a~Wrangler?");
                            data.AddOption("Yes,~I`d~like~to~join.", 1);
                            data.AddOption("Not~now.");
                            data.AddAvatar(233);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~be~able~to~equip~better~mount~armor,~your~character~should~reach~Level~90~and~your~Sub-Class~reach~Level~2.");
                                data.AddText("~Have~you~met~all~these~requirements?");
                                data.AddOption("Yup!", 8);
                                data.AddOption("Nope.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~be~able~to~equip~better~mount~armor,~your~character~should~reach~Level~110~and~your~Sub-Class~at~Level~3.");
                                    data.AddText("~Have~you~met~all~these~requirements?");
                                    data.AddOption("Yup!", 9);//90028189
                                    data.AddOption("Nope.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~be~able~to~equip~better~mount~armor,~your~character~should~reach~Level~120~and~your~Sub-Class~at~Level~4.");
                                        data.AddText("~Have~you~met~all~these~requirements?");
                                        data.AddOption("Yup!", 10);//90028239
                                        data.AddOption("Nope.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~be~able~to~equip~better~mount~armor,~you~should~meet~the~following~requirements:~Be~Reborn~once,~Character~Level~90,~and~Sub-Class~Level~5.");
                                            data.AddText("~Have~you~met~all~these~requirements?");
                                            data.AddOption("Yup!", 11);
                                            data.AddOption("Nope.");
                                            data.AddAvatar(233);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~be~able~to~equip~better~mount~armor,~you~should~meet~the~following~requirements:~Be~Reborn~once,~Character~Level~110,~and~Sub-Class~Level~6.");
                                                data.AddText("~Have~you~met~all~these~requirements?");
                                                data.AddOption("Yup!", 12);
                                                data.AddOption("Nope.");
                                                data.AddAvatar(233);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~be~able~to~equip~better~mount~armor,~you~should~meet~the~following~requirements:~Be~Reborn~once,~Character~Level~120,~and~Sub-Class~Level~7.");
                                                    data.AddText("~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Yup!", 13);
                                                    data.AddOption("Nope.");
                                                    data.AddAvatar(233);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~be~able~to~equip~better~mount~armor,~you~should~meet~the~following~requirements:~Be~Reborn~twice,~Character~Level~120,~and~Sub-Class~Level~8.");
                                                        data.AddText("~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Yup!", 14);
                                                        data.AddOption("Nope.");
                                                        data.AddAvatar(233);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~be~able~to~equip~better~mount~armor,~you~should~meet~the~following~requirements:~Be~Reborn~twice,~Character~Level~130,~and~Sub-Class~Level~9.");
                                                            data.AddText("~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Yup!", 15);
                                                            data.AddOption("Nope.");
                                                            data.AddAvatar(233);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("You`ve~reached~the~highest~phase!~There~is~nothing~else~I~have~to~teach~you.~Go~forth~and~make~us~proud!");
                                                            data.AddOption("Bye.");
                                                            data.AddAvatar(233);
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
                            data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("I~see.");
                            data.AddAvatar(233);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 9)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");

                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P9~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P9~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
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
                            data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("I~see.");
                            data.AddAvatar(233);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 8)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P8~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~P8~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
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
                            if (client.Player.Level < 110)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 7)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("I~see.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P7~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Congratulations!~You`ve~been~promoted~to~P7~Wrangler!");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 7)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P7~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~P7~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
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
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 6)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("I~see.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P6~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Congratulations!~You`ve~been~promoted~to~a~P6~Wrangler!");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 6)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P6~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P6~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
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
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 5)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("I~see.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P5~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Congratulations!~You`ve~been~promoted~to~a~P5~Wrangler!");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 5)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P5~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P5~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
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
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 4)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P4~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P4~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 4)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P4~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("Congratulations!~You`ve~been~promoted~to~a~P4~Wrangler!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(233);
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
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 3)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P3~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P3~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 3)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P3~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("Congratulations!~You`ve~been~promoted~to~a~P3~Wrangler!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(233);
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
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 2)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P2~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P2~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 2)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P2~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("Congratulations!~You`ve~been~promoted~to~a~P2~Wrangler!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }

                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("Each~Sub-Class~has~9~Phases.~The~higher~the~phase~is,~the~more~bonus~effects~you~will~get.");
                        data.AddText("~But~keep~in~mind~that~these~Sub-Classes~also~have~a~level~system,~and~you~need~to~level~them~up~before~you~can~get~promoted.~You~can~level~up~your~Sub-Class~by~gaining~Study~Points.");
                        data.AddOption("How~can~I~get~Study~Points?", 6);
                        data.AddOption("I~see.");
                        data.AddAvatar(233);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~Sub-Class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~Study~Points,");
                        data.AddText("~open~the~Sub-Class~sheet~from~the~character~panel~and~level~up~the~Sub-Class,~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,");
                        data.AddText("~so~you~might~find~some~books~after~you~kill~them.");
                        data.AddOption("Got~it.");
                        data.AddAvatar(233);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Wranglers~are~able~to~equip~special~armor~on~their~mounts.~If~you`ve~reached~Level~40,~you~can~pay~40~Saddles~or~800~Study~Points~to~join~us.~");
                        data.AddText("~After~being~a~Wrangler,~you~can~enhance~your~sub-class~skill~on~`Status`~sheet.~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~40~Saddles~to~join.", 2);
                        data.AddOption("Pay~800~Study~Points~to~join.", 3);
                        data.AddOption("I~see.");
                        data.AddAvatar(233);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 4);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(233);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("I`m~sorry,~but~you~have~to~reach~at~least~Level~70~to~join~our~fine~group~of~adventurers.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);

                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Wrangler, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~joined~up~with~the~Wranglers!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Each~Sub-Class~has~9~phases.~If~you~want~to~get~promoted~one~day,~you`ll~come~talk~to~me.");
                                        data.AddOption("I~will.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();

                                    }
                                }
                            }
                            else
                            {
                                if (client.Player.SubClass.StudyPoints < 800)
                                {
                                    data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Wrangler, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~joined~up~with~the~Wranglers!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Each~Sub-Class~has~9~phases.~If~you~want~to~get~promoted~one~day,~you`ll~come~talk~to~me.");
                                        data.AddOption("I~will.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Wrangler,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~standing~with~us!");
                            data.AddOption("Thanks~for~reminding~me.");
                            data.AddAvatar(233);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("I`m~sorry,~but~you~have~to~reach~at~least~Level~70~to~join~our~fine~group~of~adventurers.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(723903, 40))
                                    {
                                        if (client.Inventory.Remove(723903, 40, stream))
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Wrangler, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~joined~up~with~the~Wranglers!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Each~Sub-Class~has~9~phases.~If~you~want~to~get~promoted~one~day,~you`ll~come~talk~to~me.");
                                            data.AddOption("I~will.");
                                            data.AddAvatar(233);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("Sorry,~but~I~don`t~see~40~Saddles~in~your~inventory.~You~should~bring~them~with~you,~next~time.");
                                        data.AddOption("OK.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(723903, 40))
                                {
                                    if (client.Inventory.Remove(723903, 40, stream))
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Wrangler, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~joined~up~with~the~Wranglers!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Each~Sub-Class~has~9~phases.~If~you~want~to~get~promoted~one~day,~you`ll~come~talk~to~me.");
                                        data.AddOption("I~will.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Sorry,~but~I~don`t~see~40~Saddles~in~your~inventory.~You~should~bring~them~with~you,~next~time.");
                                    data.AddOption("OK.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Wrangler,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~standing~with~us!");
                            data.AddOption("Thanks~for~reminding~me.");
                            data.AddAvatar(233);
                            data.FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SubClassManager)]
        public static void SubClassManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("In addition to the existing 7 classes, there are 7 sub-classes for you to choose, which will grant you many bonus effects.");
                        data.AddText(" The 7 sub-classes are Apothecary, Chi Master, Martial Artist, Warlock, Sage, Performer, Lead Wrangler. Do you want to choose a sub-class to");
                        data.AddText("~enhance the strength of your character?");
                        data.AddOption("Sub-Class~Hall", 1);
                        data.AddOption("See~you~later.");
                        data.AddAvatar(76);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(51, 71, 6072);
                        break;
                    }
            }
        }
    }
}
