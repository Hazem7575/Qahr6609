using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;
using Conquer6609.Database;

namespace Conquer6609.Role
{
    public unsafe partial class Player
    {

        public unsafe void Dead(Role.Player killer, ushort DeadX, ushort DeadY, uint KillerUID)
        {
            if (UnlimitedArenaRooms.IsArenaRoom(DynamicID))
                return;
            Conquer6609.Framework.Modules.ModuleManager.PlayerKilled(killer, this);
            if (OnTransform && TransformInfo != null)
            {
                TransformInfo.FinishTransform();
            }
            else if (OnTransform)
                TransformationID = 0;
            if (Owner.OnAutoAttack)
            {
                Owner.OnAutoAttack = false;
            }
            if (killer != null)
            {
                #region Die Guild System #Conquer6609
                if (this.Owner.Player.MyGuild != null)
                {
                    if (killer.MyGuild != null && killer.MyGuildMember != null && this.Owner.Player.MyGuildMember != null && (Owner.Map.ID == 1011 || Owner.Map.ID == 1001 || Owner.Map.ID == 1015 || Owner.Map.ID == 1020))
                    {
                        killer.Owner.Player.MyGuildMember.PkDonation += 2;
                        if (this.Owner.Player.MyGuildMember.PkDonation > 2)
                            this.Owner.Player.MyGuildMember.PkDonation -= 2;
                        string deathMapName;
                        if (!Database.Server.MapName.TryGetValue(Map, out deathMapName))
                            deathMapName = Map.ToString();
                        string ms1 = "The (" + killer.GuildRank.ToString() + ")" + killer.Name + " He killed : (" + GuildRank.ToString() + ")" + Name + " from guild " + MyGuild.GuildName + " in " + deathMapName + "";
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Program.SendGlobalPackets.Enqueue(new Conquer6609.Game.MsgServer.MsgMessage(ms1, Conquer6609.Game.MsgServer.MsgMessage.MsgColor.white, Conquer6609.Game.MsgServer.MsgMessage.ChatMode.Guild).GetArray(stream));
                        }

                    }
                }
                #endregion
                if (Map == 10166 && killer.Map == 10166)
                {
                    if (killer.HuntingBounsLayer < 9)
                    {
                        killer.HuntingBounsLayer += 1;
                        killer.HuntingBounsStamp = new Extensions.Time32().AddMinutes(5);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, killer.UID, 1);
                            stream = packet.Append(stream, MsgUpdate.DataType.HuntingBouns, (byte)0, (uint)(killer.HuntingBounsStamp - Program.CurrentTime).AllSeconds, 0, killer.HuntingBounsLayer);
                            stream = packet.GetArray(stream);
                            killer.Owner.Send(stream);
                        }
                    }
                    
