// Player commands: teleport, level, stats, spells, flags, appearance.
// Migrated 1:1 from the old `switch (data[0])` in Game/MsgServer/MsgMessage.cs (ChatCommands).
// Every command keeps its old name (case-sensitive), its `data[]` indices and its body; `break` that left the
// switch became `return`. Role = GM is the old gate (client.GameMaster || client.ProjectManager).
// Auditing (gm-command / gm-denied) is done by CommandRegistry; argument errors give the old messages
// through CommandSupport.ArgumentError.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgTournaments;
using Conquer6609.Game.Conquer6609_Work;
using System.IO;
using Conquer6609.Role;
using Conquer6609.Framework.Commands;

namespace Conquer6609.Commands
{
    // same name lookup as inside namespace Conquer6609.Game.MsgServer / class MsgMessage
    using Conquer6609.Game;
    using Conquer6609.Game.MsgServer;
    using static Conquer6609.Game.MsgServer.MsgMessage;
    using MsgServer = Conquer6609.Game.MsgServer;
    using MsgTournaments = Conquer6609.Game.MsgTournaments;

    internal static unsafe class PlayerCommands
    {
        [GmCommand("couplespk", Role = CommandRole.GM)]
        static void Cmd_couplespk(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    client.Player.SpecialTitles.Clear();
                    var stream = rec.GetStream();
                    foreach (var itemm in Database.TitleStorage.Titles.Values)
                        client.Player.AddSpecialTitle((Game.MsgServer.MsgTitleStorage.TitleType)itemm.ID, stream);
                    //var Wing = Database.TitleStorage.Titles.Values.Where(x => x.ID >= 4001 && x.ID < 6017).ToArray();
                    //var stream = rec.GetStream();
                    //foreach (var item in Wing)
                    //    client.Player.AddSpecialTitle((Game.MsgServer.MsgTitleStorage.TitleType)item.ID, stream);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("Auto", Role = CommandRole.GM)]
        static void Cmd_Auto(Client.GameClient client, string[] data)
        {
            try
            {
                if (client.Player.ContainFlag(MsgUpdate.Flags.SoulShackle))
                    return;
                if (client.AutoHunting.Enable)
                {
                    Catching.End(client);
                }
                else
                    Catching.Start(client);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("GM", Role = CommandRole.GM)]
        static void Cmd_GM(Client.GameClient client, string[] data)
        {
            try
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                    client.Player.TransformInfo.CreateTransform(stream, 817, 223, (int)ushort.MaxValue - 1, 8213);
                }


                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("reborn", Role = CommandRole.GM)]
        static void Cmd_reborn(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.Reborn = byte.Parse(data[1]);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.SendUpdate(stream, client.Player.Reborn, MsgUpdate.DataType.Reborn);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("class", Role = CommandRole.GM)]
        static void Cmd_class(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.Class = byte.Parse(data[1]);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("spell", Role = CommandRole.GM, Usage = "@spell id level [soulLevel] [experience]")]
        static void Cmd_spell(Client.GameClient client, string[] data)
        {
            try
            {
                if (data.Length < 3)
                {
                    client.SendSysMesage("@spell id level [soulLevel] [experience]");
                    return;
                }
                ushort ID = 0;
                if (!ushort.TryParse(data[1], out ID))
                {
                    client.SendSysMesage("Invlid spell ID !");
                    return;
                }
                byte level = 0;
                if (!byte.TryParse(data[2], out level))
                {
                    client.SendSysMesage("Invlid spell Level ! ");
                    return;
                }
                byte levelHu = 0;
                if (data.Length > 3)
                {
                    if (!byte.TryParse(data[3], out levelHu))
                    {
                        client.SendSysMesage("Invlid spell Level Souls ! ");
                        return;
                    }
                }
                int Experience = 0;
                if (data.Length > 4)
                {
                    if (!int.TryParse(data[4], out Experience) || Experience < 0)
                    {
                        client.SendSysMesage("Invlid spell Experience ! ");
                        return;
                    }
                }

                using (var rec = new ServerSockets.RecycledPacket())
                    client.MySpells.Add(rec.GetStream(), ID, level, levelHu, 0, Experience);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("prof", Role = CommandRole.GM, Usage = "@prof id level experience")]
        static void Cmd_prof(Client.GameClient client, string[] data)
        {
            try
            {
                if (data.Length < 4)
                {
                    client.SendSysMesage("@prof id level experience");
                    return;
                }
                ushort ID = 0;
                if (!ushort.TryParse(data[1], out ID))
                {
                    client.SendSysMesage("Invlid prof ID !");
                    return;
                }
                byte level = 0;
                if (!byte.TryParse(data[2], out level))
                {
                    client.SendSysMesage("Invlid prof Level ! ");
                    return;
                }
                uint Experience = 0;
                if (!uint.TryParse(data[3], out Experience))
                {
                    client.SendSysMesage("Invlid prof Experience ! ");
                    return;
                }
                using (var rec = new ServerSockets.RecycledPacket())
                    client.MyProfs.Add(rec.GetStream(), ID, level, 0);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("level", Role = CommandRole.GM)]
        static void Cmd_level(Client.GameClient client, string[] data)
        {
            try
            {
                byte amount = 0;
                if (byte.TryParse(data[1], out amount))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.UpdateLevel(stream, amount, true);
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("Points", Role = CommandRole.GM)]
        static void Cmd_Points(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.Vitality += 500;
                client.Player.Strength += 500;
                client.Player.Spirit += 500;
                client.Player.Agility += 500;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                    client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                    client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                    client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);

                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("trans", Role = CommandRole.GM)]
        static void Cmd_trans(Client.GameClient client, string[] data)
        {
            try
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                    client.Player.TransformInfo.CreateTransform(stream, 817, ushort.Parse(data[1]), (int)ushort.MaxValue - 1, 8213);
                }


                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("scroll", Role = CommandRole.GM)]
        static void Cmd_scroll(Client.GameClient client, string[] data)
        {
            try
            {
                switch (data[1].ToLower())
                {
                    case "tc": client.Teleport(301, 279, 1002); break;
                    case "pc": client.Teleport(195, 260, 1011); break;
                    case "ac":
                    case "am": client.Teleport(566, 563, 1020); break;
                    case "dc": client.Teleport(500, 645, 1000); break;
                    case "bi": client.Teleport(723, 573, 1015); break;
                    case "pka": client.Teleport(050, 050, 1005); break;
                    case "ma": client.Teleport(211, 196, 1036); break;
                    case "ja": client.Teleport(100, 100, 6000); break;
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        // @hide: the GM disappears for players (they no longer get his spawn, moves, attacks, effects), staff still see
        // him, he sees everyone as usual and players can not target him. @hide again (or @visible) shows him again.
        [GmCommand("hide", "invisible", Role = CommandRole.GM, Help = "hide yourself from players (staff still see you); again = show", Usage = "@hide")]
        static void Cmd_invisible(Client.GameClient client, string[] data)
        {
            try
            {
                SetHidden(client, !client.Player.Invisible || (data.Length > 0 && data[0] == "invisible"));
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("visible", "unhide", Role = CommandRole.GM)]
        static void Cmd_visible(Client.GameClient client, string[] data)
        {
            try
            {
                SetHidden(client, false);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        static unsafe void SetHidden(Client.GameClient client, bool hide)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (hide)
                {
                    // players around forget him now (the hidden flag keeps him out of their screens from here on)
                    ActionQuery action = new ActionQuery() { ObjId = client.Player.UID, Type = ActionType.RemoveEntity };
                    var packet = stream.ActionCreate(&action);
                    foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                    {
                        var other = (obj as Role.Player).Owner;
                        if (!Role.RoleView.SeesHidden(other)) other.Send(packet);
                    }
                    client.Player.Invisible = true;
                    // a sign for the GM himself (and staff): the see-through "Invisibility" look; players get nothing while hidden
                    client.Player.AddFlag(MsgUpdate.Flags.Invisibility, Role.StatusFlagsBigVector32.PermanentFlag, true);
                    client.SendSysMesage("You are hidden: players can not see you, staff can. @hide again to show yourself.", MsgMessage.ChatMode.Center);
                }
                else
                {
                    client.Player.RemoveFlag(MsgUpdate.Flags.Invisibility);
                    client.Player.Invisible = false;
                    foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                        obj.Send(client.Player.GetArray(stream, false));
                    client.SendSysMesage("You are visible again.", MsgMessage.ChatMode.Center);
                }
            }
        }

        [GmCommand("rev", Role = CommandRole.GM)]
        static void Cmd_rev(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    client.Player.Revive(rec.GetStream());

                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("tele", Role = CommandRole.GM, Usage = "@tele map x y")]
        static void Cmd_tele(Client.GameClient client, string[] data)
        {
            try
            {

                if (data.Length < 4)
                {
                    client.SendSysMesage("@tele map x y");
                    return;
                }
                client.TerainMask = 0;
                uint mapid = 0;
                if (!uint.TryParse(data[1], out mapid))
                {
                    client.SendSysMesage("Invlid Map ID !");
                    return;
                }
                ushort X = 0;
                if (!ushort.TryParse(data[2], out X))
                {
                    client.SendSysMesage("Invlid X !");
                    return;
                }
                ushort Y = 0;
                if (!ushort.TryParse(data[3], out Y))
                {
                    client.SendSysMesage("Invlid Y !");
                    return;
                }
                uint DinamicID = 0;
                if (data.Length > 4 && !uint.TryParse(data[4], out DinamicID))
                {
                    client.SendSysMesage("Invlid DinamicID !");
                    return;
                }
                client.Teleport(X, Y, (ushort)mapid, 0);

                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("top", Role = CommandRole.GM)]
        static void Cmd_top(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.AddFlag((MsgServer.MsgUpdate.Flags)int.Parse(data[1]), Role.StatusFlagsBigVector32.PermanentFlag, false);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("hit", Role = CommandRole.GM)]
        static void Cmd_hit(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.HitPoints = ushort.Parse(data[1]);
                client.Player.SendUpdateHP();
                return;

            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("reward", Role = CommandRole.GM)]
        static void Cmd_reward(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    client.Player.DailySignUpDays |= (1ul << byte.Parse(data[1]));
                


                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("name", Role = CommandRole.GM)]
        static void Cmd_name(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    ActionQuery action = new ActionQuery()
                    {
                        Type = ActionType.OpenDialog,
                        ObjId = client.Player.UID,
                        dwParam = MsgServer.DialogCommands.ChangeName,
                        wParam1 = client.Player.X,
                        wParam2 = client.Player.Y,

                    };
                    client.Send(stream.ActionCreate(&action));
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("clearspells", Role = CommandRole.GM)]
        static void Cmd_clearspells(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var spell in client.MySpells.ClientSpells.Values)
                        client.MySpells.Remove(spell.ID, stream);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("revive", Role = CommandRole.GM)]
        static void Cmd_revive(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    client.Player.Revive(rec.GetStream());

                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("bringall", Role = CommandRole.GM)]
        static void Cmd_bringall(Client.GameClient client, string[] data)
        {
            try
            {
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Fake || user.Player.UID == client.Player.UID)
                        continue;
                    user.Teleport(client.Player.X, client.Player.Y, client.Player.Map);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("bring", Role = CommandRole.GM)]
        static void Cmd_bring(Client.GameClient client, string[] data)
        {
            try
            {
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Player.Name.ToLower() == data[1].ToLower())
                    {
                        user.Teleport(client.Player.X, client.Player.Y, client.Player.Map);
                        break;
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("onlineminutes", Role = CommandRole.GM)]
        static void Cmd_onlineminutes(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.OnlineMinutes = uint.Parse(data[1]);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        //attackmax
        [GmCommand("hero", Role = CommandRole.GM, Usage = "@hero attack(>=1)")]
        static void Cmd_hero(Client.GameClient client, string[] data)
        {
            try
            {
                uint heroAttack;
                if (data.Length < 2 || !uint.TryParse(data[1], out heroAttack) || heroAttack < 1)
                {
                    client.SendSysMesage("@hero attack(>=1)");
                    return;
                }
                client.Status.Defence = uint.MaxValue;
                client.Status.MaxAttack = heroAttack;
                client.Status.MinAttack = heroAttack - 1;
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("haire", Role = CommandRole.GM)]
        static void Cmd_haire(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.Hair = ushort.Parse(data[1]); using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("pkp", Role = CommandRole.GM)]
        static void Cmd_pkp(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.PKPoints = ushort.Parse(data[1]);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("studypoints", Role = CommandRole.GM)]
        static void Cmd_studypoints(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    client.Player.SubClass.AddStudyPoints(client, ushort.Parse(data[1]), rec.GetStream());
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("xp", Role = CommandRole.GM)]
        static void Cmd_xp(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("addflag", Role = CommandRole.GM)]
        static void Cmd_addflag(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.AddFlag((MsgUpdate.Flags)int.Parse(data[1]), 10, true, 0, 50, 39);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("remflag", Role = CommandRole.GM)]
        static void Cmd_remflag(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.RemoveFlag((MsgUpdate.Flags)int.Parse(data[1]));
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("repell", Role = CommandRole.GM)]
        static void Cmd_repell(Client.GameClient client, string[] data)
        {
            try
            {
                ushort ID = 0;
                if (!ushort.TryParse(data[1], out ID))
                {
                    client.SendSysMesage("Invlid spell ID !");
                    return;
                }
                using (var rec = new ServerSockets.RecycledPacket())
                    client.MySpells.Remove(ID, rec.GetStream());
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("addgarment", Role = CommandRole.GM)]
        static void Cmd_addgarment(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    client.Player.AddSpecialGarment(rec.GetStream(), uint.Parse(data[1]));
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("remgarment", Role = CommandRole.GM)]
        static void Cmd_remgarment(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    client.Player.RemoveSpecialGarment(rec.GetStream());
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("transfer", Role = CommandRole.GM)]
        static void Cmd_transfer(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.InitializeTransfer(ushort.Parse(data[1]));
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("hp", Role = CommandRole.GM)]
        static void Cmd_hp(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.HitPoints = int.Parse(data[1]);
                client.Player.SendUpdateHP();
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("hair", Role = CommandRole.GM)]
        static void Cmd_hair(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + ushort.Parse(data[1]));
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var packet = rec.GetStream();
                    client.Player.SendUpdate(packet, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);

                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }
    }
}
