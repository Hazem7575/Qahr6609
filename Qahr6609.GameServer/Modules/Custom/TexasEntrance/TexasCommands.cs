// GM commands of the Texas Hold'em championships.
//   @texasboard on [champ] [chips] [bb] [ante] [wait] [seat]  - probe: puts YOUR client into board mode (2501 mode 0)
//                and opens the championship board with you seated (act 4, 16, 15, 2090, 3406). Nothing is played.
//   @texasboard mate <name> [seat]  - adds another online player's look (MsgPlayer byte 136 = 2) + 2090 to your board
//   @texasboard deal                - fake preflop deal on your board (2091 + your turn 2092), to see the cards/buttons
//   @texasboard act <n> [args...]   - any 3400 action to yourself (e.g. "act 20 30" = countdown overlay)
//   @texasboard off                 - 2096 exit + 2501 mode 1 (board closes, client back to normal)
//   @texastour [status] | test [champ] [minPlayers] [startSec] [levelSec] [fee] | cancel <session> | join <session> | reload
using System;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Framework.Commands;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgServer.Poker;

namespace Conquer6609.Modules.TexasEntrance
{
    internal static class TexasCommands
    {
        const uint ProbeRoom = 9999;

        static ulong Num(string[] d, int i, ulong def)
        {
            ulong v;
            return i < d.Length && ulong.TryParse(d[i], out v) ? v : def;
        }

        [GmCommand("texasboard", Role = CommandRole.GM, Help = "championship board probe: @texasboard on|mate|deal|act|off", Usage = "@texasboard on 2998 100000 1000 0 30 0")]
        static void Board(Client.GameClient client, string[] data)
        {
            string sub = data.Length > 1 ? data[1].ToLowerInvariant() : "";
            uint uid = client.Player.UID;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                switch (sub)
                {
                    case "on":
                        {
                            uint champ = (uint)Num(data, 2, 2998);
                            long chips = (long)Num(data, 3, 100000), bb = (long)Num(data, 4, 1000), ante = (long)Num(data, 5, 0);
                            uint wait = (uint)Num(data, 6, 30);
                            ushort seat = (ushort)Num(data, 7, 0);
                            client.Send(stream.MsgInterServerIdentifier(0, uid, uid));
                            client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.ServerName, false, Program.ServerConfig.ServerName ?? "");
                            client.Send(stream.TexasExActionCreate(4, ProbeRoom, champ, wait));
                            client.Send(stream.TexasExActionCreate(16, (ulong)bb, (ulong)ante, 1, 1, 1));
                            client.Send(stream.TexasExValueCreate(15, chips));
                            client.Send(stream.EnterCreate(false, PokerJoinType.Playing, seat, champ, uid, chips));
                            client.Send(stream.BoardChipsCreate(new List<KeyValuePair<uint, long>> { new KeyValuePair<uint, long>(uid, chips) }));
                            client.SendSysMesage("board probe ON: 2501 mode 0, 3400 act 4 [" + ProbeRoom + "," + champ + "," + wait + "], act 16 [" + bb + "," + ante + ",1,1,1], act 15 " + chips + ", 2090 seat " + seat + ", 3406. Use @texasboard off to leave.");
                            break;
                        }
                    case "mate":
                        {
                            if (data.Length < 3)
                            {
                                client.SendSysMesage("@texasboard mate <name> [seat]");
                                return;
                            }
                            var other = Database.Server.GamePoll.Values.FirstOrDefault(c => c.Player != null && string.Equals(c.Player.Name, data[2], StringComparison.OrdinalIgnoreCase));
                            if (other == null)
                            {
                                client.SendSysMesage(data[2] + " is not online.");
                                return;
                            }
                            ushort seat = (ushort)Num(data, 3, 1);
                            var look = other.Player.GetArray(stream, false);
                            int off = PokerTable.BoardLookOffset;
                            unsafe
                            {
                                if (look.Size > off)
                                    look.Memory[off] = 2;
                            }
                            client.Send(look);
                            client.Send(stream.EnterCreate(false, PokerJoinType.Playing, seat, 2998, other.Player.UID, 100000));
                            client.Send(stream.BoardChipsCreate(new List<KeyValuePair<uint, long>> { new KeyValuePair<uint, long>(other.Player.UID, 100000) }));
                            client.SendSysMesage("sent " + other.Player.Name + " (MsgPlayer byte " + off + " = 2, 2090 seat " + seat + ")");
                            break;
                        }
                    case "deal":
                        {
                            var cards = new[] { new PokerCard(0, 12), new PokerCard(3, 11) };
                            var entries = new List<PokerPackets.DealtEntry> { new PokerPackets.DealtEntry { UID = uid } };
                            client.Send(stream.DealtCardCreate(0, PokerPackets.DealType.PreFlop, cards, entries, uid, uid, uid));
                            client.Send(stream.ActivePlayerCreate(20, PokerAction.Fold | PokerAction.Check | PokerAction.Raise | PokerAction.AllIn, 2000, 100000, uid));
                            client.SendSysMesage("fake preflop sent (2091 + 2092); your buttons do nothing on the server");
                            break;
                        }
                    case "act":
                        {
                            uint act = (uint)Num(data, 2, 0);
                            var args = new List<ulong>();
                            for (int i = 3; i < data.Length; i++)
                                args.Add(Num(data, i, 0));
                            client.Send(stream.TexasExActionCreate(act, args.ToArray()));
                            client.SendSysMesage("3400 act " + act + " [" + string.Join(",", args) + "] sent");
                            break;
                        }
                    case "off":
                        {
                            client.Send(stream.ExitCreate(2998, uid));
                            client.Send(stream.MsgInterServerIdentifier(1, uid, uid));
                            client.SendSysMesage("board probe OFF: 2096 + 2501 mode 1");
                            break;
                        }
                    default:
                        client.SendSysMesage("@texasboard on [champ chips bb ante wait seat] | mate <name> [seat] | deal | act <n> [args] | off");
                        break;
                }
            }
        }

        [GmCommand("texastour", Role = CommandRole.GM, Help = "poker championships: @texastour status|test|cancel|join|reload", Usage = "@texastour test 2998 2 60 20 0")]
        static void Tour(Client.GameClient client, string[] data)
        {
            string sub = data.Length > 1 ? data[1].ToLowerInvariant() : "status";
            switch (sub)
            {
                case "test":
                    client.SendSysMesage(Championship.GmTest((uint)Num(data, 2, 2998), (int)Num(data, 3, 2), (int)Num(data, 4, 60), (int)Num(data, 5, 0),
                        data.Length > 6 ? long.Parse(data[6]) : -1));
                    break;
                case "cancel":
                    client.SendSysMesage(Championship.GmCancel((uint)Num(data, 2, 0)));
                    break;
                case "join":
                    client.SendSysMesage(Championship.GmSignUp(client, (uint)Num(data, 2, 0)));
                    break;
                case "reload":
                    client.SendSysMesage(Championship.Reload());
                    break;
                default:
                    client.SendSysMesage(Championship.Status());
                    break;
            }
        }
    }
}
