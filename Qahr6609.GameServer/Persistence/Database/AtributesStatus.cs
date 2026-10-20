using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class AtributesStatus : Dictionary<byte, Dictionary<ushort, AtributesStatus.Instant>>
    {
        public class Instant
        {
            public ushort Strenght = 0;
            public ushort Vitality = 0;
            public ushort Agility = 0;
            public ushort Spirit;
        }

        public static bool IsWindWalker(byte Job) { return Job >= 160 && Job <= 165; }
        public static bool IsTrojan(byte Job) { return Job >= 10 && Job <= 15; }
        public static bool IsWarrior(byte Job) { return Job >= 20 && Job <= 25; }
        public static bool IsArcher(byte Job) { return Job >= 40 && Job <= 45; }
        public static bool IsNinja(byte Job) { return Job >= 50 && Job <= 55; }
        public static bool IsMonk(byte Job) { return Job >= 60 && Job <= 65; }
        public static bool IsPirate(byte Job) { return Job >= 70 && Job <= 75; }
        public static bool IsLee(byte Job) { return Job >= 80 && Job <= 85; }
        public static bool IsWater(byte Job) { return Job >= 132 && Job <= 135; }
        public static bool IsFire(byte Job) { return Job >= 142 && Job <= 145; }
        public static bool IsTaoist(byte Job) { return Job >= 100 && Job <= 145; }

        private byte PositionArray(byte Class)
        {
            if (Class >= 10 && Class <= 15)
                return 15;
            else if (Class >= 20 && Class <= 25)
                return 25;
            else if (Class >= 40 && Class <= 45)
                return 45;
            else if (Class >= 50 && Class <= 55)
                return 55;
            else if (Class >= 60 && Class <= 65)
                return 65;
            else if (Class >= 70 && Class <= 75)
                return 75;
            else if (Class >= 80 && Class <= 85)
                return 85;
            else if (Class >= 100 && Class <= 145)
                return 100;
            else if (Class >= 160 && Class <= 165)
                return 165;

            return 0;
        }
        public void ResetStatsNonReborn(Role.Player player)
        {
            var clas_stast = this[PositionArray(player.Class)];
            Instant stat = clas_stast[(byte)Math.Min(120, (int)player.Level)];
            player.Strength = stat.Strenght;
            player.Vitality = stat.Vitality;
            player.Spirit = stat.Spirit;
            player.Agility = stat.Agility;
            
        }
        public bool CheckStatus(Role.Player player)
        {
            var clas_stast = this[PositionArray(player.Class)];
            Instant stat = clas_stast[(byte)Math.Min(120, (int)player.Level)];
            return (player.Strength >= stat.Strenght && player.Agility >= stat.Agility && player.Spirit >= stat.Spirit && player.Vitality >= stat.Vitality) || player.Reborn > 0;
        }
        public string InfoStr(byte Class, byte Level)
        {
            var clas_stast = this[PositionArray(Class)];
            Instant stat = clas_stast[(byte)Math.Min(120, (int)Level)];
            return "You need to have Strenght : " + stat.Strenght + ", Agility : " + stat.Agility + ", Vitality: " + stat.Vitality + ", Spirit: " + stat.Spirit + "";
            
        }
        public void GetStatus(Role.Player player,bool Atreborn =false)
        {
            if (!Atreborn)
            {
                if (player.Level > 120) return;
                if (player.Reborn > 0) return;
            }
            if (player.Class >= 10 && player.Class <= 15)
            {
                var clas_stast = this[15];
                Instant stat = clas_stast[player.Level];
                player.Strength = stat.Strenght;
                player.Agility = stat.Agility;
                player.Spirit = stat.Spirit;
                player.Vitality = stat.Vitality;
            }
            else if (player.Class >= 20 && player.Class <= 25)
            {
                var clas_stast = this[25];
                Instant stat = clas_stast[player.Level];
                player.Strength = stat.Strenght;
                player.Agility = stat.Agility;
                player.Spirit = stat.Spirit;
                player.Vitality = stat.Vitality;

            }
            else if (player.Class >= 40 && player.Class <= 45)
            {
                var clas_stast = this[45];
                Instant stat = clas_stast[player.Level];
                player.Strength = stat.Strenght;
                player.Agility = stat.Agility;
                player.Spirit = stat.Spirit;
                player.Vitality = stat.Vitality;

            }
            else if (player.Class >= 50 && player.Class <= 55)
            {

                var clas_stast = this[55];
                Instant stat = clas_stast[player.Level];
                player.Strength = stat.Strenght;
                player.Agility = stat.Agility;
                player.Spirit = stat.Spirit;
                player.Vitality = stat.Vitality;
            }
            else if (player.Class >= 60 && player.Class <= 65)
            {
                var clas_stast = this[65];
                Instant stat = clas_stast[player.Level];
                player.Strength = stat.Strenght;
                player.Agility = stat.Agility;
                player.Spirit = stat.Spirit;
                player.Vitality = stat.Vitality;

            }
            else if (player.Class >= 70 && player.Class <= 75)
            {
                var clas_stast = this[75];
                Instant stat = clas_stast[player.Level];
                player.Strength = stat.Strenght;
                player.Agility = stat.Agility;
                player.Spirit = stat.Spirit;
                player.Vitality = stat.Vitality;

            }
            else if (player.Class >= 80 && player.Class <= 85)
            {
                var clas_stast = this[85];
                Instant stat = clas_stast[player.Level];
                player.Strength = stat.Strenght;
                player.Agility = stat.Agility;
                player.Spirit = stat.Spirit;
                player.Vitality = stat.Vitality;

            }
            else if (player.Class >= 100 && player.Class <= 145)
            {
                var clas_stast = this[100];
                Instant stat = clas_stast[player.Level];
                player.Strength = stat.Strenght;
                player.Agility = stat.Agility;
                player.Spirit = stat.Spirit;
                player.Vitality = stat.Vitality;
            }
            else if (player.Class >= 160 && player.Class <= 165)
            {
                var clas_stast = this[165];
                Instant stat = clas_stast[player.Level];
                player.Strength = stat.Strenght;
                player.Agility = stat.Agility;
                player.Spirit = stat.Spirit;
                player.Vitality = stat.Vitality;
            }
        }




        public void Load()
        {
            string[] baseplusText = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "Stats.ini");
            foreach (string line in baseplusText)
            {
                if (line.StartsWith("Monk"))
                {
                    string[] lin = line.Split(']');
                    byte level = byte.Parse(lin[0].Remove(0, 5));

                    string data = lin[1].Remove(0, 1);
                    string[] ne_lin = data.Split(',');

                    Instant stat = new Instant();
                    stat.Strenght = ushort.Parse(ne_lin[0]);
                    stat.Vitality = ushort.Parse(ne_lin[1]);
                    stat.Agility = ushort.Parse(ne_lin[2]);
                    stat.Spirit = ushort.Parse(ne_lin[3]);

                    if (ContainsKey(65))
                    {
                        this[65].Add(level, stat);
                    }
                    else
                    {
                        Dictionary<ushort, AtributesStatus.Instant> sta = new Dictionary<ushort, Instant>();
                        sta.Add(level, stat);
                        Add(65, sta);
                    }
                }
                else if (line.StartsWith("Archer"))
                {
                    string[] lin = line.Split(']');
                    byte level = byte.Parse(lin[0].Remove(0, 7));

                    string data = lin[1].Remove(0, 1);
                    string[] ne_lin = data.Split(',');

                    Instant stat = new Instant();
                    stat.Strenght = ushort.Parse(ne_lin[0]);
                    stat.Vitality = ushort.Parse(ne_lin[1]);
                    stat.Agility = ushort.Parse(ne_lin[2]);
                    stat.Spirit = ushort.Parse(ne_lin[3]);

                    if (ContainsKey(45))
                    {
                        this[45].Add(level, stat);
                    }
                    else
                    {
                        Dictionary<ushort, AtributesStatus.Instant> sta = new Dictionary<ushort, Instant>();
                        sta.Add(level, stat);
                        Add(45, sta);
                    }
                }
                else if (line.StartsWith("Ninja"))
                {
                    string[] lin = line.Split(']');
                    byte level = byte.Parse(lin[0].Remove(0, 6));

                    string data = lin[1].Remove(0, 1);
                    string[] ne_lin = data.Split(',');

                    Instant stat = new Instant();
                    stat.Strenght = ushort.Parse(ne_lin[0]);
                    stat.Vitality = ushort.Parse(ne_lin[1]);
                    stat.Agility = ushort.Parse(ne_lin[2]);
                    stat.Spirit = ushort.Parse(ne_lin[3]);

                    if (ContainsKey(55))
                    {
                        this[55].Add(level, stat);
                    }
                    else
                    {
                        Dictionary<ushort, AtributesStatus.Instant> sta = new Dictionary<ushort, Instant>();
                        sta.Add(level, stat);
                        Add(55, sta);
                    }
                }
                else if (line.StartsWith("Taoist"))
                {
                    string[] lin = line.Split(']');
                    byte level = byte.Parse(lin[0].Remove(0, 7));

                    string data = lin[1].Remove(0, 1);
                    string[] ne_lin = data.Split(',');

                    Instant stat = new Instant();
                    stat.Strenght = ushort.Parse(ne_lin[0]);
                    stat.Vitality = ushort.Parse(ne_lin[1]);
                    stat.Agility = ushort.Parse(ne_lin[2]);
                    stat.Spirit = ushort.Parse(ne_lin[3]);

                    if (ContainsKey(100))
                    {
                        this[100].Add(level, stat);
                    }
                    else
                    {
                        Dictionary<ushort, AtributesStatus.Instant> sta = new Dictionary<ushort, Instant>();
                        sta.Add(level, stat);
                        Add(100, sta);
                    }
                }
                else if (line.StartsWith("Trojan"))
                {
                    string[] lin = line.Split(']');
                    byte level = byte.Parse(lin[0].Remove(0, 7));

                    string data = lin[1].Remove(0, 1);
                    string[] ne_lin = data.Split(',');

                    Instant stat = new Instant();
                    stat.Strenght = ushort.Parse(ne_lin[0]);
                    stat.Vitality = ushort.Parse(ne_lin[1]);
                    stat.Agility = ushort.Parse(ne_lin[2]);
                    stat.Spirit = ushort.Parse(ne_lin[3]);

                    if (ContainsKey(15))
                    {
                        this[15].Add(level, stat);
                    }
                    else
                    {
                        Dictionary<ushort, AtributesStatus.Instant> sta = new Dictionary<ushort, Instant>();
                        sta.Add(level, stat);
                        Add(15, sta);
                    }
                }
                else if (line.StartsWith("Warrior"))
                {
                    string[] lin = line.Split(']');
                    byte level = byte.Parse(lin[0].Remove(0, 8));

                    string data = lin[1].Remove(0, 1);
                    string[] ne_lin = data.Split(',');

                    Instant stat = new Instant();
                    stat.Strenght = ushort.Parse(ne_lin[0]);
                    stat.Vitality = ushort.Parse(ne_lin[1]);
                    stat.Agility = ushort.Parse(ne_lin[2]);
                    stat.Spirit = ushort.Parse(ne_lin[3]);

                    if (ContainsKey(25))
                    {
                        this[25].Add(level, stat);
                    }
                    else
                    {
                        Dictionary<ushort, AtributesStatus.Instant> sta = new Dictionary<ushort, Instant>();
                        sta.Add(level, stat);
                        Add(25, sta);
                    }
                }
                else if (line.StartsWith("Pirate"))
                {
                    string[] lin = line.Split(']');
                    byte level = byte.Parse(lin[0].Remove(0, 7));

                    string data = lin[1].Remove(0, 1);
                    string[] ne_lin = data.Split(',');

                    Instant stat = new Instant();
                    stat.Strenght = ushort.Parse(ne_lin[0]);
                    stat.Vitality = ushort.Parse(ne_lin[1]);
                    stat.Agility = ushort.Parse(ne_lin[2]);
                    stat.Spirit = ushort.Parse(ne_lin[3]);

                    if (ContainsKey(75))
                    {
                        this[75].Add(level, stat);
                    }
                    else
                    {
                        Dictionary<ushort, AtributesStatus.Instant> sta = new Dictionary<ushort, Instant>();
                        sta.Add(level, stat);
                        Add(75, sta);
                    }
                }
                else if (line.StartsWith("LongLee"))
                {
                    string[] lin = line.Split(']');
                    byte level = byte.Parse(lin[0].Remove(0, 8));

                    string data = lin[1].Remove(0, 1);
                    string[] ne_lin = data.Split(',');

                    Instant stat = new Instant();
                    stat.Strenght = ushort.Parse(ne_lin[0]);
                    stat.Vitality = ushort.Parse(ne_lin[1]);
                    stat.Agility = ushort.Parse(ne_lin[2]);
                    stat.Spirit = ushort.Parse(ne_lin[3]);

                    if (ContainsKey(85))
                    {
                        this[85].Add(level, stat);
                    }
                    else
                    {
                        Dictionary<ushort, AtributesStatus.Instant> sta = new Dictionary<ushort, Instant>();
                        sta.Add(level, stat);
                        Add(85, sta);
                    }
                }
                else if (line.StartsWith("WindWalker"))
                {
                    string[] lin = line.Split(']');
                    byte level = byte.Parse(lin[0].Remove(0, 11));

                    string data = lin[1].Remove(0, 1);
                    string[] ne_lin = data.Split(',');

                    Instant stat = new Instant();
                    stat.Strenght = ushort.Parse(ne_lin[0]);
                    stat.Vitality = ushort.Parse(ne_lin[1]);
                    stat.Agility = ushort.Parse(ne_lin[2]);
                    stat.Spirit = ushort.Parse(ne_lin[3]);

                    if (ContainsKey(165))
                    {
                        this[165].Add(level, stat);
                    }
                    else
                    {
                        Dictionary<ushort, AtributesStatus.Instant> sta = new Dictionary<ushort, Instant>();
                        sta.Add(level, stat);
                        Add(165, sta);
                    }
                }
            }
            MyConsole.WriteLine("Atributes Status was Loading! ");
        }
    }
}
