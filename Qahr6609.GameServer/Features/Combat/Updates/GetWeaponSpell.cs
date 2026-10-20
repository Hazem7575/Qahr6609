using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.Updates
{
    public class GetWeaponSpell
    {
        public static void CheckExtraEffects(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client.Equipment.RingEffect != Role.Flags.ItemEffect.None)
            {
                if (Calculate.Base.Rate(20))
                {
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.Stigma))
                    {
                        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID
                   , 0, client.Player.X, client.Player.Y, (ushort)Role.Flags.SpellID.Stigma
                   , 4, 0);


                        client.Player.AddSpellFlag(MsgUpdate.Flags.Stigma, 20, true);
                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None));


                        MsgSpell.SetStream(stream);
                        MsgSpell.Send(client);
                    }
                }
            }
            if (client.Equipment.NecklaceEffect != Role.Flags.ItemEffect.None)
            {
                if (Calculate.Base.Rate(20))
                {
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.Shield))
                    {
                        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID
                   , 0, client.Player.X, client.Player.Y, (ushort)Role.Flags.SpellID.Shield
                   , 4, 0);


                        client.Player.AddSpellFlag(MsgUpdate.Flags.Shield, 15, true);
                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None));


                        MsgSpell.SetStream(stream);
                        MsgSpell.Send(client);
                    }
                }
            }
        }
        // [Gameplay] ProcStampFix (default 0 = original behaviour). The original code clears AttackStamp
        // before checking whether a proc spell exists, which lets some hits skip the attack-speed wait.
        // 1 = CastProc: the stamp is cleared only when a proc is actually cast, and put back when
        // ProcescMagic rejects the cast (it sets the stamp itself when it accepts).
        public static readonly bool ProcStampFix = Conquer6609.Framework.Settings.Int("Gameplay", "ProcStampFix", 0) != 0;
        private static void CastProc(Client.GameClient client, ServerSockets.Packet stream, InteractQuery AttackPaket)
        {
            Extensions.Time32 previousStamp = client.Player.AttackStamp;
            client.Player.AttackStamp = new Extensions.Time32();
            MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket);
            if (client.Player.AttackStamp.Value == 0)
                client.Player.AttackStamp = previousStamp;
        }
        public unsafe static bool Check(InteractQuery Attack, ServerSockets.Packet stream, Client.GameClient client, Role.IMapObj Target)
        {
         
           if (Calculate.Base.Rate(50))
            {
                if (client.Equipment.RightWeapon != 0)
                {

                    if (Database.AtributesStatus.IsMonk(client.Player.Class))
                        {
                            MsgGameItem GameItem;
                            MsgGameItem GameItem1;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out GameItem1))
                            {
                                if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out GameItem))
                                {
                                    if (Database.ItemType.IsMonkWeapon(GameItem.ITEM_ID) && Database.ItemType.IsMonkWeapon(GameItem1.ITEM_ID))
                                    {
                                        List<ushort> CanUse = new List<ushort>();
                                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleAttack))
                                            CanUse.Add((ushort)Role.Flags.SpellID.TripleAttack);

                                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Strike))
                                            CanUse.Add((ushort)Role.Flags.SpellID.Strike);
                                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DownSweep))
                                            CanUse.Add((ushort)Role.Flags.SpellID.DownSweep);
                                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.UpSweep))
                                            CanUse.Add((ushort)Role.Flags.SpellID.UpSweep);
                                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WrathoftheEmperor))
                                            CanUse.Add((ushort)Role.Flags.SpellID.WrathoftheEmperor);

                                        if (!ProcStampFix) client.Player.AttackStamp = new Extensions.Time32();

                                        if (CanUse.Count > 0)
                                        {
                                            InteractQuery AttackPaket = new InteractQuery();
                                            AttackPaket.OpponentUID = Attack.OpponentUID;
                                            AttackPaket.UID = Attack.UID;
                                            AttackPaket.X = Target.X;
                                            AttackPaket.Y = Target.Y;
                                            AttackPaket.SpellID = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];
                                            client.Player.RandomSpell = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];

                                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                                            if (ProcStampFix) CastProc(client, stream, AttackPaket); else MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket);
                                            return true;
                                        }
                                        else
                                            return false;
                                      
                                    }
                                }
                            }
                         
                        }
                    else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                    {
                        List<ushort> CanUse = new List<ushort>();
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AngerofStomper))
                            CanUse.Add((ushort)Role.Flags.SpellID.AngerofStomper);

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HorrorofStomper))
                            CanUse.Add((ushort)Role.Flags.SpellID.HorrorofStomper);
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PeaceofStomper))
                            CanUse.Add((ushort)Role.Flags.SpellID.PeaceofStomper);
                     
                        if (!ProcStampFix) client.Player.AttackStamp = new Extensions.Time32();

                        if (CanUse.Count > 0)
                        {
                            InteractQuery AttackPaket = new InteractQuery();
                            AttackPaket.OpponentUID = Attack.OpponentUID;
                            AttackPaket.UID = Attack.UID;
                            AttackPaket.X = Target.X;
                            AttackPaket.Y = Target.Y;
                            AttackPaket.SpellID = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];
                            client.Player.RandomSpell = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];

                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                            if (ProcStampFix) CastProc(client, stream, AttackPaket); else MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket);
                            return true;
                        }
                        else
                            return false;
                    }
                    else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                    {

                        List<ushort> CanUse = new List<ushort>();
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.LeftHook))
                            CanUse.Add((ushort)Role.Flags.SpellID.LeftHook);
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RightHook))
                            CanUse.Add((ushort)Role.Flags.SpellID.RightHook);
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScarofEarth))
                            CanUse.Add((ushort)Role.Flags.SpellID.ScarofEarth);
                        if (!ProcStampFix) client.Player.AttackStamp = new Extensions.Time32();

                        if (CanUse.Count > 0)
                        {
                            InteractQuery AttackPaket = new InteractQuery();
                            AttackPaket.OpponentUID = Attack.OpponentUID;
                            AttackPaket.UID = Attack.UID;
                            AttackPaket.X = Target.X;
                            AttackPaket.Y = Target.Y;
                            AttackPaket.SpellID = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];
                            client.Player.RandomSpell = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];

                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                            if (ProcStampFix) CastProc(client, stream, AttackPaket); else MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket);
                            return true;
                        }
                        else
                            return false;

                    }
                    else if (Database.AtributesStatus.IsLee(client.Player.Class))
                    {

                        List<ushort> CanUse = new List<ushort>();
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirStrike))
                            CanUse.Add((ushort)Role.Flags.SpellID.AirStrike);
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthSweep))
                            CanUse.Add((ushort)Role.Flags.SpellID.EarthSweep);
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Kick))
                            CanUse.Add((ushort)Role.Flags.SpellID.Kick);
                        if (!ProcStampFix) client.Player.AttackStamp = new Extensions.Time32();

                        if (CanUse.Count > 0)
                        {
                            InteractQuery AttackPaket = new InteractQuery();
                            AttackPaket.OpponentUID = Attack.OpponentUID;
                            AttackPaket.UID = Attack.UID;
                            AttackPaket.X = Target.X;
                            AttackPaket.Y = Target.Y;
                            AttackPaket.SpellID = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];
                            client.Player.RandomSpell = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];

                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                            if (ProcStampFix) CastProc(client, stream, AttackPaket); else MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket);
                            return true;
                        }
                        else
                            return false;

                    }
                    else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                    {

                        List<ushort> CanUse = new List<ushort>();
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Windstorm))
                            CanUse.Add((ushort)Role.Flags.SpellID.Windstorm);
                        if (!ProcStampFix) client.Player.AttackStamp = new Extensions.Time32();

                        if (CanUse.Count > 0)
                        {
                            InteractQuery AttackPaket = new InteractQuery();
                            AttackPaket.OpponentUID = Attack.OpponentUID;
                            AttackPaket.UID = Attack.UID;
                            AttackPaket.X = Target.X;
                            AttackPaket.Y = Target.Y;
                            AttackPaket.SpellID = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];
                            client.Player.RandomSpell = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];

                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                            if (ProcStampFix) CastProc(client, stream, AttackPaket); else MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket);
                            return true;
                        }
                        else
                            return false;

                    }

                    if (client.Equipment.RightWeaponEffect != Role.Flags.ItemEffect.None && Target.ObjType != Role.MapObjectType.SobNpc)
                    {
                        if (Calculate.Base.Rate(20) && !client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                        {
                            if (!ProcStampFix) client.Player.AttackStamp = new Extensions.Time32();

                            InteractQuery AttackPaket = new InteractQuery();

                            AttackPaket.OpponentUID = Attack.OpponentUID;
                            AttackPaket.UID = Attack.UID;
                            AttackPaket.X = Target.X;
                            AttackPaket.Y = Target.Y;

                            if (client.Equipment.RightWeaponEffect == Role.Flags.ItemEffect.Poison)
                                AttackPaket.SpellID = (ushort)Role.Flags.SpellID.Poison;
                            else if (client.Equipment.RightWeaponEffect == Role.Flags.ItemEffect.MP)
                                AttackPaket.SpellID = (ushort)Role.Flags.SpellID.EffectMP;

                            client.Player.RandomSpell = AttackPaket.SpellID;

                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;

                            if (ProcStampFix) CastProc(client, stream, AttackPaket); else MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket);

                            return true;
                        }
                    }
                    if (Database.ItemType.IsTrojanEpicWeapon(client.Equipment.RightWeapon))
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalStrike))
                        {
                            if (!ProcStampFix) client.Player.AttackStamp = new Extensions.Time32();
                            InteractQuery AttackPaket = new InteractQuery();
                            AttackPaket.OpponentUID = Attack.OpponentUID;
                            AttackPaket.UID = Attack.UID;
                            AttackPaket.X = Target.X;
                            AttackPaket.Y = Target.Y;
                            AttackPaket.SpellID = (ushort)Role.Flags.SpellID.MortalStrike;
                            client.Player.RandomSpell = (ushort)Role.Flags.SpellID.MortalStrike;

                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                            if (ProcStampFix) CastProc(client, stream, AttackPaket); else MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket);

                            return true;
                        }
                        return false;
                    }
                    uint rightWeapon = client.Equipment.RightWeapon;
                    ushort wep1subyte = (ushort)(rightWeapon / 1000), wep2subyte = 0;
                    bool doWep1Spell = false, doWep2Spell = false;
                    ushort wep1spellid = 0, wep2spellid = 0;
                    if (wep1subyte == 421)
                        wep1subyte--;
                    if (Database.Server.WeaponSpells.ContainsKey(wep1subyte))
                        wep1spellid = Database.Server.WeaponSpells[wep1subyte];
                    if (client.MySpells.ClientSpells.ContainsKey(wep1spellid))
                        doWep1Spell = Calculate.Base.Rate(50);


                    if (!doWep1Spell)
                    {
                        if (client.Equipment.LeftWeapon != 0)
                        {
                            uint leftWeapon = client.Equipment.LeftWeapon;
                            wep2subyte = (ushort)(leftWeapon / 1000);
                            if (Database.Server.WeaponSpells.ContainsKey(wep2subyte))
                                wep2spellid = Database.Server.WeaponSpells[wep2subyte];
                            if (client.MySpells.ClientSpells.ContainsKey(wep2spellid))
                                doWep2Spell = true;

                            if (doWep2Spell)
                            {
                                if (!client.MySpells.ClientSpells.ContainsKey(wep2spellid))
                                    return false;

                                if (client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                                {
                                    client.Shift(Target.X, Target.Y, stream);
                                }
                                MsgServer.MsgAttackPacket.CreateAutoAtack(Attack, client);
                                InteractQuery AttackPaket = new InteractQuery();
                                AttackPaket.OpponentUID = Attack.OpponentUID;
                                AttackPaket.UID = Attack.UID;
                                AttackPaket.X = Target.X;
                                AttackPaket.Y = Target.Y;
                                AttackPaket.SpellID = wep2spellid;
                                AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                                client.Player.RandomSpell = wep2spellid;
                                MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);

                                return true;
                            }
                            else
                            {
                                doWep1Spell = true;
                            }
                        }
                        else
                        {
                            doWep1Spell = true;
                        }
                        
                    }
                    if (doWep1Spell)
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey(wep1spellid))
                            return false;

                        if (client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                        {
                            client.Shift(Target.X, Target.Y, stream);
                        }

                        MsgServer.MsgAttackPacket.CreateAutoAtack(Attack, client);
                        InteractQuery AttackPaket = new InteractQuery();
                        AttackPaket.OpponentUID = Attack.OpponentUID;
                        AttackPaket.UID = Attack.UID;
                        AttackPaket.X = Target.X;
                        AttackPaket.Y = Target.Y;
                        AttackPaket.SpellID = wep1spellid;
                        client.Player.RandomSpell = wep1spellid;
                        AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                        MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket,true);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
