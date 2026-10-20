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
        public static void StaminaCallback(Client.GameClient client)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null)
                    return;
                Extensions.Time32 Now = Extensions.Time32.Now;
                if (client.Player.OnAutoHunt)
                    if (client.Player.VipLevel < 5)
                        return;

                if (!client.Player.Alive)
                    return;
                if (Now >= client.Player.HuntingBounsStamp)
                {
                    if (client.Player.HuntingBounsLayer > 0) { client.Player.HuntingBounsStamp = Now.AddMinutes(5); client.Player.HuntingBounsLayer -= 1; }
                }
                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Fly))
                    return;
                byte MaxStamina = (byte)(client.Player.HeavenBlessing > 0 ? 150 : 100);
                if (client.Equipment.UseMonkEpicWeapon)
                {
                    MsgSpell user_spell = null;
                    if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.GraceofHeaven, out user_spell))
                    {
                        Database.MagicType.Magic DBSpell = Database.Server.Magic[user_spell.ID][user_spell.Level];
                        MaxStamina += (byte)DBSpell.Damage;
                    }
                }
                if (client.Player.Stamina < MaxStamina)
                {
                    ushort addstamin = 0;
                    if (client.Player.Action == Role.Flags.ConquerAction.Sit)
                        addstamin += 15;//8
                    else
                        addstamin += 8;//2

                    if (client.Player.ContainFlag(MsgUpdate.Flags.WindWalkerFan))
                    {
                        if (Now > client.Player.FanRecoverStamin.AddSeconds(5))
                        {
                            addstamin += (ushort)(addstamin * 50 / 100);
                            client.Player.FanRecoverStamin = Extensions.Time32.Now;
                        }

                    }


                    client.Player.Stamina = (ushort)Math.Min((int)(client.Player.Stamina + addstamin), MaxStamina);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                    }
                }

                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Ride))
                {
                    if (client.Player.CheckInvokeFlag(Game.MsgServer.MsgUpdate.Flags.Ride, Now))
                    {
                        if (client.Vigor < client.Status.MaxVigor)
                        {
                            client.Vigor = (ushort)Math.Min(client.Vigor + 10, client.Status.MaxVigor);

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Send(stream.ServerInfoCreate(MsgServerInfo.Action.Vigor, client.Vigor));
                            }
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