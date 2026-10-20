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

        public unsafe static void BuffersCallback(Client.GameClient client)
        {
            try
            {
                if (client == null || !client.FullLoading)
                    return;
                HazemShield.OnThread(client);
                DateTime Now = DateTime.Now;
                Extensions.Time32 Timer = Extensions.Time32.Now;
                // nightly backup (23:55) and daily restart (23:59:59): see PoolProcesses.DailyTasksTick (KernelThread)
                #region BanChat
                ChatBanned.Cheek(client);
                #endregion
                #region Map 6011 Teleport miss Conquer
                if (Now.Hour == 17 && Now.Minute >= 16 && Now.Minute < 45)
                {
                    if (client.Player.Map == 8841 && Server.ServerMaps[8841].Values.Where(i => i.Player.Alive).Count() == 1)
                    {
                        client.Teleport(300, 270, 1002);

                        if (client.Player.Alive)
                        {
                            client.Player.ConquerPoints += ServerRate.MrConquer;
                            foreach (var player in Server.GamePoll.Values)
                            {
                                player.Player.RemoveFlag(MsgUpdate.Flags.MSConquerHostess);
                            }
                         client.Player.AddFlag(Conquer6609.Game.MsgServer.MsgUpdate.Flags.MSConquerHostess, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);
                            Program.SendGlobalPackets.Enqueue(new MsgMessage("[Conqueror] Event has been finished. Our hero is: " + client.Player.Name, MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(new ServerSockets.RecycledPacket().GetStream()));

                        }
                    }
                }
                #endregion
                #region Map 6012 Teleport MRConquer
                if (Now.Hour == 17 && Now.Minute >= 16 && Now.Minute < 48)
                {
                    if (client.Player.Map == 9800 && Server.ServerMaps[9800].Values.Where(i => i.Player.Alive).Count() == 1)
                    {
                        client.Teleport(300, 270, 1002);

                        if (client.Player.Alive)
                        {
                            client.Player.ConquerPoints += ServerRate.MrConquer;
                            foreach (var player in Server.GamePoll.Values)
                            {
                                player.Player.RemoveFlag(MsgUpdate.Flags.MRConquerHost);
                            }
                            client.Player.AddFlag(Conquer6609.Game.MsgServer.MsgUpdate.Flags.MSConquerHostess, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);
                            Program.SendGlobalPackets.Enqueue(new MsgMessage("[Conqueror] Event has been finished. Our hero is: " + client.Player.Name, MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(new ServerSockets.RecycledPacket().GetStream()));

                        }
                    }
                }
                #endregion
                if (Timer > client.Player.LoginTimer.AddMinutes(30))// "0.5-Hour Online": 1 progress per 30 minutes online
                {
                    client.Player.LoginTimer = Extensions.Time32.Now;
                    client.Activeness.IncreaseTask(3);
                    client.Activeness.IncreaseTask(15);
                    client.Activeness.IncreaseTask(27);

                }
                if (client.Player.BlackSpot)
                {
                    if (Timer > client.Player.Stamp_BlackSpot)
                    {
                        client.Player.BlackSpot = false;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            client.Player.View.SendView(stream.BlackspotCreate(false, client.Player.UID), true);
                        }
                    }
                }
                foreach (var flag in client.Player.BitVector.GetFlags())
                {
                    if (flag.Expire(Timer))
                    {
                        if (flag.Key >= (int)Game.MsgServer.MsgUpdate.Flags.TyrantAura && flag.Key <= (int)Game.MsgServer.MsgUpdate.Flags.EartAura)
                        {
                            client.Player.AddAura(client.Player.UseAura, null, 0);
                        }
                        else
                        {

                            if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Superman || flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Cyclone
                                || flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.SuperCyclone || flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Oblivion)
                            {
                                Role.KOBoard.KOBoardRanking.AddItem(new Role.KOBoard.Entry() { UID = client.Player.UID, Name = client.Player.Name, Points = client.Player.KillCounter }, true);
                            }
                            client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                        }
                    }
                    if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.ScarofEarth)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            if (client.Player.ScarofEarthl != null && client.Player.AttackerScarofEarthl != null)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    var DBSpell = client.Player.ScarofEarthl;
                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(
                                        client.Player.UID
                                          , 0, client.Player.X, client.Player.Y, DBSpell.ID
                                          , DBSpell.Level, 0, 1);

                                    MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj()
                                    {
                                        UID = client.Player.UID,
                                        Damage = (uint)DBSpell.Damage2,
                                        Hit = 1
                                    };

                                    Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client.Player.AttackerScarofEarthl, client.Player);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                    MsgSpell.Send(client);
                                }
                            }
                        }
                    }

                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.DragonFlow)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            byte MaxStamina = (byte)(client.Player.HeavenBlessing > 0 ? 150 : 100);

                            if (client.Player.Stamina < MaxStamina)
                            {
                                client.Player.Stamina += 20;
                                client.Player.Stamina = (ushort)Math.Min((int)client.Player.Stamina, MaxStamina); using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                                }
                            }
                        }
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.HealingSnow)
                    {
                        if (flag.CheckInvoke(Timer) && client.Player.Alive)
                        {
                            if (client.Player.HitPoints < client.Status.MaxHitpoints || client.Player.Mana < client.Status.MaxMana)
                            {
                                MsgSpell spell;
                                if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.HealingSnow, out spell))
                                {
                                    var arrayspells = Database.Server.Magic[(ushort)Role.Flags.SpellID.HealingSnow];
                                    var DbSpell = arrayspells[(ushort)Math.Min((int)spell.Level, arrayspells.Count - 1)];

                                    client.Player.HitPoints = (int)Math.Min(client.Status.MaxHitpoints, (int)(client.Player.HitPoints + DbSpell.Damage2));
                                    client.Player.Mana = (ushort)Math.Min(client.Status.MaxMana, (int)(client.Player.Mana + DbSpell.Damage3));
                                    client.Player.SendUpdateHP();
                                    client.Player.XPCount += 1;
                                }
                            }
                        }
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Poisoned)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            int damage = (int)Game.MsgServer.AttackHandler.Calculate.Base.CalculatePoisonDamage((uint)client.Player.HitPoints, client.Player.PoisonLevel);
                            if (damage > 1)
                            {
                                damage -= (int)(((damage * Math.Min(100, client.PerfectionStatus.ToxinEraser)) / 100) / 2);//without /2
                            }
                            if (client.Player.HitPoints == 1)
                            {
                                damage = 0;
                                goto jump;
                            }
                            damage -= (int)(((damage * Math.Min(client.Status.Detoxication, 90)) / 100) / 2);//without /2
                            client.Player.HitPoints = Math.Max(1, (int)(client.Player.HitPoints - damage));

                        jump:

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                InteractQuery action = new InteractQuery()
                                {
                                    Damage = damage,
                                    AtkType = MsgAttackPacket.AttackID.Physical,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    OpponentUID = client.Player.UID
                                };
                                client.Player.View.SendView(stream.InteractionCreate(&action), true);
                            }

                        }
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.ShurikenVortex)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                InteractQuery action = new InteractQuery()
                                {
                                    UID = client.Player.UID,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    SpellID = (ushort)Role.Flags.SpellID.ShurikenEffect,
                                    AtkType = MsgAttackPacket.AttackID.Magic
                                };

                                MsgAttackPacket.ProcescMagic(client, stream.InteractionCreate(&action), action);
                            }
                        }
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.RedName || flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.BlackName)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            if (client.Player.PKPoints > 0)
                                client.Player.PKPoints -= 1;

                            client.Player.PkPointsStamp = Extensions.Time32.Now;
                        }
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Cursed)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            if (client.Player.CursedTimer > 0)
                                client.Player.CursedTimer -= 1;
                        }
                    }



                }
                if (client.Player.OnTransform)
                {
                    if (client.Player.TransformInfo != null)
                    {
                        if (client.Player.TransformInfo.CheckUp(Timer))
                            client.Player.TransformInfo = null;
                    }
                }
                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Praying))
                {
                    if (client.Player.BlessTime < 7200000 - 30000)
                    {
                        if (Timer > client.Player.CastPrayStamp.AddSeconds(30))
                        {
                            bool have = false;
                            foreach (var ownerpraying in client.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, ownerpraying.X, ownerpraying.Y) <= 2)
                                {
                                    var target = ownerpraying as Role.Player;
                                    if (target.ContainFlag(MsgUpdate.Flags.CastPray))
                                    {
                                        have = true;
                                        break;
                                    }
                                }
                            }
                            if (!have)
                                client.Player.RemoveFlag(MsgUpdate.Flags.Praying);
                            client.Player.CastPrayStamp = new Extensions.Time32(Timer.AllMilliseconds);
                            client.Player.BlessTime += 30000;
                        }
                    }
                    else
                        client.Player.BlessTime = 3100000;
                }
                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.CastPray))
                {
                    if (client.Player.BlessTime < 7200000 - 60000)
                    {
                        if (Timer > client.Player.CastPrayStamp.AddSeconds(30))
                        {
                            client.Player.CastPrayStamp = new Extensions.Time32(Timer.AllMilliseconds);
                            client.Player.BlessTime += 60000;
                        }
                    }
                    else
                        client.Player.BlessTime = 7200000;
                    if (Timer > client.Player.CastPrayActionsStamp.AddSeconds(5))
                    {
                        client.Player.CastPrayActionsStamp = new Extensions.Time32(Timer.AllMilliseconds);
                        foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, obj.X, obj.Y) <= 1)
                            {
                                var Target = obj as Role.Player;
                                if (Target.Reborn < 2)
                                {
                                    if (!Target.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Praying))
                                    {
                                        Target.AddFlag(Game.MsgServer.MsgUpdate.Flags.Praying, Role.StatusFlagsBigVector32.PermanentFlag, true);

                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            ActionQuery action = new ActionQuery()
                                            {
                                                ObjId = client.Player.UID,
                                                dwParam = (uint)client.Player.Action,
                                                Timestamp = (int)obj.UID
                                            };
                                            client.Player.View.SendView(stream.ActionCreate(&action), true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (client.Player.BlessTime > 0)
                {
                    if (!client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.CastPray) && !client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Praying))
                    {

                        if (Timer > client.Player.CastPrayStamp.AddSeconds(2))
                        {
                            if (client.Player.BlessTime > 2000)
                                client.Player.BlessTime -= 2000;
                            else
                                client.Player.BlessTime = 0;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, client.Player.BlessTime, Game.MsgServer.MsgUpdate.DataType.LuckyTimeTimer);
                            }
                            client.Player.CastPrayStamp = new Extensions.Time32(Timer.AllMilliseconds);
                        }
                    }
                }
                if (client.Team != null)
                {
                    // Only the leader's invitations can be accepted (MsgTeam.AcceptInvitation), so only he auto-invites.
                    if (client.Team.AutoInvite == true && client.Player.Map != 1036 && client.Team.CkeckToAdd()
                        && client.Team.TeamLider(client))
                    {
                        if (Timer > client.Team.InviteTimer.AddSeconds(10))
                        {
                            client.Team.InviteTimer = Timer;
                            foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (!client.Team.SendInvitation.Contains(obj.UID))
                                {
                                    client.Team.SendInvitation.Add(obj.UID);

                                    if ((obj as Role.Player).Owner.Team == null)
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();

                                            // Register the invitation so the invitee's AcceptInvitation is honoured.
                                            MsgTeam.AddPending(MsgTeam.TeamTypes.InviteRequest, client.Player.UID, obj.UID);
                                            obj.Send(stream.PopupInfoCreate(client.Player.UID, obj.UID, client.Player.Level, client.Player.BattlePower));

                                            stream.TeamCreate(MsgTeam.TeamTypes.InviteRequest, client.Player.UID);
                                            obj.Send(stream);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (client.Team.TeamLider(client))
                    {
                        if (Timer > client.Team.UpdateLeaderLocationStamp.AddSeconds(4))
                        {
                            client.Team.UpdateLeaderLocationStamp = Timer;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                ActionQuery action = new ActionQuery()
                                {
                                    ObjId = client.Player.UID,
                                    dwParam = 1015,
                                    Type = ActionType.LocationTeamLieder,
                                    wParam1 = client.Team.Leader.Player.X,
                                    wParam2 = client.Team.Leader.Player.Y
                                };
                                client.Team.SendTeam(stream.ActionCreate(&action), client.Player.UID, client.Player.Map);
                            }
                        }
                    }
                   
                }
                if (UnlimitedArenaRooms.IsArenaRoom(client.Player.DynamicID))
                {
                    client.SendSysMesage("Accuracy Rates.", MsgMessage.ChatMode.FirstRightCorner);
                    foreach (var player in client.Map.Values.Where(e => e.Player.DynamicID == client.Player.DynamicID))
                        client.SendSysMesage(player.Player.Name + " " + Math.Round((double)(player.Player.Hits * 100.0 / Math.Max(1, player.Player.TotalHits)), 2) +
                            "%, Hits: " + player.Player.Hits + ", Miss: " +
                            (player.Player.TotalHits - player.Player.Hits),
                            MsgMessage.ChatMode.ContinueRightCorner);

                }

            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }
    }
}