                    if (HuntingBounsLayer > 0)
                    {
                        HuntingBounsLayer -= 1;
                        if (HuntingBounsLayer == 0)
                            HuntingBounsStamp = new Extensions.Time32().AddMinutes(-4).AddSeconds(-50);
                        else
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                                stream = packet.Append(stream, MsgUpdate.DataType.HuntingBouns, (byte)0, (uint)(HuntingBounsStamp - Program.CurrentTime).AllSeconds, 0, HuntingBounsLayer);
                                stream = packet.GetArray(stream);
                                Owner.Send(stream);
                            }
                        }
                    }
                }
            }
            GhostStamp = DateTime.Now.AddMilliseconds(1000);
            Owner.OnAutoAttack = false;
            Owner.Player.XPCount = 0;
            Owner.SendSysMesage("You are dead.", MsgMessage.ChatMode.System);
            if (Game.MsgTournaments.MsgSchedules.ArenaCpsBattle.InTournament(Owner) && Game.MsgTournaments.MsgSchedules.ArenaCpsBattle.InTournament(killer.Owner))
            {
                Game.MsgTournaments.MsgSchedules.ArenaCpsBattle.End(killer.UID, UID);
                killer.TournamentKills += 1;
            }
            GetPkPkPoints = true;
            if (this.ContainFlag(MsgUpdate.Flags.RedName)
                || this.ContainFlag(MsgUpdate.Flags.BlackName)
                || this.ContainFlag(MsgUpdate.Flags.FlashingName))
                GetPkPkPoints = false;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                if (OnAutoHunt && killer != null)
                {
                    if (MainFlag == MainFlagType.CanClaim || MainFlag == MainFlagType.ClaimGift)
                    {
                        Owner.Send(MsgAutoHunt.AutoHuntCreate(stream, 5, 0, AutoHuntExp, killer.Name));
                        if (AutoHuntExp > 0)
                        {
                            Owner.IncreaseAutoExperience(stream, AutoHuntExp);
                            AutoHuntExp = 0;
                        }
                    }
                    else
                    {
                        Owner.Send(MsgAutoHunt.AutoHuntCreate(stream, 6, 0, AutoHuntExp, killer.Name));
                        AutoHuntExp = 0;
                    }
                }

                foreach (var clone in MyClones.GetValues())
                {
                    clone.RemoveThat(this.Owner);
                }
                MyClones.Clear();



                if (!Program.FreePkMap.Contains(Map))
                {
                    if (Associate != null && killer != null)
                    {
                        killer.Associate.AddPKExplorer(killer.Owner, this);
                        Associate.AddEnemy(Owner, killer);
                    }
                }
                if (killer != null)
                {
                    if (killer.Owner.OnSoulSpell != 0)
                    {
                        killer.Send(stream.CreateJiangMessage(killer.Owner.OnSoulSpell));
                    }
                    if (killer.InElitePk || killer.InTeamPk)
                    {
                        killer.TournamentKills += 1;
                    }
                    if (Map == Game.MsgTournaments.MsgTeamDeathMatch.MapID ||
                        (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.DragonWar
                        && Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(Owner))
                        || (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.LastManStand
                        && Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(Owner))
                        || (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.ExtremePk
                        && Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(Owner))
                        || (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.KillerOfElite
                        && Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(Owner)))
                    {
                        killer.TournamentKills += 1;
                    }



                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(Owner))
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.KillerOfElite)
                        {
                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgTheKillerOfElite;
                            tournament.KillSystem.Update(killer.Owner);
                            tournament.KillSystem.CheckDead(this.UID);
                            killer.KillerPkPoints += 1;
                        }


                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.ExtremePk)
                        {
                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgExtremePk;
                            tournament.KillSystem.Update(killer.Owner);
                            tournament.KillSystem.CheckDead(this.UID);
                            tournament.SharePoints(this.Owner, killer.Owner);
                        }
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.LastManStand)
                        {
                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgLastManStand;
                            tournament.KillSystem.Update(killer.Owner);
                            tournament.KillSystem.CheckDead(this.UID);
                        }
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.TeamDeathMatch)
                        {
                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgTeamDeathMatch;
                            tournament.KillSystem.Update(killer.Owner);
                            tournament.KillSystem.CheckDead(this.UID);
                            killer.TeamDeathMacthKills += 1;
                        }
                    }

                    if (Program.MapCounterHits.Contains(Map))
                    {
                        if (Map == 1005)
                        {
                            killer.ArenaKills += 1;

                            ArenaDeads += 1;
                        }

                    }

                    if (killer.PkMode == Flags.PKMode.Jiang && JiangHuActive != 0)
                    {
                        if (Map == 1002 || Map == 1000 || Map == 1015 || Map == 1020 || Map == 1011)
                        {
                            if (killer.MyJiangHu != null && MyJiangHu != null)
                                killer.MyJiangHu.Kill(killer.Owner, this.Owner);
                        }
                    }

                }

                if (BlackSpot)
                {
                    BlackSpot = false;

                    View.SendView(stream.BlackspotCreate(false, UID), true);

                }
                Dead_X = DeadX;
                Dead_Y = DeadY;
                DeadStamp = Extensions.Time32.Now;
                HitPoints = 0;
                ClearFlags();
                AddFlag(Game.MsgServer.MsgUpdate.Flags.Dead, StatusFlagsBigVector32.PermanentFlag, true);



                if (Map == 700)
                {
                    Owner.EndQualifier();
                }

                if (killer != null)
                {
                    if (!killer.ContainFlag(killer.OnXPSkill()))
                    {
                        killer.XPCount++;
                    }



                    InteractQuery action = new InteractQuery()
                    {
                        UID = killer.UID,
                        X = DeadX,
                        Y = DeadY,
                        AtkType = MsgAttackPacket.AttackID.Death,
                        KillCounter = killer.KillCounter,
                        SpellID = (ushort)(Database.ItemType.IsBow(killer.Owner.Equipment.RightWeapon) ? 5 : 1),
                        OpponentUID = UID,
                    };
                    View.SendView(stream.InteractionCreate(&action), true);


                    if (!Program.NoDropItems.Contains(Map) && !Program.FreePkMap.Contains(Map))
                    {
                        if (killer.PkMode != Flags.PKMode.Jiang)
                            CheckDropItems(killer, stream);

                        CheckPkPoints(killer);
                    }

                }
                else
                {


                    InteractQuery action = new InteractQuery()
                    {
                        UID = KillerUID,
                        X = DeadX,
                        Y = DeadY,
                        AtkType = MsgAttackPacket.AttackID.Death,
                        OpponentUID = UID
                    };
                    View.SendView(stream.InteractionCreate(&action), true);


                    if (!Program.NoDropItems.Contains(Map) && !Program.FreePkMap.Contains(Map))
                    {
                        //CheckDropItems(killer, stream);
                    }

                }
            }
        }
        public void CheckDropItems(Role.Player killer, ServerSockets.Packet stream)
        {
            if (OnMyOwnServer == false)
                return;
            if (Map == 3935)
                return;
            try
            {
                ushort x = X;
                ushort y = Y;

                uint DropMoney = (uint)(Money + 1 / 12);
                 if (DropMoney > 1)
                 {
                     DropMoney = (uint)Program.GetRandom.Next(1, (int)DropMoney);
                     if (Owner.Map.AddGroundItem(ref x, ref y))
                     {


                         Game.MsgServer.MsgGameItem DataItem = new Game.MsgServer.MsgGameItem();
                         DataItem.ITEM_ID = Database.ItemType.MoneyItemID((uint)DropMoney);
                         DataItem.Durability = (ushort)Program.GetRandom.Next(1000, 3000);
                         DataItem.MaximDurability = (ushort)Program.GetRandom.Next(DataItem.Durability, 6000);
                         DataItem.Color = Role.Flags.Color.Red;

                         Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(DataItem, x, y, Game.MsgFloorItem.MsgItem.ItemType.Money, DropMoney, DynamicID, Map, UID, false, Owner.Map);
                         if (Owner.Map.EnqueueItem(DropItem))
                         {
                             DropItem.SendAll(stream,Game.MsgFloorItem.MsgDropID.Visible);

                             if (Money > DropMoney)
                             {
                                 Money -= DropMoney;
                                 SendUpdate(stream, Money, Game.MsgServer.MsgUpdate.DataType.Money);
                             }
                         }
                     }
                 }
                if (x > 5 && y > 5)
                {
                    var inventoryItems = Owner.Inventory.ClientItems.Values.ToArray();
                    if (inventoryItems.Length / 4 > 1)
                    {
                        uint count = (uint)Program.GetRandom.Next(1, (int)(inventoryItems.Length / 4));

                        for (int index = 0; index < count; index++)
                        {
                            try
                            {
                                if (inventoryItems.Length > index && inventoryItems[index] != null)
                                {
                                    var item = inventoryItems[index];
                                    if (item.Position == (ushort)Role.Flags.ConquerItem.AleternanteBottle || item.Position == (ushort)Role.Flags.ConquerItem.Bottle)
                                        return;
                                    if (item.Locked == 0 && item.Inscribed == 0 && item.Bound == 0
                                        && !Database.ItemType.unabletradeitem.Contains(item.ITEM_ID) && !Database.ItemType.IsSash(item.ITEM_ID))
                                    {

                                        ushort New_X = (ushort)Program.GetRandom.Next((ushort)(x - 5), (ushort)(x + 5));
                                        ushort New_Y = (ushort)Program.GetRandom.Next((ushort)(y - 5), (ushort)(y + 5));
                                        if (Owner.Map.AddGroundItem(ref New_X, ref New_Y))
                                        {
                                            DropItem(item, New_X, New_Y, stream);
                                        }
                                    }
                                }
                            }
                            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                        }

                    }
                }
                if (PKPoints >= 30 && killer != null && !Program.FreePkMap.Contains(Map))
                {
                    int Count_DropItem = (PKPoints >= 30 && PKPoints <= 99) ? 1 : 2;
                    var EquipmentArray = Owner.Equipment.CurentEquip.Where(p => p != null &&
                         p.Position != (ushort)Role.Flags.ConquerItem.Bottle && p.Position != (ushort)Role.Flags.ConquerItem.AleternanteBottle
                         && p.Position != (ushort)Role.Flags.ConquerItem.Garment && p.Position != (ushort)Role.Flags.ConquerItem.AleternanteGarment
                         && p.Position != (ushort)Role.Flags.ConquerItem.Steed && p.Position != (ushort)Role.Flags.ConquerItem.SteedMount
                         && p.Position != (ushort)Role.Flags.ConquerItem.RightWeaponAccessory && p.Position != (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory).ToArray();

                    if (EquipmentArray.Length > 0)
                    {
                        int trying = 0;
                        int Dropable = 0;
                        Dictionary<uint, Game.MsgServer.MsgGameItem> ItemsDrop = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
                        do
                        {
                            if (trying == 14)
                                break;
                            byte ArrayPosition = (byte)Program.GetRandom.Next(0, EquipmentArray.Length);
                            var Element = EquipmentArray[ArrayPosition];
                            if (!ItemsDrop.ContainsKey(Element.UID))
                            {
                                ItemsDrop.Add(Element.UID, Element);
                                Dropable++;
                            }
                            trying++;
                        }
                        while (Dropable < Count_DropItem);

                        //remove equip item--------------


                        foreach (var item in ItemsDrop.Values)
                        {

                            Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveEquipment, item.UID, item.Position, 0, 0, 0, 0));

                            Game.MsgServer.MsgGameItem Remover;
                            Owner.Equipment.ClientItems.TryRemove(item.UID, out Remover);
                            if (item.Inscribed == 1)
                            {
                                if (MyGuild != null && MyGuild.MyArsenal != null)
                                {
                                    MyGuild.MyArsenal.Remove(Role.Instance.Guild.Arsenal.GetArsenalPosition(item.ITEM_ID), item.UID);
                                }
                                item.Inscribed = 0;
                            }
                        }
                        //checkGuildBattlePower;
                        if (MyGuild != null)
                            GuildBattlePower = MyGuild.ShareMemberPotency(GuildRank);

                        //compute status
                        Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);

                        //--------------------------------

                        //add container Item
                        foreach (var item in ItemsDrop.Values)
                            Owner.Confiscator.AddItem(Owner, killer.Owner, item, stream);
                        //-----------
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public void DropItem(Game.MsgServer.MsgGameItem item, ushort x, ushort y, ServerSockets.Packet stream)
        {
            Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, x, y, Game.MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, UID, false, Owner.Map);

            if (Owner.Map.EnqueueItem(DropItem))
            {
                DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Visible);
                Owner.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);
            }
        }
        private void CheckPkPoints(Role.Player killer)
        {
            if (killer.OnMyOwnServer == true && OnMyOwnServer == false)
                return;
            if (Map == 3935)
                return;

            if (killer.PkMode != Flags.PKMode.Jiang)
            {
                if (!Program.FreePkMap.Contains(Map))
                {
                    if (!this.ContainFlag(Game.MsgServer.MsgUpdate.Flags.RedName) && !this.ContainFlag(Game.MsgServer.MsgUpdate.Flags.BlackName))
                    {
                        if (killer.Map == 5051)
                        {
                            return;
                        }
                        if (HeavenBlessing > 0)
                        {
                            if (killer.HeavenBlessing > 0)
                            {
                                Owner.LoseDeadExperience(killer.Owner);
                            }
                            else
                            {
                                Owner.SendSysMesage("Your Heaven Blessing takes effect! You lose no EXP!", MsgMessage.ChatMode.System);
                                killer.AddCursed(5 * 60);
                            }
                        }
                        else
                            Owner.LoseDeadExperience(killer.Owner);

                    

                        if (GetPkPkPoints)
                        {
                            if (killer.MyGuild != null)
                            {
                                if (killer.MyGuild.Enemy.ContainsKey(GuildID))
                                {
                                    killer.PKPoints += 3;
                                    if (Database.Server.MapName.ContainsKey(Map))
                                    {
                                        if (GuildRank >= Flags.GuildMemberRank.Manager)
                                            killer.MyGuild.SendMessajGuild("The (" + killer.GuildRank.ToString() + ")" + killer.Name + " at killed on (" + GuildRank.ToString() + ")" + Name + " from guild " + MyGuild.GuildName + " in " + Database.Server.MapName[Map] + "", Game.MsgServer.MsgMessage.ChatMode.Guild, Game.MsgServer.MsgMessage.MsgColor.yellow);
                                    }
                                    return;
                                }
                            }
                            if (killer.MyClan != null)
                            {
                                if (killer.MyClan.Enemy.ContainsKey(ClanUID))
                                {
                                    killer.PKPoints += 3;
                                    return;
                                }
                            }
                            if (killer.Associate.Contain(Role.Instance.Associate.Enemy, UID))
                            {
                                killer.PKPoints += 5;
                                return;
                            }
                            killer.PKPoints += 10;
                        }
                    }
                    else
                    {
                        if (PKPoints > 99)//just for black name's
                        {
                            MyKillerName = killer.Name;
                            MyKillerUID = killer.UID;
                            Owner.Teleport(29, 72, 6000, 0, true);

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(Name + " has been captured by " + MyKillerName + " and sent in jail! The world is now safer!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.System).GetArray(stream));
                            }
                        }
                    }

                }

            }
        }
        public unsafe void Revive(ServerSockets.Packet stream)
        {
            ProtectAttack(5 * 1000);//5 secounds

            _hpSetterSent = false;// always resend HP on revive (HitPoints setter early return)
            HitPoints = (int)Owner.Status.MaxHitpoints;
            ClearFlags();
            TransformationID = 0;


            if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwistofWar))
                XPCount = 0;
            SendUpdate(stream, XPCount, MsgUpdate.DataType.XPCircle);

            Stamina = 100;
            SendUpdate(stream, Stamina, MsgUpdate.DataType.Stamina);


            Send(stream.MapStatusCreate(Map, Map, (uint)Owner.Map.TypeStatus));
            View.SendView(GetArray(stream, false), false);
        }
    }

}
