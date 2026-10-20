using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Conquer6609.Game.MsgServer
{
    public class MsgSpellAnimation : IDisposable
    {
        public class SpellObj
        {
            public static SpellObj ShallowCopy(SpellObj obj)
            {
                return (SpellObj)obj.MemberwiseClone();
            }
            public uint UID;
            public uint Damage;
            public uint Hit;
            public MsgAttackPacket.AttackEffect Effect;
            public uint MoveX;
            public uint MoveY;

            public uint x1;
            public uint y2;
            public uint xx1;
            public uint yy2;
            public uint xxx1;
            public uint yyy2;

            public SpellObj()
            {
                Hit = 1;
            }
            public SpellObj(uint _target, uint _damage
                ,MsgAttackPacket.AttackEffect _effect)
            {
                UID = _target;
                Damage = _damage;
                Effect = _effect;
                Hit = 1;
            }
            public SpellObj(uint _target, uint _damage
               , MsgAttackPacket.AttackEffect _effect,ushort x, ushort y)
            {
                UID = _target;
                Damage = _damage;
                Effect = _effect;
                Hit = 1;
                MoveY = y;
                MoveX = x;
                x1 = xx1 = xx1 = x;
                y2 = yy2 = yyy2 = y;

            }
        }
      //  public List<ServerSockets.Packet> Arrays;

        public Queue<SpellObj> Targets;
        public uint UID;
        public uint OpponentUID;
        public ushort X;
        public ushort Y;
        public ushort SpellID;
        public byte LevelSoul;
        public ushort SpellLevel;
        public ushort NextSpell;
        public uint bomb = 0;

        public MsgSpellAnimation()
        {
            Targets = new Queue<SpellObj>();
        }
        public MsgSpellAnimation(uint _uid, uint oponnent
            , ushort _x, ushort _y, ushort _spell, ushort _spelllevel, byte _levelsoul, uint boomb =0)
        {
            Targets = new Queue<SpellObj>();
          
            UID = _uid;
            OpponentUID = oponnent;
            X = _x;
            Y = _y;
            SpellID = _spell;
            SpellLevel = _spelllevel;
            LevelSoul = _levelsoul;
            bomb = boomb;
        }
        // A spell packet carries at most this many targets; bigger target lists are sent as several packets.
        private const int MaxTargetsPerPacket = 30;
        private unsafe ServerSockets.Packet CreateAnimation(SpellObj[] Spells, int offset, int count, ServerSockets.Packet stream)
        {
            stream.InitWriter();
            stream.Write(UID);//4
            if (OpponentUID != 0)
                stream.Write(OpponentUID);//8
            else
            {
                stream.Write(X);
                stream.Write(Y);
            }
            stream.Write(SpellID);//12
            stream.Write(SpellLevel);//14
            stream.Write(NextSpell);//16

            stream.Write(LevelSoul);//18

            // Number of targets in THIS packet (it used to be the total, which broke chunked packets).
            stream.Write((byte)count);//19

            stream.Write((uint)bomb);

            for (int i = offset; i < offset + count; i++)
             {
                 var Obj = Spells[i];
                 //if (Obj.Damage >= 1)
                 {
                     stream.Write(Obj.UID);
                     stream.Write(Obj.Damage);
                     stream.Write(Obj.Hit);
                     stream.Write((uint)Obj.Effect);
                     stream.Write(0);//unknow
                     stream.Write(Obj.MoveX);
                     stream.Write(Obj.MoveY);
                     stream.Write(0);//unknow
                 }
             }
         
             stream.Finalize(Game.GamePackets.SpellUse);
             return stream;
        }
        private static int ChunkSize(SpellObj[] targets, int offset)
        {
            return Math.Min(MaxTargetsPerPacket, targets.Length - offset);
        }
        public ServerSockets.Packet _Stream;
        public unsafe void SetStream(ServerSockets.Packet stream)
        {
            _Stream = stream;
       
        }
        private bool TryDequeue(Queue<SpellObj> Data, out SpellObj obj)
        {
            if (Data.Count != 0)
            {
                obj = Data.Dequeue();
                return true;
            }
            obj = null;
            return false;
        }
        // Each method sends one packet (also when there are no targets, as before) or one packet per
        // MaxTargetsPerPacket targets.
        public void JustMe(Client.GameClient user)
        {
            var targets = Targets.ToArray();
            int offset = 0;
            do
            {
                int count = ChunkSize(targets, offset);
                user.Send(CreateAnimation(targets, offset, count, _Stream));
                offset += count;
            }
            while (offset < targets.Length);
        }
        public void Send(Client.GameClient user, bool self = true)
        {
            var targets = Targets.ToArray();
            int offset = 0;
            do
            {
                int count = ChunkSize(targets, offset);
                user.Player.View.SendView(CreateAnimation(targets, offset, count, _Stream), self);
                offset += count;
            }
            while (offset < targets.Length);
        }
       
        public void SendRole(Client.GameClient user)
        {
            var targets = Targets.ToArray();
            int offset = 0;
            do
            {
                int count = ChunkSize(targets, offset);
                user.Player.View.Role(false, CreateAnimation(targets, offset, count, _Stream));
                offset += count;
            }
            while (offset < targets.Length);
        }
        public void Send(MsgMonster.MonsterRole monster)
        {
            var targets = Targets.ToArray();
            int offset = 0;
            do
            {
                int count = ChunkSize(targets, offset);
                monster.Send(CreateAnimation(targets, offset, count, _Stream));
                offset += count;
            }
            while (offset < targets.Length);

        }
        public void Dispose()
        {
            Targets = null;
        }
    }
}
