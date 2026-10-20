using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
  public  static class MsgNameChange
    {
      public static unsafe ServerSockets.Packet NameChangeCreate(this ServerSockets.Packet stream, ActionID mode, ushort EditsCount, ushort EditsAllowed, string Name)
      {
          stream.InitWriter();

          stream.Write((ushort)mode);
          stream.Write(EditsCount);
          stream.Write(EditsAllowed);
          stream.Write(Name,16);
          stream.Finalize(GamePackets.NameChange);

          return stream;
      }
      public static unsafe void GetNameChange(this ServerSockets.Packet stream, out ActionID mode, out ushort EditsCount, out ushort EditsAllowed, out string Name)
      {
          mode = (ActionID)stream.ReadUInt16();
          EditsCount = stream.ReadUInt16();
          EditsAllowed = stream.ReadUInt16();
          Name = stream.ReadCString(16);
      }
      public enum ActionID : ushort
      {
          Request = 0,
          Success = 1,
          NameTaken = 2,
          DialogInfo = 3,
          FreeChange = 4,
      }

       public static ExecuteLogin NameChangeQueue = new ExecuteLogin();

       public const uint NameChangeCps = 810;
       public const ushort MaxEdits = 5;
       // Players allowed one free rename (the client's "free change" button), e.g. granted by a GM or after a name clash.
       static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, bool> FreeChanges = new System.Collections.Concurrent.ConcurrentDictionary<uint, bool>();
       public static void GrantFreeChange(uint uid) { FreeChanges[uid] = true; }
       public static bool HasFreeChange(uint uid) { return FreeChanges.ContainsKey(uid); }

       /// <summary>Checks that the name is free and reserves it in one step (under the NameUsed lock).</summary>
       public static bool TryReserveName(string Name)
       {
           lock (Database.Server.NameUsed)
           {
               if (Database.Server.NameUsed.Contains(Name.GetHashCode()))
                   return false;
               Database.Server.NameUsed.Add(Name.GetHashCode());
               return true;
           }
       }

       public class ExecuteLogin : ConcurrentSmartThreadQueue<ChangeObject>
       {

           public ExecuteLogin()
               : base(5)
           {
               Start(10);
           }
           public void TryEnqueue(ChangeObject obj)
           {
               base.Enqueue(obj);
           }
           protected unsafe override void OnDequeue(ChangeObject obj, int time)
           {
               try
               {
                   Client.GameClient user = obj.user;
                   string Name = obj.Name;

                   if (Name == null)
                       Name = "";
                   if (obj.mode != ActionID.DialogInfo && (Name.Contains("[GM]") || Name.Contains("[PM]") || Name.Contains("[gm]") || Name.Contains("[pm]")))
                   {
                       Database.SystemBannedAccount.AddBan(user.Player.UID, user.Player.Name, 128,"Tray Make HimSelve GM");
                       user.SendSysMesage("You Account was Banned .", Conquer6609.Game.MsgServer.MsgMessage.ChatMode.System, Conquer6609.Game.MsgServer.MsgMessage.MsgColor.white);
                       user.Socket.Disconnect();
                       return;
                    
                   }
                   switch (obj.mode)
                   {
                       case ActionID.DialogInfo:
                           {
                               // the rename window asks how many changes were used / are allowed
                               using (var rec = new ServerSockets.RecycledPacket())
                               {
                                   var stream = rec.GetStream();
                                   user.Send(stream.NameChangeCreate(ActionID.DialogInfo, user.Player.NameEditCount, MaxEdits, user.Player.Name));
                               }
                               break;
                           }
                       case ActionID.FreeChange:
                       case ActionID.Request:
                           {
                               using (var rec = new ServerSockets.RecycledPacket())
                               {
                                   var stream = rec.GetStream();
                                   // A free change (granted by the server) costs nothing and does not use up an edit.
                                   bool free = obj.Bypass;
                                   if (!free && obj.mode == ActionID.FreeChange)
                                   {
                                       if (!HasFreeChange(user.Player.UID))
                                       {
                                           user.SendSysMesage("You have no free name change.");
                                           user.Send(stream.NameChangeCreate(ActionID.DialogInfo, user.Player.NameEditCount, MaxEdits, user.Player.Name));
                                           break;
                                       }
                                       free = true;
                                   }
                                   if (!free && user.Player.ConquerPoints < NameChangeCps)
                                   {
                                       user.SendSysMesage("You need " + NameChangeCps + " CPs to change your name.");
                                       break;
                                   }
                                   if (!free && user.Player.NameEditCount >= MaxEdits)
                                   {
                                       user.SendSysMesage("You have already changed your name " + MaxEdits + " times.");
                                       break;
                                   }
                                   if (Name != null && Name != "" && Name.Length >= 4 && Program.NameStrCheck(Name) && TryReserveName(Name))
                                   {
                                       if (free)
                                       {
                                           bool ignored;
                                           FreeChanges.TryRemove(user.Player.UID, out ignored);
                                       }
                                       else
                                           user.Player.NameEditCount += 1;
                                       user.Send(stream.NameChangeCreate(ActionID.Success, user.Player.NameEditCount, MaxEdits, Name));

                                       MsgMessage messaj = new MsgMessage(user.Player.Name + " changed the name to be "+Name+" .The new name will be registered, immediately.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System);
                                     

                                       MyConsole.WriteLine(user.Player.Name + ", Changed He's/Hers Name to " + Name + ".");
                                       Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));

                                       Role.Instance.Nobility Nobility;
                                       if (Program.NobilityRanking.TryGetValue(user.Player.UID, out Nobility))
                                       {
                                           Nobility.Name = Name;
                                           user.Send(stream.NobilityIconCreate(Nobility));
                                       }
                                      
                                       if (user.Player.MyChi != null)
                                       {
                                           user.Player.MyChi.Name = Name;
                                           foreach (var chi in user.Player.MyChi)
                                               chi.Name = Name;
                                       }
                                       if (user.Player.MyJiangHu != null)
                                           user.Player.MyJiangHu.Name = Name;
                                       if (user.Player.InnerPower != null)
                                           user.Player.InnerPower.Name = Name;
                                       if (user.Player.MyGuild != null && user.Player.MyGuildMember != null)
                                       {
                                           user.Player.MyGuildMember.Name = Name;
                                           if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                                           {
                                               user.Player.MyGuild.Info.LeaderName = Name;
                                           }
                                       }
                                       if (user.Player.MyClan != null && user.Player.MyClanMember != null)
                                       {
                                           user.Player.MyClanMember.Name = Name;
                                           if (user.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                                               user.Player.MyClan.LeaderName = Name;
                                       }
                                       if (user.ArenaStatistic != null)
                                           user.ArenaStatistic.Name = Name;
                                       if (user.TeamArenaStatistic != null)
                                           user.TeamArenaStatistic.Name = Name;
                                       if (user.Player.Flowers != null)
                                       {
                                           foreach (var flower in user.Player.Flowers)
                                               if (flower != null)
                                                   flower.Name = Name;
                                       }

                                       user.Player.Name = Name;
                                       user.Player.View.ReSendView(stream);

                                       if (user.Player.Spouse != null)
                                       {
                                           Client.GameClient spouse;
                                           if (Database.Server.GamePoll.TryGetValue(user.Player.SpouseUID, out spouse))
                                           {
                                               spouse.Player.Spouse = Name;
                                               spouse.Player.SendString(stream, MsgStringPacket.StringID.Spouse, false, Name);
                                           }
                                           else
                                           {
                                               user.ClientFlag |= Client.ServerFlag.UpdateSpouse;
                                               Database.ServerDatabase.LoginQueue.Enqueue(user);
                                           }
                                       }
                                       if (!free)
                                           user.Player.ConquerPoints -= NameChangeCps;
                                       // the new name was already reserved by TryReserveName

                                       try
                                       {
                                           Role.Instance.Associate.MyAsociats MyAsociation;
                                           if (Role.Instance.Associate.Associates.TryGetValue(user.Player.UID, out MyAsociation))
                                           {
                                               foreach (var table in MyAsociation.Associat.Values)
                                               {
                                                   if (table == null)
                                                       continue;
                                                   if (table.Values == null)
                                                       continue;
                                                   foreach (var _ass in table.Values)
                                                   {
                                                       if (_ass == null)
                                                           continue;
                                                       Role.Instance.Associate.MyAsociats associat;
                                                       if (Role.Instance.Associate.Associates.TryGetValue(_ass.UID, out associat))
                                                       {
                                                           foreach (var _table in associat.Associat)
                                                           {

                                                               if (_table.Value == null)
                                                                   continue;
                                                               Role.Instance.Associate.Member my_relation;
                                                               if (_table.Value.TryGetValue(user.Player.UID, out my_relation))
                                                                   my_relation.Name = Name;
                                                               if (associat.MyClient != null)//online.
                                                               {
                                                                   if (_table.Key == (byte)Role.Instance.Associate.Friends)
                                                                   {
                                                                       associat.MyClient.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.RemovePerson, user.Player.UID, true, "", (uint)user.Player.NobilityRank, user.Player.Body));
                                                                       associat.MyClient.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddFriend, user.Player.UID, true, Name, (uint)user.Player.NobilityRank, user.Player.Body));
                                                                   }
                                                                   else if (_table.Key == (byte)Role.Instance.Associate.Partener)
                                                                   {
                                                                       associat.MyClient.Send(stream.TradePartnerCreate(user.Player.UID, MsgTradePartner.Action.BreakPartnership, true, 0, ""));
                                                                       associat.MyClient.Send(stream.TradePartnerCreate(user.Player.UID, MsgTradePartner.Action.AddPartner, true, _ass.GetTimerLeft(), Name));
                                                                   }
                                                               }
                                                           }
                                                       }
                                                   }
                                               }
                                           }

                                       }
                                       catch (Exception e)
                                       {
                                           MyConsole.SaveException(e);
                                       }
                                   }
                                   else
                                       user.Send(stream.NameChangeCreate(ActionID.NameTaken, 0, 0, Name));
                               }
                               break;
                           }
                   }
               }
               catch (Exception e)
               {
                   MyConsole.SaveException(e);
               }
           }
       }
       public class ChangeObject
       {
           public Client.GameClient user;
           public ActionID mode;
           public bool Bypass = false;
           public ushort EditsCount;
           public ushort EditsAllowed;
           public string Name = "";
       }

      [PacketAttribute(GamePackets.NameChange)]
      public unsafe static void Proces(Client.GameClient user, ServerSockets.Packet stream)
       {
           if (!user.Player.OnMyOwnServer)
               return;
          ChangeObject obj = new ChangeObject();
          obj.user = user;
          stream.GetNameChange(out obj.mode, out  obj.EditsCount, out  obj.EditsAllowed, out  obj.Name);
          NameChangeQueue.TryEnqueue(obj);
      }
      public unsafe static void ChangeName(Client.GameClient user, string name, bool bypass = false)
      {
          ChangeObject obj = new ChangeObject();
          obj.user = user;
          obj.Bypass = bypass;
          obj.Name = name;
          obj.mode = ActionID.Request;
          NameChangeQueue.TryEnqueue(obj);
      }
    }
}
