// Client side of Qahr6609.AccServer\Network\Cryptography\AuthCryptography.cs.
// The server's Encrypt and Decrypt both apply  b = K ^ swapNibbles(b ^ 0xAB)  (K = key1[lo] ^ key2[hi] of a
// per-direction byte counter), so the client needs the inverse of that:  b = swapNibbles(b ^ K) ^ 0xAB.
namespace TestBot.Crypto
{
    public class ClientAuthCipher
    {
        static readonly byte[] key1 = new byte[256], key2 = new byte[256];
        static ClientAuthCipher()
        {
            byte k1 = 0x9D, k2 = 0x62;
            for (int i = 0; i < 256; i++)
            {
                key1[i] = k1; key2[i] = k2;
                k1 = (byte)((0x0F + (byte)(k1 * 0xFA)) * k1 + 0x13);
                k2 = (byte)((0x79 - (byte)(k2 * 0x5C)) * k2 + 0x6D);
            }
        }
        ushort encCounter, decCounter;

        static byte Inverse(byte b, ushort counter)
        {
            b ^= (byte)(key1[counter & 0xFF] ^ key2[counter >> 8]);
            b = (byte)(b >> 4 | b << 4);
            return (byte)(b ^ 0xAB);
        }
        /// <summary>client -> server (undone by the server's Decrypt)</summary>
        public void Encrypt(byte[] buffer) { for (int i = 0; i < buffer.Length; i++) buffer[i] = Inverse(buffer[i], encCounter++); }
        /// <summary>server -> client (undoes the server's Encrypt)</summary>
        public void Decrypt(byte[] buffer, int length) { for (int i = 0; i < length; i++) buffer[i] = Inverse(buffer[i], decCounter++); }
    }
}
