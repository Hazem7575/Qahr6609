using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CanUseSpell
    {
        public unsafe static bool Verified(InteractQuery Attack, Client.GameClient client , Dictionary<ushort, Database.MagicType.Magic> DBSpells
            , out MsgSpell ClientSpell, out  Database.MagicType.Magic Spell)
        {
            try
            {
                //anti proxy --------------------------
     
                if (Database.MagicType.RandomSpells.Contains((Role.Flags.SpellID)Attack.SpellID))
                {
                    if (client.Player.RandomSpell !=Attack.SpellID)
                    {
                        ClientSpell = default(MsgSpell);
                        Spell = default(Database.MagicType.Magic);
                        return false;
                    }
                    client.Player.RandomSpell = 0;
                }
                //-------------------------------------
                if (client.MySpells.ClientSpells.TryGetValue(Attack.SpellID, out ClientSpell))
                {
                    if (DBSpells.TryGetValue(ClientSpell.Level, out Spell))
                    {

                        if (Spell.Type == Database.MagicType.MagicSort.DirectAttack || Spell.Type == Database.MagicType.MagicSort.Attack)
                        {

                            if (!client.IsInSpellRange(Attack.OpponentUID, Spell.Range))
                            {
                                ClientSpell = default(MsgSpell);
                                Spell = default(Database.MagicType.Magic);
                                return false;
                            }
                        }
                        /*if (Program.BlockAttackMap.Contains(client.Player.Map) && client.Player.DynamicID == 0)
                            return false;*/
                        /*if (client.InQualifier() && client.ArenaMatch.StartTimer.AddSeconds(5) > DateTime.Now)
                            return false;*/
                        uint IncreaseSpellStamina = 0;//constant
                        if (client.Player.ContainFlag(MsgUpdate.Flags.ScurvyBomb))
                            IncreaseSpellStamina = (uint)(client.Player.UseStamina + 5);

                        if (ClientSpell.UseSpellSoul > 0)
                            client.OnSoulSpell = ClientSpell.ID;
                        else
                            client.OnSoulSpell = 0;

                        if (client.Player.Map != 1039 && !client.Fake)
                        {
                            if (Spell.UseStamina + IncreaseSpellStamina > client.Player.Stamina)
                                return false;
                            else
                            {
                                if ((ushort)(Spell.UseStamina + IncreaseSpellStamina) > 0)
                                {
                                    client.Player.Stamina -= (ushort)(Spell.UseStamina + IncreaseSpellStamina);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendUpdate(stream, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                                    }
                                }
                            }
                            if (Spell.UseMana > client.Player.Mana)
                                return false;
                            else
                            {
                                if (Spell.UseMana > 0)
                                {
                                    client.Player.Mana -= Spell.UseMana;
                                }
                            }
                        }
                        if (Spell.IsSpellWithColdTime)
                        {
                            Extensions.Time32 now = Extensions.Time32.Now;
                            if (ClientSpell.ColdTime > now && ClientSpell.ID != 11030)//EagleEye
                                return false;
                            else
                            {
                                ClientSpell.IsSpellWithColdTime = true;
                                ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime);
                            }

                        }
                        return true;
                    }
                }

                ClientSpell = default(MsgSpell);
                Spell = default(Database.MagicType.Magic);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ClientSpell = default(MsgSpell);
                Spell = default(Database.MagicType.Magic);
                return false;
            }
            return false;
        }
    }
}
