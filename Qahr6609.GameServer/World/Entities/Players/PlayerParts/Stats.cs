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
        public void AddExtraAtributes(ServerSockets.Packet stream, ushort value)
        {
            if (ExtraAtributes >= 362)
            {
                return;
            }
            ExtraAtributes += value;
            Atributes += value;
            SendUpdate(stream, Atributes, MsgUpdate.DataType.Atributes);
        }

        public void UpdateTaoPower(ServerSockets.Packet stream)
        {
            if (Database.AtributesStatus.IsWater(Class) && TaoistPower == 10)
            {
                if (!ContainFlag(MsgUpdate.Flags.FullPowerWater))
                    AddFlag(MsgUpdate.Flags.FullPowerWater, StatusFlagsBigVector32.PermanentFlag, false);
            }
            else if (Database.AtributesStatus.IsFire(Class) && TaoistPower == 10)
            {
                if (!ContainFlag(MsgUpdate.Flags.FullPowerFire))
                    AddFlag(MsgUpdate.Flags.FullPowerFire, StatusFlagsBigVector32.PermanentFlag, false);
            }

            uint icon = Database.AtributesStatus.IsWater(Class) ? (uint)172 : (uint)173;

            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, MsgUpdate.DataType.AppendIcon, icon, 3, (uint)(TaoistPower * 30), 0);
            stream = packet.GetArray(stream);

            View.SendView(stream, true);
        }
        public void SendPowerTaoist(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (TaoistPower > 0)
            {
                uint icon = Database.AtributesStatus.IsWater(Class) ? (uint)172 : (uint)173;

                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = packet.Append(stream, MsgUpdate.DataType.AppendIcon, icon, 3, (uint)(TaoistPower * 30), 0);
                stream = packet.GetArray(stream);

                user.Send(stream);
            }
        }

        public void UpdateTaoistPower(Extensions.Time32 now)
        {
            if (TaoistPower < 10 && Class > 100)
            {
                if (now > TaoistStampPower.AddMinutes(1))//AddSeconds(9)
                {
                    Game.MsgServer.MsgGameItem GameItem;
                    if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon, out GameItem))
                    {
                        if (Database.ItemType.IsTaoistEpicWeapon(GameItem.ITEM_ID))
                        {
                            TaoistPower += 1;
                            using (var rec = new ServerSockets.RecycledPacket())
                                UpdateTaoPower(rec.GetStream());
                        }
                    }
                    TaoistStampPower = Extensions.Time32.Now;
                }
            }
        }
        public unsafe uint ExtraBattlePower
        {
            get { return _extbattle; }
            set
            {
                _extbattle = value;
            }
        }
        public uint GuildBattlePower
        {
            get
            {
                return guildBP;
            }
            set
            {
                ExtraBattlePower -= guildBP;
                guildBP = value;
                ExtraBattlePower += guildBP;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, guildBP, Game.MsgServer.MsgUpdate.DataType.GuildBattlePower);
                }
            }
        }
        public uint ClanBp
        {
            get { return _clanbp; }
            set
            {
                ExtraBattlePower -= _clanbp;
                _clanbp = value;
                ExtraBattlePower += _clanbp;
            }
        }
        private uint MentorBp
        {
            get { return _mentorBp; }
            set
            {
                ExtraBattlePower -= _mentorBp;
                ExtraBattlePower += value;
                _mentorBp = value;
            }
        }
        public unsafe void SetMentorBattlePowers(uint val, uint mentorPotency)
        {

            MentorBp = val;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 2);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.ExtraBattlePower, val);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.ExtraBattlePower, mentorPotency);
                stream = upd.GetArray(stream);
                Owner.Send(stream);
            }
        }
        public bool ContainReflect { get { return Database.AtributesStatus.IsWarrior(SecoundeClass); } }

        public int BattlePower
        {
            get
            {

                int val = (int)(Level + Reborn * 5 + Owner.Equipment.BattlePower + (byte)NobilityRank + ExtraBattlePower);

                if (val > 405) val = 0;//do
                return Math.Min(405, val);
            }
        }
        public int AttackBattlePower
        {
            get
            {
                int val = (int)(Reborn * 5 + Owner.Equipment.BattlePower + (byte)NobilityRank);


                return val;
            }
        }
        public int RealBattlePower
        {
            get
            {
                int val = (int)(Level + Reborn * 5 + Owner.Equipment.BattlePower + (byte)NobilityRank);


                return val;
            }
        }
        public ushort AzureShieldDefence
        {
            get { return azuredef; }
            set
            {
                azuredef = value;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.AzureShield, 60
                        , value, AzureShieldLevel, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);
                }
            }
        }
        //public ushort Stamina = 0;
        public ushort Stamina
        {
            get
            {
                if (UnlimitedArenaRooms.IsArenaRoom(DynamicID))
                    return 150;
                return _stamina;
            }
            set { _stamina = value; }
        }
        public unsafe byte Class
        {
            get { return _class; }
            set
            {
                _class = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, value, Game.MsgServer.MsgUpdate.DataType.Class);
                    }
                    if (MyGuildMember != null)
                        MyGuildMember.Class = value;
                }
            }
        }
        public unsafe ushort Level
        {
            get { return _level; }
            set
            {
                if (Owner.FullLoading)
                {
                    if (_level != 0 && Reborn == 0)
                    {
                        if (_level < 15 && value >= 15)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (Class == 160)
                                {
                                    Class = 161;
                                    SendUpdate(stream, Class, MsgUpdate.DataType.Class);

                                }
                                Owner.Inventory.Add(stream, 3005160);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Owner.Inventory.Add(stream, 3005161);
                                if (Database.AtributesStatus.IsWindWalker(Class))
                                {
                                    if (!QuestGUI.CheckQuest(3797, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    {
                                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WindwalkerGuard, Class, 3797);
                                        QuestGUI.Accept(ActiveQuest, 0);
                                    }
                                }
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Owner.Inventory.Add(stream, 3005164);
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Owner.Inventory.Add(stream, 3005167);
                            }
                        }
                    }
                    if (Database.AtributesStatus.IsTrojan(Class))
                    {
                        if (_level < 15 && value >= 15)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Golem);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hercules))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Hercules);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritHealing))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpiritHealing);

                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                                if (Reborn == 2 && FirstClass == 15 && SecoundeClass == 15)
                                {
                                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonWhirl))
                                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonWhirl);
                                }
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsWarrior(Class))
                    {
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Dash))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Dash);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShieldBlock))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ShieldBlock);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlyingMoon))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FlyingMoon);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicDefender))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicDefender);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaveofBlood))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WaveofBlood);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScarofEarth))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScarofEarth);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicDefender))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicDefender);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DefensiveStance))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DefensiveStance);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwistofWar))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TwistofWar);
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Backfire))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Backfire);

                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsArcher(Class))
                    {
                        if (_level < 23 && value >= 23)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScatterFire))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScatterFire);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PathOfShadow))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.PathOfShadow);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeFlurry))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BladeFlurry);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalWound))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MortalWound);
                            }
                        }
                        if (_level < 46 && value >= 46)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RapidFire))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RapidFire);
                            }
                        }
                        if (_level < 50 && value >= 50)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KineticSpark))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.KineticSpark);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fly))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Fly);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArrowRain))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ArrowRain);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlisteringWave))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BlisteringWave);
                            }
                        }
                        if (_level < 71 && value >= 71)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Intensify))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Intensify);
                            }
                        }
                        if (_level < 90 && value >= 90)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritFocus))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpiritFocus);
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DaggerStorm))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DaggerStorm);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsNinja(Class))
                    {
                        if (_level < 20 && value >= 20)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalDrag))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MortalDrag);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ToxicFog);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwofoldBlades))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TwofoldBlades);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloodyScythe))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BloodyScythe);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShurikenVortex))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ShurikenVortex);
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArcherBane))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ArcherBane);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsMonk(Class))
                    {
                        if (_level < 20 && value >= 20)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TyrantAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TyrantAura);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FendAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FendAura);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RadiantPalm))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RadiantPalm);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Serenity))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Serenity);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tranquility))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Tranquility);
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Compassion))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Compassion);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.EarthAura);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireAura);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MetalAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MetalAura);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WatherAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WatherAura);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WoodAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WoodAura);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsPirate(Class))
                    {
                        if (_level < 15 && value >= 15)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Golem);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Windstorm))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Windstorm);
                            }
                        }
                        if (_level < 20 && value >= 20)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GaleBomb))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.GaleBomb);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdrenalineRush))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AdrenalineRush);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EagleEye))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.EagleEye);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlackbeardsRage))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BlackbeardsRage);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KrakensRevenge))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.KrakensRevenge);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsWindWalker(Class))
                    {
                        if (_level < 3 && value >= 3)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Omnipotence))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Omnipotence);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.JusticeChant))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.JusticeChant);
                            }
                        }
                        if (_level < 15 && value >= 15)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SwirlingStorm))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SwirlingStorm);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShadowofChaser))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ShadowofChaser);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BurntFrost))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BurntFrost);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingSnow))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HealingSnow);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RageofWar))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RageofWar);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HorrorofStomper))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HorrorofStomper);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleBlasts))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TripleBlasts);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thundercloud))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thundercloud);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PeaceofStomper))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.PeaceofStomper);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChillingSnow))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ChillingSnow);

                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RevengeTail))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RevengeTail);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FreezingPelter))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FreezingPelter);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunderbolt))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunderbolt);
                            }
                        }

                    }
                    else if (Database.AtributesStatus.IsLee(Class))
                    {
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedKick))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpeedKick);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ViolentKick))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ViolentKick);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StormKick))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StormKick);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackingSwipe))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CrackingSwipe);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonRoar))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonRoar);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSwing))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonSwing);
                                if (Reborn == 2 && FirstClass == 85 && SecoundeClass == 85)
                                {
                                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFury))
                                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonFury);
                                }
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSlash))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonSlash);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SplittingSwipe))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SplittingSwipe);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsWater(Class))
                    {
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingRain))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HealingRain);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarofAccuracy))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StarofAccuracy);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Revive))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Revive);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedLightning))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpeedLightning);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Vulcano))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Vulcano);

                                if (Reborn == 2 && FirstClass == 135 && SecoundeClass == 135)
                                {
                                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AzureShield))
                                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AzureShield);
                                }

                            }
                        }
                        if (_level < 44 && value >= 44)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Meditation);
                            }
                        }
                        if (_level < 50 && value >= 50)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicShield))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                            }
                        }
                        if (_level < 55 && value >= 55)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Stigma))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                            }
                        }
                        if (_level < 60 && value >= 60)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Invisibility))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Invisibility);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Pray))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Pray);
                            }
                        }
                        if (_level < 81 && value >= 81)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdvancedCure))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AdvancedCure);
                            }
                        }
                        if (_level < 94 && value >= 94)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Nectar))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Nectar);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsFire(Class))
                    {
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedLightning))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpeedLightning);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Vulcano))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Vulcano);

                                if (Reborn == 2 && FirstClass == 145 && SecoundeClass == 145)
                                {
                                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HeavenBlade))
                                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HeavenBlade);
                                }

                            }
                        }
                        if (_level < 44 && value >= 44)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Meditation);
                            }
                        }
                        if (_level < 52 && value >= 52)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireMeteor))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireMeteor);
                            }
                        }
                        if (_level < 55 && value >= 55)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireRing))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireRing);
                            }
                        }
                        if (_level < 65 && value >= 65)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireCircle))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireCircle);
                            }
                        }
                        if (_level < 82 && value >= 82)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Bomb))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Bomb);
                            }
                        }
                    }
                }
                _level = value;
                if (_level >= 140)
                {
                    _level = 140;
                    Experience = 0;
                }
            }
        }
        public unsafe int HitPoints
        {
            get 
            {
                if (UnlimitedArenaRooms.IsArenaRoom(DynamicID))
                    return (int)Owner.Status.MaxHitpoints;
                return _minhitpoints; 
            }
            set
            {
                // Nothing the owner or the team sees would change: skip the team info + HP packets.
                // (Only when the exact HP/MaxHP pair was already sent by this setter and the team
                // entry already holds the same values; never while dead/a ghost, right after a revive
                // (Revive clears _hpSetterSent) or in an Unlimited Arena room, whose getter reports MaxHP.)
                if (value == _minhitpoints && value > 0 && Owner.FullLoading && _hpSetterSent
                    && !UnlimitedArenaRooms.IsArenaRoom(DynamicID)
                    && !ContainFlag(Game.MsgServer.MsgUpdate.Flags.Ghost)
                    && _hpSetterSentMax == Owner.Status.MaxHitpoints && _hpSetterSentHp == HitPoints)
                {
                    var team = Owner.Team;
                    if (team == null)
                        return;
                    var member = team.GetMember(UID);
                    if (member == null || (member.Info.MaxHitpoints == (ushort)Owner.Status.MaxHitpoints
                        && member.Info.MinMHitpoints == (ushort)value))
                        return;
                }
                _minhitpoints = value;
                if (Owner.Team != null)
                {
                    var TeamMember = Owner.Team.GetMember(UID);
                    if (TeamMember != null)
                    {
                        TeamMember.Info.MaxHitpoints = (ushort)Owner.Status.MaxHitpoints;
                        TeamMember.Info.MinMHitpoints = (ushort)value;
                        Owner.Team.SendTeamInfo(TeamMember);
                    }
                }
                if (Owner.FullLoading)
                {
                    _hpSetterSentMax = Owner.Status.MaxHitpoints;
                    _hpSetterSentHp = HitPoints;
                    _hpSetterSent = true;
                    SendUpdateHP();
                }
            }
        }
        // Last HP/MaxHP pair the HitPoints setter sent to the owner (used only by the setter's early return).
        private bool _hpSetterSent;
        private uint _hpSetterSentMax;
        private int _hpSetterSentHp;
        public unsafe ushort Mana
        {
            get { return _mana; }
            set
            {
                _mana = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, value, Game.MsgServer.MsgUpdate.DataType.Mana);
                    }
                }
            }
        }

        public byte HairColor
        {
            get
            {
                return (byte)(Hair / 100);
            }
            set
            {
                Hair = (ushort)((value * 100) + (Hair % 100));
            }
        }
        public byte GetGender
        {
            get
            {
                if (Body % 10 >= 3)
                    return 0;
                else
                    return 1;
            }
        }
        public unsafe ushort Body
        {
            get { return body; }
            set
            {
                body = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, Mesh, Game.MsgServer.MsgUpdate.DataType.Mesh, true);
                    }
                }
            }
        }
        public unsafe ushort TransformationID
        {
            get
            {
                return _transformationid;
            }
            set
            {
                _transformationid = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, Mesh, Game.MsgServer.MsgUpdate.DataType.Mesh, true);
                    }
                }
            }
        }

        //  uint _mesh = 0;
        public unsafe uint Mesh
        {
            get
            {
                //  if (_mesh != 0)
                //    return _mesh;
                //2471671003                     10000000
                return (uint)(TransformationID * 10000000 + Face * 10000 + Body);
            }
            //set { _mesh = value; }
        }
        public uint GetShareBattlePowers(uint target_battlepower)
        {
            return (uint)Database.TutorInfo.ShareBattle(this.Owner, (int)target_battlepower);
        }
    }

}
