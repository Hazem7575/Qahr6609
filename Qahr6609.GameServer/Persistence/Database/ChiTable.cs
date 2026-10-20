using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class ChiTable
    {
        /// <summary>Written by ServerDatabase.SaveClient. Files without it were saved by builds that stored the
        /// Turtle retreat powers under "RetreatedTiger" and the Tiger ones under "RetreatedTurtle".</summary>
        public const string RetreatKeysMarker = "RetreatKeysV2";
        public static Tuple<Conquer6609.Role.Instance.Chi.ChiAttributeType, int>[] ReadPowers(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            Tuple<Conquer6609.Role.Instance.Chi.ChiAttributeType, int>[] Powers = new Tuple<Role.Instance.Chi.ChiAttributeType, int>[4];
            string[] strs = str.Split('|');
            if (strs.Length != 4)
                return null;
            int x = 0;
            try
            {
                foreach (var stri in strs)
                {
                    string[] ss = stri.Split('-');
                    Powers[x] = Tuple.Create((Conquer6609.Role.Instance.Chi.ChiAttributeType)int.Parse(ss[0]), int.Parse(ss[1]));
                    x++;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return Powers;
        }
        public static string PowersToString(Tuple<Conquer6609.Role.Instance.Chi.ChiAttributeType, int>[] Powers)
        {
            if (Powers == null)
                return "";
            string str = (int)Powers[0].Item1 + "-" + Powers[0].Item2 + "|" + (int)Powers[1].Item1 + "-" + Powers[1].Item2 + "|" + (int)Powers[2].Item1 + "-" + Powers[2].Item2 + "|" + (int)Powers[3].Item1 + "-" + Powers[3].Item2;
            return str;
        }
        public static void Load()
        {
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                ini.FileName = fname;

                uint UID = ini.ReadUInt32("Character", "UID", 0);
                Role.Instance.Chi playerchi = new Role.Instance.Chi(UID);
                string Name = ini.ReadString("Character", "Name", "None");
                playerchi.Name = Name;
                playerchi.ChiPoints = ini.ReadInt32("Character", "ChiPoints", 0);
                playerchi.Dragon.Load(ini.ReadString("Character", "Dragon", ""), UID, Name);
                playerchi.Phoenix.Load(ini.ReadString("Character", "Pheonix", ""), UID, Name);
                playerchi.Turtle.Load(ini.ReadString("Character", "Turtle", ""), UID, Name);
                playerchi.Tiger.Load(ini.ReadString("Character", "Tiger", ""), UID, Name);
                playerchi.DragonTime = ini.ReadInt64("Character", "DragonTime", 0);
                if (playerchi.DragonTime != 0)
                {
                    playerchi.DragonPowers = null;
                    playerchi.DragonPowers = ReadPowers(ini.ReadString("Character", "RetreatedDragon", ""));
                }
                playerchi.PhoenixTime = ini.ReadInt64("Character", "PhoenixTime", 0);
                if (playerchi.PhoenixTime != 0)
                {
                    playerchi.PhoenixPowers = null;
                    playerchi.PhoenixPowers = ReadPowers(ini.ReadString("Character", "RetreatedPhoenix", ""));
                }
                bool retreatKeysV2 = ini.ReadByte("Character", RetreatKeysMarker, 0) == 1;
                string turtleKey = retreatKeysV2 ? "RetreatedTurtle" : "RetreatedTiger";
                string tigerKey = retreatKeysV2 ? "RetreatedTiger" : "RetreatedTurtle";
                playerchi.TurtleTime = ini.ReadInt64("Character", "TurtleTime", 0);
                if (playerchi.TurtleTime != 0)
                {
                    playerchi.TurtlePowers = null;
                    playerchi.TurtlePowers = ReadPowers(ini.ReadString("Character", turtleKey, ""));
                }
                playerchi.TigerTime = ini.ReadInt64("Character", "TigerTime", 0);
                if (playerchi.TigerTime != 0)
                {
                    playerchi.TigerPowers = null;
                    playerchi.TigerPowers = ReadPowers(ini.ReadString("Character", tigerKey, ""));
                }
                if (playerchi.Dragon.UnLocked)
                {
                    Role.Instance.Chi.ChiPool.TryAdd(playerchi.UID, playerchi);
                    Program.ChiRanking.Upadte(Program.ChiRanking.Dragon, playerchi.Dragon);
                }
                if (playerchi.Phoenix.UnLocked)
                    Program.ChiRanking.Upadte(Program.ChiRanking.Phoenix, playerchi.Phoenix);
                if (playerchi.Tiger.UnLocked)
                    Program.ChiRanking.Upadte(Program.ChiRanking.Tiger, playerchi.Tiger);
                if (playerchi.Turtle.UnLocked)
                    Program.ChiRanking.Upadte(Program.ChiRanking.Turtle, playerchi.Turtle);
            }

        }
    }
}
