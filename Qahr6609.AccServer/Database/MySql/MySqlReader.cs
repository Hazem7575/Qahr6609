// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Data;
using System.Text;
using MySql.Data.Types;
using System.Data.OleDb;
using MySql.Data.MySqlClient;

namespace AccServer.Database
{
    public class MySqlReader : IDisposable
    {
        private DataSet _dataset;
        private DataRow _datarow;
        private int _row;
        private const string Table = "table";

        public MySqlReader(MySqlCommand command)
        {
            if (command.Type == MySqlCommandType.SELECT)
            {
                _dataset = new DataSet();
                _row = 0;
                using (MySql.Data.MySqlClient.MySqlConnection conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var DataAdapter = new MySqlDataAdapter(command.Command, conn))
                        DataAdapter.Fill(_dataset, Table);
                    ((IDisposable) command).Dispose();
                }
            }
        }
        public MySqlReader(MySqlCommand command, string Conn)
        {
            MySql.Data.MySqlClient.MySqlConnection xx = new MySql.Data.MySqlClient.MySqlConnection();
            xx.ConnectionString = Conn;
            if (command.Type == MySqlCommandType.SELECT)
            {
                _dataset = new DataSet();
                _row = 0;
                using (MySql.Data.MySqlClient.MySqlConnection conn = xx)
                {
                    conn.Open();
                    using (var DataAdapter = new MySqlDataAdapter(command.Command, conn))
                        DataAdapter.Fill(_dataset, Table);
                    ((IDisposable)command).Dispose();
                }
            }
        }
        void IDisposable.Dispose()
        {
            if (_dataset != null)
                _dataset.Dispose();
        }

        public bool Read()
        {
            if (_dataset == null) return false;
            if (_dataset.Tables.Count == 0) return false;
            if (_dataset.Tables[Table].Rows.Count > _row)
            {
                _datarow = _dataset.Tables[Table].Rows[_row];
                _row++;
                return true;
            }
            _row++;
            return false;
        }

        public void Dispose()
        {
        }

        public void Close()
        {
        }

        public int NumberOfRows
        {
            get
            {
                if (_dataset == null) return 0;
                if (_dataset.Tables.Count == 0) return 0;
                return _dataset.Tables[Table].Rows.Count;
            }
        }

        public sbyte ReadSByte(string columnName)
        {
            if (_datarow.IsNull(columnName)) return default(sbyte);
            sbyte result = 0;
            sbyte.TryParse(_datarow[columnName].ToString(), out result);
            return result;
        }

        public byte ReadByte(string columnName)
        {
            if (_datarow.IsNull(columnName)) return default(byte);
            byte result = 0;
            byte.TryParse(_datarow[columnName].ToString(), out result);
            return result;
        }

        public short ReadInt16(string columnName)
        {
            if (_datarow.IsNull(columnName)) return default(short);
            short result = 0;
            short.TryParse(_datarow[columnName].ToString(), out result);
            return result;
        }

        public ushort ReadUInt16(string columnName)
        {
            if (_datarow.IsNull(columnName)) return default(ushort);
            ushort result = 0;
            ushort.TryParse(_datarow[columnName].ToString(), out result);
            return result;
        }

        public int ReadInt32(string columnName)
        {
            if (_datarow.IsNull(columnName)) return default(int);
            int result = 0;
            int.TryParse(_datarow[columnName].ToString(), out result);
            return result;
        }

        public uint ReadUInt32(string columnName)
        {
            if (_datarow.IsNull(columnName)) return default(uint);
            uint result = 0;
            uint.TryParse(_datarow[columnName].ToString(), out result);
            return result;
        }

        public long ReadInt64(string columnName)
        {
            if (_datarow.IsNull(columnName)) return default(long);
            long result = 0;
            long.TryParse(_datarow[columnName].ToString(), out result);
            return result;
        }

        public ulong ReadUInt64(string columnName)
        {
            if (_datarow.IsNull(columnName)) return default(ulong);
            ulong result = 0;
            ulong.TryParse(_datarow[columnName].ToString(), out result);
            return result;
        }

        public ulong ReadUInt128(string columnName)
        {
            if (_datarow.IsNull(columnName)) return default(ulong);
            ulong result = 0;
            ulong.TryParse(_datarow[columnName].ToString(), out result);
            return result;
        }

        public string ReadString(string columnName)
        {
            if (_datarow.IsNull(columnName)) return "";
            string data = _datarow[columnName].ToString();
            return data;
        }

        public ushort method_4(string columnName)
        {
            ushort num = 0;
            ushort.TryParse(this._datarow[columnName].ToString(), out num);
            ushort num1 = num;
            return num1;
        }

        public uint method_6(string columnName)
        {
            uint num = 0;
            uint.TryParse(this._datarow[columnName].ToString(), out num);
            uint num1 = num;
            return num1;
        }

        public long method_7(string columnName)
        {
            long num = (long) 0;
            long.TryParse(this._datarow[columnName].ToString(), out num);
            long num1 = num;
            return num1;
        }

        public bool ReadBoolean(string columnName)
        {
            if (_datarow.IsNull(columnName)) return false;
            bool result = false;
            string str = _datarow[columnName].ToString();
            if (str == "") return false;
            if (str[0] == '1') return true;
            if (str[0] == '0') return false;
            bool.TryParse(str, out result);
            return result;
        }

        public byte[] ReadBlob(string columnName)
        {
            if (_datarow.IsNull(columnName)) return new byte[0];
            return (byte[]) _datarow[columnName];
        }

      
    }
}