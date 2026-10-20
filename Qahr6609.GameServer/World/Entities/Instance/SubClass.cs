using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{
   public class SubClass
   {
       public Database.DBLevExp.Sort ActiveSubclass;
       public ushort StudyPoints = 0;
       public ConcurrentDictionary<Database.DBLevExp.Sort, Game.MsgServer.MsgSubClass.SubClases> src;
       public SubClass()
       {
           src = new ConcurrentDictionary<Database.DBLevExp.Sort, Game.MsgServer.MsgSubClass.SubClases>();
       }
       public void CreateSpawn(Client.GameClient client)
       {
           client.Player.ActiveSublass = ActiveSubclass;
           client.Player.SubClassHasPoints = GetHashPoint();
       }
       public uint GetHashPoint()
       {
           uint num = 0;
           foreach (var item in src.Values)
           {
               num += (uint)(Math.Pow(10, (byte)(item.ID - 1)) * item.Phrase);
           }
           return num;
       }

       public void UpdateStatus(Client.GameClient client)
       {
           foreach (var item in src.Values)
           {
               switch (item.ID)
               {
                   case Database.DBLevExp.Sort.Apothecary:
                       {
                           client.Status.Detoxication += Database.Server.SubClassInfo.GetDamage(item.ID, item.Phrase)[0];
                           break;
                       }
                   case Database.DBLevExp.Sort.ChiMaster:
                       {
                           client.Status.Immunity += (uint)(Database.Server.SubClassInfo.GetDamage(item.ID, item.Phrase)[0] * 100);
                           break;
                       }
                   case Database.DBLevExp.Sort.MartialArtist:
                       {
                           client.Status.CriticalStrike += (uint)(Database.Server.SubClassInfo.GetDamage(item.ID, item.Phrase)[0] * 100);
                           break;
                       }
                   case Database.DBLevExp.Sort.Performer:
                       {
                           client.Status.MagicAttack += (uint)(Database.Server.SubClassInfo.GetDamage(item.ID, item.Phrase)[0]);
                           client.Status.MaxAttack += (uint)(Database.Server.SubClassInfo.GetDamage(item.ID, item.Phrase)[1]);
                           client.Status.MinAttack += (uint)(Database.Server.SubClassInfo.GetDamage(item.ID, item.Phrase)[1]);
                           break;
                       }
                   case Database.DBLevExp.Sort.Sage:
                       {
                           client.Status.Penetration += (uint)(Database.Server.SubClassInfo.GetDamage(item.ID, item.Phrase)[0] * 100);
                           break;
                       }
                   case Database.DBLevExp.Sort.Warlock:
                       {
                           client.Status.SkillCStrike += (uint)(Database.Server.SubClassInfo.GetDamage(item.ID, item.Phrase)[0] * 100);
                           break;
                       }
                   case Database.DBLevExp.Sort.Wrangler:
                       {
                           client.Status.MaxHitpoints += Database.Server.SubClassInfo.GetDamage(item.ID, item.Phrase)[0];
                           break;
                       }
               }
           }
       }
       public bool LearnNewSubClass(Database.DBLevExp.Sort type, Client.GameClient user, ServerSockets.Packet stream)
       {
          return user.Player.SubClass.SendLearn(user, Game.MsgServer.MsgSubClass.SubClases.Create(type), stream);
       }
       public void SetPhrase(Database.DBLevExp.Sort type, byte phrase, Client.GameClient user, ServerSockets.Packet stream)
       {
           Game.MsgServer.MsgSubClass.SubClases record;
           if (src.TryGetValue(type, out record))
           {
               record.Phrase = phrase;
               user.Player.SubClass.SendPromoted(user, record, stream);
               user.Equipment.QueryEquipment(user.Equipment.Alternante);
           }
       }
       public byte GetClassLevel(Database.DBLevExp.Sort type)
       {
           Game.MsgServer.MsgSubClass.SubClases record;
           if (src.TryGetValue(type, out record))
           {
               return record.Level;
           }
           return 0;
       }
       public byte GetClassPhrase(Database.DBLevExp.Sort type)
       {
           Game.MsgServer.MsgSubClass.SubClases record;
           if (src.TryGetValue(type, out record))
           {
               return record.Phrase;
           }
           return 0;
       }
       public unsafe void AddStudyPoints(Client.GameClient client, ushort Study, ServerSockets.Packet stream)
       {
           StudyPoints = (ushort)Math.Min(ushort.MaxValue, (int)(StudyPoints + Study));
           client.SendSysMesage("You received " + Study.ToString() + " StudyPoints.", MsgMessage.ChatMode.System);
        
           UpdateStudyPacket(client,stream);
       }
       public void RemoveStudy(Client.GameClient client, ushort Study, ServerSockets.Packet stream)
       {
           if (StudyPoints > Study)
           {
               StudyPoints -= Study;
               UpdateStudyPacket(client,stream);
           }
       }
       public unsafe void UpdateStudyPacket(Client.GameClient client,ServerSockets.Packet stream)
       {
           client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { "zf2-e300" });

           client.Send(stream.SubClassCreate(MsgSubClass.Action.Animation, StudyPoints, 0, null));

       }
       public unsafe bool SendLearn(Client.GameClient client, Game.MsgServer.MsgSubClass.SubClases item, ServerSockets.Packet stream)
       {
           if (!src.ContainsKey(item.ID))
           {
               src.TryAdd(item.ID, item);

               client.Send(stream.SubClassCreate(MsgSubClass.Action.LearnSubClass, item.ID, item.Level, 0));
               return true;
           }
           return false;
       }
       public unsafe void SendPromoted(Client.GameClient client, Game.MsgServer.MsgSubClass.SubClases item, ServerSockets.Packet stream)
       {

           client.Send(stream.SubClassCreate(MsgSubClass.Action.MartialPromoted, item.ID, item.Phrase, 0));

       }
       public unsafe void ShowGui(Client.GameClient client, ServerSockets.Packet stream)
       {
           client.Send(stream.SubClassCreate(MsgSubClass.Action.ShowGUI, StudyPoints, 0, src.Values.ToArray()));
       }

       public override string ToString()
       {
           Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('#');
           writer.Add(StudyPoints).Add((byte)ActiveSubclass).Add(src.Count);
           foreach (var prof in src.Values)
               writer.Add((byte)prof.ID).Add(prof.Level).Add(prof.Phrase);
           return writer.Close();
       }
       public void Load(string Base_line)
       {
           if (Base_line == "" || Base_line == null) return;

           Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(Base_line, '#');
           StudyPoints = reader.Read((ushort)0);
           ActiveSubclass = (Database.DBLevExp.Sort)reader.Read((byte)0);
           int count = reader.Read((int)0);
           for (byte x = 0; x < count; x++)
           {
               Game.MsgServer.MsgSubClass.SubClases prof = Game.MsgServer.MsgSubClass.SubClases.Create((Database.DBLevExp.Sort)reader.Read((byte)0));
               prof.Level = reader.Read((byte)0);
               prof.Phrase = reader.Read((byte)0);
               if (!src.ContainsKey(prof.ID))
                   src.TryAdd(prof.ID, prof);
           }
       }
    }
}
