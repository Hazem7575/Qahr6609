using Conquer6609.Database;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.MsgLoader;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgServer.AttackHandler;
using Conquer6609.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Client
{
    public partial class PoolProcesses
    {
        public static unsafe void AutoAttackCallback(Client.GameClient client)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null)
                    return;

                if (client.Player.Alive == false && client.Player.CompleteLogin)
                {
                    if (DateTime.Now > client.Player.GhostStamp)
                    {
                        if (!client.Player.ContainFlag(MsgUpdate.Flags.Ghost))
                        {
                            client.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.Ghost, Role.StatusFlagsBigVector32.PermanentFlag, true);
                            if (client.Player.Body % 10 < 3)
                                client.Player.TransformationID = 99;
                            else
                                client.Player.TransformationID = 98;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Send(stream.MapStatusCreate(client.Player.Map, client.Map.ID, (uint)client.Map.TypeStatus));
                            }
                        }
                    }
                }
                Extensions.Time32 timer = Extensions.Time32.Now;

                if (client.OnAutoAttack && client.Player.Alive)
                {
                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dizzy))
                    {
                        client.OnAutoAttack = false;
                        return;
                    }

                    InteractQuery action = new InteractQuery();
                    action = InteractQuery.ShallowCopy(client.AutoAttack);
                    client.Player.RandomSpell = action.SpellID;
                    MsgAttackPacket.Process(client, action);

                    
                }

            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }
    }
}