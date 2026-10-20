using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgFloorItem;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
    public class LayTrap
    {

       

        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.PeaceofStomper:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            Role.IMapObj _target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                            {

                                if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                    user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.PeaceofStomper, (ushort)_target.X, (ushort)_target.Y, 14, DBSpell, 1000);
                                FloorItem.FloorPacket.DontShow = 1;
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = 1, MoveX = user.Player.X, Hit = 1, MoveY = user.Player.Y, UID = FloorItem.FloorPacket.m_UID });

                                {
                                    MsgSpell _cspell;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.HorrorofStomper, out _cspell))
                                    {
                                        var _DBSpells = Database.Server.Magic[_cspell.ID];
                                        DBSpell = _DBSpells[(ushort)Math.Min(_DBSpells.Count - 1, _cspell.Level)];


                                        if (!user.Player.FloorSpells.ContainsKey(_cspell.ID))
                                            user.Player.FloorSpells.TryAdd(_cspell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, (ushort)_target.X, (ushort)_target.Y, _cspell.SoulLevel, DBSpell, user.Map));
                                        FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.HorrorofStomper, (ushort)_target.X, _target.Y, 14, DBSpell, 2000);
                                        FloorItem.FloorPacket.DontShow = 1;
                                        FloorItem.FloorPacket.OwnerX = user.Player.X;
                                        FloorItem.FloorPacket.OwnerY = user.Player.Y;

                                        FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                                        FloorItem.FloorPacket.m_Color = 14;
                                        FloorItem.FloorPacket.Plus = 14;

                                        user.Player.FloorSpells[_cspell.ID].AddItem(FloorItem);

                                    }
                                }

                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.HorrorofStomper:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, user.Player.X, user.Player.Y, ClientSpell.ID
                                , 0, ClientSpell.UseSpellSoul);


                            Role.IMapObj _target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                            {
                                if (!user.Player.FloorSpells.ContainsKey(DBSpell.ID))
                                    user.Player.FloorSpells.TryAdd(DBSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, (ushort)_target.X, (ushort)_target.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.HorrorofStomper, (ushort)_target.X, _target.Y, 14, DBSpell, 1000);
                                FloorItem.FloorPacket.DontShow = 1;
                                FloorItem.FloorPacket.OwnerX = user.Player.X;
                                FloorItem.FloorPacket.OwnerY = user.Player.Y;
                                FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                                FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                                FloorItem.FloorPacket.m_Color = 14;
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                                user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                   
                    case (ushort)Role.Flags.SpellID.TwilightDance:
                        {

                            Attack.UID = user.Player.UID;
                            Attack.OpponentUID = user.Player.UID;
                            Attack.Damage = 0;
                            Attack.AtkType = 0;
                            ushort[] POS =new ushort[]{ user.Player.X, user.Player.Y};

                            user.Send(stream.InteractionCreate(&Attack));

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Algoritms.LayTrapThree Line = new Algoritms.LayTrapThree(user.Player.X, Attack.X, user.Player.Y, Attack.Y, 15);

                            int Stamp = 300;
                            byte Color = 2;
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            user.Player.FloorSpells[ClientSpell.ID].AttackedTwilight = 0;
                            List<MsgFloorItem.MsgItem> Items = new List<MsgFloorItem.MsgItem>();
                            foreach (var coords in Line.LCoords)
                            {
                                if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                    user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));

                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.TwilightDance, (ushort)coords.X, (ushort)coords.Y, Color, DBSpell, Stamp);
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem, POS[0], POS[1]);
                                Color++;
                                Stamp += 400;


                                user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);

                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                           MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.InfernalEcho:
                        {

                            Attack.UID = user.Player.UID;
                            Attack.OpponentUID = user.Player.UID;
                            Attack.Damage = 0;
                            Attack.AtkType = 0;


                            user.Send(stream.InteractionCreate(&Attack));

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            Algoritms.RandomFourLayTraps location = new Algoritms.RandomFourLayTraps(user.Player.X, user.Player.Y);

                            foreach (var coords in location.Coords)
                            {

                                if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                    user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.InfernalEcho, (ushort)coords.X, (ushort)coords.Y, 14, DBSpell, 4000);
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = 1, MoveX = user.Player.X, Hit = 1, MoveY = user.Player.Y, UID = FloorItem.FloorPacket.m_UID });
                                user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            }


                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WrathoftheEmperor:
                        {



                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);



                            Role.IMapObj _target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                            {
                                if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                    user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.WrathoftheEmperor, (ushort)_target.X, (ushort)_target.Y, 14, DBSpell, 2000);
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = 1, MoveX = user.Player.X, Hit = 1, MoveY = user.Player.Y, UID = FloorItem.FloorPacket.m_UID });
                                user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);

                            }
                         
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.AuroraLotus:
                        {
                            if (user.Player.TaoistPower >= 9)
                            {
                                Attack.UID = user.Player.UID;
                                Attack.OpponentUID = user.Player.UID;
                                Attack.Damage = 0;
                                Attack.AtkType = 0;


                                user.Send(stream.InteractionCreate(&Attack));

                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                    , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                    , ClientSpell.Level, ClientSpell.UseSpellSoul);


                                if (Database.Server.AddFloor(stream, user.Map, Game.MsgFloorItem.MsgItemPacket.AuroraLotus, Attack.X, Attack.Y, ClientSpell.Level, DBSpell, user, user.Player.GuildID, user.Player.UID, user.Player.DynamicID, "AuroraLotus", true))
                                {
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);
                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                                    user.Player.TaoistPower = 0;
                                    user.Player.UpdateTaoPower(stream);
                                    user.Player.RemoveFlag(MsgUpdate.Flags.FullPowerWater);
                                }
                                else
                                {
                                    user.SendSysMesage("Invalid Aurora location.");
                                   
                                }
                            }


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FlameLotus:
                        {
                            if (user.Player.TaoistPower >= 9)
                            {
                                Attack.UID = user.Player.UID;
                                Attack.OpponentUID = user.Player.UID;
                                Attack.Damage = 0;
                                Attack.AtkType = 0;


                                user.Send(stream.InteractionCreate(&Attack));

                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                    , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                    , ClientSpell.Level, ClientSpell.UseSpellSoul);

                                if (Database.Server.AddFloor(stream, user.Map, Game.MsgFloorItem.MsgItemPacket.FlameLotus, Attack.X, Attack.Y, ClientSpell.Level, DBSpell, user, user.Player.GuildID, user.Player.UID, user.Player.DynamicID, "FlameLotus", true))
                                {
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);

                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                                    user.Player.TaoistPower = 0;
                                    user.Player.UpdateTaoPower(stream);

                                    user.Player.RemoveFlag(MsgUpdate.Flags.FullPowerFire);
                                }
                                else
                                {
                                    user.SendSysMesage("Invalid Aurora location.");
                                   
                                    
                                }
                            }

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DaggerStorm:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            MsgServer.MsgGameItem item = new MsgGameItem();
                            item.Color = (Role.Flags.Color)2;

                            if (ClientSpell.UseSpellSoul == 0)
                                item.ITEM_ID = MsgFloorItem.MsgItemPacket.NormalDaggerStorm;
                            else if (ClientSpell.UseSpellSoul == 1)
                                item.ITEM_ID = MsgFloorItem.MsgItemPacket.SoulOneDaggerStorm;
                            else if (ClientSpell.UseSpellSoul == 2)
                                item.ITEM_ID = MsgFloorItem.MsgItemPacket.SoulTwoDaggerStorm;

                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, Attack.X, Attack.Y, MsgFloorItem.MsgItem.ItemType.Effect, 0, user.Player.DynamicID, user.Player.Map
                                   , user.Player.UID, false, user.Map, 3);
                            DropItem.OwnerEffert = user;
                            DropItem.DBSkill = DBSpell;

                            if (user.Map.EnqueueItem(DropItem))
                            {
                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);
                            }
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                }
            }
        }
    }
}
