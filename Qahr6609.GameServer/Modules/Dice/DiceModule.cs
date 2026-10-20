// Dice / Sic Bo module: the client's betting window 262 CDlgDice ("Please bet"), packets 3285/3286. The whole game is this folder.
//
//   DiceModule.cs          this file: start / stop, tick, login, @dice
//   code\DiceGame.cs       the table: rounds, bets, dice (RandomNumberGenerator), payouts, refunds, files
//   code\DicePackets.cs    3285 CMsgBetDiceOpt + 3286 CMsgBetDiceList (protobuf) and their handlers
//   code\DiceConfig.cs     dice.ini
//   data\dice.ini          the settings (copied to ServerDataBase\Modules\Dice\ the first time; edit that copy)
//
// A round: BetSeconds of betting (the alarm clock), then the dice, the payouts and ResultSeconds of result, then the next.
// Silver is taken when a bet is accepted and paid (bet x multiplier of the cell) at the result. Players who left are paid
// at their next login (owed.txt). A reload or a stop refunds the open round; bets.txt left by a crash are refunded at start.
// The window: the minimap icon 579-5481 (client GUI.ini parks it at x=10130), @sicbo for players, @dice open for a GM.
// GM: @dice [status] | on | off | reload | open [player]   (DevCommands=1: @dice join | resync | recover | bet <cell> <amount> | roll [d1 d2 d3])
using System;
using System.IO;
using System.Linq;
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Modules.Dice
{
    public sealed class DiceModule : ModuleBase
    {
        public override string Name { get { return "Dice"; } }

        static DiceModule instance;

        public override void OnLoad()
        {
            instance = this;
            DiceGame.Start(LoadConfig(), DataPath, Log);
            var st = DiceGame.Status();
            Print(st[0]);
        }

        DiceConfig LoadConfig()
        {
            string file = Path.Combine(DataPath, "dice.ini");
            if (!File.Exists(file))
            {
                string def = Path.Combine(RuntimeModules.FolderOf(Name), "data", "dice.ini");
                if (File.Exists(def)) File.Copy(def, file);
            }
            if (!File.Exists(file))
                throw new FileNotFoundException("dice.ini missing", file);
            return DiceConfig.Load(file, w => Print("dice.ini: " + w));
        }

        void Print(string line)
        {
            Log(line);
            MyConsole.WriteLine("[Dice] " + line, ConsoleColor.Cyan);
        }

        public override void OnShutdown() { DiceGame.Stop(); }
        public override void OnTick(Extensions.Time32 clock) { DiceGame.Tick(); }
        public override void OnLogin(Client.GameClient client) { DiceGame.Login(client); }
        public override void OnLogout(Client.GameClient client) { DiceGame.Logout(client); }

        static void OpenWindow(Client.GameClient client) { Window(client, 126, 262); }

        static unsafe void Window(Client.GameClient client, uint type, uint id)
        {
            var action = new Game.MsgServer.ActionQuery
            {
                Type = (Game.MsgServer.ActionType)type,// 126 OpenDialog
                ObjId = client.Player.UID,
                dwParam = id,
                wParam1 = client.Player.X,
                wParam2 = client.Player.Y
            };
            using (var rec = new ServerSockets.RecycledPacket())
                client.Send(rec.GetStream().ActionCreate(&action));
        }

        [GmCommand("sicbo", Role = CommandRole.Player, Help = "Opens the dice (Sic Bo) table", Usage = "@sicbo")]
        static void PlayerCommand(Client.GameClient client, string[] data)
        {
            OpenWindow(client);
        }

        [GmCommand("dice", Role = CommandRole.GM, Help = "Dice table: status | on | off | reload | open [player]", Usage = "@dice status")]
        static void Command(Client.GameClient client, string[] data)
        {
            var m = instance;
            if (m == null) return;
            string sub = data.Length > 1 ? data[1].ToLowerInvariant() : "status";
            switch (sub)
            {
                case "on":
                case "off":
                    client.SendSysMesage(DiceGame.SetEnabled(sub == "on"));
                    m.Log("@dice " + sub + " by " + client.Player.Name);
                    break;
                case "reload":
                    try { DiceGame.ApplyConfig(m.LoadConfig()); client.SendSysMesage("Dice: dice.ini reloaded (BetSeconds/ResultSeconds apply from the next round)."); }
                    catch (Exception e) { client.SendSysMesage("Dice: " + e.Message); }
                    break;
                case "open":
                    {
                        var target = client;
                        if (data.Length > 2)
                        {
                            target = Database.Server.GamePoll.Values.FirstOrDefault(c => c != null && c.Player != null && string.Equals(c.Player.Name, data[2], StringComparison.OrdinalIgnoreCase));
                            if (target == null) { client.SendSysMesage(data[2] + " is not online."); return; }
                        }
                        OpenWindow(target);
                        break;
                    }
                case "bet":// test: the same path as a click in the window
                    {
                        int cell; long amount;
                        if (!DiceGame.DevCommands) { client.SendSysMesage("Dice: DevCommands=0 in dice.ini."); return; }
                        if (data.Length < 4 || !int.TryParse(data[2], out cell) || !long.TryParse(data[3], out amount))
                        {
                            client.SendSysMesage("@dice bet <cell 0 Small,1 Big,2-7 triple 1-6,8-21 sum 4-17> <amount>");
                            return;
                        }
                        string why = DiceGame.PlaceBet(client, cell, 0, amount);
                        if (why == null) client.SendSysMesage("Dice: " + amount + " on " + DiceGame.CellName(cell) + " accepted.");
                        break;
                    }
                case "join":// test: what the client sends when the window is shown
                    if (!DiceGame.DevCommands) { client.SendSysMesage("Dice: DevCommands=0 in dice.ini."); return; }
                    DiceGame.HandleOpt(client, new DiceOpt { Type = 0 });
                    break;
                case "resync":// test: the server asks the client to refresh (same path as the Refresh button)
                    if (!DiceGame.DevCommands) { client.SendSysMesage("Dice: DevCommands=0 in dice.ini."); return; }
                    DiceGame.Resync(client);
                    break;
                case "recover":// test: a bets.txt left by a crash (ServerDataBase\Modules\Dice\crash_test.txt)
                    if (!DiceGame.DevCommands) { client.SendSysMesage("Dice: DevCommands=0 in dice.ini."); return; }
                    client.SendSysMesage("Dice: " + DiceGame.RecoverTest(Path.Combine(m.DataPath, "crash_test.txt")));
                    break;
                case "roll":
                    {
                        if (!DiceGame.DevCommands) { client.SendSysMesage("Dice: DevCommands=0 in dice.ini."); return; }
                        int[] dice = null;
                        if (data.Length >= 5)
                        {
                            dice = new int[3];
                            for (int i = 0; i < 3; i++)
                                if (!int.TryParse(data[2 + i], out dice[i]) || dice[i] < 1 || dice[i] > 6) { client.SendSysMesage("@dice roll [d1 d2 d3] (1-6)"); return; }
                        }
                        client.SendSysMesage("Dice: " + DiceGame.ForceRoll(dice));
                        m.Log("@dice roll " + (dice == null ? "random" : string.Join("-", dice)) + " by " + client.Player.Name);
                        break;
                    }
                default:
                    foreach (string l in DiceGame.Status()) client.SendSysMesage(l);
                    break;
            }
        }
    }
}
