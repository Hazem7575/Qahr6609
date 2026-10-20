// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AccServer.Database
{
    using MYSQLCOMMAND = MySql.Data.MySqlClient.MySqlCommand;
    using MYSQLREADER = MySql.Data.MySqlClient.MySqlDataReader;
    using MYSQLCONNECTION = MySql.Data.MySqlClient.MySqlConnection;

    public unsafe static class DataHolder
    {
        private static string ConnectionString;
        public static void CreateConnection(string database, string password)
        {
            CreateConnection(database, password, "localhost", "root", 3306);
        }

        public static void CreateConnection(string database, string password, string host, string user, uint port)
        {
            var b = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
            b.Server = string.IsNullOrEmpty(host) ? "localhost" : host;
            b.Port = port == 0 ? 3306 : port;
            b.Database = database;
            b.UserID = string.IsNullOrEmpty(user) ? "root" : user;
            b.Password = password;
            b.PersistSecurityInfo = true;
            b.Pooling = true;
            b.MinimumPoolSize = 32;
            b.MaximumPoolSize = 300;
            // MariaDB 10.6+ reports "utf8mb3"; MySql.Data 6.x only understands utf8mb4.
            b.CharacterSet = "utf8mb4";
            // Fail fast when MySQL is down or slow (driver defaults: 15 s connect, 30 s per command).
            // Every account-server query is a single-row lookup/update, so 5 s per command is plenty.
            b.ConnectionTimeout = 3;
            b.DefaultCommandTimeout = 5;
            ConnectionString = b.ConnectionString;
        }
        public static MYSQLCONNECTION   MySqlConnection
        {
            get
            {
                MYSQLCONNECTION conn = new MYSQLCONNECTION();
                conn.ConnectionString = ConnectionString;
                return conn;
            }
        }
    }
}