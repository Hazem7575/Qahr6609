using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Conquer6609.Database
{
   public class DataCore
    {
       public static AtributesStatus AtributeStatus = new AtributesStatus();

       public static void SetCharacterSides(Role.Player Player)
       {
           byte clas = (byte)(Player.Class / 10);
           #region SetFace
           switch (Player.Body)
           {
               #region Boy
               case 1004:
               case 1003:
                   {
                       switch (clas)
                       {
                           case 1:
                           case 2:
                           case 10:
                           case 4:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(1, 102);
                                   break;
                               }
                           case 5:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(103, 107);
                                   break;
                               }
                           case 6:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(109, 133);
                                   break;
                               }
                           case 7:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(154, 158);
                                   break;
                               }
                           case 8:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(164, 168);
                                   break;
                               }
                           case 16:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(174, 178);
                                   break;
                               }
                           default:
                               {
                                   Player.Face = 296;
                                   break;
                               }
                       }
                       break;
                   }
               #endregion
               #region Girl
               case 2001:
               case 2002:
                   {
                       switch (clas)
                       {
                           case 1:
                           case 2:
                           case 10:
                           case 4:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(201, 290);
                                   break;
                               }
                           case 5:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(291, 295);
                                   break;
                               }
                           case 6:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(300, 304);
                                   break;
                               }
                           case 7:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(345, 349);
                                   break;
                               }
                           case 8:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(355, 359);
                                   break;
                               }
                           case 16:
                               {
                                   Player.Face = (ushort)Program.GetRandom.Next(365, 369);
                                   break;
                               }
                           default:
                               {
                                   Player.Face = 296;
                                   break;
                               }
                       }
                       break;
                   }
               default:
                   break;
               #endregion
           }
           #endregion
       }
       public static void LoadClient(Role.Player player)
       {

           player.Owner.Inventory = new Role.Instance.Inventory(player.Owner);
           player.Owner.Equipment = new Role.Instance.Equip(player.Owner);
           player.Owner.Warehouse = new Role.Instance.Warehouse(player.Owner);
           player.Owner.MyProfs = new Role.Instance.Proficiency(player.Owner);
           player.Owner.MySpells = new Role.Instance.Spell(player.Owner);

           if (player.Owner.Achievement == null)
               player.Owner.Achievement = new AchievementCollection();

           player.Achievement = new Game.MsgServer.ClientAchievement(player.Owner.Achievement.Value, player.UID);
       
       }
    }
}
