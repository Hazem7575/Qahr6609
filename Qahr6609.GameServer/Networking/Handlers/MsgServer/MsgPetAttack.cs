using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{

    public unsafe static class MsgPetAttack
    {
        public static unsafe void GetPetAttack(this ServerSockets.Packet stream, out uint TagertUID, out byte CloneCount, out uint CloneOne, out uint CloneTwo)
        {
            TagertUID = stream.ReadUInt32();
            CloneCount = stream.ReadUInt8();
            CloneOne = stream.ReadUInt32();
            CloneTwo = stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet PetAttackCreate(this ServerSockets.Packet stream, uint TagertUID, byte CloneCount, uint CloneOne, uint CloneTwo)
        {
            stream.InitWriter();

            stream.Write(TagertUID);
            stream.Write(CloneCount);
            stream.Write(CloneOne);
            stream.Write(CloneTwo);


            stream.Finalize(GamePackets.MsgPetAttack);
            return stream;
        }

      
        [PacketAttribute(Game.GamePackets.MsgPetAttack)]
        public unsafe static void Proces(Client.GameClient client, ServerSockets.Packet stream)
        {

              uint TagertUID;
         byte CloneCount;
         uint CloneOne;
         uint CloneTwo;


            stream.GetPetAttack(out TagertUID,out CloneCount, out CloneOne, out CloneTwo);

            // Clone attack rules: caster alive and able to act, owns ShadowClone and has clones,
            // at most one clone attack per weapon attack interval. The broadcast is built from
            // server-side clone data, not echoed from the packet.
            if (!client.Player.Alive || client.Player.ContainFlag(MsgUpdate.Flags.Freeze)
                || client.Player.ContainFlag(MsgUpdate.Flags.Dizzy))
                return;
            MsgSpell shadowClone;
            if (!client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ShadowClone, out shadowClone))
                return;
            var clones = client.Player.MyClones.GetValues().ToArray();
            if (clones.Length == 0)
                return;
            int interval = client.Equipment != null ? client.Equipment.AttackSpeed(true) : 800;
            if (!Conquer6609.Security.Guard.Throttle("petattack:" + client.Player.UID, interval))
                return;

            CloneCount = (byte)Math.Min(2, clones.Length);
            CloneOne = clones[0].UID;
            CloneTwo = clones.Length > 1 ? clones[1].UID : 0;
            client.Player.View.SendView(stream.PetAttackCreate(TagertUID, CloneCount, CloneOne, CloneTwo), true);

            int percentage = 55;
            if (shadowClone.Level >= 1)
            {
                percentage += shadowClone.Level * 5;
            }

            foreach (var clone in clones)
            {
                InteractQuery action = new InteractQuery()
                {
                    UID = clone.UID,
                    AtkType = MsgAttackPacket.AttackID.Physical
                };

                Role.IMapObj target;
                if (client.Player.View.TryGetValue(TagertUID, out target, Role.MapObjectType.Player))
                {
                    if (AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, target.X, target.Y) <= 3)
                    {
                        Role.Player attacked = target as Role.Player;
                        if (AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, attacked, null))
                        {
                           // if(attacked.UID == client.Player.UID)

                            MsgSpellAnimation.SpellObj AnimationObj;
                            AttackHandler.Calculate.Physical.OnPlayer(client.Player, attacked, null, out AnimationObj);
                            AnimationObj.Damage = (uint)(AnimationObj.Damage * percentage / 100);
                            action.OpponentUID = target.UID;

                            action.Damage = (int)AnimationObj.Damage;
                            action.Effect = AnimationObj.Effect;

                            client.Player.View.SendView(stream.InteractionCreate(&action),true);

                            AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, attacked);
                        }
                    }
                }
                else if (client.Player.View.TryGetValue(TagertUID, out target, Role.MapObjectType.Monster))
                {

                    if (AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, target.X, target.Y) <= 3)
                    {
                        MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                        if (AttackHandler.CheckAttack.CanAttackMonster.Verified(client, attacked, null))
                        {
                            MsgSpellAnimation.SpellObj AnimationObj;

                                AttackHandler.Calculate.Physical.OnMonster(client.Player, attacked, null, out AnimationObj);

                                AnimationObj.Damage = (uint)(AnimationObj.Damage * percentage / 100);
                            action.OpponentUID = target.UID;

                            action.Damage = (int)AnimationObj.Damage;
                            action.Effect = AnimationObj.Effect;

                            client.Player.View.SendView(stream.InteractionCreate(&action), true);


                             AttackHandler.ReceiveAttack.Monster.Execute(stream,AnimationObj, client, attacked);
                        }
                    }

                }
                else if (client.Player.View.TryGetValue(TagertUID, out target, Role.MapObjectType.SobNpc))
                {
                    if (AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, target.X, target.Y) <= 3)
                    {
                        var attacked = target as Role.SobNpc;
                        if (AttackHandler.CheckAttack.CanAttackNpc.Verified(client, attacked, null))
                        {
                            MsgSpellAnimation.SpellObj AnimationObj;
                            AttackHandler.Calculate.Physical.OnNpcs(client.Player, attacked, null, out AnimationObj);

                            AnimationObj.Damage = (uint)(AnimationObj.Damage * percentage / 100);
                            action.OpponentUID = target.UID;

                            action.Damage = (int)AnimationObj.Damage;
                            action.Effect = AnimationObj.Effect;

                            client.Player.View.SendView(stream.InteractionCreate(&action), true);


                            AttackHandler.ReceiveAttack.Npc.Execute(stream,AnimationObj, client, attacked);
                        }
                    }
                }
            }
        }
    }
}
