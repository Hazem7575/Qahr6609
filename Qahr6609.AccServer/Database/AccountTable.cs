// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.IO;
using System.Text;
using AccServer.Network;

namespace AccServer.Database
{
    public unsafe class AccountTable
    {
        public enum AccountState : byte
        {
            NotActivated = 100,
            ProjectManager = 255,
            GameHelper = 5,
            GameMaster = 3,
            Player = 2,
            Banned = 1,
            Cheat = 80,
            DoesntExist = 0
        }
        public string Username;
        public string Password;
        public string IP;
        public AccountState State;
        public uint EntityID;
        public uint Identifier;
        public bool exists = false;
        public bool Banned;
    
        public AccountTable(string username)
        {
            if (username == null) return;
            Username = username;
            Password = "";
            IP = "";
            State = AccountState.DoesntExist;
            EntityID = 0;
            Identifier = 0;
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("accounts").Where("Username", username))
            using (var reader = new MySqlReader(cmd))
            {
                if (reader.Read())
                {
                    exists = true;
                    Password = reader.ReadString("Password");
                    IP = reader.ReadString("Ip");
                    EntityID = reader.ReadUInt32("EntityID");
                    State = (AccountState)reader.ReadInt32("State");
                    if (State == (AccountState)1)
                    {
                        Banned = true;
                    }
                }
            }
        }
      
        public void Save(string Mac)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("accounts").Set("EntityID", EntityID).Set("EarthID", Mac)
                    .Where("Username", Username).Execute();
        }
        public void UpdatePCInfo(string Mac, string HDSerial)
        {

            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("accounts").Set("EarthID", Mac)
                    .Where("EntityID", EntityID).Execute();
        }
        public uint ReadIdentifier()
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("accounts").Where("Username", Username))
              using (var reader = new MySqlReader(cmd))
              {
                  if (reader.Read())
                  {
                      return reader.ReadUInt32("Key");
                  }
              }
            return 0;
        }

        internal void UpdatePCInfo(object macAddress, string v)
        {
            throw new NotImplementedException();
        }
    }
}