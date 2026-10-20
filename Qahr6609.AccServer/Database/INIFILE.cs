using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AccServer.Database
{
    public unsafe class IniFile
    {
        public string FileName;
        public IniFile(string _FileName)
        {
            FileName = _FileName;
        }
        public IniFile()
        {
            FileName = null;
        }

        public const int
            Int32_Size = 15,
            Int16_Size = 9,
            Int8_Size = 6,
            Bool_Size = 6,
            Double_Size = 20,
            Int64_Size = 22,
            Float_Size = 10;

        public static Func<string, int> ToInt32 = new Func<string, int>(int.Parse);
        public static Func<string, uint> ToUInt32 = new Func<string, uint>(uint.Parse);
        public static Func<string, short> ToInt16 = new Func<string, short>(short.Parse);
        public static Func<string, ushort> ToUInt16 = new Func<string, ushort>(ushort.Parse);
        public static Func<string, sbyte> ToInt8 = new Func<string, sbyte>(sbyte.Parse);
        public static Func<string, byte> ToUInt8 = new Func<string, byte>(byte.Parse);
        public static Func<string, bool> ToBool = new Func<string, bool>(bool.Parse);
        public static Func<string, double> ToDouble = new Func<string, double>(double.Parse);
        public static Func<string, long> ToInt64 = new Func<string, long>(long.Parse);
        public static Func<string, ulong> ToUInt64 = new Func<string, ulong>(ulong.Parse);
        public static Func<string, float> ToFloat = new Func<string, float>(float.Parse);

        public string ReadString(string Section, string Key, string Default, int Size)
        {

            char* lpBuffer = stackalloc char[Size];
            Kernel32.GetPrivateProfileStringW(Section, Key, Default, lpBuffer, Size, FileName);
            return new string(lpBuffer).Trim('\0');
        }
        public string ReadString(string Section, string Key, string Default)
        {
            return ReadString(Section, Key, Default, 255);
        }
        public string ReadBigString(string Section, string Key, string Default)
        {
            return ReadString(Section, Key, Default, 255 * 4);
        }
        public void ReadString(string Section, string Key, void* Default, void* Buffer, int Size)
        {
            Kernel32.GetPrivateProfileStringA(Section, Key, Default, (sbyte*)Buffer, Size, FileName);
        }

        public T ReadValue<T>(string Section, string Key, T Default, Func<string, T> callback)
        {
            try
            {
                return callback.Invoke(ReadString(Section, Key, Default.ToString()));
            }
            catch
            {
                return Default;
            }
        }
        public T ReadValue<T>(string Section, string Key, T Default, Func<string, T> callback, int BufferSize)
        {
            try
            {
                return callback.Invoke(ReadString(Section, Key, Default.ToString(), BufferSize));
            }
            catch
            {
                return Default;
            }
        }

        public int ReadInt32(string Section, string Key, int Default)
        {
            return ReadValue<int>(Section, Key, Default, ToInt32, Int32_Size);
        }
        public ulong ReadUInt64(string Section, string Key, ulong Default)
        {
            return ReadValue<ulong>(Section, Key, Default, ToUInt64, Int64_Size);
        }
        public long ReadInt64(string Section, string Key, long Default)
        {
            return ReadValue<long>(Section, Key, Default, ToInt64, Int64_Size);
        }
        public double ReadDouble(string Section, string Key, double Default)
        {
            return ReadValue<double>(Section, Key, Default, ToDouble, Double_Size);
        }
        public uint ReadUInt32(string Section, string Key, uint Default)
        {
            return ReadValue<uint>(Section, Key, Default, ToUInt32, Int32_Size);
        }
        public short ReadInt16(string Section, string Key, short Default)
        {
            return ReadValue<short>(Section, Key, Default, ToInt16, Int16_Size);
        }
        public ushort ReadUInt16(string Section, string Key, ushort Default)
        {
            return ReadValue<ushort>(Section, Key, Default, ToUInt16, Int16_Size);
        }
        public sbyte ReadSByte(string Section, string Key, sbyte Default)
        {
            return ReadValue<sbyte>(Section, Key, Default, ToInt8, Int8_Size);
        }
        public byte ReadByte(string Section, string Key, byte Default)
        {
            return ReadValue<byte>(Section, Key, Default, ToUInt8, Int8_Size);
        }
        public bool ReadBool(string Section, string Key, bool Default)
        {
            return ReadValue<bool>(Section, Key, Default, ToBool, Bool_Size);
        }
        public float ReadFloat(string Section, string Key, float Default)
        {
            return ReadValue<float>(Section, Key, Default, ToFloat, Float_Size);
        }

        public void WriteString(string Section, string Key, string Value)
        {
            if (Value == null)
            {
                Value = "0";
            }
            Kernel32.WritePrivateProfileString(Section, Key, Value, FileName);
        }
        public void Write<T>(string Section, string Key, T Value)
        {
            Kernel32.WritePrivateProfileString(Section, Key, Value.ToString(), FileName);
        }
        public void WriteStruct(string Section, string Key, void* lpStruct, int Size)
        {
            Kernel32.WritePrivateProfileStructW(Section, Key, lpStruct, Size, FileName);
        }

        public string[] GetSectionNames(int BufferSize)
        {
            char* lpBuffer = stackalloc char[BufferSize];
            int Size = Kernel32.GetPrivateProfileSectionNamesW(lpBuffer, BufferSize, FileName);
            if (Size == 0)
                return new string[0];
            return new string(lpBuffer, 0, Size - 1).Split('\0');
        }
        public string[] GetSection(string Section, int BufferSize)
        {
            char* lpBuffer = stackalloc char[BufferSize];
            int Size = Kernel32.GetPrivateProfileSectionW(Section, lpBuffer, BufferSize, FileName);
            if (Size == 0)
                return new string[0];
            return new string(lpBuffer, 0, Size - 1).Split('\0');
        }
        public string[] GetSectionNames()
        {
            return GetSectionNames(4096);
        }
        public string[] GetSection(string Section)
        {
            return GetSection(Section, 4096);
        }
        public bool SectionExists(string Section)
        {
            char* temp = stackalloc char[Section.Length + 1];
            int r = Kernel32.GetPrivateProfileSectionW(Section, temp, Section.Length + 1, FileName);
            return (temp[0] != 0);
        }
        public bool KeyExists(string Section, string Key)
        {
            const char not_used = (char)0x007F;
            const string not_used_s = "\007F";

            char* temp = stackalloc char[2];
            uint r = Kernel32.GetPrivateProfileStringW(Section, Key, not_used_s, temp, 2, FileName);
            return (r == 0 && temp[0] == 0) || (r > 0 && temp[0] != not_used);
        }


    }
}
