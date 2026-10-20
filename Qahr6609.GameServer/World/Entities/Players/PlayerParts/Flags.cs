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
        public void ActiveAttackPotion(int Timer)
        {
            OnAttackPotion = true;
            OnAttackPotionStamp = Extensions.Time32.Now.AddMinutes(Timer);
            AddFlag(MsgUpdate.Flags.Stigma, 60, true);
            Owner.SendSysMesage("Your attack will increase during the next 30 minutes.", MsgMessage.ChatMode.System);

        }
        public void ActiveDefensePotion(int Timer)
        {
            OnDefensePotion = true;
            OnDefensePotionStamp = Extensions.Time32.Now.AddMinutes(Timer);
            AddFlag(MsgUpdate.Flags.Shield, 60, true);
            Owner.SendSysMesage("Your defense will increase during the next 30 minutes.", MsgMessage.ChatMode.System);

        }
        internal void ProtectAttack(int StampMiliSecounds)
        {
            Protect = Extensions.Time32.Now.AddMilliseconds(StampMiliSecounds);
        }
        internal void ProtectJumpAttack(int Secounds)
        {
            ProtectedJumpAttack = Extensions.Time32.Now.AddSeconds(Secounds);
        }
        internal bool AllowAttack()
        {
            return Extensions.Time32.Now > Protect && Extensions.Time32.Now > ProtectedJumpAttack;
        }

        internal void CheckAura()
        {
            if (UseAura != Game.MsgServer.MsgUpdate.Flags.Normal)
            {
                IncreaseStatusAura(UseAura, Aura);
            }
        }
        internal unsafe bool AddAura(Game.MsgServer.MsgUpdate.Flags flag, Database.MagicType.Magic new_aura, int Timer)
        {
            if (flag == UseAura)
            {
                RemoveFlag(UseAura);
                DecreaseStatusAura(UseAura);
                UseAura = Game.MsgServer.MsgUpdate.Flags.Normal;
                return false;
            }
            AuraTimer = Timer;
            if (UseAura != Game.MsgServer.MsgUpdate.Flags.Normal)
            {
                RemoveFlag(UseAura);
                DecreaseStatusAura(UseAura);
                UseAura = Game.MsgServer.MsgUpdate.Flags.Normal;
            }
            UseAura = flag;
            Aura = new_aura;
            IncreaseStatusAura(flag, new_aura);
            AddFlag(flag, Timer, true, 0);

            Game.MsgServer.MsgFlagIcon.ShowIcon icon = MsgFlagIcon.ShowIcon.EarthAura;

            if (flag == Game.MsgServer.MsgUpdate.Flags.FeandAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.FeandAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.TyrantAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.TyrantAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.MetalAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.MetalAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WoodAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.WoodAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WaterAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.WaterAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.FireAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.FireAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.EartAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.EarthAura;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Owner.Send(stream.FlagIconCreate(UID, icon, new_aura.Level, (uint)new_aura.Damage));
            }

            return true;
        }
        public void DecreaseAura(Game.MsgServer.MsgUpdate.Flags flag)
        {
            this.DecreaseStatusAura(flag);
        }
        private void DecreaseStatusAura(Game.MsgServer.MsgUpdate.Flags flag)
        {
            if (AuraStatusIncreased == true)
            {
                if (flag == Game.MsgServer.MsgUpdate.Flags.FeandAura)
                    Owner.Status.Immunity -= (uint)(Aura.Damage * 100);
                else if (flag == Game.MsgServer.MsgUpdate.Flags.TyrantAura)
                    Owner.Status.CriticalStrike -= (uint)(Aura.Damage * 100);
                else if (flag == Game.MsgServer.MsgUpdate.Flags.MetalAura)
                    Owner.Status.MetalResistance -= (uint)Aura.Damage;
                else if (flag == Game.MsgServer.MsgUpdate.Flags.WoodAura)
                    Owner.Status.WoodResistance -= (uint)Aura.Damage;
                else if (flag == Game.MsgServer.MsgUpdate.Flags.WaterAura)
                    Owner.Status.WaterResistance -= (uint)Aura.Damage;
                else if (flag == Game.MsgServer.MsgUpdate.Flags.FireAura)
                    Owner.Status.FireResistance -= (uint)Aura.Damage;
                else if (flag == Game.MsgServer.MsgUpdate.Flags.EartAura)
                    Owner.Status.EarthResistance -= (uint)Aura.Damage;
                AuraStatusIncreased = false;
            }
        }
        private void IncreaseStatusAura(Game.MsgServer.MsgUpdate.Flags flag, Database.MagicType.Magic new_aura)
        {
            if (AuraStatusIncreased == false)
            {
                if (flag == Game.MsgServer.MsgUpdate.Flags.FeandAura)
                    Owner.Status.Immunity += (uint)(new_aura.Damage * 100);
                else if (flag == Game.MsgServer.MsgUpdate.Flags.TyrantAura)
                    Owner.Status.CriticalStrike += (uint)(new_aura.Damage * 100);
                else if (flag == Game.MsgServer.MsgUpdate.Flags.MetalAura)
                    Owner.Status.MetalResistance += (uint)new_aura.Damage;
                else if (flag == Game.MsgServer.MsgUpdate.Flags.WoodAura)
                    Owner.Status.WoodResistance += (uint)new_aura.Damage;
                else if (flag == Game.MsgServer.MsgUpdate.Flags.WaterAura)
                    Owner.Status.WaterResistance += (uint)new_aura.Damage;
                else if (flag == Game.MsgServer.MsgUpdate.Flags.FireAura)
                    Owner.Status.FireResistance += (uint)new_aura.Damage;
                else if (flag == Game.MsgServer.MsgUpdate.Flags.EartAura)
                    Owner.Status.EarthResistance += (uint)new_aura.Damage;
                AuraStatusIncreased = true;
            }
        }
        public void RemoveBuffersMovements(ServerSockets.Packet stream)
        {
            InUseIntensify = false;

            RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Praying);
            RemoveFlag(Game.MsgServer.MsgUpdate.Flags.CastPray);
            if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.MagicDefender))
            {
                RemoveFlag(Game.MsgServer.MsgUpdate.Flags.MagicDefender);
                SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.MagicDefender, 0 , 0, 0, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);
            }
        }
        public void AddSpellFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0)
        {
            if (BitVector.ContainFlag((int)Flag))
                BitVector.TryRemove((int)Flag);
            AddFlag(Flag, Secounds, RemoveOnDead, StampSecounds);
        }
        public bool AddHisFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0, uint showamount = 0, uint amount = 0)
        {//Halo_Flag
            if (!BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryAdd((int)Flag, Secounds, RemoveOnDead, StampSecounds);
                UpdateFlagOffset();
               
                return true;
            }
            return false;
        }
        public bool AddFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0, uint showamount = 0, uint amount = 0,bool Halo = false)
        {//Halo_Flag
            if (!BitVector.ContainFlag((int)Flag))
            {
                if (Halo)
                {
                    int Hour = Secounds / 60 / 60;
                    Halo_Flag.AddHalo(this.Owner, Flag, Hour);
                }
                BitVector.TryAdd((int)Flag, Secounds, RemoveOnDead, StampSecounds);
                UpdateFlagOffset();
                if ((int)Flag >= 52 && (int)Flag <= 60)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        View.SendView(stream.GameUpdateCreate(UID, (Game.MsgServer.MsgGameUpdate.DataType)Flag, true, showamount, (uint)Secounds, amount), true);

                    }
                }
                return true;
            }
            return false;
        }
        public bool RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryRemove((int)Flag);
                UpdateFlagOffset();
                if (Flag == MsgUpdate.Flags.Oblivion)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Owner.IncreaseExperience(stream, Owner.ExpOblivion);
                    }
                    OblivionCounter = 0;
                    Owner.ExpOblivion = 0;
                }
                if ((int)Flag >= 52 && (int)Flag <= 60)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        //Owner.Send(stream.GameUpdateCreate(UID, (Game.MsgServer.MsgGameUpdate.DataType)Flag, false, 0, 0, 0));
                        View.SendView(stream.GameUpdateCreate(UID, (Game.MsgServer.MsgGameUpdate.DataType)Flag, false, 0, 0, 0), true);

                    }
                }
                return true;
            }
            return false;
        }
        public bool UpdateFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool SetNewTimer, int MaxTime)
        {
            return BitVector.UpdateFlag((int)Flag, Secounds, SetNewTimer, MaxTime);
        }
        public void ClearFlags()
        {
            BitVector.GetClear();
            UpdateFlagOffset();
        }
        public bool ContainFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            return BitVector.ContainFlag((int)Flag);
        }
        public bool CheckInvokeFlag(Game.MsgServer.MsgUpdate.Flags Flag, Extensions.Time32 timer32)
        {
            return BitVector.CheckInvoke((int)Flag, timer32);
        }
        public unsafe void UpdateFlagOffset()
        {
            SendUpdate(BitVector.bits, Game.MsgServer.MsgUpdate.DataType.StatusFlag, true);
        }
        public unsafe void SendUpdateHP()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var MyStream = rec.GetStream();
                Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(MyStream, UID, 2);
                MyStream = Upd.Append(MyStream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, Owner.Status.MaxHitpoints);
                MyStream = Upd.Append(MyStream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, HitPoints);
                MyStream = Upd.GetArray(MyStream);
                Owner.Send(MyStream);
            }
        }
        public void CreateExpProtection(ServerSockets.Packet stream, uint Time, bool uppdate = true)
        {
            if (uppdate)
                ExpProtection = Time;
            Game.MsgServer.MsgUpdate update = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = update.Append(stream, Game.MsgServer.MsgUpdate.DataType.ExpProtection, new uint[5] { 0, ExpProtection, 0, 0, 0 });
            stream = update.GetArray(stream);
            Owner.Send(stream);
        }
        public unsafe void CreateExtraExpPacket(ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgUpdate update = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = update.Append(stream, Game.MsgServer.MsgUpdate.DataType.DoubleExpTimer, new uint[5] { 0, DExpTime, 0, (uint)(RateExp * 100), 0 });
            stream = update.GetArray(stream);
            Owner.Send(stream);
        }
        public void AddHeavenBlessing(ServerSockets.Packet stream, int Time)
        {
            if (!ContainFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing))
                HeavenBlessTime = Extensions.Time32.Now;
            if (Time > 60 * 60 * 24)
                Owner.SendSysMesage("You`ve received " + Time / (60 * 60 * 24) + " days` blessing time.", Game.MsgServer.MsgMessage.ChatMode.System);
            else
            {
                Owner.SendSysMesage("You`ve received " + (Time / 60) / 60 + " hours` blessing time.", Game.MsgServer.MsgMessage.ChatMode.System);
            }


            bool None = HeavenBlessing == 0;
            HeavenBlessTime = HeavenBlessTime.AddSeconds(Time);

            HeavenBlessing += Time;
            CreateHeavenBlessPacket(stream, None);

            if (MyMentor != null)
            {
                MyMentor.Mentor_Blessing += (uint)(Time / 10000);
                Role.Instance.Associate.Member mee;
                if (MyMentor.Associat.ContainsKey(Role.Instance.Associate.Apprentice))
                {
                    if (MyMentor.Associat[Role.Instance.Associate.Apprentice].TryGetValue(UID, out mee))
                    {
                        mee.Blessing += (uint)(Time / 10000);
                    }
                }
            }

        }
        public void CreateHeavenBlessPacket(ServerSockets.Packet stream, bool ResetOnlineTraining)
        {
            if (HeavenBlessing > 0)
            {
                if (ResetOnlineTraining)
                {
                    ReceivePointsOnlineTraining = Extensions.Time32.Now.AddMinutes(1);
                    OnlineTrainingTime = Extensions.Time32.Now.AddMinutes(10);
                }
                AddFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing, Role.StatusFlagsBigVector32.PermanentFlag, false);
                SendUpdate(stream, HeavenBlessing, Game.MsgServer.MsgUpdate.DataType.HeavensBlessing, false);

                SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.Show, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);
                if (Map == 601 || Map == 1039)
                    SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.InTraining, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);
                SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { "bless" });
            }
        }
        public bool OnSpeedFlags()
        {
            return ContainFlag(MsgUpdate.Flags.Cyclone) || ContainFlag(MsgUpdate.Flags.BladeFlurry) || ContainFlag(MsgUpdate.Flags.Omnipotence) || ContainFlag(MsgUpdate.Flags.Oblivion) || ContainFlag(MsgUpdate.Flags.CannonBarrage) || ContainFlag(MsgUpdate.Flags.FatalStrike);
        }
    }

}
