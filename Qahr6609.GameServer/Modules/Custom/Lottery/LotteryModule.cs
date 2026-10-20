// Lottery module: hooks of the Lottery Center (Features\LotterySystem).
//  - login: a prize drawn but not accepted before the logout (saved as Character/LotteryPrize) opens the
//    lottery result window again (1314 mode Show), so the player can still accept it or use the re-draws left.
//    Lady Luck (Market 923 and Lottery Center 924) can show it again too ("Show me the prize I drew").
//  - GM: @lottery reload  re-reads ServerDataBase\lottery.ini (prizes / per-box pools) without a restart.
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;

namespace Conquer6609.Modules.Lottery
{
    public sealed class LotteryModule : ModuleBase
    {
        public override string Name { get { return "Lottery"; } }

        public override void OnLogin(Client.GameClient client)
        {
            if (client == null || client.Player == null || client.Player.LotteryItem == null)
                return;
            var prize = client.Player.LotteryItem;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Game.MsgServer.MsgLottery.SendShow(client, stream);
            }
            client.SendSysMesage("You haven`t accepted your Lottery prize (" + prize.Name + ") yet. Accept it in the Lottery window, or ask Lady Luck to show it again.",
                Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
            Database.LotteryTable.Log(client.Player.Name + " (" + client.Player.UID + ") login: pending prize " + prize.Box + "Box rank " + prize.Rank + " " + prize.Name
                + " [" + prize.ID + "] re-draws " + client.Player.AddJade);
        }

        [GmCommand("lottery", Role = CommandRole.GM, Help = "Lottery Center: @lottery reload (re-read lottery.ini)", Usage = "@lottery reload")]
        static void LotteryCommand(Client.GameClient client, string[] args)
        {
            if (args.Length > 1 && args[1].ToLowerInvariant() == "reload")
            {
                Database.Server.Lottery.LoadLotteryItems();
                client.SendSysMesage("lottery.ini reloaded (see the server console for the per-box counts).");
                return;
            }
            client.SendSysMesage("Usage: @lottery reload");
        }
    }
}
