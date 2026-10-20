using System;

namespace AccServer.Network.Cryptography
{
    public class AuthCryptography
    {
        class CryptCounter
        {
            public CryptCounter()
            {

            }
            public CryptCounter(ushort with)
            {
                m_Counter = with;
            }
            UInt16 m_Counter = 0;

            public byte Key2
            {
                get { return (byte)(m_Counter >> 8); }
            }

            public byte Key1
            {
                get { return (byte)(m_Counter & 0xFF); }
            }

            public void Increment()
            {
                m_Counter++;
            }
        }

        private CryptCounter _decryptCounter;
        private CryptCounter _encryptCounter;
        private static byte[] _cryptKey1;
        private static byte[] _cryptKey2;
        private static byte[] _cryptKey3;
        private static byte[] _cryptKey4;
        private static bool Decrypt2 = false;

        public static void PrepareAuthCryptography()
        {
            if (_cryptKey1 != null)
            {
                if (_cryptKey1.Length != 0)
                    return;
            }
            _cryptKey1 = new byte[0x100];
            _cryptKey2 = new byte[0x100];
            byte i_key1 = 0x9D;
            byte i_key2 = 0x62;
            for (int i = 0; i < 0x100; i++)
            {
                _cryptKey1[i] = i_key1;
                _cryptKey2[i] = i_key2;
                i_key1 = (byte)((0x0F + (byte)(i_key1 * 0xFA)) * i_key1 + 0x13);
                i_key2 = (byte)((0x79 - (byte)(i_key2 * 0x5C)) * i_key2 + 0x6D);
            }
        }
        public AuthCryptography()
        {
            _encryptCounter = new CryptCounter();
            _decryptCounter = new CryptCounter();
        }
        public void Encrypt(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= (byte)0xAB;
                buffer[i] = (byte)(buffer[i] >> 4 | buffer[i] << 4);
                buffer[i] ^= (byte)(_cryptKey1[_encryptCounter.Key1] ^ _cryptKey2[_encryptCounter.Key2]);
                _encryptCounter.Increment();
            }
        }

        public void Decrypt(byte[] buffer, int length)
        {
            if (!Decrypt2)
            {
                for (int i = 0; i < length; i++)
                {
                    buffer[i] ^= (byte)0xAB;
                    buffer[i] = (byte)(buffer[i] >> 4 | buffer[i] << 4);
                    buffer[i] ^= (byte)(_cryptKey2[_decryptCounter.Key2] ^ _cryptKey1[_decryptCounter.Key1]);
                    _decryptCounter.Increment();
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    buffer[i] ^= (byte)0xAB;
                    buffer[i] = (byte)(buffer[i] >> 4 | buffer[i] << 4);
                    buffer[i] ^= (byte)(_cryptKey4[_decryptCounter.Key2] ^ _cryptKey3[_decryptCounter.Key1]);
                    _decryptCounter.Increment();
                }
            }
        }

        public static void GenerateKeys(UInt32 CryptoKey, UInt32 AccountID)
        {
            UInt32 tmpkey1 = 0, tmpkey2 = 0;
            tmpkey1 = ((CryptoKey + AccountID) ^ (0x4321)) ^ CryptoKey;
            tmpkey2 = tmpkey1 * tmpkey1;
            _cryptKey3 = new byte[256];
            _cryptKey4 = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                int right = ((3 - (i % 4)) * 8);
                int left = ((i % 4)) * 8 + right;
                _cryptKey3[i] = (byte)(_cryptKey1[i] ^ tmpkey1 << right >> left);
                _cryptKey4[i] = (byte)(_cryptKey2[i] ^ tmpkey2 << right >> left);
            }
            Decrypt2 = true;
        }
        public static void GenerateKeys2(byte[] InKey1, byte[] InKey2)
        {
            byte[] addKey1 = new byte[4];
            byte[] addKey2 = new byte[4];
            byte[] addResult = new byte[4];
            //addKey1.i = 0;
            //addKey2.i = 0;
            byte[] tempKey = new byte[4];

            long LMULer;
            //			InKey1[0] = 0x20;
            //			InKey1[1] = 0x5c;
            //			InKey1[2] = 0x48;
            //			InKey1[3] = 0xf4;
            //			InKey2[0] = 0x00;
            //			InKey2[1] = 0x44;
            //			InKey2[2] = 0xa6;
            //			InKey2[3] = 0x2e;

            //if (Key3) delete [] Key3;
            //if (Key4) delete [] Key4;
            _cryptKey3 = new byte[256];
            _cryptKey4 = new byte[256];
            for (int x = 0; x < 4; x++)
            {
                addKey1[x] = InKey1[3 - x];
                addKey2[x] = InKey2[3 - x];
            }
            //cout << "Key1: " << addKey1.i << endl;
            //cout << "Key2: " << addKey2.i << endl;
            uint Adder1;
            uint Adder2;
            uint Adder3;
            Adder1 = (uint)((addKey1[3] << 24) | (addKey1[2] << 16) | (addKey1[1] << 8) | (addKey1[0]));
            Adder2 = (uint)((addKey2[3] << 24) | (addKey2[2] << 16) | (addKey2[1] << 8) | (addKey2[0]));
            Adder3 = Adder1 + Adder2;
            addResult[0] = (byte)(Adder3 & 0xff);
            addResult[1] = (byte)((Adder3 >> 8) & 0xff);
            addResult[2] = (byte)((Adder3 >> 16) & 0xff);
            addResult[3] = (byte)((Adder3 >> 24) & 0xff);
            for (int b = 3; b >= 0; b--)
            {
                //	printf("%.2x ", addResult.c[b]);
                tempKey[3 - b] = addResult[b];
            }
            tempKey[2] = (byte)(tempKey[2] ^ (byte)0x43);
            tempKey[3] = (byte)(tempKey[3] ^ (byte)0x21);

            for (int b = 0; b < 4; b++)
            {
                tempKey[b] = (byte)(tempKey[b] ^ InKey1[b]);
            }

            //Build the 3rd Key
            for (int b = 0; b < 256; b++)
            {
                _cryptKey3[b] = (byte)(tempKey[3 - (b % 4)] ^ _cryptKey1[b]);
            }


            for (int x = 0; x < 4; x++)
            {
                addResult[x] = tempKey[3 - x];
            }
            Adder3 = (uint)((addResult[3] << 24) | (addResult[2] << 16) | (addResult[1] << 8) | (addResult[0]));
            LMULer = Adder3 * Adder3;
            LMULer = LMULer << 32;
            LMULer = LMULer >> 32;

            Adder3 = Convert.ToUInt32(LMULer & 0xffffffff);

            addResult[0] = (byte)(Adder3 & 0xff);
            addResult[1] = (byte)((Adder3 >> 8) & 0xff);
            addResult[2] = (byte)((Adder3 >> 16) & 0xff);
            addResult[3] = (byte)((Adder3 >> 24) & 0xff);

            for (int b = 3; b >= 0; b--)
            {
                tempKey[3 - b] = addResult[b];
            }
            //Build the 4th Key
            for (int b = 0; b < 256; b++)
            {
                _cryptKey4[b] = Convert.ToByte(tempKey[3 - (b % 4)] ^ _cryptKey2[b]);
            }
            Decrypt2 = true;
            //cout << "Int representation: " << charadd.i << endl;
        }
    }
}
