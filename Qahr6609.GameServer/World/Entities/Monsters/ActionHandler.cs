using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgMonster
{
    public class ActionHandler
    {
        public Extensions.MSRandom Random = new Extensions.MSRandom();
        public Random SystemRandom = new Random();
        // Boss spell pools (were a new List<ushort> per attack).
        static readonly ushort[] BossSpells1 = new ushort[] { 8003, 11311, 10364, 11309, 11310 };
        static readonly ushort[] BossSpells2 = new ushort[] { 10001, 30010, 30010, 30011, 30012, 30013, 30014, 30010, 10372 };
        static readonly ushort[] BossSpells3 = new ushort[] { 7013, 7017, 10362, 10531, 11322 };
        static readonly ushort[] BossSpells4 = new ushort[] { 7013, 7014, 7015, 7016, 7017 };
        static readonly ushort[] BossSpells5 = new ushort[] { 10000, 10001, 10003 };
        static readonly ushort[] BossSpells6 = new ushort[] { 10500, 10502, 10503, 10504, 10506 };

        public bool Rate(int value)
        {
            return value > Random.Next() % 100;
        }

        public void ExecuteAction(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (monster.Family.ID == 40121 || monster.Family.ID == 40122 || monster.Family.ID == 40123 || monster.Family.ID == 40124)
                return;
            switch (monster.State)
            {
                case MobStatus.Idle:
                    {
                        monster.Target = null;
                        CheckTarget(client, monster); break;
                    }
                case MobStatus.SearchTarget: SearchTarget(client, monster); break;
                case MobStatus.Attacking: AttackingTarget(client, monster); break;
            }
        }
        public unsafe void CheckGuardPosition(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (monster.Alive)
            {
                if (monster.X != monster.RespawnX || monster.Y != monster.RespawnY)
                {
                    if (Extensions.Time32.Now > monster.MoveStamp.AddMilliseconds(300))
                    {
                        monster.MoveStamp = Extensions.Time32.Now;

                        Role.Flags.ConquerAngle dir = GetAngle(monster.X, monster.Y, monster.RespawnX, monster.RespawnY);
                        ushort WalkX = monster.X; ushort WalkY = monster.Y;
                        IncXY(dir, ref  WalkX, ref WalkY);

                        client.Owner.Map.View.MoveTo<Role.IMapObj>(monster, WalkX, WalkY);

                        monster.X = WalkX;
                        monster.Y = WalkY;

                        WalkQuery walk = new WalkQuery()
                        {
                            Direction = (byte)dir,
                            Running = 1,
                            UID = monster.UID
                        };
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            monster.Send(stream.MovementCreate(walk));
                        }
                    }
                }
            }
        }
        public unsafe bool GuardAttackPlayer(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (!monster.Alive)
                return false;
            if (client.ContainFlag(MsgServer.MsgUpdate.Flags.FlashingName) && client.Alive)
            {
                short distance = MonsterView.GetDistance(client.X, client.Y, monster.X, monster.Y);
                if (distance < monster.Family.AttackRange)
                {
                    if (!CheckRespouseDamage(client, monster))
                    {
                        uint Damage = MagicAttack(client.Owner, monster);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                , 0, client.X, client.Y, (ushort)monster.Family.SpellId, 0, 0);
                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(client.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                            SpellPacket.SetStream(stream);
                            SpellPacket.Send(monster);
                        }
                        CheckForOponnentDead(client, Damage, monster);
                        return true;
                    }
                }
            }
            return false;
        }
        public unsafe bool GuardAttackMonster(Role.GameMap map, Game.MsgMonster.MonsterRole attacked, Game.MsgMonster.MonsterRole monster)
        {
            if (!monster.Alive)
                return false;
            if (attacked.Alive)
            {
                short distance = MonsterView.GetDistance(attacked.X, attacked.Y, monster.X, monster.Y);
                if (distance < monster.Family.AttackRange)
                {
                    uint Damage = MagicAttack(attacked, monster);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                            , 0, attacked.X, attacked.Y, (ushort)monster.Family.SpellId, 0, 0);
                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(attacked.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                        SpellPacket.SetStream(stream);
                        SpellPacket.Send(monster);

                        if (Damage >= attacked.HitPoints)
                        {
                            map.SetMonsterOnTile(attacked.X, attacked.Y, false);
                            attacked.Dead(stream, null, monster.UID, map);
                        }
                        else
                            attacked.HitPoints -= Damage;

                    }
                    return true;
                }
            }
            return false;
        }
        public bool ExtraBoss(Game.MsgMonster.MonsterRole monster)
        {
            if (monster.Family.ID == 23973 || monster.Family.ID == 22973 || monster.Family.ID == 23974 || monster.Family.ID == 3969)
                return false;
            return monster.Family.MaxHealth > 100000 && monster.Family.MaxHealth < 7000000;
        }
        public unsafe void AttackingTarget(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (!monster.Alive)
                return;
            short distance = MonsterView.GetDistance(monster.Target.X, monster.Target.Y, monster.X, monster.Y);

            if (monster.Boss == 1 && monster.HitPoints >= 2000000)
                monster.Family.AttackRange = 18;
            if (distance > monster.Family.AttackRange || monster.Target == null || !monster.Target.Alive || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly)
                || !monster.Target.Owner.Socket.Alive)
            {

                monster.State = MobStatus.SearchTarget;
            }
            else
            {
                if (Extensions.Time32.Now > monster.AttackSpeed.AddMilliseconds(monster.Family.AttackSpeed))
                {
                    monster.AttackSpeed = Extensions.Time32.Now;
                    if (monster.Boss == 0 && monster.Family.SpellId == 0)
                    {
                        if (!CheckRespouseDamage(client, monster))
                        {
                            uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                            Damage = CheckDodge(monster.Target.Owner.Status.Dodge) ? Damage : 0;
                            if (Damage >= 1)
                            {
                                MsgServer.AttackHandler.CheckAttack.CheckItems.RespouseDurability(monster.Target.Owner);
                            }
                            InteractQuery action = new InteractQuery()
                            {
                                AtkType = MsgAttackPacket.AttackID.Physical,
                                X = monster.Target.X,
                                Y = monster.Target.Y,
                                UID = monster.UID,
                                OpponentUID = monster.Target.UID,
                                Damage = (int)Damage
                            };
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                monster.Send(stream.InteractionCreate(&action));
                            }

                            CheckForOponnentDead(monster.Target, Damage, monster);
                        }
                    }
                    else if (monster.Family.SpellId != 0 && monster.Boss == 0)
                    {
                        if (!CheckRespouseDamage(client, monster))
                        {
                            uint Damage = MagicAttack(monster.Target.Owner, monster);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                    , 0, monster.Target.X, monster.Target.Y, (ushort)monster.Family.SpellId, 0, 0);
                                SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                SpellPacket.SetStream(stream);
                                SpellPacket.Send(monster);
                            }

                            CheckForOponnentDead(monster.Target, Damage, monster);
                        }
                    }
                    else if (monster.Boss != 0)
                    {
                        if (!monster.Target.Alive || Role.Core.GetDistance(monster.X, monster.Y, monster.Target.X, monster.Target.Y) > 18)
                        {
                            monster.State = MobStatus.SearchTarget;
                            return;
                        }
                        switch (monster.Family.ID)
                        {
                            case 40946:
                            case 40936:
                            case 20160:
                            case 29360:
                            case 20170:
                            case 20062:
                            case 20061:
                            case 20063:
                            case 20064:
                            case 20065:
                         
                                {
                                    ushort[] Spells = BossSpells1;
                                    ushort rand = (byte)Program.GetRandom.Next(0, Spells.Length);
                                    switch (Spells[rand])
                                    {
                                        case 10364:
                                        case 8003:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);


                                                    CheckForOponnentDead(monster.Target, Damage, monster);
                                                }
                                                break;
                                            }

                                        case 11311:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.ResistEarth));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);


                                                    CheckForOponnentDead(monster.Target, Damage, monster);
                                                }
                                                break;
                                            }
                                        case 11309:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                       , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.X, monster.Y, player.X, player.Y) <= 7)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            //if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                            //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 3, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 11310:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 3)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            //if (Rate(10) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                            //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 5, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 40938:
                            case 40948:
                            case 20300:
                            case 29370:
                            case 29300:
                            case 40940:
                            case 20070://snow bamshee
                                {
                                    ushort[] Spells = BossSpells2;
                                    ushort rand = (byte)Program.GetRandom.Next(0, Spells.Length);
                                    switch (Spells[rand])
                                    {
                                        case 10001:
                                        case 30010:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                       , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.X, monster.Y, player.X, player.Y) <= 14)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            //if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                            //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 3, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 30011:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                       , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 8)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            //if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                            //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 3, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 30012:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 3)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            //if (Rate(10) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                            //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 5, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 30014:
                                        case 30013:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);


                                                    CheckForOponnentDead(monster.Target, Damage, monster);
                                                }
                                                if (Rate(5) && !monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                    monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                break;
                                            }
                                        case 10372:
                                            {
                                                uint Damage = 80000;
                                                monster.HitPoints = (uint)Math.Min(monster.Family.MaxHealth, (int)(monster.HitPoints + Damage));
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                    , 0, monster.X, monster.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 213883://chaos Guard
                                {
                                    ushort[] Spells = BossSpells3;
                                    ushort rand = (byte)Program.GetRandom.Next(0, Spells.Length);
                                    switch (Spells[rand])
                                    {
                                        case 11322:
                                            {
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);

                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                CheckForOponnentDead(monster.Target, Damage, monster);

                                                if (Rate(5) && !monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                    monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                break;
                                            }
                                        case 10531:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 18)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Dizzy))
                                                                player.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 3, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);

                                                }
                                                break;
                                            }
                                        case 10362:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 10)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Dizzy))
                                                                player.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 3, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);

                                                }
                                                break;
                                            }
                                        case 7013:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 18)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Dizzy))
                                                                player.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 3, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);

                                                }
                                                break;
                                            }
                                        case 7017:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                       , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 18)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                                player.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 20101:
                            case 40937:
                            case 40947:
                            case 40939:
                            case 40949:
                            case 29363:
                            case 20060://terato Dragon
                                {
                                    ushort[] Spells = BossSpells4;
                                    ushort rand = (byte)Program.GetRandom.Next(0, Spells.Length);
                                    switch (Spells[rand])
                                    {
                                        case 7017:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                       , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 8)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                                player.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 7014:
                                        case 7013:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 6)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Dizzy))
                                                                player.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 3, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);

                                                }
                                                break;
                                            }
                                        case 7015:
                                            {
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);

                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                CheckForOponnentDead(monster.Target, Damage, monster);

                                                if (Rate(5) && !monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                    monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                break;
                                            }
                                        case 7016:
                                            {
                                                uint Damage = 80000;
                                                monster.HitPoints = (uint)Math.Min(monster.Family.MaxHealth, (int)(monster.HitPoints + Damage));
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                    , 0, monster.X, monster.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);


                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 40935:
                            case 40945:
                            case 20055://lava beast
                                {
                                    ushort[] Spells = BossSpells5;
                                    ushort rand = (byte)Program.GetRandom.Next(0, Spells.Length);
                                    switch (Spells[rand])
                                    {
                                        case 10003:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                      , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.X, monster.Y, player.X, player.Y) <= 3)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                                player.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 10001:
                                            {

                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                CheckForOponnentDead(monster.Target, Damage, monster);

                                                if (Rate(5) && !monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Dizzy))
                                                {
                                                    monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 6, true);
                                                }
                                                break;
                                            }
                                        default:
                                            {
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                              , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                CheckForOponnentDead(monster.Target, Damage, monster);
                                                break;
                                            }

                                    }
                                    break;
                                }
                            case 66432:
                            case 40934:
                            case 40944:
                            case 6643://SwordMaster
                                {
                                    ushort[] Spells = BossSpells6;
                                    ushort rand = (byte)Program.GetRandom.Next(0, Spells.Length);
                                    uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                    if (rand == 0)
                                        Damage = (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, 130, 100);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                      , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                        SpellPacket.SetStream(stream);
                                        SpellPacket.Send(monster);
                                    }
                                    CheckForOponnentDead(monster.Target, Damage, monster);

                                    if (Rate(5))
                                    {
                                        monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 6, true);
                                    }
                                    else if (Rate(5) && !monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                        monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);


                                    break;
                                }

                        }
                        //bosses
                    }
                }
            }
        }
        public unsafe void SearchTarget(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (monster == null)
                return;
            if (!monster.Alive)
                return;
            try
            {
                if (monster.Target != null)
                {

                    if (!monster.Target.Alive || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly)
                        || !monster.Target.Owner.Socket.Alive)
                    {
                        monster.State = MobStatus.Idle;
                        return;
                    }
                }
                if (monster.Target == null)
                {
                    monster.State = MobStatus.Idle;
                    return;
                }
                if (monster.Family == null)
                    return;
                short distance = MonsterView.GetDistance(monster.Target.X, monster.Target.Y, monster.X, monster.Y);
                if (distance <= monster.Family.AttackRange || (monster.Boss == 1 && monster.HitPoints > 2000000 && distance <= 18))
                {
                    monster.State = MobStatus.Attacking;
                }
                else
                {
                    monster.State = MobStatus.Idle;
                }
                if (distance <= monster.Family.ViewRange && monster.Target != null && monster.Target.Alive)
                {

                    try
                    {
                        if (Extensions.Time32.Now > monster.MoveStamp.AddMilliseconds(monster.Family.MoveSpeed))
                        {
                            monster.MoveStamp = Extensions.Time32.Now;

                            bool Walk = Random.Next() % 100 < 70;
                            if (Walk)
                            {
                                Role.Flags.ConquerAngle dir = GetAngle(monster.X, monster.Y, monster.Target.X, monster.Target.Y);
                                ushort WalkX = monster.X; ushort WalkY = monster.Y;
                                IncXY(dir, ref WalkX, ref WalkY);

                                var Map = monster.Target.Owner.Map;
                                if (Map.ValidLocation(WalkX, WalkY) && !Map.MonsterOnTile(WalkX, WalkY))
                                {
                                    Map.SetMonsterOnTile(monster.X, monster.Y, false);
                                    Map.SetMonsterOnTile(WalkX, WalkY, true);

                                    Map.View.MoveTo<Role.IMapObj>(monster, WalkX, WalkY);

                                    monster.X = WalkX; monster.Y = WalkY;


                                    WalkQuery action = new WalkQuery()
                                    {
                                        Direction = (byte)dir,
                                        Running = 1,
                                        UID = monster.UID
                                    };
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        monster.Send(stream.MovementCreate(action));
                                    }
                                }
                                else
                                {
                                    dir = (Role.Flags.ConquerAngle)(Random.Next() % 8);
                                    WalkX = monster.X; WalkY = monster.Y;
                                    IncXY(dir, ref WalkX, ref WalkY);

                                    if (Map.ValidLocation(WalkX, WalkY) && !Map.MonsterOnTile(WalkX, WalkY))
                                    {
                                        Map.SetMonsterOnTile(monster.X, monster.Y, false);
                                        Map.SetMonsterOnTile(WalkX, WalkY, true);

                                        Map.View.MoveTo<Role.IMapObj>(monster, WalkX, WalkY);

                                        monster.X = WalkX; monster.Y = WalkY;


                                        WalkQuery action = new WalkQuery()
                                        {
                                            Direction = (byte)dir,
                                            Running = 1,
                                            UID = monster.UID
                                        };
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            monster.Send(stream.MovementCreate(action));

                                            monster.UpdateMonsterView(monster.Target.View, stream);
                                        }
                                    }
                                }
                            }

                        }

                    }
                    catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                }
                else
                    monster.State = MobStatus.Idle;
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public void CheckTarget(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (!monster.Alive)
                return;
            if (monster.Target == null && !client.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
            {
                short distance = MonsterView.GetDistance(client.X, client.Y, monster.X, monster.Y);
                if (distance <= monster.Family.ViewRange && client.Alive)
                {
                    var targ = monster.View.GetTarget(client.Owner.Map, Role.MapObjectType.Player);
                    if (targ != null)
                    {
                        monster.Target = targ as Role.Player;
                        if (!monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                        {
                            monster.State = MobStatus.SearchTarget;
                        }
                        else
                        {
                            foreach (var OtherTarget in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                            {
                                var obj = OtherTarget as Role.Player;
                                if (!obj.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                                {
                                    monster.Target = obj;
                                    monster.State = MobStatus.SearchTarget;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                short distance = MonsterView.GetDistance(client.X, client.Y, monster.X, monster.Y);
                if (monster.Target == null)
                {
                    var targ = monster.View.GetTarget(client.Owner.Map, Role.MapObjectType.Player);
                    if (targ != null)
                    {
                        monster.Target = targ as Role.Player;
                        if (!monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly) || monster.Boss == 1)
                        {
                            monster.State = MobStatus.SearchTarget;
                        }
                        else
                        {
                            foreach (var OtherTarget in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                            {
                                var obj = OtherTarget as Role.Player;
                                if (!obj.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                                {
                                    monster.Target = obj;
                                    monster.State = MobStatus.SearchTarget;
                                }
                            }
                        }
                    }
                }
                if (monster.Target != null && (distance > monster.Family.ViewRange || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly)
                    || monster.Target.Owner.Socket == null || !monster.Target.Owner.Socket.Alive))
                {
                    monster.Target = null;
                }
                else if (monster.Target == null || monster.Target.Alive)
                    monster.State = MobStatus.SearchTarget;
            }
        }
        public bool CheckDodge(uint Dodge)
        {
            bool allow = true;
            uint Noumber = (uint)Random.Next() % 150;
            if (Noumber > 60)
                Noumber = (uint)Random.Next() % 150;
            if (Noumber < Dodge)
                allow = false;
            return allow;
        }
        public void CheckForOponnentDead(Role.Player Player, uint Damage, MonsterRole monster)
        {
            if (Player.Alive == false)
                return;

            if (Player.ActivePick)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Player.RemovePick(stream);
                }
            }
            if (!Player.Owner.Socket.Alive)
                return;
            if (Damage >= Player.HitPoints)
            {

                ushort X = Player.X;
                ushort Y = Player.Y;
                Player.Dead(null, X, Y, monster.UID);
                if (Player.OnAutoHunt)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (Player.MainFlag == Role.Player.MainFlagType.ClaimGift || Player.MainFlag == Role.Player.MainFlagType.CanClaim)
                        {
                            Player.Owner.Send(MsgAutoHunt.AutoHuntCreate(stream, 5, 0, Player.AutoHuntExp, monster.Name));
                            if (Player.AutoHuntExp > 0)
                            {
                                Player.Owner.IncreaseAutoExperience(stream, Player.AutoHuntExp);
                                Player.AutoHuntExp = 0;
                            }
                        }
                        else
                        {
                            Player.Owner.Send(MsgAutoHunt.AutoHuntCreate(stream, 6, 0, Player.AutoHuntExp, monster.Name));
                            Player.AutoHuntExp = 0;
                        }
                    }
                }
            }
            else
            {
                if (Player.Action == Role.Flags.ConquerAction.Sit)
                {
                    if (Player.Stamina >= 20)
                        Player.Stamina -= 20;
                    else
                        Player.Stamina = 0;

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Player.SendUpdate(stream, Player.Stamina, MsgServer.MsgUpdate.DataType.Stamina);
                    }


                    Player.Action = Role.Flags.ConquerAction.None;
                }
                if (Player.ContainFlag(MsgUpdate.Flags.AzureShield))
                {
                    if (Damage > Player.AzureShieldDefence)
                    {
                        Conquer6609.Game.MsgServer.AttackHandler.Calculate.AzureShield.CreateDmg(Player, Player, Player.AzureShieldDefence);
                        Player.RemoveFlag(MsgUpdate.Flags.AzureShield);
                        Damage -= Player.AzureShieldDefence;

                    }
                    else
                    {
                        Player.AzureShieldDefence -= (ushort)Damage;
                        Conquer6609.Game.MsgServer.AttackHandler.Calculate.AzureShield.CreateDmg(Player, Player, Damage);
                        Damage = 1;
                    }
                }
                else
                    Player.HitPoints -= (int)Damage;
            }
        }
        public unsafe bool CheckRespouseDamage(Role.Player player, MonsterRole Monster)
        {
            //   if (Rate(99))
            {
                if (player.ContainFlag(MsgUpdate.Flags.RevengeTail))
                {
                    if (player.RevengeTailChange > 0)
                    {
                        MsgSpell ClientSpell;
                        if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.RevengeTail, out ClientSpell))
                        {
                            Database.MagicType.Magic DBSpell;
                            Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                            if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.RevengeTail, out DBSpells))
                            {
                                if (DBSpells.TryGetValue(ClientSpell.Level, out DBSpell))
                                {

                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(player.UID
                                 , 0, player.X, player.Y, ClientSpell.ID
                                 , ClientSpell.Level, ClientSpell.UseSpellSoul);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        MsgSpell.bomb = 1;
                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(player.Owner, Monster, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj()
                                            {
                                                UID = player.UID,
                                                Hit = 1
                                            };
                                            AnimationObj.Damage = (uint)DBSpell.Damage;
                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, player.Owner, Monster);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(player.Owner);
                                    }
                                    player.RevengeTailChange -= 1;
                                    if (player.RevengeTailChange == 0)
                                        player.RemoveFlag(MsgUpdate.Flags.RevengeTail);
                                }

                            }
                        }
                    }
                }
                if (player.ContainFlag(MsgUpdate.Flags.Backfire))
                {
                    player.RemoveFlag(MsgUpdate.Flags.Backfire);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (player.ContainFlag(MsgServer.MsgUpdate.Flags.ShurikenVortex))
                            return false;
                        Database.MagicType.Magic DBSpell;
                        Game.MsgServer.MsgSpell ClientSpell;
                        if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Backfire, out ClientSpell))
                        {
                            Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                            if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.Backfire, out DBSpells))
                            {
                                if (DBSpells.TryGetValue(ClientSpell.Level, out DBSpell))
                                {
                                    Game.MsgServer.MsgSpellAnimation.SpellObj DmgObj = new Game.MsgServer.MsgSpellAnimation.SpellObj(Monster.UID, 0, MsgAttackPacket.AttackEffect.None);
                                    DmgObj.Damage = (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv(player.HitPoints, 55, 100);

                                    //update spell
                                    if (ClientSpell.Level < DBSpells.Count - 1)
                                    {
                                        ClientSpell.Experience += (int)(DmgObj.Damage * Program.ServerConfig.ExpRateSpell);
                                        if (ClientSpell.Experience > DBSpells[ClientSpell.Level].Experience)
                                        {
                                            ClientSpell.Level++;
                                            ClientSpell.Experience = 0;
                                        }
                                        player.Send(stream.SpellCreate(ClientSpell));
                                    }

                                    InteractQuery action = new InteractQuery()
                                    {

                                        Damage = (int)DmgObj.Damage,
                                        AtkType = MsgAttackPacket.AttackID.BackFire,
                                        X = player.X,
                                        Y = player.Y,
                                        OpponentUID = Monster.UID,
                                        UID = player.UID,
                                        Effect = DmgObj.Effect
                                    };

                                    Monster.Send(stream.InteractionCreate(&action));


                                    Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, DmgObj, player.Owner, Monster);
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            if (Rate(30))
            {
                if (player.ContainReflect)
                {
                    Game.MsgServer.MsgSpellAnimation.SpellObj DmgObj = new Game.MsgServer.MsgSpellAnimation.SpellObj();
                    DmgObj.Damage = PhysicalAttack(player.Owner, Monster);
                    DmgObj.Damage /= 10;
                    if (DmgObj.Damage == 0)
                        DmgObj.Damage = 1;

                    InteractQuery action = new InteractQuery()
                    {
                        Damage = (int)DmgObj.Damage,
                        ResponseDamage = DmgObj.Damage,
                        AtkType = MsgAttackPacket.AttackID.Reflect,
                        X = player.X,
                        Y = player.Y,
                        OpponentUID = Monster.UID,
                        UID = player.UID,
                        Effect = DmgObj.Effect
                    };
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        Monster.Send(stream.InteractionCreate(&action));

                        Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, DmgObj, player.Owner, Monster);
                    }
                    return true;


                }

                if (player.ActivateCounterKill)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (player.ContainFlag(MsgServer.MsgUpdate.Flags.ShurikenVortex))
                            return false;
                        Database.MagicType.Magic DBSpell;
                        Game.MsgServer.MsgSpell ClientSpell;
                        if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.CounterKill, out ClientSpell))
                        {
                            Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                            if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.CounterKill, out DBSpells))
                            {
                                if (DBSpells.TryGetValue(ClientSpell.Level, out DBSpell))
                                {
                                    Game.MsgServer.MsgSpellAnimation.SpellObj DmgObj = new Game.MsgServer.MsgSpellAnimation.SpellObj();
                                    Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(player, Monster, DBSpell, out DmgObj);

                                    //update spell
                                    if (ClientSpell.Level < DBSpells.Count - 1)
                                    {
                                        ClientSpell.Experience += (int)(DmgObj.Damage * Program.ServerConfig.ExpRateSpell);
                                        if (ClientSpell.Experience > DBSpells[ClientSpell.Level].Experience)
                                        {
                                            ClientSpell.Level++;
                                            ClientSpell.Experience = 0;
                                        }
                                        player.Send(stream.SpellCreate(ClientSpell));
                                    }

                                    InteractQuery action = new InteractQuery()
                                    {
                                        ResponseDamage = DmgObj.Damage,
                                        AtkType = MsgAttackPacket.AttackID.Scapegoat,
                                        X = player.X,
                                        Y = player.Y,
                                        OpponentUID = Monster.UID,
                                        UID = player.UID,
                                        Effect = DmgObj.Effect
                                    };

                                    Monster.Send(stream.InteractionCreate(&action));


                                    Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, DmgObj, player.Owner, Monster);
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public uint MagicAttack(MonsterRole attacked, MonsterRole monster)
        {
            uint power = (uint)monster.Family.MaxAttack;
            if (power > attacked.Family.Defense)
                power -= attacked.Family.Defense;
            else power = 1;
            return power;
        }
        public uint MagicAttack(Client.GameClient client, MonsterRole monster)
        {
            if (!client.Socket.Alive)
                return 0;
            if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
                return 1;
            MsgServer.AttackHandler.CheckAttack.CheckGemEffects.CheckRespouseDamage(client);

            uint power = (uint)monster.Family.MaxAttack;
            power = DecreaseBless(power, client.Status.ItemBless);

            if (power > client.Status.MagicDefence)
                power -= client.Status.MagicDefence;
            else power = 1;
            if (power > client.Status.MagicDamageDecrease)
                power -= client.Status.MagicDamageDecrease;
            else power = 1;
            return power;
        }
        public uint PhysicalAttack(Client.GameClient client, MonsterRole monster)
        {
            if (!client.Socket.Alive)
                return 0;
            if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
                return 1;
            MsgServer.AttackHandler.CheckAttack.CheckGemEffects.CheckRespouseDamage(client);

            uint power = 0;
            if (monster.Family.MaxAttack > monster.Family.MinAttack)
                power = (uint)SystemRandom.Next(monster.Family.MinAttack, monster.Family.MaxAttack);
            else
                power = (uint)SystemRandom.Next(monster.Family.MaxAttack, monster.Family.MinAttack);

            power += (uint)(power * Program.ServerConfig.PhysicalDamage / 100);

            power = DecreaseBless(power, client.Status.ItemBless);

            if (power > client.AjustDefense)
                power -= client.AjustDefense;
            else
                power = 1;

            //            power = (uint)Database.Disdain.MonsterAttackUser(monster, client.Player, (int)power);

            if (power > client.AjustPhysicalDamageDecrease())
                power -= client.AjustPhysicalDamageDecrease();
            else
                power = 1;



            return power;
        }
        public uint DecreaseBless(uint Damage, uint bless)
        {
            uint power = Damage;
            power = (power * bless) / 100;
            power = Damage - power;
            return power;
        }
        public Role.Flags.ConquerAngle GetAngle(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            double direction = 0;

            double AddX = X2 - X;
            double AddY = Y2 - Y;
            double r = (double)Math.Atan2(AddY, AddX);

            if (r < 0) r += (double)Math.PI * 2;

            direction = 360 - (r * 180 / (double)Math.PI);

            byte Dir = (byte)((7 - (Math.Floor(direction) / 45 % 8)) - 1 % 8);
            return (Role.Flags.ConquerAngle)(byte)((int)Dir % 8);
        }
        public static void IncXY(Role.Flags.ConquerAngle Facing, ref ushort x, ref ushort y)
        {
            sbyte xi, yi;
            xi = yi = 0;
            switch (Facing)
            {
                case Role.Flags.ConquerAngle.North: xi = -1; yi = -1; break;
                case Role.Flags.ConquerAngle.South: xi = 1; yi = 1; break;
                case Role.Flags.ConquerAngle.East: xi = 1; yi = -1; break;
                case Role.Flags.ConquerAngle.West: xi = -1; yi = 1; break;
                case Role.Flags.ConquerAngle.NorthWest: xi = -1; break;
                case Role.Flags.ConquerAngle.SouthWest: yi = 1; break;
                case Role.Flags.ConquerAngle.NorthEast: yi = -1; break;
                case Role.Flags.ConquerAngle.SouthEast: xi = 1; break;
            }
            x = (ushort)(x + xi);
            y = (ushort)(y + yi);
        }
    }
}
