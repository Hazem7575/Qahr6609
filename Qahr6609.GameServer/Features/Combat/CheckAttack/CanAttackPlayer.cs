using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.CheckAttack
{
    /// <summary>
    /// Set of map / dynamic-map ids (Program.FreePkMap, Program.BlockAttackMap). Same Add/Contains/collection
    /// initializer use as the List it replaces, but O(1) lookups on every attack check and safe to Add to
    /// from event threads while the socket thread reads it.
    /// </summary>
    public sealed class MapIdSet : IEnumerable<uint>
    {
        private readonly System.Collections.Concurrent.ConcurrentDictionary<uint, byte> ids = new System.Collections.Concurrent.ConcurrentDictionary<uint, byte>();
        public void Add(uint id)
        {
            ids[id] = 0;
        }
        public bool Remove(uint id)
        {
            byte dummy;
            return ids.TryRemove(id, out dummy);
        }
        public bool Contains(uint id)
        {
            return ids.ContainsKey(id);
        }
        public int Count
        {
            get { return ids.Count; }
        }
        public IEnumerator<uint> GetEnumerator()
        {
            return ids.Keys.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public class CanAttackPlayer
    {
        public static bool Verified(Client.GameClient client, Role.Player attacked
      , Database.MagicType.Magic DBSpell)
        {
            if (client.Player.ContainFlag(MsgUpdate.Flags.ChillingSnow))
            {
                ProcessColdTime.ProcessAttack.Enqueue(new ProcessColdTime.Record()
                {
                    Attacker = client,
                    Attacked = attacked.Owner
                });
            }
            if (attacked.ContainFlag(MsgUpdate.Flags.ChillingSnow))
            {
                 if (DBSpell != null)
                {
                    if (DBSpell.IsSpellWithColdTime)
                    {
                        if (Role.Core.Rate(100 - 5 * (attacked.BattlePower - client.Player.BattlePower)))
                        {
                            int IncreasedColdTime = 0;
                            MsgSpell Spell;
                            if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ChillingSnow, out Spell))
                            {
                                var DBSpells = Database.Server.Magic[(ushort)Role.Flags.SpellID.ChillingSnow];
                                IncreasedColdTime = (int)DBSpells[(ushort)Math.Min(DBSpells.Count - 1, Spell.Level)].Damage;
                            }
                            if (client.MySpells.ClientSpells.TryGetValue(DBSpell.ID, out Spell))
                            {
                                Spell.ColdTime = Spell.ColdTime.AddMilliseconds(IncreasedColdTime * 1000);
                            }
                        }
                    }
                }
            }
            if (attacked.Map == 700 && attacked.DynamicID == 0)
                return false;
            if (!MsgTournaments.MsgSchedules.DragonIsland.Attackable(attacked.Map, attacked.X, attacked.Y))
                return false;
            if (!attacked.Alive)
                return false;
            if (attacked.Invisible)
                return false;
            if (client.Player.PkMode == Role.Flags.PKMode.Peace)
                return false;
            if (client.Player.Map == 3935)
            {
                foreach (var server in Database.GroupServerList.GroupServers.Values)
                {
                    if (Role.Core.GetDistance((ushort)server.X, (ushort)server.Y, attacked.X, attacked.Y) <= 8)
                        return false;
                }
            }

            if (client.Player.Map == 1002)
            {
                if (client.Player.OnMyOwnServer)
                {
                    if (attacked.OnMyOwnServer == false)
                        return true;
                }
                else
                {
                    return false;
                }
            }

            if (client.Player.UID == attacked.UID)
                return false;
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.LastManStand)
            {
                if(Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if(Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.KillerOfElite)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }
     
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.ExtremePk)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }
            
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.DragonWar)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }

            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.TeamDeathMatch)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                {
                    if(Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
                }
            }


            if (attacked.ContainFlag(MsgUpdate.Flags.Fly))
            {
                if (DBSpell == null)
                    return false;
                else if (!DBSpell.AttackInFly)
                    return false;
            }
            if (attacked.Owner.IsWatching())
            {
                return false;
            }
            if (client.IsWatching())
            {
                return false;
            }
            if (!attacked.AllowAttack())
                return false;
            if (client.InTeamQualifier())
            {
                if (client.Team != null && client.Player.Map== 700)
                {
                    if (!client.Team.Members.ContainsKey(attacked.UID))
                        return true;
                    else
                        return false;
                }
            }
            if (client.Player.Map == MsgTournaments.MsgCaptureTheFlag.MapID)
            {
                if (!MsgTournaments.MsgSchedules.CaptureTheFlag.Attackable(attacked))
                    return false;
            }
            if (client.Player.Map == MsgTournaments.MsgTeamDeathMatch.MapID)
            {
                if (client.Player.GarmentId == attacked.GarmentId)
                    return false;
            }
            if (client.Player.PkMode == Role.Flags.PKMode.Jiang)
            {
                // Jiang PK requires both attacker and target to have JiangHu active.
                if (client.Player.JiangHuActive != 0 && attacked.JiangHuActive != 0)
                {
                    if (client.Player.Map == 1002 || client.Player.Map == 1000 || client.Player.Map == 1015 || client.Player.Map == 1020 || client.Player.Map == 1011)
                    {
                        if (client.Player.JiangPkFlag != Role.Instance.JiangHu.AttackFlag.None)
                        {
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitFriends) == Role.Instance.JiangHu.AttackFlag.NotHitFriends)
                            {
                                return !client.Player.Associate.Contain(Role.Instance.Associate.Friends, attacked.UID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NoHitAlliesClan) == Role.Instance.JiangHu.AttackFlag.NoHitAlliesClan)
                            {
                                if (client.Player.MyClan != null)
                                    return !client.Player.MyClan.Ally.ContainsKey(attacked.ClanUID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitAlliedGuild) == Role.Instance.JiangHu.AttackFlag.NotHitAlliedGuild)
                            {
                                if (client.Player.MyGuild != null)
                                    return !client.Player.MyGuild.Ally.ContainsKey(attacked.GuildID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitClanMembers) == Role.Instance.JiangHu.AttackFlag.NotHitClanMembers)
                            {
                                return !(client.Player.ClanUID == attacked.ClanUID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitGuildMembers) == Role.Instance.JiangHu.AttackFlag.NotHitGuildMembers)
                            {
                                return !(client.Player.GuildID == attacked.GuildID);
                            }
                        }
                        return true;
                    }
                }
                else
                    return false;
            }
           
            if (client.Player.Map == 4000 || client.Player.Map == 4003 || client.Player.Map == 4006 || client.Player.Map == 4008 || client.Player.Map == 4009
                || client.Player.Map == 4020)
                return false;
            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.DragonWar)
            {
                if (Program.BlockAttackMap.Contains(client.Player.Map) && client.Player.DynamicID == 0)
                    return false;
                else if (client.Player.DynamicID != 0)
                {
                    if (Program.BlockAttackMap.Contains(client.Player.DynamicID))
                        return false;
                }
            }
            if (Program.BlockAttackMap.Contains(client.Player.Map) && client.Player.DynamicID == 0)
                return false;
            else if (client.Player.DynamicID != 0)
            {
                if (Program.BlockAttackMap.Contains(client.Player.DynamicID))
                    return false;
            }
            if (client.Player.PkMode == Role.Flags.PKMode.Team)
            {
                if (client.Team != null)
                {
                    if (client.Team.Members.ContainsKey(attacked.UID))
                        return false;
                }

                if (client.Player.Associate.Contain(Role.Instance.Associate.Friends, attacked.UID))
                {
                    if (client.Player.Map != 1038 && client.Player.Map != 2057)
                        return false;
                }

                if (client.Player.MyGuild != null)
                {
                    if (client.Player.GuildID == attacked.GuildID)
                        return false;

                    if (attacked.MyGuild != null)
                    {
                        if (client.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                            return false;
                    }
                }
                if (client.Player.MyClan != null)
                {
                    if (client.Player.ClanUID == attacked.ClanUID)
                        return false;

                    if (attacked.MyClan != null)
                    {
                        if (client.Player.MyClan.Ally.ContainsKey(attacked.ClanUID))
                            return false;
                    }
                }
            }
            if (Program.ServerConfig.IsInterServer == false)
            {
                if (client.Player.PkMode != Role.Flags.PKMode.Capture && client.Player.PkMode != Role.Flags.PKMode.Peace)
                {
                    if (!attacked.ContainFlag(MsgUpdate.Flags.FlashingName) && !attacked.ContainFlag(MsgUpdate.Flags.RedName) && !attacked.ContainFlag(MsgUpdate.Flags.BlackName))
                    {
                        if (!Program.FreePkMap.Contains(attacked.Map) && attacked.DynamicID == 0)
                            client.Player.AddFlag(MsgUpdate.Flags.FlashingName, 30, true);

                    }
                    return true;
                }
            }
            if (client.Player.PkMode == Role.Flags.PKMode.CS)
            {
                if (client.Player.ServerID == attacked.ServerID)
                    return false;
            }
            if (client.Player.PkMode == Role.Flags.PKMode.Union)
            {
                if (client.Player.InUnion && attacked.InUnion)
                {
                    if (client.Player.MyUnion.UID == attacked.MyUnion.UID)
                        return false;
                }
            }
            if (client.Player.PkMode == Role.Flags.PKMode.Capture)
            {
                if (attacked.ContainFlag(MsgUpdate.Flags.FlashingName) || attacked.ContainFlag(MsgUpdate.Flags.BlackName))
                    return true;
                else 
                    return false;
            }
         
            return true;
        }
    }
}
