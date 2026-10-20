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
        public static void StampXPCountCallback(Client.GameClient client)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || client.Player.OnAutoHunt)
                    return;

                Extensions.Time32 Timer = Extensions.Time32.Now;
                if (client.Player.OnAutoHunt)
                    return;

                if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                {
                    if (client.Equipment.LeftWeapon != 0)
                    {
                        if (Database.ItemType.IsHossu(client.Equipment.LeftWeapon) == false)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                                    client.Equipment.LeftWeapon = 0;
                                }
                            }
                        }
                    }
                }
                else if (Database.ItemType.IsTwoHand(client.Equipment.RightWeapon))
                {
                    if (client.Equipment.LeftWeapon != 0 && Database.ItemType.IsShield(client.Equipment.LeftWeapon) == false)
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream) == false)
                                    client.Equipment.Remove(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                                client.Equipment.LeftWeapon = 0;
                            }
                        }
                    }
                }

                if (client.Player.PKPoints > 0)
                {
                    if (Timer > client.Player.PkPointsStamp.AddMinutes(6))
                    {
                        client.Player.PKPoints -= 1;
                        client.Player.PkPointsStamp = Extensions.Time32.Now;
                    }
                }

                if (Timer > client.Player.XPListStamp.AddSeconds(4) && client.Player.Alive)
                {
                    client.Player.XPListStamp = Timer.AddSeconds(4);
                    if (!client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.XPList))
                    {
                        client.Player.XPCount++;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            client.Player.SendUpdate(stream, client.Player.XPCount, MsgUpdate.DataType.XPCircle);
                            if (client.Player.XPCount >= 100)
                            {
                                client.Player.XPCount = 0;
                                client.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.XPList, 20, true);
                                client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { "xp" });
                            }
                        }
                    }
                }
                if (client.Player.InUseIntensify)
                {
                    if (Timer > client.Player.IntensifyStamp.AddSeconds(2))
                    {
                        if (!client.Player.Intensify)
                        {
                            client.Player.Intensify = true;
                            client.Player.InUseIntensify = false;
                        }
                    }
                }


            }

            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }
    }
}