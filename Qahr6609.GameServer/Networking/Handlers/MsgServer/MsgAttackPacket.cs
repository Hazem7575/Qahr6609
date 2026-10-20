using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct InteractQuery
    {
        public static InteractQuery ShallowCopy(InteractQuery item)
        {
            return (InteractQuery)item.MemberwiseClone();
        }

        public int PacketStamp;
        public int TimeStamp;
        public uint UID;
        public uint OpponentUID;
        public ushort X;
        public ushort Y;
        public MsgAttackPacket.AttackID AtkType;//24
        public ushort SpellID;//28
        public bool KilledMonster
        {
            get { return (SpellID == 1); }
            set { SpellID = (ushort)(value ? 1 : 0); }
        }
        public ushort SpellLevel;//30
        public int Data
        {
            get { fixed (void* ptr = &X) { return *((int*)ptr); } }
            set { fixed (void* ptr = &X) { *((int*)ptr) = value; } }
        }
        public int dwParam
        {
            get { fixed (void* ptr = &SpellLevel) { return *((int*)ptr); } }
            set { fixed (void* ptr = &SpellLevel) { *((int*)ptr) = value; } }
        }
        public ushort KillCounter
        {
            get { return SpellLevel; }
            set { SpellLevel = value; }
        }
        public int Damage
        {
            get { fixed (void* ptr = &SpellID) { return *((int*)ptr); } }
            set { fixed (void* ptr = &SpellID) { *((int*)ptr) = value; } }
        }
        public bool OnCounterKill
        {

            get { return Damage != 0; }
            set { Damage = value ? 1 : 0; }
        }
        public uint ResponseDamage;//32
        public MsgAttackPacket.AttackEffect Effect;//36
        public uint Unknow;
    }

    public unsafe static class MsgAttackPacket
    {

        [Flags]
        public enum AttackEffect : uint
        {
            None = 0,

            Block = 1 << 0,
            Penetration = 1 << 1,
            CriticalStrike = 1 << 2,
            Imunity = 1 << 3,
            Break = Imunity | Penetration,
            ResistMetal = 1 << 4,
            ResistWood = 1 << 5,
            ResistWater = 1 << 6,
            ResistFire = 1 << 7,
            ResistEarth = 1 << 8,
            AddStudyPoint = 1 << 9,
            LuckyStrike = 1 << 10
        }
        public enum AttackID : uint
        {
            None = 0,
            Physical = 2,
            Magic = 24,
            Archer = 28,
            RequestMarriage = 8,
            AcceptMarriage = 9,
            Death = 14,
            Reflect = 26,
            Dash = 27,//28?
            MonkMelee = 34,

            UpdateHunterJar = 36,
            ClaimCP = 37,
            Obtained = 39,
            MerchantAccept = 40,
            MerchantRefuse = 41,
            MerchantProgress = 42,

            CounterKillSwitch = 44,
            Scapegoat = 43,

            FatalStrike = 45,
            InteractionRequest = 46,
            InteractionAccept = 47,
            InteractionRefuse = 48,
            InteractionEffect = 49,
            InteractionStopEffect = 50,
            InMoveSpell = 53,
            BlueDamage = 55,
            BackFire = 57,
            Unknown = 58

        }
        public static unsafe void Interaction(this ServerSockets.Packet stream, InteractQuery* pQuery, bool AutoAttack)
        {

            stream.ReadUnsafe(pQuery, sizeof(InteractQuery));

            if (pQuery->AtkType == AttackID.Magic && AutoAttack == false)
            {
                DecodeMagicAttack(pQuery);
            }
        }

        public static unsafe ServerSockets.Packet InteractionCreate(this ServerSockets.Packet stream, InteractQuery* pQuery)
        {

            stream.InitWriter();

            stream.WriteUnsafe(pQuery, sizeof(InteractQuery));
            stream.Finalize(GamePackets.Attack);

            return stream;
        }

        private static unsafe void DecodeMagicAttack(InteractQuery* pQuery)
        {
            int magicType, magicLevel;
            BitUnfold32(pQuery->Damage, out magicType, out magicLevel);

            magicType = (ushort)(ExchangeShortBits(((ushort)magicType ^ (uint)pQuery->UID ^ 0X76Cb), 16 - 3) + 0x14be);
            magicLevel = (ushort)(((byte)magicLevel) ^ 0x21);

            pQuery->Damage = BitFold32(magicType, magicLevel);
            pQuery->OpponentUID = (uint)((ExchangeLongBits((uint)pQuery->OpponentUID, 13) ^ (uint)pQuery->UID ^ 0x5f2d2463) + 0x8b90b51a);
            pQuery->X = (ushort)(ExchangeShortBits(((ushort)pQuery->X ^ (uint)pQuery->UID ^ 0x2ed6), 16 - 1) + 0xdd12);
            pQuery->Y = (ushort)(ExchangeShortBits(((ushort)pQuery->Y ^ (uint)pQuery->UID ^ 0xb99b), 16 - 5) + 0x76de);
        }
        public static int BitFold32(int lower16, int higher16)
        {
            return (lower16) | (higher16 << 16);
        }
        public static void BitUnfold32(int bits32, out int lower16, out int upper16)
        {
            lower16 = (int)(bits32 & UInt16.MaxValue);
            upper16 = (int)(bits32 >> 16);
        }
        public static void BitUnfold64(ulong bits64, out int lower32, out int upper32)
        {
            lower32 = (int)(bits64 & UInt32.MaxValue);
            upper32 = (int)(bits64 >> 32);
        }
        private static uint ExchangeShortBits(uint data, int bits)
        {
            data &= 0xffff;
            return ((data >> bits) | (data << (16 - bits))) & 0xffff;
        }

        public static uint ExchangeLongBits(uint data, int bits)
        {
            return (data >> bits) | (data << (32 - bits));
        }
        [PacketAttribute(GamePackets.Attack)]
        public static void HandlerProcess(Client.GameClient user, ServerSockets.Packet stream)
        {

            user.OnAutoAttack = false;
            user.Player.RemoveBuffersMovements(stream);
            user.Player.Action = Role.Flags.ConquerAction.None;
            if (user.Map.BaseID == 700)
            {
                if (user.InQualifier() && user.ArenaMatch.FightStamp.AddSeconds(11) > DateTime.Now)
                    return;
            }
            InteractQuery Attack;
            stream.Interaction(&Attack, user.OnAutoAttack);
            if (user.Player.ActivePick)
                user.Player.RemovePick(stream);

            Process(user, Attack);

        }


        // The former ProcessAttack queue (its own thread polling every 5 ms) was removed: nothing enqueued
        // to it and its OnDequeue was empty. Attacks are processed inline by HandlerProcess.

        public static void Process(Client.GameClient user, InteractQuery Attack)
        {

            using (var rec = new ServerSockets.RecycledPacket())
            {

                var stream = rec.GetStream();

                if (user.Player.Map == 2068 || (user.IsWatching() && user.Player.Map == 700 && user.InQualifier() == false))
                {
                    user.SendSysMesage("Spells are not allowed on this area.");

                    return;
                }
                if (user != null && Database.ItemType.IsShield(user.Equipment.LeftWeapon) && !(user.Player.Class >= 21 && user.Player.Class <= 25))
                {
                    if (user.Inventory.HaveSpace(1))
                    {
                        user.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                        user.Equipment.QueryEquipment(user.Equipment.Alternante);
                    }
                    else
                        user.SendSysMesage("Remove the shield or free 1 inventory spot.");
                    return;
                }

                if (user != null && Database.ItemType.IsBacksword(user.Equipment.LeftWeapon))
                {
                    if (user.Inventory.HaveSpace(1))
                    {
                        user.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                        user.Equipment.QueryEquipment(user.Equipment.Alternante);
                    }
                    else
                        user.SendSysMesage("Remove the shield or free 1 inventory spot.");
                    return;
                }
                Role.IMapObj target;
                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                {
                    Role.Player attacked = target as Role.Player;
                    if (attacked.Map == 1002 && attacked.UID != user.Player.UID && user.Player.PkMode != Role.Flags.PKMode.Jiang)
                        return;
                }
                if (!user.Player.Alive)
                {

                    return;
                }
                if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dizzy))
                {
                    return;
                }
                if (!ProcsAfterSpeedCheck && user.Player.Map != 1039)//training (original order)
                    AttackHandler.CheckAttack.CheckItems.AttackDurability(user, stream);

                if (user.Player.ContainFlag(MsgUpdate.Flags.Freeze))
                {

                    return;
                }
                // [Gameplay] ProcsAfterSpeedCheck=1: item wear is rolled once per accepted packet: Physical/Archer
                // roll it after their attack-speed check and client spells after their cooldown check (ProcescMagic),
                // so rejected packets don't wear items; every other packet type rolls it here as before.
                if (ProcsAfterSpeedCheck && user.Player.Map != 1039//training
                    && Attack.AtkType != AttackID.Physical && Attack.AtkType != AttackID.Archer && Attack.AtkType != AttackID.Magic)
                    AttackHandler.CheckAttack.CheckItems.AttackDurability(user, stream);
                switch (Attack.AtkType)
                {
                    case AttackID.Obtained:
                        {
                            if (user.Player.ChargCps > 0)
                            {
                                user.Player.ConquerPoints += user.Player.ChargCps;
                                Attack.ResponseDamage = user.Player.ChargCps;
                                user.Send(stream.InteractionCreate(&Attack));
                                user.Player.ChargCps = 0;
                                user.Player.MarkFirstCredit(stream);
                            }
                            break;
                        }
                    case AttackID.UpdateHunterJar:
                        {
                            MsgGameItem Jar;
                            if (user.Inventory.TryGetItem(user.DemonExterminator.ItemUID, out Jar))
                            {
                                Attack.UID = Attack.OpponentUID = user.Player.UID;
                                Attack.X = Jar.MaximDurability;
                                Attack.Y = 0;
                                Attack.ResponseDamage = user.DemonExterminator.HuntKills;

                                user.Send(stream.InteractionCreate(&Attack));
                            }
                            break;
                        }
                    case AttackID.InteractionRequest:
                        {
                            Role.IMapObj Target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out Target, Role.MapObjectType.Player))
                            {
                                Role.Player Opponent = Target as Role.Player;
                                if (user.Player.ObjInteraction == null && Opponent.ObjInteraction == null)
                                {
                                    AddPendingRequest(PendingInteractions, user.Player.UID, Opponent.UID);
                                    Opponent.ActiveDance = user.Player.ActiveDance = (ushort)Attack.ResponseDamage;
                                    Opponent.Owner.Send(stream.InteractionCreate(&Attack));
                                    Opponent.Owner.Send(stream.InteractionCreate(&Attack));
                                }
                            }
                            break;
                        }
                    case AttackID.InteractionRefuse:
                        {
                            user.Player.ActiveDance = 0;
                            Role.IMapObj Target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out Target, Role.MapObjectType.Player))
                            {
                                Role.Player Opponent = Target as Role.Player;
                                TakePendingRequest(PendingInteractions, Opponent.UID, user.Player.UID);
                                Opponent.ActiveDance = 0;
                                Opponent.Owner.Send(stream.InteractionCreate(&Attack));
                            }
                            break;
                        }
                    case AttackID.InteractionAccept:
                        {
                            Role.IMapObj Target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out Target, Role.MapObjectType.Player))
                            {
                                Role.Player Opponent = Target as Role.Player;
                                if (user.Player.ObjInteraction == null && Opponent.ObjInteraction == null)
                                {
                                    if (!IsAllowedInteractionAction((uint)Attack.Damage)
                                        || !TakePendingRequest(PendingInteractions, Opponent.UID, user.Player.UID))
                                    {
                                        Conquer6609.Security.Guard.Audit("interaction-accept-reject", user.Player.UID, user.Player.Name,
                                            "opponent=" + Opponent.UID + " action=" + Attack.Damage);
                                        break;
                                    }
                                    Attack.ResponseDamage = user.Player.ActiveDance;
                                    Opponent.Owner.Send(stream.InteractionCreate(&Attack));

                                    Attack.TimeStamp = 0;
                                    user.Send(stream.InteractionCreate(&Attack));

                                    user.Player.Action = (Role.Flags.ConquerAction)Attack.Damage;
                                    Opponent.Action = (Role.Flags.ConquerAction)Attack.Damage;

                                    user.Player.ObjInteraction = Opponent.Owner;
                                    Opponent.ObjInteraction = user;

                                }
                            }
                            break;
                        }
                    case AttackID.InteractionEffect:
                        {
                            if (user.Player.ObjInteraction != null)
                            {
                                if (user.Player.ObjInteraction.Player.ObjInteraction != null)
                                {
                                    Attack.ResponseDamage = user.Player.ActiveDance;
                                    Attack.TimeStamp = 0;
                                    CreateInteractionEffect(Attack, user);

                                    InteractQuery user_effect = user.Player.InteractionEffect;

                                    user.Player.View.SendView(stream.InteractionCreate(&user_effect), true);

                                    Attack.UID = user.Player.ObjInteraction.Player.UID;
                                    Attack.OpponentUID = user.Player.UID;

                                    CreateInteractionEffect(Attack, user.Player.ObjInteraction);

                                    user_effect = user.Player.ObjInteraction.Player.InteractionEffect;
                                    user.Player.ObjInteraction.Player.View.SendView(stream.InteractionCreate(&user_effect), true);
                                }
                            }
                            break;
                        }
                    case AttackID.InteractionStopEffect:
                        {
                            Attack.ResponseDamage = user.Player.ActiveDance;
                            user.Player.View.SendView(stream.InteractionCreate(&Attack), true);

                            Attack.UID = Attack.OpponentUID;
                            Attack.OpponentUID = user.Player.UID;
                            user.Player.View.SendView(stream.InteractionCreate(&Attack), true);

                            if (user.Player.ObjInteraction != null)
                            {
                                user.Player.OnInteractionEffect = false;
                                user.Player.Action = Role.Flags.ConquerAction.None;
                                user.Player.ObjInteraction.Player.OnInteractionEffect = false;
                                user.Player.ObjInteraction.Player.Action = Role.Flags.ConquerAction.None;
                                user.Player.ObjInteraction.Player.ObjInteraction = null;
                                user.Player.ObjInteraction = null;
                            }
                            break;
                        }
                    case AttackID.RequestMarriage:
                        {
                            Role.IMapObj Target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out Target, Role.MapObjectType.Player))
                            {
                                Role.Player Opponent = Target as Role.Player;
                                if (user.Player.Spouse != "None" || Opponent.Spouse != "None")
                                {
                                    user.SendSysMesage("You cannot marry someone that is already married with someone else!");

                                    break;
                                }
                                if (user.Player.Body % 10 <= 2 && Opponent.Body % 10 >= 3 || user.Player.Body % 10 >= 2 && Opponent.Body % 10 <= 3)
                                {
                                    Opponent.Send(stream.PopupInfoCreate(user.Player.UID, Opponent.UID, user.Player.Level, user.Player.BattlePower));

                                    Attack.X = Opponent.X;
                                    Attack.Y = Opponent.Y;

                                    AddPendingRequest(PendingMarriages, user.Player.UID, Opponent.UID);
                                    Opponent.Send(stream.InteractionCreate(&Attack));

                                }
                                else
                                {
                                    user.SendSysMesage("You cannot marry someone of your gender!");

                                }
                            }
                            break;
                        }
                    case AttackID.AcceptMarriage:
                        {
                            Role.IMapObj Target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out Target, Role.MapObjectType.Player))
                            {
                                Role.Player Opponent = Target as Role.Player;
                                if (user.Player.Spouse != "None" || Opponent.Spouse != "None")
                                {
                                    user.SendSysMesage("You cannot marry someone that is already married with someone else!");

                                    break;
                                }
                                if (!TakePendingRequest(PendingMarriages, Opponent.UID, user.Player.UID))
                                {
                                    Conquer6609.Security.Guard.Audit("marriage-accept-reject", user.Player.UID, user.Player.Name, "no pending request from " + Opponent.UID);
                                    break;
                                }
                                if (user.Player.Body % 10 <= 2 && Opponent.Body % 10 >= 3 || user.Player.Body % 10 >= 2 && Opponent.Body % 10 <= 3)
                                {
                                    user.Player.Spouse = Opponent.Name;
                                    user.Player.SpouseUID = Opponent.UID;

                                    Opponent.Spouse = user.Player.Name;
                                    Opponent.SpouseUID = user.Player.UID;

                                    MsgMessage messaj = new MsgMessage("Joy and happiness! " + user.Player.Name + " and " + Opponent.Name + " have joined together in the holy marriage. We wish them a stone house.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

                                    Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));

                                    user.Player.SendString(stream, MsgStringPacket.StringID.Spouse, false, new string[1] { user.Player.Spouse });
                                    Opponent.SendString(stream, MsgStringPacket.StringID.Spouse, false, new string[1] { Opponent.Spouse });
                                    user.Player.SendString(stream, MsgStringPacket.StringID.Fireworks, true, new string[1] { "1122" });
                                    //firework-2love
                                    user.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[1] { "firework-2love" });
                                }
                                else
                                {
                                    user.SendSysMesage("You cannot marry someone of your gender!");

                                }
                            }
                            break;
                        }
                    case AttackID.Physical:
                        {

                            Extensions.Time32 Timer = Extensions.Time32.Now;
                            if (Timer < user.Player.AttackStamp.AddMilliseconds(user.Equipment.AttackSpeed(true)))
                            {
                                return;
                            }
                            user.Player.AttackStamp = Timer;
                            if (ProcsAfterSpeedCheck && user.Player.Map != 1039)//training
                                AttackHandler.CheckAttack.CheckItems.AttackDurability(user, stream);
                            AttackHandler.Updates.GetWeaponSpell.CheckExtraEffects(user, stream);
                            MsgServer.AttackHandler.CheckAttack.CheckGemEffects.TryngEffect(user);
                            if (user.MySpells.ClientSpells.ContainsKey(Attack.SpellID))
                            {

                                InteractQuery AttackPaket = new InteractQuery();
                                Role.IMapObj _target;

                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)|| user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)|| user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                                {
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, _target.X, _target.Y) <= 3)
                                    {
                                        AttackPaket.X = _target.X;
                                        AttackPaket.Y = _target.Y;
                                        if (user.OnAutoAttack)
                                        {
                                            AttackPaket.SpellID = Attack.SpellID;
                                            user.Player.RandomSpell = AttackPaket.SpellID;
                                            AttackPaket.OpponentUID = Attack.OpponentUID;
                                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;

                                            MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket);
                                            if (_target.Alive)
                                            {

                                                CreateAutoAtack(Attack, user);
                                            }
                                            else
                                                user.OnAutoAttack = false;
                                            break;
                                        }

                                        if (!AttackHandler.Updates.GetWeaponSpell.Check(Attack, stream, user, _target))
                                        {
                                            AttackPaket.UID = Attack.UID;
                                            AttackPaket.SpellID = Attack.SpellID;
                                            user.Player.RandomSpell = Attack.SpellID;

                                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                                            MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket);
                                            if (_target.Alive)
                                            {
                                                CreateAutoAtack(Attack, user);
                                            }
                                            else
                                                user.OnAutoAttack = false;

                                        }
                                    }
                                    else
                                        user.OnAutoAttack = false;
                                    break;
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                if (target.Map == 1002 && user.Player.PkMode != Role.Flags.PKMode.Jiang) return;
                                if (!AttackHandler.Updates.GetWeaponSpell.Check(Attack, stream, user, target))
                                {
                                    if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 3)
                                    {
                                        Role.Player attacked = target as Role.Player;
                                        if (AttackHandler.CheckAttack.CanAttackPlayer.Verified(user, attacked, null))
                                        {
                                            Attack.TimeStamp = 0;
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            AttackHandler.Calculate.Physical.OnPlayer(user.Player, attacked, null, out AnimationObj);
                                            Attack.Damage = (int)AnimationObj.Damage;
                                            Attack.Effect = AnimationObj.Effect;
                                            user.Player.View.SendView(stream.InteractionCreate(&Attack), true);
                                            Attack.AtkType = AttackID.Physical;
                                            AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                            if (attacked.Alive)
                                                CreateAutoAtack(Attack, user);
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                    }
                                    else
                                        user.OnAutoAttack = false;
                                }
                            }
                            else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                if (!AttackHandler.Updates.GetWeaponSpell.Check(Attack, stream, user, target))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= user.Equipment.GetAttackRange(attacked.SizeAdd) || user.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                                    {

                                        if (AttackHandler.CheckAttack.CanAttackMonster.Verified(user, attacked, null))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;

                                            Attack.TimeStamp = 0;

                                            if (user.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                                            {
                                                Attack.AtkType = AttackID.FatalStrike;
                                                user.Shift(target.X, target.Y, stream);
                                                AttackHandler.Calculate.Physical.OnMonster(user.Player, attacked, Database.Server.Magic[(ushort)Role.Flags.SpellID.FatalStrike][0], out AnimationObj);
                                            }
                                            else
                                                AttackHandler.Calculate.Physical.OnMonster(user.Player, attacked, null, out AnimationObj);

                                            Attack.Damage = (int)AnimationObj.Damage;
                                            Attack.Effect = AnimationObj.Effect;

                                            user.Player.View.SendView(stream.InteractionCreate(&Attack), true);

                                            Attack.AtkType = AttackID.Physical;
                                            AttackHandler.Updates.IncreaseExperience.Up(stream, user, AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked));
                                            AttackHandler.Updates.UpdateSpell.CheckUpdate(stream, user, Attack, AnimationObj.Damage, null);
                                            if (attacked.Alive)
                                                CreateAutoAtack(Attack, user);
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                    }
                                    else
                                        user.OnAutoAttack = false;
                                }
                            }
                            else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {

                                if (!AttackHandler.Updates.GetWeaponSpell.Check(Attack, stream, user, target))
                                {
                                    if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 3)
                                    {
                                        var attacked = target as Role.SobNpc;
                                        if (AttackHandler.CheckAttack.CanAttackNpc.Verified(user, attacked, null))
                                        {
                                            Attack.TimeStamp = 0;

                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            AttackHandler.Calculate.Physical.OnNpcs(user.Player, attacked, null, out AnimationObj);

                                            Attack.Damage = (int)AnimationObj.Damage;
                                            Attack.Effect = AnimationObj.Effect;
                                            user.Player.View.SendView(stream.InteractionCreate(&Attack), true);


                                            Attack.AtkType = AttackID.Physical;

                                            AttackHandler.Updates.IncreaseExperience.Up(stream, user, AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked));
                                            AttackHandler.Updates.UpdateSpell.CheckUpdate(stream, user, Attack, AnimationObj.Damage, null);
                                            if (attacked.Alive)
                                                CreateAutoAtack(Attack, user);
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                    }
                                    else
                                        user.OnAutoAttack = false;
                                }

                            }
                            else
                                user.OnAutoAttack = false;
                            break;
                        }
                    case AttackID.Archer:
                        {

                            if (!AttackHandler.CheckAttack.CheckLineSpells.CheckUp(user, Attack.SpellID))
                                break;
                            if (!ProcsAfterSpeedCheck)
                                AttackHandler.Updates.GetWeaponSpell.CheckExtraEffects(user, stream);

                            Extensions.Time32 Timer = Extensions.Time32.Now;
                            if (Timer < user.Player.AttackStamp.AddMilliseconds(user.Equipment.AttackSpeed(false)))
                                return;
                            user.Player.AttackStamp = Timer;
                            if (ProcsAfterSpeedCheck)
                            {
                                if (user.Player.Map != 1039)//training
                                    AttackHandler.CheckAttack.CheckItems.AttackDurability(user, stream);
                                AttackHandler.Updates.GetWeaponSpell.CheckExtraEffects(user, stream);
                            }


                            MsgServer.AttackHandler.CheckAttack.CheckGemEffects.TryngEffect(user);

                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {

                                if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 18)
                                {
                                    Role.Player attacked = target as Role.Player;
                                    if (AttackHandler.CheckAttack.CanAttackPlayer.Verified(user, attacked, null))
                                    {
                                        if (user.Player.ContainFlag(MsgUpdate.Flags.KineticSpark))
                                        {
                                            if (AttackHandler.Calculate.Base.Rate(30))
                                            {
                                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                                if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.KineticSpark, out Spells))
                                                {
                                                    Attack.SpellID = (ushort)Role.Flags.SpellID.KineticSpark;
                                                    Database.MagicType.Magic spell;
                                                    if (Spells.TryGetValue(Attack.SpellLevel, out spell))
                                                    {
                                                        AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (AttackHandler.Calculate.Base.Rate(10))
                                        {
                                            MsgSpell clientspell;
                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FreezingArrow, out clientspell))
                                            {
                                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                                if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.FreezingArrow, out Spells))
                                                {
                                                    Database.MagicType.Magic spell;
                                                    if (Spells.TryGetValue(clientspell.Level, out spell))
                                                    {

                                                        Attack.SpellID = (ushort)Role.Flags.SpellID.FreezingArrow;
                                                        AttackHandler.AttackStatus.Execute(user, Attack, stream, Spells);
                                                        break;


                                                    }
                                                }
                                            }
                                        }
                                        else if (user.Player.ContainFlag(MsgUpdate.Flags.ShadowofChaser))
                                        {
                                            MsgSpell clientspell;
                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out clientspell))
                                            {
                                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                                if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out Spells))
                                                {
                                                    Database.MagicType.Magic spell;
                                                    if (Spells.TryGetValue(clientspell.Level, out spell))
                                                    {
                                                        if (AttackHandler.Calculate.Base.Rate(spell.Rate))
                                                        {
                                                            Attack.SpellID = (ushort)Role.Flags.SpellID.ShadowofChaser;
                                                            AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                            break;
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                        else if (AttackHandler.Calculate.Base.Rate(5))
                                        {
                                            MsgSpell clientspell;
                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.PoisonousArrows, out clientspell))
                                            {
                                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                                if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.PoisonousArrows, out Spells))
                                                {
                                                    Database.MagicType.Magic spell;
                                                    if (Spells.TryGetValue(clientspell.Level, out spell))
                                                    {

                                                        Attack.SpellID = (ushort)Role.Flags.SpellID.PoisonousArrows;
                                                        AttackHandler.AttackStatus.Execute(user, Attack, stream, Spells);
                                                        break;


                                                    }
                                                }
                                            }
                                        }
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        AttackHandler.Calculate.Range.OnPlayer(user.Player, attacked, null, out AnimationObj);

                                        Attack.Damage = (int)AnimationObj.Damage;
                                        Attack.Effect = AnimationObj.Effect;
                                        user.Player.View.SendView(stream.InteractionCreate(&Attack), true);


                                        Attack.AtkType = AttackID.Archer;
                                        CreateAutoAtack(Attack, user);

                                        AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                    }
                                    else
                                        user.OnAutoAttack = false;
                                }
                                else
                                    user.OnAutoAttack = false;

                            }
                            else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {

                                if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 18)
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (AttackHandler.CheckAttack.CanAttackMonster.Verified(user, attacked, null))
                                    {
                                        if (user.Player.ContainFlag(MsgUpdate.Flags.KineticSpark))
                                        {
                                            if (AttackHandler.Calculate.Base.Rate(30))
                                            {
                                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                                if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.KineticSpark, out Spells))
                                                {
                                                    Attack.SpellID = (ushort)Role.Flags.SpellID.KineticSpark;
                                                    Database.MagicType.Magic spell;
                                                    if (Spells.TryGetValue(Attack.SpellLevel, out spell))
                                                    {
                                                        AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (AttackHandler.Calculate.Base.Rate(5))
                                        {
                                            MsgSpell clientspell;
                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FreezingArrow, out clientspell))
                                            {
                                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                                if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.FreezingArrow, out Spells))
                                                {
                                                    Database.MagicType.Magic spell;
                                                    if (Spells.TryGetValue(clientspell.Level, out spell))
                                                    {

                                                        Attack.SpellID = (ushort)Role.Flags.SpellID.FreezingArrow;
                                                        AttackHandler.AttackStatus.Execute(user, Attack, stream, Spells);
                                                        break;


                                                    }
                                                }
                                            }
                                        }
                                        else if (user.Player.ContainFlag(MsgUpdate.Flags.ShadowofChaser))
                                        {
                                            MsgSpell clientspell;
                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out clientspell))
                                            {
                                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                                if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out Spells))
                                                {
                                                    Database.MagicType.Magic spell;
                                                    if (Spells.TryGetValue(clientspell.Level, out spell))
                                                    {
                                                        if (AttackHandler.Calculate.Base.Rate(spell.Rate))
                                                        {
                                                            Attack.SpellID = (ushort)Role.Flags.SpellID.ShadowofChaser;
                                                            AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                            break;
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                        else if (AttackHandler.Calculate.Base.Rate(5))
                                        {
                                            MsgSpell clientspell;
                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.PoisonousArrows, out clientspell))
                                            {
                                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                                if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.PoisonousArrows, out Spells))
                                                {
                                                    Database.MagicType.Magic spell;
                                                    if (Spells.TryGetValue(clientspell.Level, out spell))
                                                    {

                                                        Attack.SpellID = (ushort)Role.Flags.SpellID.PoisonousArrows;
                                                        AttackHandler.AttackStatus.Execute(user, Attack, stream, Spells);
                                                        break;


                                                    }
                                                }
                                            }
                                        }
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        AttackHandler.Calculate.Range.OnMonster(user.Player, attacked, null, out AnimationObj);



                                        Attack.Damage = (int)AnimationObj.Damage;
                                        Attack.Effect = AnimationObj.Effect;
                                        user.Player.View.SendView(stream.InteractionCreate(&Attack), true);

                                        Attack.AtkType = AttackID.Archer;
                                        AttackHandler.Updates.IncreaseExperience.Up(stream, user, AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked));
                                        AttackHandler.Updates.UpdateSpell.CheckUpdate(stream, user, Attack, AnimationObj.Damage, null);
                                        CreateAutoAtack(Attack, user);
                                        AttackHandler.Updates.UpdateSpell.CheckUpdate(stream, user, Attack, AnimationObj.Damage, null);
                                    }
                                    else
                                        user.OnAutoAttack = false;
                                }
                                else
                                    user.OnAutoAttack = false;

                            }
                            else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {

                                if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 18)
                                {
                                    var attacked = target as Role.SobNpc;
                                    if (AttackHandler.CheckAttack.CanAttackNpc.Verified(user, attacked, null))
                                    {
                                        if (user.Player.ContainFlag(MsgUpdate.Flags.KineticSpark))
                                        {
                                            if (AttackHandler.Calculate.Base.Rate(30))
                                            {
                                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                                if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.KineticSpark, out Spells))
                                                {
                                                    Attack.SpellID = (ushort)Role.Flags.SpellID.KineticSpark;
                                                    Database.MagicType.Magic spell;
                                                    if (Spells.TryGetValue(Attack.SpellLevel, out spell))
                                                    {
                                                        AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else if (user.Player.ContainFlag(MsgUpdate.Flags.ShadowofChaser))
                                        {
                                            MsgSpell clientspell;
                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out clientspell))
                                            {
                                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                                if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out Spells))
                                                {
                                                    Database.MagicType.Magic spell;
                                                    if (Spells.TryGetValue(clientspell.Level, out spell))
                                                    {
                                                        if (AttackHandler.Calculate.Base.Rate(spell.Rate))
                                                        {
                                                            Attack.SpellID = (ushort)Role.Flags.SpellID.ShadowofChaser;
                                                            AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                            break;
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        AttackHandler.Calculate.Range.OnNpcs(user.Player, attacked, null, out AnimationObj);

                                        Attack.Damage = (int)AnimationObj.Damage;
                                        Attack.Effect = AnimationObj.Effect;

                                        user.Player.View.SendView(stream.InteractionCreate(&Attack), true);


                                        Attack.AtkType = AttackID.Archer;
                                        CreateAutoAtack(Attack, user);
                                        AttackHandler.Updates.IncreaseExperience.Up(stream, user, AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked));

                                        AttackHandler.Updates.UpdateSpell.CheckUpdate(stream, user, Attack, AnimationObj.Damage, null);

                                    }
                                    else
                                        user.OnAutoAttack = false;
                                }
                                else
                                    user.OnAutoAttack = false;

                            }
                            else
                                user.OnAutoAttack = false;

                            break;
                        }
                    case AttackID.CounterKillSwitch:
                        {

                            if (!AttackHandler.CheckAttack.CheckLineSpells.CheckUp(user, Attack.SpellID))
                                break;
                            Dictionary<ushort, Database.MagicType.Magic> Spells;
                            if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.CounterKill, out Spells))
                            {
                                MsgSpell ClientSpell;
                                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.CounterKill, out ClientSpell))
                                {
                                    Database.MagicType.Magic spell;
                                    if (Spells.TryGetValue(ClientSpell.Level, out spell))
                                    {
                                        switch (spell.Type)
                                        {
                                            case Database.MagicType.MagicSort.CounterKill:
                                                {
                                                    Database.MagicType.Magic DBSpell;

                                                    Attack.SpellID = (ushort)Role.Flags.SpellID.CounterKill;
                                                    Attack.SpellLevel = ClientSpell.Level;

                                                    if (AttackHandler.CheckAttack.CanUseSpell.Verified(Attack, user, Spells, out ClientSpell, out DBSpell))
                                                    {
                                                        user.Player.ActivateCounterKill = !user.Player.ActivateCounterKill;
                                                        Attack.OnCounterKill = user.Player.ActivateCounterKill;
                                                        user.Send(stream.InteractionCreate(&Attack));
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case AttackID.Magic:
                        {

                            ProcescMagic(user, stream, Attack, false, true);


                            break;
                        }

                }

            }


        }
        public static int FastLineSkillIntervalMs = 300;
        // [Gameplay] ProcsAfterSpeedCheck (default 0 = original order): 1 rolls item wear, weapon-skill extra
        // effects and gem effects only after the attack-speed / cooldown check accepted the packet.
        public static readonly bool ProcsAfterSpeedCheck = Conquer6609.Framework.Settings.Int("Gameplay", "ProcsAfterSpeedCheck", 0) != 0;
        public static void ProcescMagic(Client.GameClient user, ServerSockets.Packet stream, InteractQuery Attack, bool ignoreStamp = false, bool fromClient = false)
        {

            // [Gameplay] ProcsAfterSpeedCheck=1: server-originated casts (weapon procs, physical skill paths) keep
            // rolling the gem effect here as before; a spell packet from the client rolls it only once it passed
            // the cooldown check below. Default 0 = original: always rolled here.
            if (!ProcsAfterSpeedCheck || !fromClient)
                MsgServer.AttackHandler.CheckAttack.CheckGemEffects.TryngEffect(user);


            if (!user.AllowUseSpellOnSteed(Attack.SpellID))
            {
                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                return;
            }
            if (!AttackHandler.CheckAttack.CheckLineSpells.CheckUp(user, Attack.SpellID))
            {

                return;
            }
            Role.IMapObj target;
            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player) && user.Player.PkMode != Role.Flags.PKMode.Jiang && Attack.SpellID != 30000)
            {
                if (user.Player.UID != target.UID)
                    if (target.Map == 1002) return;
            }
            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc) || user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Npc))
            {
                if (target.Map == 1002 || target.Map == 1036) return;
            }
            bool OnTGAutoAttack = true;
            Dictionary<ushort, Database.MagicType.Magic> Spells;
            if (Database.Server.Magic.TryGetValue(Attack.SpellID, out Spells))
            {

                // Spell rule (client-requested casts only): the caster must own the spell and its
                // level comes from the server-side spell book, never from the packet.
                // Server-originated casts (weapon procs such as Poison/EffectMP, auto-attack weapon
                // skills, the ShurikenVortex pulse) are built by the server and keep their level.
                if (fromClient)
                {
                    if (Attack.SpellID == (ushort)Role.Flags.SpellID.ShurikenEffect)
                    {
                        // ShurikenEffect is the server-driven pulse of ShurikenVortex; ShurikenVortex.cs
                        // only knows level 0 of it, so the level is pinned to 0.
                        if (!user.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex)
                            || !user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShurikenVortex))
                            return;
                        Attack.SpellLevel = 0;
                    }
                    else
                    {
                        MsgSpell ownSpell;
                        if (!user.MySpells.ClientSpells.TryGetValue(Attack.SpellID, out ownSpell))
                            return;
                        Attack.SpellLevel = ownSpell.Level;
                    }
                }
                Database.MagicType.Magic spell;
                if (Spells.TryGetValue(Attack.SpellLevel, out spell))
                {
                    Extensions.Time32 Timer = Extensions.Time32.Now;
                    // Cooldowns are computed locally; the shared spell DB entry is never modified.
                    int coolDown = spell.CoolDown;
                    if (coolDown == 0)
                        coolDown = 500;
                    if (ignoreStamp == false)
                    {
                        if (coolDown > 1000 && coolDown < 3000)
                            coolDown = 800;
                        if (spell.ID == (ushort)Role.Flags.SpellID.MortalWound)
                        {
                            if (Timer < user.Player.AttackStamp.AddMilliseconds(800))
                            {
                                return;
                            }
                        }
                        else if (spell.ID == (ushort)Role.Flags.SpellID.Thunder)
                        {
                            if (Timer < user.Player.AttackStamp.AddMilliseconds(800))
                            {
                                return;
                            }
                        }
                        else if (spell.ID == (ushort)Role.Flags.SpellID.Tornado)
                        {
                            if (Timer < user.Player.AttackStamp.AddMilliseconds(500))
                            {
                                return;
                            }
                        }
                        else if (spell.ID == (ushort)Role.Flags.SpellID.RadiantPalm)
                        {
                            if (Timer < user.Player.AttackStamp.AddMilliseconds(500))
                            {
                                return;
                            }
                        }
                        else if (spell.ID == (ushort)Role.Flags.SpellID.BladeFlurry)
                        {
                            if (Timer < user.Player.AttackStamp.AddMilliseconds(1000))
                            {
                                return;
                            }
                        }
                        else if (spell.ID == (ushort)Role.Flags.SpellID.RadiantPalm)
                        {
                            if (Timer < user.Player.AttackStamp.AddMilliseconds(user.Equipment.AttackSpeed(false)))
                            {
                                return;
                            }
                        }
                        else if (spell.ID == (ushort)Role.Flags.SpellID.FastBlader || spell.ID == (ushort)Role.Flags.SpellID.ScrenSword
                            || spell.ID == (ushort)Role.Flags.SpellID.ViperFang)
                        {
                            // Rapid line skills: minimum interval instead of the DB cooldown.
                            if (Timer < user.Player.AttackStamp.AddMilliseconds(FastLineSkillIntervalMs))
                            {
                                return;
                            }
                        }
                        else if (Timer < user.Player.AttackStamp.AddMilliseconds(coolDown))
                        {

                            return;
                        }

                        user.Player.AttackStamp = Timer;
                    }
                    if (ProcsAfterSpeedCheck && fromClient)
                    {
                        if (user.Player.Map != 1039)//training
                            AttackHandler.CheckAttack.CheckItems.AttackDurability(user, stream);
                        MsgServer.AttackHandler.CheckAttack.CheckGemEffects.TryngEffect(user);
                    }


                    if (user.OnAutoAttack == false)
                    {
                        if (user.Equipment.RightWeaponEffect == Role.Flags.ItemEffect.MP)
                        {
                            {
                                if (Attack.SpellID == (ushort)Role.Flags.SpellID.EffectMP || AttackHandler.Calculate.Base.Rate(50))
                                {
                                    user.Player.RandomSpell = 1175;
                                    AttackHandler.EffectMP.Execute(Attack, user, stream, Spells);

                                }
                            }

                        }
                        AttackHandler.Updates.GetWeaponSpell.CheckExtraEffects(user, stream);


                    }

                    switch (spell.Type)
                    {
                        case Database.MagicType.MagicSort.PetAttachStatus: AttackHandler.PetAttachStatus.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.RemoveStamin: AttackHandler.RemoveStamin.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.SwirlingStorm: AttackHandler.SwirlingStorm.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Omnipotence: AttackHandler.Omnipotence.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.BurntFrost: AttackHandler.BurntFrost.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Rectangle: AttackHandler.Rectangle.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.AddMana: AttackHandler.AddMana.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Strike: AttackHandler.Strike.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.AirKick: AttackHandler.AirKick.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.DragonCyclone: AttackHandler.DragonCyclone.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.FatalCross: AttackHandler.FatalCross.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.SectorBack: AttackHandler.SectorBack.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.BladeFlurry: AttackHandler.BladeFlurry.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.KineticSpark: AttackHandler.KineticSpark.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.MortalDrag: AttackHandler.MortalDrag.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.ChargingVortex: AttackHandler.ChargingVortex.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.PirateXpSkill: AttackHandler.PirateXpSkill.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.AddBlackSpot: AttackHandler.AddBlackSpot.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.MoveLine: AttackHandler.MoveLine.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.BombLine: AttackHandler.BombLine.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.BlackSpot: AttackHandler.BlackSpot.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.CannonBarrage: AttackHandler.CannonBarrage.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.ScurvyBomb: AttackHandler.ScurvyBomb.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.PhysicalSpells: AttackHandler.PhysicalSpells.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.WhirlwindKick: AttackHandler.WhirlwindKick.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Oblivion: AttackHandler.Oblivion.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.DispatchXp: AttackHandler.DispatchXp.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.ShieldBlock: AttackHandler.ShieldBlock.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Compasion:
                        case Database.MagicType.MagicSort.Tranquility:
                        case Database.MagicType.MagicSort.RemoveBuffers:
                            {

                                AttackHandler.RemoveBuffers.Execute(user, Attack, stream, Spells);
                                break;
                            }
                        case Database.MagicType.MagicSort.Perimeter: AttackHandler.Perimeter.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Auras: AttackHandler.Auras.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.DirectAttack: AttackHandler.DirectAttack.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.DragonWhirl: AttackHandler.DragonWhirl.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.StarArrow: AttackHandler.StarArrow.ExecuteExecute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.ChainBolt: AttackHandler.ChainBolt.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Spook: AttackHandler.Spook.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.WarCry: AttackHandler.WarCry.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Riding: AttackHandler.Riding.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.ShurikenVortex: AttackHandler.ShurikenVortex.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Toxic: AttackHandler.Toxic.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.DecLife: AttackHandler.DecLife.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.LayTrap: AttackHandler.LayTrap.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Transform: AttackHandler.Transform.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.AttackStatus: AttackHandler.AttackStatus.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Collide: AttackHandler.Collide.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Sector: AttackHandler.Sector.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Line: AttackHandler.Line.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Attack: AttackHandler.Attack.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.AttachStatus:
                            {
                                OnTGAutoAttack = false;
                                AttackHandler.AttachStatus.Execute(user, Attack, stream, Spells); break;
                            }
                        case Database.MagicType.MagicSort.DetachStatus: AttackHandler.DetachStatus.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Recruit: AttackHandler.Recruit.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Bomb: AttackHandler.Bomb.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.FatalSpin: AttackHandler.FatalSpin.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.CallPet: AttackHandler.CallPet.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.SectorPasive: AttackHandler.SectorPasive.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.StraightFist: AttackHandler.StraightFist.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.ManiacDance: AttackHandler.ManiacDance.Execute(user, Attack, stream, Spells); break;
                        case Database.MagicType.MagicSort.Pounce: AttackHandler.Pounce.Execute(user, Attack, stream, Spells); break;
                    }

                    if (Database.AtributesStatus.IsTrojan(user.Player.Class))
                    {
                        if (Database.ItemType.IsTrojanEpicWeapon(user.Equipment.RightWeapon))
                        {
                            if (AttackHandler.Calculate.Base.Rate(35))
                            {
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                                {
                                    AttackHandler.BreathFocus.Execute(Attack, user, stream, Spells);
                                }
                            }
                        }
                    }

                }
            }
            if (user.Player.Map == 1039 && OnTGAutoAttack)
            {
                //EncodeMagicAttack(&Attack);
                CreateAutoAtack(Attack, user);
            }
        }

        public static void CreateAutoAtack(InteractQuery pQuery, Client.GameClient client)
        {
            client.OnAutoAttack = true;

            client.AutoAttack = new InteractQuery();
            client.AutoAttack.AtkType = pQuery.AtkType;
            client.AutoAttack.Damage = pQuery.Damage;
            client.AutoAttack.Data = pQuery.Data;
            client.AutoAttack.dwParam = pQuery.dwParam;
            client.AutoAttack.Effect = pQuery.Effect;
            client.AutoAttack.OpponentUID = pQuery.OpponentUID;
            client.AutoAttack.PacketStamp = pQuery.PacketStamp;
            client.AutoAttack.ResponseDamage = pQuery.ResponseDamage;
            client.AutoAttack.SpellID = pQuery.SpellID;
            client.AutoAttack.SpellLevel = pQuery.SpellLevel;
            client.AutoAttack.UID = pQuery.UID;
            client.AutoAttack.X = pQuery.X;
            client.AutoAttack.Y = pQuery.Y;
        }

        // Pending request rule: an interaction / marriage accept is only valid if the opponent
        // sent the matching request to this player within PendingRequestTimeoutSec.
        public static int PendingRequestTimeoutSec = 60;
        static readonly System.Collections.Concurrent.ConcurrentDictionary<ulong, DateTime> PendingInteractions = new System.Collections.Concurrent.ConcurrentDictionary<ulong, DateTime>();
        static readonly System.Collections.Concurrent.ConcurrentDictionary<ulong, DateTime> PendingMarriages = new System.Collections.Concurrent.ConcurrentDictionary<ulong, DateTime>();

        static ulong RequestKey(uint from, uint to) { return ((ulong)from << 32) | to; }

        static void AddPendingRequest(System.Collections.Concurrent.ConcurrentDictionary<ulong, DateTime> pending, uint from, uint to)
        {
            if (pending.Count > 10000)
            {
                var now = DateTime.Now;
                foreach (var kv in pending)
                    if ((now - kv.Value).TotalSeconds > PendingRequestTimeoutSec)
                    {
                        DateTime old;
                        pending.TryRemove(kv.Key, out old);
                    }
            }
            pending[RequestKey(from, to)] = DateTime.Now;
        }

        /// <summary>Consumes a pending request from -> to; false when none exists or it expired.</summary>
        static bool TakePendingRequest(System.Collections.Concurrent.ConcurrentDictionary<ulong, DateTime> pending, uint from, uint to)
        {
            DateTime at;
            if (!pending.TryRemove(RequestKey(from, to), out at))
                return false;
            return (DateTime.Now - at).TotalSeconds <= PendingRequestTimeoutSec;
        }

        /// <summary>Interaction actions a client may set through InteractionAccept.</summary>
        static bool IsAllowedInteractionAction(uint action)
        {
            if (action >= 34460 && action <= 34499)//couple interactions (kiss, hold, hug, dances)
                return true;
            switch ((Role.Flags.ConquerAction)action)
            {
                case Role.Flags.ConquerAction.None:
                case Role.Flags.ConquerAction.Dance:
                case Role.Flags.ConquerAction.Cool:
                case Role.Flags.ConquerAction.Kneel:
                case Role.Flags.ConquerAction.Sad:
                case Role.Flags.ConquerAction.Happy:
                case Role.Flags.ConquerAction.Angry:
                case Role.Flags.ConquerAction.Lie:
                case Role.Flags.ConquerAction.Wave:
                case Role.Flags.ConquerAction.Bow:
                case Role.Flags.ConquerAction.Sit:
                    return true;
            }
            return false;
        }

        public static void CreateInteractionEffect(InteractQuery pQuery, Client.GameClient client)
        {
            client.Player.OnInteractionEffect = true;

            client.Player.InteractionEffect = new InteractQuery();
            client.Player.InteractionEffect.AtkType = pQuery.AtkType;
            client.Player.InteractionEffect.Damage = pQuery.Damage;
            client.Player.InteractionEffect.Data = pQuery.Data;
            client.Player.InteractionEffect.dwParam = pQuery.dwParam;
            client.Player.InteractionEffect.Effect = pQuery.Effect;
            client.Player.InteractionEffect.OpponentUID = pQuery.OpponentUID;
            client.Player.InteractionEffect.PacketStamp = pQuery.PacketStamp;
            client.Player.InteractionEffect.ResponseDamage = pQuery.ResponseDamage;
            client.Player.InteractionEffect.SpellID = pQuery.SpellID;
            client.Player.InteractionEffect.SpellLevel = pQuery.SpellLevel;
            client.Player.InteractionEffect.UID = pQuery.UID;
            client.Player.InteractionEffect.X = pQuery.X;
            client.Player.InteractionEffect.Y = pQuery.Y;
        }

    }
}
