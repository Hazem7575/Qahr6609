
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conquer6609.Game.Conquer6609_Work
{
    public class Halo_Flag
    {//Halo_Flag
        // This System Code By :Conquer6609 01140071296
        public class Clientt
        {
            public int FlagName;
            public int Hours;
            public long StartHalo;

            public override string ToString()
            {

                Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
                writer.Add(FlagName).Add(Hours).Add(StartHalo);
                return writer.Close();
            }
        }

        public static void AddHalo(Client.GameClient user, MsgServer.MsgUpdate.Flags flag, int Hours)
        {

            Clientt msg = new Clientt();
            msg.FlagName = (int)flag;
            msg.Hours = Hours;
            msg.StartHalo = DateTime.Now.Ticks;
            if (!user.Player.HaloPoll.ContainsKey(msg.FlagName))
            {
                user.Player.HaloPoll.TryAdd(msg.FlagName, msg);
            }
            
        }

        public static void Login(Client.GameClient user)
        {
            if (user.Player.HaloPoll.Count != 0)
            {
                foreach (var client in user.Player.HaloPoll.Values)
                {

                    if (DateTime.FromBinary(client.StartHalo).AddHours(client.Hours) < DateTime.Now)
                    {
                        Clientt T;
                        user.Player.HaloPoll.TryRemove(client.FlagName, out T);
                    }
                    else
                    {

                        DateTime receiveban = DateTime.FromBinary(client.StartHalo);
                        DateTime TimerBan = receiveban.AddHours(client.Hours);
                        TimeSpan time = TimeSpan.FromTicks(TimerBan.Ticks) - TimeSpan.FromTicks(DateTime.Now.Ticks);
                        int T = 0;
                        if (time.Days > 0 && time.Minutes > 0)
                            T = time.Seconds * time.Minutes * time.Days;
                        else if (time.Minutes > 0)
                            T = time.Seconds * time.Minutes;

                        user.Player.AddHisFlag((MsgServer.MsgUpdate.Flags)client.FlagName, T, false);

                    }

                }
            }

        }

        public static void Save(Client.GameClient user)
        {
            using (Database.DBActions.Write writer = new Database.DBActions.Write("\\Halo_Flag\\" + user.Player.UID + ".ini"))
            {
                foreach (var c in user.Player.HaloPoll.Values)
                {
                    writer.Add(c.ToString());
                }
                writer.Execute(Database.DBActions.Mode.Open);
            }
        }

        public static void Load(Client.GameClient user)
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "\\Halo_Flag\\" + user.Player.UID + ".ini"))
            {
                using (Database.DBActions.Read Reader = new Database.DBActions.Read("\\Halo_Flag\\" + user.Player.UID + ".ini"))
                {

                    if (Reader.Reader())
                    {
                        uint count = (uint)Reader.Count;
                        for (uint x = 0; x < count; x++)
                        {
                            Database.DBActions.ReadLine readline = new Database.DBActions.ReadLine(Reader.ReadString(""), '/');
                            Clientt msg = new Clientt();

                            msg.FlagName = readline.Read((int)MsgServer.MsgUpdate.Flags.Normal);
                            msg.Hours = readline.Read((int)0);
                            msg.StartHalo = readline.Read((long)0);
                            user.Player.HaloPoll.TryAdd(msg.FlagName, msg);
                        }
                    }
                }
            }
        }
    }
}

