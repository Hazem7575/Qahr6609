using System;
using System.Linq;

namespace Conquer6609.Game.MsgNpc
{
    public unsafe partial class NpcHandler
    {
        /// <summary>Chi Token item dialog: sets one chi attribute (max value) for one Chi Token. Data/logic in ChiDialog (ChiMaker.cs).</summary>
        [NpcAttribute(NpcID.ChiToken)]
        public static void ChiToken(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog a = new Dialog(client, stream);
            var PowerTypee = (Game.MsgServer.MsgChiInfo.ChiPowerType)Enum.Parse(typeof(Game.MsgServer.MsgChiInfo.ChiPowerType), "Turtle");
            var Powerr = client.Player.MyChi.Where(p => p.Type == PowerTypee).FirstOrDefault();
            if (!Powerr.UnLocked)
            {
                a.AddText("You should unlock the 4 chi stages to be able to use this item.");
                a.AddOption("i see .", 255);
                a.AddAvatar(0).FinalizeDialog();
                return;
            }
            if (ChiDialog.ShowMenu(a, Option))
                return;
            ChiDialog.Pick pick;
            if (ChiDialog.Picks.TryGetValue(Option, out pick))
                ChiDialog.SetAttribute(client, stream, a, Option, pick, ChiDialog.TokenRules);
        }
    }
}
