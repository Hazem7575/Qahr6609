using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
   public class FlowersTable
    {
       public static void Load()
       {
             WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
             foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
             {
                 ini.FileName = fname;

                 ushort Body = ini.ReadUInt16("Character", "Body", 1002);
                 uint UID = ini.ReadUInt32("Character", "UID", 0);
                 string Name = ini.ReadString("Character", "Name", "None");
                 Role.Instance.Flowers flower = new Role.Instance.Flowers(UID, Name);
                 string FlowerArray = ini.ReadString("Character", "Flowers", "None");
                 Database.DBActions.ReadLine reader = new DBActions.ReadLine(FlowerArray, '/');
                 flower.FreeFlowers = reader.Read((uint)0);
                 foreach (var flow in flower)
                 {
                     flow.Amount = reader.Read((uint)0);
                     flow.Amount2day = reader.Read((uint)0);
                 }
                 if (flower.Sum((x) => x.Amount) != 0)
                 {
                     if (!Role.Instance.Flowers.ClientPoll.ContainsKey(UID))
                         Role.Instance.Flowers.ClientPoll.TryAdd(UID, flower);
                     if (Role.Core.IsBoy(Body))
                     {
                         foreach (var flow in flower)
                             Program.BoysFlowersRanking.UpdateRank(flow, flow.Type);
                     }
                     else if (Role.Core.IsGirl(Body))
                     {
                         foreach (var flow in flower)
                             Program.GirlsFlowersRanking.UpdateRank(flow, flow.Type);
                     }
                 }

                 
             }
             // [FastBoot, from Qahr] was GC.Collect(): a full blocking GC of every heap on each loader; the runtime collects on its own
       }
    }
}
