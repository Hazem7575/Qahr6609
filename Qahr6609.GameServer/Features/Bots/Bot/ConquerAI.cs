using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using Conquer6609;
using Conquer6609.Client;
using System.Collections.Concurrent;
using Conquer6609.MsgInterServer;
using Conquer6609.Role;
using Conquer6609.Database;
using Conquer6609.Game.MsgServer;
using Conquer6609.ServerSockets;
using Org.BouncyCastle.Asn1.Cmp;

namespace Conquer6609.BotManager
{
    public enum BotType
    {
        MyAi,
        AI,
        MatrixAI
    }
    public enum BotLevel
    {
        None = 0,
        Easy = 1,
        Normal = 2,
        Hard = 3,
        Insane = 4,
        ProPlayer = 5,
        TheBest = 6,
    }
    public enum BotSkills
    {
        Fastblade = (ushort)Flags.SpellID.FastBlader,
        ScrenSword = (ushort)Flags.SpellID.ScrenSword,
        ViperFang = (ushort)Flags.SpellID.ViperFang,
        DragonTail = (ushort)Flags.SpellID.DragonTail,
    }
    public unsafe class ConquerAI
    {
        public static Dictionary<uint, ConquerAI> Ais = new Dictionary<uint, ConquerAI>();
        public static ConcurrentDictionary<uint, GameClient> Pool = new ConcurrentDictionary<uint, GameClient>();
        public static uint[] Body = { 1003, 2001 };
        public GameClient Client = null;
        public GameClient Bot = null;
        public IMapObj Target = null;
        public int HP;
        public int AttackBot = 0;
        public Time32 LastBotJump;
        private int JumpSpeed = 0;
        public BotSkills Attack;
        public uint UID;
        public BotLevel levelbot;//bulls
        public bool Empezar = false;
        public uint Segundos = 0;
        private int ShootChance = 0;
        public bool Stop1, Stop2, Stop3, Stop4, Stop5 = false;
        private Time32 LastAttack;
        private int Accuracy = 0;
        public Time32 Conteo2;
        public Time32 Conteo;
        public BotType Type;
        //public ConquerAI(GameClient client) => Client = client;
        public ConquerAI(GameClient client)
        {
            Client = client;
        }
        public void ConteoExtra()
        {
            if (!Empezar)
            {
                using (var rec = new RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (Time32.Now >= Bot.MyBot.Conteo2.AddSeconds(1))
                    {
                        if (Segundos < 5)
                        {
                            Segundos++;
                            Bot.MyBot.Conteo2 = Time32.Now;
                        }
                    }
                    if (Segundos == 5)
                    {
                        if (!Stop1)
                        {
                            Bot.Player.Owner.SendSysMesage("1 seconds until the duel starts", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white, true);
                            Stop1 = true;
                        }
                    }
                    else if (Segundos == 4)
                    {
                        if (!Stop2)
                        {
                            Bot.Player.Owner.SendSysMesage("2 seconds until the duel starts", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white, true);
                            Stop2 = true;
                        }
                    }
                    else if (Segundos == 3)
                    {
                        if (!Stop3)
                        {
                            Bot.Player.Owner.SendSysMesage("3 seconds until the duel starts", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white, true);
                            Stop3 = true;

                        }
                    }
                    else if (Segundos == 2)
                    {
                        if (!Stop4)
                        {
                            Bot.Player.Owner.SendSysMesage("4 seconds until the duel starts", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white, true);
                            Stop4 = true;

                        }
                    }
                    else if (Segundos == 1)
                    {
                        if (!Stop5)
                        {
                            Bot.Player.Owner.SendSysMesage("5 second until the duel starts", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white, true);
                            Stop5 = true;
                        }
                    }
                }
                if (Time32.Now >= Bot.MyBot.Conteo.AddSeconds(5))
                {
                    Empezar = true;
                    Segundos = 0;
                    Bot.SendTalk("You're dead boy! Lets fight", Bot.Player.Name, "ALL");
                    Bot.Player.Owner.SendSysMesage("The duel has begun", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white, true);
                }
            }
        }
        public IMapObj Buscar()
        {
            if (Target == null)
            {
                foreach (IMapObj Obj in Bot.Player.View.Roles(MapObjectType.Player))
                {

                    if (Role.Core.GetDistance(Bot.Player.X, Bot.Player.Y, Obj.X, Obj.Y) <= 12)
                    {
                        if (Obj.Alive)
                        {
                            Target = Obj;
                            break;
                        }
                    }
                    break;
                }
            }
            return Target;
        }
        public void Brincar()
        {//نطه البوت
            if (Bot != null && Bot.Map != null && Bot.Player != null)
            {
                if (Target == null)
                    return;
                if (!Empezar)
                    return;
                if (Time32.Now >= Bot.MyBot.LastBotJump.AddMilliseconds(Bot.MyBot.JumpSpeed))
                {
                    ushort X = Bot.Player.X;
                    ushort Y = Bot.Player.Y;

                    var dist = Role.Core.GetDistance(Bot.Player.X, Bot.Player.Y, Target.X, Target.Y);
                    if (dist != 0)
                    {
                        int count = (int)Math.Ceiling((double)dist / 16);
                        var path = Pathfinding.AStar.Calculate.FindWay(Target.X, Target.Y, Bot.Player.X, Bot.Player.Y, Bot.Map);
                        if (path != null)
                        {
                            var point = Math.Ceiling((double)path.Count / count);
                            if (point > 0)
                            {
                                point = Math.Min(path.Count - 1, point);
                                X = path[(int)point].X;
                                Y = path[(int)point].Y;
                            }
                            else
                            {
                                X = path[path.Count - 1].X;
                                Y = path[path.Count - 1].Y;
                            }
                        }
                        if (Target != null)
                        {
                            X = (ushort)Program.GetRandom.Next(Target.X - 4, Target.X + 4);
                            Y = (ushort)Program.GetRandom.Next(Target.Y - 4, Target.Y + 4);
                        }
                        if (!Bot.Map.ValidLocation(X, Y))
                        {
                            X = Bot.Player.X;
                            Y = Bot.Player.Y;
                        }
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            ActionQuery action = new ActionQuery()
                            {
                                ObjId = Bot.Player.UID,
                                Type = ActionType.Jump,
                                dwParam = (uint)((Bot.Player.Y << 16) | Bot.Player.X),
                                dwParam_Lo = X,
                                dwParam_Hi = Y,
                            };
                            Bot.Player.View.SendView(stream.ActionCreate(&action), true);
                            //Bot.Player.View.SendView(stream.InterActionWalk(inter), true);
                            Bot.Player.Angle = Role.Core.GetAngle(Bot.Player.X, Bot.Player.Y, X, Y);
                            Bot.Player.Action = Flags.ConquerAction.Jump;
                            Bot.Map.View.MoveTo<IMapObj>(Bot.Player, X, Y);
                            Bot.Player.X = X;
                            Bot.Player.Y = Y;
                            Bot.Player.View.Role(false, stream);
                            Bot.Player.LastMove = DateTime.Now;
                            LastBotJump = Time32.Now;
                        }
                    }
                }
            }
        }

