using Conquer6609.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.CheckAttack
{
   public class CanAttackNpc
    {
       public static bool Verified(Client.GameClient client, Role.SobNpc attacked
    , Database.MagicType.Magic DBSpell)
       {
           if (attacked.Map == MsgTournaments.MsgFootball.MapID)
               return true;
           if (attacked.HitPoints == 0)
               return false;
           if (attacked.IsStatue)
           {
               if (attacked.HitPoints == 0)
                   return false;
               if (client.Player.PkMode == Role.Flags.PKMode.PK)
                   return true;
               else
                   return false;
           }
           if (attacked.UID == 890)
           {
               if (client.Player.MyClan == null)
                   return false;
               var tournament = Game.MsgTournaments.MsgSchedules.ClanWar.GetWar(client);// all clan war maps run at once
               if (tournament == null)
                   return false;

               if(!tournament.InWar(client))
                   return false;
               if (tournament.Winner == null)
                   return false;
               if ( tournament.Winner.ClainID == client.Player.ClanUID)
                   return false;
            
           }
           var guildWar = Game.MsgTournaments.MsgSchedules.GuildWar;
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(guildWar.Furnitures, Role.SobNpc.StaticMesh.LeftGate)
               || attacked.UID == Conquer6609.Role.FurnitureUid.Of(guildWar.Furnitures, Role.SobNpc.StaticMesh.RightGate))
           {
               // gates: only during the war, only from inside the Guild Area, never by the guild that holds it
               if (client.Player.Map != attacked.Map || client.Player.MyGuild == null)
                   return false;
               if (guildWar.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
               if (client.Player.GuildID == guildWar.Winner.GuildID)
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (client.Player.MyGuild == null)
                   return false;
               if (client.Player.Map != attacked.Map)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Idle)
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures, MsgTournaments.MsgSuperGuildWar.FurnituresType.Pole))
           {
               if (client.Player.MyGuild == null)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.Pole].HitPoints == 0)
                   return false;
               if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Idle)
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (client.Player.MyGuild == null)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.EliteGuildWar.Winner.GuildID)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.EliteGuildWar.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.EliteGuildWar.Proces == MsgTournaments.ProcesType.Idle)
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgSchedules.GuildWarScore.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (client.Player.MyGuild == null)
                   return false;
               if (MsgSchedules.GuildWarScore.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (MsgSchedules.GuildWarScore.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
           }
           #region NobilityPole
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgNobilityPole.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgNobilityPole.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgNobilityPole.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgNobilityPole.Proces == MsgTournaments.ProcesType.Dead)//MsgNobilityPole
                   return false;
               if (client.Player.NobilityRank != Role.Instance.Nobility.NobilityRank.King)
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgNobilityPole1.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgNobilityPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgNobilityPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgNobilityPole1.Proces == MsgTournaments.ProcesType.Dead)//MsgNobilityPole
                   return false;
               if (client.Player.NobilityRank != Role.Instance.Nobility.NobilityRank.Prince)
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgNobilityPole2.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgNobilityPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgNobilityPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgNobilityPole2.Proces == MsgTournaments.ProcesType.Dead)//MsgNobilityPole
                   return false;
               if (client.Player.NobilityRank != Role.Instance.Nobility.NobilityRank.Duke)
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgNobilityPole3.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgNobilityPole3.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgNobilityPole3.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgNobilityPole3.Proces == MsgTournaments.ProcesType.Dead)//MsgNobilityPole
                   return false;
               if (client.Player.NobilityRank != Role.Instance.Nobility.NobilityRank.Earl)
                   return false;
           }
           #endregion
           #region class pole war
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgWindWalker.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgWindWalker.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgWindWalker.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (client.Player.Class != 165)
                   return false;
               if (MsgTournaments.MsgWindWalker.Proces == MsgTournaments.ProcesType.Dead)
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgKungfuKing.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgKungfuKing.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgKungfuKing.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (client.Player.Class != 85)
                   return false;
               if (MsgTournaments.MsgKungfuKing.Proces == MsgTournaments.ProcesType.Dead)
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgArcherClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgArcherClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgArcherClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (client.Player.Class != 45)
                   return false;
               if (MsgTournaments.MsgArcherClass.Proces == MsgTournaments.ProcesType.Dead)//MsgArcherClass
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgFireClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgFireClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgFireClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (client.Player.Class != 145)
                   return false;
               if (MsgTournaments.MsgFireClass.Proces == MsgTournaments.ProcesType.Dead)//MsgFireClass
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgMonkClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgMonkClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgMonkClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (client.Player.Class != 65)
                   return false;
               if (MsgTournaments.MsgMonkClass.Proces == MsgTournaments.ProcesType.Dead)//MsgMonkClass
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgNinjaClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgNinjaClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgNinjaClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (client.Player.Class != 55)
                   return false;
               if (MsgTournaments.MsgNinjaClass.Proces == MsgTournaments.ProcesType.Dead)//MsgNinjaClass
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgTrojanClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgTrojanClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgTrojanClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (client.Player.Class != 15)
                   return false;
               if (MsgTournaments.MsgTrojanClass.Proces == MsgTournaments.ProcesType.Dead)//MsgTrojanClass
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgWarriorClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgWarriorClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgWarriorClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (client.Player.Class != 25)
                   return false;
               if (MsgTournaments.MsgWarriorClass.Proces == MsgTournaments.ProcesType.Dead)//MsgWarriorClass
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgWaterClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgWaterClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgWaterClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (client.Player.Class != 135)
                   return false;
               if (MsgTournaments.MsgWaterClass.Proces == MsgTournaments.ProcesType.Dead)//MsgWaterClass
                   return false;
           }
           if (attacked.UID == Conquer6609.Role.FurnitureUid.Of(MsgTournaments.MsgPariteClass.Furnitures, Role.SobNpc.StaticMesh.Pole))
           {
               if (MsgTournaments.MsgPariteClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgPariteClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (client.Player.Class != 75)
                   return false;
               if (MsgTournaments.MsgPariteClass.Proces == MsgTournaments.ProcesType.Dead)//MsgWaterClass
                   return false;
           }
           #endregion
           MsgTournaments.MsgCaptureTheFlag.Basse Bas;
           if (MsgTournaments.MsgSchedules.CaptureTheFlag.Bases.TryGetValue(attacked.UID, out Bas))
           {
               if (MsgTournaments.MsgSchedules.CaptureTheFlag.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
               if (client.Player.MyGuild == null)
                   return false;
               if (Bas.Npc.HitPoints == 0)
                   return false;
               if (Bas.CapturerID == client.Player.GuildID)
                   return false;

           }
           return true;
       }
    }
}

namespace Conquer6609.Role
{
    /// <summary>UID of an event furniture (pole/gate) or 0 when its SobNpc row is missing. Event poles whose rows were
    /// removed from SobNpcs.txt (GuildWarScore 8447, nobility poles 22340-22343, ...) left null entries, and the
    /// NullReferenceException in CanAttackNpc blocked every attack on any SobNpc checked after them (Guild War pole too).</summary>
    public static class FurnitureUid
    {
        public static uint Of<TKey>(System.Collections.Generic.IDictionary<TKey, SobNpc> furnitures, TKey key)
        {
            SobNpc npc;
            if (furnitures == null || !furnitures.TryGetValue(key, out npc) || npc == null)
                return 0;
            return npc.UID;
        }
    }
}
