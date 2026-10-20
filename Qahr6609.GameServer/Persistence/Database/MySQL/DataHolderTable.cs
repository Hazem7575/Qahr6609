
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using MYSQLCONNECTION = MySqlConnector.MySqlConnection; // [net10] .NET 10 build
using MYSQLBUILDER = MySqlConnector.MySqlConnectionStringBuilder;

namespace Project2D.Database
{
    
    public unsafe static class DataHolder
    {
        public static string ConnectionString;
        
        public static void CreateConnection(string MySqlDatabase, string MySqlPassword)
        {
            CreateConnection(MySqlDatabase, MySqlPassword, "localhost", "root", 3306);
        }

        public static void CreateConnection(string MySqlDatabase, string MySqlPassword, string host, string user, uint port)
        {
            var b = new MYSQLBUILDER();
            b.Server = string.IsNullOrEmpty(host) ? "localhost" : host;
            b.Port = port == 0 ? 3306 : port;
            b.Database = MySqlDatabase;
            b.UserID = string.IsNullOrEmpty(user) ? "root" : user;
            b.Password = MySqlPassword;
            b.Pooling = true;
            b.MaximumPoolSize = 300;
            b.MinimumPoolSize = 5;
            // full Unicode (Arabic names, symbols); MariaDB 10.6+ calls plain utf8 "utf8mb3".
            b.CharacterSet = "utf8mb4";
            // Fail fast when MySQL is down or slow instead of blocking the calling thread (login queue, packet
            // handlers) for the driver defaults of 15 s connect / 30 s per command.
            // shell.ini [Performance] MySqlConnectTimeoutSec (default 3), MySqlCommandTimeoutSec (default 5).
            // Bulk SELECTs through MySqlReader (admin panel) keep a longer limit: MySqlReaderTimeoutSec (default 30).
            b.ConnectionTimeout = (uint)Math.Max(1, Conquer6609.Framework.Settings.Int("Performance", "MySqlConnectTimeoutSec", 3));
            b.DefaultCommandTimeout = (uint)Math.Max(1, Conquer6609.Framework.Settings.Int("Performance", "MySqlCommandTimeoutSec", 5));
            ConnectionString = b.ConnectionString;
        }

        /// <summary>Command timeout (seconds) for the bulk SELECTs run through MySqlReader.</summary>
        public static int ReaderTimeoutSec
        {
            get { return Math.Max(1, Conquer6609.Framework.Settings.Int("Performance", "MySqlReaderTimeoutSec", 30)); }
        }

        public static MYSQLCONNECTION MySqlConnection
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