        public void SetLevel(BotLevel Level)
        {
            switch (Level)
            {

                case BotLevel.Easy:
                    JumpSpeed = 1500;
                    ShootChance = 25;
                    Accuracy = 10;
                    AttackBot = 1200;
                    break;
                case BotLevel.Normal:
                    JumpSpeed = 1250;
                    ShootChance = 33;
                    Accuracy = 20;
                    AttackBot = 1000;
                    break;
                case BotLevel.Hard:
                    JumpSpeed = 700;
                    ShootChance = 80;
                    Accuracy = 98;
                    AttackBot = 600;
                    break;
                case BotLevel.Insane:
                    JumpSpeed = 600;
                    ShootChance = 100;
                    Accuracy = 98;
                    AttackBot = 600;
                    break;
                case BotLevel.ProPlayer:
                    JumpSpeed = 400;
                    ShootChance = 100;
                    Accuracy = 100;
                    AttackBot = 400;
                    break;


            }
        }
        public void Atacar()
        {//هجوم البوت
            if (Bot != null && Bot.Map != null && Bot.Player.View != null && Bot.Player != null && Bot.Player.HitPoints > 0)
            {
                if (!Empezar)
                    return;
                if (Target == null)
                    return;
                if (Time32.Now >= Bot.MyBot.LastAttack.AddMilliseconds(Bot.MyBot.AttackBot))
                {
                    Client.GameClient attacked = null;
                    if (Server.GamePoll.TryGetValue(Target.UID, out attacked))
                    {
                        using (var rec = new RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            ushort SpellID = 0;
                            if (AtributesStatus.IsTrojan(Bot.Player.Class))
                            {
                                if (Attack == BotSkills.Fastblade)
                                {
                                    SpellID = (ushort)BotSkills.Fastblade;

                                }
                                else if (Attack == BotSkills.ScrenSword)
                                {
                                    SpellID = (ushort)BotSkills.ScrenSword;
                                }
                                else if (Attack == BotSkills.ViperFang)
                                {
                                    SpellID = (ushort)BotSkills.ViperFang;
                                }
                                else if (Attack == BotSkills.DragonTail)
                                {
                                    SpellID = (ushort)BotSkills.DragonTail;
                                }
                            }
                            if (!Bot.Player.ContainFlag(MsgUpdate.Flags.Cyclone) && !Bot.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                            {
                                Dictionary<ushort, MagicType.Magic> Spells;
                                if (Server.Magic.TryGetValue(SpellID, out Spells))
                                {
                                    MsgSpell ClientSpell;
                                    if (Bot.MySpells.ClientSpells.TryGetValue(SpellID, out ClientSpell))
                                    {
                                        MagicType.Magic spell;
                                        if (Spells.TryGetValue(ClientSpell.Level, out spell))
                                        {
                                            if (SpellID != 0)
                                            {
                                                if (MyMath.Success(Bot.MyBot.ShootChance))
                                                {
                                                    InteractQuery action = new InteractQuery();
                                                    action.AtkType = MsgAttackPacket.AttackID.Magic;
                                                    action.UID = Bot.Player.UID;
                                                    action.OpponentUID = attacked.Player.UID;
                                                    if (MyMath.Success(Bot.MyBot.Accuracy))
                                                    {
                                                        action.X = Target.X;
                                                        action.Y = Target.Y;
                                                    }
                                                    else
                                                    {
                                                        action.X = (ushort)(Target.X + 1);
                                                        action.Y = (ushort)(Target.Y + 1);
                                                    }
                                                    action.Damage = (int)SpellID;
                                                    action.SpellID = (ushort)SpellID;
                                                    MsgAttackPacket.Process(Bot, action);
                                                }

                                            }
                                        }

                                    }
                                }

                            }
                        }
                    }
                    Bot.MyBot.LastAttack = Time32.Now;
                }
            }
        }
        public void TerminarHit()
        {//انهاء القتال
            if (Type == BotType.AI)
            {
                if (Target == null)
                    return;
                if (Bot.Player.Hits >= 100)
                {
                    using (var rec = new RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Bot.SendWhisper("You are Loser Go To Play GTA :P ", "AI Bot", Client.Player.Name);
                        Client.Teleport(300, 293, 1002);
                    }
                }
                else if (Client.Player.Hits >= 100)
                {
                    Client.Teleport(300, 293, 1002);

                }
            }
        }
        // [Performance] BotIdleTimeoutSec (default 0 = off, original; e.g. 120): a bot that never found a target is removed after this time.
        private static readonly int BotIdleTimeoutSec = Conquer6609.Framework.Settings.Int("Performance", "BotIdleTimeoutSec", 0);
        public readonly DateTime CreatedAt = DateTime.UtcNow;

        // True when the bot's owner logged out / relogged, or is no longer on the bot's map + dynamic id (room).
        private bool OwnerLeftRoom()
        {
            var owner = Client;
            if (owner == null || owner.Player == null)
                return true;
            Client.GameClient online;
            if (!Server.GamePoll.TryGetValue(owner.Player.UID, out online) || !object.ReferenceEquals(online, owner))
                return true;
            return owner.Player.Map != Bot.Player.Map || owner.Player.DynamicID != Bot.Player.DynamicID;
        }
        public void Remover()
        {
            if (Bot == null || Bot.Player == null)
                return;
            if (!ConquerAI.Pool.ContainsKey(Bot.Player.UID))
                return;// already removed
            bool remove = false;
            if (Target != null)
            {
                Client.GameClient client = null;
                if (!Server.GamePoll.TryGetValue(Target.UID, out client))
                    remove = true;
                else if (client.Player.Map != Bot.Player.Map)
                    remove = true;
            }
            bool ownerGone = OwnerLeftRoom();
            if (ownerGone)
                remove = true;
            if (!remove && Target == null && BotIdleTimeoutSec > 0
                && (DateTime.UtcNow - CreatedAt).TotalSeconds >= BotIdleTimeoutSec)
                remove = true;
            if (!remove)
                return;

            uint dynamicId = Bot.Player.DynamicID;
            if (Bot.Map != null)
                Bot.Map.RemoveAI(Bot);
            else
            {
                Client.GameClient removed;
                ConquerAI.Pool.TryRemove(Bot.Player.UID, out removed);
            }
            Empezar = false;
            Stop1 = false;
            Stop2 = false;
            Stop3 = false;
            Stop4 = false;
            Stop5 = false;
            Segundos = 0;
            // The room ends with its bot once nobody is left inside (the periodic sweep catches the rest).
            if (ownerGone)
                UnlimitedArenaRooms.RemoveIfEmpty(dynamicId);
        }

    }
}
