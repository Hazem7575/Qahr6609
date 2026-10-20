using Conquer6609.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Npc
    {
        public static uint Execute(ServerSockets.Packet stream, MsgSpellAnimation.SpellObj obj, Client.GameClient client, Role.SobNpc attacked)
        {
            var currentTournament = MsgTournaments.MsgSchedules.CurrentTournament;
            if (currentTournament.Type == MsgTournaments.TournamentType.FootBall)
            {
                if (currentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    var tournament = (MsgTournaments.MsgFootball)currentTournament;
                    tournament.RemoveNpc();
                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.lianhuaran04, Role.StatusFlagsBigVector32.PermanentFlag, true);
                    return 0;
                }
            }
            #region CalmWind
            if (client.PerfectionStatus.CalmWind > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.CalmWind))
                {
                    foreach (var spell in client.MySpells.ClientSpells.Values)
                        spell.ColdTime = Extensions.Time32.Now;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {

                        client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.CalmWind,
                            Id = client.Player.UID,
                            dwParam = client.Player.UID
                        }), true);
                    }
                }
            }
            #endregion
           
            if (client.PerfectionStatus.KillingFlash > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.KillingFlash))
                {
                    client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                    {
                        Effect = MsgRefineEffect.RefineEffects.KillingFlash,
                        Id = client.Player.UID,
                        dwParam = client.Player.UID
                    }), true);
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.XPList) && client.Player.OnXPSkill() == MsgUpdate.Flags.Normal)
                        client.Player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                }
            }
            if (client.PerfectionStatus.DrainingTouch > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.DrainingTouch))
                {
                    client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                    {
                        Effect = MsgRefineEffect.RefineEffects.DrainingTouch,
                        Id = client.Player.UID,
                        dwParam = client.Player.UID
                    }), true);

                    bool update = false;
                    if (client.Player.HitPoints < client.Player.Owner.Status.MaxHitpoints)
                    {
                        update = true;
                        client.Player.HitPoints = (int)client.Player.Owner.Status.MaxHitpoints;
                    }
                    if (client.Player.Mana < client.Player.Owner.Status.MaxMana)
                    {
                        update = true;
                        client.Player.Mana = (ushort)client.Player.Owner.Status.MaxMana;
                    }
                    if (update)
                    {
                        client.Player.SendUpdateHP();
                        client.Player.SendUpdate(stream, client.Player.Mana, MsgUpdate.DataType.Mana, false);
                    }
                }
            }
            if (obj.Damage >= attacked.HitPoints)
            {
                uint exp = (uint)attacked.HitPoints;
                attacked.Die(stream, client);
                if (attacked.Map == 1039)
                    return exp / 10;
            }
            else
            {
                attacked.HitPoints -= (int)obj.Damage;

                if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures, Role.SobNpc.StaticMesh.Pole))
                    Game.MsgTournaments.MsgSchedules.GuildWar.UpdateScore(client.Player, obj.Damage);
                else if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures, MsgTournaments.MsgSuperGuildWar.FurnituresType.Pole))
                    Game.MsgTournaments.MsgSchedules.SuperGuildWar.UpdateScore(client.Player, obj.Damage);
               if (Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Bases.ContainsKey(attacked.UID))
                {
                    Game.MsgTournaments.MsgSchedules.CaptureTheFlag.UpdateFlagScore(client.Player, attacked, obj.Damage, stream);
                }
                else if (Game.MsgTournaments.MsgSchedules.ClanWar.Process == MsgTournaments.ProcesType.Alive)
                {
                    var clanWar = Game.MsgTournaments.MsgSchedules.ClanWar.GetWar(client);// all clan war maps run at once
                    if (clanWar != null)
                        clanWar.UpdateScore(client.Player, obj.Damage);
                }
               else if (MsgTournaments.MsgNobilityPole.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgNobilityPole.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgNobilityPole.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgNobilityPole1.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgNobilityPole1.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgNobilityPole1.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgNobilityPole2.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgNobilityPole2.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgNobilityPole2.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgNobilityPole3.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgNobilityPole3.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgNobilityPole3.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgSchedules.GuildWarScore.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgSchedules.GuildWarScore.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgSchedules.GuildWarScore.UpdateScore(stream, obj.Damage, client.Player.MyGuild);
               //if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.UnionWar.Furnitures, Role.SobNpc.StaticMesh.Pole))
               //    Game.MsgTournaments.MsgSchedules.UnionWar.UpdateScore(client.Player, obj.Damage);
               if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   Game.MsgTournaments.MsgSchedules.EliteGuildWar.UpdateScore(client.Player, obj.Damage);
               else if (MsgTournaments.MsgMonkClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgMonkClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgMonkClass.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgNinjaClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgNinjaClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgNinjaClass.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgPariteClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgPariteClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgPariteClass.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgArcherClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgArcherClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgArcherClass.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgFireClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgFireClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgFireClass.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgKungfuKing.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgKungfuKing.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgKungfuKing.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgWindWalker.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgWindWalker.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgWindWalker.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgWaterClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgWaterClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgWaterClass.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgWarriorClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgWarriorClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgWarriorClass.UpdateScore(stream, obj.Damage, client.Player);
               else if (MsgTournaments.MsgTrojanClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgTrojanClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
                   MsgTournaments.MsgTrojanClass.UpdateScore(stream, obj.Damage, client.Player);
                if (attacked.Map == 1039 || attacked.Map == 1038)
                    return obj.Damage / 1000;
               
            }
            return 0;
        }
    }
}
