using Conquer6609.Database;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.MsgLoader;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgServer.AttackHandler;
using Conquer6609.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Client
{
    public partial class PoolProcesses
    {
        public static unsafe void FloorSpell(Client.GameClient client)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null)
                    return;
                Extensions.Time32 Now = Extensions.Time32.Now;

                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.ManiacDance))
                {
                    if (Now > client.Player.ManiacDanceStamp)
                    {
                        client.Player.ManiacDanceStamp = Extensions.Time32.Now.AddMilliseconds(1000);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var ClientSpell = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.ManiacDance];
                            var DBSpell = Database.Server.Magic[(ushort)Role.Flags.SpellID.ManiacDance][0];
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(
                                client.Player.UID
                                  , 0, client.Player.X, client.Player.Y, ClientSpell.ID
                                  , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in client.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                Game.MsgMonster.MonsterRole attacked = target as Game.MsgMonster.MonsterRole;
                                if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in client.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(client);
                        }
                    }
                }


                if (client.Player.FloorSpells.Count != 0)
                {
                    foreach (var ID in client.Player.FloorSpells)
                    {
                        switch (ID.Key)
                        {
                            case (ushort)Role.Flags.SpellID.ShadowofChaser:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        foreach (var spell in spellclient.Spells.GetValues())
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);


                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);



                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Range.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Range.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Range.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);


                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            AnimationObj.Hit = 1;//??

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                client.Player.View.SendView(stream.ItemPacketCreate(spell.FloorPacket), true);
                                              
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            case (ushort)Role.Flags.SpellID.HorrorofStomper:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        foreach (var spell in spellclient.Spells.GetValues())
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);
                                                client.Player.View.SendView(stream.ItemPacketCreate(spell.FloorPacket), true);

                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                spellclient.SpellPacket.X = spell.FloorPacket.OwnerX;
                                                spellclient.SpellPacket.Y = spell.FloorPacket.OwnerY;
                                                spellclient.SpellPacket.SpellLevel = spell.DBSkill.Level;


                                               
                                                var line = new Game.MsgServer.AttackHandler.Algoritms.Line(spell.FloorPacket.OwnerX, spell.FloorPacket.OwnerY, spell.FloorPacket.m_X, spell.FloorPacket.m_Y, 9);
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) < 9)
                                                    {
                                                        if (line.InLine(obj.X, obj.Y))
                                                        {
                                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                            {
                                                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID, 0, spell.FloorPacket.OwnerX, spell.FloorPacket.OwnerY, spellclient.DBSkill.ID , spellclient.DBSkill.Level, 0);
                                                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                                Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                                Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                                MsgSpell.Targets.Enqueue(AnimationObj);

                                                                MsgSpell.SetStream(stream);
                                                                MsgSpell.Send(monster);
                                                            }
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 9)
                                                    {
                                                        if (line.InLine(obj.X, obj.Y))
                                                        {
                                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                            {
                                                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                                Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                                Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                                spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                            }
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 9)
                                                    {
                                                        if (line.InLine(obj.X, obj.Y))
                                                        {
                                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                            {
                                                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                                Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);


                                                                Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                                AnimationObj.Hit = 1;//??

                                                                spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                            }
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                client.Player.View.SendView(stream.ItemPacketCreate(spell.FloorPacket), true);


                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            case (ushort)Role.Flags.SpellID.PeaceofStomper:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        foreach (var spell in spellclient.Spells.GetValues())
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);

                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);


                                                spellclient.SpellPacket.bomb = 1;

                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            //Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }

                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);


                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                client.Player.View.SendView(stream.ItemPacketCreate(spell.FloorPacket), true);

                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);


                                    }
                                    break;
                                }
                            case (ushort)Role.Flags.SpellID.RageofWar:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        foreach (var spell in spellclient.Spells.GetValues())
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);

                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);


                                                spellclient.SpellPacket.bomb = 1;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            //AnimationObj.Damage /= 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);


                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            AnimationObj.Hit = 1;//??

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                            
                                                client.Player.View.SendView(stream.ItemPacketCreate(spell.FloorPacket), true);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }

                                    break;
                                }
                            case (ushort)Role.Flags.SpellID.WrathoftheEmperor:
                            case (ushort)Role.Flags.SpellID.InfernalEcho:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        foreach (var spell in spellclient.Spells.GetValues())
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);

                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);


                                                spellclient.SpellPacket.bomb = 1;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);


                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            AnimationObj.Hit = 1;//??

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                client.Player.View.SendView(stream.ItemPacketCreate(spell.FloorPacket), true);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }

                                    break;
                                }
                            case (ushort)Role.Flags.SpellID.TwilightDance:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        foreach (var spell in spellclient.Spells.GetValues())
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;

                                                RemoveSpells.Enqueue(spell);
                                                spellclient.CreateMsgSpell(100);

                                                int increased_attack = 0;
                                                if (spellclient.Spells.Count == 3)
                                                    increased_attack = 15;
                                                else if (spellclient.Spells.Count == 2)
                                                    increased_attack = 25;
                                                else if (spellclient.Spells.Count == 1)
                                                    increased_attack = 35;

                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj, false, increased_attack);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);

                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            AnimationObj.Hit = 1;//??

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }


                                                //if (spellclient.AttackedTwilight == 0)
                                                {
                                                    spellclient.SendView(stream, client);
                                                }
                                                ActionQuery action = new ActionQuery()
                                            {
                                                ObjId = spell.FloorPacket.m_UID,
                                                dwParam_Hi = spell.FloorPacket.m_Y,
                                                dwParam_Lo = spell.FloorPacket.m_X,
                                                wParam1 = spellclient.AttackedPOSX,//(ushort)(spell.FloorPacket.m_X - 2),
                                                wParam2 = spellclient.AttckedPOSY, //(ushort)(spell.FloorPacket.m_Y - 3),
                                                Type = ActionType.RemoveTrap
                                            };

                                                client.Player.View.SendView(stream.ActionCreate(&action), true);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                client.Player.View.SendView(stream.ActionCreate(&action), true);
                                                client.Player.View.SendView(stream.ItemPacketCreate(spell.FloorPacket), true);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                            }

                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }
    }
}