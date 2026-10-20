



using System;
using System.Text;
using System.Collections.Generic;

namespace Project2D.Database
{
    using MYSQLCOMMAND = MySqlConnector.MySqlCommand; // [net10] .NET 10 build
    using MYSQLREADER = MySqlConnector.MySqlDataReader;
    using MYSQLCONNECTION = MySqlConnector.MySqlConnection;

    public unsafe class MySqlCommand : IDisposable
    {
        private MySqlCommandType _type;
        public MySqlCommandType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public StringBuilder _command;
        public string Command
        {
            get { return _command.ToString(); }
            set { _command = new StringBuilder(value); }
        }
        public bool firstPart = true;
        public Dictionary<byte, string> insertFields;
        public Dictionary<byte, string> insertValues;
        public byte lastpair;
        public MySqlCommand(MySqlCommandType Type)
        {
            this.Type = Type;
            switch (Type)
            {
                case MySqlCommandType.SELECT:
                    {
                        _command = new StringBuilder("SELECT * FROM <R>");
                        break;
                    }
                case MySqlCommandType.UPDATE:
                    {
                        _command = new StringBuilder("UPDATE <R> SET ");
                        break;
                    }
                case MySqlCommandType.INSERT:
                    {
                        insertFields = new Dictionary<byte, string>();
                        insertValues = new Dictionary<byte, string>();
                        lastpair = 0;
                        _command = new StringBuilder("INSERT INTO <R> (<F>) VALUES (<V>)");
                        break;
                    }
                case MySqlCommandType.DELETE:
                    {
                        _command = new StringBuilder("DELETE FROM <R> WHERE <C> = <V>");
                        break;
                    }
                case MySqlCommandType.DELETE2:
                    {
                        _command = new StringBuilder("DELETE FROM <R> WHERE <C> = <V> AND <C2> = <V2>");
                        break;
                    }
                case MySqlCommandType.COUNT:
                    {
                        _command = new StringBuilder("SELECT count(<V>) FROM <R>");
                        break;
                    }
            }
        }
        private bool Comma()
        {
            if (firstPart)
            {
                firstPart = false;
                return false;
            }
            string command = _command.ToString();
            if (command[command.Length - 1] == ',' || command[command.Length - 2] == ',' || command[command.Length - 3] == ',')
                return false;
            return true;
        }
        #region Select
        public MySqlCommand Select(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        #endregion
        #region Count
        public MySqlCommand Count(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        #endregion
        #region Delete
        public MySqlCommand C(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
                return this;
            }
        public MySqlCommand Delete2Conedition(string table, string column, uint value, string column2, uint value2)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", "'" + value.ToString() + "'");
            _command = _command.Replace("<C2>", "`" + column2 + "`");
            _command = _command.Replace("<V2>", "'" + value2.ToString() + "'");
            return this;
        }
        public MySqlCommand Delete(string table, string column, string value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", "'" + value.MySqlEscape() + "'");
            return this;
        }
        public MySqlCommand Delete(string table, string column, long value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", value.ToString());
            return this;
        }
        public MySqlCommand Delete(string table, string column, ulong value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", value.ToString());
            return this;
        }
        public MySqlCommand Delete(string table, string column, bool value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", (value ? "1" : "0"));
            return this;
        }
        #endregion
        #region Update
        public MySqlCommand Update(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        public MySqlCommand Set(string column, long value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command.Append(",`" + column + "` = " + value.ToString() + " ");
                else
                    _command = _command.Append("`" + column + "` = " + value.ToString() + " ");
            }
            return this;
        }
        public MySqlCommand Set(string column, ulong value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command.Append(",`" + column + "` = " + value.ToString() + " ");
                else
                    _command = _command.Append("`" + column + "` = " + value.ToString() + " ");
            }
            return this;
        }
        public MySqlCommand Set(string column, string value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command.Append(",`" + column + "` = '" + value.MySqlEscape() + "' ");
                else
                    _command = _command.Append("`" + column + "` = '" + value.MySqlEscape() + "' ");
            }
            return this;
        }
        public MySqlCommand Set(string column, bool value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command.Append(",`" + column + "` = " + (value ? "1" : "0") + " ");
                else
                    _command = _command.Append("`" + column + "` = " + (value ? "1" : "0") + " ");
            }
            return this;
        }
        public MySqlCommand Set(string column, object value)
        {
            if (value is bool) Set(column, (bool)value);
            else Set(column, value.ToString());
            return this;
        }
        #endregion
        #region Insert
        public MySqlCommand Insert(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        public MySqlCommand Insert(string field, long value)
        {
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, value.ToString());
            lastpair++;
            return this;
        }
        public MySqlCommand Insert(string field, ulong value)
        {
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, value.ToString());
            lastpair++;
            return this;
        }
        public MySqlCommand Insert(string field, bool value)
        {
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, (value ? 1 : 0).ToString());
            lastpair++;
            return this;
        }
        public MySqlCommand Insert(string field, string value)
        {
            var array = value.ToCharArray();
            string str = global::Conquer6609.AnsiText.Value.GetString(Encoding.Unicode.GetBytes(array, 0, array.Length));
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, value.MySqlEscape());
            lastpair++;
            return this;
        }
        #endregion
        #region Where
        public MySqlCommand Where(string column, long value)
        {
            _command = _command.Append("WHERE `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand Where(string column, long value, bool greater)
        {
            if (greater)
                _command = _command.Append("WHERE `" + column + "` > " + value);
            else
                _command = _command.Append("WHERE `" + column + "` < " + value);
            return this;
        }
        public MySqlCommand Where(string column, ulong value)
        {
            _command = _command.Append("WHERE `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand Where(string column, string value)
        {
            _command = _command.Append("WHERE `" + column + "` = '" + value.MySqlEscape() + "'");
            return this;
        }
        public MySqlCommand Where(string column, bool value)
        {
            _command = _command.Append("WHERE `" + column + "` = " + (value ? "1" : "0"));
            return this;
        }
        #endregion
        #region And
        public MySqlCommand And(string column, long value)
        {
            _command = _command.Append(" AND `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand And(string column, ulong value)
        {
            _command = _command.Append(" AND `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand And(string column, string value)
        {
            _command = _command.Append(" AND `" + column + "` = '" + value.MySqlEscape() + "'");
            return this;
        }
        public MySqlCommand And(string column, bool value)
        {
            _command = _command.Append(" AND `" + column + "` = " + (value ? "1" : "0"));
            return this;
        }
        #endregion
        #region Or
        public MySqlCommand Or(string column, long value)
        {
            _command = _command.Append(" Or `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand Or(string column, ulong value)
        {
            _command = _command.Append(" Or `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand Or(string column, string value)
        {
            _command = _command.Append(" Or `" + column + "` = '" + value.MySqlEscape() + "'");
            return this;
        }
        public MySqlCommand Or(string column, bool value)
        {
            _command = _command.Append(" Or `" + column + "` = " + (value ? "1" : "0"));
            return this;
        }
        #endregion
        #region Order
        public MySqlCommand Order(string column)
        {
            _command = _command.Append("ORDER BY " + column + "");
            return this;
        }
        #endregion
        public int Execute()
        {
            if (Type == MySqlCommandType.INSERT)
            {
                string fields = "";
                string values = "";
                byte x;
                for (x = 0; x < lastpair; x++)
                {
                    bool comma = (x + 1) == lastpair ? false : true;
                    #region Fields
                    if (comma)
                        fields += "`" + insertFields[x] + "`,";
                    else
                        fields += "`" + insertFields[x] + "`";
                    #endregion
                    #region Values
                    if (comma)
                        values += "'" + insertValues[x] + "'" + ",";
                    else
                        values += "'" + insertValues[x] + "'";
                    #endregion
                }
                _command = _command.Replace("<F>", fields);
                _command = _command.Replace("<V>", values);
            }

            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                MYSQLCOMMAND cmd = new MYSQLCOMMAND(Command + ";", conn);
                return cmd.ExecuteNonQuery();
            }
        }
        public int Execute2()
        {
            if (Type == MySqlCommandType.INSERT)
            {
                string fields = "";
                string values = "";
                byte x;
                for (x = 0; x < lastpair; x++)
                {
                    bool comma = (x + 1) == lastpair ? false : true;
                    #region Fields
                    if (comma)
                        fields += "`" + insertFields[x] + "`,";
                    else
                        fields += "`" + insertFields[x] + "`";
                    #endregion
                    #region Values
                    if (comma)
                        values += "'" + insertValues[x] + "'" + ",";
                    else
                        values += "'" + insertValues[x] + "'";
                    #endregion
                }
                _command = _command.Replace("<F>", fields);
                _command = _command.Replace("<V>", values);
            }

            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                MYSQLCOMMAND cmd = new MYSQLCOMMAND(Command + ";", conn);
                return cmd.ExecuteNonQuery();
            }
        }
        public MySqlReader CreateReader()
        {
            return new MySqlReader(this);
        }
        void IDisposable.Dispose()
        {
            if (insertValues != null)
            {
                insertValues.Clear();
                insertFields.Clear();
            }
            _command = null;
        }
    }
    public enum MySqlCommandType
    {
        DELETE, DELETE2, INSERT, SELECT, UPDATE, COUNT, C
    }
}